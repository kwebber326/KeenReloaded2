using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public abstract class Platform : CollisionObject, IUpdatable, ISprite
    {
        protected PlatformType _type;
        private Image _sprite;
        protected CommanderKeen _keen;
        protected int _moveVelocity = 5;
        protected int _acceleration = 10;
        protected Direction _direction;
        protected int MAX_GRAVITY_SPEED = 50;
        protected int _currentVerticalVelocity;

        protected Image[] _images;
        protected readonly Guid _activationId;
        protected readonly int _zIndex;

        public Platform(SpaceHashGrid grid, Rectangle hitbox, PlatformType type, int zIndex, Guid activationId)
            : base(grid, hitbox)
        {
            _zIndex = zIndex;
            _type = type;
            _activationId = activationId;
            this.HitBox = hitbox;
            Initialize();
        }

        public void AssignKeen(CommanderKeen keen)
        {
            _keen = keen;
            _keen.KeenMoved += new EventHandler(_keen_KeenMoved);
        }

        void _keen_KeenMoved(object sender, EventArgs e)
        {
            if (_keen != null && (_keen.IsDead() || _keen.MoveState == Enums.MoveState.ON_POLE || !IsKeenStandingOnPlatform()))
            {
                UnassignKeen();
            }
        }

        private void UnassignKeen()
        {
            _keen.KeenMoved -= _keen_KeenMoved;
            _keen = null;
        }

        public void UnassignKeen(CommanderKeen keen)
        {
            if (_keen == keen)
            {
                UnassignKeen();
            }
        }

        protected virtual void Initialize()
        {
            SetInitialSprite();
            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + (_sprite.Height / 2), _sprite.Width, _sprite.Height / 2);
        }

        protected void SetInitialSprite()
        {
            switch (_type)
            {
                case PlatformType.KEEN4:
                    _sprite = Properties.Resources.keen4_platorm_stationary;
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_platorm_stationary
                    };
                    break;
                case PlatformType.KEEN5_PINK:
                    _sprite = Properties.Resources.keen5_pink_platform;
                    _images = new Image[]
                    {
                        Properties.Resources.keen5_pink_platform
                    };
                    break;
                case PlatformType.KEEN5_ORANGE:
                    _sprite = Properties.Resources.keen5_orange_platform;
                    _images = new Image[]
                    {
                        Properties.Resources.keen5_orange_platform
                    };
                    break;
                case PlatformType.KEEN6:
                    _sprite = Properties.Resources.keen6_bip_platform;
                    _images = new Image[]
                    {
                        Properties.Resources.keen6_bip_platform
                    };
                    break;
            }
        }

        protected virtual bool IsKeenStandingOnPlatform()
        {
            if (_keen == null)
                return false;

            bool intersectsX = _keen.HitBox.Right > this.HitBox.Left && _keen.HitBox.Left < this.HitBox.Right;//_keen.HitBox.Left > this.HitBox.Left && _keen.HitBox.Left < this.HitBox.Right;
            if (_keen.IsLookingDown && _direction == Direction.DOWN)
            {
                bool standingOnPlatformLookingDown = intersectsX && _keen.HitBox.Bottom >= this.HitBox.Top - 21 && _keen.HitBox.Top <= this.HitBox.Top;
                if (standingOnPlatformLookingDown)
                {
                    this.UpdateKeenVerticalPosition();
                }
                return standingOnPlatformLookingDown;
            }
            return _keen.HitBox.Bottom == this.HitBox.Top - 1 && intersectsX;
        }

        protected virtual void UpdateKeenVerticalPosition()
        {
            if (!_keen.IsDead())
                _keen.GetMovedVertically(this);
        }

        protected virtual void UpdateKeenHorizontalPosition()
        {
            if (!_keen.IsDead())
                _keen.GetMovedHorizontally(this, _direction, _moveVelocity);
        }

        public abstract void Activate();

        public abstract void Deactivate();

        public virtual bool IsActive
        {
            get { return false; }
        }

        public abstract void Update();

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null && _collisionGrid != null && value != null)
                {
                    this.UpdateCollisionNodes(_direction);
                }
            }
        }

        public Guid ActivationID => _activationId;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;
    }
}
