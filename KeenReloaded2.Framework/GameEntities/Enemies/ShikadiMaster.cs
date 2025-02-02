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
    public class ShikadiMaster : CollisionObject, IUpdatable, ISprite, IEnemy, IFireable, ICreateRemove
    {
        private const int BASIC_FALL_VELOCITY = 30;

        private const int LOOK_SPRITE_CHANGE_DELAY = 1;
        private int _currentLookSpriteChangeDelayTick;
        private int _currentLookSprite;

        private int _currentTeleportSprite;
        private readonly int _zIndex;
        private Rectangle _teleportBounds;
        private const int TELEPORT_CHANCE = 80;
        private const int TELEPORT_DELAY = 10;
        private int _currentTeleportDelayTick;
        private const int MAX_TELEPORT_RETRIES = 10;
        private int _currentTeleportRetries;

        private const int FIRE_CHANCE = 80;
        private const int FIRE_TIME = 10;
        private int _currentFireTimeTick;

        public ShikadiMaster(Rectangle area, SpaceHashGrid grid, int zIndex, Rectangle teleportBounds)
            : base(grid, area)
        {
            if (teleportBounds.Width == 0 || teleportBounds.Height == 0)
                throw new ArgumentException("Teleport Bounds need to be an area with a positive width and height");

            _zIndex = zIndex;
            _teleportBounds = teleportBounds;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            _previousState = ShikadiMasterState.FALLING;
            this.State = ShikadiMasterState.FALLING;
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
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_RIGHT);
                }
            }
        }

        public void Update()
        {
            _keen = this.GetClosestPlayer();
            switch (_state)
            {
                case ShikadiMasterState.FALLING:
                    this.Fall();
                    break;
                case ShikadiMasterState.LOOKING:
                    this.Look();
                    break;
                case ShikadiMasterState.FIRING:
                    this.Fire();
                    break;
                case ShikadiMasterState.TELEPORTING:
                    this.Teleport();
                    break;
            }
        }

        private void Teleport()
        {
            if (this.State != ShikadiMasterState.TELEPORTING)
            {
                //if the previous sprite was falling, we need to readjust the hitbox and position to reflect a 
                //much more narrow figure
                int spriteWidth = _sprite.Width;
                this.State = ShikadiMasterState.TELEPORTING;
                int newSpriteWidth = _sprite.Width;
                if (_previousState == ShikadiMasterState.LOOKING || _previousState == ShikadiMasterState.FIRING)
                {
                    int widthDiff = spriteWidth - newSpriteWidth;
                    this.HitBox = new Rectangle(this.HitBox.X + widthDiff / 2, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
                _currentTeleportDelayTick = 0;
                _currentTeleportRetries = 0;
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.SHIKADI_MASTER_TELEPORT);
            }

            if (++_currentTeleportDelayTick == TELEPORT_DELAY / 2)
            {
                _currentTeleportSprite = 1;
                UpdateSprite();
            }
            else if (_currentTeleportDelayTick == TELEPORT_DELAY)
            {
                CreateShockWaves();
                var previousLocation = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                TeleportToRandomLocationWithinBounds();
                bool collides = this.CheckCollision(this.HitBox, true).Any();
                while (collides && _currentTeleportRetries < MAX_TELEPORT_RETRIES)
                {
                    TeleportToRandomLocationWithinBounds();
                    collides = this.CheckCollision(this.HitBox, true).Any();
                    _currentTeleportRetries++;
                }
                if (collides)
                {
                    this.HitBox = new Rectangle(previousLocation.Location, previousLocation.Size);
                }
                _currentTeleportRetries = 0;
                _previousState = ShikadiMasterState.TELEPORTING;
                this.Fall();
            }
        }

        private void TeleportToRandomLocationWithinBounds()
        {
            int newXPos = this.GenerateRandomInteger(_teleportBounds.X, _teleportBounds.Right);
            int newYPos = this.GenerateRandomInteger(_teleportBounds.Y, _teleportBounds.Bottom);
            this.HitBox = new Rectangle(newXPos, newYPos, this.HitBox.Width, this.HitBox.Height);
        }

        private void Look()
        {
            if (this.State != ShikadiMasterState.LOOKING)
            {
                //if the previous sprite was falling, we need to readjust the hitbox and position to reflect a 
                //much more narrow figure
                int spriteWidth = _sprite.Width;
                this.State = ShikadiMasterState.LOOKING;
                int newSpriteWidth = _sprite.Width;
                if (_previousState == ShikadiMasterState.FALLING || _previousState == ShikadiMasterState.TELEPORTING)
                {
                    int widthDiff = spriteWidth - newSpriteWidth;
                    this.HitBox = new Rectangle(this.HitBox.X + widthDiff / 2, this.HitBox.Y, _sprite.Width, _sprite.Height);
                }
            }

            if (IsNothingBeneath())
            {
                this.BasicFall(BASIC_FALL_VELOCITY);
            }

            if (_currentLookSpriteChangeDelayTick++ == LOOK_SPRITE_CHANGE_DELAY)
            {
                _currentLookSpriteChangeDelayTick = 0;
                _currentLookSprite++;
                UpdateSprite();
            }

            int teleportVal = _random.Next(1, TELEPORT_CHANCE + 1);
            if (teleportVal == TELEPORT_CHANCE)
            {
                _previousState = ShikadiMasterState.LOOKING;
                this.Teleport();
                return;
            }
            else
            {
                int fireVal = _random.Next(1, FIRE_CHANCE + 1);
                if (fireVal == FIRE_CHANCE)
                {
                    _previousState = ShikadiMasterState.LOOKING;
                    this.Fire();
                }
            }
        }

        private void Fall()
        {
            if (this.State != ShikadiMasterState.FALLING)
            {
                this.State = ShikadiMasterState.FALLING;
            }
            bool landed = false;
            if (IsNothingBeneath())
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + BASIC_FALL_VELOCITY);
                var tile = this.BasicFallReturnTile(BASIC_FALL_VELOCITY);
                if (tile != null)
                {
                    landed = true;
                }
                else if (_keen.HitBox.IntersectsWith(areaToCheck))
                {
                    _keen.Die();
                }
            }
            else
            {
                landed = true;
            }

            if (landed)
            {
                if (_keen.HitBox.IntersectsWith(this.HitBox))
                {
                    _keen.Die();
                }
                _previousState = ShikadiMasterState.FALLING;
                this.Look();
            }
        }

        public bool DeadlyTouch
        {
            get { return true; }
        }

        public void HandleHit(IProjectile projectile)
        {
            //invincible, does not take damage
        }

        public bool IsActive
        {
            get { return true; }
        }

        public void Fire()
        {
            if (this.State != ShikadiMasterState.FIRING)
            {
                this.State = ShikadiMasterState.FIRING;
                this.Direction = SetDirectionFromObjectHorizontal(_keen, true);
                _currentFireTimeTick = 0;
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.SHIKADI_MASTER_PROJECTILE);
            }

            if (++_currentFireTimeTick == FIRE_TIME / 2)
            {
                FireEnergyBall();
            }
            else if (_currentFireTimeTick == FIRE_TIME)
            {
                _previousState = ShikadiMasterState.FIRING;
                this.Look();
            }
        }

        private void CreateShockWaves()
        {
            int width = 56, height = 14;
            //left wave
            int xPos1 = this.HitBox.X - width - 1, yPos1 = this.HitBox.Bottom - height;
            Rectangle areaToCheck1 = new Rectangle(xPos1, yPos1, width, height);
            var collisions1 = this.CheckCollision(areaToCheck1, true);
            if (!collisions1.Any())
            {
                ShikadiMasterShockwave shockwave1 = new ShikadiMasterShockwave(_collisionGrid, new Rectangle(xPos1, yPos1, width, height), Enums.Direction.LEFT);
                shockwave1.Create += new EventHandler<ObjectEventArgs>(shockwave_Create);
                shockwave1.Remove += new EventHandler<ObjectEventArgs>(shockwave_Remove);

                OnCreate(new ObjectEventArgs() { ObjectSprite = shockwave1 });
            }
            //right wave
            int xPos2 = this.HitBox.Right + 1, yPos2 = this.HitBox.Bottom - height;
            Rectangle areaToCheck2 = new Rectangle(xPos2, yPos2, width, height);
            var collisions2 = this.CheckCollision(areaToCheck2, true);
            if (!collisions2.Any())
            {
                ShikadiMasterShockwave shockwave2 = new ShikadiMasterShockwave(_collisionGrid, new Rectangle(xPos2, yPos2, width, height), Enums.Direction.RIGHT);
                shockwave2.Create += new EventHandler<ObjectEventArgs>(shockwave_Create);
                shockwave2.Remove += new EventHandler<ObjectEventArgs>(shockwave_Remove);

                OnCreate(new ObjectEventArgs() { ObjectSprite = shockwave2 });
            }
        }

        void shockwave_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void shockwave_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        private void FireEnergyBall()
        {
            int width = 28, height = 28;
            int xPos = _direction == Enums.Direction.LEFT ? this.HitBox.Left : this.HitBox.Right - width;
            int yPos = this.HitBox.Y + this.HitBox.Height / 3;

            ShikadiMasterEnergyBall energyBall = new ShikadiMasterEnergyBall(_collisionGrid, new Rectangle(xPos, yPos, width, height), _direction);
            energyBall.Create += new EventHandler<ObjectEventArgs>(energyBall_Create);
            energyBall.Remove += new EventHandler<ObjectEventArgs>(energyBall_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = energyBall });
        }

        void energyBall_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void energyBall_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public bool IsFiring
        {
            get { return _state == ShikadiMasterState.FIRING; }
        }

        public Rectangle TeleportBounds
        {
            get
            {
                return _teleportBounds;
            }
        }

        public int Ammo
        {
            get { return -1; }
        }

        ShikadiMasterState State
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
                if (_state == ShikadiMasterState.FIRING)
                {
                    UpdateSprite();
                }
            }
        }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        private void UpdateSprite()
        {
            switch (_state)
            {
                case ShikadiMasterState.FALLING:
                    _sprite = Properties.Resources.keen5_shikadi_master_teleport1;
                    break;
                case ShikadiMasterState.LOOKING:
                    var spriteSet = SpriteSheet.SpriteSheet.ShikadiMasterLookImages;
                    if (_currentLookSprite >= spriteSet.Length)
                    {
                        _currentLookSprite = 0;
                    }
                    _sprite = spriteSet[_currentLookSprite];
                    break;
                case ShikadiMasterState.FIRING:
                    _sprite = _direction == Enums.Direction.LEFT
                        ? Properties.Resources.keen5_shikadi_master_shoot_left
                        : Properties.Resources.keen5_shikadi_master_shoot_right;
                    break;
                case ShikadiMasterState.TELEPORTING:
                    spriteSet = SpriteSheet.SpriteSheet.ShikadiMasterTeleportImages;
                    if (_currentTeleportSprite >= spriteSet.Length)
                    {
                        _currentTeleportSprite = 0;
                    }
                    _sprite = spriteSet[_currentTeleportSprite];
                    break;
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private ShikadiMasterState _state;
        private ShikadiMasterState _previousState;
        private Image _sprite;
        private CommanderKeen _keen;
        private Enums.Direction _direction;

        public bool CanUpdate => true;

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
            string imageName = nameof(Properties.Resources.keen5_shikadi_master_look1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_teleportBounds.X}{separator}{_teleportBounds.Y}{separator}{_teleportBounds.Width}{separator}{_teleportBounds.Height}";
        }
    }

    enum ShikadiMasterState
    {
        LOOKING,
        FIRING,
        TELEPORTING,
        FALLING
    }
}
