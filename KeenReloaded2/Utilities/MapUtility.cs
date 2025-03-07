﻿using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace KeenReloaded2.Utilities
{
    public static class MapUtility
    {
        public static bool SaveMap(string mapName, string gameMode, Size mapSize, List<GameObjectMapping> mapData)
        {
            try
            {
                string gameModeFolder = GetFolderFromGameMode(gameMode);
                if (mapData == null)
                    throw new ArgumentNullException("map data is null");

                if (string.IsNullOrEmpty(gameModeFolder))
                    throw new ArgumentException("Game mode not recognized");

                string path = Path.Combine(System.Environment.CurrentDirectory, MapMakerConstants.SAVED_MAPS_FOLDER, gameModeFolder, mapName + ".txt");
                StringBuilder builder = new StringBuilder();
                string mapSizeLine = $"{mapSize.Width}{MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR}{mapSize.Height}";
                builder.AppendLine(mapSizeLine);
                mapData = mapData.OrderBy(o => o.GameObject is IActivator).ToList();
                foreach (GameObjectMapping mapDataLine in mapData)
                {
                    string line = mapDataLine.GameObject.ToString();
                    builder.AppendLine(line);
                }
                string mapDataStr = builder.ToString();
                bool savedMapSuccessfully = FileIOUtility.SaveMap(path, mapDataStr);
                return savedMapSuccessfully;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public static MapMakerData LoadMapData(string mapFile)
        {
            try
            {
                List<IActivateable> activateables = new List<IActivateable>();
                Dictionary<int, Door> doorNetwork = new Dictionary<int, Door>();
                Dictionary<int, EnemyTransporter> transporterNetwork = new Dictionary<int, EnemyTransporter>();
                if (string.IsNullOrWhiteSpace(mapFile))
                    throw new ArgumentException("Invalid map name");

                MapMakerData mapMakerData = new MapMakerData()
                {
                    MapName = FileIOUtility.ExtractFileNameFromPath(mapFile),
                    MapPath = mapFile,
                    GameMode = GetGameModeFromFolderPath(mapFile)
                };
                List<GameObjectMapping> mapData = new List<GameObjectMapping>();

                string path = mapFile;
                string[] mapDataLines = FileIOUtility.LoadMapData(path);
                Size mapSize = ParseMapSizeData(mapDataLines[0]);
                mapMakerData.MapSize = mapSize;
                SpaceHashGrid grid = new SpaceHashGrid(mapSize.Width, mapSize.Height);
                var dataLines = mapDataLines.Skip(1);
                foreach (string line in dataLines)
                {
                    GameObjectMapping mapping = BuildMapDataObjectFromRawString(line, grid, activateables, doorNetwork, transporterNetwork);
                    mapData.Add(mapping);
                }
                mapMakerData.MapData = mapData;

                var doors = mapData.Select(m => m.GameObject).OfType<Door>();
                if (doors.Any())
                {
                    foreach (var door in doors)
                    {
                        if (door.DestinationDoorId.HasValue &&
                            doorNetwork.TryGetValue(door.DestinationDoorId.Value, out Door destinationDoor))
                        {
                            door.DestinationDoor = destinationDoor;
                        }
                    }
                }
                var transporters = mapData.Select(m => m.GameObject).OfType<EnemyTransporter>();
                if (transporters.Any())
                {
                    foreach (var transporter in transporters)
                    {
                        if (transporter.DestinationNodeId.HasValue &&
                            transporterNetwork.TryGetValue(transporter.DestinationNodeId.Value, out EnemyTransporter destinationTransporter))
                        {
                            transporter.DestinationNode = destinationTransporter;
                        }
                    }
                }

                return mapMakerData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new MapMakerData();
            }
        }

        public static string GetSavedMapsPath(string gameMode)
        {
            string gameModeFolder = GetFolderFromGameMode(gameMode);

            if (string.IsNullOrEmpty(gameModeFolder))
                throw new ArgumentException("Game mode not recognized");

            string path = Path.Combine(System.Environment.CurrentDirectory, MapMakerConstants.SAVED_MAPS_FOLDER, gameModeFolder);
            return path;
        }

        private static Size ParseMapSizeData(string data)
        {
            string[] rawData = data.Split(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR[0]);
            int width = Convert.ToInt32(rawData[0]);
            int height = Convert.ToInt32(rawData[1]);
            Size size = new Size(width, height);
            return size;
        }

        private static GameObjectMapping BuildMapDataObjectFromRawString(string data, SpaceHashGrid grid, List<IActivateable> activateables, Dictionary<int, Door> doorNetwork, Dictionary<int, EnemyTransporter> transporterNetwork)
        {
            string[] properties = data.Split(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR[0]);
            if (properties.Length < 5)
                throw new ArgumentException("All objects must contain 5 parameters. One for type name, and 1 for x, y, width, and height");

            string typeName = properties[0];
            //area needs to be in this format
            int x = Convert.ToInt32(properties[1]);
            int y = Convert.ToInt32(properties[2]);
            int width = Convert.ToInt32(properties[3]);
            int height = Convert.ToInt32(properties[4]);

            Rectangle area = new Rectangle(x, y, width, height);

            MapMakerObject obj = ImageToObjectCreationFactory.GetMapMakerObjectFromImageName(typeName);
            var parameters = obj.ConstructorParameters;

            var explicitNonStandardParameters = parameters.Where(p => !p.IsIgnoredInMapData && p.PropertyName != GeneralGameConstants.AREA_PROPERTY_NAME).ToList();
            //area parameter needs to be present
            MapMakerObjectProperty areaProperty = parameters.FirstOrDefault(p => p.PropertyName == GeneralGameConstants.AREA_PROPERTY_NAME);
            if (areaProperty == null)
                throw new ArgumentException("all objects must contain an area property");

            //set space hash grid property
            MapMakerObjectProperty spaceHashGridProperty = parameters.FirstOrDefault(p => p.PropertyName == GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME);
            if (spaceHashGridProperty != null)
            {
                spaceHashGridProperty.Value = grid;
            }

            areaProperty.Value = area;

            for (int i = 5; i < properties.Length; i++)
            {
                string rawValue = properties[i];
                MapMakerObjectProperty associatedProperty = explicitNonStandardParameters[i - 5];//there should be only one area parameter
                if (associatedProperty.DataType == typeof(int))
                {
                    int value = Convert.ToInt32(rawValue);
                    associatedProperty.Value = value;
                }
                else if (associatedProperty.DataType.IsArray)
                {
                    rawValue = rawValue.Replace(MapMakerConstants.MAP_MAKER_ARRAY_START, "")
                        .Replace(MapMakerConstants.MAP_MAKER_ARRAY_END, "");
                    string[] values = rawValue.Split(MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR[0]);
                    associatedProperty.Value = values;
                    if (associatedProperty.DataType == typeof(IActivateable[]))
                    {
                        List<IActivateable> assignedActivateables =
                            activateables.Where(a => values.Contains(a.ActivationID.ToString())).ToList();

                        associatedProperty.Value = assignedActivateables.ToArray();
                    }
                }
                else if (associatedProperty.DataType == typeof(bool))
                {
                    bool value = Convert.ToBoolean(rawValue);
                    associatedProperty.Value = value;
                }
                else if (associatedProperty.DataType == typeof(string))
                {
                    associatedProperty.Value = rawValue;
                }
                else if (associatedProperty.DataType == typeof(Guid))
                {
                    Guid value = Guid.Parse(rawValue);
                    associatedProperty.Value = value;
                }
                else if (associatedProperty.DataType.IsEnum)
                {
                    var value = Enum.Parse(associatedProperty.DataType, rawValue);
                    associatedProperty.Value = value;
                }
                else if (associatedProperty.DataType == typeof(int?)
                    && int.TryParse(rawValue, out int result))
                {
                    associatedProperty.Value = result;
                }
                else if (associatedProperty.DataType == typeof(Rectangle))
                {
                    int areaX = Convert.ToInt32(properties[i++]);
                    int areaY = Convert.ToInt32(properties[i++]);
                    int areaWidth = Convert.ToInt32(properties[i++]);
                    int areaHeight = Convert.ToInt32(properties[i]);
                    Rectangle areaPropVal = new Rectangle(areaX, areaY, areaWidth, areaHeight);
                    associatedProperty.Value = areaPropVal;
                }
                else if (associatedProperty.DataType == typeof(List<Point>))
                {
                    associatedProperty.Value = GetPointsFromArrayString(rawValue);
                }
            }

            GameObjectMapping mapping = new GameObjectMapping()
            {
                MapMakerObject = new MapMakerObject(obj.ObjectType, obj.ImageControl.ImageLocation, obj.IsManualPlacement, parameters)
            };

            mapping.GameObject = (ISprite)mapping.MapMakerObject.Construct();
            mapping.Location = mapping.GameObject.Location;
            mapping.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            mapping.Image = mapping.GameObject.Image;

            if (mapping.GameObject is IActivateable)
            {
                activateables.Add((IActivateable)mapping.GameObject);
            }

            if (mapping.GameObject is Door)
            {
                var door = (Door)mapping.GameObject;
                if (!doorNetwork.ContainsKey(door.Id))
                {
                    doorNetwork.Add(door.Id, door);
                }
            }
            else if (mapping.GameObject is EnemyTransporter)
            {
                var transporter = (EnemyTransporter)mapping.GameObject;
                if (!transporterNetwork.ContainsKey(transporter.Id))
                {
                    transporterNetwork.Add(transporter.Id, transporter);
                }
            }

            return mapping;
        }

        private static List<Point> GetPointsFromArrayString(string rawValue)
        {
            List<Point> points = new List<Point>();
            if (string.IsNullOrEmpty(rawValue))
                return points;


            string[] values = rawValue.Replace(MapMakerConstants.MAP_MAKER_ARRAY_START, "")
                .Replace(MapMakerConstants.MAP_MAKER_ARRAY_END, "")
                .Split(MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR.ToCharArray()[0]);

            if (!values.Any() || values.Length % 2 != 0)
                return points;

            int index = 0;
            while (index < values.Length)
            {
                string part1 = values[index];
                string part2 = values[++index];
                int x = Convert.ToInt32(part1);
                int y = Convert.ToInt32(part2);
                Point p = new Point(x, y);
                points.Add(p);
                index++;
            }

            return points;
        }

        private static string GetFolderFromGameMode(string gameMode)
        {
            switch (gameMode)
            {
                case MainMenuConstants.OPTION_LABEL_NORMAL_MODE:
                    return MapMakerConstants.NORMAL_MAPS_FOLDER;
                case MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE:
                    return MapMakerConstants.ZOMBIE_MAPS_FOLDER;
                case MainMenuConstants.OPTION_LABEL_KOTH_MODE:
                    return MapMakerConstants.KING_OF_THE_HILL_FOLDER;
                case MainMenuConstants.OPTION_LABEL_CTF_MODE:
                    return MapMakerConstants.CAPTURE_THE_FLAG_MAPS_FOLDER;
            }
            return string.Empty;
        }

        private static string GetGameModeFromFolderPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (path.Contains(MapMakerConstants.NORMAL_MAPS_FOLDER))
                return MainMenuConstants.OPTION_LABEL_NORMAL_MODE;
            if (path.Contains(MapMakerConstants.ZOMBIE_MAPS_FOLDER))
                return MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE;
            if (path.Contains(MapMakerConstants.KING_OF_THE_HILL_FOLDER))
                return MainMenuConstants.OPTION_LABEL_KOTH_MODE;
            if (path.Contains(MapMakerConstants.CAPTURE_THE_FLAG_MAPS_FOLDER))
                return MainMenuConstants.OPTION_LABEL_CTF_MODE;

            return null;
        }

        public static class Validation
        {
            public static bool ValidateObjectPlacement(Rectangle objectArea, Rectangle mapArea)
            {
               
                bool isValid = objectArea.Top >= mapArea.Top && objectArea.Top <= mapArea.Bottom
                     && objectArea.Right <= mapArea.Right && objectArea.Left >= mapArea.Left;

                return isValid;
            }

            public static bool ValidateAllMapObjects(MapMakerData mapData, out List<string> errorMessages)
            {
                errorMessages = new List<string>();
                try
                {
                    bool areAllObjectsValid = true;
                    foreach (var obj in mapData.MapData)
                    {
                        if (obj.GameObject is MapEdgeTile)
                            continue;

                        if (obj.GameObject.Location.X < 0 || obj.GameObject.Location.X + (obj.GameObject.Image?.Width ?? 0) > mapData.MapSize.Width
                            || obj.GameObject.Location.Y < 0 || obj.GameObject.Location.Y > mapData.MapSize.Height)
                        {
                            areAllObjectsValid = false;
                            break;
                        }
                    }
                    var gameObjects = mapData.MapData.Select(g => g.GameObject);
                    if (!gameObjects.Any(d => d is CommanderKeen))
                    {
                        string errorMessage = "Player object is not present";
                        errorMessages.Add(errorMessage);
                        return false;
                    }

                    var doors = gameObjects.OfType<Door>().Where(d => !(d is ExitDoor));
                    if (doors.Any())
                    {
                        bool duplicateDoors = doors.GroupBy(d => d.Id).Any(d => d.Count() > 1);
                        if (duplicateDoors)
                        {
                            string doorDupeMessage = "Some doors have duplicate numbers. Make sure all door numbers are unique.";
                            errorMessages.Add(doorDupeMessage);
                            areAllObjectsValid = false;
                        }
                    }

                    if (!areAllObjectsValid)
                    {
                        string errorMessage = "Some map objects are outside the bounds of the map.  Please correct before saving.";
                        errorMessages.Add(errorMessage);
                        return false;
                    }

                    return true;
                }
                catch (NullReferenceException)
                {
                    string errorMessage = "Map data not given";
                    errorMessages.Add(errorMessage);
                    return false;
                }
            }

            public static bool ValidateMap(MapMakerData mapData, out List<string> errorMessages)
            {
                errorMessages = new List<string>();
                bool isValid = true;
                if (mapData == null)
                {
                    string errorMessage = "Map data not given";
                    errorMessages.Add(errorMessage);
                    return false;
                }

                string mapName = mapData.MapName;
                if (string.IsNullOrWhiteSpace(mapName))
                {
                    string errorMessage = "Invalid map name";
                    errorMessages.Add(errorMessage);
                    isValid = false;
                }

                if (mapData.MapSize.IsEmpty || mapData.MapSize.Width == 0 || mapData.MapSize.Height == 0)
                {
                    string errorMessage = "Invalid map dimensions";
                    errorMessages.Add(errorMessage);
                    isValid = false;
                }

                List<string> errMsgs = new List<string>();
                isValid = isValid && ValidateAllMapObjects(mapData, out errMsgs);

                if (errMsgs != null && errMsgs.Any())
                    errorMessages.AddRange(errMsgs);

                return isValid;
            }
        }
    }
}
