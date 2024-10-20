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
    public class Babobba : DestructibleObject, IUpdatable, ISprite, IEnemy, IFireable, ICreateRemove, IZombieBountyEnemy
    {
        private CommanderKeen _keen;
        private Image[] _stunnedSprites, _sleepSprites;

        #region physics variables
        private const int BASIC_FALL_VELOCITY = 30;
        private const int MAX_VERTICAL_VELOCITY = 80;
        private const int GRAVITY_ACCELERATION = 5;
        private const int AIR_RESISTANCE = 2;
        private const int INITIAL_HORIZONTAL_VELOCITY_ON_JUMP = 20;
        private const int MIN_HORIZONTAL_VELOCITY = 10;
        private const int INITIAL_VERTICAL_VELOCITY_ON_JUMP = 15;
        private int _currentVerticalVelocity, _currentHorizontalVelocity;
        #endregion

        #region delay variables
        private const int JUMPS_BEFORE_DECISION = 2;
        private int _currentJumpCount;
        private const int LAND_TIME = 6;
        private int _landTimeTick;
        private const int SLEEP_CHANCE = 15;
        private const int FIRE_TIME = 10;
        private int _fireTimeTick;
        private const int SLEEP_TIME = 160;
        private int _sleepTimeTick;

        private const int SLEEP_SPRITE_CHANGE_DELAY = 3;
        private int _currentSleepSpriteChangeDelayTick;
        private int _currentSleepSprite;

        private const int STUNNED_SPRITE_CHANGE_DELAY = 1;
        private int _currentStunnedSpriteChangeDelayTick;
        private int _currentStunnedSprite;
        private readonly int _zIndex;
        #endregion

        public Babobba(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            _stunnedSprites = SpriteSheet.SpriteSheet.BabobbaStunnedImages;
            _initialStunnedImage = SpriteSheet.SpriteSheet.BabobbaStunnedImages[0];
            _sleepSprites = SpriteSheet.SpriteSheet.BabobbaSleepImages;

            int directionVal = _random.Next(0, 2);
            this.Direction = directionVal == 0 ? Direction.LEFT : Enums.Direction.RIGHT;

            this.State = BabobbaState.FALLING;
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
                if (_collidingNodes != null && _collisionGrid != null && this.HitBox != null)
                {
                    if (this.State != BabobbaState.FALLING)
                    {
                        this.UpdateCollisionNodes(this.Direction);
                        if (this.State == BabobbaState.JUMPING)
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

        public override void Die()
        {
            this.UpdateStunnedState();
        }

        public void Update()
        {
            if (_state != BabobbaState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }
            switch (_state)
            {
                case BabobbaState.FALLING:
                    this.Fall();
                    break;
                case BabobbaState.JUMPING:
                    this.Jump();
                    break;
                case BabobbaState.LANDED:
                    this.Land();
                    break;
                case BabobbaState.STUNNED:
                    this.UpdateStunnedState();
                    break;
                case BabobbaState.FIRING:
                    this.Fire();
                    break;
                case BabobbaState.SLEEPING:
                    this.Sleep();
                    break;
                case BabobbaState.AWAKENING:
                    this.Awaken();
                    break;
            }
        }

        private void Sleep()
        {
            if (this.State != BabobbaState.SLEEPING)
            {
                this.State = BabobbaState.SLEEPING;
                _sleepTimeTick = 0;
                _currentJumpCount = 0;
                _currentSleepSprite = 0;
            }

            if (IsNothingBeneath())
            {
                this.Fall();
                return;
            }

            if (_sleepTimeTick++ == SLEEP_TIME)
            {
                this.Awaken();
                return;
            }

            if (_currentSleepSprite < _sleepSprites.Length)
            {
                this.UpdateSpriteByDelayBase(ref _currentSleepSpriteChangeDelayTick, ref _currentSleepSprite, SLEEP_SPRITE_CHANGE_DELAY, UpdateSprite);
            }
        }

        private void Awaken()
        {
            if (this.State != BabobbaState.AWAKENING)
            {
                this.State = BabobbaState.AWAKENING;
                _currentSleepSprite = _sleepSprites.Length - 1;
            }

            if (IsNothingBeneath())
            {
                this.Fall();
                return;
            }

            if (_currentSleepSprite > 0)
            {
                if (_currentSleepSpriteChangeDelayTick++ == SLEEP_SPRITE_CHANGE_DELAY)
                {
                    _currentSleepSpriteChangeDelayTick = 0;
                    _currentSleepSprite--;
                    UpdateSprite();
                }
            }
            else
            {
                _currentSleepSprite = 0;
                this.Land();
            }
        }

        private void UpdateStunnedState()
        {
            if (this.State != BabobbaState.STUNNED)
            {
                this.State = BabobbaState.STUNNED;
                _sprite = _initialStunnedImage;
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

        private void Land()
        {
            if (this.State != BabobbaState.LANDED)
            {
                this.State = BabobbaState.LANDED;
                _landTimeTick = 0;
            }

            if (_landTimeTick++ == LAND_TIME)
            {

                if (_currentJumpCount < JUMPS_BEFORE_DECISION)
                {
                    if (IsOnEdge(this.Direction, -90))
                        SwitchHorizontalDirection();
                    this.Jump();
                }
                else
                {
                    int sleepVal = _random.Next(1, SLEEP_CHANCE + 1);
                    if (sleepVal == SLEEP_CHANCE)
                    {
                        this.Sleep();
                    }
                    else
                    {
                        this.Fire();
                    }
                }
            }
            else if (IsNothingBeneath())
            {
                this.Fall();
            }
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK && c.HitBox.Bottom <= this.HitBox.Top && c.HitBox.Left <= this.HitBox.Right && c.HitBox.Right >= this.HitBox.Left).ToList();
            if (debugTiles.Any())
            {
                int maxBottom = debugTiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile;
            var landingTiles = collisions.Where(h => (h.CollisionType == CollisionType.BLOCK
                    || h.CollisionType == CollisionType.PLATFORM || h.CollisionType == CollisionType.POLE_TILE)
                && h.HitBox.Top >= this.HitBox.Top && h.HitBox.Left <= this.HitBox.Right && h.HitBox.Right >= this.HitBox.Left);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        private void Jump()
        {
            if (this.State != BabobbaState.JUMPING)
            {
                this.State = BabobbaState.JUMPING;
                _currentHorizontalVelocity = this.Direction == Enums.Direction.LEFT
                    ? INITIAL_HORIZONTAL_VELOCITY_ON_JUMP * -1
                    : INITIAL_HORIZONTAL_VELOCITY_ON_JUMP;
                if (_currentJumpCount < JUMPS_BEFORE_DECISION)
                {
                    _currentJumpCount++;
                }
                _currentVerticalVelocity = INITIAL_VERTICAL_VELOCITY_ON_JUMP * -1;
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
                this.KillCollidingPlayers();
                DecelerateHorizontalMovement();
                SwitchHorizontalDirection();
            }
            else
            {
                Rectangle areaToCheckToKillKeen = new Rectangle(
                    _currentHorizontalVelocity < 0 ? this.HitBox.X + _currentHorizontalVelocity : this.HitBox.X,
                    this.HitBox.Y, this.HitBox.Width + Math.Abs(_currentHorizontalVelocity), this.HitBox.Height);
                this.KillCollidingPlayers(areaToCheckToKillKeen);
                this.HitBox = new Rectangle(this.HitBox.X + _currentHorizontalVelocity, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                DecelerateHorizontalMovement();
            }

            if (verticalTile != null)
            {
                int _collisionYPos = _currentVerticalVelocity < 0 ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                this.HitBox = new Rectangle(this.HitBox.X, _collisionYPos, this.HitBox.Width, this.HitBox.Height);
                this.KillCollidingPlayers();
                if (_currentVerticalVelocity > 0)
                {
                    this.Land();
                }
                AccelerateVerticalMovement();
            }
            else
            {
                Rectangle areaToCheckToKillKeen = new Rectangle(
                   this.HitBox.X,
                   _currentVerticalVelocity < 0 ? this.HitBox.Y + _currentVerticalVelocity : this.HitBox.Y
                   , this.HitBox.Width, this.HitBox.Height + Math.Abs(_currentVerticalVelocity));

                this.KillCollidingPlayers();
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                AccelerateVerticalMovement();
            }
        }

        private void SwitchHorizontalDirection()
        {
            _currentHorizontalVelocity *= -1;
            this.Direction = this.ChangeHorizontalDirection(this.Direction);
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
                    _currentHorizontalVelocity = 0;
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
                    _currentHorizontalVelocity = 0;
                }
            }
        }

        private void Fall()
        {
            if (this.State != BabobbaState.FALLING)
            {
                if (!this.IsDead())
                    this.State = BabobbaState.FALLING;
            }

            var tile = this.BasicFallReturnTile(BASIC_FALL_VELOCITY);
            Rectangle areaToCheckKeen = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + BASIC_FALL_VELOCITY);
            if (!this.IsDead())
            {
                if (tile != null)
                {
                    if (_keen.HitBox.IntersectsWith(this.HitBox))
                        _keen.Die();
                    this.Land();
                }
                else if (_keen.HitBox.IntersectsWith(areaToCheckKeen))
                    _keen.Die();
            }
        }

        public bool DeadlyTouch
        {
            get { return !IsDead(); }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return _state != BabobbaState.STUNNED; }
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

        BabobbaState State
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
                case BabobbaState.FALLING:
                    if (!this.IsDead())
                    {
                        _sprite = this.Direction == Enums.Direction.LEFT
                            ? Properties.Resources.keen6_babobba_fall_left
                            : Properties.Resources.keen6_babobba_fall_right;
                    }
                    break;
                case BabobbaState.JUMPING:
                    _sprite = this.Direction == Enums.Direction.LEFT
                      ? Properties.Resources.keen6_babobba_jump_left
                      : Properties.Resources.keen6_babobba_jump_right;
                    break;
                case BabobbaState.LANDED:
                case BabobbaState.FIRING:
                    _sprite = this.Direction == Enums.Direction.LEFT
                       ? Properties.Resources.keen6_babobba_land_left
                       : Properties.Resources.keen6_babobba_land_right;
                    break;
                case BabobbaState.STUNNED:
                    if (_currentStunnedSprite >= _stunnedSprites.Length)
                    {
                        _currentStunnedSprite = 1;
                    }
                    _sprite = _stunnedSprites[_currentStunnedSprite];
                    break;
                case BabobbaState.SLEEPING:
                case BabobbaState.AWAKENING:
                    if (_currentSleepSprite < _sleepSprites.Length)
                    {
                        _sprite = _sleepSprites[_currentSleepSprite];
                    }
                    break;
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private Enums.Direction _direction;
        private BabobbaState _state;
        private Image _sprite;
        private Image _initialStunnedImage;



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

        public void Fire()
        {
            if (this.State != BabobbaState.FIRING)
            {
                this.State = BabobbaState.FIRING;
                _fireTimeTick = 0;
                _currentJumpCount = 0;
                ShootFireBall();
            }

            if (IsNothingBeneath())
            {
                this.Fall();
                return;
            }

            if (_fireTimeTick++ == FIRE_TIME)
            {
                if (IsOnEdge(this.Direction, -90))
                    SwitchHorizontalDirection();
                this.Jump();
            }
        }

        private void ShootFireBall()
        {
            int xPos = _direction == Enums.Direction.LEFT ? this.HitBox.Left : this.HitBox.Right;
            int yPos = this.HitBox.Y + 10;
            BabobbaFireBall fireBall = new BabobbaFireBall(_collisionGrid, new Rectangle(xPos, yPos, 10, 10), _direction);
            fireBall.Create += new EventHandler<ObjectEventArgs>(fireBall_Create);
            fireBall.Remove += new EventHandler<ObjectEventArgs>(fireBall_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = fireBall });
        }

        void fireBall_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void fireBall_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public bool IsFiring
        {
            get { return _state == BabobbaState.FIRING; }
        }

        public int Ammo
        {
            get { return -1; }
        }

        public PointItemType PointItem => PointItemType.KEEN6_ICE_CREAM_BAR;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;


        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_babobba_land_left);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum BabobbaState
    {
        LANDED,
        JUMPING,
        FALLING,
        STUNNED,
        SLEEPING,
        FIRING,
        AWAKENING
    }
}
