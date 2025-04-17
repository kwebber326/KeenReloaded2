using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Keen6Tree
{
    public class Keen6TreeVine : Pole
    {
        private Rectangle _area;
        private readonly string _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
        private int _vineLength;
        
        public Keen6TreeVine(Rectangle area, SpaceHashGrid grid, int zIndex, int vineLength)
            : base(area, grid, zIndex, null, PoleType.MIDDLE, Biomes.BIOME_KEEN6_FOREST)
        {
            _objectKey = nameof(Properties.Resources.keen6_tree_vine_bottom);
            _vineLength = vineLength;
            _area = area;
            if (vineLength < 0)
                throw new ArgumentException("Vine length must be a non-negative number");

            Initialize();
        }

        private void Initialize()
        {
            _image = Properties.Resources.keen6_tree_vine_bottom;
            int width = _image.Width;
            int height = _image.Height;
            int x = 0, y = 0;

            List<LocatedImage> locatedImages = new List<LocatedImage>();

            Image previousImg = null;
            for (int i = 0; i < _vineLength; i++)
            {
                var img = Properties.Resources.keen6_tree_vine;
                int previousImgHeight = previousImg?.Height ?? 0;
                y += previousImgHeight;
                height += previousImgHeight;
                LocatedImage locatedImage = new LocatedImage()
                {
                    Image = img,
                    Location = new Point(x, y)
                };
                locatedImages.Add(locatedImage);
                previousImg = img;
            }

            int previousImageHeight = previousImg?.Height ?? 0;
            y += previousImageHeight;
            height += previousImageHeight;
            LocatedImage bottomImage = new LocatedImage()
            {
                Image = _image,
                Location = new Point(x, y)
            };
            locatedImages.Add(bottomImage);

            var images = locatedImages.Select(l => l.Image).ToArray();
            var locations = locatedImages.Select(l => l.Location).ToArray();

            Size canvas = new Size(width, height);
            _image = BitMapTool.DrawImagesOnCanvas(canvas, null, images, locations);
            _area = new Rectangle(_area.X, _area.Y, _image.Width, _image.Height);
        }

        public override string ToString()
        {
            return $"{_objectKey}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}{_separator}{_vineLength}";
        }
    }
}
