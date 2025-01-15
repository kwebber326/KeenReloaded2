using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public abstract class Slicestar : DestructibleObject, IUpdatable, ISprite, IEnemy, ICreateRemove, IZombieBountyEnemy
    {
        private const int DEATH_TIME = 15;
        private int _deathTimeTick;

        protected Image _sprite;
        protected readonly int _zIndex;
        protected int VELOCITY = 10;

        public Slicestar(SpaceHashGrid grid, Rectangle area, int zIndex, Direction direction)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            this.Direction = direction;
            Initialize();
        }

        protected virtual void Initialize()
        {
            this.Health = 20;
            _sprite = Properties.Resources.keen5_slicestar;
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
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(this.Direction);
                }
            }
        }

        protected virtual Enums.Direction Direction
        {
            get;
            set;
        }

        protected override void HandleCollision(CollisionObject obj)
        {

        }

        public override void Die()
        {
            _sprite = Properties.Resources.keen5_slicestar_destroyed;
            UpdateDeathSprite();
        }

        public void Update()
        {
            if (!this.IsDead())
            {
                this.Move();
            }
            else
            {
                UpdateDeathSprite();
            }
        }

        private void UpdateDeathSprite()
        {
            if (_deathTimeTick++ == DEATH_TIME)
            {
                ObjectEventArgs e = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                OnRemove(e);
            }
        }

        public abstract void Move();

        public bool DeadlyTouch
        {
            get { return !IsDead(); }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return !IsDead(); }
        }

        public PointItemType PointItem => PointItemType.KEEN5_BAG_O_SUGAR;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        private void OnRemove(ObjectEventArgs e)
        {
            if (Remove != null)
            {
                if (e.ObjectSprite == this)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                    }
                }
                Remove(this, e);
            }
        }

        private void OnCreate(ObjectEventArgs e)
        {
            if (Create != null)
            {
                Create(this, e);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_slicestar);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{this.Direction}";
        }
    }
}
