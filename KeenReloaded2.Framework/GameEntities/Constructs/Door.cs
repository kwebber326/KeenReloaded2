using KeenReloaded.Framework;
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

        public Door(SpaceHashGrid grid, Rectangle hitbox, DoorType type, int doorId, Door destinationDoor = null)
            : base(grid, hitbox)
        {
            this.Id = doorId;
            Initialize(type, destinationDoor);
        }

        protected virtual void Initialize(DoorType type, Door destination)
        {
            _destinationDoor = destination;
            _doorType = type;
            switch (_doorType)
            {
                case DoorType.KEEN4_BLUE:
                    _sprite = Properties.Resources.keen4_blue_door;
                    break;
                case DoorType.KEEN4_GRAY:
                    _sprite = Properties.Resources.keen4_gray_door;
                    break;
                case DoorType.KEEN4_ORACLE:
                    _sprite = Properties.Resources.keen4_oracle_door1;
                    break;
                case DoorType.KEEN5_REGULAR:
                    _sprite = Properties.Resources.keen5_door;
                    break;
                case DoorType.KEEN5_EXIT:
                    _sprite = Properties.Resources.keen5_exit_door_closed;
                    break;
                case DoorType.KEEN6:
                    _sprite = Properties.Resources.keen6_door;
                    break;
                case DoorType.CHUTE:
                    _sprite = Properties.Resources.chute;
                    _destinationDoor = null;
                    break;

            }
        }



        public long Id { get; private set; }

        public Door DestinationDoor
        {
            get
            {
                return _destinationDoor;
            }
            set
            {
                _destinationDoor = value;
            }
        }

        public override CollisionType CollisionType => CollisionType.DOOR;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override string ToString()
        {
            var destinationDoorString = this.DestinationDoor != null ? this.DestinationDoor.Id.ToString() : string.Empty;
            return $"{this.GetType().Name}|{this.Location.X}|{this.Location.Y}|{this._doorType.ToString()}|{this.Id.ToString()}|{destinationDoorString}";
        }
    }
}
