using KeenReloaded.Framework;
using KeenReloaded2.Constants;
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
        private readonly Rectangle[] _hitBoxes;

        public MultiHitBoxWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Guid activationId, Rectangle[] entryPoints, Rectangle[] hitboxes) 
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, activationId, entryPoints)
        {
            _hitBoxes = hitboxes;
            if (_collisionGrid != null && _collidingNodes != null)
            {
                foreach (var hitbox in _hitBoxes)
                {
                    var trueArea = new Rectangle(this.HitBox.X + hitbox.X, this.HitBox.Y + hitbox.Y,
                        hitbox.Width, hitbox.Height);
                    InvisibleTile tile = new InvisibleTile(grid, trueArea);
                }
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

     

        public override string ToString()
        {
            string hitBoxArr = BuildRectangleArrayStringRepresentation(_hitBoxes);
            return base.ToString() + $"{_separator}{hitBoxArr}";
        }
    }
}
