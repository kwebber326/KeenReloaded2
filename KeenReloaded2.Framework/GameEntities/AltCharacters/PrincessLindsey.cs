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
    public class PrincessLindsey : CollisionObject, ISprite, IUpdatable, IHintMessenger, IMoveable
    {
        private readonly int _zIndex;
        private Image _sprite;

        private bool _messageDelayed;
        private const int MESSAGE_DELAY = 60;
        private int _messageDelayTick;

        private const int SPRITE_CHANGE_DELAY = 4;
        private int _spriteChangeDelayTick;
        private int _currentSpriteIndex;

        private const int SPEED = 2;
        private const int MAX_DISTANCE = 12;

        private readonly int _originalY;

        private Direction _direction;
        private readonly string _hintMessage;
        private Image[] _sprites = SpriteSheet.SpriteSheet.PrincessLindseyImages;

        public PrincessLindsey(Rectangle area, SpaceHashGrid grid, int zIndex, string hintMessage) : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            _sprite = _sprites[_currentSpriteIndex];
            _direction = Direction.DOWN;
            _hintMessage = hintMessage;
            _originalY = this.HitBox.Y;
        }

        public override CollisionType CollisionType => CollisionType.HINT_MESSAGE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public MoveState MoveState { get => MoveState.FALLING; set { } }
        public Direction Direction { get => _direction; set { } }

        public event EventHandler<string> Message;

        public void SendMessage()
        {
            if (!_messageDelayed)
            { 
                Message?.Invoke(this, _hintMessage);
                _messageDelayed = true; 
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
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        public void Update()
        {
            //update sprite
            this.UpdateSpriteByDelayBase(ref _spriteChangeDelayTick, ref _currentSpriteIndex,
                SPRITE_CHANGE_DELAY, () =>
                {
                    if (_currentSpriteIndex >= _sprites.Length)
                        _currentSpriteIndex = 0;

                    _sprite = _sprites[_currentSpriteIndex];
                });

            //movement
            this.Move();

            //delay updating
            if (_messageDelayed && ++_messageDelayTick >= MESSAGE_DELAY)
            {
                _messageDelayTick = 0;
                _messageDelayed = false;
            }
        }

        public void Move()
        {
            int moveVelocity = _direction == Direction.DOWN ? SPEED : SPEED * -1;
            //since the speed is a constant, slow speed of 4, this is fine
            var areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y + moveVelocity,
                this.HitBox.Width, this.HitBox.Height + Math.Abs(moveVelocity));

            var collisions = this.CheckCollision(areaToCheck, true)
                .Where(c => c.CollisionType == CollisionType.BLOCK).ToList();

            if (collisions.Any())
            {
                var tile = _direction == Direction.DOWN ?
                    this.GetTopMostLandingTile(collisions) :
                    this.GetCeilingTile(collisions);

                this.HitBox = _direction == Direction.DOWN ?
                    new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height) :
                    new Rectangle(this.HitBox.X, tile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                    
                Stop();
                return;
            }

            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + moveVelocity, this.HitBox.Width, this.HitBox.Height);
            
            if (_direction == Direction.DOWN && this.HitBox.Y - _originalY >= MAX_DISTANCE
             || _direction == Direction.UP && this.HitBox.Y == _originalY)
            {
                Stop();
            }
        }

        public void Stop()
        {
           _direction = this.ReverseDirection(_direction);
        }

        public override string ToString()
        {
            var initialImageName = nameof(Properties.Resources.princess_lindsey1);
            var separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var area = this.HitBox;
            return $"{initialImageName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_hintMessage}";
        }
    }
}
