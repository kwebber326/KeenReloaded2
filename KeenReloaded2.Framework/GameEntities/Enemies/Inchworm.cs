using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
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

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Inchworm : CollisionObject, IUpdatable, ISprite
    {
        private CommanderKeen _keen;
        private readonly Image[] _moveLeftSprites = SpriteSheet.SpriteSheet.InchwormLeftImages;
        private readonly Image[] _moveRightSprites = SpriteSheet.SpriteSheet.InchwormRightImages;
        private readonly int _zIndex;
        private Image _sprite;
        private InchWormMoveState _state;
        private int _currentSpriteIndex;
        private int _currentSpriteChangeDelayTick;
        private int _bounceOffSteps = 0;
        private int _currentBounceOffStep = 0;
        private Direction _direction;

        private const int SPRITE_CHANGE_DELAY = 15;
        private const int MIN_BOUNCE_OFF_STEPS = 1, MAX_BOUNCE_OFF_STEPS = 5;
        protected const int FALL_VELOCITY = 20;
        protected const int MOVE_VELOCITY = 10;
        public Inchworm(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        public override Rectangle HitBox
        {
            get => base.HitBox;
            protected set
            {
                base.HitBox = value;
                if (_collisionGrid != null && _collidingNodes != null && base.HitBox != null)
                {
                    var directionToUpdate = _state == InchWormMoveState.FALLING ? Direction.DOWN : _direction;
                    //update collision detection algorithm to show which space hashes currently intersect with this object
                    this.UpdateCollisionNodes(directionToUpdate);
                }
            }
        }

        private void Initialize()
        {
            _state = InchWormMoveState.FALLING;
            this._direction = this.GetRandomHorizontalDirection();
            _sprite = _direction == Direction.LEFT ? _moveLeftSprites.FirstOrDefault() : _moveRightSprites.FirstOrDefault();
            _random = new Random();
        }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public void Update()
        {
            _keen = this.GetClosestPlayer();
            if (_state == InchWormMoveState.FALLING)
            {
                var tile = this.BasicFallReturnTile(FALL_VELOCITY);
                if (tile != null)
                {
                    this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                    _state = InchWormMoveState.FOLLOWING_KEEN;
                }
            }
            else
            {
                //check if the floor is not underneath this object (disappearing platforms can trigger this)
                if (this.IsNothingBeneath())
                {
                    _state = InchWormMoveState.FALLING;
                    return;
                }

                //update move state on delay
                if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
                {
                    _currentSpriteChangeDelayTick = 0;
                    //follow keen
                    if (_state == InchWormMoveState.FOLLOWING_KEEN)
                    {
                        TryFollowKeen();
                    }
                    else
                    {
                        ExecuteDirectionChangeSequence();
                    }
                }
            }
        }

        private void ExecuteDirectionChangeSequence()
        {
            //checks if this is the first step in the sequence
            if (_bounceOffSteps == 0)
            {
                _random = new Random();
                _bounceOffSteps = _random.Next(MIN_BOUNCE_OFF_STEPS, MAX_BOUNCE_OFF_STEPS + 1);
                _currentBounceOffStep = 0;
            }
            else if (++_currentBounceOffStep <= _bounceOffSteps)
            {
                this.UpdateSprite();
                this.UpdateMovement();
            }
            else
            {
                _currentBounceOffStep = 0;
                _bounceOffSteps = 0;
                this.TurnAround();
                _state = InchWormMoveState.FOLLOWING_KEEN;
            }
        }

        private void TurnAround()
        {
            _direction = this.ChangeHorizontalDirection(_direction);
            this.UpdateSprite();
        }

        private void TryFollowKeen()
        {
            _direction = SetDirectionFromObjectHorizontal(_keen, true);
            this.UpdateSprite();
            this.UpdateMovement();
        }

        private void UpdateMovement()
        {
            int xPos = _direction == Direction.LEFT
                ? this.HitBox.Left - MOVE_VELOCITY
                : this.HitBox.Right;

            Rectangle areaToCheck = new Rectangle(xPos, this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            bool isOnEdge = this.IsOnEdge(_direction);
            if (collisions.Any() || isOnEdge)
            {
                TurnAround();
                _state = InchWormMoveState.BOUNCING_OFF_BARRIER;
            }
            else
            {
                int xChange = _direction == Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;
                this.HitBox = new Rectangle(this.HitBox.X + xChange, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
        }

        private void UpdateSprite()
        {
            var sprites = _direction == Direction.LEFT ? _moveLeftSprites : _moveRightSprites;

            if (++_currentSpriteIndex >= sprites.Length)
            {
                _currentSpriteIndex = 0;
            }
            int oldBottom = this.HitBox.Y + _sprite.Height;
            _sprite = sprites[_currentSpriteIndex];
            int newBottom = this.HitBox.Y + _sprite.Height;

            if (oldBottom != newBottom)
            {
                int heightDifference = oldBottom - newBottom;
                this.HitBox = new Rectangle(new Point(this.HitBox.X, this.HitBox.Y + heightDifference), _sprite.Size);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_inchworm_right1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum InchWormMoveState
    {
        FOLLOWING_KEEN,
        BOUNCING_OFF_BARRIER,
        FALLING
    }
}
