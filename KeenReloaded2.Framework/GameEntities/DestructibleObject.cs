using KeenReloaded.Framework;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities
{
    public abstract class DestructibleObject : CollisionObject
    {
        public DestructibleObject(SpaceHashGrid grid, Rectangle hitbox)
            : base(grid, hitbox)
        {
            this.Health = 1;
        }

        protected bool _killedEventFired;

        public int Health { get; protected set; }

        public virtual void TakeDamage(int damage)
        {
            this.Health -= damage;
            if (this.Health <= 0)
            {
                this.Die();
                if (!_killedEventFired)
                {
                    OnKilled();
                    _killedEventFired = true;
                }
            }
        }

        public virtual void TakeDamage(IProjectile projectile)
        {
            this.Health -= projectile.Damage;
            if (this.Health <= 0)
            {
                this.Die();
                if (!_killedEventFired)
                {
                    OnKilled();
                    _killedEventFired = true;
                }
            }
        }

        public bool IsDead()
        {
            return this.Health <= 0;
        }

        public abstract void Die();

        public event EventHandler<ObjectEventArgs> Killed;

        protected void OnKilled()
        {
            if (Killed != null)
                this.Killed(this, new ObjectEventArgs() { ObjectSprite = this as ISprite });
        }

        protected virtual void HandleCollision(CollisionObject obj)
        {
            if (obj is IProjectile)
            {
                var projectile = (IProjectile)obj;
                this.TakeDamage(projectile);
            }
        }
    }
}
