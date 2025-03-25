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
    public class CouncilMember : CollisionObject, IUpdatable, ISprite
    {
        private int _zIndex;
        private Image _sprite;
        private Direction _direction;
        private Image[] _walkLeftSprites = SpriteSheet.SpriteSheet.Keen4CouncilMemberWalkLeftImages;
        private Image[] _walkRightSprites = SpriteSheet.SpriteSheet.Keen4CouncilMemberWalkRightImages;
        private const int SPRITE_CHANGE_DELAY = 8;
        private const int WALK_SPEED = 5;
        private const int FALL_SPEED = 30;
        private const int PONDER_TIME = 40;
        private const int PONDER_CHANCE = 15;
        private const int DIRECTION_CHANGE_CHANCE = 10;
        private int _ponderTimeTickValue;
        private int _currentSpriteChangeDelayTick;
        private int _currentSpriteIndex;
        private CouncilMemberMoveState _state;

        public CouncilMember(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            _direction = this.GetRandomHorizontalDirection();
            this.Initialize();
        }

        private void Initialize()
        {
            _sprite = _direction == Direction.LEFT ? _walkLeftSprites[0] : _walkRightSprites[0];
        }

        public override CollisionType CollisionType => CollisionType.EXIT;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        private CouncilMemberMoveState MoveState
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                this.UpdateSprite();
            }
        }

        public override Rectangle HitBox
        {
            get => base.HitBox;
            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null && _collisionGrid != null)
                {
                    if (_direction == Direction.LEFT)
                    {
                        this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    }
                    else
                    {
                        this.UpdateCollisionNodes(Direction.DOWN_RIGHT);
                    }
                }
            }
        }

        public void Update()
        {
            if (this.IsNothingBeneath())
            {
                this.BasicFall(FALL_SPEED);
            }
            else if (_state == CouncilMemberMoveState.ROAMING
                && _currentSpriteChangeDelayTick++ >= SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;

                if (this.RandomPonderChance())
                {
                    this.MoveState = CouncilMemberMoveState.PONDERING;
                    return;
                }

                var areaToCheck = _direction == Direction.LEFT
                    ? new Rectangle(this.HitBox.X - WALK_SPEED, this.HitBox.Y,
                      this.HitBox.Width + WALK_SPEED, this.HitBox.Height)
                    : new Rectangle(this.HitBox.X, this.HitBox.Y,
                        this.HitBox.Width + WALK_SPEED, this.HitBox.Height);
                var wallCollisions = this.CheckCollision(areaToCheck, true);
                wallCollisions = wallCollisions.Where(c => c.CollisionType == CollisionType.BLOCK).ToList();
                bool wallCollide = wallCollisions.Any();
                bool isOnEdge = this.IsOnEdge(_direction);
                bool randomDirectionChange = this.RandomDirectionChange();
                if (wallCollide || isOnEdge || randomDirectionChange)
                {
                    _direction = this.ChangeHorizontalDirection(_direction);
                }
                else
                {
                    int xOffset = _direction == Direction.LEFT ? WALK_SPEED * -1 : WALK_SPEED;
                    this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
                UpdateSprite();
            }
            else if (_state == CouncilMemberMoveState.PONDERING && _ponderTimeTickValue++ >= PONDER_TIME)
            {
                _ponderTimeTickValue = 0;
                this.MoveState = CouncilMemberMoveState.ROAMING;
            }
        }

        private bool RandomDirectionChange()
        {
            int randVal = this.GenerateRandomInteger(1, DIRECTION_CHANGE_CHANCE);
            return (randVal == DIRECTION_CHANGE_CHANCE);
        }

        private bool RandomPonderChance()
        {
            int randVal = this.GenerateRandomInteger(1, PONDER_CHANCE);
            return (randVal == PONDER_CHANCE);
        }

        private void UpdateSprite()
        {
            switch (_state)
            {
                case CouncilMemberMoveState.PONDERING:
                    _sprite = _direction == Direction.LEFT
                        ? Properties.Resources.keen4_council_member_ponder_left
                        : Properties.Resources.keen4_council_member_ponder_right;
                    break;
                case CouncilMemberMoveState.ROAMING:
                    var images = _direction == Direction.LEFT
                        ? _walkLeftSprites : _walkRightSprites;
                    if (++_currentSpriteIndex >= images.Length)
                    {
                        _currentSpriteIndex = 0;
                    }
                    _sprite = images[_currentSpriteIndex];
                    break;
            }
        }

        public override string ToString()
        {
            var initialImageName = nameof(Properties.Resources.keen4_council_member_ponder_right);
            var separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var area = this.HitBox;
            return $"{initialImageName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}";
        }
    }

    internal enum CouncilMemberMoveState
    {
        ROAMING,
        PONDERING
    }
}
