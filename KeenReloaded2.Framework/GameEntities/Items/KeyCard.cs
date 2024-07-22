using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class KeyCard : Item
    {
        public KeyCard(Rectangle area, SpaceHashGrid grid, string imageName, int zIndex) : base(area, imageName, grid, zIndex)
        {
            Initialize();
        }

        public override CollisionType CollisionType => CollisionType.ITEM;

        private void Initialize()
        {
            _canSteal = false;
            this.SpriteList = SpriteSheet.SpriteSheet.Keen5KeyCardImages;
            this.AcquiredSpriteList = SpriteSheet.SpriteSheet.Keen5KeyCardAcquiredImages;

            _sprite = this.SpriteList[0];
        }
    }
}
