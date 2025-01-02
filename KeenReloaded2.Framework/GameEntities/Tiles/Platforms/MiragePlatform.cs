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

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class MiragePlatform : CollisionObject, IUpdatable, ISprite, ICreateRemove
    {
        private const int STANDING_OFFSET_Y = 30;
        private const int STANDING_OFFSET_X = 10;
        private const int SPRITE_CHANGE_DELAY = 30;
        private int _currentSpriteChangeDelayTick;
        private readonly int _zIndex;
        private MiragePlatformState _state;
        private Dictionary<MiragePlatformState, Image> _spritesByState;
        private string _imageName;
        private Rectangle _area;

        public MiragePlatform(Rectangle area, SpaceHashGrid grid, int zIndex, MiragePlatformState initialState)
            : base(grid, area)
        {
            _zIndex = zIndex;
            _state = initialState;
            _area = area;
            Initialize();
        }

        private void Initialize()
        {
            if (_collidingNodes != null)
                this.HitBox = new Rectangle(this.HitBox.X + STANDING_OFFSET_X, this.HitBox.Y + STANDING_OFFSET_Y, this.HitBox.Width - (STANDING_OFFSET_X * 2), this.HitBox.Height - STANDING_OFFSET_Y);

            _spritesByState = new Dictionary<MiragePlatformState, Image>();
            _spritesByState.Add(MiragePlatformState.PHASE1, Properties.Resources.keen4_mirage_platform1);
            _spritesByState.Add(MiragePlatformState.PHASE2, Properties.Resources.keen4_mirage_platform2);
            _spritesByState.Add(MiragePlatformState.PHASE3, Properties.Resources.keen4_mirage_platform3);
            _spritesByState.Add(MiragePlatformState.PHASE4, Properties.Resources.keen4_mirage_platform4);

            SetImageByState();
            if (_state != MiragePlatformState.PHASE3)
            {
                SetStandingTile();
            }
        }

        private void SetImageByState()
        {
            _sprite = _spritesByState[_state];
            switch (_state)
            {
                case MiragePlatformState.PHASE1:
                    _imageName = nameof(Properties.Resources.keen4_mirage_platform1);
                    break;
                case MiragePlatformState.PHASE2:
                    _imageName = nameof(Properties.Resources.keen4_mirage_platform2);
                    break;
                case MiragePlatformState.PHASE3:
                    _imageName = nameof(Properties.Resources.keen4_mirage_platform3);
                    break;
                case MiragePlatformState.PHASE4:
                    _imageName = nameof(Properties.Resources.keen4_mirage_platform4);
                    break;
            }
        }

        public void Update()
        {
            _keen = this.GetClosestPlayer();
            switch (_state)
            {
                case MiragePlatformState.PHASE1:
                    UpdatePhase1();
                    break;
                case MiragePlatformState.PHASE2:
                    UpdatePhase2();
                    break;
                case MiragePlatformState.PHASE3:
                    UpdatePhase3();
                    break;
                case MiragePlatformState.PHASE4:
                    UpdatePhase4();
                    break;
            }
        }

        private void UpdatePhase4()
        {
            if (_state != MiragePlatformState.PHASE4)
            {
                _state = MiragePlatformState.PHASE4;
                SetStandingTile();
                SetImageByState();
            }

            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                UpdatePhase1();
            }
        }

        private void UpdatePhase3()
        {
            if (_state != MiragePlatformState.PHASE3)
            {
                _state = MiragePlatformState.PHASE3;
                RemoveStandingTile();
                SetImageByState();
                if (_keen.MoveState == Enums.MoveState.HANGING)
                {
                    _keen.Fall();
                }
            }

            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                UpdatePhase4();
            }
        }


        private void UpdatePhase2()
        {
            if (_state != MiragePlatformState.PHASE2)
            {
                _state = MiragePlatformState.PHASE2;
                SetImageByState();
            }

            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                UpdatePhase3();
            }
        }

        private void UpdatePhase1()
        {
            if (_state != MiragePlatformState.PHASE1)
            {
                _state = MiragePlatformState.PHASE1;
                SetStandingTile();
                SetImageByState();
            }

            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                UpdatePhase2();
            }
        }

        private void SetStandingTile()
        {
            if (_firstTime)
            {
                this.StandingTile = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.Location, this.HitBox.Size), true);
                _firstTime = false;
            }
            else
            {
                this.StandingTile?.AddTileToGrid();
            }
        }

        private void RemoveStandingTile()
        {
            this.StandingTile?.RemoveTileFromGrid();
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
                if (this.HitBox != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public MiragePlatformState State
        {
            get
            {
                return _state;
            }
        }

        public InvisibleTile StandingTile
        {
            get;
            private set;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public override CollisionType CollisionType => CollisionType.NONE;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private Image _sprite;
        private bool _firstTime = true;
        private CommanderKeen _keen;

        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                // Create(this, args);
                if (args.ObjectSprite == this.StandingTile)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Add(this.StandingTile);
                        node.Tiles.Add(this.StandingTile);
                    }
                }
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
                        node.NonEnemies.Remove(this);
                    }
                }
                else if (args.ObjectSprite == this.StandingTile)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this.StandingTile);
                        node.Tiles.Remove(this.StandingTile);
                    }
                }
                this.Remove(this, args);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = _imageName;
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    public enum MiragePlatformState
    {
        PHASE1,
        PHASE2,
        PHASE3,
        PHASE4
    }
}
