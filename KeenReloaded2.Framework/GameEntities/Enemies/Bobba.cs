using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities;
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

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Bobba : CollisionObject, IUpdatable, ISprite, IEnemy, IFireable, ICreateRemove
    {

        #region physics variables
        private const int BASIC_FALL_VELOCITY = 20;
        private const int MAX_VERTICAL_VELOCITY = 35;
        private const int GRAVITY_ACCELERATION = 3;
        private const int AIR_RESISTANCE = 1;
        private const int INITIAL_HORIZONTAL_VELOCITY_ON_JUMP = 15;
        private const int MIN_HORIZONTAL_VELOCITY = 10;
        private const int INITIAL_VERTICAL_VELOCITY_ON_JUMP = 15;
        private int _currentVerticalVelocity, _currentHorizontalVelocity;
        #endregion

        #region delay variables
        private const int JUMPS_BEFORE_DECISION = 2;
        private int _currentJumpCount;
        private const int LAND_TIME = 6;
        private int _landTimeTick;
        private const int FIRE_TIME = 10;
        private int _fireTimeTick;

        private Enums.Direction _direction;
        private BobbaState _state;
        private Image _sprite;
        protected readonly int _zIndex;
        #endregion

        public Bobba(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {

            this.Direction = GetRandomHorizontalDirection();

            this.State = BobbaState.FALLING;
        }

        private void Fall()
        {
            if (this.State != BobbaState.FALLING)
            {
                this.State = BobbaState.FALLING;
            }

            var tile = this.BasicFallReturnTile(BASIC_FALL_VELOCITY);
            Rectangle areaToCheckKeen = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + BASIC_FALL_VELOCITY);
            if (tile != null)
            {
                this.KillCollidingPlayers();
                this.Land();
            }
            else
            {
                this.KillCollidingPlayers(areaToCheckKeen);
            }
        }

        private void Land()
        {
            if (this.State != BobbaState.LANDED)
            {
                this.State = BobbaState.LANDED;
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
                    this.Fire();
                }
            }
            else if (IsNothingBeneath())
            {
                this.Fall();
            }
        }

        private void Jump()
        {
            if (this.State != BobbaState.JUMPING)
            {
                this.State = BobbaState.JUMPING;
                _currentHorizontalVelocity = this.Direction == Enums.Direction.LEFT
                    ? INITIAL_HORIZONTAL_VELOCITY_ON_JUMP * -1
                    : INITIAL_HORIZONTAL_VELOCITY_ON_JUMP;
                if (_currentJumpCount < JUMPS_BEFORE_DECISION)
                {
                    _currentJumpCount++;
                }
                _currentVerticalVelocity = INITIAL_VERTICAL_VELOCITY_ON_JUMP * -1;
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.BOBBA_JUMP);
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

                this.KillCollidingPlayers(areaToCheckToKillKeen);
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                AccelerateVerticalMovement();
            }
        }

        public void Fire()
        {
            if (this.State != BobbaState.FIRING)
            {
                this.State = BobbaState.FIRING;
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
            StraightShotProjectile fireBall = new StraightShotProjectile(_collisionGrid, new Rectangle(xPos, yPos, 32, 32), _direction, EnemyProjectileType.KEEN6_BOBBA_SHOT);
            fireBall.Create += new EventHandler<ObjectEventArgs>(fireBall_Create);
            fireBall.Remove += new EventHandler<ObjectEventArgs>(fireBall_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = fireBall });

            EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
             GeneralGameConstants.Sounds.BOBBA_SHOOT);
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
            if (_deccelerate)
            {
                _deccelerate = false;
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
            else
            {
                _deccelerate = true;
            }
        }

        private void SwitchHorizontalDirection()
        {
            _currentHorizontalVelocity *= -1;
            this.Direction = this.ChangeHorizontalDirection(this.Direction);
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
            var landingTiles = collisions.Where(h => (h.CollisionType == CollisionType.BLOCK || h.CollisionType == CollisionType.PLATFORM || h.CollisionType == CollisionType.POLE_TILE)
                && h.HitBox.Top >= this.HitBox.Top && h.HitBox.Left <= this.HitBox.Right && h.HitBox.Right >= this.HitBox.Left);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
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
                    if (this.State != BobbaState.FALLING)
                    {
                        this.UpdateCollisionNodes(this.Direction);
                        if (this.State == BobbaState.JUMPING)
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
            get { return true; }
        }

        public void HandleHit(IProjectile projectile)
        {

        }

        public bool IsActive
        {
            get { return true; }
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

        private void UpdateSprite()
        {
            switch (_state)
            {
                case BobbaState.FALLING:
                    _sprite = this.Direction == Enums.Direction.LEFT
                        ? Properties.Resources.keen6_bobba_fall_left
                        : Properties.Resources.keen6_bobba_fall_right;
                    break;
                case BobbaState.JUMPING:
                    _sprite = this.Direction == Enums.Direction.LEFT
                      ? Properties.Resources.keen6_bobba_jump_left
                      : Properties.Resources.keen6_bobba_jump_right;
                    break;
                case BobbaState.LANDED:
                case BobbaState.FIRING:
                    _sprite = this.Direction == Enums.Direction.LEFT
                       ? Properties.Resources.keen6_bobba_land_left
                       : Properties.Resources.keen6_bobba_land_right;
                    break;
            }
        }

        BobbaState State
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


        void fireBall_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void fireBall_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

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

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private bool _deccelerate;


        public bool IsFiring
        {
            get { return _state == BobbaState.FIRING; }
        }

        public int Ammo
        {
            get { return -1; }
        }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public void Update()
        {
            switch (_state)
            {
                case BobbaState.FALLING:
                    this.Fall();
                    break;
                case BobbaState.JUMPING:
                    this.Jump();
                    break;
                case BobbaState.LANDED:
                    this.Land();
                    break;
                case BobbaState.FIRING:
                    this.Fire();
                    break;
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_bobba_land_right);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum BobbaState
    {
        LANDED,
        JUMPING,
        FALLING,
        FIRING
    }
}
