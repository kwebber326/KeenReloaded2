using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using KeenReloaded2.Utilities;
using System;
using System.Diagnostics;

namespace KeenReloaded2.Entities
{
    public class WorldMapSaveState
    {
        public WorldMapObjectiveManager WorldObjectiveState { get; set; }

        public MapMakerData WorldMapData { get; set; }

        public MapMakerData LevelData { get; set; }

        public WorldMapPlayerInventoryState PlayerInventoryState { get; set; }

        public static bool Load(string worldMapFile, string levelFile, out WorldMapSaveState worldState)
        {
            worldState = null;
            try
            {
                string mapName = worldMapFile.Substring(worldMapFile.LastIndexOf('/') + 1);
                var objectiveState = MapUtility.LoadWorldMapObjectives(mapName);
                MapMakerData worldMap = MapUtility.LoadMapData(worldMapFile);
                MapMakerData level = null;
                if (!string.IsNullOrWhiteSpace(levelFile))
                {
                    level = MapUtility.LoadMapData(levelFile);
                }
                worldState = new WorldMapSaveState()
                {
                    WorldObjectiveState = objectiveState,
                    WorldMapData = worldMap,
                    LevelData = level,
                };
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                MapUtility.SaveWorldMapObjectives(this.WorldObjectiveState, this.WorldMapData.MapName);
                MapUtility.SaveMap(this.WorldMapData.MapName, MainMenuConstants.OPTION_LABEL_NORMAL_MODE,
                    this.WorldMapData.MapSize, this.WorldMapData.MapData, true);

                if (this.LevelData != null && this.LevelData.MapPath != null)
                {
                    MapUtility.SaveMap(this.LevelData.MapName,
                        MapMakerConstants.NORMAL_MAPS_FOLDER, this.LevelData.MapSize,
                        this.LevelData.MapData, true);
                }


                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
