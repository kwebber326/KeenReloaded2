using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Keen5ControlPanel : CollisionObject, IUpdatable, ISprite
    {
        private Image _sprite;
        private Rectangle _calibrateHitbox;
        private const int SPRITE_CHANGE_DELAY = 12;
        private const int CALIBRATE_HITBOX_LEFT_OFFSET = 20, CALIBRATE_HITBOX_RIGHT_OFFSET = 20;
        private readonly int _zIndex;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;
        private Image[] _sprites = SpriteSheet.SpriteSheet.Keen5ControlPanelImages;

        public Keen5ControlPanel(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            _sprite = _sprites[_currentSprite];
            _calibrateHitbox = new Rectangle(this.HitBox.X + CALIBRATE_HITBOX_LEFT_OFFSET//x
                , this.HitBox.Y//y
                , this.HitBox.Width - CALIBRATE_HITBOX_LEFT_OFFSET - CALIBRATE_HITBOX_RIGHT_OFFSET//width
                , this.HitBox.Height);//height

            if (_collisionGrid != null && _collidingNodes != null)
            {
                this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
            }

        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.NONE;

        public void Update()
        {
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSprite, SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        private void UpdateSprite()
        {
            if (_currentSprite >= _sprites.Length)
            {
                _currentSprite = 0;
            }
            _sprite = _sprites[_currentSprite];
        }

        public Rectangle CalibrateHitbox
        {
            get
            {
                return _calibrateHitbox;
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_control_panel1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
