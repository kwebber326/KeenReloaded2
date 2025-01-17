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
    public class SinglePlatform : MaskedTile, IBiomeTile
    {
        protected string _biome;
        public SinglePlatform(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, string biome) 
            : base(area, grid, hitbox, imageFile, zIndex)
        {
            _biome = biome;
            this.SetImageFromBiome();
            _downwardCollisionOffset = 16;
            _leftwardCollisionOffset = 2;
            _rightwardCollisionOffset = 2;
            this.AdjustHitboxBasedOnOffsets();
        }

        public string Biome => _biome;

        public event EventHandler<ObjectEventArgs> BiomeChanged;

        public override CollisionType CollisionType => CollisionType.PLATFORM;

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
                case Biomes.BIOME_KEEN4_FOREST:
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _image = Properties.Resources.keen4_forest_platform_single;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _image = Properties.Resources.keen4_mirage_platform_single;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                case Biomes.BIOME_KEEN5_GREEN:
                    _image = Properties.Resources.keen5_single_platform_blue;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _image = Properties.Resources.keen5_single_platform_red;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                case Biomes.BIOME_KEEN6_FOREST:
                    _image = Properties.Resources.keen6_dome_platform_single;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _image = Properties.Resources.keen6_industrial_single_platform;
                    break;
            }
        }
    }
}
