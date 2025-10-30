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
    public class BlueEagle : CollisionObject, IUpdatable, ISprite, IEnemy, IStunnable
    {
        private BirdMoveState _state;
        private Image _sprite;
        private CommanderKeen _keen;

        private const int SPRITE_CHANGE_DELAY = 2;
        private int _spriteChangeDelayTick;

        private int _currentWalkSprite = 0;
        private Image[] _walkRightImages = new Image[]
        {
            Properties.Resources.keen4_blue_eagle_walk_right1,
            Properties.Resources.keen4_blue_eagle_walk_right2,
            Properties.Resources.keen4_blue_eagle_walk_right3,
            Properties.Resources.keen4_blue_eagle_walk_right4
        };

        private Image[] _walkLeftImages = new Image[]
        {
            Properties.Resources.keen4_blue_eagle_walk_left1,
            Properties.Resources.keen4_blue_eagle_walk_left2,
            Properties.Resources.keen4_blue_eagle_walk_left3,
            Properties.Resources.keen4_blue_eagle_walk_left4
        };

        private int _currentFlyingImage = 0;
        private Image[] _flyImages = new Image[]
        {
            Properties.Resources.keen4_blue_eagle_fly1,
            Properties.Resources.keen4_blue_eagle_fly2,
            Properties.Resources.keen4_blue_eagle_fly3,
            Properties.Resources.keen4_blue_eagle_fly4,
        };
        private Direction _horizontalDirection;

        private const int STUN_TIME = 100;
        private int _currentStunTimeTick = 0;

        private const int FALL_VELOCITY = 30;
        private const int WALK_VELOCITY = 10;
        private const int FLY_VELOCITY = 10;

        private const int WAIT_TIME = 30;
        private int _currentWaitTimeTick;

        private readonly int _zIndex;
        private Size _walkHitBoxSize = new Size();
        private Size _flyHitBoxSize = new Size();

        public BlueEagle(Rectangle area, SpaceHashGrid grid,  int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            SetDirectionFromKeenLocation();
            this.VerticalDirection = Direction.UP;
            _walkHitBoxSize = _walkLeftImages[0].Size;
        }

        private void SetDirectionFromKeenLocation()
        {
            _keen = this.GetClosestPlayer();
            if (_keen.HitBox.X < this.HitBox.X)
            {
                this.HorizontalDirection = Direction.LEFT;
            }
            else
            {
                this.HorizontalDirection = Direction.RIGHT;
            }
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
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(this.HorizontalDirection);
                    this.UpdateCollisionNodes(this.VerticalDirection);
                }
            }
        }

        public Direction HorizontalDirection
        {
            get
            {
                return _horizontalDirection;
            }
            set
            {
                _horizontalDirection = value;
                UpdateSprite();
            }
        }

        public void Update()
        {
            _keen = this.GetClosestPlayer();
            switch (_state)
            {
                case BirdMoveState.WAITING:
                    this.UpdateWaitState();
                    break;
                case BirdMoveState.WALKING:
                    this.Walk();
                    break;
                case BirdMoveState.FLYING:
                    this.Fly();
                    break;
                case BirdMoveState.STUNNED:
                    this.UpdateStunnedState();
                    break;
            }
        }

        private void UpdateWaitState()
        {
            if (_currentWaitTimeTick++ == WAIT_TIME)
            {
                _currentWaitTimeTick = 0;
                this.Walk();
            }
            this.KillKeenIfColliding();
        }

        private void UpdateStunnedState()
        {
            if (_currentStunTimeTick++ == STUN_TIME)
            {
                _currentStunTimeTick = 0;
                this.Walk();
            }
            else
            {
                this.ExecuteGravity();
            }
        }

        private void ExecuteGravity()
        {
            CollisionObject obj = GetTopMostLandingTile(FALL_VELOCITY);
            if (obj != null)
            {
                this.Land(obj);
            }
            else
            {
                this.Fall();
            }
        }

        private void Fall()
        {
            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + FALL_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            this.UpdateCollisionNodes(Direction.DOWN);
        }

        private void Land(CollisionObject obj)
        {
            if (obj != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                this.UpdateCollisionNodes(Direction.DOWN);
            }
        }
        private void Walk()
        {
            if (this.State != BirdMoveState.WALKING)
            {
                this.State = BirdMoveState.WALKING;
                _spriteChangeDelayTick = 0;
            }

            SetDirectionFromKeenLocation();

            var spriteSet = this.HorizontalDirection == Direction.LEFT ? _walkLeftImages : _walkRightImages;
            if (_currentWalkSprite++ == spriteSet.Length)
            {
                _currentWalkSprite = 0;
            }

            if (_spriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _spriteChangeDelayTick = 0;
                UpdateSprite();

            }

            if (this.HorizontalDirection == Direction.LEFT)
            {
                var collisions = this.CheckCollision(new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height - 1), true);
                CollisionObject leftTile = GetRightMostLeftTile(collisions);
                if (leftTile != null)
                {
                    this.HitBox = new Rectangle(leftTile.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
            }

            var landingTile = GetTopMostLandingTile(FALL_VELOCITY);
            if (landingTile == null)
            {
                this.Fly();
                return;
            }

            if (_keen.HitBox.Bottom < this.HitBox.Top)
            {
                this.Fly();
                return;
            }

            this.HitBox = new Rectangle(this.HitBox.X, landingTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);

            int xOffset = this.HorizontalDirection == Direction.LEFT ? WALK_VELOCITY * -1 : WALK_VELOCITY;
            int xlocation = this.HorizontalDirection == Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xlocation, this.HitBox.Y, this.HitBox.Width + WALK_VELOCITY, this.HitBox.Height);
            var items = this.CheckCollision(areaToCheck, true);
            CollisionObject tile = this.HorizontalDirection == Direction.LEFT ? GetRightMostLeftTile(items) : GetLeftMostRightTile(items);

            if (tile != null)
            {
                int location = this.HorizontalDirection == Direction.LEFT ? tile.HitBox.Right - 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(location, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            KillKeenIfColliding();
        }

        protected override CollisionObject GetTopMostLandingTile(int currentFallVelocity)
        {
            CollisionObject topMostTile;
            Rectangle areaTocheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, currentFallVelocity);
            var items = this.CheckCollision(areaTocheck, true);

            var landingTiles = items.Where(h => h.HitBox.Top >= this.HitBox.Bottom - 2);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        private void Fly()
        {
            if (_keen == null)
                return;

            if (this.State != BirdMoveState.FLYING)
            {
                this.State = BirdMoveState.FLYING;
                _spriteChangeDelayTick = 0;
                SetDirectionFromKeenLocation();
            }

            if (_spriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                if (_currentFlyingImage++ == _flyImages.Length)
                {
                    _currentFlyingImage = 0;
                }
                _spriteChangeDelayTick = 0;
                UpdateSprite();
            }

            var collisions = this.CheckCollision(new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height), true);
          
            CollisionObject tile = GetTopMostLandingTile(FLY_VELOCITY);
            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, _walkHitBoxSize.Width, _flyHitBoxSize.Height);
                if (!this.IsOnEdge(_horizontalDirection) && _keen.HitBox.Bottom >= this.HitBox.Bottom)
                {
                    this.Walk();
                    return;
                }
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, _flyHitBoxSize.Width, _flyHitBoxSize.Height);
            }

            if (this.HitBox.Top >= _keen.HitBox.Top)
            {
                MoveUp();
            }
            else if (this.HitBox.Bottom <= _keen.HitBox.Bottom)
            {
                MoveDown();
            }

            CollisionObject rightTile = GetLeftMostRightTile(collisions);
            if (this.HorizontalDirection == Direction.RIGHT)
            {
                if (rightTile != null)
                {
                    this.HitBox = new Rectangle(rightTile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
            }

            if (this.HorizontalDirection == Direction.LEFT)
            {
                CollisionObject leftTile = GetRightMostLeftTile(collisions);
                if (leftTile != null)
                {
                    this.HitBox = new Rectangle(leftTile.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
            }


            if (this.HitBox.Left > _keen.HitBox.Left)
            {
                MoveLeft();
            }
            else if (this.HitBox.Right < _keen.HitBox.Right)
            {
                MoveRight();
            }
            KillKeenIfColliding();
        }

        private void MoveDown()
        {
            int yOffset = FLY_VELOCITY;
            this.VerticalDirection = Direction.DOWN;
            CollisionObject tile = GetTopMostLandingTile(FLY_VELOCITY);
            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                if (this.State == BirdMoveState.FLYING && !this.IsOnEdge(_horizontalDirection))
                {
                    this.Walk();
                }
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height);
            }
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var tiles = collisions.Where(c => c.HitBox.Bottom - 2 <= this.HitBox.Top).ToList();
            if (tiles.Any())
            {
                int maxBottom = tiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        private void MoveUp()
        {
            int yOffset = FLY_VELOCITY * -1;
            this.VerticalDirection = Direction.UP;
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height + FLY_VELOCITY);
            var collisions = this.CheckCollision(areaToCheck, true);
            CollisionObject tile = GetCeilingTile(collisions);
            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height);
            }
        }

        private void MoveLeft()
        {
            this.HorizontalDirection = Direction.LEFT;
            int xOffset = FLY_VELOCITY * -1;
            Rectangle areaToCheck = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, FLY_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            CollisionObject horizontalTile = GetRightMostLeftTile(collisions);
            if (horizontalTile != null)
            {
                this.HitBox = new Rectangle(horizontalTile.HitBox.Right - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
        }

        private void MoveRight()
        {
            this.HorizontalDirection = Direction.RIGHT;
            int xOffset = FLY_VELOCITY;
            Rectangle areaToCheck = new Rectangle(this.HitBox.Right, this.HitBox.Y, FLY_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            CollisionObject horizontalTile = GetLeftMostRightTile(collisions);
            if (horizontalTile != null)
            {
                this.HitBox = new Rectangle(horizontalTile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
        }

        private void KillKeenIfColliding()
        {
            if (this.CollidesWith(_keen))
            {
                _keen.Die();
            }
        }

        public bool DeadlyTouch
        {
            get { return this.State != BirdMoveState.STUNNED; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.Stun();
        }

        public bool IsActive
        {
            get { return true; }
        }

        public void Stun()
        {
            _currentStunTimeTick = 0;
            this.State = BirdMoveState.STUNNED;
        }

        public bool IsStunned
        {
            get { return this.State == BirdMoveState.STUNNED; }
        }

        private BirdMoveState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            Rectangle previousHitbox = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            switch (_state)
            {
                case BirdMoveState.WAITING:
                    _sprite = this.HorizontalDirection == Direction.LEFT ? _walkLeftImages[0] : _walkRightImages[0];
                    break;
                case BirdMoveState.WALKING:
                    var spriteSet = this.HorizontalDirection == Direction.LEFT ? _walkLeftImages : _walkRightImages;
                    if (_currentWalkSprite >= spriteSet.Length)
                    {
                        _currentWalkSprite = 0;
                    }
                    _sprite = spriteSet[_currentWalkSprite];
                    _walkHitBoxSize = new Size(_sprite.Width, _sprite.Height);
                    break;
                case BirdMoveState.STUNNED:
                    _sprite = Properties.Resources.keen4_blue_eagle_stunned;
                    break;
                case BirdMoveState.FLYING:
                    if (_currentFlyingImage >= _flyImages.Length)
                    {
                        _currentFlyingImage = 0;
                    }
                    _sprite = _flyImages[_currentFlyingImage];
                    _flyHitBoxSize = new Size(_sprite.Width, _sprite.Height);
                    break;
            }
            int xDifference = _sprite.Width - previousHitbox.Width;
            int yDifference = _sprite.Height - previousHitbox.Height;
            this.HitBox = new Rectangle(new Point(this.HitBox.X - xDifference, this.HitBox.Y - yDifference), _sprite.Size);
        }

        public Direction VerticalDirection { get; set; }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public bool CanUpdate => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_blue_eagle_egg);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum BirdMoveState
    {
        WAITING,
        WALKING,
        FLYING,
        STUNNED
    }
}
