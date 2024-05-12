using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Animations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Backgrounds
{
    public class AnimatedBackground : Background
    {
        private readonly string[] _images;
        private readonly Animation _animation;
        private readonly int _imageRotationDelayMilliseconds;

        public AnimatedBackground(Rectangle area, string imagePath, bool stretchImage,
            string[] images, int imageRotationDelayMilliseconds, int zIndex) 
            : base(area, imagePath, stretchImage, zIndex)
        {
            _images = images;
            _imageRotationDelayMilliseconds = imageRotationDelayMilliseconds;
            var imageList = _images.Select(i =>
            {
                try
                {
                    Background b = new Background(area, i, stretchImage, zIndex);
                    return b.Draw();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return null;
                }
            }).Where(i => i != null).ToList();
            _animation = new Animation(imageList, imageRotationDelayMilliseconds, true);
            _animation.AnimationMoveNext += _animation_AnimationMoveNext;
        }

        private void _animation_AnimationMoveNext(object sender, EventArgs e)
        {
            _image = _animation.CurrentImage;
        }

        public void StartAnimation()
        {
            _animation.Start();
        }

        public void StopAnimation()
        {
            _animation.Stop();
        }

        public override string ToString()
        {
            var separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            var arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;
            var elementSeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            var images = string.Join(elementSeparator, _images);
            return base.ToString() + $"{separator}{arrayStart}{images}{arrayEnd}{separator}{_imageRotationDelayMilliseconds}";
        }
    }
}
