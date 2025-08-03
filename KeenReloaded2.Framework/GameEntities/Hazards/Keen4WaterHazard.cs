using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen4WaterHazard : ResizableLiquidHazard
    {
        public Keen4WaterHazard(Rectangle area, SpaceHashGrid grid, int zIndex, int lengths = 1, int depths = 0)
            : base(area, grid, zIndex, HazardType.KEEN4_WATER_HAZARD, SpriteSheet.SpriteSheet.Keen4WaterHazardSprites,
                  Properties.Resources.keen4_water_deep, lengths, depths)
        {
           
        }

        protected override Image GetRandomImageFromSpriteSheet(out int randVal)
        {
            randVal = _random.Next(0, 3);
            if (randVal != 0)
                return _depthSprite;

            randVal = _random.Next(0, _spriteSheet.Length);
            return _spriteSheet[randVal];
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string key = nameof(Properties.Resources.keen4_water5);
            var area = _area;
            return $"{key}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_lengths}{separator}{_depths}"; ;
        }
    }
}
