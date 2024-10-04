using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Constructs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class ShikadiShock : StraightShotProjectile
    {
        public ShikadiShock(SpaceHashGrid grid, Rectangle hitbox, Direction direction)
            : base(grid, hitbox, direction, EnemyProjectileType.KEEN5_SHIKADI_SHOCK)
        {
        }

        public override void Update()
        {
            base.Update();
            var collisions = this.CheckCollision(this.HitBox);
            var poles = collisions.Where(p => p.CollisionType == CollisionType.POLE || p.CollisionType == CollisionType.POLE_TILE);
            if (!poles.Any())
            {
                _shotComplete = true;
            }
        }
    }
}
