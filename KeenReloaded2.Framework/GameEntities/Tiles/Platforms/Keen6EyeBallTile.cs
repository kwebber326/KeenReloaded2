using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class Keen6EyeBallTile : SinglePlatform
    {
        public Keen6EyeBallTile(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(area, grid, area, null, zIndex, Biomes.BIOME_KEEN6_FINAL)
        {
            Initialize();
        }

        private void Initialize()
        {
            _image = Properties.Resources.keen6_eyeball_platform;
            _initialImageName = nameof(Properties.Resources.keen6_eyeball_platform);
        }
    }
}
