using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Bloog : DestructibleObject, IUpdatable, ISprite, IEnemy, IZombieBountyEnemy
    {
        protected Image _sprite;
        protected BloogState _state;
        protected Enums.Direction _direction;
        protected Image[] _walkRightSprites, _walkLeftSprites, _stunnedSprites;
        protected bool _holdsItem;
        protected int _currentStunnedSprite;
        protected const int STUNNED_SPRITE_CHANGE_DELAY = 1;
        protected int _currentStunnedSpriteChangeDelayTick;
        protected const int MOVE_SPRITE_CHANGE_DELAY = 3;
        protected int _currentMoveSpriteChangeDelayTick;
        protected int _currentMoveSprite;
        protected const int FALL_VELOCITY = 30;
        protected const int MOVE_VELOCITY = 5;
        protected const int TICKS_BEFORE_CHASING_KEEN = 10;
        protected readonly int _zIndex;
        protected int _currentChaseTick;
        protected CommanderKeen _keen;
        protected Image _intialStunImage = Properties.Resources.keen6_bloog_stunned1;

        public Bloog(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
        }

        protected virtual void Initialize()
        {
            this.Health = 1;
            _walkLeftSprites = SpriteSheet.SpriteSheet.BloogWalkLeftImages;
            _walkRightSprites = SpriteSheet.SpriteSheet.BloogWalkRightImages;
            _stunnedSprites = SpriteSheet.SpriteSheet.BloogStunnedImages;

            int directionVal = _random.Next(0, 2);
            this.Direction = directionVal == 0 ? Enums.Direction.LEFT : Enums.Direction.RIGHT;

            this.State = BloogState.FALLING;
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
                    if (this.State != BloogState.FALLING)
                    {
                        this.UpdateCollisionNodes(this.Direction);
                    }
                    else
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                }
            }
        }

        public virtual void Update()
        {
            this.KillIfOutSideBoundsOfMap(_collisionGrid);
            if (_state != BloogState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }
            switch (_state)
            {
                case BloogState.MOVING:
                    this.Move();
                    break;
                case BloogState.STUNNED:
                    this.UpdateStunnedState();
                    break;
                case BloogState.FALLING:
                    this.Fall();
                    break;
            }
        }

        protected virtual void Fall()
        {
            if (this.State != BloogState.FALLING)
            {
                this.State = BloogState.FALLING;
            }
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + FALL_VELOCITY);
            var tile = this.BasicFallReturnTile(FALL_VELOCITY);

            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                if (!this.IsDead())
                {
                    this.Move();
                }
                else
                {
                    this.UpdateStunnedState();
                }
            }
            else if (this.IsDead())
            {
                this.UpdateHitboxBasedOnStunnedImage(
               _stunnedSprites
               , ref _currentStunnedSprite
               , ref _currentStunnedSpriteChangeDelayTick
               , STUNNED_SPRITE_CHANGE_DELAY
               , UpdateSprite);
            }
            else if (_keen.HitBox.IntersectsWith(areaToCheck))
            {
                _keen.Die();
            }
        }

        protected virtual void Move()
        {
            if (this.State != BloogState.MOVING)
            {
                this.State = BloogState.MOVING;
            }

            if (IsNothingBeneath())
            {
                this.Fall();
                return;
            }
            if (_currentChaseTick++ == TICKS_BEFORE_CHASING_KEEN)
            {
                _currentChaseTick = 0;
                this.Direction = SetDirectionFromObjectHorizontal(_keen, true);
            }

            if (IsOnEdge(this.Direction, 3))
            {
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
            }

            int xOffset = _direction == Enums.Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;

            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (tile != null)
            {
                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                if (this.HitBox.IntersectsWith(_keen.HitBox))
                {
                    _keen.Die();
                }
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    _keen.Die();
                }
            }

            this.UpdateSpriteByDelayBase(ref _currentMoveSpriteChangeDelayTick, ref _currentMoveSprite, MOVE_SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        protected virtual void UpdateStunnedState()
        {
            if (this.State != BloogState.STUNNED)
            {
                this.State = BloogState.STUNNED;
                _sprite = _intialStunImage;
                return;
            }

            this.UpdateHitboxBasedOnStunnedImage(
                _stunnedSprites
                , ref _currentStunnedSprite
                , ref _currentStunnedSpriteChangeDelayTick
                , STUNNED_SPRITE_CHANGE_DELAY
                , UpdateSprite);

            if (IsNothingBeneath())
            {
                this.Fall();
            }
        }

        public override void Die()
        {
            this.UpdateStunnedState();
        }

        protected virtual void UpdateSprite()
        {
            switch (_state)
            {
                case BloogState.MOVING:
                case BloogState.FALLING:
                    if (!this.IsDead())
                    {
                        var spriteSet = this.Direction == Enums.Direction.LEFT ? _walkLeftSprites : _walkRightSprites;
                        if (_currentMoveSprite >= spriteSet.Length || _currentMoveSprite < 0)
                        {
                            _currentMoveSprite = 0;
                        }

                        _sprite = spriteSet[_currentMoveSprite];
                    }
                    break;
                case BloogState.STUNNED:
                    if (_currentStunnedSprite >= _stunnedSprites.Length || _currentStunnedSprite < 0)
                    {
                        _currentStunnedSprite = 1;
                    }
                    _sprite = _stunnedSprites[_currentStunnedSprite];
                    break;
            }
        }

        protected BloogState State
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


        protected Direction Direction
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

        public bool DeadlyTouch
        {
            get { return !IsDead(); }
        }

        public virtual void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return _state != BloogState.STUNNED; }
        }

        public virtual PointItemType PointItem => PointItemType.KEEN6_ICE_CREAM_BAR;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_bloog_right2);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    public enum BloogState
    {
        MOVING,
        STUNNED,
        FALLING,
        SMASHING
    }
}
