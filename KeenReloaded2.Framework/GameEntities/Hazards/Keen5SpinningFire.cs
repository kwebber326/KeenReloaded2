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
    public class Keen5SpinningFire : Hazard, IUpdatable
    {
        private const int SPRITE_CHANGE_DELAY = 0;
        private int _currentSpriteChangeDelayTick = 0;
        private int _currentSprite = 0;
        public Keen5SpinningFire(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area, Enums.HazardType.KEEN5_SPINNING_FIRE, zIndex)
        {

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
            var spriteSet = SpriteSheet.SpriteSheet.Keen5SpinningFireImages;
            if (++_currentSprite >= spriteSet.Length)
            {
                _currentSprite = 0;
            }
            _sprite = spriteSet[_currentSprite];
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{_imageName}{separator}{this.HitBox.X}{separator}{this.HitBox.Y}{separator}{this.HitBox.Width}{separator}{this.HitBox.Height}{separator}{_zIndex}";
        }
    }
}
