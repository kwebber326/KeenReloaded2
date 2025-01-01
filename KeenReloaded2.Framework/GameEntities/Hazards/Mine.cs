using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Mine : Hazard, IUpdatable, IMoveable, ICreateRemove, IExplodable
    {
        private Direction _direction;
        private Rectangle _bounds;
        private const int VELOCITY = 5;
        private const int SPRITE_UPDATE_DELAY = 6;
        private int _currentDelayTick = 0;
        private int _currentExplodeSprite = 0;
        private bool _exploded = false;
        public event EventHandler<ObjectEventArgs> Remove;
        public event EventHandler<ObjectEventArgs> Create;
        private ExplosionState _explosionState;

        private Image[] _explodedSprites = new Image[]
        {
             Properties.Resources.keen4_mine_exploded1,
             Properties.Resources.keen4_mine_exploded2
        };

        public Mine(Rectangle area, SpaceHashGrid grid, Direction initialDirection, int boundsX, int boundsY, int boundsWidth, int boundsHeight, int zIndex) 
            : base(grid, area, Enums.HazardType.KEEN4_MINE, zIndex)
        {
            _direction = initialDirection;
            if (boundsX == -1)
                boundsX = area.X;
            if (boundsY == -1)
                boundsY = area.Y;

            _bounds = new Rectangle(boundsX, boundsY, boundsWidth, boundsHeight); 
    
            _explosionState = ExplosionState.NOT_EXPLODING;
        }

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (this.HitBox != null && _collidingNodes != null && _collidingNodes.Any())
                {
                    this.UpdateCollisionNodes(this.Direction);
                }
            }
        }

        public bool IsExploded
        {
            get
            {
                return _exploded;
            }
        }

        public void Explode()
        {
            _exploded = true;
            _explosionState = ExplosionState.EXPLODING;
            EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                GeneralGameConstants.Sounds.KEEN4_MINE_EXPLODE);
        }

        protected void OnRemoved()
        {
            ObjectEventArgs args = new ObjectEventArgs()
            {
                ObjectSprite = this
            };

            if (Remove != null)
            {
                Remove(this, args);
            }
        }

        protected void OnCreate()
        {
            ObjectEventArgs args = new ObjectEventArgs()
            {
                ObjectSprite = this
            };

            Create?.Invoke(this, args);
        }

        public void Update()
        {
            if (!_exploded)
            {
                this.Move();
            }
            else
            {
                UpdateExplodeAnimation();
            }
        }

        private void UpdateExplodeAnimation()
        {
            if (_currentDelayTick == SPRITE_UPDATE_DELAY)
            {
                _currentDelayTick = 0;
                if (_currentExplodeSprite < _explodedSprites.Length)
                {
                    _sprite = _explodedSprites[_currentExplodeSprite++];
                }
                else
                {
                    if (_collidingNodes != null)
                    {
                        foreach (var node in _collidingNodes)
                        {
                            node.Objects.Remove(this);
                        }
                    }
                    OnRemoved();
                }
            }
            else
            {
                _currentDelayTick++;
            }
        }

        public void Move()
        {
            Rectangle areaToCheck = new Rectangle();
            switch (_direction)
            {
                case Enums.Direction.LEFT:
                    areaToCheck = new Rectangle(this.HitBox.X - VELOCITY, this.HitBox.Y, this.HitBox.Width + VELOCITY, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width + VELOCITY, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y - VELOCITY, this.HitBox.Width, this.HitBox.Height + VELOCITY);
                    break;
                case Enums.Direction.DOWN:
                    areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + VELOCITY);
                    break;
            }
            if (!areaToCheck.IntersectsWith(_bounds))
            {
                ChangeDirection();
                return;
            }
            var collisionItems = this.CheckCollision(areaToCheck);
            var collisionWalls = collisionItems.OfType<MaskedTile>();
            var collisionKeens = collisionItems.OfType<CommanderKeen>();

            if (collisionWalls.Any())
            {
                ChangeDirection();
            }
            else
            {
                MoveBasedOnDirection();
            }

            foreach (var keen in collisionKeens)
            {
                keen.Die();
                _exploded = true;
            }
        }

        private void MoveBasedOnDirection()
        {
            switch (this.Direction)
            {
                case Enums.Direction.LEFT:
                    this.HitBox = new Rectangle(this.HitBox.X - VELOCITY, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    this.HitBox = new Rectangle(this.HitBox.X + VELOCITY, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.DOWN:
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + VELOCITY, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - VELOCITY, this.HitBox.Width, this.HitBox.Height);
                    break;
            }
        }

        private void ChangeDirection()
        {
            switch (this.Direction)
            {
                case Enums.Direction.LEFT:
                    this.Direction = Enums.Direction.RIGHT;
                    break;
                case Enums.Direction.RIGHT:
                    this.Direction = Enums.Direction.LEFT;
                    break;
                case Enums.Direction.DOWN:
                    this.Direction = Enums.Direction.UP;
                    break;
                case Enums.Direction.UP:
                    this.Direction = Enums.Direction.DOWN;
                    break;
            }
        }

        public void Stop()
        {

        }

        public MoveState MoveState
        {
            get
            {
                return MoveState.RUNNING;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Direction Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        public ExplosionState ExplosionState => _explosionState;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + $"{separator}{_direction}{separator}{_bounds.X}{separator}{_bounds.Y}{separator}{_bounds.Width}{separator}{_bounds.Height}{separator}{_zIndex}";
        }
    }
}
