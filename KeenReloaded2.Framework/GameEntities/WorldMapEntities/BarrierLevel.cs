using KeenReloaded.Framework;
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
        public BarrierLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Guid activationId, Rectangle[] entryPoints, Rectangle[] hitboxes, Rectangle[] foregroundAreas) 
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, activationId, entryPoints, hitboxes, foregroundAreas)
        {
        }

        public override int EntryPriority => 10;

        public override void Deactivate()
        {
            base.Deactivate();
            this.OnRemove(new GameEventArgs.ObjectEventArgs() { ObjectSprite = this });
            foreach (InvisibleTile tile in _collisionTiles)
            {
                tile.RemoveTileFromGrid();
            }
        }
    }
}
