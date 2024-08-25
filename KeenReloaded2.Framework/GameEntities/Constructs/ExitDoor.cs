using KeenReloaded.Framework;
using KeenReloaded2.Constants;
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
    public class ExitDoor : Door, IUpdatable
    {
        private const int OPEN_DOOR_SPRITE_CHANGE_DELAY = 4;
        private int _currentSprite;
        private int _currentSpriteChangeDelay;
        private Image[] _doorOpenSprites;
        private bool _isOpening;
        private bool _isOpened;
        public ExitDoor(Rectangle area, SpaceHashGrid grid)
            : base(area, grid, DoorType.KEEN5_EXIT, -1, null)
        {
            _doorOpenSprites = SpriteSheet.SpriteSheet.Keen5ExitDoorOpenImages;
        }

        public bool IsOpened
        {
            get
            {
                return _isOpened;
            }
        }

        public bool IsOpening
        {
            get
            {
                return _isOpening;
            }
        }

        public void Open()
        {
            _isOpening = true;
        }

        public void Update()
        {
            if (_isOpening && !_isOpened)
            {
                this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelay, ref _currentSprite, OPEN_DOOR_SPRITE_CHANGE_DELAY, UpdateSprite);
            }
        }

        private void UpdateSprite()
        {
            if (_currentSprite >= _doorOpenSprites.Length)
            {
                _isOpened = true;
                return;
            }
            _sprite = _doorOpenSprites[_currentSprite];
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{_imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}";
        }
    }
}
