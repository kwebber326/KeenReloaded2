using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KeenReloaded2.Framework.GameEntities.Backgrounds
{
    public class Background
    {
        protected readonly Rectangle _area;
        protected readonly string _imagePath;
        protected readonly bool _stretchImage;
        protected int _zIndex;
        protected Image _image;

        public Background(Rectangle area, string imagePath, bool stretchImage, int zIndex)
        {
            _area = area;
            _imagePath = imagePath;
            _stretchImage = stretchImage;
            _zIndex = zIndex;
            try
            {
                _image = Image.FromFile(imagePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public virtual Image Draw()
        {
            if (_image == null)
                return _image;

            if (_stretchImage)
            {
                return _image;
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
            using (var bitmap = new Bitmap(_area.Width, _area.Height))
            {

                //for each subsequent length and height, draw the image onto the bitmap
                //to fill out the background rectangle
                for (int i = 0; i < _area.Width; i += width)
                {
                    for (int j = 0; j < _area.Height; j += height)
                    {
                        Graphics.FromImage(bitmap).DrawImage(_image, new Rectangle(currentX, currentY, _image.Width, _image.Height));
                        currentY += height;
                    }
                    currentY = 0;
                    currentX += width;
                }
                //assign the resulting bitmap to the picture box's image property so it loads as 
                //one image onto the form
                _image = bitmap;
            }
        }
    }
}
