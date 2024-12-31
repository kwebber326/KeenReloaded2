using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Weapons
{
    public class SnakeGun : NeuralStunner
    {
        public SnakeGun(SpaceHashGrid grid, Rectangle hitbox, int ammo = 5) : base(grid, hitbox, ammo)
        {
            VELOCITY = 150;
            REFIRE_DELAY = 30;
            BLAST_RADIUS = 64;
            DAMAGE = 3;
            PIERCE = 2;
        }

        public override void Fire()
        {
            if (!_isFiring)
            {   //TODO: make all these values driven by constants
                for (int i = 0; i < SHOTS_PER_FIRE; i++)
                {
                    if (i <= _ammo)
                    {
                        SnakeGunShot s = null;
                        switch (this.Direction)
                        {
                            case Enums.Direction.RIGHT:
                                s = new SnakeGunShot(_grid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Top + 10, 60, 15), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.LEFT:
                                s = new SnakeGunShot(_grid, new Rectangle(this.HitBox.Left - 34, this.HitBox.Top + 10, 60, 15), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.UP:
                                s = new SnakeGunShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Top - 34, 15, 60), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.DOWN:
                                s = new SnakeGunShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Bottom + 1, 15, 60), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            default:
                                s = new SnakeGunShot(_grid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Top + 10, 60, 15), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
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
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.SNAKE_GUN_FIRE);
            }
        }

        protected override void s_ObjectComplete(object sender, EventArgs e)
        {
            var projectile = sender as SnakeGunShot;
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
            var explosion = e.ObjectSprite as SnakeGunExplosion;
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
