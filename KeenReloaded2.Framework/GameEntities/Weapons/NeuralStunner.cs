using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Weapons
{
    public class NeuralStunner : IFireable, IUpdatable
    {
        public NeuralStunner(SpaceHashGrid grid, Rectangle hitbox, int ammo = 5)
        {
            _ammo = ammo;
            _grid = grid;
            this.HitBox = hitbox;
            this.Direction = Enums.Direction.RIGHT;
        }

        protected bool _isFiring = false;
        protected int _ammo;
        protected SpaceHashGrid _grid;
        protected int VELOCITY = 70;
        protected int PIERCE = 0;
        protected int SPREAD = 0;
        protected int BLAST_RADIUS = 0;
        protected int REFIRE_DELAY = 4;
        protected int _currentRefireDelayTick = 0;
        protected int SHOTS_PER_FIRE = 1;
        protected int DAMAGE = 1;
        protected bool IS_AUTO = false;

        public bool IsAuto
        {
            get
            {
                return IS_AUTO;
            }
        }

        public int RefireDelay
        {
            get
            {
                return REFIRE_DELAY;
            }
        }

        public virtual void Fire()
        {
            if (!_isFiring)
            {
                for (int i = 0; i < SHOTS_PER_FIRE; i++)
                {
                    if (_ammo > 0)
                    {
                        KeenStunShot s = null;
                        switch (this.Direction)
                        {
                            case Enums.Direction.RIGHT:
                                s = new KeenStunShot(_grid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.LEFT:
                                s = new KeenStunShot(_grid, new Rectangle(this.HitBox.Left - 34, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.UP:
                                s = new KeenStunShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Top - 34, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.DOWN:
                                s = new KeenStunShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Bottom + 1, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            default:
                                s = new KeenStunShot(_grid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
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

        protected virtual void s_ObjectComplete(object sender, EventArgs e)
        {
            var trajectory = sender as KeenStunShot;
            ObjectEventArgs args = new ObjectEventArgs()
            {
                ObjectSprite = trajectory
            };
            OnRemovedObject(args);
        }

        public event EventHandler<ObjectEventArgs> CreatedObject;
        public event EventHandler<ObjectEventArgs> RemovedObject;

        protected void OnCreatedObject(ObjectEventArgs e)
        {
            if (CreatedObject != null)
                CreatedObject(this, e);
        }
        protected void OnRemovedObject(ObjectEventArgs e)
        {
            if (RemovedObject != null)
                RemovedObject(this, e);
        }

        public bool IsFiring
        {
            get { return _isFiring; }
        }

        public Direction Direction
        {
            get;
            set;
        }

        public int Ammo
        {
            get { return _ammo; }
            set { _ammo = value; }
        }

        public int CurrentDelayTick
        {
            get
            {
                return _currentRefireDelayTick;
            }
        }

        public Rectangle HitBox { get; set; }

        public void Update()
        {
            if (_currentRefireDelayTick != 0)
            {
                _currentRefireDelayTick--;
            }
        }
    }
}
