using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class ExtraLife : Item
    {
        private ExtraLifeType _type;
        public ExtraLife(Rectangle area, SpaceHashGrid grid, string imageName, int zIndex, ExtraLifeType type)
            : base(area, imageName, grid, zIndex)
        {
            _type = type;
            Initialize();
        }

        public ExtraLifeType ExtraLifeType
        {
            get
            {
                return _type;
            }
        }

        public override CollisionType CollisionType => CollisionType.ITEM;

        private void Initialize()
        {
            _canSteal = true;
            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.keen_1up
            };
            switch (_type)
            {
                case ExtraLifeType.KEEN4_LIFEWATER_FLASK:
                    this.SpriteList = SpriteSheet.SpriteSheet.Keen4LifeWaterFlaskImages;
                    break;
                case ExtraLifeType.KEEN5_KEG_O_VITALIN:
                    this.SpriteList = SpriteSheet.SpriteSheet.Keen5KegOVitalin;
                    break;
                case ExtraLifeType.KEEN6_VIVA_QUEEN:
                    this.SpriteList = SpriteSheet.SpriteSheet.Keen6VivaQueenImages;
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
            return base.ToString() + $"{separator}{this.ZIndex}{separator}{_type.ToString()}";
        }
    }
}
