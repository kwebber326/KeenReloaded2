using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Keen4OracleDoor : Door, IUpdatable
    {
        private const int SPRITE_CHANGE_DELAY = 10;
        private int _spriteChangeDelayTick = 0;
        private int _currentSpriteIndex = 0;
        public Keen4OracleDoor(Rectangle area, SpaceHashGrid grid, int zindex, DoorType type, int doorId, int? destinationDoorId) 
            : base(area, grid, zindex, type, doorId, destinationDoorId)
        {
        }

        public void Update()
        {
            this.UpdateSpriteByDelayBase(ref _spriteChangeDelayTick, ref _currentSpriteIndex, SPRITE_CHANGE_DELAY, () =>
            {
                if (_currentSpriteIndex >= 2)
                    _currentSpriteIndex = 0;

                _sprite = _currentSpriteIndex == 0
                    ? Properties.Resources.keen4_oracle_door1
                    : Properties.Resources.keen4_oracle_door2;
            });
        }
    }
}
