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
    public class BFGProjectile : KeenStunShot, IExplodable, ICreateRemove
    {
        private ExplosionState _state;
        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        public BFGProjectile(SpaceHashGrid grid, Rectangle hitbox, int damage, int velocity, int pierce, int spread, int blastRadius, int refireDelay, Direction direction) : base(grid, hitbox, damage, velocity, pierce, spread, blastRadius, refireDelay, direction)
        {
            this.InitializeSprites();
        }

        void KeenRPGShot_ObjectComplete(object sender, EventArgs e)
        {
            this.Explode();
        }

        public void Explode()
        {
            BFGExplosion explosion = new BFGExplosion(_collisionGrid, new Rectangle(this.HitBox.Location, this.HitBox.Size), this.BlastRadius, this.Damage, this);
            RegisterExplostionEvents(explosion);
        }

        public void RegisterExplostionEvents(BFGExplosion explosion)
        {
            explosion.Remove += new EventHandler<ObjectEventArgs>(explosion_Remove);
            explosion.Create += new EventHandler<ObjectEventArgs>(explosion_Create);
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = explosion
            };
            OnCreate(e);
        }

        protected override void InitializeSprites()
        {
            base.InitializeSprites();

            _shotCompleteSprites = new Image[] { };
            _shotSprites = new Image[]
            {
                Properties.Resources.BFG_shot1,
                Properties.Resources.BFG_shot2,
                Properties.Resources.BFG_shot3,
                Properties.Resources.BFG_shot4,
            };
            _sprite = _shotSprites[0];
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

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;

                if (_collidingNodes != null)
                    this.UpdateCollisionNodes(this.Direction);
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
