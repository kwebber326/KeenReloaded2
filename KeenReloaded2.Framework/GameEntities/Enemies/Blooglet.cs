using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Items;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Blooglet : DestructibleObject, IUpdatable, ISprite, IEnemy, ICreateRemove, IZombieBountyEnemy
    {
        private Color _color;
        private Image[] _walkRightSprites, _walkLeftSprites, _stunnedSprites;
        private bool _holdsItem;
        private Image _sprite;
        private BloogletState _state;
        private Enums.Direction _direction;
        private int _currentStunnedSprite;
        private const int STUNNED_SPRITE_CHANGE_DELAY = 1;
        private int _currentStunnedSpriteChangeDelayTick;
        private const int MOVE_SPRITE_CHANGE_DELAY = 0;
        private int _currentMoveSpriteChangeDelayTick;
        private int _currentMoveSprite;
        private const int FALL_VELOCITY = 30;
        private const int MOVE_VELOCITY = 17;
        private const int CHASE_KEEN_CHANCE = 20;
        private CommanderKeen _keen;
        private bool _spawnedGem;

        public Blooglet(Rectangle area, SpaceHashGrid grid, int zIndex, Color color, bool holdsItem)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            _color = color;
            _holdsItem = holdsItem;
            Initialize();
        }

        private void Initialize()
        {
            //default to red
            _walkLeftSprites = SpriteSheet.SpriteSheet.BloogletRedLeftImages;
            _walkRightSprites = SpriteSheet.SpriteSheet.BloogletRedRightImages;
            _stunnedSprites = SpriteSheet.SpriteSheet.BloogletRedStunnedImages;

            //TODO: once sprites are in, set the sprite arrays inside these 'if/else' conditions
            if (_color == Color.Blue)
            {
                _walkLeftSprites = SpriteSheet.SpriteSheet.BloogletBlueLeftImages;
                _walkRightSprites = SpriteSheet.SpriteSheet.BloogletBlueRightImages;
                _stunnedSprites = SpriteSheet.SpriteSheet.BloogletBlueStunnedImages;
            }
            else if (_color == Color.Green)
            {
                _walkLeftSprites = SpriteSheet.SpriteSheet.BloogletGreenLeftImages;
                _walkRightSprites = SpriteSheet.SpriteSheet.BloogletGreenRightImages;
                _stunnedSprites = SpriteSheet.SpriteSheet.BloogletGreenStunnedImages;
            }
            else if (_color == Color.Yellow)
            {
                _walkLeftSprites = SpriteSheet.SpriteSheet.BloogletYellowLeftImages;
                _walkRightSprites = SpriteSheet.SpriteSheet.BloogletYellowRightImages;
                _stunnedSprites = SpriteSheet.SpriteSheet.BloogletYellowStunnedImages;
            }

            this.Direction = this.GetRandomHorizontalDirection();

            this.State = BloogletState.FALLING;
        }

        public override void Die()
        {
            this.UpdateStunnedState();
            if (_holdsItem && !_spawnedGem)
            {
                SpawnGem();
            }
        }

        private void SpawnGem()
        {
            GemColor color = GetGemColorFromThisColor();
            Gem gem = 
                new Gem(new Rectangle(this.HitBox.X + this.HitBox.Width / 2 - 13, this.HitBox.Y, 26, 22), 
                _collisionGrid, this.GetGemImageNameFromColor(), color, _zIndex + 1, true);
            gem.Create += new EventHandler<ObjectEventArgs>(gem_Create);
            gem.Remove += new EventHandler<ObjectEventArgs>(gem_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = gem });
            _spawnedGem = true;
        }

        void gem_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void gem_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        private GemColor GetGemColorFromThisColor()
        {
            if (_color == Color.Red)
                return GemColor.RED;
            if (_color == Color.Yellow)
                return GemColor.YELLOW;
            if (_color == Color.Green)
                return GemColor.GREEN;
            if (_color == Color.Blue)
                return GemColor.BLUE;

            return GemColor.RED;
        }

        public void Update()
        {
            if (_state != BloogletState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }
            switch (_state)
            {
                case BloogletState.MOVING:
                    this.Move();
                    break;
                case BloogletState.STUNNED:
                    this.UpdateStunnedState();
                    break;
                case BloogletState.FALLING:
                    this.Fall();
                    break;
            }
        }

        private void Fall()
        {
            if (this.State != BloogletState.FALLING)
            {
                this.State = BloogletState.FALLING;
                _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
            }

            var tile = this.BasicFallReturnTile(FALL_VELOCITY);

            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                if (!this.IsDead())
                {
                    this.Move();
                }
                else
                {
                    this.UpdateStunnedState();
                }
            }
            else if (this.IsDead())
            {
                this.UpdateHitboxBasedOnStunnedImage(
               _stunnedSprites
               , ref _currentStunnedSprite
               , ref _currentStunnedSpriteChangeDelayTick
               , STUNNED_SPRITE_CHANGE_DELAY
               , UpdateSprite);
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
                if (_collidingNodes != null && _collisionGrid != null && this.HitBox != null)
                {
                    if (this.State != BloogletState.FALLING)
                    {
                        this.UpdateCollisionNodes(this.Direction);
                    }
                    else
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                }
            }
        }

        private int _zIndex;

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

        BloogletState State
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

        public Color Color
        {
            get
            {
                return _color;
            }
        }

        public bool HoldsItem
        {
            get
            {
                return _holdsItem;
            }
        }

        private void UpdateSprite()
        {
            switch (_state)
            {
                case BloogletState.MOVING:
                case BloogletState.FALLING:
                    if (!this.IsDead())
                    {
                        var spriteSet = this.Direction == Enums.Direction.LEFT ? _walkLeftSprites : _walkRightSprites;
                        if (_currentMoveSprite >= spriteSet.Length || _currentMoveSprite < 0)
                        {
                            _currentMoveSprite = 0;
                        }

                        _sprite = spriteSet[_currentMoveSprite];
                    }
                    break;
                case BloogletState.STUNNED:
                    if (_currentStunnedSprite >= _stunnedSprites.Length || _currentStunnedSprite < 0)
                    {
                        _currentStunnedSprite = 1;
                    }
                    _sprite = _stunnedSprites[_currentStunnedSprite];
                    break;
            }
        }

        private void UpdateStunnedState()
        {
            if (this.State != BloogletState.STUNNED)
            {
                this.State = BloogletState.STUNNED;
                _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
                SetInitialStunnedSprite();
                return;
            }

            this.UpdateHitboxBasedOnStunnedImage(
                _stunnedSprites
                , ref _currentStunnedSprite
                , ref _currentStunnedSpriteChangeDelayTick
                , STUNNED_SPRITE_CHANGE_DELAY
                , UpdateSprite);

            if (IsNothingBeneath())
            {
                this.Fall();
            }
        }

        private void SetInitialStunnedSprite()
        {
            if (_color == Color.Red)
                _sprite = Properties.Resources.keen6_blooglet_red_stunned1;
            else if (_color == Color.Blue)
                _sprite = Properties.Resources.keen6_blooglet_blue_stunned1;
            else if (_color == Color.Green)
                _sprite = Properties.Resources.keen6_blooglet_green_stunned1;
            else if (_color == Color.Yellow)
                _sprite = Properties.Resources.keen6_blooglet_yellow_stunned1;
        }

        private void Move()
        {
            if (this.State != BloogletState.MOVING)
            {
                this.State = BloogletState.MOVING;
                if (_keen.HitBox.IntersectsWith(this.HitBox) && !IsKeenInFrontOfThis())
                {
                    _keen.SetKeenPushState(ChangeHorizontalDirection(_keen.Direction), true, this);
                }
                else
                {
                    _keen.SetKeenPushState(ChangeHorizontalDirection(_keen.Direction), false, this);
                }
            }

            if (IsNothingBeneath())
            {
                this.Fall();
                return;
            }
            int chaseKeenVal = _random.Next(1, CHASE_KEEN_CHANCE + 1);
            if (chaseKeenVal == CHASE_KEEN_CHANCE)
            {
                this.Direction = SetDirectionFromObjectHorizontal(_keen, true);
            }

            if (IsOnEdge(this.Direction, 3))
            {
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
                _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);
            }

            int xOffset = _direction == Enums.Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;

            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (tile != null)
            {
                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                xOffset = _direction == Enums.Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;
                ExecuteFreeMoveLogic(xOffset, areaToCheck);
            }
            else
            {
                ExecuteFreeMoveLogic(xOffset, areaToCheck);
            }

            this.UpdateSpriteByDelayBase(ref _currentMoveSpriteChangeDelayTick, ref _currentMoveSprite, MOVE_SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        private void ExecuteFreeMoveLogic(int xOffset, Rectangle areaToCheck)
        {
            _keen.SetKeenPushState(Enums.Direction.LEFT, false, this);
            _keen.SetKeenPushState(Enums.Direction.RIGHT, false, this);

            Rectangle pushAreaToCheck = this.Direction == Enums.Direction.LEFT
                ? new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, xOffset * -1, this.HitBox.Height)
                : new Rectangle(this.HitBox.Right, this.HitBox.Y, xOffset, this.HitBox.Height);
            this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);

            if (!(_keen.MoveState == MoveState.ON_POLE || _keen.IsDead()))
            {

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
        }

        private bool IsKeenInFrontOfThis()
        {
            if (!_keen.HitBox.IntersectsWith(this.HitBox))
                return false;
            if (_keen.Direction == Enums.Direction.LEFT)
            {
                if (_keen.HitBox.Left <= this.HitBox.Right - 15)
                    return true;
            }
            else
            {
                if (_keen.HitBox.Right >= this.HitBox.Left + 15)
                    return true;
            }

            return false;
        }

        public bool DeadlyTouch
        {
            get { return false; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return _state != BloogletState.STUNNED; }
        }

        public PointItemType PointItem => PointItemType.KEEN6_BLOOG_SODA;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

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
            string imageName = this.GetImageNameFromColor();
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_color}{separator}{_holdsItem}";
        }

        private string GetImageNameFromColor()
        {
            if (this.Color == Color.Red)
                return nameof(Properties.Resources.keen6_blooglet_red_right1);
            if (this.Color == Color.Blue)
                return nameof(Properties.Resources.keen6_blooglet_blue_right1);
            if (this.Color == Color.Green)
                return nameof(Properties.Resources.keen6_blooglet_green_right1);
            if (this.Color == Color.Yellow)
                return nameof(Properties.Resources.keen6_blooglet_yellow_right1);

            throw new ArgumentException("Blooglet was not assigned a valid color");
        }

        private string GetGemImageNameFromColor()
        {
            if (this.Color == Color.Red)
                return nameof(Properties.Resources.gem_red1);
            if (this.Color == Color.Blue)
                return nameof(Properties.Resources.gem_blue1);
            if (this.Color == Color.Green)
                return nameof(Properties.Resources.gem_green1);
            if (this.Color == Color.Yellow)
                return nameof(Properties.Resources.gem_yellow1);

            throw new ArgumentException("Blooglet was not assigned a valid color");
        }
    }

    enum BloogletState
    {
        MOVING,
        STUNNED,
        FALLING
    }
}
