using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class KeenRPGShot : KeenStunShot, IExplodable, ICreateRemove
    {
        private ExplosionState _state;
        public KeenRPGShot(SpaceHashGrid grid, Rectangle hitbox,
            int damage, int velocity, int pierce, int spread, int blastRadius, int refireDelay, Direction direction)
            : base(grid, hitbox, damage, velocity, pierce, spread, blastRadius, refireDelay, direction)
        {
            this.InitializeSprites();
        }

        void KeenRPGShot_ObjectComplete(object sender, EventArgs e)
        {
            this.Explode();
        }
        protected override void InitializeSprites()
        {
            base.InitializeSprites();

            _shotCompleteSprites = new Image[]{

            };
            switch (this.Direction)
            {
                case Enums.Direction.RIGHT:
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.neural_stunner_rpg1,
                        Properties.Resources.neural_stunner_rpg2
                    };
                    break;
                case Enums.Direction.LEFT:
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.neural_stunner_rpg1_left,
                        Properties.Resources.neural_stunner_rpg2_left
                    };
                    break;
                case Enums.Direction.UP:
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.neural_stunner_rpg1_up,
                        Properties.Resources.neural_stunner_rpg2_up
                    };
                    break;
                case Enums.Direction.DOWN:
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.neural_stunner_rpg1_down,
                        Properties.Resources.neural_stunner_rpg2_down
                    };
                    break;
            }
            _sprite = _shotSprites[0];
        }

        public void Explode()
        {
            RPGExplosion explosion = new RPGExplosion(_collisionGrid, this.HitBox, this.BlastRadius, this.Damage);
            explosion.Remove += new EventHandler<ObjectEventArgs>(explosion_Remove);
            explosion.Create += new EventHandler<ObjectEventArgs>(explosion_Create);
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = explosion
            };
            OnCreate(e);
        }

        public override void Update()
        {
            base.Update();
            if (_shotComplete)
            {
                foreach (var nodes in _collidingNodes)
                {
                    nodes.Objects.Remove(this);
                    nodes.NonEnemies.Remove(this);
                }
            }
        }

        void explosion_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        void explosion_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        public ExplosionState ExplosionState
        {
            get { return _state; }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        protected void OnCreate(ObjectEventArgs e)
        {
            if (this.Create != null)
            {
                this.Create(this, e);
            }
        }

        protected void OnRemove(ObjectEventArgs e)
        {
            if (this.Remove != null)
            {
                this.Remove(this, e);
            }
        }
    }
}
