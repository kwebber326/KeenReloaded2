using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class MiddlePlatformTile : MaskedTile, IBiomeTile
    {
        protected string _biome;

        public MiddlePlatformTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, string biome) : base(area, grid, hitbox, imageFile, zIndex)
        {
            _biome = biome;
            SetImageFromBiome();
            _downwardCollisionOffset = 8;
            this.AdjustHitboxBasedOnOffsets();
        }

        public string Biome => _biome;

        protected virtual void SetImageFromBiome()
        {
            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_CAVE:
                    _image = Properties.Resources.keen4_cave_platform_middle;
                    break;
                case Biomes.BIOME_KEEN4_FOREST:
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _image = Properties.Resources.keen4_forest_platform_middle;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _image = Properties.Resources.keen4_mirage_platform_middle;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _image = Properties.Resources.keen5_platform_blue_middle;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _image = Properties.Resources.keen5_platform_green_middle;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _image = Properties.Resources.keen5_platform_red_middle;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                case Biomes.BIOME_KEEN6_FOREST:
                    _image = Properties.Resources.keen6_dome_platform_middle;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _image = Properties.Resources.keen6_industrial_platform_middle;
                    break;
            }
        }

        public override CollisionType CollisionType => CollisionType.PLATFORM;

        public void ChangeBiome(string biome)
        {
            _biome = biome;
            this.SetImageFromBiome();
        }
    }
}
