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
    public class MiddleFloorTile : MaskedTile, IBiomeTile
    {
        protected string _biome;
        public MiddleFloorTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, string biome) : base(area, grid, hitbox, imageFile, zIndex)
        {
            _biome = biome;
            SetImageFromBiome();
            _downwardCollisionOffset = 32;
            this.AdjustHitboxBasedOnOffsets();
        }

        protected virtual void SetImageFromBiome()
        {
            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_CAVE:
                    _image = Properties.Resources.keen4_cave_floor_middle;
                    break;
                case Biomes.BIOME_KEEN4_FOREST:
                    _image = Properties.Resources.keen4_forest_floor_middle;
                    break;
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _image = Properties.Resources.keen4_pyramid_floor_middle;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _image = Properties.Resources.keen4_mirage_floor_middle;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _image = Properties.Resources.keen5_floor_black_middle;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _image = Properties.Resources.keen5_floor_green_middle;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _image = Properties.Resources.keen5_floor_red_middle;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _image = Properties.Resources.keen6_dome_floor_middle;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _image = Properties.Resources.keen6_forest_floor_middle;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _image = Properties.Resources.keen6_industrial_floor_middle;
                    break;
            }
            if (_image != null)
                _image = CommonGameFunctions.DrawImage(_area, _image);
        }

        public string Biome => _biome;

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public event EventHandler<ObjectEventArgs> BiomeChanged;

        public void ChangeBiome(string biome)
        {
            _biome = biome;
            SetImageFromBiome();
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = this
            };
            BiomeChanged?.Invoke(this, e);
        }
    }
}
