﻿using KeenReloaded2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KeenReloaded2.Constants;
using System.Diagnostics;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System.Drawing;

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
                if (string.IsNullOrWhiteSpace(mapFile))
                    throw new ArgumentException("Invalid map name");

                MapMakerData mapMakerData = new MapMakerData()
                {
                    MapName = FileIOUtility.ExtractFileNameFromPath(mapFile)
                };
                List<GameObjectMapping> mapData = new List<GameObjectMapping>();
                
                string path = mapFile;
                string[] mapDataLines = FileIOUtility.LoadMapData(path);
                Size mapSize = ParseMapSizeData(mapDataLines[0]);
                mapMakerData.MapSize = mapSize;
                var dataLines = mapDataLines.Skip(1);
                foreach (string line in dataLines)
                {
                    GameObjectMapping mapping = BuildMapDataObjectFromRawString(line);
                    mapData.Add(mapping);
                }
                mapMakerData.MapData = mapData;
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

        private static GameObjectMapping BuildMapDataObjectFromRawString(string data)
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
            var parameters = obj.CloneParameterList();

            var nonAreaParameters = parameters.Where(p => p.DataType != typeof(Rectangle)).ToList();
            //area parameter needs to be present
            MapMakerObjectProperty areaProperty = parameters.FirstOrDefault(p => p.DataType == typeof(Rectangle));
            if (areaProperty == null)
                throw new ArgumentException("all objects must contain an area property");

            areaProperty.Value = area;

            for (int i = 5; i < properties.Length; i++)
            {
                string rawValue = properties[i];
                MapMakerObjectProperty associatedProperty = nonAreaParameters[i - 5];//there should be only one area parameter
                if (associatedProperty.DataType == typeof(int))
                {
                    int value = Convert.ToInt32(rawValue);
                    associatedProperty.Value = value;
                }
                else if (associatedProperty.DataType == typeof(string[]))
                {
                    rawValue = rawValue.Replace(MapMakerConstants.MAP_MAKER_ARRAY_START, "")
                        .Replace(MapMakerConstants.MAP_MAKER_ARRAY_END, "");
                    string[] values = rawValue.Split(MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR[0]);
                    associatedProperty.Value = values;
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
            }

            GameObjectMapping mapping = new GameObjectMapping()
            {
                MapMakerObject = new MapMakerObject(obj.ObjectType, obj.ImageControl.ImageLocation, obj.IsManualPlacement, parameters)
            };

            mapping.GameObject = (ISprite)mapping.MapMakerObject.Construct();
            mapping.Location = mapping.GameObject.Location;
            mapping.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            mapping.Image = mapping.GameObject.Image;

            return mapping;
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

        public static class Validation
        {
            public static bool ValidateObjectPlacement(Rectangle objectArea, Rectangle mapArea)
            {
                bool isValid = objectArea.Top >= mapArea.Top && objectArea.Bottom <= mapArea.Bottom
                     && objectArea.Right <= mapArea.Right && objectArea.Left >= mapArea.Left;

                return isValid;
            }
        }
    }
}