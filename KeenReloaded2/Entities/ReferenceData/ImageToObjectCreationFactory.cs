using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Backgrounds;

namespace KeenReloaded2.Entities.ReferenceData
{
    public static class ImageToObjectCreationFactory
    {
        private static Dictionary<string, List<MapMakerObjectProperty>> _mapMakerObjectTypeMapping = new Dictionary<string, List<MapMakerObjectProperty>>()
        {
            {
                nameof(Background),
                new List<MapMakerObjectProperty>()
                {
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "area",
                        DisplayName = "Area",
                        DataType = typeof(Rectangle)
                    },
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "imagePath",
                        DisplayName = "Image Path",
                        IsSpriteProperty = true,
                        DataType = typeof(string)
                    },
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "stretchImage",
                        DisplayName = "Stretch Image",
                        DataType = typeof(bool)
                    }
                }
            }
        };

        public static MapMakerObject GetMapMakerObjectFromType(string typeName, string imageFile, bool manuallyPlaced)
        {
            if (_mapMakerObjectTypeMapping.TryGetValue(typeName, out List<MapMakerObjectProperty> properties) && properties != null)
            {
                foreach (var property in properties)
                {
                    if (property.IsSpriteProperty)
                    {
                        property.Value = imageFile;
                    }
                }
                MapMakerObject obj = new MapMakerObject(typeName, imageFile, manuallyPlaced, properties.ToArray());
                return obj;
            }
            return null;
        }
    }
}
