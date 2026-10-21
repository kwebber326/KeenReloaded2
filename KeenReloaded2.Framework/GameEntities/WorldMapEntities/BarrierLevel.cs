using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class BarrierLevel : ForeGroundMultiHitboxWorldMapLevel
    {
        public BarrierLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Guid activationId, Rectangle[] entryPoints, Rectangle[] hitboxes, Rectangle[] foregroundAreas, bool isActive)
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, activationId, entryPoints, hitboxes, foregroundAreas)
        {
            _isActive = isActive;
            if (_collisionGrid != null && _collidingNodes != null
                && !_isActive)
            {
                this.Deactivate();
            }
        }

        public override int EntryPriority => 10;

        public override void Deactivate()
        {
            base.Deactivate();
            _sprite = null;
            this.OnRemove(new GameEventArgs.ObjectEventArgs() { ObjectSprite = this });
            foreach (InvisibleTile tile in _collisionTiles)
            {
                tile.RemoveTileFromGrid();
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + separator + _isActive.ToString();
        }
    }
}
