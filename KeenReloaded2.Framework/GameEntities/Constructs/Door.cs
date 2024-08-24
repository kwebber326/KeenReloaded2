using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Door : CollisionObject, ISprite
    {
        protected DoorType _doorType;

        protected const int LEFT_HITBOX_OFFSET = 32;
        protected const int RIGHT_HITBOX_OFFSET = 32;
        protected readonly int _zIndex;
        protected Door _destinationDoor;
        protected Image _sprite;
        protected Rectangle _area;

        public Door(Rectangle area, SpaceHashGrid grid, DoorType type, int doorId, int? destinationDoorId)
            : base(grid, area)
        {
            this.Id = doorId;
            _area = area;
            this.HitBox = _area;
            Initialize(type, destinationDoorId);
        }

        protected virtual void Initialize(DoorType type, int? destination)
        {
            this.DestinationDoorId = destination;
            _doorType = type;
            switch (_doorType)
            {
                case DoorType.KEEN4_BLUE:
                    _sprite = Properties.Resources.keen4_blue_door;
                    _sprite.Tag = nameof(Properties.Resources.keen4_blue_door);
                    break;
                case DoorType.KEEN4_GRAY:
                    _sprite = Properties.Resources.keen4_gray_door;
                    _sprite.Tag = nameof(Properties.Resources.keen4_gray_door);
                    break;
                case DoorType.KEEN4_ORACLE:
                    _sprite = Properties.Resources.keen4_oracle_door1;
                    _sprite.Tag = nameof(Properties.Resources.keen4_oracle_door1);
                    break;
                case DoorType.KEEN5_REGULAR:
                    _sprite = Properties.Resources.keen5_door;
                    _sprite.Tag = nameof(Properties.Resources.keen5_door);
                    break;
                case DoorType.KEEN5_EXIT:
                    _sprite = Properties.Resources.keen5_exit_door_closed;
                    _sprite.Tag = nameof(Properties.Resources.keen5_exit_door_closed);
                    break;
                case DoorType.KEEN6:
                    _sprite = Properties.Resources.keen6_door;
                    _sprite.Tag = nameof(Properties.Resources.keen6_door);
                    break;
                case DoorType.CHUTE:
                    _sprite = Properties.Resources.chute;
                    _sprite.Tag = nameof(Properties.Resources.chute);
                    _destinationDoor = null;
                    break;

            }
        }



        public int Id { get; private set; }

        public Door DestinationDoor
        {
            get
            {
                return _destinationDoor;
            }
            set
            {
                _destinationDoor = value;
                this.DestinationDoorId = _destinationDoor?.Id ?? 0;
            }
        }

        public int? DestinationDoorId { get; set; }

        public override CollisionType CollisionType => CollisionType.DOOR;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override string ToString()
        {
            var separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var destinationDoorString = this.DestinationDoor?.Id.ToString() ?? this.DestinationDoorId?.ToString() ?? string.Empty;
            return $"{_sprite.Tag?.ToString()}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{this._doorType.ToString()}{separator}{this.Id.ToString()}{separator}{destinationDoorString}";
        }
    }
}
