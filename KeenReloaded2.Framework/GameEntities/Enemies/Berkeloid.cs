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
    public class Berkeloid : CollisionObject, IUpdatable, IGravityObject, IMoveable, IFireable, IEnemy, ISprite, ICreateRemove
    {
        private bool _isFiring;
        private Enums.Direction _direction;
        private Enums.MoveState _moveState;
        private Image _sprite;
        private CommanderKeen _keen;
        private Rectangle _fieldOfVision;
        private const int VISION_WIDTH = 150;
        private const int VISION_HEIGHT = 100;
        private int _currentMoveSprite;
        private int _currentThrowSprite;
        private const int FALL_VELOCITY = 15;
        private const int MOVE_VELOCITY = 2;
        private const int CHASE_KEEN_DELAY = 40;
        private int _currentChaseKeenDelayTick;
        private const int ATTACK_CHANCE = 40; //set this number to a higher value for less attack probability
        private const int THROW_DELAY = 10;
        private int _currentThrowDelayTick = 0;
        private const int END_ATTACK_DELAY = 4;
        private int _currentEndAttackDelayTick;
        private bool _thrownObject = false;
        private const int MOVE_SPRITE_CHANGE_DELAY = 4;
        private int _currentMoveSpriteChangeDelayTick = 0;
        private int _zIndex;


        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        private Image[] _moveLeftImages = new Image[]
        {
            Properties.Resources.keen4_berkeloid_move_left1,
            Properties.Resources.keen4_berkeloid_move_left2,
            Properties.Resources.keen4_berkeloid_move_left3,
            Properties.Resources.keen4_berkeloid_move_left4
        };

        private Image[] _moveRightImages = new Image[]
        {
            Properties.Resources.keen4_berkeloid_move_right1,
            Properties.Resources.keen4_berkeloid_move_right2,
            Properties.Resources.keen4_berkeloid_move_right3,
            Properties.Resources.keen4_berkeloid_move_right4
        };

        private Image[] _throwLeftImages = new Image[]
        {
            Properties.Resources.keen4_berkeloid_throw_left1,
            Properties.Resources.keen4_berkeloid_throw_left2
        };

        private Image[] _throwRightImages = new Image[]
        {
            Properties.Resources.keen4_berkeloid_throw_right1,
            Properties.Resources.keen4_berkeloid_throw_right2
        };

        public Berkeloid(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = new Rectangle(area.X, area.Y, area.Width, area.Height);
            Initialize();
        }

        private void Initialize()
        {
            SetToRandomDirection();

            if (_collisionGrid == null || _collidingNodes == null)
                return;

            if (GetLandingTile() == null)
            {
                this.MoveState = Enums.MoveState.FALLING;
            }
            else
            {
                this.MoveState = Enums.MoveState.RUNNING;
            }
        }

        private void SetFieldOfVision()
        {
            if (this.Direction == Enums.Direction.LEFT)
            {
                _fieldOfVision = new Rectangle(this.HitBox.Left - VISION_WIDTH, this.HitBox.Y, VISION_WIDTH, VISION_HEIGHT);
            }
            else
            {
                _fieldOfVision = new Rectangle(this.HitBox.Right, this.HitBox.Y, VISION_WIDTH, VISION_HEIGHT);
            }
        }

        private CollisionObject GetLandingTile()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + FALL_VELOCITY);
            var collisions = this.CheckCollision(areaToCheck);
            var debugTiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK);
            var platFormTiles = collisions.Where(c => c.CollisionType == CollisionType.PLATFORM);
            CollisionObject tile = null;

            if (debugTiles.Any())
            {
                int minY = debugTiles.Select(t => t.HitBox.Top).Min();
                tile = debugTiles.FirstOrDefault(t => t.HitBox.Top == minY);
            }
            if (platFormTiles.Any())
            {
                int minY = platFormTiles.Select(t => t.HitBox.Top).Min();
                if (tile == null || tile.HitBox.Top > minY)
                {
                    tile = platFormTiles.FirstOrDefault(t => t.HitBox.Top == minY);
                }
            }

            return tile;
        }

        private CollisionObject GetRightMostLeftTile()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.Left - MOVE_VELOCITY, this.HitBox.Y, MOVE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck);
            var tiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK);
            CollisionObject tile = null;

            if (tiles.Any())
            {
                int maxX = tiles.Select(c => c.HitBox.Right).Max();
                tile = tiles.FirstOrDefault(c => c.HitBox.Right == maxX);
            }

            return tile;
        }

        private CollisionObject GetLeftMostRightTile()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.Right, this.HitBox.Y, MOVE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck);
            var tiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK);
            CollisionObject tile = null;

            if (tiles.Any())
            {
                int minX = tiles.Select(c => c.HitBox.Left).Min();
                tile = tiles.FirstOrDefault(c => c.HitBox.Left == minX);
            }

            return tile;
        }

        private void SetToRandomDirection()
        {
            int val = _random.Next(1, 3);
            this.Direction = val == 1 ? Direction.LEFT : Direction.RIGHT;
        }

        private void UpdateSprite()
        {
            switch (this.MoveState)
            {
                case Enums.MoveState.FALLING:
                default:
                    _sprite = this.Direction == Enums.Direction.LEFT ? _moveLeftImages[0] : _moveRightImages[0];
                    this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                    break;
                case Enums.MoveState.RUNNING:
                    if (_isFiring)
                    {
                        _sprite = this.Direction == Enums.Direction.LEFT ? _throwLeftImages[_currentThrowSprite] : _throwRightImages[_currentThrowSprite];
                    }
                    else
                    {
                        _sprite = this.Direction == Enums.Direction.LEFT ? _moveLeftImages[_currentMoveSprite] : _moveRightImages[_currentMoveSprite];
                    }
                    this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                    break;
            }
        }

        public bool DeadlyTouch
        {
            get { return true; }
        }

        public void HandleHit(IProjectile trajectory)
        {
        }

        public bool IsActive
        {
            get { return false; }
        }


        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                Create(this, args);
            }
        }

        protected void OnRemove(ObjectEventArgs args)
        {
            if (this.Remove != null)
            {
                this.Remove(this, args);
            }
        }

        public void Fire()
        {
            if (_currentThrowDelayTick == THROW_DELAY)
            {
                if (_currentThrowSprite == 0)
                {
                    _currentThrowSprite = 1;
                    UpdateSprite();
                }
                if (!_thrownObject)
                {
                    //throwObject here
                    ThrowFire();
                }
                if (_currentEndAttackDelayTick++ == END_ATTACK_DELAY)
                {
                    //reset from attack state
                    ResetFromThrowingState();
                }
            }
            else
            {
                _currentThrowDelayTick++;
            }
        }

        private void ResetFromThrowingState()
        {
            _currentEndAttackDelayTick = 0;
            _currentThrowDelayTick = 0;
            _currentThrowSprite = 0;
            _isFiring = false;
            _thrownObject = false;
            UpdateSprite();
        }

        private void ThrowFire()
        {
            _thrownObject = true;
            int xPos = this.Direction == Enums.Direction.LEFT ? this.HitBox.Left : this.HitBox.Right - BerkeloidFire.INITIAL_WIDTH;
            BerkeloidFire fire = new BerkeloidFire(_collisionGrid, new Rectangle(xPos, this.HitBox.Y + 25, BerkeloidFire.INITIAL_WIDTH, BerkeloidFire.INITIAL_HEIGHT), _zIndex, this.Direction);
            fire.Remove += new EventHandler<ObjectEventArgs>(fire_Remove);
            ObjectEventArgs args = new ObjectEventArgs()
            {
                ObjectSprite = fire
            };
            OnCreate(args);
        }

        void fire_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        public bool IsFiring
        {
            get { return _isFiring; }
        }

        public int Ammo
        {
            get { return -1; }
        }

        public void Move()
        {
            bool changedDirection = false;
            var spriteSet = this.Direction == Enums.Direction.LEFT ? _moveLeftImages : _moveRightImages;
            int xOffset = this.Direction == Enums.Direction.LEFT ? this.HitBox.Width * -1 : this.HitBox.Width;
            int velocty = this.Direction == Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;

            CollisionObject moveCollideTile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile() : GetLeftMostRightTile();

            if (moveCollideTile != null)
            {
                int xPos = this.Direction == Enums.Direction.LEFT ? moveCollideTile.HitBox.Right + 1 : moveCollideTile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xPos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                ChangeDirection();
                changedDirection = true;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + velocty, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }

            if (!changedDirection)
            {
                ChaseKeen();
            }
            KillKeen();

            if (TryAttackKeen())
                return;

            if (_currentMoveSpriteChangeDelayTick >= MOVE_SPRITE_CHANGE_DELAY)
            {
                _currentMoveSpriteChangeDelayTick = 0;
                if (_currentMoveSprite++ >= spriteSet.Length - 1)
                {
                    _currentMoveSprite = 0;
                }
            }
            else
            {
                _currentMoveSpriteChangeDelayTick++;
            }

            if (IsOnEdge(xOffset))
            {
                ChangeDirection();
            }

            UpdateSprite();
        }

        private bool TryAttackKeen()
        {
            int attackVal = _random.Next(0, ATTACK_CHANCE);
            if (attackVal == 0)
            {
                _isFiring = true;
                UpdateSprite();
                return true;
            }
            return false;
        }

        private bool IsOnEdge(int xOffset)
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Bottom, this.HitBox.Width, FALL_VELOCITY);
            var tiles = this.CheckCollision(areaToCheck, true);
          
            if (!tiles.Any())
            {
                return true;
            }
            return false;
        }

        private void KillKeen()
        {
            if (_keen != null)
            {
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    _keen.Die();
                }
            }
        }

        private void ChaseKeen()
        {
            if (_keen != null)
            {
                if ((this.Direction == Enums.Direction.LEFT && this.HitBox.Right < _keen.HitBox.Left) ||
                    (this.Direction == Enums.Direction.RIGHT && this.HitBox.Left > _keen.HitBox.Right))
                {
                    if (_currentChaseKeenDelayTick++ == CHASE_KEEN_DELAY)
                    {
                        _currentChaseKeenDelayTick = 0;
                        ChangeDirection();
                    }
                }
                else
                {
                    _currentChaseKeenDelayTick = 0;
                }
            }
        }

        private void ChangeDirection()
        {
            this.Direction = this.Direction == Enums.Direction.LEFT ? Direction.RIGHT : Direction.LEFT;
            _currentMoveSprite = 0;
        }

        public void Stop()
        {
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
                if (this.HitBox != null && _collisionGrid != null && _collidingNodes != null)
                {
                    SetFieldOfVision();
                    if (this.MoveState == Enums.MoveState.RUNNING)
                    {
                        this.UpdateCollisionNodes(this.Direction);
                    }
                    else if (this.MoveState == Enums.MoveState.FALLING)
                    {
                        this.UpdateCollisionNodes(Direction.DOWN);
                    }
                }
            }
        }

        public Enums.MoveState MoveState
        {
            get
            {
                return _moveState;
            }
            set
            {
                _moveState = value;
                UpdateSprite();
            }
        }

        public Enums.Direction Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                UpdateSprite();
                SetFieldOfVision();
            }
        }

        public void Update()
        {
            if (this.MoveState == Enums.MoveState.FALLING)
            {
                this.Fall();
            }
            else if (this.MoveState == Enums.MoveState.RUNNING)
            {
                if (!_isFiring)
                {
                    _keen = this.GetClosestPlayer();
                    this.Move();
                }
                else
                {
                    this.Fire();
                }
            }
        }

        public void Jump()
        {
        }

        public bool CanJump
        {
            get { return false; }
        }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public void Fall()
        {
            CollisionObject landingTile = GetLandingTile();
            if (landingTile == null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + FALL_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, landingTile.HitBox.Top - this.HitBox.Height - 5, this.HitBox.Width, this.HitBox.Height);
                this.MoveState = Enums.MoveState.RUNNING;
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_berkeloid_move_left4);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
