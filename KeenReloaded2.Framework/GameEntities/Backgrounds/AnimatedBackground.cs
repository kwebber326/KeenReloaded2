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
        private readonly List<string> _images;
        private readonly Animation _animation;

        public AnimatedBackground(Rectangle area, string imagePath, bool stretchImage, int zIndex,
            List<string> images, int imageRotationDelayMilliseconds) 
            : base(area, imagePath, stretchImage, zIndex)
        {
            _images = images;
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
    }
}
