using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class DestructibleCollisionTile : DestructibleObject
    {
        protected CollisionType _collisionType = CollisionType.BLOCK;
        protected int _forceThreshold = 1000;
        protected int _damageThreshold = 500;
        protected bool _isDead;
        protected readonly bool _canBeDestroyedByPogo;

        public DestructibleCollisionTile(SpaceHashGrid grid, Rectangle hitbox, bool canBeDestroyedByPogo)
            : base(grid, hitbox)
        {
            this.Health = 1;
            _canBeDestroyedByPogo = canBeDestroyedByPogo;
        }

        public bool CanBeDestroyedByPogo
        {
            get
            {
                return _canBeDestroyedByPogo;
            }
        }

        public override CollisionType CollisionType => _collisionType;

        public virtual ObjectiveCompleteEvent EventType => ObjectiveCompleteEvent.NONE;

        public override void TakeDamage(int damage)
        {
            if (damage >= _damageThreshold)
            {
                this.Die();
            }
        }

        public override void TakeDamage(IProjectile projectile)
        {
            if (projectile == null)
                return;

            int force = projectile.Velocity * projectile.Pierce;
            int damage = projectile.Damage;
            if (force >= _forceThreshold || damage >= _damageThreshold)
            {
                this.Die();
            }
        }

        public override void Die()
        {
            _collisionType = CollisionType.NONE;
            _isDead = true;
        }

        public override bool IsDead()
        {
            return _isDead;
        }

        public override bool Equals(object obj)
        {
            DestructibleCollisionTile t1 = obj as DestructibleCollisionTile;
            if (t1 == null)
                return false;

            bool hitBoxEqual = t1.HitBox.Equals(this.HitBox);
            return hitBoxEqual;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
