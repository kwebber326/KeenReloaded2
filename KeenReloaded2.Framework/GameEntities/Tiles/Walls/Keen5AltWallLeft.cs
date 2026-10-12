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
    public class Keen5AltWallLeft : MaskedTile
    {
        public Keen5AltWallLeft(Rectangle area, SpaceHashGrid grid, int zIndex) : base(area, grid, area, null, zIndex)
        {
            _image = Properties.Resources.keen5_alternate_black_tile_left;
            _initialImageName = nameof(Properties.Resources.keen5_alternate_black_tile_left);
            this.HitBox = new Rectangle(_area.X + _image.Width - 10, _area.Y, 10, _image.Height);
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;
    }
}
