using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class StraightShotProjectile : CollisionObject, IProjectile, ISprite, IUpdatable, ICreateRemove
    {
        private Image _sprite;
        private EnemyProjectileType _projectileType;
        private Enums.Direction _direction;
        protected int _damage;
        protected int _velocity;
        protected int _pierce;
        protected int _spread;
        protected int _blastRadius;
        protected int _refireDelay;

        protected Image[] _shotSprites;
        protected Image[] _shotCompleteSprites;
        protected bool _shotComplete;
        protected int _spreadOffset;
        private int _currentShootSprite;
        private int _currentCompleteSprite;
        private int _currentSpriteDelay;
        private int UPDATE_SPRITE_DELAY = 1;
        private string _impactSound;

        public StraightShotProjectile(SpaceHashGrid grid, Rectangle hitbox, Direction direction, EnemyProjectileType projectileType)
            : base(grid, hitbox)
        {
            this.Direction = direction;
            _projectileType = projectileType;
            Initialize();
        }

        private void Initialize()
        {
            InitializeProjectile();
        }

        protected virtual void HandleCollision(CollisionObject obj)
        {
            if (obj.CollisionType == CollisionType.BLOCK)
            {
                StopAtCollisionObject(obj);
            }
            else if (obj is CommanderKeen)
            {
                var keen = (CommanderKeen)obj;
                keen.Die();
                if (--_pierce < 0)
                {
                    StopAtCollisionObject(obj);
                }
            }
        }

        public int Damage
        {
            get { return _damage; }
        }

        public int Velocity
        {
            get { return _velocity; }
        }

        public int Pierce
        {
            get { return _pierce; }
        }

        public int Spread
        {
            get { return _spread; }
        }

        public int BlastRadius
        {
            get { return _blastRadius; }
        }

        public int RefireDelay
        {
            get { return _refireDelay; }
        }

        public bool KillsKeen
        {
            get { return true; }
        }

        public void Move()
        {
            int x = this.HitBox.X;
            int y = this.HitBox.Y;
            Rectangle newLocation = new Rectangle(x, y, this.HitBox.Width, this.HitBox.Height);

            switch (this.Direction)
            {
                case Enums.Direction.LEFT:
                    x -= _velocity;
                    newLocation = new Rectangle(x, y + _spreadOffset, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    x += _velocity;
                    newLocation = new Rectangle(x, y + _spreadOffset, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    y -= _velocity;
                    newLocation = new Rectangle(x + _spreadOffset, y, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.DOWN:
                    y += _velocity;
                    newLocation = new Rectangle(x + _spreadOffset, y, this.HitBox.Width, this.HitBox.Height);
                    break;
            }

            this.HitBox = newLocation;

            UpdateSprite();
            this.UpdateCollisionNodes(this.Direction);
        }

        public virtual void Stop()
        {
            _shotComplete = true;
            if (!string.IsNullOrEmpty(_impactSound))
            {
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    _impactSound);
            }
            UpdateSprite();
        }

        protected virtual void InitializeProjectile()
        {
            switch (_projectileType)
            {
                case EnemyProjectileType.KEEN4_SPRITE_SHOT:
                    UPDATE_SPRITE_DELAY = 0;
                    _velocity = 70;
                    _pierce = 0;
                    _damage = int.MaxValue;
                    _refireDelay = -1;
                    _spread = 0;
                    _blastRadius = 0;
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.keen4_sprite_shot1,
                        Properties.Resources.keen4_sprite_shot2,
                        Properties.Resources.keen4_sprite_shot3,
                        Properties.Resources.keen4_sprite_shot4
                    };

                    _shotCompleteSprites = new Image[]
                    {
                        Properties.Resources.keen4_sprite_shot1
                    };
                    break;
                case EnemyProjectileType.KEEN5_LASER_TURRET_SHOT:
                    UPDATE_SPRITE_DELAY = 0;
                    _velocity = 70;
                    _pierce = 0;
                    _damage = int.MaxValue;
                    _refireDelay = -1;
                    _spread = 0;
                    _blastRadius = 0;
                    _impactSound = GeneralGameConstants.Sounds.LASER_TURRET_HIT;
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.keen5_turret_laser1,
                        Properties.Resources.keen5_turret_laser2,
                        Properties.Resources.keen5_turret_laser3,
                        Properties.Resources.keen5_turret_laser4
                    };

                    _shotCompleteSprites = new Image[]
                    {
                        Properties.Resources.keen5_turret_laser_hit1,
                        Properties.Resources.keen5_turret_laser_hit2
                    };
                    break;
                case EnemyProjectileType.KEEN5_ROBO_RED_SHOT:
                    UPDATE_SPRITE_DELAY = 0;
                    _velocity = 70;
                    _pierce = 0;
                    _damage = int.MaxValue;
                    _refireDelay = -1;
                    _spread = 3;
                    _blastRadius = 0;
                    _impactSound = GeneralGameConstants.Sounds.LASER_TURRET_HIT;
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.keen5_robo_red_shot1,
                        Properties.Resources.keen5_robo_red_shot2
                    };

                    _shotCompleteSprites = new Image[]
                    {
                        Properties.Resources.keen5_robo_red_shot_hit1,
                        Properties.Resources.keen5_robo_red_shot_hit2
                    };
                    break;
                case EnemyProjectileType.KEEN5_SHOCKSHUND_SHOT:
                    UPDATE_SPRITE_DELAY = 0;
                    _velocity = 60;
                    _pierce = 0;
                    _damage = int.MaxValue;
                    _refireDelay = -1;
                    _spread = 0;
                    _blastRadius = 0;
                    _impactSound = GeneralGameConstants.Sounds.SHOCKSHUND_SHOT_HIT;
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.keen5_shockshund_shot1,
                        Properties.Resources.keen5_shockshund_shot2
                    };

                    _shotCompleteSprites = new Image[]
                    {
                        Properties.Resources.keen5_shockshund_shot_hit1,
                        Properties.Resources.keen5_shockshund_shot_hit2
                    };
                    break;
                case EnemyProjectileType.KEEN5_SHIKADI_SHOCK:
                    UPDATE_SPRITE_DELAY = 0;
                    _velocity = 14;
                    _pierce = 0;
                    _damage = int.MaxValue;
                    _refireDelay = -1;
                    _spread = 0;
                    _blastRadius = 0;
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.keen5_standard_shikadi_electricity1
                    };

                    _shotCompleteSprites = new Image[]
                    {
                        Properties.Resources.keen5_standard_shikadi_electricity2
                    };
                    break;
                case EnemyProjectileType.KEEN6_BOBBA_SHOT:
                    UPDATE_SPRITE_DELAY = 0;
                    _velocity = 35;
                    _pierce = 0;
                    _damage = int.MaxValue;
                    _refireDelay = -1;
                    _spread = 0;
                    _blastRadius = 0;
                    _shotSprites = new Image[]
                    {
                        Properties.Resources.keen6_bobba_fireball1,
                        Properties.Resources.keen6_bobba_fireball2,
                        Properties.Resources.keen6_bobba_fireball3,
                        Properties.Resources.keen6_bobba_fireball4
                    };

                    _shotCompleteSprites = new Image[]
                    {
                         Properties.Resources.keen6_bobba_fireball1
                    };
                    break;
            }

            _sprite = _shotSprites[_currentShootSprite];
        }

        public Enums.MoveState MoveState
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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

        protected void GetSpreadOffset()
        {
            Random random = new Random();
            _spreadOffset = random.Next(_spread * -1, _spread + 1);
        }

        protected Rectangle GetAreaToCheckForCollision()
        {
            int x = this.HitBox.X;
            int y = this.HitBox.Y;
            Rectangle areaToCheck = new Rectangle(x, y, this.HitBox.Width, this.HitBox.Height);

            switch (this.Direction)
            {
                case Enums.Direction.LEFT:
                    areaToCheck = new Rectangle(x - _velocity, y + _spreadOffset, this.HitBox.Width + _velocity, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    areaToCheck = new Rectangle(x, y + _spreadOffset, this.HitBox.Width + _velocity, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    areaToCheck = new Rectangle(x + _spreadOffset, y - _velocity, this.HitBox.Width, this.HitBox.Height + _velocity);
                    break;
                case Enums.Direction.DOWN:
                    areaToCheck = new Rectangle(x + _spreadOffset, y, this.HitBox.Width, this.HitBox.Height + _velocity);
                    break;
            }

            return areaToCheck;
        }

        public virtual void Update()
        {
            if (IsOutOfBounds(this.Direction))
            {
                this.Stop();
                return;
            }
            if (!_shotComplete)
            {
                GetSpreadOffset();
                var areaToCheck = GetAreaToCheckForCollision();
                var collisionObjects = this.CheckCollision(areaToCheck);
                var debugTiles = collisionObjects.Where(c => c.CollisionType == CollisionType.BLOCK);
                var keens = collisionObjects.OfType<CommanderKeen>().ToList();
                var itemsToCheck = new List<CollisionObject>();
                itemsToCheck.AddRange(debugTiles);
                foreach (var keen in keens)
                {
                    if (keen != null)
                        itemsToCheck.Add(keen);
                }
                if (itemsToCheck.Any())
                {
                    HandleCollisionByDirection(collisionObjects);
                }
                else
                {
                    this.Move();
                }
            }
            else if (_shotCompleteSprites != null && _shotCompleteSprites.Any())
            {
                UpdateSprite();
            }
            else
            {
                this.UpdateCollisionNodes(this.Direction);
                CleanUpCollisionNodes();
                OnObjectComplete();
            }
        }

        protected virtual void UpdateSprite()
        {
            if (_currentSpriteDelay == UPDATE_SPRITE_DELAY)
            {
                if (!_shotComplete)
                {
                    if (_currentShootSprite < _shotSprites.Length - 1)
                    {
                        _currentShootSprite++;
                    }
                    else
                    {
                        _currentShootSprite = 0;
                    }
                    _sprite = _shotSprites[_currentShootSprite];
                }
                else if (_currentCompleteSprite <= _shotCompleteSprites.Length - 1)
                {
                    _sprite = _shotCompleteSprites[_currentCompleteSprite++];
                }
                else
                {
                    _sprite = null;
                    CleanUpCollisionNodes();
                    OnObjectComplete();
                }

                _currentSpriteDelay = 0;
            }
            else
            {
                _currentSpriteDelay++;
            }
        }

        protected void CleanUpCollisionNodes()
        {
            if (_collidingNodes != null)
            {
                foreach (var node in _collidingNodes)
                {
                    node.Objects.Remove(this);
                }
            }
        }

        protected void OnObjectComplete()
        {
            if (Remove != null)
            {
                ObjectEventArgs e = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                foreach (var node in _collidingNodes)
                {
                    node.NonEnemies.Remove(this);
                    node.Objects.Remove(this);
                }
                Remove(this, e);
            }
        }

        protected void OnCreate()
        {
            if (this.Create != null)
            {
                ObjectEventArgs e = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                this.Create(this, e);
            }
        }

        protected void HandleCollisionByDirection(IEnumerable<CollisionObject> collisions)
        {
            switch (Direction)
            {
                case Enums.Direction.DOWN:
                    collisions = collisions.OrderBy(c => c.HitBox.Top).ToList();
                    break;
                case Enums.Direction.UP:
                    collisions = collisions.OrderByDescending(c => c.HitBox.Bottom).ToList();
                    break;
                case Enums.Direction.LEFT:
                    collisions = collisions.OrderByDescending(c => c.HitBox.Right).ToList();
                    break;
                case Enums.Direction.RIGHT:
                    collisions = collisions.OrderBy(c => c.HitBox.Left).ToList();
                    break;
            }
            bool handledDebugTileCollision = false;
            foreach (var collision in collisions)
            {
                bool handle = !handledDebugTileCollision;
                if (handle)
                {
                    if (collision.CollisionType == CollisionType.BLOCK)
                        handledDebugTileCollision = true;
                    this.HandleCollision(collision);
                }
            }
            UpdateCollisionNodes(this.Direction);
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
                if (_collisionGrid != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(this.Direction);
                }
            }
        }

        public int ZIndex => 200;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.ENEMY_PROJECTILE;

        protected void StopAtCollisionObject(CollisionObject obj)
        {
            SetLocationByCollision(obj);
            this.Stop();
        }

        private void SetLocationByCollision(CollisionObject obj)
        {
            switch (Direction)
            {
                case Enums.Direction.LEFT:
                    this.HitBox = new Rectangle(obj.HitBox.Right, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    this.HitBox = new Rectangle(obj.HitBox.Left - this.HitBox.Width, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Bottom, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.DOWN:
                    this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Top - this.HitBox.Height, this.HitBox.Width, this.HitBox.Height);
                    break;
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
    }
}
