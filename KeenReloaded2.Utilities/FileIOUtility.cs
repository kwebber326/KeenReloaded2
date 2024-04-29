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
    }
}
