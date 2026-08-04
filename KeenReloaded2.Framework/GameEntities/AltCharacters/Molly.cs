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

namespace KeenReloaded2.Framework.GameEntities.AltCharacters
{
    public class Molly : CollisionObject, IUpdatable, ISprite, ILevelObjective
    {
        private readonly int _zIndex;
        private bool _rescued;
        private Image[] _sprites = SpriteSheet.SpriteSheet.MollyImages;

        private const int SPRITE_CHANGE_DELAY = 16;
        private const int FALL_VELOCITY = 30;
        private int _spriteChangeDelayTick = 0;
        private int _currentSpriteIndex = 0;
        
        private Image _sprite;

        public Molly(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            _sprite = _sprites[_currentSpriteIndex];
        }

        public override CollisionType CollisionType => CollisionType.EXIT;

        public override Rectangle HitBox
        {
            get => base.HitBox;
            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null && _collisionGrid != null)
                {
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public ObjectiveEventType EventType => ObjectiveEventType.LEVEL_EXIT;

        public bool ObjectiveComplete => _rescued;

        public void Update()
        {
            var keen = GetClosestAlivePlayer();
            if (keen != null && keen.HitBox.IntersectsWith(this.HitBox))
            {
                _rescued = true;
                keen.PassLevel();
            }
            else
            {
                this.UpdateSpriteByDelayBase(ref _spriteChangeDelayTick, ref _currentSpriteIndex,
                    SPRITE_CHANGE_DELAY, () =>
                    {
                        if (_currentSpriteIndex >= _sprites.Length)
                            _currentSpriteIndex = 0;

                        _sprite = _sprites[_currentSpriteIndex];
                    });

                if (this.IsNothingBeneath())
                    this.BasicFall(FALL_VELOCITY);
            }
        }

        public override string ToString()
        {
            var initialImageName = nameof(Properties.Resources.keen6_molly1);
            var separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var area = this.HitBox;
            return $"{initialImageName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}";
        }
    }
}
