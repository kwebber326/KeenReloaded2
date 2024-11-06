using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class BipPlatformOperator : ISprite
    {
        private readonly int _zIndex;
        private Image _sprite;
        private readonly Point _location;
        private Direction _direction;

        public BipPlatformOperator(Point location, int zIndex)
        {
            _location = location;
            _zIndex = zIndex;
            this.Direction = Direction.DOWN;
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
                _sprite = GetImageFromCurrentDirection();
            }
        }

        private Image GetImageFromCurrentDirection()
        {
            switch (_direction)
            {
                case Direction.DOWN:
                default:
                    return Properties.Resources.keen6_bip_platform_down;
                case Direction.UP:
                    return Properties.Resources.keen6_bip_platform_up;
                case Direction.LEFT:
                    return Properties.Resources.keen6_bip_platform_left;
                case Direction.RIGHT:
                    return Properties.Resources.keen6_bip_platform_right;
                case Direction.UP_LEFT:
                    return Properties.Resources.keen6_bip_platform_up_left;
                case Direction.UP_RIGHT:
                    return Properties.Resources.keen6_bip_platform_up_right;
                case Direction.DOWN_LEFT:
                    return Properties.Resources.keen6_bip_platform_down_left;
                case Direction.DOWN_RIGHT:
                    return Properties.Resources.keen6_bip_platform_down_right;
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _location;
    }
}
