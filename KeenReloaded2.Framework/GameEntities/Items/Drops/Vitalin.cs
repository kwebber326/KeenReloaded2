using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class Vitalin : Item, IDropCollector
    {
        public Vitalin(Rectangle area, SpaceHashGrid grid, string imageName, int zIndex)
            : base(area, imageName, grid, zIndex)
        {
            Initialize();
        }

        private void Initialize()
        {
            _canSteal = true;
            _moveUp = false;
            this.AcquiredSpriteList = SpriteSheet.SpriteSheet.Keen5VitalinAcquiredImages;

            this.SpriteList = SpriteSheet.SpriteSheet.Keen5VitalinImages;
            this.Image = this.SpriteList[0];
        }

        public int DropVal
        {
            get { return 1; }
        }

        public override CollisionType CollisionType => CollisionType.ITEM;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + $"{separator}{this.ZIndex}";
        }
    }
}
