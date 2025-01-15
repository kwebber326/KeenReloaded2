using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class FlippingPlatform : CollisionObject, IUpdatable, ISprite
    {
        private Image _sprite;
        private Dictionary<FlippingPlatformState, Image> _spritesByState;
        private Dictionary<FlippingPlatformState, Point> _offsetsByState;
        private const int STILL_TIME = 60;
        private int _stillTimeTick;
        Point _originalLocation;
        private Size _originalSize;
        private bool _firstTime = true;

        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick;

        private const int STANDING_OFFSET_Y = 9;

        private FlippingPlatformState _currentState;
        private Rectangle _area;
        private readonly int _zIndex;

        public FlippingPlatform(Rectangle area, SpaceHashGrid grid, int zIndex, FlippingPlatformState initialState)
            : base(grid, area)
        {
            _currentState = initialState;
            _area = area;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            _originalLocation = new Point(this.HitBox.X, this.HitBox.Y);
            _originalSize = new Size(this.HitBox.Width, this.HitBox.Height);
            if (_collisionGrid != null && _collidingNodes != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + STANDING_OFFSET_Y, this.HitBox.Width, this.HitBox.Height - STANDING_OFFSET_Y);
                if (_currentState == FlippingPlatformState.STILL)
                {
                    this.StandingTile = new InvisiblePlatformTile(_collisionGrid, new Rectangle(this.HitBox.Location, this.HitBox.Size));
                    _firstTime = false;
                }
            }
            _spritesByState = new Dictionary<FlippingPlatformState, Image>();
            _spritesByState.Add(FlippingPlatformState.STILL, Properties.Resources.flipping_platform_still);
            _spritesByState.Add(FlippingPlatformState.FLIP_45, Properties.Resources.flipping_platform_flip_45);
            _spritesByState.Add(FlippingPlatformState.FLIP_90, Properties.Resources.flipping_platform_flip_90);
            _spritesByState.Add(FlippingPlatformState.FLIP_135, Properties.Resources.flipping_platform_flip_135);

            _offsetsByState = new Dictionary<FlippingPlatformState, Point>();
            _offsetsByState.Add(FlippingPlatformState.STILL, new Point(0, 15));
            _offsetsByState.Add(FlippingPlatformState.FLIP_45, new Point(2, -15));
            _offsetsByState.Add(FlippingPlatformState.FLIP_90, new Point(15, 0));
            _offsetsByState.Add(FlippingPlatformState.FLIP_135, new Point(-15, 2));
            _sprite = _spritesByState[_currentState];


        }

        public void Update()
        {
            switch (_currentState)
            {
                case FlippingPlatformState.STILL:
                    this.UpdateStillState();
                    break;
                case FlippingPlatformState.FLIP_45:
                    this.Flip45();
                    break;
                case FlippingPlatformState.FLIP_90:
                    this.FLip90();
                    break;
                case FlippingPlatformState.FLIP_135:
                    this.FLip135();
                    break;
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
                if (this.HitBox != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public InvisiblePlatformTile StandingTile
        {
            get;
            private set;
        }

        private void UpdateStillState()
        {
            if (this.State != FlippingPlatformState.STILL)
            {
                this.State = FlippingPlatformState.STILL;
                _stillTimeTick = 0;
                _area = new Rectangle(_originalLocation.X, _originalLocation.Y, _originalSize.Width, _originalSize.Height);
                if (!_firstTime)
                {
                    this.StandingTile.AddTileToGrid();
                }
                else
                {
                    this.StandingTile = new InvisiblePlatformTile(_collisionGrid, new Rectangle(this.HitBox.Location, this.HitBox.Size));
                    _firstTime = false;
                }
            }

            if (_stillTimeTick++ == STILL_TIME)
            {
                this.Flip45();
            }
        }

        private void SetOffsetByCurrentState()
        {
            int offsetX = 0, offsetY = 0;
            offsetX = _offsetsByState[_currentState].X;
            offsetY = _offsetsByState[_currentState].Y;
            _area.Location = new Point(_area.Location.X + offsetX, _area.Location.Y + offsetY);
        }

        private void Flip45()
        {
            if (this.State != FlippingPlatformState.FLIP_45)
            {
                this.State = FlippingPlatformState.FLIP_45;
                SetOffsetByCurrentState();
                this.StandingTile?.RemoveTileFromGrid();
            }

            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                this.FLip90();
            }
        }

        private void FLip90()
        {
            if (this.State != FlippingPlatformState.FLIP_90)
            {
                this.State = FlippingPlatformState.FLIP_90;
                SetOffsetByCurrentState();
            }

            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                this.FLip135();
            }
        }

        private void FLip135()
        {
            if (this.State != FlippingPlatformState.FLIP_135)
            {
                this.State = FlippingPlatformState.FLIP_135;
                SetOffsetByCurrentState();
            }

            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                this.UpdateStillState();
            }
        }

        FlippingPlatformState State
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                _sprite = _spritesByState[_currentState];
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.NONE;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageKey = GetImageKeyByState(_currentState);
            return $"{imageKey}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }

        public static string GetImageKeyByState(FlippingPlatformState state)
        {
            switch (state)
            {
                case FlippingPlatformState.FLIP_135:
                    return nameof(Properties.Resources.flipping_platform_flip_135);
                case FlippingPlatformState.FLIP_45:
                    return nameof(Properties.Resources.flipping_platform_flip_45);
                case FlippingPlatformState.FLIP_90:
                    return nameof(Properties.Resources.flipping_platform_flip_90);
                case FlippingPlatformState.STILL:
                    return nameof(Properties.Resources.flipping_platform_still);
            }

            return nameof(Properties.Resources.flipping_platform_still);
        }
    }

    public enum FlippingPlatformState
    {
        STILL,
        FLIP_45,
        FLIP_90,
        FLIP_135
    }
}
