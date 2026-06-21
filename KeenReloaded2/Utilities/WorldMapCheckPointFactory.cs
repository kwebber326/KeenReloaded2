using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Utilities
{
    public static class WorldMapCheckPointFactory
    {
        public static MapMakerObject GetCheckPointFromImage(string path, string imgName, Image img)
        {
            MapMakerObject retVal = null;
            Rectangle area = new Rectangle(0, 0, img.Width, img.Height);
            Type type = null;

            List<MapMakerObjectProperty> commonProperties = 
                new List<MapMakerObjectProperty>()
                {
                      new MapMakerObjectProperty()
                         {
                             DisplayName = "Display Area:",
                             PropertyName  = GeneralGameConstants.AREA_PROPERTY_NAME,
                             DataType = typeof(Rectangle),
                             Value = area
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                             DataType = typeof(SpaceHashGrid),
                             Value = null,
                             IsIgnoredInMapData = true,
                             Hidden = true
                         },
                         new MapMakerObjectProperty()
                         {
                             DisplayName = "ZIndex:",
                             DataType = typeof(int),
                             Value = 10,
                             PropertyName = "zIndex"
                         },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "toggleObjects",
                            DataType = typeof(IActivateable[]),
                            Value = new IActivateable[] { },
                            DisplayName = "Activation Objects: "
                        },
                };

            switch (imgName)
            {
                case nameof(Properties.Resources.keen6_flag_hand_opened) + "_wm":
                    type = typeof(Keen6WorldMapCheckPoint);
                    break;
                case nameof(Properties.Resources.keen5_flag_base1) + "_wm":
                    type = typeof(Keen5WorldMapCheckPoint);
                    break;
                case nameof(Properties.Resources.keen4_flag_base_red1) + "_wm":
                    type = typeof(Keen4RedWorldMapCheckPoint);
                    break;
                case nameof(Properties.Resources.keen4_flag_base_yellow) + "_wm":
                    type = typeof(Keen4YellowWorldMapCheckPoint);
                    break;
            }

            retVal = new MapMakerObject(type, path, false, commonProperties.ToArray());

            return retVal;
        }
    }
}
