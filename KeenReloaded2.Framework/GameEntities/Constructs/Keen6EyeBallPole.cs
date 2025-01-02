using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Keen6EyeBallPole : Pole
    {
        private readonly Rectangle _area;
        private readonly string _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
        public Keen6EyeBallPole(Rectangle area, SpaceHashGrid grid, int zIndex) 
            : base(area, grid, zIndex, null, PoleType.MIDDLE, Biomes.BIOME_KEEN6_FINAL)
        {
            _objectKey = nameof(Properties.Resources.keen6_eyeball_pole);
            _area = area;
        }

        public override string ToString()
        {
            return $"{_objectKey}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}";
        }
    }
}
