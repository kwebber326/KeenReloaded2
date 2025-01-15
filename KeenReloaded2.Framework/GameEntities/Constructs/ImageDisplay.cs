using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class ImageDisplay : ISprite
    {
        private readonly int _zIndex;
        private Image _image;
        private Point _location;

        public ImageDisplay(Image image, Point location, int zIndex)
        {
            _image = image;
            _location = location;
            _zIndex = zIndex;
        }

        public int ZIndex => _zIndex;

        public Image Image => _image;

        public Point Location => _location;

        public bool CanUpdate => true;

        public void UpdateImage(Image image)
        {
            _image = image;
        }
    }
}
