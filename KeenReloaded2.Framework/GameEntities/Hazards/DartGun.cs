using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class DartGun : Hazard, IUpdatable, IFireable, ICreateRemove
    {
        public DartGun(Rectangle area, SpaceHashGrid grid, int zIndex, Direction direction, bool isFiring = true, int shotDelayOffset = 0)
            : base(grid, area, Enums.HazardType.DART_GUN, zIndex)
        {
            _currentShotDelayTick = shotDelayOffset;
            Initialize(direction, isFiring);
        }

        private void Initialize(Direction direction, bool isFiring)
        {
            _isFiring = isFiring;
            _direction = direction;
            switch (_direction)
            {
                case Direction.DOWN:
                    _sprite = Properties.Resources.keen4_dart_gun_down;
                    _sprite.Tag = nameof(Properties.Resources.keen4_dart_gun_down);
                    break;
                case Direction.UP:
                    _sprite = Properties.Resources.keen4_dart_gun_up;
                    _sprite.Tag = nameof(Properties.Resources.keen4_dart_gun_up);
                    break;
                case Direction.RIGHT:
                    _sprite = Properties.Resources.keen4_dart_gun_right;
                    _sprite.Tag = nameof(Properties.Resources.keen4_dart_gun_right);
                    break;
                case Direction.LEFT:
                    _sprite = Properties.Resources.keen4_dart_gun_left;
                    _sprite.Tag = nameof(Properties.Resources.keen4_dart_gun_left);
                    break;
            }
        }

        private Direction _direction;
        private bool _isFiring;
        private const int SHOT_DELAY = 40;
        private int _currentShotDelayTick = 0;

        public void Update()
        {
            if (_isFiring)
            {
                if (_currentShotDelayTick == SHOT_DELAY)
                {
                    _currentShotDelayTick = 0;
                    this.Fire();
                }
                else
                {
                    _currentShotDelayTick++;
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
                    fireLocation = new Point(this.HitBox.X + 16, this.HitBox.Top - 32);
                    fireHitbox = new Size(10, 32);
                    break;
                case Direction.DOWN:
                    fireLocation = new Point(this.HitBox.X + 16, this.HitBox.Bottom);
                    fireHitbox = new Size(10, 32);
                    break;
                case Direction.LEFT:
                    fireLocation = new Point(this.HitBox.Left - 32, this.HitBox.Y + 16);
                    fireHitbox = new Size(32, 10);
                    break;
                case Direction.RIGHT:
                    fireLocation = new Point(this.HitBox.Right, this.HitBox.Y + 16);
                    fireHitbox = new Size(32, 10);
                    break;
            }
            Dart dart = new Dart(new Rectangle(fireLocation, fireHitbox), _collisionGrid, _direction);
            dart.Create += new EventHandler<ObjectEventArgs>(dart_Create);
            dart.Remove += new EventHandler<ObjectEventArgs>(dart_Remove);
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = dart
            };
            OnCreate(e);
        }

        void dart_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void dart_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

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

        public bool IsFiring
        {
            get { return _isFiring; }
        }

        public int Ammo
        {
            get { return -1; }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var area = this.HitBox;
            return $"{_sprite.Tag}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_direction}{separator}{_isFiring}{separator}{_currentShotDelayTick}";
        }
    }
}
