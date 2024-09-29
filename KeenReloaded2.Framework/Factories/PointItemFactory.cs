using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.Factories
{
    public static class PointItemFactory
    {
        private static Dictionary<PointItemType, ImageData> _itemTypeImageDict = new Dictionary<PointItemType, ImageData>()
        {
            { PointItemType.KEEN4_DOUGHNUT, new ImageData() { ImageName = nameof(Properties.Resources.keen4_doughnut1), Image = Properties.Resources.keen4_doughnut1 } },
            { PointItemType.KEEN4_ICECREAM_CONE, new ImageData() { ImageName = nameof(Properties.Resources.keen4_icecream_cone1), Image = Properties.Resources.keen4_icecream_cone1 } },
            { PointItemType.KEEN4_JAWBREAKER, new ImageData() { ImageName = nameof(Properties.Resources.keen4_jawbreaker1), Image = Properties.Resources.keen4_jawbreaker1 } },
            { PointItemType.KEEN4_SHIKADI_SODA, new ImageData() { ImageName = nameof(Properties.Resources.keen4_shikadi_soda1), Image = Properties.Resources.keen4_shikadi_soda1 } },
            { PointItemType.KEEN4_SHIKKERS_CANDY_BAR, new ImageData() { ImageName = nameof(Properties.Resources.keen4_candy_bar1), Image = Properties.Resources.keen4_candy_bar1 } },
            { PointItemType.KEEN4_THREE_TOOTH_GUM, new ImageData() { ImageName = nameof(Properties.Resources.keen4_shikadi_soda1), Image = Properties.Resources.keen4_shikadi_soda1 } },
            { PointItemType.KEEN5_BAG_O_SUGAR, new ImageData() { ImageName = nameof(Properties.Resources.keen5_bag_o_sugar1), Image = Properties.Resources.keen5_bag_o_sugar1 } },
            { PointItemType.KEEN5_CHOCOLATE_MILK, new ImageData() { ImageName = nameof(Properties.Resources.keen5_chocolate_milk1), Image = Properties.Resources.keen5_chocolate_milk1 } },
            { PointItemType.KEEN5_MARSHMALLOW, new ImageData() { ImageName = nameof(Properties.Resources.keen5_marshmallow1), Image = Properties.Resources.keen5_marshmallow1 } },
            { PointItemType.KEEN5_SHIKADI_GUM, new ImageData() { ImageName = nameof(Properties.Resources.keen5_shikadi_gum1), Image = Properties.Resources.keen5_shikadi_gum1 } },
            { PointItemType.KEEN5_SUGAR_STOOPIES_CEREAL, new ImageData() { ImageName = nameof(Properties.Resources.keen5_sugar_stoopies_cereal1), Image = Properties.Resources.keen5_sugar_stoopies_cereal1 } },
            { PointItemType.KEEN5_TART_STIX, new ImageData() { ImageName = nameof(Properties.Resources.keen5_tart_stix1), Image = Properties.Resources.keen5_tart_stix1 } },
            { PointItemType.KEEN6_BANANA_SPLIT, new ImageData() { ImageName = nameof(Properties.Resources.keen6_banana_split1), Image = Properties.Resources.keen6_banana_split1 } },
            { PointItemType.KEEN6_BLOOG_SODA, new ImageData() { ImageName = nameof(Properties.Resources.keen6_bloog_soda1), Image = Properties.Resources.keen6_bloog_soda1 } },
            { PointItemType.KEEN6_ICE_CREAM_BAR, new ImageData() { ImageName = nameof(Properties.Resources.keen6_ice_cream_bar1), Image = Properties.Resources.keen6_ice_cream_bar1 } },
            { PointItemType.KEEN6_PIZZA_SLICE, new ImageData() { ImageName = nameof(Properties.Resources.keen6_pizza_slice1), Image = Properties.Resources.keen6_pizza_slice1 } },
            { PointItemType.KEEN6_PUDDING, new ImageData() { ImageName = nameof(Properties.Resources.keen6_pudding1), Image = Properties.Resources.keen6_pudding1 } },
            { PointItemType.KEEN6_ROOT_BEER_FLOAT, new ImageData() { ImageName = nameof(Properties.Resources.keen6_root_beer_float1), Image = Properties.Resources.keen6_root_beer_float1 } },
        };
        public static PointItem GeneratePointItemFromType(IZombieBountyEnemy enemy)
        {
            try
            {
                ISprite enemySprite = enemy as ISprite;
                if (enemySprite != null)
                {
                    ImageData imageData = _itemTypeImageDict[enemy.PointItem];
                    int imageWidth = imageData.Image.Width;
                    int imageHeight = imageData.Image.Height;
                    string imageName = imageData.ImageName;
                    int centerX = enemySprite.Location.X + (enemySprite.Image.Width / 2) - (imageWidth / 2);
                    int centerY = enemySprite.Location.Y + (enemySprite.Image.Height / 2) - (imageHeight / 2);

                    Rectangle area = new Rectangle(centerX, centerY, imageWidth, imageHeight);
                    if (enemy is CollisionObject)
                    {
                        var obj = (CollisionObject)enemy;
                        PointItem item = new PointItem(area, obj.CollisionGrid, imageName, enemySprite.ZIndex, enemy.PointItem);
                        return item;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }
    }

    struct ImageData
    {
        public string ImageName { get; set; }

        public Image Image { get; set; }
    }
}
