using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
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

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class LaserTurret : Hazard, IUpdatable, IFireable, ICreateRemove
    {
        private Direction _direction;
        private bool _isActive;
        private const int FIRE_DELAY = 30;
        private int _currentFireDelayTick;
        private Rectangle _area;

        public LaserTurret(Rectangle area, SpaceHashGrid grid, int zIndex, Direction direction, bool IsActive, TurretType type, int shotDelayOffset = 0) :
            base(grid, area, type == TurretType.KEEN5 ? HazardType.KEEN5_LASER_TURRET : HazardType.KEEN6_LASER_TURRET, zIndex)
        {
            _type = type;
            _direction = direction;
            _isActive = IsActive;
            _area = area;
            if (shotDelayOffset < 0)
                shotDelayOffset = 0;
            _currentFireDelayTick = shotDelayOffset >= FIRE_DELAY ? shotDelayOffset % FIRE_DELAY : shotDelayOffset;
            Initialize();
        }

        public override bool IsDeadly
        {
            get
            {
                return false;
            }
        }

        protected void Initialize()
        {
            switch (_direction)
            {
                case Direction.RIGHT:
                    _sprite = _type == TurretType.KEEN5 ? Properties.Resources.keen5_laser_turret_right : Properties.Resources.keen6_laser_turret_right;
                    _sprite.Tag = _type == TurretType.KEEN5 ? nameof(Properties.Resources.keen5_laser_turret_right) : nameof(Properties.Resources.keen6_laser_turret_right);
                    this.StandingTile = new InvisiblePlatformTile(_collisionGrid, new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width - 10, this.HitBox.Height));
                    break;
                case Direction.LEFT:
                    _sprite = _type == TurretType.KEEN5 ? Properties.Resources.keen5_laser_turret_left : Properties.Resources.keen6_laser_turret_left;
                    _sprite.Tag = _type == TurretType.KEEN5 ? nameof(Properties.Resources.keen5_laser_turret_left) : nameof(Properties.Resources.keen6_laser_turret_left);
                    this.StandingTile = new InvisiblePlatformTile(_collisionGrid, new Rectangle(this.HitBox.X + 10, this.HitBox.Y, this.HitBox.Width - 10, this.HitBox.Height));
                    break;
                case Direction.UP:
                    _sprite = _type == TurretType.KEEN5 ? Properties.Resources.keen5_laser_turret_up : Properties.Resources.keen6_laser_turret_up;
                    _sprite.Tag = _type == TurretType.KEEN5 ? nameof(Properties.Resources.keen5_laser_turret_up) : nameof(Properties.Resources.keen6_laser_turret_up);
                    break;
                case Direction.DOWN:
                    _sprite = _type == TurretType.KEEN5 ? Properties.Resources.keen5_laser_turret_down : Properties.Resources.keen6_laser_turret_down;
                    _sprite.Tag = _type == TurretType.KEEN5 ? nameof(Properties.Resources.keen5_laser_turret_down) : nameof(Properties.Resources.keen6_laser_turret_down);
                    break;
            }
            this.HitBox = _area;
        }

        public InvisiblePlatformTile StandingTile
        {
            get;
            private set;
        }

        public virtual void Update()
        {
            if (_isActive)
            {
                if (_currentFireDelayTick++ == FIRE_DELAY)
                {
                    _currentFireDelayTick = 0;
                    this.Fire();
                }
            }
        }

        public void Fire()
        {
            Point fireLocation = new Point();
            Size fireHitbox = new Size();
            switch (_direction)
            {
                case Direction.UP:
                    fireLocation = new Point(this.HitBox.X, this.HitBox.Top - 24);
                    fireHitbox = new Size(32, 32);
                    break;
                case Direction.DOWN:
                    fireLocation = new Point(this.HitBox.X, this.HitBox.Bottom - 8);
                    fireHitbox = new Size(32, 32);
                    break;
                case Direction.LEFT:
                    fireLocation = new Point(this.HitBox.Left - 24, this.HitBox.Y);
                    fireHitbox = new Size(32, 32);
                    break;
                case Direction.RIGHT:
                    fireLocation = new Point(this.HitBox.Right - 8, this.HitBox.Y);
                    fireHitbox = new Size(32, 32);
                    break;
            }

            StraightShotProjectile projectile = new StraightShotProjectile(_collisionGrid, new Rectangle(fireLocation, fireHitbox), _direction, EnemyProjectileType.KEEN5_LASER_TURRET_SHOT);
            projectile.Create += new EventHandler<ObjectEventArgs>(trajectory_Create);
            projectile.Remove += new EventHandler<ObjectEventArgs>(trajectory_Remove);
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = projectile
            };
            OnCreate(e);
            EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                GeneralGameConstants.Sounds.LASER_TURRET_SHOT);
        }

        void trajectory_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void trajectory_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public bool IsFiring
        {
            get { return _isActive; }
        }

        public int Ammo
        {
            get { return -1; }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private TurretType _type;

        private void OnRemove(ObjectEventArgs e)
        {
            if (Remove != null)
            {
                Remove(this, e);
            }
        }

        private void OnCreate(ObjectEventArgs e)
        {
            if (Create != null)
            {
                Create(this, e);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

            return $"{_sprite.Tag}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_direction}{separator}{_isActive}{separator}{_type}{separator}{_currentFireDelayTick}";
        }
    }

    public enum TurretType
    {
        KEEN5,
        KEEN6
    }
}
