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
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Framework.GameEntities.Items;


namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class GnosticeneAncient : DestructibleObject, IEnemy, IUpdatable, IGravityObject, ISprite, IStunnable, ICreateRemove, IZombieBountyEnemy
    {
        private GnosticeneAncientState _moveState;
        private GnosticeneAncientState _previousState;
        private CommanderKeen _keen;
        private Rectangle _rangeOfVision;
        private Size _originalHitboxSize;

        private const int MAX_HORIZONTAL_VISION = 300;
        private const int MAX_VERTICAL_VISION = 200;

        private const int MAX_FALL_VELOCITY = 50;
        private const int GRAVITY_ACCELERATION = 10;
        private const int INITIAL_FALL_VELOCITY = 5;
        private const int INITIAL_JUMP_VELOCITY = 50;
        private const int INITIAL_HORIZONTAL_VELOCITY = 28;
        private const int HORIZONTAL_VELOCITY_DECELERATION = 4;
        private const int EDGE_OFFSET = 64;
        private int _maxJumpHeight;

        private int _currentHorizontalVelocity = INITIAL_HORIZONTAL_VELOCITY;
        private int _currentVerticalVelocity = INITIAL_FALL_VELOCITY;
        private Image _sprite;
        private Enums.Direction _direction;

        private int _currentJumpImage;
        private const int JUMP_SPRITE_CHANGE_DELAY = 2;
        private int _currentJumpSpriteChangeDelayTick;
        private const int JUMP_DELAY = 5;
        private int _currentJumpDelayTick;
        private bool _justStoleItems;
        private bool _disposed;

        private Image[] _jumpLeftImages = new Image[]
        {
            Properties.Resources.keen4_gnosticene_ancient_jump_left1,
            Properties.Resources.keen4_gnosticene_ancient_jump_left2,
            Properties.Resources.keen4_gnosticene_ancient_jump_left3
        };

        private Image[] _jumpRightImages = new Image[]
        {
            Properties.Resources.keen4_gnosticene_ancient_jump_right1,
            Properties.Resources.keen4_gnosticene_ancient_jump_right2,
            Properties.Resources.keen4_gnosticene_ancient_jump_right3
        };

        private int _currentVanishImage;
        private const int VANISH_SPRITE_CHANGE_DELAY = 2;
        private const int VANISH_CHANCE = 5;
        private int _currentVanishSpriteChangeDelayTick;
        private Image[] _vanishingImages = new Image[]
        {
            Properties.Resources.keen4_gnosticene_ancient_vanish1,
            Properties.Resources.keen4_gnosticene_ancient_vanish2,
            Properties.Resources.keen4_gnosticene_ancient_vanish3,
            Properties.Resources.keen4_gnosticene_ancient_vanish4
        };
        private bool _reAppeared;

        private const int STUN_SPRITE_CHANGE_DELAY = 1;
        private readonly int _zIndex;
        private int _currentStunSpriteChangeDelayTick;
        private int _currentStunImage;
        private Image[] _stunImages = new Image[]
        {
            Properties.Resources.keen4_gnosticene_ancient_stun2,
            Properties.Resources.keen4_gnosticene_ancient_stun3,
            Properties.Resources.keen4_gnosticene_ancient_stun4
        };

        public GnosticeneAncient(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            _maxJumpHeight = GetMaxJumpHeightFromPhysicsVariables();
            SetRangeOfVision();
            this.MoveState = GnosticeneAncientState.LOOKING;
            _previousState = this.MoveState;
            this.Direction = Enums.Direction.LEFT;
            _originalHitboxSize = this.HitBox.Size;
           
            if (_collisionGrid != null && _collidingNodes != null 
                && GetTopMostLandingTile(_currentVerticalVelocity) == null)
            {
                this.Fall();
            }
        }

        private void SetRangeOfVision()
        {
            _rangeOfVision = new Rectangle(this.HitBox.X - MAX_HORIZONTAL_VISION, this.HitBox.Y - MAX_VERTICAL_VISION, this.HitBox.Width + MAX_HORIZONTAL_VISION * 2, this.HitBox.Height + MAX_VERTICAL_VISION);
        }

        GnosticeneAncientState MoveState
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


        public override void Die()
        {
            this.Stun();
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
            get { return !this.IsStunned; }
        }

        public void Update()
        {

            if (!_disposed)
            {
                _keen = this.GetClosestPlayer();
                switch (_moveState)
                {
                    case GnosticeneAncientState.LOOKING:
                        this.Look();
                        break;
                    case GnosticeneAncientState.JUMPING:
                        this.Jump();
                        break;
                    case GnosticeneAncientState.FALLING:
                        this.Fall();
                        break;
                    case GnosticeneAncientState.VANISHING:
                        this.Vanish();
                        break;
                    case GnosticeneAncientState.APPEARING:
                        this.Appear();
                        break;
                    case GnosticeneAncientState.STUNNED:
                        this.Stun();
                        break;
                }

                if (!_collidingNodes.Any())
                {
                    if (!_disposed)
                    {
                        this.DetachFromCollisionGrid();
                        _disposed = true;
                        this.TakeDamage(int.MaxValue);
                        OnRemove(new ObjectEventArgs() { ObjectSprite = this });
                    }
                }
            }
        }

        private void Appear()
        {
            if (this.MoveState != GnosticeneAncientState.APPEARING)
            {
                this.MoveState = GnosticeneAncientState.APPEARING;
                _reAppeared = false;
            }

            if (_previousState == GnosticeneAncientState.VANISHING && !_reAppeared)
            {
                int maxX = _collisionGrid._nodes.Select(s => s.HashBox.X).Max();
                int maxY = _collisionGrid._nodes.Select(s => s.HashBox.Y).Max();
                int x = _random.Next(32, maxX - this.HitBox.Width);
                int y = _random.Next(32, maxY - this.HitBox.Height);
                Point newLocation = new Point(x, y);
                TeleportToLocation(newLocation);
                _reAppeared = true;
            }

            this.UpdateSprite();
            this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
            if (_currentVanishImage == 0)
            {
                _previousState = GnosticeneAncientState.APPEARING;
                this.Fall();
            }
        }

        private void TeleportToLocation(Point newLocation)
        {
            this.HitBox = new Rectangle(newLocation, this.HitBox.Size);
            _collidingNodes = _collisionGrid.GetCurrentHashes(this);
        }

        private void Vanish()
        {
            if (this.MoveState != GnosticeneAncientState.VANISHING)
            {
                this.MoveState = GnosticeneAncientState.VANISHING;
                _justStoleItems = false;
            }
            this.UpdateSprite();
            this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
            if (_currentVanishImage == _vanishingImages.Length - 1)
            {
                _previousState = GnosticeneAncientState.VANISHING;
                this.Appear();
            }
        }

        private void Look()
        {
            //initialize look state if it is from previous State
            if (this.MoveState != GnosticeneAncientState.LOOKING)
            {
                this.MoveState = GnosticeneAncientState.LOOKING;
                _currentVerticalVelocity = 0;
                _currentHorizontalVelocity = 0;
            }

            if (_currentJumpDelayTick++ == JUMP_DELAY)
            {
                _currentJumpDelayTick = 0;
                var collisions = this.CheckCollision(_rangeOfVision, false, false);
                var items = collisions.OfType<Item>();
                //if items are in sight, approach the first one I find
                if (items.Any())
                {
                    var itemToChase = items.FirstOrDefault(i => i.CanSteal);
                    if (itemToChase != null)
                    {
                        this.Direction = SetDirectionFromObjectHorizontal(itemToChase, true);
                        //if I cannot reach the Item, vanish
                        this.Jump();
                    }         
                    else
                    {
                        this.Jump();
                    }
                    //return;
                }
                else //otherwise, avoid keen
                {
                    this.Direction = SetDirectionFromObjectHorizontal(_keen, false);
                }
                //vanish if I stole items or keen is in vision
                if (_keen.HitBox.IntersectsWith(_rangeOfVision))
                {
                    int vanishVal = _random.Next(1, VANISH_CHANCE + 1);
                    if (_justStoleItems || vanishVal == VANISH_CHANCE)
                    {
                        this.Vanish();
                    }
                }
                else//otherwise, jump
                {
                    this.Jump();
                }
            }
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK && c.HitBox.Bottom < this.HitBox.Top).ToList();

            if (_direction == Enums.Direction.RIGHT)
            {
                if (debugTiles.Any(t => t.HitBox.Left <= this.HitBox.Right))
                {
                    int maxBottom = debugTiles.Where(t => t.HitBox.Left <= this.HitBox.Right).Select(c => c.HitBox.Bottom).Max();
                    CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                    return obj;
                }
            }
            else if (debugTiles.Any(t => t.HitBox.Right >= this.HitBox.Left))
            {
                int maxBottom = debugTiles.Where(t => t.HitBox.Right >= this.HitBox.Left).Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }

            return null;
        }

        public void Jump()
        {
            if (this.MoveState != GnosticeneAncientState.JUMPING)
            {
                this.MoveState = GnosticeneAncientState.JUMPING;
                _currentVerticalVelocity = INITIAL_JUMP_VELOCITY;
                _currentHorizontalVelocity = INITIAL_HORIZONTAL_VELOCITY;
            }
            //initialize collision detection variables
            var horizontalSpeed = _previousState != GnosticeneAncientState.VANISHING ? _currentHorizontalVelocity : 0;
            int xOffset = _direction == Enums.Direction.LEFT ? horizontalSpeed * -1 : horizontalSpeed;
            int xLocation = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            //check for collisions
            Rectangle areaToCheck = new Rectangle(xLocation, this.HitBox.Y - _currentVerticalVelocity, this.HitBox.Width + horizontalSpeed, this.HitBox.Height + _currentVerticalVelocity);
            var collisions = this.CheckCollision(areaToCheck, false, false);

            //get collided items to steal
            var items = collisions.OfType<Item>();
            var stealableItems = items.Any() ? items.Where(c => c.CanSteal).ToList() : new List<Item>();

            //get horizontal collision tile if any
            var horizontalTile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            //get bottom most ceiling tile if any
            var ceilingTile = GetCeilingTile(collisions);
            //initiate move logic
            //Get Horizontal Move Point
            if (horizontalSpeed != 0)//if speed is zero, dont bother
            {
                Point newHorizontalLocation;
                if (horizontalTile != null)
                {
                    newHorizontalLocation = _direction == Enums.Direction.LEFT ? new Point(horizontalTile.HitBox.Right + 1, this.HitBox.Y) :
                        new Point(horizontalTile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y);
                    //steal colliding items
                    var itemsToSteal = _direction == Enums.Direction.LEFT ? stealableItems.Where(i => i.HitBox.Right > horizontalTile.HitBox.Right).ToList()
                        : stealableItems.Where(i => i.HitBox.Left < horizontalTile.HitBox.Left).ToList();
                    if (itemsToSteal.Any())
                    {
                        _justStoleItems = true;
                    }
                    AddDestroyersForItems(itemsToSteal);
                    stealableItems = stealableItems.Except(itemsToSteal).ToList();
                    this.Direction = ChangeHorizontalDirection(this.Direction);
                }
                else
                {
                    newHorizontalLocation = new Point(this.HitBox.X + xOffset, this.HitBox.Y);
                    //steal colliding items
                    if (stealableItems.Any())
                    {
                        _justStoleItems = true;
                    }
                    AddDestroyersForItems(stealableItems);
                    stealableItems = new List<Item>();
                }
                //if we won't set the velocity to zero, decelerate;
                if (_currentHorizontalVelocity - HORIZONTAL_VELOCITY_DECELERATION > 0)
                {
                    _currentHorizontalVelocity -= HORIZONTAL_VELOCITY_DECELERATION;
                }
                //set horizontal Direction and update horizontal collision nodes
                this.HitBox = new Rectangle(newHorizontalLocation, this.HitBox.Size);
                this.UpdateCollisionNodes(_direction);
            }
            //get new vertical position
            Point newVerticalLocation;
            if (ceilingTile != null)
            {
                newVerticalLocation = new Point(this.HitBox.X, ceilingTile.HitBox.Bottom + 1);
                //steal items
                var itemsToSteal = stealableItems.Where(i => i.HitBox.Bottom > ceilingTile.HitBox.Bottom).ToList();
                if (itemsToSteal.Any())
                {
                    _justStoleItems = true;
                }
                AddDestroyersForItems(itemsToSteal);
                //terminate jump when hitting ceiling
                _previousState = GnosticeneAncientState.JUMPING;
                this.Fall();
            }
            else
            {
                newVerticalLocation = new Point(this.HitBox.X, this.HitBox.Y - _currentVerticalVelocity);
                //steal items
                if (stealableItems.Any())
                {
                    _justStoleItems = true;
                }
                AddDestroyersForItems(stealableItems);
                //update vertical position and collision nodes
                this.HitBox = new Rectangle(newVerticalLocation, this.HitBox.Size);
                this.UpdateCollisionNodes(Enums.Direction.DOWN);

                //decrease jumping velocity if not at max
                if (_currentVerticalVelocity - GRAVITY_ACCELERATION >= 0)
                {
                    _currentVerticalVelocity -= GRAVITY_ACCELERATION;
                }
                else
                {
                    //fall if reached maximum jump
                    _previousState = GnosticeneAncientState.JUMPING;
                    this.Fall();
                    return;
                }
            }
            //Update Sprite
            this.UpdateSprite();
        }

        private void SelfDestruct()
        {
            foreach (var node in _collidingNodes)
            {
                node.Objects.Remove(this);
            }
            OnRemove(new ObjectEventArgs() { ObjectSprite = this });
        }

        public void Fall()
        {
            //initialize fall state if this is first time 
            if (this.MoveState != GnosticeneAncientState.FALLING)
            {
                this.MoveState = GnosticeneAncientState.FALLING;
                _currentVerticalVelocity = INITIAL_FALL_VELOCITY;
                _currentHorizontalVelocity = INITIAL_HORIZONTAL_VELOCITY;
            }

            if (_previousState == GnosticeneAncientState.APPEARING)
            {
                this.HitBox = new Rectangle(this.HitBox.Location, _originalHitboxSize);
            }

            //initialize collision detection variables
            var horizontalSpeed = _previousState == GnosticeneAncientState.LOOKING ? _currentHorizontalVelocity : 0;
            int xOffset = _direction == Enums.Direction.LEFT ? horizontalSpeed * -1 : horizontalSpeed;
            int xLocation = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            //check for collisions
            Rectangle areaToCheck = new Rectangle(xLocation, this.HitBox.Y, this.HitBox.Width + horizontalSpeed, this.HitBox.Height + _currentVerticalVelocity);
            var collisions = this.CheckCollision(areaToCheck, false, false);

            //get collided items to steal
            var items = collisions.OfType<Item>();
            var stealableItems = items.Any() ? items.Where(c => c.CanSteal).ToList() : new List<Item>();

            //get horizontal collision tile if any
            var horizontalTile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            //get top most landing tile if any
            var landingTile = GetTopMostLandingTile(collisions);
            //initiate move logic
            //Get Horizontal Move Point
            if (horizontalSpeed != 0)//if speed is zero, dont bother
            {
                Point newHorizontalLocation;
                if (horizontalTile != null)
                {
                    newHorizontalLocation = _direction == Enums.Direction.LEFT ? new Point(horizontalTile.HitBox.Right + 1, this.HitBox.Y) :
                        new Point(horizontalTile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y);
                    //steal colliding items
                    var itemsToSteal = _direction == Enums.Direction.LEFT ? stealableItems.Where(i => i.HitBox.Right > horizontalTile.HitBox.Right).ToList()
                        : stealableItems.Where(i => i.HitBox.Left < horizontalTile.HitBox.Left).ToList();
                    if (itemsToSteal.Any())
                    {
                        _justStoleItems = true;
                    }
                    AddDestroyersForItems(itemsToSteal);
                    stealableItems = stealableItems.Except(itemsToSteal).ToList();
                }
                else
                {
                    newHorizontalLocation = new Point(this.HitBox.X + xOffset, this.HitBox.Y);
                    //steal colliding items
                    if (stealableItems.Any())
                    {
                        _justStoleItems = true;
                    }
                    AddDestroyersForItems(stealableItems);
                    stealableItems = new List<Item>();
                }
                //if we won't set the velocity to zero, decelerate;
                if (_currentHorizontalVelocity - HORIZONTAL_VELOCITY_DECELERATION > 0)
                {
                    _currentHorizontalVelocity -= HORIZONTAL_VELOCITY_DECELERATION;
                }
                //set horizontal Direction and update horizontal collision nodes
                this.HitBox = new Rectangle(newHorizontalLocation, this.HitBox.Size);
                this.UpdateCollisionNodes(_direction);
            }
            //get new vertical position
            Point newVerticalLocation;
            if (landingTile != null)
            {
                newVerticalLocation = new Point(this.HitBox.X, landingTile.HitBox.Top - this.HitBox.Height - 1);
                //steal items
                var itemsToSteal = stealableItems.Where(i => i.HitBox.Top < landingTile.HitBox.Top).ToList();
                if (itemsToSteal.Any())
                {
                    _justStoleItems = true;
                }
                AddDestroyersForItems(itemsToSteal);
                //terminate fall when landing on something
                _previousState = GnosticeneAncientState.FALLING;
                this.Look();
            }
            else
            {
                newVerticalLocation = new Point(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity);
                //steal items
                if (stealableItems.Any())
                {
                    _justStoleItems = true;
                }
                AddDestroyersForItems(stealableItems);
                //increase falling velocity if not at max
                if (_currentVerticalVelocity + GRAVITY_ACCELERATION <= MAX_FALL_VELOCITY)
                {
                    _currentVerticalVelocity += GRAVITY_ACCELERATION;
                }
            }
            //update vertical position and collision nodes
            this.HitBox = new Rectangle(newVerticalLocation, this.HitBox.Size);
            this.UpdateCollisionNodes(Enums.Direction.DOWN);

            //Update Sprite
            this.UpdateSprite();

            //if we've fallen way off the map, self destruct
            if (_collidingNodes == null || !_collidingNodes.Any())
            {
                SelfDestruct();
            }
        }

        private void AddDestroyersForItems(List<Item> stealableItems)
        {
            foreach (var item in stealableItems)
            {
                GnosticeneItemDestroyer destroyer = new GnosticeneItemDestroyer(_collisionGrid, item.HitBox, _zIndex, item);
                destroyer.Remove += new EventHandler<ObjectEventArgs>(destroyer_Remove);
                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = destroyer
                };
                OnCreate(args);
            }
        }

        void destroyer_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        void destroyer_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        protected void OnRemove(ObjectEventArgs args)
        {
            if (Remove != null)
            {
                Remove(this, args);
            }
        }

        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                Create(this, args);
            }
        }

        private int GetMaxJumpHeightFromPhysicsVariables()
        {
            int initialVelocity = INITIAL_JUMP_VELOCITY;
            int retVal = initialVelocity;
            int i = INITIAL_JUMP_VELOCITY - GRAVITY_ACCELERATION;
            while (i > 0)
            {
                if (i < GRAVITY_ACCELERATION)
                {
                    retVal += i;
                    i = 0;
                }
                else
                {
                    retVal += i;
                    i -= GRAVITY_ACCELERATION;
                }
            }
            return retVal;
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
                    SetRangeOfVision();
                }
            }
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile;
            var items = collisions.Where(c => c.CollisionType == CollisionType.BLOCK || c.CollisionType == CollisionType.PLATFORM || c.CollisionType == CollisionType.POLE_TILE).ToList();

            var landingTiles = items.Where(h => h.HitBox.Top >= this.HitBox.Top);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        public bool CanJump
        {
            get { return this.MoveState == GnosticeneAncientState.LOOKING; }
        }

        private void UpdateSprite()
        {
            switch (_moveState)
            {
                case GnosticeneAncientState.LOOKING:
                    _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_gnosticene_ancient_look_left
                        : Properties.Resources.keen4_gnosticene_ancient_look_right;
                    break;
                case GnosticeneAncientState.JUMPING:
                case GnosticeneAncientState.FALLING:
                    var spriteSet = _direction == Enums.Direction.LEFT ? _jumpLeftImages : _jumpRightImages;
                    if (_currentJumpSpriteChangeDelayTick++ == JUMP_SPRITE_CHANGE_DELAY)
                    {
                        _currentJumpSpriteChangeDelayTick = 0;
                        if (_currentJumpImage >= spriteSet.Length)
                        {
                            _currentJumpImage = 0;
                        }
                        _sprite = spriteSet[_currentJumpImage++];
                    }
                    break;
                case GnosticeneAncientState.VANISHING:
                    if (_currentVanishSpriteChangeDelayTick++ == VANISH_SPRITE_CHANGE_DELAY)
                    {
                        _currentVanishSpriteChangeDelayTick = 0;
                        if (_currentVanishImage < _vanishingImages.Length - 1)
                        {
                            _currentVanishImage++;
                        }
                        _sprite = _vanishingImages[_currentVanishImage];
                    }
                    break;
                case GnosticeneAncientState.APPEARING:
                    if (_currentVanishSpriteChangeDelayTick++ == VANISH_SPRITE_CHANGE_DELAY)
                    {
                        _currentVanishSpriteChangeDelayTick = 0;
                        if (_currentVanishImage > 0)
                        {
                            _currentVanishImage--;
                        }
                        _sprite = _vanishingImages[_currentVanishImage];
                    }
                    break;
                case GnosticeneAncientState.STUNNED:
                    if (_currentStunSpriteChangeDelayTick++ == STUN_SPRITE_CHANGE_DELAY)
                    {
                        _currentStunSpriteChangeDelayTick = 0;
                        if (_currentStunImage >= _stunImages.Length)
                        {
                            _currentStunImage = 0;
                        }
                        _sprite = _stunImages[_currentStunImage++];
                    }
                    break;
            }
        }

        public void Stun()
        {
            if (this.MoveState != GnosticeneAncientState.STUNNED)
            {
                this.MoveState = GnosticeneAncientState.STUNNED;
                _sprite= Properties.Resources.keen4_gnosticene_ancient_stun1;
                this.HitBox = new Rectangle(this.HitBox.Location, new Size(_sprite.Width, _sprite.Height + 30));
                this.UpdateCollisionNodes(Enums.Direction.DOWN);
                _currentVerticalVelocity = INITIAL_FALL_VELOCITY;
            }

            var tile = GetTopMostLandingTile(_currentHorizontalVelocity);
            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                this.UpdateCollisionNodes(Enums.Direction.DOWN);
                _currentHorizontalVelocity = INITIAL_FALL_VELOCITY;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentHorizontalVelocity, this.HitBox.Width, this.HitBox.Height);
                this.UpdateCollisionNodes(Direction.DOWN);
                if (_currentHorizontalVelocity + GRAVITY_ACCELERATION <= MAX_FALL_VELOCITY)
                {
                    _currentHorizontalVelocity += GRAVITY_ACCELERATION;
                }
            }
            UpdateSprite();
        }

        public bool IsStunned
        {
            get { return this.MoveState == GnosticeneAncientState.STUNNED; }
        }

        public PointItemType PointItem => PointItemType.KEEN4_SHIKADI_SODA;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_gnosticene_ancient_jump_left1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }

    }

    enum GnosticeneAncientState
    {
        LOOKING,
        JUMPING,
        FALLING,
        VANISHING,
        APPEARING,
        STUNNED
    }
}
