using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
        private readonly Dictionary<string, bool> _keysPressed;
        private const int SPRITE_CHANGE_DELAY = 2;
        private int _currentSpriteIndex;
        private int _currentSpriteChangeDelayTick;
        private const string KEY_LEFT = GeneralGameConstants.Keys.KEY_LEFT;
        private const string KEY_RIGHT = GeneralGameConstants.Keys.KEY_RIGHT;
        private const string KEY_UP = GeneralGameConstants.Keys.KEY_UP;
        private const string KEY_DOWN = GeneralGameConstants.Keys.KEY_DOWN;
        private const string KEY_CTRL = GeneralGameConstants.Keys.KEY_CTRL;
        private const string KEY_SPACE = GeneralGameConstants.Keys.KEY_SPACE;
        private const string KEY_ENTER = GeneralGameConstants.Keys.KEY_ENTER;

        private const int MOVE_VELOCITY = 10;

        public event EventHandler Moved;

        public WorldMapPlayer(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            this.Direction = Direction.UP;
            _area = area;
            _zIndex = zIndex;
            _keysPressed = new Dictionary<string, bool>();
            InitializeKeyControls();
        }

        private void InitializeKeyControls()
        {
            _keysPressed.Add(KEY_LEFT, false);
            _keysPressed.Add(KEY_RIGHT, false);
            _keysPressed.Add(KEY_UP, false);
            _keysPressed.Add(KEY_DOWN, false);
            _keysPressed.Add(KEY_CTRL, false);
            _keysPressed.Add(KEY_SPACE, false);
            _keysPressed.Add(KEY_ENTER, false);
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

        public void SetKeyPressed(string key, bool pressed)
        {
            if (_keysPressed.ContainsKey(key))
            {
                _keysPressed[key] = pressed;
            }
        }

        public bool IsKeyPressed(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            return _keysPressed.ContainsKey(key) && _keysPressed[key];
        }

        public bool AreKeysPressed(params string[] keys)
        {
            if (keys == null || keys.Length == 0)
                return false;

            return keys.All(k => IsKeyPressed(k));
        }

        public bool AreAnyKeysPressed(params string[] keys)
        {
            if (keys == null || keys.Length == 0)
                return false;

            return keys.Any(k => IsKeyPressed(k));
        }

        private bool OpposingKeysPressed()
        {
            return AreKeysPressed(KEY_LEFT, KEY_RIGHT)
                || AreKeysPressed(KEY_UP, KEY_DOWN);
        }

        private bool AnyDirectionKeysPressed()
        {
            return AreAnyKeysPressed(KEY_UP, KEY_DOWN, KEY_LEFT, KEY_RIGHT);
        }

        private bool AnyActionKeyPressed()
        {
            return AreAnyKeysPressed(KEY_CTRL, KEY_ENTER, KEY_SPACE);
        }

        public void ClearAllKeyPressStates()
        {
            var keys = new List<string>(_keysPressed.Keys);
            foreach (var key in keys)
            {
                _keysPressed[key] = false;
            }
        }

        public void Update()
        {
            if (AnyActionKeyPressed())
            {
                Rectangle areaToCheck = this.HitBox;
                var collisions = this.CheckCollision(areaToCheck);
                var levelCollision = collisions.FirstOrDefault(c => c is IWorldMapLevel) as WorldMapLevel;
                if (levelCollision != null)
                {
                    levelCollision.TryEnterLevel(this);
                }
            }
            //no keys or opposing keys pressed on an update cycle where plays is not stationary
            if ((!AnyDirectionKeysPressed() || OpposingKeysPressed())
                && this.MoveState != WorldMapPlayerMoveState.STILL)
            {
                this.MoveState = WorldMapPlayerMoveState.STILL;
                return;
            }

            SetDirectionFromPressedMovementKeys();
            if (AnyDirectionKeysPressed())
            {
                this.MoveState = WorldMapPlayerMoveState.RUNNING;
                TryMove();
            }
        }

        private void TryMove()
        {
            if (IsLeftDirection(_direction))
            {
                TryMoveLeft();
            }
            else if (IsRightDirection(_direction))
            {
                TryMoveRight();
            }

            if (IsUpDirection(_direction))
            {
                TryMoveUp();
            }
            else if (IsDownDirection(_direction))
            {
                TryMoveDown();
            }

            OnMoved();
        }

        private void OnMoved()
        {
            this.Moved?.Invoke(this, EventArgs.Empty);
        }

        private void TryMoveRight()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X + MOVE_VELOCITY,
          this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height);

            var collisions = this.CheckCollision(areaToCheck, true);

            var collisionTile = this.GetLeftMostRightTile(collisions);
                  Rectangle newArea = new Rectangle(this.HitBox.X + MOVE_VELOCITY,
                this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            if (collisionTile != null)
            {
                newArea.X = collisionTile.HitBox.Left - this.HitBox.Width - 1;
            }

            this.HitBox = newArea;
        }

        private void TryMoveDown()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X,
                this.HitBox.Y + MOVE_VELOCITY, this.HitBox.Width,
                this.HitBox.Height + MOVE_VELOCITY);

            var collisions = this.CheckCollision(areaToCheck, true);

            var collisionTile = this.GetTopMostLandingTile(collisions);
            Rectangle newArea = new Rectangle(this.HitBox.X,
                this.HitBox.Y + MOVE_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            if (collisionTile != null)
            {
                newArea.Y = collisionTile.HitBox.Top - this.HitBox.Height - 1;
            }

            this.HitBox = newArea;
        }

        private void TryMoveLeft()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X - MOVE_VELOCITY,
                this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height);

            var collisions = this.CheckCollision(areaToCheck, true);

            var collisionTile = this.GetRightMostLeftTile(collisions);
            Rectangle newArea = new Rectangle(this.HitBox.X - MOVE_VELOCITY,
          this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            if (collisionTile != null)
            {
                newArea.X = collisionTile.HitBox.Right + 1;
            }

            this.HitBox = newArea;
        }

        private void TryMoveUp()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X,
                this.HitBox.Y - MOVE_VELOCITY, this.HitBox.Width,
                this.HitBox.Height + MOVE_VELOCITY);

            var collisions = this.CheckCollision(areaToCheck, true);

            var collisionTile = this.GetCeilingTile(collisions);
            Rectangle newArea = new Rectangle(this.HitBox.X,
          this.HitBox.Y - MOVE_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            if (collisionTile != null)
            {
                newArea.Y = collisionTile.HitBox.Bottom + 1;
            }

            this.HitBox = newArea;
        }

        private void SetDirectionFromPressedMovementKeys()
        {
            if (IsKeyPressed(KEY_LEFT))
            {
                this.Direction = Direction.LEFT;
                if (IsKeyPressed(KEY_UP))
                    this.Direction = Direction.UP_LEFT;
                else if (IsKeyPressed(KEY_DOWN))
                    this.Direction = Direction.DOWN_LEFT;
            }
            else if (IsKeyPressed(KEY_RIGHT))
            {
                this.Direction = Direction.RIGHT;
                if (IsKeyPressed(KEY_UP))
                    this.Direction = Direction.UP_RIGHT;
                else if (IsKeyPressed(KEY_DOWN))
                    this.Direction = Direction.DOWN_RIGHT;
            }
            else if (IsKeyPressed(KEY_UP))
            {
                this.Direction = Direction.UP;
                if (IsKeyPressed(KEY_LEFT))
                    this.Direction = Direction.UP_LEFT;
                else if (IsKeyPressed(KEY_RIGHT))
                    this.Direction = Direction.UP_RIGHT;
            }
            else if (IsKeyPressed(KEY_DOWN))
            {
                this.Direction = Direction.DOWN;
                if (IsKeyPressed(KEY_LEFT))
                    this.Direction = Direction.DOWN_LEFT;
                else if (IsKeyPressed(KEY_RIGHT))
                    this.Direction = Direction.DOWN_RIGHT;
            }
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

        public WorldMapPlayerMoveState MoveState
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
                            images = SpriteSheet.SpriteSheet.PlayerSwimUpRightImages; break; break;
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
                    _sprite = Properties.Resources.keen_stop_down_right; break;
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
