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
        protected CheckPointFlagState _state;
        protected Image _sprite;

        protected int FLIP_ANIMATION_DELAY = 4;
        protected int _flipAnimationDelayCount = 0;
        protected int _flipSpriteIndex = 0;

        protected int LAND_ANIMATION_DELAY = 2;
        protected int _landAnimationDelayCount = 0;
        protected int _landSpriteIndex = 0;

        protected int WAVE_ANIMATION_DELAY = 2;
        protected int _waveAnimationDelayCount = 0;
        protected int _waveSpriteIndex = 0;

        public CheckPointFlag(SpaceHashGrid grid, Rectangle hitbox, int zIndex, CheckPointFlagState initialState) : base(grid, hitbox)
        {
            _zIndex = zIndex;
            _state = initialState;
            this.HitBox = hitbox;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.NONE;

        public CheckPointFlagState State => _state;

        protected abstract Image[] FlippingSprites { get; }

        protected abstract Image[] LandingSprites { get; }

        protected abstract Image[] WavingSprites { get; }

        public virtual void Update()
        {
            switch (_state)
            {
                case CheckPointFlagState.FLIPPING:
                    UpdateFlippingState();
                    break;
                case CheckPointFlagState.LANDING:
                    UpdateLandingState();
                    break;
                case CheckPointFlagState.WAVING:
                    UpdateWavingState();
                    break;
            }
        }

        protected virtual void UpdateWavingState()
        {
            _sprite = this.WavingSprites[_waveSpriteIndex];
            if (++_waveAnimationDelayCount >= WAVE_ANIMATION_DELAY)
            {
                _waveAnimationDelayCount = 0;
                if (++_waveSpriteIndex >= this.WavingSprites.Length)
                {
                    _waveSpriteIndex = 0;
                }
            }
        }

        protected virtual void UpdateLandingState()
        {
            _sprite = this.LandingSprites[_landSpriteIndex];
            if (++_landAnimationDelayCount >= LAND_ANIMATION_DELAY)
            {
                _landAnimationDelayCount = 0;
                if (++_landSpriteIndex >= this.LandingSprites.Length)
                {
                    _state = CheckPointFlagState.WAVING;
                }
            }
        }

        protected virtual void UpdateFlippingState()
        {
            _sprite = this.FlippingSprites[_flipSpriteIndex];
            if (++_flipAnimationDelayCount >= FLIP_ANIMATION_DELAY)
            {
                _flipAnimationDelayCount = 0;
                if (++_flipSpriteIndex >= this.FlippingSprites.Length)
                {
                    _state = CheckPointFlagState.LANDING;
                }
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
