using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class PointItem : Item
    {
        private readonly PointItemType _type;

        public PointItem(Rectangle area, SpaceHashGrid grid, string imageName, int zIndex, PointItemType type) 
            : base(area, imageName, grid, zIndex)
        {
            _type = type;
            Initialize();
        }

        public override CollisionType CollisionType => CollisionType.ITEM;

        public int PointValue
        {
            get;
            private set;
        }

        public PointItemType PointItemType
        {
            get
            {
                return _type;
            }
        }

        private void Initialize()
        {
            _canSteal = true;
            switch (_type)
            {
                case PointItemType.KEEN4_SHIKADI_SODA:
                    PointValue = 100;
                    SpriteList = SpriteSheet.SpriteSheet.Keen4ShikadiSodaImages;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints100;
                    break;
                case PointItemType.KEEN4_THREE_TOOTH_GUM:
                    PointValue = 200;
                    SpriteList = SpriteSheet.SpriteSheet.Keen43ToothGum;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints200;
                    break;
                case PointItemType.KEEN4_SHIKKERS_CANDY_BAR:
                    PointValue = 500;
                    SpriteList = SpriteSheet.SpriteSheet.Keen4ShikkersCandyBar;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints500;
                    break;
                case PointItemType.KEEN4_JAWBREAKER:
                    PointValue = 1000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen4JawBreaker;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints1000;
                    break;
                case PointItemType.KEEN4_DOUGHNUT:
                    PointValue = 2000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen4Doughnut;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints2000;
                    break;
                case PointItemType.KEEN4_ICECREAM_CONE:
                    PointValue = 5000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen4IceCreamCone;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints5000;
                    break;
                case PointItemType.KEEN5_SHIKADI_GUM:
                    PointValue = 100;
                    SpriteList = SpriteSheet.SpriteSheet.Keen5ShikadiGum;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints100;
                    break;
                case PointItemType.KEEN5_MARSHMALLOW:
                    PointValue = 200;
                    SpriteList = SpriteSheet.SpriteSheet.Keen5Marshmallow;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints200;
                    break;
                case PointItemType.KEEN5_CHOCOLATE_MILK:
                    PointValue = 500;
                    SpriteList = SpriteSheet.SpriteSheet.Keen5ChocolateMilk;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints500;
                    break;
                case PointItemType.KEEN5_TART_STIX:
                    PointValue = 1000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen5TartStix;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints1000;
                    break;
                case PointItemType.KEEN5_SUGAR_STOOPIES_CEREAL:
                    PointValue = 2000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen5SugarStoopiesCereal;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints2000;
                    break;
                case PointItemType.KEEN5_BAG_O_SUGAR:
                    PointValue = 5000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen5BagOSugar;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints5000;
                    break;
                case PointItemType.KEEN6_BLOOG_SODA:
                    PointValue = 100;
                    SpriteList = SpriteSheet.SpriteSheet.Keen6BloogSoda;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints100;
                    break;
                case PointItemType.KEEN6_ICE_CREAM_BAR:
                    PointValue = 200;
                    SpriteList = SpriteSheet.SpriteSheet.Keen6IceCreamBar;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints200;
                    break;
                case PointItemType.KEEN6_PUDDING:
                    PointValue = 500;
                    SpriteList = SpriteSheet.SpriteSheet.Keen6Pudding;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints500;
                    break;
                case PointItemType.KEEN6_ROOT_BEER_FLOAT:
                    PointValue = 1000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen6RootBeerFloat;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints1000;
                    break;
                case PointItemType.KEEN6_BANANA_SPLIT:
                    PointValue = 2000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen6BananaSplit;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints2000;
                    break;
                case PointItemType.KEEN6_PIZZA_SLICE:
                    PointValue = 5000;
                    SpriteList = SpriteSheet.SpriteSheet.Keen6PizzaSlice;
                    AcquiredSpriteList = SpriteSheet.SpriteSheet.KeenPoints5000;
                    break;
            }
            if (this.SpriteList != null && this.SpriteList.Length > 0)
            {
                this.Image = this.SpriteList[0];
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + $"{separator}{this.ZIndex}{separator}{this.PointItemType.ToString()}";
        }
    }
}
