using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace KeenReloaded2.Utilities
{
    public static class FileIOUtility
    {
        public const string SAVED_CHARACTER_FILE_NAME = "SavedCharacter.txt";
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

        public static string ExtractFileNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var imgName = path.Substring(path.LastIndexOf(@"\") + 1);
            imgName = imgName.Substring(0, imgName.LastIndexOf('.'));
            return imgName;
        }

        //public static string ExtractGameModeFromPath(string path)
        //{

        //}

        public static string GetResourcePathForMainProject()
        {
            string directory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            directory = Path.Combine(directory, "Resources");
            return directory;
        }
    }
}
