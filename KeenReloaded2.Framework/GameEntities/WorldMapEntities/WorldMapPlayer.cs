using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class WorldMapPlayer : CollisionObject, IUpdatable, ISprite
    {
        private Direction _direction;
        private Rectangle _area;
        private Image _sprite;
        private bool _isMoving;
        private WorldMapPlayerMoveState _moveState;
        private readonly int _zIndex;

        private const int SPRITE_CHANGE_DELAY = 2;
        private int _currentSpriteIndex;
        private int _currentSpriteChangeDelayTick;

        public WorldMapPlayer(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            this.Direction = Direction.UP;
            _area = area;
            _zIndex = zIndex;
        }

        public override CollisionType CollisionType => CollisionType.PLAYER;

        public Direction Direction
        {
            get { return _direction; }
            private set
            {
                _direction = value;
                this.UpdateSprite();
            }
        }

        public void Update()
        {
            //TODO: implement movement
        }

        public override Rectangle HitBox
        {
            get => base.HitBox;

            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null && _collisionGrid != null)
                    this.UpdateCollisionNodes(_direction);
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;
        
        public WorldMapPlayerMoveState MoveState { 
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

        public bool CanUpdate => true;

        private void UpdateSprite()
        {
            //TODO: account for movement state when setting sprite
            Image[] images = new Image[0];
            switch (_direction)
            {
                case Direction.UP:
                   switch (MoveState)
                    {
                        case WorldMapPlayerMoveState.RUNNING:
                            images = SpriteSheet.SpriteSheet.PlayerMoveUpImages; break;
                        case WorldMapPlayerMoveState.SWIMMING:
                            images = SpriteSheet.SpriteSheet.PlayerSwimUpImages; break;
                    }
                    _sprite = Properties.Resources.keen_stop_up; break;
                case Direction.LEFT:
                    switch (MoveState)
                    {
                        case WorldMapPlayerMoveState.RUNNING:
                            images = SpriteSheet.SpriteSheet.PlayerMoveLeftImages; break;
                        case WorldMapPlayerMoveState.SWIMMING:
                            images = SpriteSheet.SpriteSheet.PlayerSwimLeftImages; break;
                    }
                    _sprite = Properties.Resources.keen_stop_left; break;
                case Direction.RIGHT:
                    switch (MoveState)
                    {
                        case WorldMapPlayerMoveState.RUNNING:
                            images = SpriteSheet.SpriteSheet.PlayerMoveRightImages; break;
                        case WorldMapPlayerMoveState.SWIMMING:
                            images = SpriteSheet.SpriteSheet.PlayerSwimRightImages; break;
                    }
                    _sprite = Properties.Resources.keen_stop_right; break;
                case Direction.DOWN:
                    switch (MoveState)
                    {
                        case WorldMapPlayerMoveState.RUNNING:
                            images = SpriteSheet.SpriteSheet.PlayerMoveDownImages; break;
                        case WorldMapPlayerMoveState.SWIMMING:
                            images = SpriteSheet.SpriteSheet.PlayerSwimDownImages; break;
                    }
                    _sprite = Properties.Resources.keen_stop_down; break;
                case Direction.UP_LEFT:
                    switch (MoveState)
                    {
                        case WorldMapPlayerMoveState.RUNNING:
                            images = SpriteSheet.SpriteSheet.PlayerMoveUpLeftImages; break;
                        case WorldMapPlayerMoveState.SWIMMING:
                            images = SpriteSheet.SpriteSheet.PlayerSwimUpLeftImages; break;
                    }
                    _sprite = Properties.Resources.keen_stop_up_left; break;
                case Direction.UP_RIGHT:
                    switch (MoveState)
                    {
                        case WorldMapPlayerMoveState.RUNNING:
                            images = SpriteSheet.SpriteSheet.PlayerMoveUpRightImages; break;
                        case WorldMapPlayerMoveState.SWIMMING:
                            images = SpriteSheet.SpriteSheet.PlayerSwimUpRightImages; break;
                    }
                    _sprite = Properties.Resources.keen_stop_up_right; break;
                case Direction.DOWN_LEFT:
                    switch (MoveState)
                    {
                        case WorldMapPlayerMoveState.RUNNING:
                            images = SpriteSheet.SpriteSheet.PlayerMoveDownLeftImages; break;
                        case WorldMapPlayerMoveState.SWIMMING:
                            images = SpriteSheet.SpriteSheet.PlayerSwimDownLeftImages; break;
                    }
                    _sprite = Properties.Resources.keen_stop_down_left; break;
                case Direction.DOWN_RIGHT:
                    switch (MoveState)
                    {
                        case WorldMapPlayerMoveState.RUNNING:
                            images = SpriteSheet.SpriteSheet.PlayerMoveDownRightImages; break;
                        case WorldMapPlayerMoveState.SWIMMING:
                            images = SpriteSheet.SpriteSheet.PlayerSwimDownRightImages; break;
                    }
                    _sprite = Properties.Resources.keen_stop_down_left; break;
            }

            if (MoveState != WorldMapPlayerMoveState.STILL && images.Length > 0)
            {
                this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick,
                    ref _currentSpriteIndex, SPRITE_CHANGE_DELAY, 
                    () =>
                    {
                        if (_currentSpriteIndex >= images.Length)
                        {
                            _currentSpriteIndex = 0;
                        }
                        _sprite = images[_currentSpriteIndex];
                    });
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{nameof(Properties.Resources.keen_stop_up)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}";
        }
    }

    public enum WorldMapPlayerMoveState
    {
        STILL,
        RUNNING,
        SWIMMING,
        FLYING_ON_MAGIC_FOOT,
        CLIMBING,
        FLYING_SHIP,
        TRANSPORTING
    }
}
