using KeenReloaded2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KeenReloaded2.Constants;
using System.Diagnostics;

namespace KeenReloaded2.Utilities
{
    public static class MapUtility
    {
        public static bool SaveMap(string mapName, string gameMode, List<GameObjectMapping> mapData)
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
    }
}
