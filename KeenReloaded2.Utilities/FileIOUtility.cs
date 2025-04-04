﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using KeenReloaded2.Constants;

namespace KeenReloaded2.Utilities
{
    public static class FileIOUtility
    {
        public const string SAVED_CHARACTER_FILE_NAME = "SavedCharacter.txt";
        public const string AUDIO_SETTINGS_FILE_NAME = "AudioSettings.txt";
        public const string PARTIAL_ALGO_SETTINGS_FILE_NAME = "PartialAlgoSettings.txt";
        public const string HIGH_SCORE_FOLDER = "HighScores";
        public const string MUSIC_FOLDER = "Music";

        public static string LoadSavedCharacterSelection()
        {
            string characterName = null;
            try
            {
                string path = System.Environment.CurrentDirectory + "/" + SAVED_CHARACTER_FILE_NAME;
                if (!File.Exists(path))
                    File.Create(path);

                using (FileStream fs = File.OpenRead(path))
                using (StreamReader reader = new StreamReader(fs))
                {
                    characterName = reader.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return characterName;
        }

        public static bool SaveCharacterSelection(string characterName)
        {
            try
            {
                string path = System.Environment.CurrentDirectory + "/" + SAVED_CHARACTER_FILE_NAME;
                File.WriteAllText(path, characterName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public static bool SaveAudioSettings(AudioSettings settings)
        {
            try
            {
                if (settings == null)
                    return false;

                string path = System.Environment.CurrentDirectory + "/" + AUDIO_SETTINGS_FILE_NAME;

                if (File.Exists(path))
                    File.WriteAllText(path, string.Empty);

                using (FileStream fs = File.OpenWrite(path))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine($"Sounds|{settings.Sounds}");
                    writer.WriteLine($"Music|{settings.Music}");
                    writer.WriteLine($"Song|{settings.SelectedSong}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public static AudioSettings LoadAudioSettings()
        {
            AudioSettings settings = new AudioSettings();
            try
            {
                string path = System.Environment.CurrentDirectory + "/" + AUDIO_SETTINGS_FILE_NAME;
                if (!File.Exists(path))
                    File.Create(path);

                using (FileStream fs = File.OpenRead(path))
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line1 = reader.ReadLine();
                    string line2 = reader.ReadLine();
                    string line3 = reader.ReadLine();
                    string soundsStr = line1?.Split(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR[0])[1];
                    string musicStr = line2?.Split(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR[0])[1];
                    string songStr = line3?.Split(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR[0])[1];
                    settings.Sounds = bool.Parse(soundsStr);
                    settings.Music = bool.Parse(musicStr);
                    settings.SelectedSong = songStr;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return settings;
        }

        public static bool SavePartialAlgoSettings(bool usePartialAlgo)
        {
            try
            {
                string path = System.Environment.CurrentDirectory + "/" + PARTIAL_ALGO_SETTINGS_FILE_NAME;

                if (File.Exists(path))
                    File.WriteAllText(path, string.Empty);

                using (FileStream fs = File.OpenWrite(path))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(usePartialAlgo.ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public static bool LoadUsePartialAlgoSettings()
        {
            bool settings = false;
            try
            {
                string path = System.Environment.CurrentDirectory + "/" + PARTIAL_ALGO_SETTINGS_FILE_NAME;
                if (!File.Exists(path))
                    File.Create(path);

                using (FileStream fs = File.OpenRead(path))
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line = reader.ReadLine();
                    settings = Convert.ToBoolean(line);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return settings;
        }

        public static List<string> LoadWavFormatSongs()
        {
            try
            {
                List<string> songs = new List<string>();
                string path = Path.Combine(Environment.CurrentDirectory, MUSIC_FOLDER);
                string[] songFiles = Directory.GetFiles(path);
                songs = songFiles
                    .Where(s => s.ToLower().EndsWith(".wav"))
                    .Select(s1 => s1.Substring(s1.LastIndexOf('\\') + 1)).ToList();
                return songs;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<string>();
            }
        }

        public static bool SaveMap(string filePath, string fileData)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(fileData);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public static string[] LoadMapData(string filePath)
        {
            try
            {
                List<string> lines = new List<string>();
                using (FileStream fs = File.OpenRead(filePath))
                using (StreamReader reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        lines.Add(reader.ReadLine());
                    }
                }
                return lines.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        public static List<Tuple<string, string>> ReadHighScoresByGameModeAndLevel(string gameMode, string mapName)
        {
            string path = Path.Combine(Environment.CurrentDirectory, HIGH_SCORE_FOLDER, gameMode, mapName + ".txt");
            List<Tuple<string, string>> data = new List<Tuple<string, string>>();
            try
            {
                if (!File.Exists(path))
                {
                    using (FileStream fs1 = File.Create(path)) { };
                }

                using (FileStream fs = File.OpenRead(path))
                using (StreamReader reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        string rawData = reader.ReadLine();
                        int separatorIndex = rawData.LastIndexOf(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR);
                        if (separatorIndex != -1)
                        {
                            string playerName = rawData.Substring(0, separatorIndex);
                            string score = rawData.Substring(separatorIndex + 1);

                            Tuple<string, string> dataPoint = new Tuple<string, string>(playerName, score);
                            data.Add(dataPoint);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return data;
        }

        public static bool WriteHighScoresByGameMode(string gameMode, string mapName, List<Tuple<string, string>> highScores)
        {
            string path = Path.Combine(Environment.CurrentDirectory, HIGH_SCORE_FOLDER, gameMode, mapName + ".txt");
            try
            {
                if (!File.Exists(path))
                    File.Create(path);

                using (FileStream fs = File.OpenWrite(path))
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (var score in highScores)
                    {
                        string dataLine = string.Join(MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR, score.Item1, score.Item2);
                        writer.WriteLine(dataLine);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public static string ExtractFileNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var imgName = path.Substring(path.LastIndexOf(@"\") + 1);
            imgName = imgName.Substring(0, imgName.LastIndexOf('.'));
            return imgName;
        }



        public static string GetResourcePathForMainProject()
        {
            string directory = Directory.GetParent(System.Environment.CurrentDirectory).Parent.FullName;
           
            directory = Path.Combine(directory, "Resources");
            return directory;
        }
    }
}
