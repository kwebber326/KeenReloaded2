using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class NoSpike : DestructibleObject, IUpdatable, ISprite, IEnemy, ICreateRemove, IZombieBountyEnemy
    {
        #region sprite and state variables
        private CommanderKeen _keen;
        private NospikeState _state;
        private NospikeQuestionMark _questionMark;
        private Enums.Direction _direction;
        private Image _sprite;
        private Image[] _patrolLeftSprites, _patrolRightSprites, _chargeLeftSprites, _chargeRightSprites, _stunnedSprites;
        private int _currentPatrolSprite, _currentChargeSprite, _currentStunnedSprite;
        private const int PATROL_SPRITE_CHANGE_DELAY = 2, CHARGE_SPRITE_CHANGE_DELAY = 0, STUNNED_SPRITE_CHANGE_DELAY = 1;
        private int _currentPatrolSpriteChangeDelayTick, _currentChargeSpriteChangeDelayTick, _currentStunnedSpriteChangeDelayTick;
        #endregion

        #region behavior logic variables
        private const int DIRECTION_EDGE_BUFFER = 5;
        private const int RANGE_OF_VISION = 200;
        private const int CHARGE_CHANCE = 20;
        private const int CHARGE_MIN_DISTANCE = 100;
        private int _currentChargeDist;
        private const int CHARGE_STOP_CHANCE = 60;

        private const int LOOK_CHANCE = 80;
        private const int LOOK_TIME = 20;
        private int _lookTimeTick;

        private const int CONFUSED_STAGE_1_END = 5;
        private const int CONFUSED_STAGE_2_END = 25;
        private const int CONFUSED_STAGE_3_END = 30;
        private int _confusedTimeTick;
        private const int QUESTION_MARK_DISTANCE_OVER_HEAD = 10;

        private bool _hitAnimation;
        private const int HIT_ANIMATION_TIME = 1;
        private int _hitAnimationTimeTick;
        #endregion

        #region physics variables

        private const int MAX_FALL_VELOCITY = 40;
        private const int GRAVITY_ACCELERATION = 5;
        private int _verticalVelocity;
        private readonly int _zIndex;
        private const int PATROL_VELOCITY = 5;

        private const int CHARGE_VELOCITY = 20;

        #endregion

        public NoSpike(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            this.Health = 4;

            _patrolLeftSprites = SpriteSheet.SpriteSheet.NospikePatrolLeftImages;
            _patrolRightSprites = SpriteSheet.SpriteSheet.NospikePatrolRightImages;
            _chargeLeftSprites = SpriteSheet.SpriteSheet.NospikeChargeLeftImages;
            _chargeRightSprites = SpriteSheet.SpriteSheet.NospikeChargeRightImages;
            _stunnedSprites = SpriteSheet.SpriteSheet.NospikeStunnedImages;

            this.Direction = this.GetRandomHorizontalDirection();
            this.State = NospikeState.FALLING;
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
                    this.UpdateCollisionNodes(Enums.Direction.DOWN);
                }
            }
        }

        public override void Die()
        {
            this.UpdateStunnedState();
            this.OnKilled();
        }

        public void Update()
        {
            if (_hitAnimation && _hitAnimationTimeTick++ == HIT_ANIMATION_TIME)
            {
                _hitAnimationTimeTick = 0;
                _hitAnimation = false;
            }
            if (_state != NospikeState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }
            switch (_state)
            {
                case NospikeState.LOOKING:
                    this.Look();
                    break;
                case NospikeState.PATROLLING:
                    this.Patrol();
                    break;
                case NospikeState.CHARGING:
                    this.Charge();
                    break;
                case NospikeState.CONFUSED:
                    this.UpdateConfusedState();
                    break;
                case NospikeState.FALLING:
                    this.Fall();
                    break;
                case NospikeState.STUNNED:
                    this.UpdateStunnedState();
                    break;
            }
        }

        private void Look()
        {
            if (this.State != NospikeState.LOOKING)
            {
                _lookTimeTick = 0;
                this.State = NospikeState.LOOKING;
                AdjustHitboxAndSpriteHeight();
            }

            if (IsNothingBeneath())
            {
                this.UpdateConfusedState();
                return;
            }

            if (_lookTimeTick++ == LOOK_TIME)
            {
                this.Patrol();
            }
        }

        private void AdjustHitboxAndSpriteHeight()
        {
            if (_sprite.Height > this.HitBox.Height)
            {
                int heightDiff = _sprite.Height - this.HitBox.Height;
                this.HitBox = new Rectangle(this.HitBox.Location.X, this.HitBox.Location.Y - heightDiff, _sprite.Width, _sprite.Height);
            }
        }

        private bool IsKeenInRangeOfVision()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.Left - RANGE_OF_VISION, this.HitBox.Y, this.HitBox.Width + (RANGE_OF_VISION * 2), this.HitBox.Height);
            return (_keen.HitBox.IntersectsWith(areaToCheck));
        }

        private void Patrol()
        {
            if (this.State != NospikeState.PATROLLING)
            {
                this.State = NospikeState.PATROLLING;
                this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                this.BasicFall(MAX_FALL_VELOCITY);
            }

            if (IsNothingBeneath())
            {
                this.UpdateConfusedState();
                return;
            }

            if (IsOnEdge(this.Direction, DIRECTION_EDGE_BUFFER))
            {
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
            }

            int lookVal = _random.Next(1, LOOK_CHANCE + 1);
            if (lookVal == LOOK_CHANCE)
            {
                this.Look();
                return;
            }

            if (IsKeenInRangeOfVision())
            {
                int chargeVal = _random.Next(1, CHARGE_CHANCE + 1);
                if (chargeVal == CHARGE_CHANCE)
                {
                    this.Charge();
                    return;
                }
            }

            int xOffset = _direction == Enums.Direction.LEFT ? PATROL_VELOCITY * -1 : PATROL_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;

            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width + PATROL_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);

            if (tile != null)
            {
                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                this.KillCollidingPlayers();
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.KillCollidingPlayers(areaToCheck);
            }
            var spriteSet = _direction == Enums.Direction.LEFT ? _patrolLeftSprites : _patrolRightSprites;
            this.UpdateHitboxBasedOnStunnedImage(spriteSet, ref _currentPatrolSprite, ref _currentPatrolSpriteChangeDelayTick, PATROL_SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        private void UpdateConfusedState()
        {
            if (this.State != NospikeState.CONFUSED)
            {
                this.State = NospikeState.CONFUSED;
                AdjustHitboxAndSpriteHeight();
                _confusedTimeTick = 0;
            }

            _confusedTimeTick++;
            if (_confusedTimeTick == CONFUSED_STAGE_1_END)
            {
                //create question mark over head
                SetQuestionMark();
            }
            else if (_confusedTimeTick == CONFUSED_STAGE_2_END)
            {
                //remove question mark
                RemoveQuestionMark();
            }
            else if (_confusedTimeTick == CONFUSED_STAGE_3_END)
            {
                //fall
                this.Fall();
            }
        }

        private void RemoveQuestionMark()
        {
            OnRemove(new ObjectEventArgs() { ObjectSprite = _questionMark });
            _questionMark.Remove -= _questionMark_Remove;
            _questionMark.Create -= _questionMark_Create;
            _questionMark = null;
        }

        private void SetQuestionMark()
        {
            int xPos = this.HitBox.Location.X + (_sprite.Width / 2) - (NospikeQuestionMark.WIDTH / 2);
            int yPos = this.HitBox.Location.Y - NospikeQuestionMark.HEIGHT - QUESTION_MARK_DISTANCE_OVER_HEAD;
            _questionMark = new NospikeQuestionMark(_collisionGrid, new Rectangle(xPos, yPos, NospikeQuestionMark.WIDTH, NospikeQuestionMark.HEIGHT), _zIndex);
            _questionMark.Create += new EventHandler<ObjectEventArgs>(_questionMark_Create);
            _questionMark.Remove += new EventHandler<ObjectEventArgs>(_questionMark_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = _questionMark });
        }

        void _questionMark_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void _questionMark_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        private void Fall()
        {
            if (this.State != NospikeState.FALLING)
            {
                this.State = NospikeState.FALLING;
                _verticalVelocity = 0;
            }

            var tile = this.BasicFallReturnTile(_verticalVelocity);
            if (tile != null)
            {
                if (_verticalVelocity == MAX_FALL_VELOCITY)
                {
                    this.Die();
                }
                else
                {
                    this.Patrol();
                }
            }
            else if (_verticalVelocity + GRAVITY_ACCELERATION <= MAX_FALL_VELOCITY)
            {
                _verticalVelocity += GRAVITY_ACCELERATION;
            }
            else
            {
                _verticalVelocity = MAX_FALL_VELOCITY;
            }
        }

        private void UpdateStunnedState()
        {
            if (this.State != NospikeState.STUNNED)
            {
                this.State = NospikeState.STUNNED;
            }

            this.UpdateHitboxBasedOnStunnedImage(
              _stunnedSprites
              , ref _currentStunnedSprite
              , ref _currentStunnedSpriteChangeDelayTick
              , STUNNED_SPRITE_CHANGE_DELAY
              , UpdateSprite);

            if (IsNothingBeneath())
            {
                this.BasicFall(MAX_FALL_VELOCITY);
            }
        }


        private void Charge()
        {
            if (this.State != NospikeState.CHARGING)
            {
                this.Direction = SetDirectionFromObjectHorizontal(_keen, true);
                _currentChargeDist = 0;
                this.State = NospikeState.CHARGING;
                this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                //avoid getting stuck in a wall when charging left with back to said wall
                if (this.Direction == Enums.Direction.LEFT)
                {
                    var wallCollisions = this.CheckCollision(this.HitBox, true);
                    if (wallCollisions.Any())
                    {
                        var tileWall = GetLeftMostRightTile(wallCollisions);
                        this.HitBox = new Rectangle(tileWall.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                }
                this.BasicFall(MAX_FALL_VELOCITY);
            }

            //go through potential cases to stop charging first
            if (IsNothingBeneath())
            {
                this.UpdateConfusedState();
                return;
            }

            if (_currentChargeDist >= CHARGE_MIN_DISTANCE)
            {
                int chargeStopVal = _random.Next(1, CHARGE_STOP_CHANCE + 1);
                if (chargeStopVal == CHARGE_STOP_CHANCE)
                {
                    this.Patrol();
                    return;
                }
            }

            //charge
            int xOffset = _direction == Enums.Direction.LEFT ? CHARGE_VELOCITY * -1 : CHARGE_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;

            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width + CHARGE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);

            if (tile != null)
            {
                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                int chargeDist = Math.Abs(xCollidePos - this.HitBox.X);
                _currentChargeDist += chargeDist;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                this.KillCollidingPlayers();
                this.Look();
                return;
            }
            else
            {
                _currentChargeDist += Math.Abs(xOffset);
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.KillCollidingPlayers(areaToCheck);
            }

            this.UpdateSpriteByDelayBase(ref _currentChargeSpriteChangeDelayTick, ref _currentChargeSprite, CHARGE_SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        public bool DeadlyTouch
        {
            get { return _state == NospikeState.PATROLLING || _state == NospikeState.CHARGING || _state == NospikeState.LOOKING; }
        }

        public void HandleHit(IProjectile projectile)
        {
            base.TakeDamage(projectile);

            if (this.Health > 0)
            {
                _hitAnimation = true;
                if (_state != NospikeState.CHARGING)
                    this.Charge();
                UpdateSprite();
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (!this.IsDead())
            {
                _hitAnimation = true;
                if (_state != NospikeState.CHARGING)
                    this.Charge();
                UpdateSprite();
            }
        }

        public bool IsActive
        {
            get { return _state != NospikeState.STUNNED && _state != NospikeState.FALLING && _state != NospikeState.CONFUSED; }
        }

        NospikeState State
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

        Direction Direction
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

        public PointItemType PointItem => PointItemType.KEEN6_PIZZA_SLICE;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        private void UpdateSprite()
        {
            Rectangle previousHitbox = new Rectangle(this.HitBox.X, this.HitBox.Y,
                this.HitBox.Width, this.HitBox.Height);
            switch (_state)
            {
                case NospikeState.LOOKING:
                case NospikeState.CONFUSED:
                case NospikeState.FALLING:
                    _sprite = Properties.Resources.keen6_nospike_look;
                    AdjustHitboxFromPrevious(previousHitbox);
                    break;
                case NospikeState.PATROLLING:
                    var spriteSet = _direction == Enums.Direction.LEFT ? _patrolLeftSprites : _patrolRightSprites;
                    if (_currentPatrolSprite >= spriteSet.Length)
                    {
                        _currentPatrolSprite = 0;
                    }
                    _sprite = spriteSet[_currentPatrolSprite];
                    AdjustHitboxFromPrevious(previousHitbox);
                    break;
                case NospikeState.CHARGING:
                    spriteSet = _direction == Enums.Direction.LEFT ? _chargeLeftSprites : _chargeRightSprites;
                    if (_currentChargeSprite >= spriteSet.Length)
                    {
                        _currentChargeSprite = 0;
                    }
                    _sprite = spriteSet[_currentChargeSprite];
                    AdjustHitboxFromPrevious(previousHitbox);
                    break;
                case NospikeState.STUNNED:
                    spriteSet = _stunnedSprites;
                    if (_currentStunnedSprite >= spriteSet.Length)
                    {
                        _currentStunnedSprite = 1;
                    }
                    _sprite = spriteSet[_currentStunnedSprite];
                    AdjustHitboxFromPrevious(previousHitbox);
                    break;
            }
            if (_hitAnimation)
                _sprite = this.GetCurrentSpriteWithWhiteBackground(_sprite);
        }

        private void AdjustHitboxFromPrevious(Rectangle previousHitbox)
        {
            int heightDiff = previousHitbox.Height - _sprite.Height;
            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + heightDiff, _sprite.Width, _sprite.Height);
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

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

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_nospike_look);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum NospikeState
    {
        LOOKING,
        PATROLLING,
        CHARGING,
        CONFUSED,
        FALLING,
        STUNNED
    }
}
