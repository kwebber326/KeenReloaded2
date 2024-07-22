using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class DestructibleCollisionTile : MaskedTile
    {
        public DestructibleCollisionTile(SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex) 
            : base(hitbox, grid, hitbox, imageFile, zIndex)
        {
        }

        public override CollisionType CollisionType => CollisionType.DESTRUCTIBLE_BLOCK | CollisionType.BLOCK;
    }
}
