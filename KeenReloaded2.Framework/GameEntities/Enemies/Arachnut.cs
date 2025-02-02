using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
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
    public class Arachnut : CollisionObject, IUpdatable, IMoveable, IEnemy, ISprite, IGravityObject, IStunnable
    {
        private Size _layDownSize = new Size(122, 58);
        public static readonly int STAND_WIDTH = 80;
        public static readonly int STAND_HEIGHT = 80;
        private readonly int _zIndex;
        private Size _standSize = new Size(80, 80);
        private Enums.MoveState _moveState;
        private Enums.Direction _direction;

        private Image _sprite;
        private const int FALL_VELOCITY = 25;

        private int _currentRunSprite;

        private bool _awakening = false;
        private int _awakenTick;
        private const int AWAKEN_DELAY = 20;

        private const int STUN_TIME = 100;
        private int _currentStunTick;

        private const int CHASE_DELAY = 10;
        private int _currentChaseDelayTick = 0;

        private CommanderKeen _keen;

        private Image[] _runImages = new Image[]
        {
            Properties.Resources.keen4_arachnut1,
            Properties.Resources.keen4_arachnut2,
            Properties.Resources.keen4_arachnut3,
            Properties.Resources.keen4_arachnut4
        };
        private const int VELOCITY = 17;
        private bool _isLayingDown;

        public Arachnut(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            _direction = Enums.Direction.RIGHT;
            _sprite = Properties.Resources.keen4_arachnut1;
            this.MoveState = Enums.MoveState.RUNNING;
            _keen = this.GetClosestPlayer();
        }

        public void Update()
        {
            if (NothingBelow())
            {
                Fall();
            }
            else
            {
                if (this.MoveState == Enums.MoveState.RUNNING)
                {
                    this.Move();
                }
                else
                {
                    UpdateStunnedState();
                }
            }
        }

        private void UpdateStunnedState()
        {
            if (_awakening || _currentStunTick++ == STUN_TIME)
            {
                _awakening = true;
                UpdateSprite();
                if (_awakenTick++ == AWAKEN_DELAY)
                {
                    this.MoveState = Enums.MoveState.RUNNING;
                    _keen = this.GetClosestPlayer();
                    _awakenTick = 0;
                    _awakening = false;
                    _currentStunTick = 0;
                    this.Direction = _keen != null && _keen.HitBox.Right < this.HitBox.Left ? Enums.Direction.LEFT : Enums.Direction.RIGHT;
                    this.IsLayingDown = false;
                    KillCollidingKeens();
                }
            }
        }

        private void KillCollidingKeens()
        {
            var items = this.CheckCollision(this.HitBox);
            var keens = items.OfType<CommanderKeen>()?.ToList() ?? new List<CommanderKeen>();
            foreach (var keen in keens)
            {
                keen.Die();
            }
        }

        public bool DeadlyTouch
        {
            get { return this.MoveState != Enums.MoveState.STUNNED; }
        }

        public void Move()
        {
            int xOffset = this.Direction == Enums.Direction.LEFT ? VELOCITY * -1 : VELOCITY;
            Rectangle areaToCheck = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            Rectangle areaBelowToCheck = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + 2);
            var collisionItems = this.CheckCollision(areaToCheck);
            var collisionWalls = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK);
            UpdateCollisionNodes(this.Direction);
            var collisionWallsBelow = this.CheckCollision(areaBelowToCheck, true).Where(t => t.HitBox.Y > this.HitBox.Y).ToList();
            bool collisionItemsAhead;
            bool onEdge;
            bool keenEscaping;
            int xPos = this.HitBox.X;
            int yPos = this.HitBox.Y;
            _keen = this.GetClosestPlayer();
            if (this.Direction == Enums.Direction.LEFT)
            {
                collisionItemsAhead = collisionWalls.Any(w => w.HitBox.Right >= this.HitBox.Left + xOffset && w.HitBox.Top < this.HitBox.Bottom);
                onEdge = !collisionWallsBelow.Any(w => w.HitBox.Left <= this.HitBox.Left);
                keenEscaping = _keen != null && _keen.HitBox.Left > this.HitBox.Right;
                if (onEdge)
                {
                    if (collisionWallsBelow.Any())
                    {
                        xPos = collisionWallsBelow
                            .Select(c => c.HitBox.X).Min();
                        yPos = this.GetTopMostLandingTile(collisionWallsBelow).HitBox.Y - this.HitBox.Height - 1;
                    }
                    ChangeDirection();
                }
                else if (collisionItemsAhead)
                {
                    xPos = collisionWalls
                        .Where(w => w.HitBox.Right >= this.HitBox.Left + xOffset && w.HitBox.Top < this.HitBox.Bottom)
                        .Select(t => t.HitBox.Right + 1).Max();
                    ChangeDirection();
                }
                else if (_currentChaseDelayTick == CHASE_DELAY)
                {
                    _currentChaseDelayTick = 0;
                    if (keenEscaping)
                        ChangeDirection();
                }
                else
                {
                    xPos += xOffset;
                }
                _currentChaseDelayTick++;
                this.HitBox = new Rectangle(xPos, yPos, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                collisionItemsAhead = collisionWalls.Any(w => w.HitBox.Left <= this.HitBox.Right + xOffset && w.HitBox.Top < this.HitBox.Bottom);
                onEdge = !collisionWallsBelow.Any(w => w.HitBox.Right >= this.HitBox.Right);
                keenEscaping = _keen != null && _keen.HitBox.Right < this.HitBox.Left;

                if (onEdge)
                {
                    if (collisionWallsBelow.Any())
                    {
                        xPos = collisionWallsBelow
                            .Select(c => c.HitBox.Right - this.HitBox.Width).Max();
                        yPos = this.GetTopMostLandingTile(collisionWallsBelow).HitBox.Y - this.HitBox.Height - 1;
                    }
                    ChangeDirection();
                    _currentChaseDelayTick = 0;
                }
                else if (collisionItemsAhead)
                {
                    _currentChaseDelayTick = 0;
                    xPos = collisionWalls
                        .Where(w => w.HitBox.Left <= this.HitBox.Right + xOffset && w.HitBox.Top < this.HitBox.Bottom)
                        .Select(t => t.HitBox.X - this.HitBox.Width - 1).Min();
                    ChangeDirection();
                }
                else if (_currentChaseDelayTick == CHASE_DELAY)
                {
                    _currentChaseDelayTick = 0;
                    if (keenEscaping)
                        ChangeDirection();
                }
                else
                {
                    xPos += xOffset;
                }
                _currentChaseDelayTick++;
                this.HitBox = new Rectangle(xPos, yPos, this.HitBox.Width, this.HitBox.Height);
            }
            KillCollidingKeens();
        }

        private void ChangeDirection()
        {
            this.Direction = this.Direction == Enums.Direction.LEFT ? Enums.Direction.RIGHT : Enums.Direction.LEFT;
            _currentChaseDelayTick = 0;
        }

        public void Stop()
        {
            this.MoveState = Enums.MoveState.STUNNED;
        }

        public override System.Drawing.Rectangle HitBox
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
                    UpdateSprite();
                    this.UpdateCollisionNodes(this.Direction);
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

        private void UpdateSprite()
        {
            switch (MoveState)
            {
                case Enums.MoveState.RUNNING:
                    if (_currentRunSprite < _runImages.Length - 1)
                        _currentRunSprite++;
                    else
                        _currentRunSprite = 0;

                    _sprite = _runImages[_currentRunSprite];
                    break;
                case Enums.MoveState.STUNNED:
                    if (_awakening)
                    {
                        _currentRunSprite = 0;
                        if (_awakenTick % 4 == 0)
                        {
                            _sprite = Properties.Resources.keen4_arachnut1;
                            this.IsLayingDown = false;
                        }
                        else
                        {
                            _sprite = Properties.Resources.keen4_arachnut_stunned;
                            this.IsLayingDown = true;
                        }
                    }
                    else
                    {
                        _sprite = Properties.Resources.keen4_arachnut_stunned;
                        this.IsLayingDown = true;
                    }
                    break;
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
            }
        }


        public void Jump()
        {

        }

        public bool CanJump
        {
            get { return false; }
        }

        public void Fall()
        {
            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + FALL_VELOCITY, this.HitBox.Width, this.HitBox.Height);
        }

        private bool NothingBelow()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y + FALL_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            var collisionItems = this.CheckCollision(areaToCheck);

            var tilesBelow = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK);
            var platformsBelow = collisionItems.Where(c => c.CollisionType == CollisionType.PLATFORM);
            bool somethingBelow = (tilesBelow.Any() || platformsBelow.Any());
            if (somethingBelow)
            {
                int minYTiles = tilesBelow.Any() ? tilesBelow.Select(t => t.HitBox.Top).Min() : int.MaxValue;
                int minYPLatforms = platformsBelow.Any() ? platformsBelow.Select(t => t.HitBox.Top).Min() : int.MaxValue;
                int minY = minYTiles < minYPLatforms ? minYTiles : minYPLatforms;
                this.HitBox = new Rectangle(this.HitBox.X, minY - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
            }
            return !somethingBelow;
        }

        protected void HandleCollision(IProjectile obj)
        {
            if (obj is KeenStunShot)
            {
                this.Stun();
            }
        }

        public bool IsActive
        {
            get
            {
                return this.MoveState != MoveState.STUNNED;
            }
        }

        public void Stun()
        {
            this.MoveState = Enums.MoveState.STUNNED;
            _currentChaseDelayTick = 0;
            _currentStunTick = 0;
            _awakening = false;
            _awakenTick = 0;
        }

        private bool IsLayingDown
        {
            get
            {
                return _isLayingDown;
            }
            set
            {
                if (_isLayingDown != value)
                {
                    _isLayingDown = value;
                    if (_isLayingDown)
                    {
                        this.HitBox = new Rectangle(new Point(this.HitBox.X, this.HitBox.Y + (STAND_HEIGHT - _layDownSize.Height)), _layDownSize);
                    }
                    else
                    {
                        this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - (STAND_HEIGHT - _layDownSize.Height), STAND_WIDTH, STAND_HEIGHT);
                    }
                }
            }
        }

        public bool IsStunned
        {
            get { return this.MoveState == Enums.MoveState.STUNNED; }
        }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            return $"{nameof(Properties.Resources.keen4_arachnut1)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }

        public void HandleHit(IProjectile projectile)
        {
            this.HandleCollision(projectile);
        }
    }
}
