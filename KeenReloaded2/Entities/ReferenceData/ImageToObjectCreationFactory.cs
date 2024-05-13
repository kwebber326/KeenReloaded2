using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;

namespace KeenReloaded2.Entities.ReferenceData
{
    public static class ImageToObjectCreationFactory
    {
        private static Dictionary<string, MapMakerObject> GetBackgroundObjectData()
        {
            Dictionary<string, MapMakerObject> backgroundReferenceData = new Dictionary<string, MapMakerObject>();

            for (int i = 4; i <= 6; i++)
            {
                string path = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_BACKGROUNDS, $"keen{i}", Biomes.BIOME_KEEN5_BLACK);

                string[] imageFiles = Directory.GetFiles(path);
                foreach (var file in imageFiles)
                {
                    try
                    {
                        Image img = Image.FromFile(file);
                        string imageName = FileIOUtility.ExtractFileNameFromPath(file);
                        Type type = typeof(Background);
                        string imagePath = file;

                        var parameters = new List<MapMakerObjectProperty>()
                    {
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                             DisplayName = "Area: ",
                             DataType = typeof(Rectangle),
                             Value = new Rectangle(0, 0, img.Width, img.Height)
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "imagePath",
                             DisplayName = "Image: ",
                             DataType = typeof(string),
                             Value = imageName + ".png",
                             IsSpriteProperty = true,
                             Readonly = true,
                             Hidden = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "stretchImage",
                             DisplayName = "Stretch Image: ",
                             DataType = typeof(bool),
                             Value = false
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "zIndex",
                             DisplayName = "Z Index: ",
                             DataType = typeof(int),
                             Value = 0
                         }
                    }.ToArray();

                        MapMakerObject obj = new MapMakerObject(type, imagePath, false, parameters);
                        backgroundReferenceData.Add(imageName, obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return new Dictionary<string, MapMakerObject>();
                    }
                }
            }
            return backgroundReferenceData;
        }
        #region reference data
        private static Dictionary<string, MapMakerObject> _imageObjectMapping = GetBackgroundObjectData();

        #endregion

        public static string GetImageDirectory(string categoryFolder, string episodeFolder, string biomeFolder)
        {
            string path = string.Empty;
            string mapMakerFolder = MapMakerConstants.MAP_MAKER_FOLDER;
            if (string.IsNullOrEmpty(categoryFolder)
             || string.IsNullOrEmpty(episodeFolder)
             || string.IsNullOrEmpty(biomeFolder))
                return path;



            if (categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_WEAPONS
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_PLAYER
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_GEMS)
            {
                path = Path.Combine(System.Environment.CurrentDirectory, mapMakerFolder, categoryFolder);
            }
            else if (categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_TILES)
            {
                path = Path.Combine(System.Environment.CurrentDirectory, mapMakerFolder, categoryFolder, episodeFolder, biomeFolder);
            }
            else
            {
                path = Path.Combine(System.Environment.CurrentDirectory, mapMakerFolder, categoryFolder, episodeFolder);
            }
            return path;
        }

        public static MapMakerObject GetMapMakerObjectFromImageName(string imageName)
        {
            if (_imageObjectMapping.TryGetValue(imageName, out MapMakerObject item) && item != null)
            {
                MapMakerObjectProperty[] clonedProperties = item.CloneParameterList();
                MapMakerObject copy = new MapMakerObject(item.ObjectType, item.ImageControl.ImageLocation, item.IsManualPlacement, clonedProperties);
                return copy;
            }
            return null;
        }
    }
}
