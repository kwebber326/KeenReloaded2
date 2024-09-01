using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Fire : Hazard, IUpdatable, ISprite
    {
        public Fire(Rectangle area, SpaceHashGrid grid, int zIndex, Direction direction)
            : base(grid, area, Enums.HazardType.KEEN4_FIRE, zIndex)
        {
            this.HitBox = area;
            Initialize(direction);
        }

        private void Initialize(Direction direction)
        {
            _direction = direction == Direction.LEFT ? Direction.LEFT : Direction.RIGHT;
            if (_direction == Direction.LEFT)
            {
                _images = new Image[]
                {
                    Properties.Resources.keen4_fire_left1,
                    Properties.Resources.keen4_fire_left2,
                    Properties.Resources.keen4_fire_left3
                };
            }
            else
            {
                _images = new Image[]
                {
                    Properties.Resources.keen4_fire_right1,
                    Properties.Resources.keen4_fire_right2,
                    Properties.Resources.keen4_fire_right3
                };
            }
            _sprite = _images[0];
        }

        private Direction _direction;
        private Image[] _images;

        private int _currentSprite;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick = 0;

        public void Update()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                if (_images != null && _images.Any())
                {
                    _sprite = _images[_currentSprite];
                    _currentSprite = _currentSprite < _images.Length - 1 ? _currentSprite + 1 : 0;
                }
                _currentSpriteChangeDelayTick = 0;
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string key = _direction == Direction.RIGHT
                ? nameof(Properties.Resources.keen4_fire_right1)
                : nameof(Properties.Resources.keen4_fire_left1);
            var area = this.HitBox;
            return $"{key}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_direction}"; ;
        }
    }
}
