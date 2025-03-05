using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class RocketPropelledPlatform : Hazard, IUpdatable
    {
        private Image[] _sprites = SpriteSheet.SpriteSheet.RocketPropelledPlatformImages;
        private const int SPRITE_CHANGE_DELAY = 0;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;

        private const int VERTICAL_OFFSET_FOR_DEATH_COLLISION = 6;
        private const int HORIZONTAL_OFFSET_FOR_DEATH_COLLISION = 8;
        private Rectangle _deathHitBox;
        private CommanderKeen _keen;
        private readonly Rectangle _area;

        public RocketPropelledPlatform(Rectangle area, SpaceHashGrid grid,  int zIndex)
            : base(grid, area, Enums.HazardType.KEEN4_ROCKET_PROPELLED_PLATFORM, zIndex)
        {
            _sprite = _sprites[0];
            _area = area;
            Initialize();
        }

        InvisibleTile LandingTile { get; set; }

        private void Initialize()
        {
            UpdateSprite();
            if (_collidingNodes != null && _collisionGrid != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X + VERTICAL_OFFSET_FOR_DEATH_COLLISION, this.HitBox.Y + VERTICAL_OFFSET_FOR_DEATH_COLLISION,
                    this.HitBox.Width - (HORIZONTAL_OFFSET_FOR_DEATH_COLLISION * 2), this.HitBox.Height - VERTICAL_OFFSET_FOR_DEATH_COLLISION);
                LandingTile = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, VERTICAL_OFFSET_FOR_DEATH_COLLISION));
            }
        }

        public void Update()
        {
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSprite, SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        public void UpdateSprite()
        {
            if (_currentSprite >= _sprites.Length)
            {
                _currentSprite = 0;
            }
            _sprite = _sprites[_currentSprite];
        }

        public override Point Location => _area.Location;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{nameof(Properties.Resources.keen4_rocket_propelled_platform1)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
