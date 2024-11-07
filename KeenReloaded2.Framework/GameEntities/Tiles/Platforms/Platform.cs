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
    public abstract class Platform : MovingPlatformTile, IUpdatable, ISprite
    {
        protected PlatformType _type;
        protected CommanderKeen _keen;
        protected int _moveVelocity = 5;
        protected int _acceleration = 10;
        protected Direction _direction;
        protected int MAX_GRAVITY_SPEED = 50;
        protected int _currentVerticalVelocity;

        protected Image[] _images;
        protected readonly Guid _activationId;

        public Platform(SpaceHashGrid grid, Rectangle hitbox, PlatformType type, int zIndex, Guid activationId)
            : base(grid, hitbox, null, zIndex)
        {
            _type = type;
            _activationId = activationId;
            this.HitBox = hitbox;
            Initialize();
        }

        protected virtual void Initialize()
        {
            SetInitialSprite();
        }

        protected void SetInitialSprite()
        {
            switch (_type)
            {
                case PlatformType.KEEN4:
                    _image = Properties.Resources.keen4_platorm_stationary;
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_platorm_stationary
                    };
                    break;
                case PlatformType.KEEN5_PINK:
                    _image = Properties.Resources.keen5_pink_platform;
                    _images = new Image[]
                    {
                        Properties.Resources.keen5_pink_platform
                    };
                    break;
                case PlatformType.KEEN5_ORANGE:
                    _image = Properties.Resources.keen5_orange_platform;
                    _images = new Image[]
                    {
                        Properties.Resources.keen5_orange_platform
                    };
                    break;
                case PlatformType.KEEN6:
                    _image = Properties.Resources.keen6_bip_platform;
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

        protected string GetImageNameFromType()
        {
            switch (_type)
            {
                case PlatformType.KEEN4:
                    return nameof(Properties.Resources.keen4_platform_horizontal_left1);
                case PlatformType.KEEN5_PINK:
                    return nameof(Properties.Resources.keen5_pink_platform);
                case PlatformType.KEEN5_ORANGE:
                    return nameof(Properties.Resources.keen5_orange_platform);
                case PlatformType.KEEN6:
                    return nameof(Properties.Resources.keen6_bip_platform);
            }

            return nameof(Properties.Resources.keen6_bip_platform);
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
                _area = new Rectangle(this.HitBox.X, this.HitBox.Y - this.HitBox.Height / 2,
                    this.HitBox.Width, this.HitBox.Height * 2);
                if (_collidingNodes != null && _collisionGrid != null && value != null)
                {
                    this.UpdateCollisionNodes(_direction);
                }
            }
        }
    }
}
