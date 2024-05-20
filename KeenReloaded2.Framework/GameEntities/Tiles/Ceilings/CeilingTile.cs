using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class CeilingTile : MaskedTile, IBiomeTile
    {
        protected string _biome;

        public CeilingTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, string biome) : base(area, grid, hitbox, imageFile, zIndex)
        {
            _biome = biome;
            _upwardCollisionOffset = 8;
            this.AdjustHitboxBasedOnOffsets();
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public string Biome => _biome;

        public void ChangeBiome(string biome)
        {
            _biome = biome;
            this.SetImageFromBiome();
        }

        protected virtual void SetImageFromBiome()
        {
            Image[] images = null;
            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_CAVE:
                    images = SpriteSheet.SpriteSheet.Keen4CaveCeilings;
                    break;
                case Biomes.BIOME_KEEN4_FOREST:
                    images = SpriteSheet.SpriteSheet.Keen4ForestCeilings;
                    break;
                case Biomes.BIOME_KEEN4_PYRAMID:
                    images = SpriteSheet.SpriteSheet.Keen4PyramidCeilings;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    images = SpriteSheet.SpriteSheet.Keen4MirageCeilings;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    images = SpriteSheet.SpriteSheet.Keen5BlackCeilings;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    images = SpriteSheet.SpriteSheet.Keen5GreenCeilings;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    images = SpriteSheet.SpriteSheet.Keen5RedCeilings;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    images = SpriteSheet.SpriteSheet.Keen6DomeCeilings;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    images = SpriteSheet.SpriteSheet.Keen6ForestCeilings;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    images = SpriteSheet.SpriteSheet.Keen6IndustrialCeilings;
                    break;
            }
            if (images != null)
            {
                int max = images.Length;
                int rand = this.GenerateRandomInteger(0, max);
                _image = images[rand];
            }
        }
    }
}
