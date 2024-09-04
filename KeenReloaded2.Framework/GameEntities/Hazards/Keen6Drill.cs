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

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen6Drill : Hazard, IUpdatable
    {
        private const int SPRITE_CHANGE_DELAY = 2;
        private readonly Rectangle _area;
        private int _currentSpriteChangeDelayTick = 0;
        private int _currentSprite = 0;
        private Image[] _sprites = SpriteSheet.SpriteSheet.Keen6DrillSprites;
        public Keen6Drill(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area, HazardType.KEEN6_DRILL, zIndex)
        {
            _area = area;
        }

        public void Update()
        {
            this.UpdateSpriteByDelayBase(
                  ref _currentSpriteChangeDelayTick
                , ref _currentSprite
                , SPRITE_CHANGE_DELAY
                , UpdateSprite);
        }

        private void UpdateSprite()
        {
            if (_currentSprite >= _sprites.Length)
            {
                _currentSprite = 0;
            }
            _sprite = _sprites[_currentSprite];
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{nameof(Properties.Resources.keen6_drill1)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
