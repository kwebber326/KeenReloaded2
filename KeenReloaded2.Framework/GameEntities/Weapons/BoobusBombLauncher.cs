using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
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
    public class BoobusBombLauncher : NeuralStunner
    {
        public BoobusBombLauncher(SpaceHashGrid grid, Rectangle hitbox, int ammo = 10)
            : base(grid, hitbox, ammo)
        {
            VELOCITY = 60;
            BLAST_RADIUS = 20;
            DAMAGE = 2;
        }

        protected override void s_ObjectComplete(object sender, EventArgs e)
        {
            var trajectory = sender as BoobusBombShot;
            ObjectEventArgs args = new ObjectEventArgs()
            {
                ObjectSprite = trajectory
            };
            OnRemovedObject(args);
            if (trajectory != null)
            {
                trajectory.Create += new EventHandler<ObjectEventArgs>(trajectory_Create);
                trajectory.Explode();
            }
        }

        void trajectory_Create(object sender, ObjectEventArgs e)
        {
            OnCreatedObject(e);
            var explosion = e.ObjectSprite as BoobusBombExplosion;
            if (explosion != null)
            {
                explosion.Remove += new EventHandler<ObjectEventArgs>(explosion_Remove);
            }
        }

        void explosion_Remove(object sender, ObjectEventArgs e)
        {
            OnRemovedObject(e);
        }

        public Direction KeenStandDirection { get; set; }

        public override void Fire()
        {
            if (!_isFiring)
            {   //TODO: make all these values driven by constants
                for (int i = 0; i < SHOTS_PER_FIRE; i++)
                {
                    if (i <= _ammo)
                    {
                        BoobusBombShot b = null;

                        switch (this.Direction)
                        {
                            case Enums.Direction.RIGHT:
                                b = new BoobusBombShot(_grid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction, this.KeenStandDirection);
                                break;
                            case Enums.Direction.LEFT:
                                b = new BoobusBombShot(_grid, new Rectangle(this.HitBox.Left - 34, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction, this.KeenStandDirection);
                                break;
                            case Enums.Direction.UP:
                                b = new BoobusBombShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Top, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction, this.KeenStandDirection);
                                break;
                            case Enums.Direction.DOWN:
                                b = new BoobusBombShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Bottom, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction, this.KeenStandDirection);
                                break;
                            default:
                                b = new BoobusBombShot(_grid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction, this.KeenStandDirection);
                                break;
                        }
                        b.KeenStandDirection = this.KeenStandDirection;
                        b.ObjectComplete += new EventHandler(s_ObjectComplete);
                        ObjectEventArgs e = new ObjectEventArgs()
                        {
                            ObjectSprite = b
                        };
                        OnCreatedObject(e);
                        _ammo--;
                    }
                }
                _currentRefireDelayTick = REFIRE_DELAY;
            }
        }
    }
}
