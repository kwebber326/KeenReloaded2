using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Tiles;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen6IronSpike : Hazard
    {
        private readonly Rectangle _area;
        private const int COLLISION_Y_OFFSET = 26;

        public Keen6IronSpike(Rectangle area, SpaceHashGrid grid,  int zIndex) 
            : base(grid, area, HazardType.KEEN6_IRON_SPIKE, zIndex)
        {
            _area = area;
            Initialize();
        }
        public override Point Location => _area.Location;
        private void Initialize()
        {
            if (_collidingNodes != null && _collisionGrid != null)
            {
                int yPos = _area.Y + _area.Height - COLLISION_Y_OFFSET;
                Rectangle tileCollsionArea = new Rectangle(_area.X, yPos, _area.Width, COLLISION_Y_OFFSET);
                var collisionTile = new InvisibleTile(_collisionGrid, tileCollsionArea, true);
            }
        }
    }
}
