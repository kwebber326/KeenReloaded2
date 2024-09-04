using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen6BurnHazard : Hazard, IUpdatable
    {
        private const int SPRITE_CHANGE_DELAY = 0;
        private int _currentSpriteChangeDelayTick = 0;
        private int _currentSprite = 0;
        Rectangle _area;

        public Keen6BurnHazard(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area, Enums.HazardType.KEEN6_BURN_HAZARD, zIndex)
        {
            _area = area;
        }
        public void Update()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            var spriteSet = SpriteSheet.SpriteSheet.Keen6BurnHazardImages;
            if (++_currentSprite >= spriteSet.Length)
            {
                _currentSprite = 0;
            }
            _sprite = spriteSet[_currentSprite];
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{nameof(Properties.Resources.keen6_burn_hazard3)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
