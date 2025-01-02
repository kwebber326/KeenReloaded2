using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Hazards;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEntities.Tiles;
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
    public class Keen4Sprite : CollisionObject, IUpdatable, IEnemy, IFireable, ISprite, ICreateRemove
    {
        private bool _isFiring;
        private Enums.Direction _direction;
        private Enums.Direction _floatDirection;
        private SpriteMoveState _moveState;

        private const int FLOAT_VELOCITY = 2;
        private const int MAX_FLOAT_DISTANCE = 6;

        private const int LOOK_TO_SHOOT_TRANSITION_DELAY = 10;
        private int _currentLookToShootDelayTick = 0;
        private const int SHOOT_DELAY = 6;
        private int _currentShootDelayTick = 0;
        private Image _sprite;
        private CommanderKeen _keen;
        private int _currentFloatDistance;
        private const int HOLD_SHOT_DELAY = 4;
        private int _currentHoldShotDelayTick = 0;
        private readonly int _zIndex;

        public Keen4Sprite(Rectangle area, SpaceHashGrid grid,  int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {

            _floatDirection = Enums.Direction.UP;
            this.MoveState = SpriteMoveState.WAITING;
        }

        public void Update()
        {
            _keen = this.GetClosestPlayer();
            if (this.MoveState == SpriteMoveState.WAITING)
            {
                this.Move();
            }
            else if (this.MoveState == SpriteMoveState.LOOKING)
            {
                CheckKeenLocation();
                if (_currentLookToShootDelayTick == LOOK_TO_SHOOT_TRANSITION_DELAY)
                {
                    _currentLookToShootDelayTick = 0;
                    this.MoveState = SpriteMoveState.SHOOTING;
                }
                else
                {
                    _currentLookToShootDelayTick++;
                }
            }
            else if (this.MoveState == SpriteMoveState.SHOOTING)
            {
                if (_currentShootDelayTick == SHOOT_DELAY || _isFiring)
                {
                    this.Fire();
                    _currentShootDelayTick = 0;

                    if (_currentHoldShotDelayTick++ == HOLD_SHOT_DELAY)
                    {
                        _currentHoldShotDelayTick = 0;
                        this.MoveState = SpriteMoveState.WAITING;
                        _isFiring = false;
                    }
                }
                else
                {
                    _currentShootDelayTick++;
                }
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
            }
        }

        internal SpriteMoveState MoveState
        {
            get
            {
                return _moveState;
            }
            private set
            {
                _moveState = value;
                UpdateSprite();
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
            get { return false; }
        }

        private void Move()
        {
            if (_floatDirection == Enums.Direction.UP)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y - FLOAT_VELOCITY, this.HitBox.Width, this.HitBox.Height);
                var collisionItems = this.CheckCollision(areaToCheck);
                var items = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK);
                if (items.Any())
                {
                    int maxBottom = items.Select(c => c.HitBox.Bottom).Max();
                    this.HitBox = new Rectangle(this.HitBox.X, maxBottom, this.HitBox.Width, this.HitBox.Height);
                    _floatDirection = Enums.Direction.DOWN;
                    _currentFloatDistance = 0;
                }
                else if (_currentFloatDistance <= MAX_FLOAT_DISTANCE)
                {
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - FLOAT_VELOCITY, this.HitBox.Width, this.HitBox.Height);
                    _currentFloatDistance += FLOAT_VELOCITY;
                }
                else
                {
                    _currentFloatDistance = 0;
                    _floatDirection = Enums.Direction.DOWN;
                }
            }
            else
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom + FLOAT_VELOCITY, this.HitBox.Width, this.HitBox.Height);
                var collisionItems = this.CheckCollision(areaToCheck);
                var items = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK);
                if (items.Any())
                {
                    int minTop = items.Select(c => c.HitBox.Top).Min();
                    this.HitBox = new Rectangle(this.HitBox.X, minTop - this.HitBox.Height, this.HitBox.Width, this.HitBox.Height);
                    _floatDirection = Enums.Direction.UP;
                    _currentFloatDistance = 0;
                }
                else if (_currentFloatDistance <= MAX_FLOAT_DISTANCE)
                {
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + FLOAT_VELOCITY, this.HitBox.Width, this.HitBox.Height);
                    _currentFloatDistance += FLOAT_VELOCITY;
                }
                else
                {
                    _currentFloatDistance = 0;
                    _floatDirection = Enums.Direction.UP;
                }
            }
            CheckKeenLocation();
        }

        private void CheckKeenLocation()
        {
            this.Direction = _keen != null && _keen.HitBox.X < this.HitBox.X ? Enums.Direction.LEFT : Enums.Direction.RIGHT;
            if (_keen.HitBox.Bottom > this.HitBox.Top && _keen.HitBox.Top < this.HitBox.Bottom && this.MoveState != SpriteMoveState.SHOOTING)
            {
                this.MoveState = SpriteMoveState.LOOKING;
                _currentShootDelayTick = 0;
            }
            else if (this.MoveState == SpriteMoveState.LOOKING)
            {
                this.MoveState = SpriteMoveState.WAITING;
                _currentLookToShootDelayTick = 0;
            }
        }

        private void UpdateSprite()
        {
            switch (MoveState)
            {
                case SpriteMoveState.WAITING:
                    _sprite = Properties.Resources.keen4_sprite_waiting;
                    break;
                case SpriteMoveState.LOOKING:
                    _sprite = this.Direction == Enums.Direction.LEFT ? Properties.Resources.keen4_sprite_look_left
                        : Properties.Resources.keen4_sprite_look_right;
                    break;
                case SpriteMoveState.SHOOTING:
                    _sprite = this.Direction == Enums.Direction.LEFT ? Properties.Resources.keen4_sprite_shoot_left
                        : Properties.Resources.keen4_sprite_shoot_right;
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
            }
        }

        public void Fire()
        {
            if (!_isFiring)
            {
                _isFiring = true;
                int xPos = this.Direction == Direction.LEFT ? this.HitBox.Left - 30 : this.HitBox.Right;
                StraightShotProjectile shot = new StraightShotProjectile(_collisionGrid, new Rectangle(xPos, this.HitBox.Y + 10, 30, 30), this.Direction, EnemyProjectileType.KEEN4_SPRITE_SHOT);
                shot.Create += new EventHandler<ObjectEventArgs>(shot_Create);
                shot.Remove += new EventHandler<ObjectEventArgs>(shot_Remove);
                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = shot
                };
                OnCreate(args);
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.KEEN4_SHOOT);
            }
        }

        protected void OnCreate(ObjectEventArgs e)
        {
            if (Create != null)
            {
                Create(this, e);
            }
        }

        protected void OnRemove(ObjectEventArgs e)
        {
            if (Remove != null)
            {
                Remove(this, e);
            }
        }

        void shot_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void shot_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public bool IsFiring
        {
            get { return this.MoveState == SpriteMoveState.SHOOTING; }
        }

        public int Ammo
        {
            get { return -1; }
        }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_sprite_look_right);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum SpriteMoveState
    {
        WAITING,
        LOOKING,
        SHOOTING
    }
}
