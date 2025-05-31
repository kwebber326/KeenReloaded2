using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints
{
    public class SpinningCheckpointFlag : CheckPointFlag
    {
        private readonly Image[] _flippingSprites = new Image[]
        {
        };

        private readonly Image[] _landingSprites = new Image[]
        {
        };

        private readonly Image[] _wavingSprites = new Image[]
        {
            Properties.Resources.keen5_flag1,
            Properties.Resources.keen5_flag2,
            Properties.Resources.keen5_flag3,
            Properties.Resources.keen5_flag4,
        };

        public SpinningCheckpointFlag(SpaceHashGrid grid, Rectangle hitbox, int zIndex) : base(grid, hitbox, zIndex, CheckPointFlagState.WAVING)
        {
            WAVE_ANIMATION_DELAY = 4;
        }

        protected override Image[] FlippingSprites => _flippingSprites;

        protected override Image[] LandingSprites => _landingSprites;

        protected override Image[] WavingSprites => _wavingSprites;

        protected override void UpdateWavingState()
        {
            _sprite = this.WavingSprites[_waveSpriteIndex];
            if (++_waveAnimationDelayCount >= WAVE_ANIMATION_DELAY)
            {
                _waveAnimationDelayCount = 0;
                if (++_waveSpriteIndex >= this.WavingSprites.Length)
                {
                    _waveSpriteIndex = 0;
                }
                int previousIndex = _waveSpriteIndex == 0 ? this._wavingSprites.Length - 1 : _waveSpriteIndex - 1;
                var previousImage = _wavingSprites[previousIndex];
                var currentImage = _wavingSprites[_waveSpriteIndex];

                int previousWidth = previousImage.Width;
                int widthDiff = currentImage.Width - previousWidth;

                int previousHeight = previousImage.Height;
                int heightDiff = previousHeight - currentImage.Height;

                this.HitBox = new Rectangle(this.HitBox.X + widthDiff, this.HitBox.Y + heightDiff, 
                    this.HitBox.Width, this.HitBox.Height);
            }
        }
    }
}
