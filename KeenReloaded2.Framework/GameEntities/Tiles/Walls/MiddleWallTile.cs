using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class MiddleWallTile : MaskedTile, IBiomeTile
    {
        protected string _biome;

        public MiddleWallTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, string biome) : base(area, grid, hitbox, imageFile, zIndex)
        {
            _biome = biome;
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public event EventHandler<ObjectEventArgs> BiomeChanged;

        public string Biome => _biome;

        protected virtual void SetImageFromBiome()
        {
            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_CAVE:
                    _image = Properties.Resources.keen4_cave_wall_middle;
                    break;
                case Biomes.BIOME_KEEN4_FOREST:
                    _image = Properties.Resources.keen4_forest_wall_middle;
                    break;
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _image = Properties.Resources.keen4_pyramid_wall_middle;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _image = Properties.Resources.keen4_mirage_wall_middle;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _image = Properties.Resources.keen5_wall_black_middle;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _image = Properties.Resources.keen5_wall_green_middle;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _image = Properties.Resources.keen5_wall_red_middle;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    var rand = _random.Next(1, 3);
                    _image = rand == 1 ? Properties.Resources.keen6_dome_wall_middle1 : Properties.Resources.keen6_dome_wall_middle2;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    rand = _random.Next(1, 3);
                    _image = rand == 1 ? Properties.Resources.keen6_forest_wall_middle1 : Properties.Resources.keen6_forest_wall_middle2;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _image = Properties.Resources.keen6_industrial_wall_middle;
                    break;
            }
        }

        public void ChangeBiome(string biome)
        {
            _biome = biome;
            this.SetImageFromBiome();
        }
    }
}
