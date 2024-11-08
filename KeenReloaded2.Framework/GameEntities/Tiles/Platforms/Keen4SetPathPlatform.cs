using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
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
    public class Keen4SetPathPlatform : SetPathPlatform
    {
        private readonly Image[] _leftMovementImages = SpriteSheet.SpriteSheet.Keen4MovingPlatformLeftImages;
        private readonly Image[] _rightMovementImages = SpriteSheet.SpriteSheet.Keen4MovingPlatformRightImages;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick = 0;
        private int _currentSpriteIndex = 0;

        public Keen4SetPathPlatform(Rectangle area, SpaceHashGrid grid, int zIndex, List<Point> locations, Guid activationId, bool initiallyActive = false) 
            : base(area, grid, zIndex, PlatformType.KEEN4, locations, activationId, initiallyActive)
        {
        }

        public override void Update()
        {
            base.Update();
            UpdateSprite();
        }

        public override Direction Direction
        {
            get
            {
                return _direction;
            }
            protected set
            {
                _direction = value;
                SetImageBasedOnDirectionAndActiveState();
            }
        }

        private void SetImageBasedOnDirectionAndActiveState()
        {
            if (!_isActive)
            {
                _image = Properties.Resources.keen4_platform_stationary;
                return;
            }
            if (this.IsLeftDirection(_direction))
            {
                _currentSpriteIndex = 0;
                _image = _leftMovementImages[_currentSpriteIndex];
               
            }
            else if (this.IsRightDirection(_direction))
            {
                _currentSpriteIndex = 0;
                _image = _rightMovementImages[_currentSpriteIndex];
            }
            else
            {
                _image = Properties.Resources.keen4_platform_stationary;
            }
        }

        private void UpdateSprite()
        {
            if (_isActive)
            {
                if (this.IsLeftDirection(_direction))
                {
                    this.UpdateSpriteByDelayBase(
                        ref _currentSpriteChangeDelayTick,
                        ref _currentSpriteIndex,
                        SPRITE_CHANGE_DELAY,
                       () => ContinueAnimation(_leftMovementImages));
                }
                else if (this.IsRightDirection(_direction))
                {
                    this.UpdateSpriteByDelayBase(
                       ref _currentSpriteChangeDelayTick,
                       ref _currentSpriteIndex,
                       SPRITE_CHANGE_DELAY,
                      () => ContinueAnimation(_rightMovementImages));
                }
            }
            else
            {
                _image = Properties.Resources.keen4_platform_stationary;
            }
        }

        private void ContinueAnimation(Image[] sprites)
        {
            if (sprites == null || sprites.Length == 0)
                return;

            if (_currentSpriteIndex >= sprites.Length)
            {
                _currentSpriteIndex = 0;
            }
            _image = sprites[_currentSpriteIndex];
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_platform_stationary);
            string pathArray = MapMakerConstants.MAP_MAKER_ARRAY_START;
            foreach (var node in _pathwayPoints)
            {
                string item = node.X + MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR + node.Y + (node == _pathwayPoints.Last()
                    ? ""
                    : MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR);
                pathArray += item;
            }
            pathArray += MapMakerConstants.MAP_MAKER_ARRAY_END;
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{pathArray}{separator}{_activationId}{separator}{_isActive}";
        }
    }
}
