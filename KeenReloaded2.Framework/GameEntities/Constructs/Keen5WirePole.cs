using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Keen5WirePole : Pole
    {
        private readonly Rectangle _area;
        private readonly int _lengths;
        private readonly Image[] _poleSprites = SpriteSheet.SpriteSheet.Keen5WireImages;
        private readonly string _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
        public Keen5WirePole(Rectangle area, SpaceHashGrid grid, int zIndex, int lengths = 1)
            : base(area, grid, zIndex, null, PoleType.MIDDLE, Biomes.BIOME_KEEN5_BLACK)
        {
            _objectKey = nameof(Properties.Resources.keen5_wire1);
            _area = area;
            _lengths = lengths;
            if (_lengths < 1)
                throw new ArgumentException("Pole length needs to be a positive number");
            Initialize();
        }

        private void Initialize()
        {
            BuildImage();
            SetHitbox();
        }

        private void SetHitbox()
        {
            int offset = _image.Width - POLE_WIDTH;
            this.HitBox = new Rectangle(_area.X + offset, _area.Y, POLE_WIDTH, _image.Height);
        }

        private void BuildImage()
        {
            List<Image> images = new List<Image>();
            List<Point> locations = new List<Point>();
            int maxWidth = 0, totalHeight = 0;
            int x = 0, y = 0;
            //generate random images;
            for (int i = 0; i < _lengths; i++)
            {
                var image = GenerateRandomWireImage();
                images.Add(image);
                if (image.Width > maxWidth) maxWidth = image.Width;

                totalHeight += image.Height;
            }

            //shift shorter images right
            foreach (var image in images)
            {
                int offset = 0;
                if (image.Width <  maxWidth)
                {
                    offset = maxWidth - POLE_WIDTH;
                }
                x += offset;
                Point loc = new Point(x, y);
                locations.Add(loc);

                y += image.Height;
            }
            Size canvasSize = new Size(maxWidth, totalHeight);

           _image = BitMapTool.DrawImagesOnCanvas(canvasSize, null, images.ToArray(),
                locations.ToArray());
        }

        private Image GenerateRandomWireImage()
        {
            int random = _random.Next(1, _poleSprites.Length);
            return _poleSprites[random];
        }

        public override string ToString()
        {
            return $"{_objectKey}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}{_separator}{_lengths}";
        }
    }
}
