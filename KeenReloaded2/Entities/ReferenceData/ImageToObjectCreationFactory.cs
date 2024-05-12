using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.ReferenceDataClasses;

namespace KeenReloaded2.Entities.ReferenceData
{
    public static class ImageToObjectCreationFactory
    {
        private static Dictionary<string, MapMakerObject> _imageObjectMapping = new Dictionary<string, MapMakerObject>()
        {
            {
               nameof(Properties.Resources.keen5_background_omegamatic_blue1),
               new MapMakerObject(
                 typeof(Background),
                 Path.Combine(GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_BACKGROUNDS, "Keen5", Biomes.BIOME_KEEN5_BLACK), nameof(Properties.Resources.keen5_background_omegamatic_blue1) + ".png"),
                 true,
                 new List<MapMakerObjectProperty>()
                 {
                     new MapMakerObjectProperty()
                     {
                         PropertyName = "area",
                         DisplayName = "Area: ",
                         DataType = typeof(Rectangle),
                         Value = new Rectangle(0, 0, Properties.Resources.keen5_background_omegamatic_blue1.Width, Properties.Resources.keen5_background_omegamatic_blue1.Height)
                     },
                     new MapMakerObjectProperty()
                     {
                         PropertyName = "imagePath",
                         DisplayName = "Image: ",
                         DataType = typeof(string),     
                         Value = nameof(Properties.Resources.keen5_background_omegamatic_blue1) + ".png",
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
                 }.ToArray()
               )
            }
        };

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

        public static MapMakerObject GetMapMakerObjectFromImageName(string imageFile)
        {
            if (_imageObjectMapping.TryGetValue(imageFile, out MapMakerObject item) && item != null)
            {
                MapMakerObjectProperty[] clonedProperties = item.CloneParameterList();
                MapMakerObject copy = new MapMakerObject(item.ObjectType, item.ImageControl.ImageLocation, item.IsManualPlacement, clonedProperties);
                return copy;
            }
            return null;
        }
    }
}
