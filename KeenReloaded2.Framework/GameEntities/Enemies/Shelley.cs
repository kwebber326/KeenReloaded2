using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
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

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Shelley : DestructibleObject, IEnemy, IUpdatable, ISprite, ICreateRemove, IExplodable, IZombieBountyEnemy
    {
        private const int MOVE_SPRITE_CHANGE_DELAY = 2;
        private int _currentMoveSpriteChangeDelayTick;
        private int _currentMoveSprite;
        private readonly Image[] _moveLeftSprites = new Image[]
        {
            Properties.Resources.keen5_shelley_left1,
            Properties.Resources.keen5_shelley_left2,
            Properties.Resources.keen5_shelley_left3,
            Properties.Resources.keen5_shelley_left4
        };

        private readonly Image[] _moveRightSprites = new Image[]
        {
            Properties.Resources.keen5_shelley_right1,
            Properties.Resources.keen5_shelley_right2,
            Properties.Resources.keen5_shelley_right3,
            Properties.Resources.keen5_shelley_right4
        };

        private readonly Image[] _jumpLeftSprites = new Image[]
        {
            Properties.Resources.keen5_shelley_jump_left1,
            Properties.Resources.keen5_shelley_jump_left2
        };

        private readonly Image[] _jumpRightSprites = new Image[]
        {
            Properties.Resources.keen5_shelley_jump_right1,
            Properties.Resources.keen5_shelley_jump_right2
        };
        private readonly int _zIndex;
        private Direction _direction;
        private const int FALL_VELOCITY = 20;
        private const int MOVE_VELOCITY = 10;
        private const int CHASE_KEEN_CHANCE = 50;
        private const int INITIAL_LUNGE_DISTANCE = 20;
        private int _currentLungeVelocity = INITIAL_LUNGE_DISTANCE;
        private const int INITIAL_JUMP_HEIGHT = 30;
        private int _currentVerticalVelocity = INITIAL_JUMP_HEIGHT * -1;
        private const int FALL_ACCELERATION = 10;
        private const int MAX_FALL_VELOCITY = 70;
        private const int HORIZONTAL_DECELERATION = 5;
        private const int LOOK_TIME = 35;
        private const int X_OFFSET_FOR_JUMP = 32;
        private const int X_JUMP_DECISION_WIDTH = 80;
        private const int Y_VISION = 1500;
        private int _currentLookTime;


        public Shelley(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            this.Direction = this.GetRandomHorizontalDirection();
            this.State = ShelleyState.FALLING;
        }

        private void Fall()
        {
            if (this.State != ShelleyState.FALLING)
            {
                this.State = ShelleyState.FALLING;
            }

            var tile = this.BasicFallReturnTile(FALL_VELOCITY);
            if (tile != null)
            {
                this.Move();
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
                if (_collisionGrid != null && _collisionGrid != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(this.Direction);
                    if (_currentVerticalVelocity < 0)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.UP);
                    }
                    else if (_currentVerticalVelocity > 0)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                }
            }
        }

        private void Move()
        {
            if (this.State != ShelleyState.MOVING)
            {
                this.State = ShelleyState.MOVING;
            }

            if (this.IsOnEdge(_direction))
            {
                this.Look();
                return;
            }
            if (IsNothingBeneath())
            {
                this.Fall();
                return;
            }
            int xOffset = this.Direction == Enums.Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;
            int xCheckLocation = this.Direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xCheckLocation, this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height);

            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (tile != null)
            {
                int collisionXPos = this.Direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(collisionXPos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    this.Explode();
                    _keen.Die();
                }
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    this.Explode();
                    _keen.Die();
                }
                ChaseKeenByRandomChance();
            }
            UpdateMoveSpriteByDelay();
        }

        private void ChaseKeenByRandomChance()
        {
            int randChaseVal = _random.Next(1, CHASE_KEEN_CHANCE + 1);
            if (randChaseVal == CHASE_KEEN_CHANCE)
            {
                this.Direction = this.SetDirectionFromObjectHorizontal(_keen, true);
            }
        }

        private void UpdateMoveSpriteByDelay()
        {

            if (_currentMoveSpriteChangeDelayTick++ == MOVE_SPRITE_CHANGE_DELAY)
            {
                _currentMoveSpriteChangeDelayTick = 0;
                var spriteSet = this.Direction == Enums.Direction.LEFT ? _moveLeftSprites : _moveRightSprites;
                if (++_currentMoveSprite >= spriteSet.Length)
                {
                    _currentMoveSprite = 0;
                }
                UpdateSprite();
            }
        }
        public override void Die()
        {
            this.Explode();
        }

        public bool DeadlyTouch
        {
            get { return !IsDead(); }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return !IsDead(); }
        }

        public Direction Direction
        {
            get
            {
                return _direction;
            }
            private set
            {
                _direction = value;
                UpdateSprite();
            }
        }

        public void Update()
        {
            this.KillIfOutSideBoundsOfMap(_collisionGrid);
            _keen = this.GetClosestPlayer();
            switch (_state)
            {
                case ShelleyState.FALLING:
                    this.Fall();
                    return;
                case ShelleyState.JUMPING:
                    this.Jump();
                    break;
                case ShelleyState.MOVING:
                    this.Move();
                    break;
                case ShelleyState.LOOKING:
                    this.Look();
                    break;
            }
        }

        private void Look()
        {
            if (this.State != ShelleyState.LOOKING)
            {
                this.State = ShelleyState.LOOKING;
                _sprite = this.Direction == Enums.Direction.LEFT
                    ? Properties.Resources.keen5_shelley_left4
                    : Properties.Resources.keen5_shelley_right4;
                _currentLookTime = 0;
            }

            if (this.IsNothingBeneath())
            {
                this.Fall();
            }

            if (_currentLookTime++ == LOOK_TIME)
            {
                this.Direction = ChangeHorizontalDirection(this.Direction);
                this.Move();
            }
            else if (this.Direction == Enums.Direction.RIGHT)
            {
                CheckForJumpRight();
            }
            else if (this.Direction == Enums.Direction.LEFT)
            {
                CheckForJumpLeft();
            }
        }

        private void CheckForJumpLeft()
        {
            if (_keen.HitBox.Bottom < this.HitBox.Top)
                return;

            var ending = this.HitBox.Left - X_OFFSET_FOR_JUMP;
            var beginning = this.HitBox.Left - X_OFFSET_FOR_JUMP - X_JUMP_DECISION_WIDTH;
            Rectangle areaToCheck = new Rectangle(ending, this.HitBox.Bottom, X_JUMP_DECISION_WIDTH, Y_VISION);
            var collisions = this.CheckCollision(areaToCheck, true);
            collisions = collisions.Where(c => (c.HitBox.Left <= _keen.HitBox.Right && c.HitBox.Right >= _keen.HitBox.Left)).ToList();
            var landingTile = this.GetTopMostLandingTile(collisions);
            if (landingTile != null
                && _keen.HitBox.Top < landingTile.HitBox.Top
                && _keen.HitBox.Right >= beginning
                && _keen.HitBox.Left <= ending)
            {
                this.Jump();
            }
        }

        protected override bool IsOnEdge(Direction directionToCheck, int edgeOffset = 0)
        {
            if (directionToCheck == Direction.LEFT)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.Left - this.HitBox.Width + edgeOffset, this.HitBox.Bottom, this.HitBox.Width, 2);
                var tiles = this.CheckCollision(areaToCheck, true);
                return !tiles.Any();
            }
            else if (directionToCheck == Direction.RIGHT)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.Right - edgeOffset, this.HitBox.Bottom, this.HitBox.Width, 2);
                var tiles = this.CheckCollision(areaToCheck, true);
                return !tiles.Any();
            }
            return false;
        }

        private void CheckForJumpRight()
        {
            if (_keen.HitBox.Bottom < this.HitBox.Top)
                return;

            var beginning = this.HitBox.Right + X_OFFSET_FOR_JUMP;
            var ending = this.HitBox.Right + X_OFFSET_FOR_JUMP + X_JUMP_DECISION_WIDTH;
            Rectangle areaToCheck = new Rectangle(beginning, this.HitBox.Bottom, X_JUMP_DECISION_WIDTH, Y_VISION);
            var collisions = this.CheckCollision(areaToCheck, true);
            collisions = collisions.Where(c => (c.HitBox.Left <= _keen.HitBox.Right && c.HitBox.Right >= _keen.HitBox.Left)).ToList();
            var landingTile = this.GetTopMostLandingTile(collisions);
            if (landingTile != null
                && _keen.HitBox.Top < landingTile.HitBox.Top
                && _keen.HitBox.Right >= beginning
                && _keen.HitBox.Left <= ending)
            {
                this.Jump();
            }
        }

        private void Jump()
        {
            if (this.State != ShelleyState.JUMPING)
            {
                this.State = ShelleyState.JUMPING;
                _currentVerticalVelocity = INITIAL_JUMP_HEIGHT * -1;
                _currentLungeVelocity = this.Direction == Enums.Direction.LEFT ? INITIAL_LUNGE_DISTANCE * -1 : INITIAL_LUNGE_DISTANCE;
            }

            int xOffset = _currentLungeVelocity;
            int xPos = this.Direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            int yPos = _currentVerticalVelocity < 0 ? this.HitBox.Y + _currentVerticalVelocity : this.HitBox.Y;
            Rectangle areaToCheck = new Rectangle(xPos, yPos, this.HitBox.Width + Math.Abs(_currentLungeVelocity), this.HitBox.Height + Math.Abs(_currentVerticalVelocity));
            var collisions = this.CheckCollision(areaToCheck, true);

            var horizontalTile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            var verticalTile = _currentVerticalVelocity < 0 ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);

            if (horizontalTile != null)
            {
                int collisionXPos = this.Direction == Enums.Direction.LEFT ? horizontalTile.HitBox.Right : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(collisionXPos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    KillKeenAreal();
                }
            }
            else
            {
                var areaToCheckToKillKeen = new Rectangle(this.Direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X, this.HitBox.Y, Math.Abs(xOffset), this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(areaToCheckToKillKeen))
                {
                    KillKeenAreal();
                }
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            if (verticalTile != null)
            {
                int collisionYPos = _currentVerticalVelocity < 0 ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                this.HitBox = new Rectangle(this.HitBox.X, collisionYPos, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    KillKeenAreal();
                }
                if (_currentVerticalVelocity > 0)
                {
                    this.Explode();
                }
            }
            else
            {
                var areaToCheckToKillKeen = new Rectangle(this.HitBox.X, _currentVerticalVelocity < 0 ? this.HitBox.Y + _currentVerticalVelocity : this.HitBox.Y
                    , this.HitBox.Width, Math.Abs(_currentVerticalVelocity));
                if (_keen.HitBox.IntersectsWith(areaToCheckToKillKeen))
                {
                    KillKeenAreal();
                }
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
            }

            AccelerateGravity();
            DecelerateHorizontalVelocity();
            UpdateSprite();
        }

        private void DecelerateHorizontalVelocity()
        {
            if (this.Direction == Enums.Direction.LEFT)
            {
                if (_currentLungeVelocity + HORIZONTAL_DECELERATION <= 0)
                {
                    _currentLungeVelocity += HORIZONTAL_DECELERATION;
                }
                else
                {
                    _currentLungeVelocity = 0;
                }
            }
            else
            {
                if (_currentLungeVelocity - HORIZONTAL_DECELERATION >= 0)
                {
                    _currentLungeVelocity -= HORIZONTAL_DECELERATION;
                }
                else
                {
                    _currentLungeVelocity = 0;
                }
            }
        }

        private void AccelerateGravity()
        {
            if (_currentVerticalVelocity + FALL_ACCELERATION <= MAX_FALL_VELOCITY)
            {
                _currentVerticalVelocity += FALL_ACCELERATION;
            }
            else
            {
                _currentVerticalVelocity = MAX_FALL_VELOCITY;
            }
        }

        private void KillKeenAreal()
        {
            _keen.Die();
            if (_currentVerticalVelocity > 0)
            {
                this.HitBox = new Rectangle(this.HitBox.X, _keen.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                this.Explode();
            }
        }

        ShelleyState State
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
            switch (_state)
            {
                case ShelleyState.FALLING:
                case ShelleyState.MOVING:
                    var spriteSet = this.Direction == Enums.Direction.LEFT ? _moveLeftSprites : _moveRightSprites;
                    if (_currentMoveSprite >= spriteSet.Length)
                    {
                        _currentMoveSprite = 0;
                    }
                    _sprite = spriteSet[_currentMoveSprite];
                    break;
                case ShelleyState.JUMPING:
                    spriteSet = this.Direction == Enums.Direction.LEFT ? _jumpLeftSprites : _jumpRightSprites;
                    _sprite = _currentVerticalVelocity < 0 ? spriteSet[0] : spriteSet[1];
                    break;
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private Image _sprite;
        private ShelleyState _state;
        private Enums.ExplosionState _explodeState = Enums.ExplosionState.NOT_EXPLODING;
        private CommanderKeen _keen;

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
                if (args.ObjectSprite == this)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                    }
                }
                this.Remove(this, args);
            }
        }

        public void Explode()
        {
            //smoke explosion
            ShelleyExplosion explosion = new ShelleyExplosion(_collisionGrid, new Rectangle(this.HitBox.Location, new Size(32, 30)));
            explosion.Create += new EventHandler<ObjectEventArgs>(explosion_Create);
            explosion.Remove += new EventHandler<ObjectEventArgs>(explosion_Remove);
            ObjectEventArgs args = new ObjectEventArgs()
            {
                ObjectSprite = explosion
            };
            OnCreate(args);
            //left shard
            Fragment leftFragment = new Fragment(_collisionGrid, new Rectangle(this.HitBox.X - 31, this.HitBox.Y, 30, 20), Direction.LEFT, FragmentType.KEEN5_SHELLEY, 40, 0);
            leftFragment.Create += new EventHandler<ObjectEventArgs>(explosion_Create);
            leftFragment.Remove += new EventHandler<ObjectEventArgs>(explosion_Remove);
            ObjectEventArgs argsF1 = new ObjectEventArgs()
            {
                ObjectSprite = leftFragment
            };
            OnCreate(argsF1);
            //right shard
            Fragment rightFragment = new Fragment(_collisionGrid, new Rectangle(this.HitBox.Right + 1, this.HitBox.Y, 14, 12), Direction.RIGHT, FragmentType.KEEN5_SHELLEY, 40, 0);
            rightFragment.Create += new EventHandler<ObjectEventArgs>(explosion_Create);
            rightFragment.Remove += new EventHandler<ObjectEventArgs>(explosion_Remove);
            ObjectEventArgs argsF2 = new ObjectEventArgs()
            {
                ObjectSprite = rightFragment
            };
            OnCreate(argsF2);
            //remove this from the map
            var argsKilled = new ObjectEventArgs() { ObjectSprite = this };
            OnRemove(argsKilled);
            OnKilled();
            this.PublishSoundPlayEvent(
                GeneralGameConstants.Sounds.SHELLEY_EXPLOSION);
        }


        void explosion_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void explosion_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public Enums.ExplosionState ExplosionState
        {
            get { return _explodeState; }
        }

        public PointItemType PointItem => PointItemType.KEEN5_CHOCOLATE_MILK;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public bool ExplodesFromProjectileCollision => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_shelley_left2);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
    enum ShelleyState
    {
        MOVING,
        LOOKING,
        JUMPING,
        FALLING
    }
}
