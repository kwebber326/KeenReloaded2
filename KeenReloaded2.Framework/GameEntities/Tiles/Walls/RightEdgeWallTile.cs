using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class RightEdgeWallTile : MaskedTile, IBiomeTile
    {
        protected string _biome;

        public RightEdgeWallTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, string biome) : base(area, grid, hitbox, imageFile, zIndex)
        {
            _biome = biome;
            this.SetImageFromBiome();
            _rightwardCollisionOffset = 9;
            this.AdjustHitboxBasedOnOffsets();
        }

        public string Biome => _biome;

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public event EventHandler<ObjectEventArgs> BiomeChanged;

        public void ChangeBiome(string biome)
        {
            _biome = biome;
            this.SetImageFromBiome();
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = this
            };
            BiomeChanged?.Invoke(this, e);
        }

        protected virtual void SetImageFromBiome()
        {
            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_CAVE:
                    _image = Properties.Resources.keen4_cave_wall_edge_right;
                    break;
                case Biomes.BIOME_KEEN4_FOREST:
                    _image = Properties.Resources.keen4_forest_wall_right_edge;
                    break;
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _image = Properties.Resources.keen4_pyramid_wall_edge_right;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _image = Properties.Resources.keen4_mirage_wall_right_edge;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _image = Properties.Resources.keen5_wall_black_edge_right;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _image = Properties.Resources.keen5_wall_green_edge_right;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _image = Properties.Resources.keen5_wall_red_edge_right;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _image = Properties.Resources.keen6_dome_wall_edge_right;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _image = Properties.Resources.keen6_forest_wall_edge_right;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _image = Properties.Resources.keen6_industrial_wall_edge_right;
                    break;
            }
            if (_image != null)
                _image = CommonGameFunctions.DrawImage(_area, _image);
        }
    }
}
