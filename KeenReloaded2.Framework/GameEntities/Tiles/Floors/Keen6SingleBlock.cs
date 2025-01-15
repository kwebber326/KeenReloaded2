using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Floors
{
    public class Keen6SingleBlock : MaskedTile
    {
        public Keen6SingleBlock(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(area, grid, area, null, zIndex)
        {
            _image = Properties.Resources.keen6_industrial_single_block;
            _downwardCollisionOffset = 8;
            _rightwardCollisionOffset = 8;
            _initialImageName = nameof(Properties.Resources.keen6_industrial_single_block);
            if (_collisionGrid != null && _collidingNodes != null)
                this.AdjustHitboxBasedOnOffsets();
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;
    }
}
