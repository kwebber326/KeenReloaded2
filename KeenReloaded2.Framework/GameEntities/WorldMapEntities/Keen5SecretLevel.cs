using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class Keen5SecretLevel : BarrierLevel
    {
        private readonly Rectangle _barrierHitBox;
        private InvisibleTile _barrierTile;

        public Keen5SecretLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Guid activationId, Rectangle[] entryPoints, Rectangle[] hitboxes, Rectangle[] foregroundAreas, bool isActive, Rectangle barrierHitbox)
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, activationId, entryPoints, hitboxes, foregroundAreas, isActive)
        {
            _barrierHitBox = barrierHitbox;
            if (grid != null)
            {
                var trueArea = new Rectangle(this.HitBox.X + barrierHitbox.X, this.HitBox.Y + barrierHitbox.Y,
                      barrierHitbox.Width, barrierHitbox.Height);
                _barrierTile = new InvisibleTile(grid, trueArea);
                _collisionTiles.Add(_barrierTile);
            }
        }

        public override void Deactivate()
        {
            _sprite = Properties.Resources.keen5_world_map_secret_level_beaten;
            _barrierTile.RemoveTileFromGrid();
            _collisionTiles.Remove(_barrierTile);
            _isActive = false;
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + $"{separator}{_barrierHitBox.X}{separator}{_barrierHitBox.Y}{separator}{_barrierHitBox.Width}{separator}{_barrierHitBox.Height}";
        }
    }
}
