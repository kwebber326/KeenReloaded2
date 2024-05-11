using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KeenReloaded2.Framework.GameEntities.Animations
{
    public class Animation
    {
        protected readonly List<Image> _images;
        protected readonly int _delayMilliseconds;
        private readonly bool _isRepeating;
        protected int _currentDelayTick;
        protected int _currentIndex;
        protected Timer _animationTimer = new Timer();

        public event EventHandler AnimationStart;
        public event EventHandler AnimationEnd;
        public event EventHandler AnimationMoveNext;
        public event EventHandler AnimationMovePrevious;
        public Animation(List<Image> images, int delayMilliseconds, bool repeat)
        {
            _images = images;
            _delayMilliseconds = delayMilliseconds;
            _isRepeating = repeat;
            _animationTimer.Interval = _delayMilliseconds;
            _animationTimer.Elapsed += _animationTimer_Elapsed;
        }

        protected virtual void _animationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.MoveNext();
        }

        public virtual void Start()
        {
            _animationTimer.Start();
            this.AnimationStart?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Stop()
        {
            _animationTimer.Stop();
            this.AnimationEnd?.Invoke(this, EventArgs.Empty);
        }

        public virtual void MoveNext()
        {
            if (_images == null || !_images.Any())
                return;

            if (_currentIndex == _images.Count - 1)
            {
                _currentIndex = 0;
                if (!_isRepeating)
                {
                    this.Stop();
                }
            }
            else
            {
                _currentIndex++;
            }

            this.AnimationMoveNext?.Invoke(this, EventArgs.Empty);
        }

        public virtual void MovePrevious()
        {
            if (_images == null || !_images.Any())
                return;

            if (_currentIndex > 0)
            {
                _currentIndex--;
            }
            else
            {
                _currentIndex = _images.Count - 1;
            }

            this.AnimationMovePrevious?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Reset()
        {
            _currentIndex = 0;
            this.Stop();
        }

        public Image CurrentImage
        {
            get
            {
                return _images[_currentIndex];
            }
        }

        public bool IsRepeating
        {
            get
            {
                return _isRepeating;
            }
        }
    }
}
