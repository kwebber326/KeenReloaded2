using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Mimrock : DestructibleObject, IUpdatable, ISprite, IEnemy, IGravityObject, IMoveable, IZombieBountyEnemy
    {
        private Direction _direction;
        private Enums.MoveState _moveState;
        private Image _sprite;
        private CommanderKeen _keen;

        private int _currentRunSprite;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick;

        private const int MAX_FALL_VELOCITY = 50;
        private const int INITIAL_FALL_VELOCITY = 5;
        private const int FALL_ACCELERATION = 10;
        private int _currentFallVelocity;

        private const int INITIAL_JUMP_VELOCITY = 30;
        private const int MIN_JUMP_VELOCITY = -50;
        private const int JUMP_MOVE_DECELERATION = 1;
        private const int JUMP_MOVE_MIN = 2;
        private const int JUMP_MOVE_MAX = 20;
        private int _currentJumpVelocity = INITIAL_JUMP_VELOCITY;
        private int _currentJumpMoveSpeed = JUMP_MOVE_MAX;

        private const int JUMP_DELAY = 10;
        private int _currentJumpDelay = JUMP_DELAY;
        private const int JUMP_SPRITE_CHANGE_DELAY = 2;
        private int _currentJumpSpriteChangeDelay;

        private const int VISION_LENGTH = 300;

        private const int MOVE_VELOCITY = 5;
        private bool _isBouncing;
        private const int BOUNCE_HEIGHT = 10;
        private readonly int _zIndex;
        private Image[] _runLeftSprites = new Image[]
        {
            Properties.Resources.keen4_mimrock_walk_left1,
            Properties.Resources.keen4_mimrock_walk_left2,
            Properties.Resources.keen4_mimrock_walk_left3,
            Properties.Resources.keen4_mimrock_walk_left4,
        };

        private Image[] _runRightSprites = new Image[]
        {
            Properties.Resources.keen4_mimrock_walk_right1,
            Properties.Resources.keen4_mimrock_walk_right2,
            Properties.Resources.keen4_mimrock_walk_right3,
            Properties.Resources.keen4_mimrock_walk_right4,
        };

        private int _currentStunSprite;
        private Image[] _stunSprites = new Image[]
        {
            Properties.Resources.keen4_mimrock_stun1,
            Properties.Resources.keen4_mimrock_stun2,
            Properties.Resources.keen4_mimrock_stun3,
            Properties.Resources.keen4_mimrock_stun4,
        };
        private int _currentFallImage;
        private Image[] _fallLeftImages = new Image[]
        {
            Properties.Resources.keen4_mimrock_fall_left1,
            Properties.Resources.keen4_mimrock_fall_left2
        };

        private Image[] _fallRightImages = new Image[]
        {
            Properties.Resources.keen4_mimrock_fall_right1,
            Properties.Resources.keen4_mimrock_fall_right2
        };

        public Mimrock(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            this.MoveState = Enums.MoveState.STANDING;
            this.Direction = this.GetRandomHorizontalDirection();
        }

        public override void Die()
        {
            if (this.MoveState != Enums.MoveState.STUNNED)
            {
                this.MoveState = Enums.MoveState.STUNNED;
                _sprite = Properties.Resources.keen4_mimrock_stun1;
                _currentSpriteChangeDelayTick = 0;
            }
        }

        private void SetDirectionFromKeen()
        {
            if (_keen.HitBox.X < this.HitBox.X)
            {
                this.Direction = Enums.Direction.LEFT;
            }
            else
            {
                this.Direction = Enums.Direction.RIGHT;
            }
        }

        public void Update()
        {
            this.KillIfOutSideBoundsOfMap(_collisionGrid);
            if (!this.IsDead())
                _keen = this.GetClosestPlayer();

            switch (this.MoveState)
            {
                case Enums.MoveState.STANDING:
                    CollisionObject standingTile = GetTopMostLandingTile(2);
                    if (standingTile == null)
                    {
                        this.Fall();
                        return;
                    }

                    if (_currentJumpDelay++ >= JUMP_DELAY)
                    {
                        if (!IsKeenLooking())
                        {
                            if (IsKeenCloseEnoughToJump() && !IsKeenVerticalPathObstructed())
                                this.Jump();
                            else if (IsKeenCloseEnoughToChase())
                                this.Move();
                        }
                    }
                    break;
                case Enums.MoveState.RUNNING:
                    this.Move();
                    break;
                case Enums.MoveState.JUMPING:
                    this.Jump();
                    break;
                case Enums.MoveState.FALLING:
                    this.Fall();
                    break;
                case Enums.MoveState.STUNNED:
                    standingTile = GetTopMostLandingTile(2);
                    if (standingTile == null)
                    {
                        this.Fall();
                        return;
                    }
                    UpdateSpriteByDelay();
                    break;
            }

        }

        private bool IsKeenVerticalPathObstructed()
        {
            Rectangle areaToCheck = new Rectangle(_keen.HitBox.Left, this.HitBox.Top, _keen.HitBox.Width, _keen.HitBox.Top - this.HitBox.Top);
            var collisions = this.CheckCollision(areaToCheck, true);
            return collisions.Any();
        }

        private void UpdateSpriteByDelay()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                UpdateSprite();
            }
        }

        private bool IsKeenCloseEnoughToChase()
        {
            if (_keen.HitBox.Left < this.HitBox.Right && _keen.HitBox.Left > this.HitBox.Left)
            {
                return false;
            }

            if (_keen.HitBox.X < this.HitBox.X)
            {
                bool isCloseEnoughLeft = this.HitBox.X - _keen.HitBox.Right <= VISION_LENGTH;
                return isCloseEnoughLeft;
            }

            bool isCloseEnoughRight = _keen.HitBox.Left - this.HitBox.Right <= VISION_LENGTH;
            return isCloseEnoughRight;
        }

        private bool IsKeenCloseEnoughToJump()
        {
            if (this.CollidesWith(_keen))
            {
                return false;
            }

            if (_keen.HitBox.Bottom >= this.HitBox.Top - INITIAL_JUMP_VELOCITY - 10)
            {
                if (_keen.HitBox.X < this.HitBox.X)
                {
                    bool isCloseEnoughLeft = this.HitBox.X - _keen.HitBox.Right <= VISION_LENGTH / 2;
                    return isCloseEnoughLeft;
                }

                bool isCloseEnoughRight = _keen.HitBox.Left - this.HitBox.Right <= VISION_LENGTH / 2;
                return isCloseEnoughRight;
            }
            return false;
        }

        private bool IsKeenLooking()
        {
            if (_keen.HitBox.Right >= this.HitBox.Right && _keen.Direction == Enums.Direction.LEFT)
                return true;
            if (_keen.HitBox.Left <= this.HitBox.Left && _keen.Direction == Enums.Direction.RIGHT)
                return true;

            return false;
        }

        public bool DeadlyTouch
        {
            get { return this.MoveState == Enums.MoveState.JUMPING; }
        }

        public void HandleHit(IProjectile trajectory)
        {
            this.TakeDamage(trajectory);
        }

        public bool IsActive
        {
            get { return this.MoveState == Enums.MoveState.RUNNING || this.MoveState == Enums.MoveState.JUMPING; }
        }

        public void Jump()
        {
            if (this.MoveState != Enums.MoveState.JUMPING)
            {
                this.MoveState = Enums.MoveState.JUMPING;
                _currentJumpDelay = 0;
                SetDirectionFromKeen();
            }

            int xOffset = this.Direction == Enums.Direction.LEFT ? _currentJumpMoveSpeed * -1 : _currentJumpMoveSpeed;
            int xLoc = this.Direction == Enums.Direction.LEFT ? this.HitBox.X - _currentJumpMoveSpeed : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xLoc, this.HitBox.Y - _currentJumpVelocity, this.HitBox.Width + _currentJumpMoveSpeed, this.HitBox.Height + _currentJumpVelocity);
            var collisions = this.CheckCollision(areaToCheck, true);

            CollisionObject horizontalTile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (_currentJumpVelocity >= 0)
            {
                CollisionObject tile = GetCeilingTile(collisions);
                if (tile != null)
                {
                    _currentJumpVelocity = -1;
                    if (horizontalTile != null)
                    {
                        xLoc = this.Direction == Enums.Direction.LEFT ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                    }
                    this.HitBox = new Rectangle(xLoc, tile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    if (horizontalTile != null)
                    {
                        xLoc = this.Direction == Enums.Direction.LEFT ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                    }
                    else if (this.Direction == Direction.RIGHT)
                    {
                        xLoc += _currentJumpMoveSpeed;
                    }
                    this.HitBox = new Rectangle(xLoc, this.HitBox.Y - _currentJumpVelocity, this.HitBox.Width, this.HitBox.Height);
                }
            }
            else
            {
                if (_currentJumpSpriteChangeDelay++ == JUMP_SPRITE_CHANGE_DELAY)
                {
                    _currentJumpSpriteChangeDelay = 0;
                    var spriteSet = this.Direction == Enums.Direction.LEFT ? _fallLeftImages : _fallRightImages;
                    if (_currentFallImage >= spriteSet.Length)
                    {
                        _currentFallImage = 0;
                    }
                    _sprite = spriteSet[_currentFallImage++];
                }
                if (horizontalTile != null)
                {
                    xLoc = this.Direction == Enums.Direction.LEFT ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                }
                else
                {
                    if (this.Direction == Direction.RIGHT)
                    {
                        xLoc += _currentJumpMoveSpeed;
                    }
                }
                CollisionObject landingTile = GetTopMostLandingTile(Math.Abs(_currentJumpVelocity));
                if (landingTile != null)
                {

                    this.HitBox = new Rectangle(xLoc, landingTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                    if (_isBouncing)
                    {
                        this.MoveState = Enums.MoveState.STANDING;
                        _currentJumpVelocity = INITIAL_JUMP_VELOCITY;
                        _currentJumpMoveSpeed = JUMP_MOVE_MAX;
                        _currentJumpDelay = 0;
                        _isBouncing = false;
                    }
                    else
                    {
                        _isBouncing = true;
                        _currentJumpVelocity = BOUNCE_HEIGHT;
                        _currentJumpDelay = 0;
                    }
                }
                else
                {
                    this.HitBox = new Rectangle(xLoc, this.HitBox.Y - _currentJumpVelocity, this.HitBox.Width, this.HitBox.Height);
                }
            }

            if (_currentJumpMoveSpeed >= JUMP_MOVE_MIN - JUMP_MOVE_DECELERATION)
                _currentJumpMoveSpeed -= JUMP_MOVE_DECELERATION;

            if (_currentJumpVelocity - MOVE_VELOCITY > MIN_JUMP_VELOCITY)
                _currentJumpVelocity -= MOVE_VELOCITY;

            KillKeenIfColliding();
        }

        public bool CanJump
        {
            get { return true; }
        }

        public void Fall()
        {
            if (this.MoveState != Enums.MoveState.FALLING && !this.IsDead())
            {
                this.MoveState = Enums.MoveState.FALLING;
            }

            CollisionObject landingTile = GetTopMostLandingTile(_currentFallVelocity);
            if (landingTile != null)
            {
                this.MoveState = this.IsDead() ? MoveState.STUNNED : Enums.MoveState.STANDING;
                this.HitBox = new Rectangle(this.HitBox.X, landingTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                _currentFallVelocity = INITIAL_FALL_VELOCITY;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentFallVelocity, this.HitBox.Width, this.HitBox.Height);
                if (_currentFallVelocity + FALL_ACCELERATION <= MAX_FALL_VELOCITY)
                    _currentFallVelocity += FALL_ACCELERATION;
            }
        }

        public void Move()
        {
            if (this.MoveState != Enums.MoveState.RUNNING)
            {
                this.MoveState = Enums.MoveState.RUNNING;
                _currentJumpDelay = 0;
            }

            SetDirectionFromKeen();

            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                UpdateSprite();
            }

            CollisionObject landingTile = GetTopMostLandingTile(INITIAL_FALL_VELOCITY);
            if (landingTile == null)
            {
                this.Fall();
                return;
            }
            bool isKeenLooking = IsKeenLooking();

            if (isKeenLooking || !IsKeenCloseEnoughToChase())
            {
                this.MoveState = Enums.MoveState.STANDING;
                return;
            }
            else if (!isKeenLooking && IsKeenCloseEnoughToJump() && !IsKeenVerticalPathObstructed())
            {
                this.Jump();
                return;
            }

            int xOffset = this.Direction == Enums.Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;
            Rectangle areaToCheck = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height - 5);
            var collisions = this.CheckCollision(areaToCheck, true);

            CollisionObject tile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (tile != null)
            {
                int xLoc = this.Direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xLoc, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            else if (!IsOnEdge(this.Direction, xOffset))
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                SwitchDirection();
            };
        }

        private void KillKeenIfColliding()
        {
            if (this.CollidesWith(_keen))
            {
                _keen.Die();
            }
        }

        private void SwitchDirection()
        {
            this.Direction = this.Direction == Enums.Direction.LEFT ? Enums.Direction.RIGHT : Enums.Direction.LEFT;
        }

        private List<CollisionObject> GetTilesBelow(int xOffset)
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Bottom, this.HitBox.Width, 10);
            var collisions = this.CheckCollision(areaToCheck, true);
            var tiles = collisions.ToList();
            return tiles;
        }

        public void Stop()
        {
            if (this.MoveState != Enums.MoveState.STANDING)
            {
                this.MoveState = Enums.MoveState.STANDING;
            }
        }

        public MoveState MoveState
        {
            get
            {
                return _moveState;
            }
            set
            {
                _moveState = value;
                UpdateSprite();
            }
        }

        public Direction Direction
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

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (this.HitBox != null && _collisionGrid != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(this.Direction);
                    if (this.MoveState == Enums.MoveState.JUMPING)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.UP);
                    }
                    else if (this.MoveState == Enums.MoveState.FALLING)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                }
            }
        }

        public PointItemType PointItem => PointItemType.KEEN4_JAWBREAKER;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        private void UpdateSprite()
        {
            switch (_moveState)
            {
                case Enums.MoveState.STANDING:
                    _sprite = Properties.Resources.keen4_mimrock_wait;
                    break;
                case Enums.MoveState.RUNNING:
                    var spriteSet = this.Direction == Enums.Direction.LEFT ? _runLeftSprites : _runRightSprites;
                    if (++_currentRunSprite >= spriteSet.Length)
                    {
                        _currentRunSprite = 0;
                    }
                    _sprite = spriteSet[_currentRunSprite];
                    break;
                case Enums.MoveState.STUNNED:
                    if (++_currentStunSprite >= _stunSprites.Length)
                    {
                        _currentStunSprite = 1;
                    }
                    var oldSprite = _sprite;
                    _sprite = _stunSprites[_currentStunSprite];
                    var heightDiff = oldSprite.Height - _sprite.Height;
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + heightDiff, _sprite.Width, _sprite.Height);
                    break;
                case Enums.MoveState.JUMPING:
                    _sprite = this.Direction == Enums.Direction.LEFT ? Properties.Resources.keen4_mimrock_jump_left : Properties.Resources.keen4_mimrock_jump_right;
                    break;
            }

        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_mimrock_wait);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
