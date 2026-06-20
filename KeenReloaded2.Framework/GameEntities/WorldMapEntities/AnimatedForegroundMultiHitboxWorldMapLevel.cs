using KeenReloaded.Framework;
using KeenReloaded2.Framework.GameEntities.Animations;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class AnimatedForegroundMultiHitboxWorldMapLevel : ForeGroundMultiHitboxWorldMapLevel
    {
        private readonly int _animationDelay;
        private readonly int _animationStartIndex;
        private readonly Image[] _imageList;
        private readonly Animation _animation;

        public AnimatedForegroundMultiHitboxWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Rectangle[] entryPoints, Rectangle[] hitboxes, Rectangle[] foregroundAreas, string imagesPath, int animationDelay, int animationStartIndex, string key) 
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, entryPoints, hitboxes, foregroundAreas)
        {
            try
            {
                _animationStartIndex = animationStartIndex;
                _animationDelay = animationDelay;
                _imageList = new Image[0];
                if (AnimationDictionary.Animations.TryGetValue(key, out Image[] images))
                {
                    _animation = new Animation(images.ToList(), animationDelay, true);
                    _imageList = images;
                }
                else
                {

                    var fullPath = Path.Combine(Environment.CurrentDirectory, imagesPath);
                    var imageFiles = Directory.GetFiles(fullPath);
                    _imageList = imageFiles.Select(f =>
                    {
                        Image image = Image.FromFile(Path.Combine(Environment.CurrentDirectory, f));
                        image.Tag = f;
                        return image;
                    }).ToArray();
                    AnimationDictionary.Animations.Add(key, _imageList);
                    _animation = new Animation(_imageList.ToList(), animationDelay, true);
                }

                _animation.AnimationMoveNext += _animation_AnimationMoveNext;
                if (animationStartIndex >= _imageList.Length)
                    animationStartIndex = _imageList.Length - 1;

                _sprite = _animation.CurrentImage;
                _sprite.Tag = sprite.Tag;
                for (int i = 0; i < animationStartIndex; i++)
                {
                    _animation.MoveNext();
                }
                if (_collidingNodes != null)
                {
                    _animation.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void _animation_AnimationMoveNext(object sender, EventArgs e)
        {
            _sprite = _animation.CurrentImage;
        }

        public override string ToString()
        {
            return base.ToString() + $"{_separator}{_animationDelay}{_separator}{_animationStartIndex}";
        }
    }
}
