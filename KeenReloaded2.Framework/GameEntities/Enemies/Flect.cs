using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities;
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
    public class Flect : DestructibleObject, IUpdatable, ISprite, IEnemy, IDeflector, IZombieBountyEnemy
    {
        private FlectState _state;
        private Enums.Direction _direction;
        private Image _sprite;
        private Image[] _walkLeftSprites, _walkRightSprites, _stunnedSprites;
        private int _currentWalkSprite, _currentStunnedSprite;
        private const int WALK_SPRITE_CHANGE_DELAY = 2;
        private int _currentWalkSpriteChangeDelayTick;
        private const int STUNNED_SPRITE_CHANGE_DELAY = 1;
        private int _currentStunnedSpriteChangeDelayTick;

        private const int BASIC_FALL_VELOCITY = 30;
        private const int WALK_VELOCITY = 5;
        private const int CHASE_KEEN_CHANCE = 20;
        private bool _walkingOffWallCollision;
        private const int TURN_TIME = 2;
        private int _turnTimeTick;
        private const int MIN_TURN_OFF_COLLISION_TIME = 2;
        private readonly int _zIndex;
        private int _collisionTimeTick;

        public Flect(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
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
                    if (_state == FlectState.FALLING)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                    else
                    {
                        this.UpdateCollisionNodes(this.Direction);
                    }
                }
            }
        }

        private void Initialize()
        {
            _walkLeftSprites = SpriteSheet.SpriteSheet.FlectWalkLeftImages;
            _walkRightSprites = SpriteSheet.SpriteSheet.FlectWalkRightImages;
            _stunnedSprites = SpriteSheet.SpriteSheet.FlectStunnedImages;

            this.Direction = this.GetRandomHorizontalDirection();
            this.State = FlectState.FALLING;
        }

        public override void Die()
        {
            foreach (var _keen in CurrentPlayerList.Players)
            {
                _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
            }
            this.UpdateStunnedState();
        }

        public void Update()
        {
            this.KillIfOutSideBoundsOfMap(_collisionGrid);
            switch (_state)
            {
                case FlectState.FALLING:
                    this.Fall();
                    break;
                case FlectState.TURNING:
                    this.Turn();
                    break;
                case FlectState.WALKING:
                    this.Walk();
                    break;
                case FlectState.STUNNED:
                    this.UpdateStunnedState();
                    break;
            }
        }

        private void Fall()
        {
            if (this.State != FlectState.FALLING)
            {
                this.State = FlectState.FALLING;
            }

            var tile = this.BasicFallReturnTile(BASIC_FALL_VELOCITY);
            if (tile != null)
            {
                this.Walk();
            }
        }

        private void Turn()
        {
            if (this.State != FlectState.TURNING)
            {
                this.State = FlectState.TURNING;
                _turnTimeTick = 0;
                foreach (var _keen in CurrentPlayerList.Players)
                {
                    _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                    _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
                }
                _collisionTimeTick = 0;
                CheckPushFromThisDirection();
            }

            if (_turnTimeTick++ == TURN_TIME)
            {
                var _keen = this.GetClosestPlayer();
                this.Direction = this.SetDirectionFromObjectHorizontal(_keen, true);

                this.Walk();
            }
        }

        private void Walk()
        {
            if (this.State != FlectState.WALKING)
            {
                this.State = FlectState.WALKING;
                CheckPushFromThisDirection();
            }

            if (IsNothingBeneath())
            {
                this.Fall();
                return;
            }

            if (IsOnEdge(this.Direction, 2))
            {
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                foreach (var keen in CurrentPlayerList.Players)
                {
                    keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                    keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
                }
                CheckPushFromThisDirection();
                _walkingOffWallCollision = true;
            }

            var _keen = this.GetClosestPlayer();
            if (IsKeenBehindThis(_keen))
            {
                if (!_walkingOffWallCollision && _collisionTimeTick++ == MIN_TURN_OFF_COLLISION_TIME)
                {
                    this.Turn();
                    return;
                }
                else
                {
                    int chaseVal = _random.Next(1, CHASE_KEEN_CHANCE + 1);
                    if (chaseVal == CHASE_KEEN_CHANCE || (IsKeenIntersectingOnVerticalPlane(_keen) && _collisionTimeTick++ == MIN_TURN_OFF_COLLISION_TIME))
                    {
                        _walkingOffWallCollision = false;
                        this.Turn();
                        return;
                    }
                }
            }

            int xOffset = _direction == Enums.Direction.LEFT ? WALK_VELOCITY * -1 : WALK_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;

            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width + WALK_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);

            if (tile != null)
            {
                _walkingOffWallCollision = true;
                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                ExecuteFreeMoveLogic(xOffset);
            }
            else
            {
                ExecuteFreeMoveLogic(xOffset);
            }

            this.UpdateSpriteByDelayBase(ref _currentWalkSpriteChangeDelayTick, ref _currentWalkSprite, WALK_SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        private void CheckPushFromThisDirection()
        {
            foreach (var _keen in CurrentPlayerList.Players)
            {
                if (_keen.HitBox.IntersectsWith(this.HitBox) && !IsKeenInFrontOfThis())
                {
                    _keen.SetKeenPushState(ChangeHorizontalDirection(_keen.Direction), true, this);
                }
                else
                {
                    _keen.SetKeenPushState(ChangeHorizontalDirection(_keen.Direction), false, this);
                }
            }
        }

        private bool IsKeenBehindThis(CommanderKeen _keen)
        {
            if (_keen.HitBox.Right < this.HitBox.Left + this.HitBox.Width / 2 && _direction == Enums.Direction.RIGHT)
                return true;

            if (_keen.HitBox.Left > this.HitBox.Left + this.HitBox.Width / 2 && _direction == Enums.Direction.LEFT)
                return true;

            return false;
        }

        private bool IsKeenInFrontOfThis()
        {
            foreach (var _keen in CurrentPlayerList.Players)
            {
                if (!_keen.HitBox.IntersectsWith(this.HitBox))
                    return false;
                if (_keen.Direction == Enums.Direction.LEFT)
                {
                    if (_keen.HitBox.Left <= this.HitBox.Right)
                        return true;
                }
                else
                {
                    if (_keen.HitBox.Right >= this.HitBox.Left)
                        return true;
                }
            }
            return false;
        }


        private bool IsKeenIntersectingOnVerticalPlane(CommanderKeen _keen)
        {
            return _keen.HitBox.Bottom >= this.HitBox.Top && _keen.HitBox.Top <= this.HitBox.Bottom;
        }

        private void ExecuteFreeMoveLogic(int xOffset)
        {
            foreach (var _keen in CurrentPlayerList.Players)
            {
                _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
            }

            Rectangle pushAreaToCheck = this.Direction == Enums.Direction.LEFT
                ? new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, xOffset * -1, this.HitBox.Height)
                : new Rectangle(this.HitBox.Right, this.HitBox.Y, xOffset, this.HitBox.Height);
            this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);

            foreach (var keen in CurrentPlayerList.Players)
            {
                if (!(keen.MoveState == MoveState.ON_POLE || keen.IsDead() || keen.IsStunned))
                {
                    if (keen.HitBox.IntersectsWith(pushAreaToCheck))
                    {
                        keen.SetKeenPushState(this.Direction, true, this);
                        keen.GetMovedHorizontally(this, this.Direction, WALK_VELOCITY);
                    }
                    else
                    {
                        keen.SetKeenPushState(this.Direction, false, this);
                    }
                }
            }
        }

        private void UpdateStunnedState()
        {
            if (this.State != FlectState.STUNNED)
            {
                this.State = FlectState.STUNNED;
            }

            this.UpdateHitboxBasedOnStunnedImage(
               _stunnedSprites
               , ref _currentStunnedSprite
               , ref _currentStunnedSpriteChangeDelayTick
               , STUNNED_SPRITE_CHANGE_DELAY
               , UpdateSprite);

            if (IsNothingBeneath())
            {
                this.BasicFall(BASIC_FALL_VELOCITY);
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
            get { return _state != FlectState.STUNNED; }
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
            }
        }

        FlectState State
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
                case FlectState.FALLING:
                case FlectState.WALKING:
                    var spriteSet = _direction == Enums.Direction.LEFT ? _walkLeftSprites : _walkRightSprites;
                    if (_currentWalkSprite >= spriteSet.Length)
                    {
                        _currentWalkSprite = 0;
                    }
                    _sprite = spriteSet[_currentWalkSprite];
                    break;
                case FlectState.TURNING:
                    _sprite = Properties.Resources.keen6_flect_look;
                    break;
                case FlectState.STUNNED:
                    spriteSet = _stunnedSprites;
                    if (_currentStunnedSprite >= spriteSet.Length)
                    {
                        _currentStunnedSprite = 1;
                    }
                    _sprite = spriteSet[_currentStunnedSprite];
                    break;
            }
        }

        public bool DeflectsHorizontally
        {
            get { return true; }
        }

        public bool DeflectsVertically
        {
            get { return false; }
        }

        public PointItemType PointItem => PointItemType.KEEN6_PIZZA_SLICE;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_flect_look);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
    enum FlectState
    {
        WALKING,
        TURNING,
        STUNNED,
        FALLING
    }
}
