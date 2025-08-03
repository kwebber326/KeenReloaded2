using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class TarPit : ResizableLiquidHazard
    {
        public TarPit(Rectangle area, SpaceHashGrid grid, int zIndex, int lengths = 1, int depths = 0) 
            : base(area, grid, zIndex, HazardType.KEEN4_TAR, SpriteSheet.SpriteSheet.Keen4TarPitImages, Properties.Resources.keen4_tar_depth, lengths, depths)
        {

        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string key = nameof(Properties.Resources.keen4_tar1);
            var area = _area;
            return $"{key}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_lengths}{separator}{_depths}"; ;
        }
    }
}
