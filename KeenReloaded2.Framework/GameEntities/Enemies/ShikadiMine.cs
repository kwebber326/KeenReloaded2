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

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class ShikadiMine : CollisionObject, IUpdatable, ISprite, IEnemy, IExplodable, ICreateRemove, IZombieBountyEnemy
    {
        public ShikadiMine(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            Initialize();
            this.HitBox = area;
        }

        private void Initialize()
        {
            _direction = this.GetRandomHorizontalDirection();

            this.State = ShikadiMineState.LOOKING;
            _previousDirection = _direction;
            _previousState = _state;
            InitializeEye();
        }

        private void InitializeEye()
        {
            _eye = new ShikadiMineEye(this);
        }

        private Enums.ExplosionState _explosionState = Enums.ExplosionState.NOT_EXPLODING;
        private Image _sprite;
        private ShikadiMineState _state;
        private ShikadiMineState _previousState;
        private Direction _direction;
        private Direction _previousDirection;
        private const int EYE_MOVE_VELOCITY = 2;
        private const int MAX_EYE_DISTANCE_FROM_CENTER = 14;
        private bool _determinedVerticalLocation, _determinedHorizontalLocation, _determinedDirection;
        private bool _checkedX, _checkedY;
        private bool _hitWallVertical, _hitWallHorizontal;

        private const int MOVE_VELOCITY = 5;
        private const int MOVE_DISTANCE_MAX = 50;
        private int _currentMoveDistance;
        private const int EXPLODE_DISTANCE = 40;

        private const int SELF_DESTRUCT_SPRITE_CHANGE_DELAY = 4;
        private int _currentSelfDestructSpriteChangeDelayTick;
        private const int SELF_DESTRUCT_SEQUENCE_COUNT = 5;
        private int _currentSelfDestructSequenceNum = 1;
        private bool _selfDestructInitiated;

        private const int EXPLODE_SPRITE_CHANGE_DELAY = 6;
        private readonly int _zIndex;
        private int _currentExplodeSpriteChangeDelayTick;
        private int _currentExplodeSprite;
        private bool _addedEye;

        public void Update()
        {
            if (!_addedEye && _eye != null)
            {
                OnCreate(new ObjectEventArgs() { ObjectSprite = _eye });
                _addedEye = true;
            }
            _keen = this.GetClosestPlayer();
            if (IsKeenInExplodingRange() && _state != ShikadiMineState.EXPLODING)
            {
                this.RunSelfDestructSequence();
            }
            switch (_state)
            {
                case ShikadiMineState.LOOKING:
                    this.Look();
                    break;
                case ShikadiMineState.FOLLOWING:
                    this.FollowKeen();
                    break;
                case ShikadiMineState.SELF_DESTRUCT_SEQUENCE_INITIATED:
                    this.RunSelfDestructSequence();
                    break;
                case ShikadiMineState.EXPLODING:
                    this.Explode();
                    break;
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
                int xDiff = value.X - this.HitBox.X;
                int yDiff = value.Y - this.HitBox.Y;
                base.HitBox = value;
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null && _eye != null)
                {
                    _eye.UpdateEyeLocation(new Point(_eye.Location.X + xDiff, _eye.Location.Y + yDiff));
                    this.UpdateCollisionNodes(this._direction);
                }
            }
        }

        public ShikadiMineEye Eye
        {
            get
            {
                return _eye;
            }
        }
        private void RunSelfDestructSequence()
        {
            if (this.State != ShikadiMineState.SELF_DESTRUCT_SEQUENCE_INITIATED)
            {
                this.State = ShikadiMineState.SELF_DESTRUCT_SEQUENCE_INITIATED;
                OnRemove(new ObjectEventArgs() { ObjectSprite = this.Eye });
                _selfDestructInitiated = true;
            }

            if (_currentSelfDestructSequenceNum == SELF_DESTRUCT_SEQUENCE_COUNT)
            {
                _currentSelfDestructSequenceNum = 0;
                this.Explode();
            }

            if (_currentSelfDestructSpriteChangeDelayTick++ == SELF_DESTRUCT_SPRITE_CHANGE_DELAY)
            {
                _currentSelfDestructSpriteChangeDelayTick = 0;
                _currentSelfDestructSequenceNum++;
                this.UpdateSprite();
            }
        }

        private void FollowKeen()
        {
            if (this.State != ShikadiMineState.FOLLOWING)
            {
                this.State = ShikadiMineState.FOLLOWING;
                _currentMoveDistance = 0;
                _hitWallVertical = false;
                _hitWallHorizontal = false;
            }

            if (_currentMoveDistance < MOVE_DISTANCE_MAX)
            {
                switch (_direction)
                {
                    case Direction.LEFT:
                        MoveLeft();
                        break;
                    case Direction.RIGHT:
                        MoveRight();
                        break;
                    case Direction.UP:
                        MoveUp();
                        break;
                    case Direction.DOWN:
                        MoveDown();
                        break;
                }

            }
            else if (!AmIGoingInOppositeDirection() &&
                 (
                       (IsHorizontalDirection(_direction) && !IsKeenInXRange() && !_hitWallHorizontal)
                    || (IsVerticalDirection(_direction) && !IsKeenInYRange() && !_hitWallVertical)
                 ))
            {
                _currentMoveDistance = 0;
            }
            else
            {
                _previousDirection = _direction;
                _previousState = _state;
                this.Look();
            }
        }

        private bool AmIGoingInOppositeDirection()
        {
            switch (_direction)
            {
                case Direction.DOWN:
                    return _keen.HitBox.Bottom < this.HitBox.Top;
                case Direction.UP:
                    return _keen.HitBox.Top > this.HitBox.Bottom;
                case Direction.LEFT:
                    return _keen.HitBox.Left > this.HitBox.Right;
                case Direction.RIGHT:
                    return _keen.HitBox.Right < this.HitBox.Left;
            }
            return false;
        }

        private bool IsKeenInExplodingRange()
        {
            Rectangle visionRange = new Rectangle(this.HitBox.X - EXPLODE_DISTANCE, this.HitBox.Y - EXPLODE_DISTANCE //location
                , this.HitBox.Width + (EXPLODE_DISTANCE * 2), this.HitBox.Height + (EXPLODE_DISTANCE * 2)); //dimensions
            return _keen.HitBox.IntersectsWith(visionRange);
        }

        private void MoveDown()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + MOVE_VELOCITY);
            var collisions = this.CheckCollision(areaToCheck, true);
            var tile = GetTopMostLandingTile(collisions);
            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    _keen.Die();
                }
                _hitWallVertical = true;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + MOVE_VELOCITY, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    _keen.Die();
                }
            }
            _currentMoveDistance += MOVE_VELOCITY;
        }

        private void MoveUp()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y - MOVE_VELOCITY, this.HitBox.Width, this.HitBox.Height + MOVE_VELOCITY);
            var collisions = this.CheckCollision(areaToCheck, true);
            var tile = GetCeilingTile(collisions);
            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    _keen.Die();
                }
                _hitWallVertical = true;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - MOVE_VELOCITY, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    _keen.Die();
                }
            }
            _currentMoveDistance += MOVE_VELOCITY;
        }

        private void MoveRight()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            var tile = GetLeftMostRightTile(collisions);
            if (tile != null)
            {
                this.HitBox = new Rectangle(tile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    _keen.Die();
                }
                _hitWallHorizontal = true;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + MOVE_VELOCITY, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    _keen.Die();
                }
            }
            _currentMoveDistance += MOVE_VELOCITY;
        }

        private void MoveLeft()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X - MOVE_VELOCITY, this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            var tile = GetRightMostLeftTile(collisions);
            if (tile != null)
            {
                this.HitBox = new Rectangle(tile.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    _keen.Die();
                }
                _hitWallHorizontal = true;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X - MOVE_VELOCITY, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    _keen.Die();
                }
            }
            _currentMoveDistance += MOVE_VELOCITY;
        }

        private void Look()
        {
            if (this.State != ShikadiMineState.LOOKING)
            {
                this.State = ShikadiMineState.LOOKING;
                _determinedHorizontalLocation = false;
                _determinedVerticalLocation = false;
                _determinedDirection = false;
                _checkedX = false;
                _checkedY = false;
            }

            if (_previousState == ShikadiMineState.LOOKING)
            {
                UpdateChaseVariables();

                int xDist = Math.Abs(_eye.Location.X - CenterX), yDist = Math.Abs(_eye.Location.Y - CenterY);

                if ((IsHorizontalDirection(_direction) && xDist < MAX_EYE_DISTANCE_FROM_CENTER)
                    || (IsVerticalDirection(_direction) && yDist < MAX_EYE_DISTANCE_FROM_CENTER))
                {
                    ExecuteEyeLook();
                }
                else
                {
                    this.FollowKeen();
                }
            }
            else if (_previousState == ShikadiMineState.FOLLOWING)
            {
                if (IsEyeCentered())
                {
                    _previousState = ShikadiMineState.LOOKING;
                }
                else
                {
                    MoveEyeToCenter();
                }
            }
        }

        private void MoveEyeToCenter()
        {
            if (_eye.Location.X < CenterX)
            {
                _eye.UpdateEyeLocation(new Point(_eye.Location.X + EYE_MOVE_VELOCITY, _eye.Location.Y));
            }
            else if (_eye.Location.X > CenterX)
            {
                _eye.UpdateEyeLocation(new Point(_eye.Location.X - EYE_MOVE_VELOCITY, _eye.Location.Y));
            }

            if (_eye.Location.Y < CenterY)
            {
                _eye.UpdateEyeLocation(new Point(_eye.Location.X, _eye.Location.Y + EYE_MOVE_VELOCITY));
            }
            else if (_eye.Location.Y > CenterY)
            {
                _eye.UpdateEyeLocation(new Point(_eye.Location.X, _eye.Location.Y - EYE_MOVE_VELOCITY));
            }
        }

        private void ExecuteEyeLook()
        {
            switch (_direction)
            {
                case Direction.LEFT:
                    _eye.UpdateEyeLocation(new Point(_eye.Location.X - EYE_MOVE_VELOCITY, _eye.Location.Y));
                    break;
                case Direction.RIGHT:
                    _eye.UpdateEyeLocation(new Point(_eye.Location.X + EYE_MOVE_VELOCITY, _eye.Location.Y));
                    break;
                case Direction.UP:
                    _eye.UpdateEyeLocation(new Point(_eye.Location.X, _eye.Location.Y - EYE_MOVE_VELOCITY));
                    break;
                case Direction.DOWN:
                    _eye.UpdateEyeLocation(new Point(_eye.Location.X, _eye.Location.Y + EYE_MOVE_VELOCITY));
                    break;
            }
        }

        private void UpdateChaseVariables()
        {
            if (!_determinedDirection)
            {
                if (IsHorizontalDirection(_previousDirection) && !_checkedY)
                {
                    _direction = SetDirectionFromObjectVertical(_keen, true);
                    _checkedY = true;
                    _determinedDirection = true;
                }
                else if (!_checkedX)
                {
                    _direction = SetDirectionFromObjectHorizontal(_keen, true);
                    _checkedX = true;
                    _determinedDirection = true;
                }
            }
        }

        private bool IsKeenInXRange()
        {
            return _keen.HitBox.Right >= this.HitBox.Left && _keen.HitBox.Left <= this.HitBox.Right;
        }

        private bool IsKeenInYRange()
        {
            return _keen.HitBox.Top <= this.HitBox.Bottom && _keen.HitBox.Bottom >= this.HitBox.Top;
        }

        private int CenterX
        {
            get
            {
                return this.HitBox.X + (this.HitBox.Width / 2 - _eye.Size.Width / 2);
            }
        }

        private int CenterY
        {
            get
            {
                return this.HitBox.Y + (this.HitBox.Height / 2 - _eye.Size.Height / 2);
            }
        }

        private bool IsEyeCentered()
        {
            bool isCentered = _eye.Location.X == CenterX && _eye.Location.Y == CenterY;

            return isCentered;
        }

        ShikadiMineState State
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
                case ShikadiMineState.LOOKING:
                case ShikadiMineState.FOLLOWING:
                    _sprite = Properties.Resources.keen5_shikadi_mine;
                    break;
                case ShikadiMineState.SELF_DESTRUCT_SEQUENCE_INITIATED:
                    if (_currentSelfDestructSequenceNum % 2 == 0)
                    {
                        _sprite = Properties.Resources.keen5_shikadi_mine_self_destruct_sequence1;
                    }
                    else
                    {
                        _sprite = Properties.Resources.keen5_shikadi_mine_self_destruct_sequence2;
                    }
                    break;
                case ShikadiMineState.EXPLODING:
                    if (_currentExplodeSprite < SpriteSheet.SpriteSheet.ShikadiMineExplodeImages.Length)
                    {
                        _sprite = SpriteSheet.SpriteSheet.ShikadiMineExplodeImages[_currentExplodeSprite];
                        int xDiff = (_sprite.Width - this.HitBox.Width) / 2, yDiff = (_sprite.Height - this.HitBox.Height) / 2;
                        this.HitBox = new Rectangle(this.HitBox.X - xDiff, this.HitBox.Y - yDiff, _sprite.Width, _sprite.Height);
                    }
                    break;
            }
        }

        public bool DeadlyTouch
        {
            get { return true; }
        }

        public void HandleHit(IProjectile projectile)
        {

        }

        public bool IsActive
        {
            get { return this.State != ShikadiMineState.EXPLODING; }
        }

        public void Explode()
        {
            if (this.State != ShikadiMineState.EXPLODING)
            {
                this.State = ShikadiMineState.EXPLODING;
                _explosionState = Enums.ExplosionState.EXPLODING;
                if (!_selfDestructInitiated)
                    OnRemove(new ObjectEventArgs() { ObjectSprite = this.Eye });
                CreateFragmentsFromExplosion();
            }

            if (_currentExplodeSpriteChangeDelayTick++ == EXPLODE_SPRITE_CHANGE_DELAY)
            {
                _currentExplodeSpriteChangeDelayTick = 0;
                _currentExplodeSprite++;
                UpdateSprite();
                if (_currentExplodeSprite >= SpriteSheet.SpriteSheet.ShikadiMineExplodeImages.Length)
                {
                    _explosionState = Enums.ExplosionState.DONE;
                    OnKilled();
                    OnRemove(new ObjectEventArgs() { ObjectSprite = this });
                }
            }
        }

        private Point GetRandomExplosionFragmentStartPoint(int fragmentSize)
        {
            _random = new Random();
            int randX = _random.Next(this.HitBox.X + 1, this.HitBox.Right - fragmentSize - 10);
            int randY = _random.Next(this.HitBox.Y + 1, this.HitBox.Bottom - fragmentSize - 10);
            return new Point(randX, randY);
        }

        private void CreateFragmentsFromExplosion()
        {
            int fragmentSize = 14;

            //top left
            Fragment fragment1 = new Fragment(_collisionGrid, new Rectangle(GetRandomExplosionFragmentStartPoint(fragmentSize), new Size(fragmentSize, fragmentSize)), Direction.LEFT, FragmentType.KEEN5_SHIKADI_MINE, 40, 0);
            fragment1.Create += new EventHandler<ObjectEventArgs>(fragment_Create);
            fragment1.Remove += new EventHandler<ObjectEventArgs>(fragment_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = fragment1 });

            //bottom left
            Fragment fragment2 = new Fragment(_collisionGrid, new Rectangle(GetRandomExplosionFragmentStartPoint(fragmentSize), new Size(fragmentSize, fragmentSize)), Direction.LEFT, FragmentType.KEEN5_SHIKADI_MINE, 40, 0);
            fragment2.Create += new EventHandler<ObjectEventArgs>(fragment_Create);
            fragment2.Remove += new EventHandler<ObjectEventArgs>(fragment_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = fragment2 });

            //top right
            Fragment fragment3 = new Fragment(_collisionGrid, new Rectangle(GetRandomExplosionFragmentStartPoint(fragmentSize), new Size(fragmentSize, fragmentSize)), Direction.RIGHT, FragmentType.KEEN5_SHIKADI_MINE, 40, 0);
            fragment3.Create += new EventHandler<ObjectEventArgs>(fragment_Create);
            fragment3.Remove += new EventHandler<ObjectEventArgs>(fragment_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = fragment3 });

            //bottom right
            Fragment fragment4 = new Fragment(_collisionGrid, new Rectangle(GetRandomExplosionFragmentStartPoint(fragmentSize), new Size(fragmentSize, fragmentSize)), Direction.RIGHT, FragmentType.KEEN5_SHIKADI_MINE, 50, 0);
            fragment4.Create += new EventHandler<ObjectEventArgs>(fragment_Create);
            fragment4.Remove += new EventHandler<ObjectEventArgs>(fragment_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = fragment4 });

            int rand1 = _random.Next(0, 2);
            int vertDir1 = rand1 == 0 ? -1 : 1;

            //vertical left
            Fragment fragment5 = new Fragment(_collisionGrid, new Rectangle(GetRandomExplosionFragmentStartPoint(fragmentSize), new Size(fragmentSize, fragmentSize)), Direction.LEFT, FragmentType.KEEN5_SHIKADI_MINE, 30, -50 * vertDir1, true);
            fragment5.Create += new EventHandler<ObjectEventArgs>(fragment_Create);
            fragment5.Remove += new EventHandler<ObjectEventArgs>(fragment_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = fragment5 });

            rand1 = _random.Next(0, 2);
            vertDir1 = rand1 == 0 ? -1 : 1;

            //vertical right
            Fragment fragment6 = new Fragment(_collisionGrid, new Rectangle(GetRandomExplosionFragmentStartPoint(fragmentSize), new Size(fragmentSize, fragmentSize)), Direction.RIGHT, FragmentType.KEEN5_SHIKADI_MINE, 30, -50 * vertDir1, true);
            fragment6.Create += new EventHandler<ObjectEventArgs>(fragment_Create);
            fragment6.Remove += new EventHandler<ObjectEventArgs>(fragment_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = fragment6 });

            EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                GeneralGameConstants.Sounds.SHIKADI_MINE_EXPLODE);
        }

        void fragment_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void fragment_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public Enums.ExplosionState ExplosionState
        {
            get { return _explosionState; }
        }

        public PointItemType PointItem => PointItemType.KEEN5_CHOCOLATE_MILK;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public bool ExplodesFromProjectileCollision => false;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        public event EventHandler<ObjectEventArgs> Killed;

        protected void OnKilled()
        {
            this.Killed?.Invoke(this, new ObjectEventArgs() { ObjectSprite = this });
        }

        private CommanderKeen _keen;
        private ShikadiMineEye _eye;

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
            string imageName = nameof(Properties.Resources.keen5_shikadi_mine);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum ShikadiMineState
    {
        LOOKING,
        FOLLOWING,
        SELF_DESTRUCT_SEQUENCE_INITIATED,
        EXPLODING
    }

    public class ShikadiMineEye : ISprite
    {
        private readonly int _zIndex;
        private Image _sprite;
        private Rectangle _area;

        public ShikadiMineEye(ShikadiMine mine)
        {
            if (mine == null)
                throw new ArgumentNullException("shikadi mine eye must be associated with a shikadi mine");
            _zIndex = mine.ZIndex + 1;
            _sprite = Properties.Resources.keen5_shikadi_mine_eye;
            _area = new Rectangle(new Point(mine.HitBox.Location.X + (mine.HitBox.Width / 2 - this._sprite.Width / 2), mine.HitBox.Location.Y + (mine.HitBox.Height / 2 - this._sprite.Height / 2)), _sprite.Size);
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public Size Size => _area.Size;

        public void UpdateEyeLocation(Point p)
        {
            _area.Location = p;
        }
    }
}
