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

        private readonly int _initialBottom;

        public SpinningCheckpointFlag(SpaceHashGrid grid, Rectangle hitbox, int zIndex) : base(grid, hitbox, zIndex, CheckPointFlagState.WAVING)
        {
            WAVE_ANIMATION_DELAY = 4;
            _initialBottom = hitbox.Bottom;
        }

        protected override Image[] FlippingSprites => _flippingSprites;

        protected override Image[] LandingSprites => _landingSprites;

        protected override Image[] WavingSprites => _wavingSprites;

        protected override void UpdateWavingState()
        {
            if (++_waveAnimationDelayCount >= WAVE_ANIMATION_DELAY)
            {
                _waveAnimationDelayCount = 0;
                if (++_waveSpriteIndex >= this.WavingSprites.Length)
                {
                    _waveSpriteIndex = 0;
                }
                _sprite = this.WavingSprites[_waveSpriteIndex];
                int previousIndex = _waveSpriteIndex == 0 ? this._wavingSprites.Length - 1 : _waveSpriteIndex - 1;
                var previousImage = _wavingSprites[previousIndex];

                int previousWidth = previousImage.Width;
                int widthDiff = _sprite.Width - previousWidth;

                this.HitBox = new Rectangle(this.HitBox.X + widthDiff, this.HitBox.Y,
                    _sprite.Width, _sprite.Height);

                if (this.HitBox.Bottom != _initialBottom)
                {
                    int bottomDiff = this.HitBox.Bottom - _initialBottom;
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - bottomDiff,
                        this.HitBox.Width, this.HitBox.Height);
                }
            }
        }
    }
}
