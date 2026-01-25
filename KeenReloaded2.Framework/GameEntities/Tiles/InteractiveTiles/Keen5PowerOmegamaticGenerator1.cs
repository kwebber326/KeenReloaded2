using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles
{
    public class Keen5PowerOmegamaticGenerator1 : CollisionObject, IUpdatable, ISprite
    {
        private Image _sprite;
        private readonly int _zIndex;
        private readonly Rectangle _area;
        private readonly Image[] _spritesList = SpriteSheet.SpriteSheet.Keen5PowerGenerator1Images;
        private const int SPRITE_CHANGE_DELAY = 4;
        private int _currentSpriteChangeDelayTick;
        private int _currentSpriteIndex;

        private InvisibleTile _metalPipeCollisionArea;
        private InvisibleTile _leftSideCollisionArea;
        private InvisibleTile _rightSideCollisionArea;

        private ObjectiveEventType _eventType;

        public Keen5PowerOmegamaticGenerator1(Rectangle area, SpaceHashGrid grid, int zIndex, ObjectiveEventType eventType) : base(grid, area)
        {
            _zIndex = zIndex;
            _area = area;
            Initialize();
            _eventType = eventType;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.NONE;

        public void Update()
        {
            this.UpdateSprite();
        }

        private void Initialize()
        {
            _sprite = _spritesList[_currentSpriteIndex];

            //metal pipe hitbox
            Rectangle metalPipeHitbox = new Rectangle(_area.X, _area.Y + 74, 26, 46);
            _metalPipeCollisionArea = new InvisibleTile(_collisionGrid, metalPipeHitbox, true);

            //left block hitbox
            int blockVerticalOffset = 28;
            int leftBlockHorizontalOffset = 6;
            Rectangle leftBlockHitbox = new Rectangle(_area.X + metalPipeHitbox.Width + leftBlockHorizontalOffset,
                _area.Y + blockVerticalOffset, 
                100 - leftBlockHorizontalOffset,
                _area.Height - blockVerticalOffset);
            _leftSideCollisionArea = new InvisibleTile(_collisionGrid, leftBlockHitbox, true);

            //right block hitbox
            int rightBlockHorizontalOffset = 228;
            int rightBlockWidth = 70;
            Rectangle rightBlockHitbox = new Rectangle(_area.X + rightBlockHorizontalOffset,
                _area.Y + blockVerticalOffset,
                rightBlockWidth, _area.Height - blockVerticalOffset);
            _rightSideCollisionArea = new InvisibleTile(_collisionGrid, rightBlockHitbox, true);
        }

        private void UpdateSprite()
        {
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSpriteIndex, SPRITE_CHANGE_DELAY,
                () =>
                {
                    if (_currentSpriteIndex >= _spritesList.Length)
                    {
                        _currentSpriteIndex = 0;
                    }
                    _sprite = _spritesList[_currentSpriteIndex];
                });
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen5_omegamatic_first_machine1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_eventType}";
        }
    }
}
