using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Gix : CollisionObject, IUpdatable, ISprite, IEnemy
    {
        private CommanderKeen _keen;
        private GixState _state;
        private Image _sprite;
        private Enums.Direction _direction;
        private Rectangle _jumpVisionRange;

        private Image[] _moveLeftSprites, _moveRightSprites;
        private const int MOVE_SPRITE_CHANGE_DELAY = 2;
        private int _currentMoveSpriteChangeDelayTick;
        private int _currentMoveSprite;

        private Image[] _slideLeftSprites, _slideRightSprites;
        private const int SLIDE_SPRITE_CHANGE_DELAY = 0;
        private int _currentSlideSpriteChangeDelayTick;
        private int _currentSlideSprite;

        private const int BASIC_FALL_VELOCITY = 30;
        private const int WALK_VELOCITY = 5;
        private const int MAX_VERTICAL_VELOCITY = 40;
        private const int GRAVITY_ACCELERATION = 5;
        private const int AIR_RESISTANCE = 2;
        private const int INITIAL_HORIZONTAL_VELOCITY_ON_JUMP = 20;
        private const int MIN_HORIZONTAL_VELOCITY = 10;
        private const int INITIAL_VERTICAL_VELOCITY_ON_JUMP = 15;
        private int _currentVerticalVelocity, _currentHorizontalVelocity;
        private const int JUMP_VISION_RANGE_WIDTH = 150, JUMP_VISION_RANGE_HEIGHT = 40;
        private const int SLIDE_VELOCITY = 20;
        private const int SLIDE_HIT_TIME = 3;
        private int _slideHitTick;
        private const int HORIZONTAL_CAP_OFFSET = 10;
        private const int VERTICAL_CAP_OFFSET = 5;
        private readonly int _zIndex;

        public Gix(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            //initialize sprite sheet
            _moveLeftSprites = SpriteSheet.SpriteSheet.GixMoveLeftImages;
            _moveRightSprites = SpriteSheet.SpriteSheet.GixMoveRightImages;
            _slideLeftSprites = SpriteSheet.SpriteSheet.GixSlideLeftImages;
            _slideRightSprites = SpriteSheet.SpriteSheet.GixSlideRightImages;
            //initialize direction
            this.Direction = this.GetRandomHorizontalDirection();
            //initialize vision rectangle
            SetVisionRectangle();
            //initialize move state
            this.State = GixState.FALLING;
        }

        private void SetShellCapTile()
        {
            ShellCapTile = new MovingPlatformTile(_collisionGrid, new Rectangle(
                this.HitBox.X + HORIZONTAL_CAP_OFFSET,
                this.HitBox.Y + 1,
                this.HitBox.Width - HORIZONTAL_CAP_OFFSET * 2,
                this.HitBox.Height - VERTICAL_CAP_OFFSET - 1), null, _zIndex);
        }

        private void UpdateShellCapTileLocation()
        {
            ShellCapTile.Move(new Point(this.HitBox.X + HORIZONTAL_CAP_OFFSET, this.HitBox.Y + 1));
        }

        private void SetVisionRectangle()
        {
            _jumpVisionRange = new Rectangle(
                this.HitBox.Left - JUMP_VISION_RANGE_WIDTH//x
              , this.HitBox.Y - JUMP_VISION_RANGE_HEIGHT//y
              , JUMP_VISION_RANGE_WIDTH * 2 + this.HitBox.Width//width
              , JUMP_VISION_RANGE_HEIGHT + this.HitBox.Height);//height
        }

        private MovingPlatformTile ShellCapTile
        {
            get;
            set;
        }

        public void Update()
        {
            _keen = this.GetClosestPlayer();
            switch (_state)
            {
                case GixState.WALKING:
                    this.Walk();
                    break;
                case GixState.JUMPING:
                    this.Jump();
                    break;
                case GixState.SLIDING:
                    this.Slide();
                    break;
                case GixState.FALLING:
                    this.Fall();
                    break;
                case GixState.SLIDE_HIT:
                    this.UpdateSlideHit();
                    break;
            }
        }

        private void UpdateSlideHit()
        {
            if (this.State != GixState.SLIDE_HIT)
            {
                this.State = GixState.SLIDE_HIT;
                _slideHitTick = 0;
            }

            if (_slideHitTick++ == SLIDE_HIT_TIME)
            {
                this.Walk();
            }
        }

        private void Fall()
        {
            if (this.State != GixState.FALLING)
            {
                this.State = GixState.FALLING;
            }

            var tile = this.BasicFallReturnTile(BASIC_FALL_VELOCITY);
            if (tile != null)
            {
                this.Walk();
            }
        }

        private void Slide()
        {
            if (this.State != GixState.SLIDING)
            {
                this.State = GixState.SLIDING;
                _currentVerticalVelocity = 0;
            }

            CollisionObject landingTile = null;
            int yMovement = 0;
            int xMovement = 0;
            Rectangle areaToCheckForKeen = new Rectangle(this.HitBox.Location, this.HitBox.Size);
            if (IsNothingBeneath())
            {
                int previousY = this.HitBox.Y;
                landingTile = this.BasicFallReturnTile(_currentVerticalVelocity);
                int currentY = this.HitBox.Y;
                yMovement = currentY - previousY;
                AccelerateVerticalMovement();
            }

            int xOffset = _direction == Enums.Direction.LEFT ? SLIDE_VELOCITY * -1 : SLIDE_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width + SLIDE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (tile != null)
            {

                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                xMovement = Math.Abs(xCollidePos - this.HitBox.X);
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.UpdateSlideHit();
            }
            else
            {
                xMovement = Math.Abs(xOffset);
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }

            if (xMovement > 0 || yMovement > 0)
            {
                if (_direction == Enums.Direction.LEFT)
                {
                    areaToCheckForKeen = new Rectangle(areaToCheckForKeen.X - xMovement, areaToCheckForKeen.Y - yMovement, areaToCheckForKeen.Width + xMovement, areaToCheckForKeen.Height + yMovement);
                }
                else
                {
                    areaToCheckForKeen.Size = new Size(areaToCheckForKeen.Width + xMovement, areaToCheckForKeen.Height + yMovement);
                }
            }

            this.KillCollidingPlayers(areaToCheckForKeen);
            if (_keen != null && !_keen.IsDead() && IsKeenStandingOnCap())
            {
                _keen.Die();
            }
            var spriteSet = _direction == Enums.Direction.LEFT ? _slideLeftSprites : _slideRightSprites;
            this.UpdateHitboxBasedOnStunnedImage(spriteSet, ref _currentSlideSprite, ref _currentSlideSpriteChangeDelayTick, SLIDE_SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        private bool IsKeenInFrontOfThis()
        {
            if (!_keen.HitBox.IntersectsWith(this.HitBox))
                return false;
            if (_keen.Direction == Enums.Direction.LEFT)
            {
                if (_keen.HitBox.Left <= this.HitBox.Right - 15)
                    return true;
            }
            else
            {
                if (_keen.HitBox.Right >= this.HitBox.Left + 15)
                    return true;
            }

            return false;
        }

        private void ExecuteFreeMoveLogic(int xOffset)
        {
            foreach (var keen in CurrentPlayerList.Players)
            {
                keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
            }

            Rectangle pushAreaToCheck = this.Direction == Enums.Direction.LEFT
                ? new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, xOffset * -1, this.HitBox.Height)
                : new Rectangle(this.HitBox.Right, this.HitBox.Y, xOffset, this.HitBox.Height);

            foreach (var keen in CurrentPlayerList.Players)
            {
                if (!(keen.MoveState == MoveState.ON_POLE || keen.IsDead()))
                {

                    if (keen.HitBox.IntersectsWith(pushAreaToCheck))
                    {
                        keen.SetKeenPushState(this.Direction, true, this);
                        keen.GetMovedHorizontally(this, this.Direction, Math.Abs(_currentHorizontalVelocity));
                    }
                    else
                    {
                        keen.SetKeenPushState(this.Direction, false, this);
                    }
                }
            }
        }

        private void Jump()
        {
            if (this.State != GixState.JUMPING)
            {
                this.Direction = this.SetDirectionFromObjectHorizontal(_keen, true);
                this.State = GixState.JUMPING;
                _currentHorizontalVelocity = _direction == Enums.Direction.LEFT ? INITIAL_HORIZONTAL_VELOCITY_ON_JUMP * -1 : INITIAL_HORIZONTAL_VELOCITY_ON_JUMP;
                _currentVerticalVelocity = INITIAL_VERTICAL_VELOCITY_ON_JUMP * -1;

                foreach (var keen in CurrentPlayerList.Players)
                {
                    if (keen.HitBox.IntersectsWith(this.HitBox) && !IsKeenInFrontOfThis())
                    {
                        keen.SetKeenPushState(ChangeHorizontalDirection(keen.Direction), true, this);
                    }
                    else
                    {
                        keen.SetKeenPushState(ChangeHorizontalDirection(keen.Direction), false, this);
                    }
                }
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.GIK_FLIP);
            }

            Rectangle areaToCheck = new Rectangle(
           _currentHorizontalVelocity < 0 ? this.HitBox.X + _currentHorizontalVelocity : this.HitBox.X //X
         , _currentVerticalVelocity < 0 ? this.HitBox.Y + _currentVerticalVelocity : this.HitBox.Y //Y
         , this.HitBox.Width + Math.Abs(_currentHorizontalVelocity)//width
         , this.HitBox.Height + Math.Abs(_currentVerticalVelocity));//height

            var collisions = this.CheckCollision(areaToCheck, true);

            var horizontalTile = _currentHorizontalVelocity < 0 ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            var verticalTile = _currentVerticalVelocity < 0 ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);

            if (horizontalTile != null)
            {
                int collisionXPos = _currentHorizontalVelocity < 0 ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(collisionXPos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                DecelerateHorizontalMovement();
            }
            else
            {
                Rectangle areaToCheckToKillKeen = new Rectangle(
                    _currentHorizontalVelocity < 0 ? this.HitBox.X + _currentHorizontalVelocity : this.HitBox.X,
                    this.HitBox.Y, this.HitBox.Width + Math.Abs(_currentHorizontalVelocity), this.HitBox.Height);
                this.HitBox = new Rectangle(this.HitBox.X + _currentHorizontalVelocity, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                DecelerateHorizontalMovement();
                ExecuteFreeMoveLogic(_currentHorizontalVelocity);
            }

            if (verticalTile != null)
            {
                int _collisionYPos = _currentVerticalVelocity < 0 ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                this.HitBox = new Rectangle(this.HitBox.X, _collisionYPos, this.HitBox.Width, this.HitBox.Height);

                AccelerateVerticalMovement();
                if (_currentVerticalVelocity > 0)
                {
                    _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                    _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
                    this.Slide();
                }
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                AccelerateVerticalMovement();
            }
        }

        private void AccelerateVerticalMovement()
        {
            if (_currentVerticalVelocity + GRAVITY_ACCELERATION <= MAX_VERTICAL_VELOCITY)
            {
                _currentVerticalVelocity += GRAVITY_ACCELERATION;
            }
            else
            {
                _currentVerticalVelocity = MAX_VERTICAL_VELOCITY;
            }
        }

        private void DecelerateHorizontalMovement()
        {
            if (_currentHorizontalVelocity < 0)
            {
                if (_currentHorizontalVelocity + AIR_RESISTANCE <= MIN_HORIZONTAL_VELOCITY * -1)
                {
                    _currentHorizontalVelocity += AIR_RESISTANCE;
                }
                else
                {
                    _currentHorizontalVelocity = MIN_HORIZONTAL_VELOCITY * -1;
                }
            }
            else if (_currentHorizontalVelocity > 0)
            {
                if (_currentHorizontalVelocity - AIR_RESISTANCE >= MIN_HORIZONTAL_VELOCITY)
                {
                    _currentHorizontalVelocity -= AIR_RESISTANCE;
                }
                else
                {
                    _currentHorizontalVelocity = MIN_HORIZONTAL_VELOCITY;
                }
            }
        }

        private bool IsKeenStandingOnCap()
        {
            Rectangle areaToCheck = new Rectangle(ShellCapTile.HitBox.X, ShellCapTile.HitBox.Y - 2, ShellCapTile.HitBox.Width, ShellCapTile.HitBox.Height + 1);
            bool isStanding = (_keen.MoveState == MoveState.STANDING || _keen.MoveState == MoveState.RUNNING)
               && _keen.HitBox.IntersectsWith(areaToCheck);

            return isStanding;
        }

        private void Walk()
        {
            if (this.State != GixState.WALKING)
            {
                this.State = GixState.WALKING;
            }

            if (IsNothingBeneath())
            {
                this.Fall();
                return;
            }

            if (IsOnEdge(this.Direction))
                this.Direction = this.ChangeHorizontalDirection(this.Direction);

            bool moveKeen = IsKeenStandingOnCap();

            int xOffset = _direction == Enums.Direction.LEFT ? WALK_VELOCITY * -1 : WALK_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width + WALK_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (tile != null)
            {
                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }

            this.UpdateSpriteByDelayBase(ref _currentMoveSpriteChangeDelayTick, ref _currentMoveSprite, MOVE_SPRITE_CHANGE_DELAY, UpdateSprite);
            if (_jumpVisionRange.IntersectsWith(_keen.HitBox) && (_keen.HitBox.Right < this.HitBox.Left || _keen.HitBox.Left > this.HitBox.Right))
            {
                this.Jump();
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
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null)
                {
                    SetVisionRectangle();
                    if (ShellCapTile == null)
                        SetShellCapTile();

                    UpdateShellCapTileLocation();
                    if (this.State != GixState.FALLING)
                    {
                        this.UpdateCollisionNodes(this.Direction);
                        if (this.State == GixState.JUMPING)
                        {
                            if (_currentVerticalVelocity < 0)
                            {
                                this.UpdateCollisionNodes(Enums.Direction.UP);
                            }
                            else if (_currentVerticalVelocity > 0)
                            {
                                this.UpdateCollisionNodes(Enums.Direction.DOWN);
                            }
                        }
                    }
                    else
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                }
            }
        }

        public bool DeadlyTouch
        {
            get { return _state == GixState.SLIDING; }
        }

        public void HandleHit(IProjectile projectile)
        {

        }

        public bool IsActive
        {
            get { return false; }
        }

        internal GixState State
        {
            get { return _state; }
            set
            {
                _state = value;
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            switch (_state)
            {
                case GixState.FALLING:
                case GixState.WALKING:
                    var spriteSet = _direction == Enums.Direction.LEFT ? _moveLeftSprites : _moveRightSprites;
                    if (_currentMoveSprite >= spriteSet.Length)
                    {
                        _currentMoveSprite = 0;
                    }
                    _sprite = spriteSet[_currentMoveSprite];
                    int heightDiff = _sprite.Height - this.HitBox.Height;
                    this.HitBox = new Rectangle(new Point(this.HitBox.Location.X, this.HitBox.Location.Y - heightDiff), _sprite.Size);
                    break;
                case GixState.JUMPING:
                    _sprite = _direction == Enums.Direction.LEFT
                        ? Properties.Resources.keen6_gix_jump_left
                        : Properties.Resources.keen6_gix_jump_right;
                    break;
                case GixState.SLIDING:
                case GixState.SLIDE_HIT:
                    spriteSet = _direction == Enums.Direction.LEFT ? _slideLeftSprites : _slideRightSprites;
                    if (_currentSlideSprite >= spriteSet.Length)
                    {
                        _currentSlideSprite = 0;
                    }
                    _sprite = spriteSet[_currentSlideSprite];
                    break;
            }
        }

        public Enums.Direction Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                UpdateSprite();
            }
        }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_gix_right3);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
    enum GixState
    {
        WALKING,
        JUMPING,
        SLIDING,
        FALLING,
        SLIDE_HIT
    }
}
