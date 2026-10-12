using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Walls
{
    public class Keen5AltWallRight : MaskedTile
    {
        public Keen5AltWallRight(Rectangle area, SpaceHashGrid grid, int zIndex) 
            : base(area, grid, area, null, zIndex)
        {
            _image = Properties.Resources.keen5_alternate_black_tile_right;
            _initialImageName = nameof(Properties.Resources.keen5_alternate_black_tile_right);
            this.HitBox = new Rectangle(_area.X, _area.Y, 36, _image.Height);
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;
    }
}
