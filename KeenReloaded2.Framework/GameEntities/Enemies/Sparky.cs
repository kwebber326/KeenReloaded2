using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Sparky : DestructibleObject, IUpdatable, ISprite, IEnemy, IZombieBountyEnemy
    {
        private CommanderKeen _keen;
        private Image _sprite;
        private SparkyState _state;

        private const int FALL_VELOCITY = 30;

        private const int LOOK_SPRITE_CHANGE_DELAY = 2;
        private int _currentLookSpriteChangeDelayTick;

        private const int LOOKS_UNTIL_MOVE = 2;
        private int _currentLook;

        private const int WALK_VELOCITY = 7;
        private const int WALK_SPRITE_CHANGE_DELAY = 2;
        private int _currentWalkSpriteChangeDelayTick;

        private const int CHARGE_VELOCITY = 25;
        private const int CHARGE_SPRITE_CHANGE_DELAY = 0;
        private const int CHARGE_DELAY = 15;
        private int _currentChargeDelayTick;
        private int _currentChargeSpriteChangeDelayTick;

        private const int STUNNED_SPRITE_CHANGE_DELAY = 1;
        private int _currentStunnedSpriteChangeDelayTick;

        private const int CHANGE_DIRECTION_CHANCE = 80; //set these to lower values for higher chance
        private const int CHARGE_CHANCE = 50;
        private const int VISION_DISTANCE_X = 400;
        private const int VISION_DISTANCE_Y = 200;

        private int _currentLookSprite;
        private Image[] _lookSprites = new Image[]
        {
            Properties.Resources.keen5_sparky_change_direction1,
            Properties.Resources.keen5_sparky_change_direction2,
            Properties.Resources.keen5_sparky_change_direction3
        };

        private int _currentMoveSprite;
        private Image[] _moveLeftSprites = new Image[]
        {
            Properties.Resources.keen5_sparky_left1,
            Properties.Resources.keen5_sparky_left2,
            Properties.Resources.keen5_sparky_left3,
            Properties.Resources.keen5_sparky_left4
        };

        private Image[] _moveRightSprites = new Image[]
        {
            Properties.Resources.keen5_sparky_right1,
            Properties.Resources.keen5_sparky_right2,
            Properties.Resources.keen5_sparky_right3,
            Properties.Resources.keen5_sparky_right4
        };

        private int _currentStunnedSprite;
        private Image[] _stunnedSprites = new Image[]
        {
            Properties.Resources.keen5_sparky_stunned1,
            Properties.Resources.keen5_sparky_stunned2,
            Properties.Resources.keen5_sparky_stunned3,
            Properties.Resources.keen5_sparky_stunned4
        };
        private Enums.Direction _direction;

        public Sparky(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            this.Health = 1;
            this.Direction = this.GetRandomHorizontalDirection();
            this.State = SparkyState.FALLING;
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
                if (_collisionGrid != null && _collisionGrid != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(this.Direction);
                    if (_state == SparkyState.FALLING)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                }
            }
        }

        private readonly int _zIndex;

        public override void Die()
        {
            this.UpdateStunnedState();
        }

        public void Update()
        {
            if (_state != SparkyState.FALLING && IsNothingBeneath())
            {
                this.Fall();
            }

            if (_state != SparkyState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }

            switch (_state)
            {
                case SparkyState.FALLING:
                    this.Fall();
                    break;
                case SparkyState.LOOKING:
                    this.Look();
                    break;
                case SparkyState.WALKING:
                    this.Walk();
                    break;
                case SparkyState.CHANGING_DIRECTION:
                    this.ChangeDirection();
                    break;
                case SparkyState.CHARGE_LOADING:
                    this.LoadCharge();
                    break;
                case SparkyState.CHARGING:
                    this.Charge();
                    break;
                case SparkyState.STUNNED:
                    this.UpdateStunnedState();
                    break;
            }
        }

        private void ChangeDirection()
        {
            if (this.State != SparkyState.CHANGING_DIRECTION)
            {
                this.State = SparkyState.CHANGING_DIRECTION;
                SetCurrentLookSpriteIndexBasedOnDirection();
                this.Direction = ChangeHorizontalDirection(this.Direction);
            }

            if (this.Direction == Enums.Direction.LEFT)
            {
                if (_currentLookSprite > 0)
                {
                    if (_currentLookSpriteChangeDelayTick++ == LOOK_SPRITE_CHANGE_DELAY)
                    {
                        _currentLookSpriteChangeDelayTick = 0;
                        _currentLookSprite--;
                        UpdateSprite();
                    }
                }
                else
                {
                    this.Walk();
                }
            }
            else
            {
                if (_currentLookSprite < _lookSprites.Length - 1)
                {
                    if (_currentLookSpriteChangeDelayTick++ == LOOK_SPRITE_CHANGE_DELAY)
                    {
                        _currentLookSpriteChangeDelayTick = 0;
                        _currentLookSprite++;
                        UpdateSprite();
                    }
                }
                else
                {
                    this.Walk();
                }
            }
        }

        private void LoadCharge()
        {
            if (this.State != SparkyState.CHARGE_LOADING)
            {
                this.State = SparkyState.CHARGE_LOADING;
                _currentChargeDelayTick = 0;
            }

            UpdateSpriteByDelay(ref _currentChargeSpriteChangeDelayTick, ref _currentMoveSprite, CHARGE_SPRITE_CHANGE_DELAY);

            if (_currentChargeDelayTick++ == CHARGE_DELAY)
            {
                this.Charge();
            }
        }

        private void UpdateSpriteByDelay(ref int delayTicker, ref int spriteIndex, int delayThreshold)
        {
            if (delayTicker++ == delayThreshold)
            {
                delayTicker = 0;
                spriteIndex++;
                UpdateSprite();
            }
        }

        private void Fall()
        {
            if (this.State != SparkyState.FALLING)
            {
                this.State = SparkyState.FALLING;
            }

            int yOffset = FALL_VELOCITY;
            var tile = this.GetTopMostLandingTile(FALL_VELOCITY);
            if (tile != null)
            {
                yOffset = Math.Abs(this.HitBox.Y - (tile.HitBox.Top - this.HitBox.Height - 1));
                KillKeenBasedOnFall(yOffset);
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                if (!this.IsDead())
                {
                    this.Walk();
                }
                else
                {
                    UpdateStunnedState();
                }
            }
            else
            {
                KillKeenBasedOnFall(yOffset);
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + FALL_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            }
        }

        private void KillKeenBasedOnFall(int yOffset)
        {
            if (!this.IsDead())
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + yOffset);
                if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    _keen.Die();
                }
            }
        }

        private void KillKeenByXOffset(int xOffset)
        {
            if (!this.IsDead())
            {
                Rectangle areaToCheck = new Rectangle(xOffset < 0 ? this.HitBox.X + xOffset : this.HitBox.X, this.HitBox.Y, this.HitBox.Width + Math.Abs(xOffset), this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    _keen.Die();
                }
            }
        }

        private void UpdateStunnedState()
        {
            if (this.State != SparkyState.STUNNED)
            {
                this.State = SparkyState.STUNNED;
                _sprite = Properties.Resources.keen5_sparky_stunned1;
                return;
            }
            var spriteIndex = _currentStunnedSprite;
            this.UpdateSpriteByDelay(ref _currentChargeSpriteChangeDelayTick, ref _currentStunnedSprite, STUNNED_SPRITE_CHANGE_DELAY);
            if (_currentStunnedSprite != spriteIndex)
            {
                var image = _stunnedSprites[_currentStunnedSprite];
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - (image.Size.Height - this.HitBox.Height), image.Width, image.Height);
            }
        }

        private void Charge()
        {
            if (this.State != SparkyState.CHARGING)
            {
                this.State = SparkyState.CHARGING;
            }

            if (IsOnEdge(this.Direction, 3))
            {
                this.ChangeDirection();
                return;
            }

            this.Move(CHARGE_VELOCITY);
        }

        private void Walk()
        {
            if (this.State != SparkyState.WALKING)
            {
                this.State = SparkyState.WALKING;
            }

            if (IsOnEdge(this.Direction, 20))
            {
                this.ChangeDirection();
                return;
            }

            var direction = this.Direction;
            SetHorizontalDirectionFromKeenLocation(_keen, ref direction);
            if (direction == this.Direction && Math.Abs(this.HitBox.X - _keen.HitBox.X) <= VISION_DISTANCE_X
                && Math.Abs(this.HitBox.Y - _keen.HitBox.Y) <= VISION_DISTANCE_Y)
            {
                int chargeVal = _random.Next(1, CHARGE_CHANCE + 1);
                if (chargeVal == CHARGE_CHANCE)
                {
                    this.LoadCharge();
                    return;
                }
            }
            else
            {
                int changeDirectionVal = _random.Next(1, CHANGE_DIRECTION_CHANCE + 1);
                if (changeDirectionVal == CHANGE_DIRECTION_CHANCE)
                {
                    this.ChangeDirection();
                    return;
                }
            }

            Move(WALK_VELOCITY);
        }

        private void Move(int velocity)
        {
            int xOffset = this.Direction == Enums.Direction.LEFT ? velocity * -1 : velocity;
            int xPos = this.Direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xPos, this.HitBox.Y, this.HitBox.Width + velocity, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);

            if (tile != null)
            {
                if (this.Direction == Enums.Direction.LEFT)
                {
                    xOffset = this.HitBox.X - (tile.HitBox.Right + 1);
                    this.HitBox = new Rectangle(tile.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    xOffset = tile.HitBox.Left - this.HitBox.Width - 1 - this.HitBox.X;
                    this.HitBox = new Rectangle(tile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
                KillKeenByXOffset(xOffset);
                this.ChangeDirection();
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.UpdateSpriteByDelay(ref _currentWalkSpriteChangeDelayTick, ref _currentMoveSprite, WALK_SPRITE_CHANGE_DELAY);
                KillKeenByXOffset(xOffset);
            }
        }

        private void Look()
        {
            if (this.State != SparkyState.LOOKING)
            {
                this.State = SparkyState.LOOKING;
                SetCurrentLookSpriteIndexBasedOnDirection();
            }

            if (_currentLook >= LOOKS_UNTIL_MOVE)
            {
                _currentLook = 0;
                this.Walk();
                return;
            }

            var direction = this.Direction;
            SetHorizontalDirectionFromKeenLocation(_keen, ref direction);

            if (this.Direction == Enums.Direction.LEFT)
            {
                if (_currentLookSprite > 0)
                {
                    if (_currentLookSpriteChangeDelayTick++ == LOOK_SPRITE_CHANGE_DELAY)
                    {
                        _currentLookSpriteChangeDelayTick = 0;
                        _currentLookSprite--;
                        UpdateSprite();
                    }
                }
                else if (direction == Enums.Direction.LEFT)
                {
                    this.Walk();
                }
                else if (_currentLookSprite <= 0)
                {
                    _currentLook++;
                }
            }
            else
            {
                if (_currentLookSprite < _lookSprites.Length - 1)
                {
                    if (_currentLookSpriteChangeDelayTick++ == LOOK_SPRITE_CHANGE_DELAY)
                    {
                        _currentLookSpriteChangeDelayTick = 0;
                        _currentLookSprite++;
                        UpdateSprite();
                    }
                }
                else if (direction == Enums.Direction.RIGHT)
                {
                    this.Walk();
                }
                else if (_currentLookSprite >= _lookSprites.Length)
                {
                    _currentLook++;
                }
            }
        }

        private void SetCurrentLookSpriteIndexBasedOnDirection()
        {
            _currentLookSprite = this.Direction == Enums.Direction.LEFT ? 0 : _lookSprites.Length - 1;
        }

        SparkyState State
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
                case SparkyState.LOOKING:
                case SparkyState.CHANGING_DIRECTION:
                    if (_currentLookSprite >= _lookSprites.Length || _currentLookSprite < 0)
                    {
                        _currentLookSprite = 0;
                    }
                    _sprite = _lookSprites[_currentLookSprite];
                    break;
                case SparkyState.CHARGING:
                case SparkyState.WALKING:
                case SparkyState.CHARGE_LOADING:
                case SparkyState.FALLING:
                    if (!this.IsDead())
                    {
                        var spriteSet = this.Direction == Enums.Direction.LEFT ? _moveLeftSprites : _moveRightSprites;
                        if (_currentMoveSprite >= spriteSet.Length || _currentMoveSprite < 0)
                        {
                            _currentMoveSprite = 0;
                        }

                        _sprite = spriteSet[_currentMoveSprite];
                    }
                    break;
                case SparkyState.STUNNED:
                    if (_currentStunnedSprite >= _stunnedSprites.Length || _currentStunnedSprite < 0)
                    {
                        _currentStunnedSprite = 1;
                    }
                    _sprite = _stunnedSprites[_currentStunnedSprite];
                    break;
            }
        }

        public bool DeadlyTouch
        {
            get { return !this.IsDead(); }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return !this.IsDead(); }
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

        public PointItemType PointItem => PointItemType.KEEN5_MARSHMALLOW;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_sparky_left4);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
    enum SparkyState
    {
        FALLING,
        LOOKING,
        WALKING,
        CHANGING_DIRECTION,
        CHARGE_LOADING,
        CHARGING,
        STUNNED
    }
}
