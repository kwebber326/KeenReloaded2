using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
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

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class Keen5Manhole : SinglePlatform
    {
        private InvisibleTile _leftBlockTile;
        private InvisibleTile _rightBlockTile;

        public Keen5Manhole(Rectangle area, SpaceHashGrid grid, int zIndex, string imageFile, string biome)
            : base(area, grid, area, imageFile, zIndex, biome)
        {
            this.HitBox = area;
            this.SetImageFromBiome();
           

            _downwardCollisionOffset = 16;
            this.AdjustHitboxBasedOnOffsets();

            _leftBlockTile = new InvisibleTile(grid,
               new Rectangle(this.HitBox.X, this.HitBox.Y, 10, this.HitBox.Height), true);

            _rightBlockTile = new InvisibleTile(grid,
                new Rectangle(this.HitBox.Right - 10, this.HitBox.Y, 10, this.HitBox.Height), true);
        }

        protected override void SetImageFromBiome()
        {
            switch (_biome)
            {
                case Biomes.BIOME_KEEN5_BLACK:
                    _image = Properties.Resources.keen5_manhole_black;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _image = Properties.Resources.keen5_manhole_green;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                default:
                    _image = Properties.Resources.keen5_manhole_red;
                    break;
            }

            _image = GetExpandedImage(_image);
        }

        private Image GetExpandedImage(Image img)
        {
            try
            {
                var leftHalf = BitMapTool.CropImage(img, new Rectangle(0, 0, img.Width / 2, img.Height));
                var rightHalf = BitMapTool.CropImage(img, new Rectangle(img.Width / 2, 0,
                    img.Width / 2, img.Height));

                Image[] imgs = new Image[] { leftHalf, rightHalf };
                Point[] locations = new Point[]
                {
                    new Point(0, 0),
                    new Point(this.HitBox.Width - rightHalf.Width, 0),
                };

                var finalImage = BitMapTool.DrawImagesOnCanvas(
                    this.HitBox.Size, null, imgs, locations);

                return finalImage;
            }
            catch
            {
                return _image;
            }
        }
    }
}
