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
    public class SnakeGunShot : KeenStunShot, IExplodable, ICreateRemove
    {
        private ExplosionState _state;
        public SnakeGunShot(SpaceHashGrid grid, Rectangle hitbox,
            int damage, int velocity, int pierce, int spread, int blastRadius, int refireDelay, Direction direction)
            : base(grid, hitbox, damage, velocity, pierce, spread, blastRadius, refireDelay, direction)
        {
            //this.ObjectComplete += new EventHandler(KeenRPGShot_ObjectComplete);
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
                case Enums.Direction.LEFT:
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.snake_gun_shot_horizontal1,
                        Properties.Resources.snake_gun_shot_horizontal2,
                        Properties.Resources.snake_gun_shot_horizontal3,
                        Properties.Resources.snake_gun_shot_horizontal4,
                    };
                    break;
                case Enums.Direction.UP:
                case Enums.Direction.DOWN:
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.snake_gun_shot_vertical1,
                        Properties.Resources.snake_gun_shot_vertical2,
                        Properties.Resources.snake_gun_shot_vertical3,
                        Properties.Resources.snake_gun_shot_vertical4,
                    };
                    break;
            }
            _sprite = _shotSprites[0];
        }

        public void Explode()
        {
            SnakeGunExplosion explosion = new SnakeGunExplosion(_collisionGrid, this.HitBox, this.BlastRadius, this.Damage, this, new Rectangle(this.HitBox.Location, this.HitBox.Size), this.Direction);
            RegisterExplosionEvents(explosion);
        }

        public void RegisterExplosionEvents(SnakeGunExplosion explosion)
        {
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
