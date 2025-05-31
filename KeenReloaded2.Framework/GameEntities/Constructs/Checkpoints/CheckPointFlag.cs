using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints
{
    public abstract class CheckPointFlag : CollisionObject, IUpdatable, ISprite
    {
        protected readonly int _zIndex;
        private CheckPointFlagState _state;
        private Image _sprite;

        private const int FLIP_ANIMATION_DELAY = 4;
        private int _flipAnimationDelayCount = 0;
        private int _flipSpriteIndex = 0;

        private const int LAND_ANIMATION_DELAY = 2;
        private int _landAnimationDelayCount = 0;
        private int _landSpriteIndex = 0;

        private const int WAVE_ANIMATION_DELAY = 2;
        private int _waveAnimationDelayCount = 0;
        private int _waveSpriteIndex = 0;

        public CheckPointFlag(SpaceHashGrid grid, Rectangle hitbox, int zIndex, CheckPointFlagState initialState) : base(grid, hitbox)
        {
            _zIndex = zIndex;
            _state = initialState;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.NONE;

        protected abstract Image[] FlippingSprites { get; }

        protected abstract Image[] LandingSprites { get; }

        protected abstract Image[] WavingSprites { get; }

        public virtual void Update()
        {
            switch (_state)
            {
                case CheckPointFlagState.FLIPPING:
                    _sprite = this.FlippingSprites[_flipSpriteIndex];
                    if (++_flipAnimationDelayCount >= FLIP_ANIMATION_DELAY)
                    {
                        _flipAnimationDelayCount = 0;
                        if (++_flipSpriteIndex >= this.FlippingSprites.Length)
                        {
                            _state = CheckPointFlagState.LANDING;
                        }
                    }
                    break;
                case CheckPointFlagState.LANDING:
                    _sprite = this.LandingSprites[_landSpriteIndex];
                    if (++_landAnimationDelayCount >= LAND_ANIMATION_DELAY)
                    {
                        _landAnimationDelayCount = 0;
                        if (++_landSpriteIndex >= this.LandingSprites.Length)
                        {
                            _state = CheckPointFlagState.WAVING;
                        }
                    }
                    break;
                case CheckPointFlagState.WAVING:
                    _sprite = this.WavingSprites[_waveSpriteIndex];
                    if (++_waveAnimationDelayCount >= WAVE_ANIMATION_DELAY)
                    {
                        _waveAnimationDelayCount = 0;
                        if (++_waveSpriteIndex >= this.WavingSprites.Length)
                        {
                            _waveSpriteIndex = 0;
                        }
                    }
                    break;
            }
        }
    }

    public enum CheckPointFlagState
    {
        FLIPPING,
        LANDING,
        WAVING
    }
}
