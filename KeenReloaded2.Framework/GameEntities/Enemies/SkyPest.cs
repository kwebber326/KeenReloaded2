using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class SkyPest : CollisionObject, IUpdatable, IEnemy, IMoveable, ISprite, ISquashable, IZombieBountyEnemy
    {
        private Image _sprite;
        private Enums.Direction _direction;
        private SkyPestState _state;
        private CommanderKeen _keen;
        private Random _random = new Random();

        private int _currentFlyingImage;
        private const int WAIT_STATE_CHANGE_DELAY = 15;
        private const int LICK_SPRITE_CHANGE_DELAY = 1;
        private int _currentWaitStateChangeDelayTick;

        private const int FLY_DISTANCE = 10;
        private int _currentFlyDistance;
        private const int FLY_VELOCITY_HORIZONTAL = 20;
        private const int FLY_VELOCITY_VERTICAL = 20;
        private const int WAIT_STATES = 8;
        private const int WAIT_CYCLES = 2;
        private int _currentWaitCycle = 1;

        private const int CHASE_KEEN_CHANCE = 3;
        private readonly int _zIndex;
        private Image[] _leftFlyingImages = new Image[]
        {
            Properties.Resources.keen4_skypest_flyleft1,
            Properties.Resources.keen4_skypest_flyleft2
        };

        private Image[] _rightFlyingImages = new Image[]
        {
            Properties.Resources.keen4_skypest_fly_right1,
            Properties.Resources.keen4_skypest_fly_right2
        };

        private int _currentStandingImage;
        private Image[] _standingImages = new Image[]
        {
            Properties.Resources.keen4_skypest_wait,
            Properties.Resources.keen4_skypest_wait_lick1,
            Properties.Resources.keen4_skypest_wait_lick2,
            Properties.Resources.keen4_skypest_wait_lick3,
            Properties.Resources.keen4_skypest_wait_lick4,
            Properties.Resources.keen4_skypest_wait_lick5,
            Properties.Resources.keen4_skypest_wait_lick6,
            Properties.Resources.keen4_skypest_wait_lick7,
        };

        public SkyPest(Rectangle area, SpaceHashGrid grid,  int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            this.State = SkyPestState.FLYING;
            SetRandomInitialFlyingDirection();
        }

        public void Update()
        {
            switch (this.State)
            {
                case SkyPestState.FLYING:
                    _keen = this.GetClosestPlayer();
                    this.Move();
                    break;
                case SkyPestState.STANDING:
                    _keen = this.GetClosestPlayer();
                    this.UpdateStandingState();
                    break;
                case SkyPestState.SQUASHED:
                    this.BasicFall(30);
                    break;
            }
        }

        private void UpdateStandingState()
        {
            switch (_currentStandingImage)
            {
                case 0:
                    UpdateWaitSprite();
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    UpdateLickSprite();
                    break;
                case 8:
                    UpdateWaitSprite();

                    break;
            }
            if (IsNothingBeneath())
            {
                ResetToFlyingFromWaitingState();
            }
        }

        protected override bool IsNothingBeneath()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, 10);
            var items = this.CheckCollision(areaToCheck, true);
            return !items.Any();
        }

        private void UpdateLickSprite()
        {
            if (_currentWaitStateChangeDelayTick++ == LICK_SPRITE_CHANGE_DELAY)
            {
                _currentWaitStateChangeDelayTick = 0;
                _currentStandingImage++;
                UpdateSprite();
            }
        }

        void ResetToFlyingFromWaitingState()
        {
            _currentStandingImage = 0;
            _currentWaitStateChangeDelayTick = 0;
            _currentStandingImage = 0;
            _currentWaitCycle = 1;
            SetRandomInitialFlyingDirection();
            this.State = SkyPestState.FLYING;
        }

        private void UpdateWaitSprite()
        {
            if (_currentWaitStateChangeDelayTick++ == WAIT_STATE_CHANGE_DELAY)
            {
                _currentWaitStateChangeDelayTick = 0;
                if (_currentStandingImage++ >= WAIT_STATES)
                {
                    _currentStandingImage = 0;
                    _currentWaitStateChangeDelayTick = 0;
                    _currentStandingImage = 0;
                    if (_currentWaitCycle++ == WAIT_CYCLES)
                    {
                        _currentWaitCycle = 1;
                        SetRandomInitialFlyingDirection();
                        this.State = SkyPestState.FLYING;
                    }
                }
                UpdateSprite();
            }
        }

        public bool DeadlyTouch
        {
            get { return this.State == SkyPestState.FLYING; }
        }

        public void HandleHit(IProjectile projectile)
        {

        }

        public bool IsActive
        {
            get { return false; }
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile;
            var landingTiles = collisions.Where(h => h.HitBox.Top >= this.HitBox.Top &&
                h.HitBox.Right >= this.HitBox.Left
                && h.HitBox.Left <= this.HitBox.Right);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => !(c.CollisionType == CollisionType.PLATFORM)
                && c.HitBox.Bottom <= this.HitBox.Top
                && c.HitBox.Right >= this.HitBox.Left
                && c.HitBox.Left <= this.HitBox.Right).ToList();
            if (debugTiles.Any())
            {
                int maxBottom = debugTiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        private void ChaseKeen()
        {
            if (_keen.HitBox.X < this.HitBox.X)
            {
                int upOrDown = _random.Next(0, 2);
                if (upOrDown == 0)
                {
                    this.Direction = Enums.Direction.UP_LEFT;
                }
                else
                {
                    this.Direction = Enums.Direction.DOWN_LEFT;
                }
            }
            else if (_keen.HitBox.Right > this.HitBox.Right)
            {
                int upOrDown = _random.Next(0, 2);
                if (upOrDown == 0)
                {
                    this.Direction = Enums.Direction.UP_RIGHT;
                }
                else
                {
                    this.Direction = Enums.Direction.DOWN_RIGHT;
                }
            }
        }

        public void Move()
        {
            if (this.State != SkyPestState.FLYING)
            {
                this.State = SkyPestState.FLYING;
            }

            if (_currentFlyDistance++ == FLY_DISTANCE)
            {
                _currentFlyDistance = 0;
                //if (!this.CheckCollision(this.HitBox, true).Any())
                //{
                SetRandomVerticalDirection();
                if (_random.Next(0, CHASE_KEEN_CHANCE + 1) == CHASE_KEEN_CHANCE)
                {
                    ChaseKeen();
                }
                //}
            }

            bool isLeftDirection = this.IsLeftDirection(this.Direction);
            bool isUpDirection = this.IsUpDirection(this.Direction);

            int xOffset = isLeftDirection ? FLY_VELOCITY_HORIZONTAL * -1 : FLY_VELOCITY_HORIZONTAL;
            int yOffset = isUpDirection ? FLY_VELOCITY_VERTICAL * -1 : FLY_VELOCITY_VERTICAL;
            int xPosCheck = isLeftDirection ? this.HitBox.X + xOffset : this.HitBox.X;
            int yPosCheck = isUpDirection ? this.HitBox.Y + yOffset : this.HitBox.Y;
            Rectangle areaToCheck = new Rectangle(xPosCheck, yPosCheck, this.HitBox.Width + FLY_VELOCITY_HORIZONTAL, this.HitBox.Height + FLY_VELOCITY_VERTICAL);
            var collisions = this.CheckCollision(areaToCheck, true);

            var horizontalTile = isLeftDirection ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            var verticalTile = isUpDirection ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);

            UpdateFlyingSprite(isLeftDirection);
            if (verticalTile == null && horizontalTile == null)
            {
                KillKeenIfCollidingDiagonalMovement(areaToCheck, _keen, this.Direction);
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height);
                return;
            }
            else if (verticalTile != null && horizontalTile != null && verticalTile != horizontalTile)
            {
                if (this.CollidesWith(_keen))
                    _keen.Die();

                //go diagonal the other way
                SwitchHorizontalDirection();
                SwitchVerticalDirection();
                return;
            }

            if (horizontalTile != null)
            {
                int xCollidePos = isLeftDirection ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                int yCollidePos = this.HitBox.Y;
                this.HitBox = new Rectangle(xCollidePos, yCollidePos, this.HitBox.Width, this.HitBox.Height);
                SwitchHorizontalDirection();
                if (this.CollidesWith(_keen))
                {
                    _keen.Die();
                }
                return;
            }

            if (verticalTile != null)
            {
                int xCollidePos = this.HitBox.X;
                int yCollidePos = isUpDirection ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                this.HitBox = new Rectangle(xCollidePos, yCollidePos, this.HitBox.Width, this.HitBox.Height);

                if (isUpDirection)
                {
                    var collisionCeiling = this.CheckCollision(this.HitBox, true);
                    if (collisionCeiling.Any())
                    {
                        int newY = collisionCeiling.Select(c => c.HitBox.Bottom + 1).Max();
                        this.HitBox = new Rectangle(this.HitBox.X, newY, this.HitBox.Width, this.HitBox.Height);
                    }
                    SwitchVerticalDirection();
                }
                else if (CanLandOnTile(verticalTile))
                {
                    this.Stop();
                }

                if (this.CollidesWith(_keen))
                {
                    _keen.Die();
                }
                return;
            }

        }

        private void KillKeenIfColliding(Rectangle areaToCheck)
        {
            if (_keen.HitBox.IntersectsWith(areaToCheck) && this.DeadlyTouch)
            {
                _keen.Die();
            }
        }

        private void SwitchHorizontalDirection()
        {
            if (this.Direction == Enums.Direction.DOWN_RIGHT)
            {
                this.Direction = Enums.Direction.DOWN_LEFT;
            }
            else if (this.Direction == Enums.Direction.DOWN_LEFT)
            {
                this.Direction = Enums.Direction.DOWN_RIGHT;
            }
            else if (this.Direction == Enums.Direction.UP_LEFT)
            {
                this.Direction = Enums.Direction.UP_RIGHT;
            }
            else if (this.Direction == Enums.Direction.UP_RIGHT)
            {
                this.Direction = Enums.Direction.UP_LEFT;
            }
        }

        private void SwitchVerticalDirection()
        {
            if (this.Direction == Enums.Direction.DOWN_RIGHT)
            {
                this.Direction = Enums.Direction.UP_RIGHT;
            }
            else if (this.Direction == Enums.Direction.DOWN_LEFT)
            {
                this.Direction = Enums.Direction.UP_LEFT;
            }
            else if (this.Direction == Enums.Direction.UP_LEFT)
            {
                this.Direction = Enums.Direction.DOWN_LEFT;
            }
            else if (this.Direction == Enums.Direction.UP_RIGHT)
            {
                this.Direction = Enums.Direction.DOWN_RIGHT;
            }
        }


        private void UpdateFlyingSprite(bool movingLeft)
        {
            if (_currentFlyingImage++ == (movingLeft ? _leftFlyingImages : _rightFlyingImages).Length)
            {
                _currentFlyingImage = 0;
            }
            UpdateSprite();
        }

        private CollisionObject GetTopMostLandingTile()
        {
            CollisionObject topMostTile;
            Rectangle areaTocheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, FLY_VELOCITY_VERTICAL);
            var items = this.CheckCollision(areaTocheck, true);

            if (!items.Any(x => x.HitBox.Top > this.HitBox.Bottom))
                return null;

            int minY = items.Where(x => x.HitBox.Top >= this.HitBox.Bottom).Select(c => c.HitBox.Top).Min();
            topMostTile = items.FirstOrDefault(t => t.HitBox.Top == minY);
            if (CanLandOnTile(topMostTile))
                return topMostTile;

            return null;
        }

        private bool CanLandOnTile(CollisionObject tile)
        {
            //if (tile.HitBox.Top < this.HitBox.Bottom)
            //    return false;

            if (tile != null)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, tile.HitBox.Y - this.HitBox.Height, this.HitBox.Width, this.HitBox.Height);
                var collisions = this.CheckCollision(areaToCheck, true);
                return !collisions.Any(c => c != tile);
            }
            return false;
        }

        private void SetRandomInitialFlyingDirection()
        {
            int val = _random.Next(0, 2);
            this.Direction = val == 0 ? Direction.UP_LEFT : Direction.UP_RIGHT;
        }

        private void SetRandomVerticalDirection()
        {
            int val = _random.Next(0, 2);
            if (val == 0)
            {
                if (this.Direction == Enums.Direction.DOWN_RIGHT)
                {
                    this.Direction = Enums.Direction.UP_RIGHT;
                }
                else if (this.Direction == Enums.Direction.DOWN_LEFT)
                {
                    this.Direction = Enums.Direction.UP_LEFT;
                }
            }
            else
            {
                if (this.Direction == Enums.Direction.UP_RIGHT)
                {
                    this.Direction = Enums.Direction.DOWN_RIGHT;
                }
                else if (this.Direction == Enums.Direction.UP_LEFT)
                {
                    this.Direction = Enums.Direction.DOWN_LEFT;
                }
            }
        }

        public void Stop()
        {
            this.State = SkyPestState.STANDING;
            this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
            this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
            _currentFlyDistance = 0;
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
                    this.UpdateCollisionNodes(this.Direction);
                }

            }
        }

        private SkyPestState State
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

        public MoveState MoveState
        {
            get
            {
                throw new NotImplementedException();
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
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            var oldSprite = _sprite;
            switch (this.State)
            {
                case SkyPestState.FLYING:

                    if (this.Direction == Enums.Direction.DOWN_LEFT || this.Direction == Enums.Direction.UP_LEFT)
                    {
                        if (_currentFlyingImage >= _leftFlyingImages.Length)
                            _currentFlyingImage = 0;
                        _sprite = _leftFlyingImages[_currentFlyingImage];
                    }
                    else
                    {
                        if (_currentFlyingImage >= _rightFlyingImages.Length)
                            _currentFlyingImage = 0;
                        _sprite = _rightFlyingImages[_currentFlyingImage];
                    }
                    break;
                case SkyPestState.STANDING:
                    if (_currentStandingImage == 0 || _currentStandingImage == 8)
                    {
                        _sprite = _standingImages[0];
                    }
                    else
                    {
                        _sprite = _standingImages[_currentStandingImage];
                    }
                    break;
                case SkyPestState.SQUASHED:
                    _sprite = Properties.Resources.keen4_skypest_dead;
                    break;
            }
            var heightDifference = oldSprite == null ? 0 : oldSprite.Height - _sprite.Height;
            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + heightDifference, _sprite.Width, _sprite.Height);
        }

        public void Squash()
        {
            if (this.State != SkyPestState.SQUASHED)
            {
                this.State = SkyPestState.SQUASHED;
                OnSquashed();
            }
        }

        public bool IsSquashed
        {
            get { return this.State == SkyPestState.SQUASHED; }
        }


        public bool CanSquash
        {
            get { return this.State == SkyPestState.STANDING; }
        }

        public PointItemType PointItem => PointItemType.KEEN4_DOUGHNUT;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;


        public event EventHandler<ObjectEventArgs> Squashed;
        public event EventHandler<ObjectEventArgs> Killed;

        protected void OnSquashed()
        {
            Squashed?.Invoke(this, new ObjectEventArgs() { ObjectSprite = this });
            OnKilled();
        }

        protected void OnKilled()
        {
            this.Killed?.Invoke(this, new ObjectEventArgs() { ObjectSprite = this });
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_skypest_fly_right1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum SkyPestState
    {
        FLYING,
        STANDING,
        SQUASHED
    }
}
