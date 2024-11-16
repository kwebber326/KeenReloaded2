using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Bounder : DestructibleObject, IEnemy, IGravityObject, IUpdatable, ISprite, IZombieBountyEnemy
    {
        private Image _sprite;
        private bool _isDead;
        private const int FALL_ACCELERATION = 10;
        private const int MAX_FALL_VELOCITY = 100;
        private const int MAX_JUMP_VELOCITY = 30;
        private const int INITIAL_HORIZONTAL_VELOCITY = 20;
        private const int MIN_HORIZONTAL_VELOCITY = 5;
        private const int SPEED_DECELERATION = 2;
        private int _verticalVelocity;
        private int _horizontalVelocity = INITIAL_HORIZONTAL_VELOCITY;
        private BounderState _state;
        private Enums.Direction _direction;
        private CommanderKeen _keen;
        private Rectangle _vision;
        private const int VISION_WIDTH = 200;
        private const int VISION_HEIGHT = 200;
        private const int VERTICAL_OFFSET_FOR_HEAD_TILE_STUNNED_STATE = 12;

        private const int BOUNCES_TIL_MOVE = 2;
        private int _currentBounceTick;

        private const int BOUNCES_TIL_STOP = 1;
        private readonly int _zIndex;
        private int _currentBounceSprite;
        private Image[] _bouncingSprites = new Image[]
        {
            Properties.Resources.keen4_bounder1,
            Properties.Resources.keen4_bounder2
        };

        private int _currentBounceLeftSprite;
        private Image[] _bouncingLeftSprites = new Image[]
        {
            Properties.Resources.keen4_bounder_bounceleft1,
            Properties.Resources.keen4_bounder_bounceleft2
        };
        private int _currentBounceRightSprite;
        private Image[] _bouncingRightSprites = new Image[]
        {
            Properties.Resources.keen4_bounder_bounceright1,
            Properties.Resources.keen4_bounder_bounceright2
        };

        private int _currentStunnedSprite;
        private Image[] _stunnedSprites = new Image[]
        {
            Properties.Resources.keen4_bounder_stun1,
            Properties.Resources.keen4_bounder_stun2,
            Properties.Resources.keen4_bounder_stun3,
            Properties.Resources.keen4_bounder_stun4
        };

        private MovingPlatformTile _headTile;
        private bool _spriteAdjustedForStunnedState;

        public Bounder(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            this.Health = 1;
            _sprite = _bouncingSprites[_currentBounceSprite];
            this.State = BounderState.LOOK_BOUNCING;
            //SetDirectionFromKeenLocation();
            SetVision();
            if (_collisionGrid != null)
            {
                Rectangle platformRect = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, 16);
                _headTile = new MovingPlatformTile(_collisionGrid, platformRect, null, _zIndex);
            }
        }


        protected virtual bool IsKeenStandingOnHead()
        {
            bool intersectsX = _keen.HitBox.Left > this.HitBox.Left && _keen.HitBox.Left < this.HitBox.Right;
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

        private void SetVision()
        {
            _vision = new Rectangle(this.HitBox.X - VISION_WIDTH, this.HitBox.Y - VISION_HEIGHT,
                this.HitBox.Width + (VISION_WIDTH * 2),
                this.HitBox.Height + (VISION_HEIGHT * 2));
        }

        public MovingPlatformTile HeadTile
        {
            get
            {
                return _headTile;
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
                    this.UpdateCollisionNodes(this.Direction);
                    if (_verticalVelocity < 0)
                    {
                        this.UpdateCollisionNodes(Direction.UP);
                    }
                    else
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                    SetVision();
                    if (_headTile != null)
                    {
                        if (this.State != BounderState.STUNNED)
                        {
                            _headTile.Move(this.HitBox.Location);
                        }
                        else
                        {
                            _headTile.Move(new Point(this.HitBox.X, this.HitBox.Y + VERTICAL_OFFSET_FOR_HEAD_TILE_STUNNED_STATE));
                        }
                    }
                }
            }
        }

        public override void Die()
        {
            _isDead = true;
            if (this.State != BounderState.STUNNED)
            {
                this.State = BounderState.STUNNED;
            }
        }

        public bool DeadlyTouch
        {
            get { return false; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return !_isDead; }
        }

        public void Jump()
        {
            throw new NotImplementedException();
        }

        Direction Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                UpdateSprite();
                if ((_direction == Enums.Direction.LEFT && _horizontalVelocity > 0)
                    || _direction == Enums.Direction.RIGHT && _horizontalVelocity < 0)
                {
                    _horizontalVelocity *= -1;
                }

            }
        }

        BounderState State
        {
            get
            {
                return _state;
            }
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
                case BounderState.LOOK_BOUNCING:
                    if (_currentBounceSprite >= _bouncingSprites.Length)
                        _currentBounceSprite = 0;
                    _sprite = _bouncingSprites[_currentBounceSprite];
                    break;
                case BounderState.STUNNED:
                    if (_currentStunnedSprite >= _stunnedSprites.Length)
                        _currentStunnedSprite = 0;

                    _sprite = _stunnedSprites[_currentStunnedSprite];
                    break;
                case BounderState.CHASING:
                    if (this.Direction == Enums.Direction.LEFT)
                    {
                        if (_currentBounceLeftSprite >= _bouncingLeftSprites.Length)
                            _currentBounceLeftSprite = 0;

                        _sprite = _bouncingLeftSprites[_currentBounceLeftSprite];
                    }
                    else
                    {
                        if (_currentBounceRightSprite >= _bouncingRightSprites.Length)
                            _currentBounceRightSprite = 0;

                        _sprite = _bouncingRightSprites[_currentBounceRightSprite];
                    }
                    break;
            }
        }

        public bool CanJump
        {
            get { return _state != BounderState.STUNNED; }
        }

        public void Fall()
        {
            Rectangle areaToCheck = new Rectangle(
                _horizontalVelocity < 0 ? this.HitBox.X + _horizontalVelocity : this.HitBox.X, //X
                _verticalVelocity < 0 ? this.HitBox.Y + _verticalVelocity : this.HitBox.Y, //Y
                this.HitBox.Width + Math.Abs(_horizontalVelocity), //Width
                this.HitBox.Height + Math.Abs(_verticalVelocity)); //Height
            var collisions = this.CheckCollision(areaToCheck, true);

            CollisionObject tile = _horizontalVelocity != 0 ? this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions) : null;

            if (_verticalVelocity < 0)
            {
                var ceilingTile = GetCeilingTile(collisions);
                int xPos = this.HitBox.X + _horizontalVelocity;
                if (tile != null)
                {
                    if (this.Direction == Enums.Direction.RIGHT)
                    {
                        xPos = tile.HitBox.Left - this.HitBox.Width - 1;
                    }
                    else
                    {
                        xPos = tile.HitBox.Right + 1;
                    }
                    this.Direction = this.ChangeHorizontalDirection(this.Direction);
                }

                if (ceilingTile != null)
                {
                    this.HitBox = new Rectangle(xPos, ceilingTile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                    _verticalVelocity *= -1;
                }
                else
                {
                    MoveByVelocity(xPos);
                }
            }
            else
            {
                var landingTile = this.GetTopMostLandingTile(_verticalVelocity);
                int xPos = this.HitBox.X + _horizontalVelocity;
                if (tile != null)
                {
                    if (this.Direction == Enums.Direction.RIGHT)
                    {
                        xPos = tile.HitBox.Left - this.HitBox.Width - 1;
                    }
                    else
                    {
                        xPos = tile.HitBox.Right + 1;
                    }
                    this.Direction = this.ChangeHorizontalDirection(this.Direction);
                }
                if (landingTile != null)
                {
                    this.HitBox = new Rectangle(xPos, landingTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                    if (_state == BounderState.STUNNED)
                    {
                        _verticalVelocity = 0;
                    }
                    else
                    {
                        _currentBounceTick++;
                        _verticalVelocity = MAX_JUMP_VELOCITY * -1;//bounce back up if not stunned
                    }
                }
                else
                {
                    MoveByVelocity(xPos);
                }
            }

            MoveKeen(areaToCheck);

            //if (_state == BounderState.STUNNED)
            //{
            //    AdjustSpriteLocationForStunnedState();
            //}
        }

        private void MoveKeen(Rectangle areaToCheck)
        {
            if (_headTile.Keen == null && _keen.HitBox.IntersectsWith(this.HitBox) && _keen.MoveState != MoveState.ON_POLE && _keen.MoveState != MoveState.HANGING && _keen.MoveState != MoveState.CLIMBING)
            {
                _keen.GetMovedHorizontally(this, this.Direction, Math.Abs(_horizontalVelocity));
            }
        }

        private void StandKeenOnHead()
        {
            int velocity = _state == BounderState.LOOK_BOUNCING ? 0 : _horizontalVelocity;
            _keen.MoveKeenToPosition(new Point(_keen.HitBox.X + velocity, this.HitBox.Top - _keen.HitBox.Height - 1), this);
        }


        private void MoveByVelocity(int xPos)
        {
            this.HitBox = new Rectangle(xPos, this.HitBox.Y + _verticalVelocity, this.HitBox.Width, this.HitBox.Height);
            if (_verticalVelocity + FALL_ACCELERATION <= MAX_FALL_VELOCITY)
            {
                _verticalVelocity += FALL_ACCELERATION;
            }
            else
            {
                _verticalVelocity = MAX_FALL_VELOCITY;
            }

            var horizontalSpeed = Math.Abs(_horizontalVelocity);

            if (horizontalSpeed - SPEED_DECELERATION >= MIN_HORIZONTAL_VELOCITY)
            {
                horizontalSpeed -= SPEED_DECELERATION;
            }
            else
            {
                horizontalSpeed = MIN_HORIZONTAL_VELOCITY;
            }
            _horizontalVelocity = _horizontalVelocity < 0 ? horizontalSpeed * -1 : horizontalSpeed;
        }

        public void Update()
        {
            this.KillIfOutSideBoundsOfMap(_collisionGrid);
            switch (_state)
            {
                case BounderState.CHASING:
                case BounderState.LOOK_BOUNCING:
                    _keen = this.GetClosestPlayer();
                    UpdateBounceState();
                    break;
                case BounderState.STUNNED:
                    UpdateStunnedState();
                    break;
            }
        }

        private void UpdateStunnedState()
        {
            bool isNothingBeneath = this.IsNothingBeneath();
            if (_currentStunnedSprite == 0 && !isNothingBeneath)
            {
                _currentStunnedSprite++;
                UpdateSprite();
            }

          
            if (++_currentStunnedSprite == _stunnedSprites.Length)
            {
                _currentStunnedSprite = 1;
            }
            UpdateSprite();

            if (!_spriteAdjustedForStunnedState)
            {
                AdjustSpriteLocationForStunnedState();
                _spriteAdjustedForStunnedState = true;
            }

            if (isNothingBeneath)
            {
                this.Fall();
            }
        }

        private void AdjustSpriteLocationForStunnedState()
        {
            int verticalOffset = VERTICAL_OFFSET_FOR_HEAD_TILE_STUNNED_STATE;
            this.HitBox = new Rectangle(new Point(this.HitBox.X, this.HitBox.Y - verticalOffset), new Size(_sprite.Width, _sprite.Height));
        }

        private void UpdateBounceState()
        {
            if (_state == BounderState.LOOK_BOUNCING)
            {
                _horizontalVelocity = 0;
                if (_currentBounceTick == BOUNCES_TIL_MOVE)
                {
                    _currentBounceTick = 0;
                    this.State = BounderState.CHASING;
                    _horizontalVelocity = this.Direction == Enums.Direction.LEFT ? INITIAL_HORIZONTAL_VELOCITY * -1 : INITIAL_HORIZONTAL_VELOCITY;
                    SetDirectionFromKeenLocation();
                }
            }
            else if (_state == BounderState.CHASING)
            {
                bool nothingBeneath = IsNothingBeneath();
                if (!nothingBeneath)
                    SetDirectionFromKeenLocation();

                if (_headTile.Keen == null)
                {
                    if (_currentBounceTick == BOUNCES_TIL_STOP)
                    {
                        _currentBounceTick = 0;
                        this.State = BounderState.LOOK_BOUNCING;
                        _horizontalVelocity = 0;
                    }
                }
                else
                {
                    _currentBounceTick = 0;
                    if (!nothingBeneath)
                    {
                        _horizontalVelocity = this.Direction == Enums.Direction.LEFT ? INITIAL_HORIZONTAL_VELOCITY * -1 : INITIAL_HORIZONTAL_VELOCITY;
                    }
                }
            }
            this.Fall();
        }

        protected void SetHorizontalDirectionFromKeenLocation(CommanderKeen keen, ref Direction direction)
        {

            if (keen == null)
                throw new ArgumentNullException("keen cannot be null");

            if (keen.HitBox.Left + keen.HitBox.Width / 2 < this.HitBox.Left + this.HitBox.Width / 2)
            {
                direction = Direction.LEFT;
            }
            else if (keen.HitBox.Left + keen.HitBox.Width / 2 > this.HitBox.Left + this.HitBox.Width / 2)
            {
                direction = Direction.RIGHT;
            }
        }

        private void SetDirectionFromKeenLocation()
        {
            var direction = this.Direction;
            SetHorizontalDirectionFromKeenLocation(_keen, ref direction);
            this.Direction = direction;
        }

        public PointItemType PointItem => PointItemType.KEEN4_SHIKADI_SODA;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_bounder1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum BounderState
    {
        LOOK_BOUNCING,
        CHASING,
        STUNNED
    }
}
