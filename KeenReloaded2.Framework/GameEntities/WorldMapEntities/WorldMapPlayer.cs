using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class WorldMapPlayer : CollisionObject, IUpdatable, ISprite
    {
        private Direction _direction;
        private Rectangle _area;
        private Image _sprite;
        private bool _isMoving;
        private readonly int _zIndex;

        public WorldMapPlayer(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            this.Direction = Direction.UP;
            _area = area;
            _zIndex = zIndex;
        }

        public override CollisionType CollisionType => CollisionType.PLAYER;

        public Direction Direction
        {
            get { return _direction; }
            private set
            {
                _direction = value;
                this.UpdateSprite();
            }
        }

        public void Update()
        {
            //TODO: implement movement
        }

        public override Rectangle HitBox
        {
            get => base.HitBox;

            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null && _collisionGrid != null)
                    this.UpdateCollisionNodes(_direction);
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        private void UpdateSprite()
        {
            //TODO: account for movement state when setting sprite
            switch (_direction)
            {
                case Direction.UP:
                    _sprite = Properties.Resources.keen_stop_up; break;
                case Direction.LEFT:
                    _sprite = Properties.Resources.keen_stop_left; break;
                case Direction.RIGHT:
                    _sprite = Properties.Resources.keen_stop_right; break;
                case Direction.DOWN:
                    _sprite = Properties.Resources.keen_stop_down; break;
                case Direction.UP_LEFT:
                    _sprite = Properties.Resources.keen_stop_up_left; break;
                case Direction.UP_RIGHT:
                    _sprite = Properties.Resources.keen_stop_up_right; break;
                case Direction.DOWN_LEFT:
                    _sprite = Properties.Resources.keen_stop_down_left; break;
                case Direction.DOWN_RIGHT:
                    _sprite = Properties.Resources.keen_stop_down_left; break;
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{nameof(Properties.Resources.keen_stop_up)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}";
        }
    }
}
