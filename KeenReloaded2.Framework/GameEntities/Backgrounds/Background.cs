using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Constants;
using KeenReloaded2.Utilities;

namespace KeenReloaded2.Framework.GameEntities.Backgrounds
{
    public class Background : ISprite
    {
        protected readonly Rectangle _area;
        protected readonly string _imagePath;
        protected readonly bool _stretchImage;
        private readonly int _zIndex;
        protected Image _image;

        public Background(Rectangle area, string imageName, bool stretchImage, int zIndex)
        {
            _area = area;
            _imagePath = imageName;
            _stretchImage = stretchImage;
            _zIndex = zIndex;
            try
            {
                string directory = FileIOUtility.GetResourcePathForMainProject();
                string imagePath = System.IO.Path.Combine(directory, imageName);
                _image = Image.FromFile(imagePath);
               
                this.Draw();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public Background(Rectangle area, Image sprite, bool stretchImage, int zIndex)
        {
            _area = area;
            _image = sprite;
            _stretchImage = stretchImage;
            _zIndex = zIndex;
        }

        public int ZIndex => _zIndex;

        public Image Image => _image;

        public Point Location => _area.Location;

        public virtual bool CanUpdate => false;

        public virtual Image Draw()
        {
            if (_image == null)
                return _image;

            if (_stretchImage)
            {
                this.InitializeSpriteMap(_area.Size);
            }
            else
            {
                this.InitializeSpriteMap(_image.Size);
            }
            return _image;
        }

        protected virtual void InitializeSpriteMap(Size imageDimensions)
        {
            //image width and height
            int width = imageDimensions.Width;
            int height = imageDimensions.Height;
            //location to write image (0,0 is the top left corner of the picture box, not the form)
            int currentX = 0;
            int currentY = 0;
            //declare a bitmap for the image
            var bitmap = new Bitmap(_area.Width, _area.Height);

            //for each subsequent length and height, draw the image onto the bitmap
            //to fill out the background rectangle
            for (int i = 0; i < _area.Width; i += width)
            {
                for (int j = 0; j < _area.Height; j += height)
                {
                    Graphics.FromImage(bitmap).DrawImage(_image, new Rectangle(currentX, currentY, imageDimensions.Width, imageDimensions.Height));
                    currentY += height;
                }
                currentY = 0;
                currentX += width;
            }
            //assign the resulting bitmap to the picture box's image property so it loads as 
            //one image onto the form
            _image = bitmap;
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = FileIOUtility.ExtractFileNameFromPath(_imagePath);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_imagePath}{separator}{_stretchImage}{separator}{ZIndex}";
        }
    }
}
