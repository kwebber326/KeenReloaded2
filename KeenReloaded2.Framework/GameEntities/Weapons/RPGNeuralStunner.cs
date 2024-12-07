using KeenReloaded.Framework;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Weapons
{
    public class RPGNeuralStunner : NeuralStunner
    {
        public RPGNeuralStunner(SpaceHashGrid grid, Rectangle hitbox, int ammo = 25)
            : base(grid, hitbox, ammo)
        {
            VELOCITY = 60;
            REFIRE_DELAY = 20;
            BLAST_RADIUS = 80;
            DAMAGE = 2;
        }

        public override void Fire()
        {
            if (!_isFiring)
            {   //TODO: make all these values driven by constants
                for (int i = 0; i < SHOTS_PER_FIRE; i++)
                {
                    if (i <= _ammo)
                    {
                        KeenRPGShot s = null;
                        switch (this.Direction)
                        {
                            case Enums.Direction.RIGHT:
                                s = new KeenRPGShot(_grid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.LEFT:
                                s = new KeenRPGShot(_grid, new Rectangle(this.HitBox.Left - 34, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.UP:
                                s = new KeenRPGShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Top - 34, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.DOWN:
                                s = new KeenRPGShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Bottom + 1, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            default:
                                s = new KeenRPGShot(_grid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                        }
                        s.ObjectComplete += new EventHandler(s_ObjectComplete);
                        ObjectEventArgs e = new ObjectEventArgs()
                        {
                            ObjectSprite = s
                        };
                        OnCreatedObject(e);
                        _ammo--;
                    }
                }
                _currentRefireDelayTick = REFIRE_DELAY;
            }
        }

        protected override void s_ObjectComplete(object sender, EventArgs e)
        {
            var projectile = sender as KeenRPGShot;
            ObjectEventArgs args = new ObjectEventArgs()
            {
                ObjectSprite = projectile
            };
            OnRemovedObject(args);
            if (projectile != null)
            {
                projectile.Create += new EventHandler<ObjectEventArgs>(trajectory_Create);
                projectile.Explode();
            }
        }

        void trajectory_Create(object sender, ObjectEventArgs e)
        {
            OnCreatedObject(e);
            var explosion = e.ObjectSprite as RPGExplosion;
            if (explosion != null)
            {
                explosion.Remove += new EventHandler<ObjectEventArgs>(explosion_Remove);
            }
        }

        void explosion_Remove(object sender, ObjectEventArgs e)
        {
            OnRemovedObject(e);
        }
    }
}
