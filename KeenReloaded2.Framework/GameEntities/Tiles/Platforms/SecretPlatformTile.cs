using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class SecretPlatformTile : MaskedTile, IUpdatable
    {
        private SecretPlatformState _state;
        private const int SHOW_STATE_CHANGE_DELAY = 0;
        private int _currentShowStateChangeDelayTick;
        private int _currentSprite;

        private const int HIDE_TIME = 60;
        private int _hideTimeTick;

        private Image[] _sprites = SpriteSheet.SpriteSheet.Keen4SecretPlatformImages;

        public SecretPlatformTile(Rectangle area, SpaceHashGrid grid, int zIndex, SecretPlatformState initialState)
            : base(area, grid, area, null, zIndex)
        {
            this.HitBox = area;
           
            Inititalize(initialState);
        }

        private void Inititalize(SecretPlatformState initialState)
        {
            this.State = initialState;
            _initialImageName = nameof(Properties.Resources.keen4_secret_platform_large_icon);
        }

        public SecretPlatformState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                _currentSprite = (int)_state;
                UpdateSprite();
            }
        }

        public override bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.PLATFORM;

        private void UpdateSprite()
        {
            _image = _sprites[_currentSprite];
        }

        public void Update()
        {
            switch (_state)
            {
                case SecretPlatformState.HIDDEN:
                    this.UpdateHideState();
                    break;
                default:
                    this.Show();
                    break;
            }
        }

        private void UpdateHideState()
        {
            if (this.State != SecretPlatformState.HIDDEN)
            {
                this.State = SecretPlatformState.HIDDEN;
                _hideTimeTick = 0;
            }

            if (_hideTimeTick++ == HIDE_TIME)
            {
                this.Show();
            }
        }

        private void Show()
        {
            switch (_state)
            {
                case SecretPlatformState.HIDDEN:
                    this.State = SecretPlatformState.SHOW1;
                    break;
                case SecretPlatformState.SHOW1:
                    if (_currentShowStateChangeDelayTick++ == SHOW_STATE_CHANGE_DELAY)
                    {
                        _currentShowStateChangeDelayTick = 0;
                        this.State = SecretPlatformState.SHOW2;

                    }
                    break;
                case SecretPlatformState.SHOW2:
                    if (_currentShowStateChangeDelayTick++ == SHOW_STATE_CHANGE_DELAY)
                    {
                        _currentShowStateChangeDelayTick = 0;
                        this.State = SecretPlatformState.SHOW3;
                    }
                    break;
                case SecretPlatformState.SHOW3:
                    if (_currentShowStateChangeDelayTick++ == SHOW_STATE_CHANGE_DELAY)
                    {
                        _currentShowStateChangeDelayTick = 0;
                        this.UpdateHideState();
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return $"{_initialImageName}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}{_separator}{_state}";
        }
    }

    public enum SecretPlatformState
    {
        HIDDEN,
        SHOW1,
        SHOW2,
        SHOW3
    }
}
