using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class MultiHitBoxWorldMapLevel : WorldMapLevel
    {
        private readonly List<Rectangle> _hitBoxes;
        public MultiHitBoxWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, List<Rectangle> hitboxes, Image sprite, string levelName, string levelEntryText) 
            : base(area, grid, zIndex, area, sprite, levelName, levelEntryText)
        {
            _hitBoxes = hitboxes;
            if (_collisionGrid != null && _collidingNodes != null)
            {
                foreach (var hitbox in _hitBoxes)
                {
                    InvisibleTile tile = new InvisibleTile(grid, hitbox);
                }
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;
    }
}
