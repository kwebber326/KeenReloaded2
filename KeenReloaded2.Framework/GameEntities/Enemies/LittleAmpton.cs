using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class LittleAmpton : DestructibleObject, IUpdatable, ISprite, IEnemy, IZombieBountyEnemy
    {
        private Image _sprite;
        private LittleAmptonState _state;
        private Pole _currentPole;
        private Keen5ControlPanel _currentControlPanel;
        private Direction _poleClimbDirection = Direction.UP;

        private const int FALL_VELOCITY = 30;
        private const int MOVE_VELOCITY = 7;
        private const int POLE_CLIMB_VELOCITY = 20;
        private const int POLE_CLIMB_VERTICAL_OFFSET = 32;

        private int _currentCalibrateSprite;
        private Dictionary<int, Image> _calibrateStateSprites = SpriteSheet.SpriteSheet.Keen5LittleAmptonCalibrationImages;

        private int _currentMoveSprite;
        private Image[] _moveLeftSprites = new Image[]{
            Properties.Resources.keen5_little_ampton_left1,
            Properties.Resources.keen5_little_ampton_left2,
            Properties.Resources.keen5_little_ampton_left3,
            Properties.Resources.keen5_little_ampton_left4
        };

        private Image[] _moveRightSprites = new Image[]{
            Properties.Resources.keen5_little_ampton_right1,
            Properties.Resources.keen5_little_ampton_right2,
            Properties.Resources.keen5_little_ampton_right3,
            Properties.Resources.keen5_little_ampton_right4
        };

        private int _currentStunnedSprite;
        private Image[] _stunnedSprites = new Image[]{
            Properties.Resources.keen5_little_ampton_stunned1,
            Properties.Resources.keen5_little_ampton_stunned2,
            Properties.Resources.keen5_little_ampton_stunned3,
            Properties.Resources.keen5_little_ampton_stunned4
        };
        private Enums.Direction _direction;
        private CommanderKeen _keen;

        private int _currentWalkSpriteChangeDelayTick;
        private const int WALK_SPRITE_CHANGE_DELAY = 2;

        private int _currentChargeSpriteChangeDelayTick;
        private const int STUNNED_SPRITE_CHANGE_DELAY = 1;
        private readonly int _zIndex;
        private bool _crossedPoleTile;
        private bool _ignorePoleClimb;
        private bool _ignoreCalibration;
        private int _poleMovementDirectionChanges = 0;

        private Timer _ignorePoleClimbTimer = new Timer();
        private Timer _ignorCalibrationTimer = new Timer();

        public LittleAmpton(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            this.State = LittleAmptonState.MOVING;
            this.Health = 1;
            this.Direction = this.GetRandomHorizontalDirection();
            _ignorePoleClimbTimer.Interval = 5000;
            _ignorePoleClimbTimer.Elapsed += _ignoreTimer_Elapsed;

            _ignorCalibrationTimer.Interval = 3000;
            _ignorCalibrationTimer.Elapsed += _ignorCalibrationTimer_Elapsed;
        }

        private void _ignorCalibrationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _ignoreCalibration = false;
            _ignorCalibrationTimer.Stop();
        }

        private void _ignoreTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _ignorePoleClimb = false;
            _ignorePoleClimbTimer.Stop();
        }

        public override void Die()
        {
            this.UpdateStunnedState();
        }

        public void Update()
        {
            this.KillIfOutSideBoundsOfMap(_collisionGrid);
            if (_state != LittleAmptonState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }
            switch (_state)
            {
                case LittleAmptonState.ON_POLE:
                    this.MoveOnPole(_currentPole);
                    break;
                case LittleAmptonState.MOVING:
                    this.Move(MOVE_VELOCITY);
                    break;
                case LittleAmptonState.LOOKING:
                    this.Look();
                    break;
                case LittleAmptonState.CALIBRATING:
                    this.CalibrateMachine();
                    break;
                case LittleAmptonState.STUNNED:
                    this.UpdateStunnedState();
                    break;
            }
        }

        private void DelayPoleClimbLogic()
        {
            _ignorePoleClimb = true;
            _ignorePoleClimbTimer.Start();
        }

        private void DelayCalibrationLogic()
        {
            _ignoreCalibration = true;
            _ignorCalibrationTimer.Start();
        }

        private void UpdateStunnedState()
        {
            if (this.State != LittleAmptonState.STUNNED)
            {
                this.State = LittleAmptonState.STUNNED;
                if (_keen != null)
                {
                    _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                    _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
                }
                return;
            }

            var spriteIndex = _currentStunnedSprite;
            this.UpdateSpriteByDelay(ref _currentChargeSpriteChangeDelayTick, ref _currentStunnedSprite, STUNNED_SPRITE_CHANGE_DELAY);
            if (_currentStunnedSprite != spriteIndex)
            {
                var image = _stunnedSprites[_currentStunnedSprite];
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - (image.Size.Height - this.HitBox.Height), image.Width, image.Height);
            }

            if (IsNothingBeneath())
            {
                BasicFall(FALL_VELOCITY);
            }
        }

        private void CalibrateMachine()
        {
            if (this.State != LittleAmptonState.CALIBRATING)
            {
                this.State = LittleAmptonState.CALIBRATING;
                _currentCalibrateSprite = 0;
            }

            if (++_currentCalibrateSprite >= _calibrateStateSprites.Count)
            {
                _currentCalibrateSprite = 0;
                DelayCalibrationLogic();

                this.Move(MOVE_VELOCITY);
                return;
            }

            _sprite = _calibrateStateSprites[_currentCalibrateSprite];
        }

        private void Look()
        {
            if (this.State != LittleAmptonState.LOOKING)
            {
                this.State = LittleAmptonState.LOOKING;
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
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        protected override Direction ChangeHorizontalDirection(Direction direction)
        {
            _currentPole = null;
            _currentControlPanel = null;
            _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
            _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
            return base.ChangeHorizontalDirection(direction);
        }

        private void Move(int velocity)
        {
            if (this.State != LittleAmptonState.MOVING)
            {
                this.State = LittleAmptonState.MOVING;
                _crossedPoleTile = false;
            }



            bool nothingBeneath = IsNothingBeneath();
            if (nothingBeneath)
            {
                this.BasicFall(FALL_VELOCITY);
                this.UpdateCollisionNodes(Enums.Direction.DOWN);
            }
            else
            {
                if (IsOnEdge(this.Direction, 3))
                {
                    this.Direction = this.ChangeHorizontalDirection(this.Direction);
                    return;
                }

                int xOffset = this.Direction == Enums.Direction.LEFT ? velocity * -1 : velocity;
                int xPos = this.Direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
                Rectangle areaToCheck = new Rectangle(xPos, this.HitBox.Y, this.HitBox.Width + velocity, this.HitBox.Height);
                var collisions = this.CheckCollision(areaToCheck);
                //pole collisions
                var poles = collisions.OfType<Pole>();
                var panels = collisions.OfType<Keen5ControlPanel>();

                var tile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);

                if (tile != null)
                {
                    if (this.Direction == Enums.Direction.LEFT)
                    {
                        xOffset = this.HitBox.X - (tile.HitBox.Right + 1);
                        this.HitBox = new Rectangle(tile.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                    else
                    {
                        xOffset = tile.HitBox.Left - this.HitBox.Width - 1 - this.HitBox.X;
                        this.HitBox = new Rectangle(tile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                    this.Direction = this.ChangeHorizontalDirection(this.Direction);
                }
                else if (!_ignorePoleClimb && poles.Any(p => p != _currentPole))
                {
                    int closestX = this.HitBox.X;
                    poles = poles.Where(p => (this.HitBox.Bottom - POLE_CLIMB_VERTICAL_OFFSET) <= p.HitBox.Bottom).ToList();
                    if (poles.Any())
                    {
                        if (this.Direction == Enums.Direction.LEFT)
                        {
                            closestX = poles.Select(p => p.HitBox.Right).Max();
                        }
                        else
                        {
                            closestX = poles.Select(p => p.HitBox.Right).Min();
                        }
                        this.HitBox = new Rectangle(closestX - this.HitBox.Width / 2, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        var pole = poles.FirstOrDefault(p => p.HitBox.Right == closestX);
                        if (pole != null)
                            this.MoveOnPole(pole);
                    }
                    else
                    {
                        ExecuteFreeMoveLogic(xOffset, areaToCheck);
                    }
                }
                else if (panels.Any(p => p != _currentControlPanel))
                {
                    int closestX = this.HitBox.X;
                    if (this.Direction == Enums.Direction.LEFT)
                    {
                        closestX = panels.Select(p => p.HitBox.Right).Max();
                    }
                    else
                    {
                        closestX = panels.Select(p => p.HitBox.Right).Min();
                    }

                    var panel = panels.FirstOrDefault(p => p.HitBox.Right == closestX);
                    if (!_ignoreCalibration && panel != null && this.HitBox.IntersectsWith(panel.CalibrateHitbox))
                    {
                        this.HitBox = new Rectangle((panel.HitBox.X + panel.HitBox.Width / 2) - (this.HitBox.Width / 2), this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        this.BeingMachineCalibration(panel);
                        ExecuteFreeMoveLogic(xOffset, areaToCheck);
                    }
                    else
                    {
                        ExecuteFreeMoveLogic(xOffset, areaToCheck);
                    }
                }
                else
                {
                    ExecuteFreeMoveLogic(xOffset, areaToCheck);
                }
            }
        }

        private void ExecuteFreeMoveLogic(int xOffset, Rectangle areaToCheck)
        {
            Rectangle pushAreaToCheck = this.Direction == Enums.Direction.LEFT
                ? new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, xOffset * -1, this.HitBox.Height)
                : new Rectangle(this.HitBox.Right, this.HitBox.Y, xOffset, this.HitBox.Height);
            this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            this.UpdateSpriteByDelay(ref _currentWalkSpriteChangeDelayTick, ref _currentMoveSprite, WALK_SPRITE_CHANGE_DELAY);

            if (_keen.HitBox.IntersectsWith(pushAreaToCheck))
            {
                _keen.SetKeenPushState(this.Direction, true, this);
                _keen.GetMovedHorizontally(this, this.Direction, MOVE_VELOCITY);
            }
            else
            {
                _keen.SetKeenPushState(this.Direction, false, this);
            }
        }


        private void UpdateSpriteByDelay(ref int delayTicker, ref int spriteIndex, int delayThreshold)
        {
            if (delayTicker++ == delayThreshold)
            {
                delayTicker = 0;
                spriteIndex++;
                UpdateSprite();
            }
        }

        private void KillKeenBasedOnYPosition(int yOffset)
        {
            if (!this.IsDead())
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, yOffset < 0 ? this.HitBox.Y + yOffset : this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + Math.Abs(yOffset));
                var collisions = this.CheckCollision(areaToCheck);
                var keens = collisions.Where(c => c.CollisionType == CollisionType.PLAYER);
                if (keens.Any())
                {
                    foreach (var keen in keens)
                    {
                        ((CommanderKeen)keen).Die();
                    }
                }
            }
        }

        private void BeingMachineCalibration(Keen5ControlPanel panel)
        {
            _currentControlPanel = panel;
            if (_currentControlPanel == null)
            {
                this.Move(MOVE_VELOCITY);
                return;
            }
            this.CalibrateMachine();
        }

        private void MoveOnPole(Pole p)
        {
            _currentPole = p;
            if (_currentPole == null)
            {
                this.Move(MOVE_VELOCITY);
                return;
            }

            if (this.State != LittleAmptonState.ON_POLE)
            {
                this.State = LittleAmptonState.ON_POLE;
                var landingTile = this.GetTopMostLandingTile(FALL_VELOCITY);
                if (landingTile?.CollisionType == CollisionType.POLE_TILE)
                {
                    _poleClimbDirection = this.GetRandomVerticalDirection();
                }
                else
                {
                    _poleClimbDirection = Direction.UP;
                }
            }

            if (_poleClimbDirection == Enums.Direction.UP)
            {
                int yOffset = POLE_CLIMB_VELOCITY * -1;
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height + POLE_CLIMB_VELOCITY);
                var collisions = this.CheckCollision(areaToCheck, true);

                KillKeenBasedOnYPosition(yOffset);

                var ceilingTile = GetCeilingTile(collisions);

                if (collisions.Any(c => c.CollisionType == CollisionType.POLE_TILE))
                    _crossedPoleTile = true;

                if (ceilingTile != null)
                {
                    this.HitBox = new Rectangle(this.HitBox.X, ceilingTile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                    _poleClimbDirection = Enums.Direction.DOWN;
                }
                else
                {
                    Rectangle newLocation = new Rectangle(this.HitBox.X, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height);
                    var allCollisions = this.CheckCollision(newLocation);
                    var poles = allCollisions.Where(c => c.CollisionType == CollisionType.POLE || c.CollisionType == CollisionType.POLE_TILE);
                    if (poles.Any())
                    {
                        this.HitBox = newLocation;
                    }
                    else
                    {
                        _poleClimbDirection = Direction.DOWN;
                        _poleMovementDirectionChanges++;
                    }
                }
                this.UpdateCollisionNodes(Enums.Direction.UP);
            }
            else
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + POLE_CLIMB_VELOCITY);
                Rectangle newLocation = new Rectangle(this.HitBox.X, this.HitBox.Y + POLE_CLIMB_VELOCITY, this.HitBox.Width, this.HitBox.Height);

                var landingTile = GetTopMostLandingTile(POLE_CLIMB_VELOCITY);
                if ((landingTile != null && !(landingTile.CollisionType == CollisionType.POLE_TILE))
                 || (_crossedPoleTile && landingTile?.CollisionType == CollisionType.POLE_TILE))
                {
                    TransitionToMovingFromPoleClimb(landingTile);
                }
                else
                {
                    if (landingTile != null && landingTile.CollisionType == CollisionType.POLE_TILE)
                        _crossedPoleTile = true;

                    KillKeenBasedOnYPosition(POLE_CLIMB_VELOCITY);
                   // this.HitBox = new Rectangle(this.HitBox.X, _currentPole.HitBox.Bottom - this.HitBox.Height, this.HitBox.Width, this.HitBox.Height);
                    landingTile = GetTopMostLandingTile(POLE_CLIMB_VERTICAL_OFFSET);
                    var otherCollisions = this.CheckCollision(areaToCheck);
                    var poles = otherCollisions.Where(pole => pole.CollisionType == CollisionType.POLE);
                    if (landingTile == null)
                    {
                        if (poles.Any())
                        {
                            this.HitBox = newLocation;
                        }
                        else 
                        {
                            _poleMovementDirectionChanges++;
                            _poleClimbDirection = Enums.Direction.UP;
                        }
                    }
                    else if (_poleMovementDirectionChanges > 1 || landingTile.CollisionType != CollisionType.POLE_TILE)
                    {
                        TransitionToMovingFromPoleClimb(landingTile);
                    }
                    else if (poles.Any())
                    {
                        this.HitBox = newLocation;
                    }
                }

                this.UpdateCollisionNodes(Enums.Direction.DOWN);
            }

        }

        private void TransitionToMovingFromPoleClimb(CollisionObject landingTile)
        {
            this.HitBox = new Rectangle(this.HitBox.X, landingTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
            _poleClimbDirection = Enums.Direction.UP;
            _poleMovementDirectionChanges = 0;
            KillKeenBasedOnYPosition(0);
            DelayPoleClimbLogic();
            this.Move(MOVE_VELOCITY);
        }

        LittleAmptonState State
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

        protected override CollisionObject GetTopMostLandingTile(int currentFallVelocity)
        {
            CollisionObject topMostTile;
            Rectangle areaTocheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, currentFallVelocity);
            var items = this.CheckCollision(areaTocheck);

            var landingTiles = items.Where(h => 
                (h.CollisionType == CollisionType.BLOCK 
                || h.CollisionType == CollisionType.PLATFORM 
                || h.CollisionType == CollisionType.POLE_TILE) 
                && h.HitBox.Top >= this.HitBox.Top);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        private void UpdateSprite()
        {
            switch (_state)
            {
                case LittleAmptonState.ON_POLE:
                    _sprite = Properties.Resources.keen5_little_ampton_pole;
                    break;
                case LittleAmptonState.MOVING:
                    var spriteSet = this.Direction == Enums.Direction.LEFT ? _moveLeftSprites : _moveRightSprites;
                    if (_currentMoveSprite >= spriteSet.Length)
                    {
                        _currentMoveSprite = 0;
                    }
                    _sprite = spriteSet[_currentMoveSprite];
                    break;
                case LittleAmptonState.LOOKING:
                    _sprite = Properties.Resources.keen5_little_ampton_look;
                    break;
                case LittleAmptonState.STUNNED:
                    if (_currentStunnedSprite >= _stunnedSprites.Length)
                    {
                        _currentStunnedSprite = 1;
                    }
                    _sprite = _stunnedSprites[_currentStunnedSprite];
                    break;
            }
        }

        public bool DeadlyTouch
        {
            get { return _state == LittleAmptonState.ON_POLE; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return !this.IsDead(); }
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
            }
        }

        public PointItemType PointItem => PointItemType.KEEN5_SHIKADI_GUM;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_little_ampton_look);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum LittleAmptonState
    {
        MOVING,
        LOOKING,
        CALIBRATING,
        ON_POLE,
        STUNNED
    }
}
