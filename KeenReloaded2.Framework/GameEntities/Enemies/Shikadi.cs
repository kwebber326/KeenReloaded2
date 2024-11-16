using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Shikadi : DestructibleObject, IUpdatable, ISprite, IEnemy, IFireable, ICreateRemove, IZombieBountyEnemy
    {
        private int _currentMoveSprite;
        private int _currentLookSprite;
        private int _currentStunnedSprite;

        private const int BASIC_FALL_VELOCITY = 40;

        private const int WALK_VELOCITY = 10;
        private const int CHASE_KEEN_CHANCE = 30;
        private const int WALK_SPRITE_CHANGE_DELAY = 2;
        private int _currentWalkSpriteChangeDelayTick;

        private const int STUNNED_SPRITE_CHANGE_DELAY = 1;
        private int _currentStunnedSpriteChangeDelayTick;

        private const int LOOK_TIME = 50;
        private const int LOOK_CHANCE = 300;
        private int _currentLookTimeTick;
        private const int LOOK_SPRITE_CHANGE_DELAY = 1;
        private int _currentLookSpriteChangeDelayTick;

        private Pole _currentPole;
        private const int POLE_FIRE_TIME = 15;
        private int _currentPoleFireTimeTick;
        private const int POLE_FIRE_HORIZONTAL_OFFSET = 14;

        private bool _hitAnimation;
        private const int HIT_ANIMATION_TIME = 1;
        private readonly int _zIndex;
        private int _hitAnimationTimeTick;

        private bool _ignorePoleLogic;
        private Timer _ignorePoleLogicTimer = new Timer();

        public Shikadi(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            this.Health = 4;
            this.Direction = this.GetRandomHorizontalDirection();
            this.State = ShikadiState.WALKING;
            _ignorePoleLogicTimer.Interval = 2000;
            _ignorePoleLogicTimer.Elapsed += _ignorePoleLogicTimer_Elapsed;
        }

        private void _ignorePoleLogicTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _ignorePoleLogic = false;
            _ignorePoleLogicTimer.Stop();
        }

        public override void Die()
        {
            this.Stun();
        }

        public void Update()
        {
            this.KillIfOutSideBoundsOfMap(_collisionGrid);
            if (_state != ShikadiState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }
            if (_hitAnimation && _hitAnimationTimeTick++ == HIT_ANIMATION_TIME)
            {
                _hitAnimationTimeTick = 0;
                _hitAnimation = false;
                UpdateSprite();
            }

            switch (_state)
            {
                case ShikadiState.LOOKING:
                    this.Look();
                    break;
                case ShikadiState.WALKING:
                    this.Walk();
                    break;
                case ShikadiState.FIRING:
                    this.Fire();
                    break;
                case ShikadiState.STUNNED:
                    this.Stun();
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
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(this.Direction);
                }
            }
        }

        private void Stun()
        {
            if (this.State != ShikadiState.STUNNED)
            {
                this.State = ShikadiState.STUNNED;
                AdjustYPositionFromFloorTile();
            }

            if (IsNothingBeneath())
            {
                this.BasicFall(BASIC_FALL_VELOCITY);
            }

            var spriteIndex = _currentStunnedSprite;
            this.UpdateSpriteByDelay(ref _currentStunnedSpriteChangeDelayTick, ref _currentStunnedSprite, STUNNED_SPRITE_CHANGE_DELAY);
            if (_currentStunnedSprite != spriteIndex)
            {
                var image = SpriteSheet.SpriteSheet.ShikadiStunnedImages[_currentStunnedSprite];
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - (image.Size.Height - this.HitBox.Height), image.Width, image.Height);
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

        private bool ShouldFireElectricCharge(Pole pole)
        {
            return pole != null && pole.HitBox.Left < _keen.HitBox.Right && pole.HitBox.Right > _keen.HitBox.Left;
        }

        protected override Direction ChangeHorizontalDirection(Direction direction)
        {
            _currentPole = null;
            return direction == Direction.LEFT ? Direction.RIGHT : Direction.LEFT;
        }

        private void Walk()
        {
            if (this.State != ShikadiState.WALKING)
            {
                this.State = ShikadiState.WALKING;
                this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                AdjustYPositionFromFloorTile();
            }

            if (IsNothingBeneath())
            {
                this.BasicFall(BASIC_FALL_VELOCITY);
                return;
            }
            else
            {
                int lookVal = _random.Next(1, LOOK_CHANCE + 1);
                if (lookVal == LOOK_CHANCE)
                {
                    this.Look();
                    return;
                }
            }


            int chaseVal = _random.Next(1, CHASE_KEEN_CHANCE + 1);
            if (chaseVal == CHASE_KEEN_CHANCE)
            {
                this.Direction = SetDirectionFromObjectHorizontal(_keen, true);
            }

            if (this.IsOnEdge(this.Direction, 2))
            {
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
            }

            int xOffset = _direction == Enums.Direction.LEFT ? WALK_VELOCITY * -1 : WALK_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;

            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width + WALK_VELOCITY, this.HitBox.Height);

            var collisions = this.CheckCollision(areaToCheck);
            //pole collisions
            var poles = collisions.OfType<Pole>();

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);

            if (tile != null)
            {
                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                this.KillCollidingPlayers();
            }
            else if (!_ignorePoleLogic && poles.Any(p => p != _currentPole))
            {
                if (_direction == Enums.Direction.LEFT)
                {
                    Pole pole = poles.Where(p => p != _currentPole).OrderByDescending(p1 => p1.HitBox.X).FirstOrDefault();
                    if (ShouldFireElectricCharge(pole))
                    {
                        _currentPole = pole;
                        this.HitBox = new Rectangle(pole.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        this.Fire();
                    }
                    else
                    {
                        MoveForward(xOffset, poles);
                    }
                }
                else
                {
                    Pole pole = poles.Where(p => p != _currentPole).OrderBy(p1 => p1.HitBox.X).FirstOrDefault();
                    if (ShouldFireElectricCharge(pole))
                    {
                        _currentPole = pole;
                        this.HitBox = new Rectangle(pole.HitBox.Right - this.HitBox.Width - POLE_FIRE_HORIZONTAL_OFFSET, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        this.Fire();
                    }
                    else
                    {
                        MoveForward(xOffset, poles);
                    }
                }
            }
            else
            {
                MoveForward(xOffset, poles);
            }

            if (_currentWalkSpriteChangeDelayTick++ == WALK_SPRITE_CHANGE_DELAY)
            {
                _currentWalkSpriteChangeDelayTick = 0;
                _currentMoveSprite++;
                UpdateSprite();
            }
        }

        private void MoveForward(int xOffset, IEnumerable<Pole> poles)
        {
            this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            this.KillCollidingPlayers();
            if (!poles.Any())
            {
                _currentPole = null;
            }
        }

        private void AdjustYPositionFromFloorTile()
        {
            var landingTile = GetTopMostLandingTile(1);
            if (landingTile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, landingTile.HitBox.Y - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
            }
        }

        private void IgnorePoleLogicTemporarily()
        {
            _ignorePoleLogic = true;
            _ignorePoleLogicTimer.Start();
        }

        private void Look()
        {
            if (this.State != ShikadiState.LOOKING)
            {
                _currentLookTimeTick = 0;
                this.State = ShikadiState.LOOKING;
            }

            if (IsNothingBeneath())
            {
                this.BasicFall(BASIC_FALL_VELOCITY);
            }

            if (_currentLookTimeTick++ == LOOK_TIME)
            {
                this.Direction = this.SetDirectionFromObjectHorizontal(_keen, true);
                this.Walk();
            }
            else if (_currentLookSpriteChangeDelayTick++ == LOOK_SPRITE_CHANGE_DELAY)
            {
                _currentLookSpriteChangeDelayTick = 0;
                _currentLookSprite++;
                UpdateSprite();
            }
        }

        public bool DeadlyTouch
        {
            get { return _state != ShikadiState.STUNNED; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);

            if (this.Health > 0)
            {
                _hitAnimation = true;
                UpdateSprite();
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (!this.IsDead())
            {
                _hitAnimation = true;
                UpdateSprite();
            }
        }

        public bool IsActive
        {
            get { return _state != ShikadiState.STUNNED; }
        }

        public void Fire()
        {
            if (this.State != ShikadiState.FIRING)
            {
                _currentPoleFireTimeTick = 0;
                this.State = ShikadiState.FIRING;

                if (_currentPole != null)
                {
                    int shockWidth = 26, shockHeight = 40;
                    int xOffset = (shockWidth - _currentPole.HitBox.Width) / 2;
                    ShikadiShock shock = new ShikadiShock(_collisionGrid, new Rectangle(_currentPole.HitBox.X - xOffset, this.HitBox.Top - shockHeight, shockWidth, shockHeight), Enums.Direction.UP);
                    shock.Create += new EventHandler<ObjectEventArgs>(shock_Create);
                    shock.Remove += new EventHandler<ObjectEventArgs>(shock_Remove);

                    OnCreate(new ObjectEventArgs() { ObjectSprite = shock });
                    this.IgnorePoleLogicTemporarily();
                }
            }

            if (_currentPoleFireTimeTick++ == POLE_FIRE_TIME)
            {
                this.Direction = SetDirectionFromObjectHorizontal(_keen, true);
                this.Walk();
            }
        }

        void shock_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void shock_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public bool IsFiring
        {
            get { return _state == ShikadiState.FIRING; }
        }

        public int Ammo
        {
            get { return -1; }
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

        ShikadiState State
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

        public PointItemType PointItem => PointItemType.KEEN5_TART_STIX;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        private void UpdateSprite()
        {
            switch (_state)
            {
                case ShikadiState.WALKING:
                    var spriteSet = _direction == Enums.Direction.LEFT ? SpriteSheet.SpriteSheet.ShikadWalkLeftImages : SpriteSheet.SpriteSheet.ShikadWalkRightImages;
                    if (_currentMoveSprite >= spriteSet.Length)
                    {
                        _currentMoveSprite = 0;
                    }
                    _sprite = spriteSet[_currentMoveSprite];
                    break;
                case ShikadiState.LOOKING:
                    spriteSet = SpriteSheet.SpriteSheet.ShikadiLookImages;
                    if (_currentLookSprite >= spriteSet.Length)
                    {
                        _currentLookSprite = 0;
                    }
                    _sprite = spriteSet[_currentLookSprite];
                    break;
                case ShikadiState.STUNNED:
                    spriteSet = SpriteSheet.SpriteSheet.ShikadiStunnedImages;
                    if (_currentStunnedSprite >= spriteSet.Length)
                    {
                        _currentStunnedSprite = 1;
                    }
                    else if (_currentStunnedSprite == 0)
                    {
                        var firstStunnedSprite = spriteSet[0];
                        int yDiff = this.HitBox.Height - firstStunnedSprite.Height;
                        int xDiff = this.HitBox.Width - firstStunnedSprite.Width;
                        this.HitBox = new Rectangle(this.HitBox.X - (xDiff / 2), this.HitBox.Y + yDiff, _sprite.Width, _sprite.Height);
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                        this.UpdateCollisionNodes(Enums.Direction.UP);
                    }
                    _sprite = spriteSet[_currentStunnedSprite];
                    break;
                case ShikadiState.FIRING:
                    _sprite = _direction == Enums.Direction.LEFT
                      ? Properties.Resources.keen5_standard_shikadi_shoot_left
                      : Properties.Resources.keen5_standard_shikadi_shoot_right;
                    break;
            }

            if (_hitAnimation)
            {
                _sprite = this.GetCurrentSpriteWithWhiteBackground(_sprite);
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private ShikadiState _state;
        private Image _sprite;
        private CommanderKeen _keen;
        private Direction _direction;

        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                Create(this, args);
            }
        }

        protected void OnRemove(ObjectEventArgs args)
        {
            if (this.Remove != null)
            {
                if (args.ObjectSprite == this)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                    }
                }
                this.Remove(this, args);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_standard_shikadi_look4);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum ShikadiState
    {
        LOOKING,
        WALKING,
        FIRING,
        STUNNED
    }
}
