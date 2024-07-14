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
    public class RailgunNeuralStunner : NeuralStunner
    {
        public RailgunNeuralStunner(SpaceHashGrid grid, Rectangle hitbox, int ammo = 25)
            : base(grid, hitbox, ammo)
        {
            VELOCITY = 400;
            REFIRE_DELAY = 50;
            PIERCE = 100;
            DAMAGE = 20;
        }

        public override void Fire()
        {
            if (!_isFiring)
            {
                for (int i = 0; i < SHOTS_PER_FIRE; i++)
                {
                    if (i <= _ammo)
                    {
                        KeenStunShot s = null;
                        switch (this.Direction)
                        {
                            case Enums.Direction.RIGHT:
                                s = new KeenRailgunShot(_grid, new Rectangle(this.HitBox.Left, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.LEFT:
                                s = new KeenRailgunShot(_grid, new Rectangle(this.HitBox.Right, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.UP:
                                s = new KeenRailgunShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Top - 34, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            case Enums.Direction.DOWN:
                                s = new KeenRailgunShot(_grid, new Rectangle(this.HitBox.Right - 30, this.HitBox.Bottom + 1, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
                                break;
                            default:
                                s = new KeenRailgunShot(_grid, new Rectangle(this.HitBox.Left, this.HitBox.Top + 10, 34, 33), DAMAGE, VELOCITY, PIERCE, SPREAD, BLAST_RADIUS, REFIRE_DELAY, this.Direction);
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
    }
}
