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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Orbatrix : CollisionObject, IUpdatable, ISprite, IEnemy
    {
        private OrbatrixState _state;
        private Enums.Direction _direction;
        private Image _sprite;
        private CommanderKeen _keen;

        private Image[] _lookSprites, _lookLeftSprites, _lookRightSprites, _attackSprites;

        private const int LOOK_SPRITE_CHANGE_DELAY = 1;
        private int _currentLookSpriteChangeDelayTick;
        private int _currentLookSprite;

        private const int ATTACK_SPRITE_CHANGE_DELAY = 1;
        private int _currentAttackSpriteChangeDelayTick;
        private int _currentAttackSprite;

        private const int ATTACK_CHANCE = 15;
        private const int ATTACK_PREP_TIME = 5;
        private int _attackPrepTimeTick;
        private const int ATTACK_VELOCITY = 30;
        private const int GRAVITY_ACCELERATION = 5;
        private const int INITIAL_VERTICAL_VELOCITY = 5;
        private const int MAX_VERTICAL_VELOCITY = 30;

        private const int HORIZONTAL_VISION_DISTANCE = 150;
        private const int VERTICAL_VISION_DISTANCE = TOP_VERTICAL_HOVER_DISTANCE;

        private const int BOUNCES = 5;
        private int _bounceCount;

        private const int LOOK_TIME = 8;
        private const int LOOK_CHANCE = 30;
        private int _lookTimeTick;

        private const int TOP_VERTICAL_HOVER_DISTANCE = 60;
        private const int BOTTOM_VERTICAL_HOVER_DISTANCE = 20;
        private const int MOVE_VELOCITY = 8;
        private const int VERTICAL_HOVER_SPEED = 2;
        private Direction _hoverDirection = Direction.UP;
        private int _verticalVelocity, _horizontalVelocity;

        private const int ASCENSION_VELOCITY = 4;
        private readonly int ASCENSIONS = TOP_VERTICAL_HOVER_DISTANCE / ASCENSION_VELOCITY;
        private int _ascensionCount;

        private const int MOVE_PREP_TIME = 5;
        private int _movePrepTimeTick;

        private bool _isMovingFromAttack;
        private readonly int _zIndex;

        public Orbatrix(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            _lookSprites = SpriteSheet.SpriteSheet.OrbatrixLookImages;
            _lookLeftSprites = SpriteSheet.SpriteSheet.OrbatrixLookLeftImages;
            _lookRightSprites = SpriteSheet.SpriteSheet.OrbatrixLookRightImages;
            _attackSprites = SpriteSheet.SpriteSheet.OrbatrixAttackImages;

            _direction = this.GetRandomHorizontalDirection();
            UpdateSprite();
            this.State = OrbatrixState.LOOKING;
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
                    if (_state == OrbatrixState.MOVING)
                    {
                        this.UpdateCollisionNodes(this.Direction);
                        this.UpdateCollisionNodes(_hoverDirection);
                    }
                    else if (_state == OrbatrixState.ATTACKING)
                    {
                        if (_horizontalVelocity < 0)
                        {
                            this.UpdateCollisionNodes(Enums.Direction.LEFT);
                        }
                        else if (_horizontalVelocity > 0)
                        {
                            this.UpdateCollisionNodes(Enums.Direction.RIGHT);
                        }

                        if (_verticalVelocity < 0)
                        {
                            this.UpdateCollisionNodes(Enums.Direction.UP);
                        }
                        else if (_verticalVelocity > 0)
                        {
                            this.UpdateCollisionNodes(Enums.Direction.DOWN);
                        }
                    }
                    else if (_state == OrbatrixState.ASCENDING)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.UP);
                    }
                }
            }
        }

        public void Update()
        {
            _keen = this.GetClosestPlayer();
            switch (_state)
            {
                case OrbatrixState.LOOKING:
                    this.Look();
                    break;
                case OrbatrixState.MOVING:
                    this.Move();
                    break;
                case OrbatrixState.PREPARING_ATTACK:
                    this.PrepareAttack();
                    break;
                case OrbatrixState.ATTACKING:
                    this.Attack();
                    break;
                case OrbatrixState.ASCENDING:
                    this.Ascend();
                    break;
                case OrbatrixState.PREPARING_MOVE:
                    this.PrepareMove();
                    break;
            }
        }

        private void PrepareMove()
        {
            if (this.State != OrbatrixState.PREPARING_MOVE)
            {
                this.State = OrbatrixState.PREPARING_MOVE;
                this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                _movePrepTimeTick = 0;
                _isMovingFromAttack = true;
            }

            if (_movePrepTimeTick++ == MOVE_PREP_TIME)
            {
                this.Look();
            }
        }

        private void Ascend()
        {
            if (this.State != OrbatrixState.ASCENDING)
            {
                this.State = OrbatrixState.ASCENDING;
                _ascensionCount = 0;
            }

            //set up which direction to hover in
            var floorTile = GetTopMostLandingTile(TOP_VERTICAL_HOVER_DISTANCE + 4);
            if (floorTile != null)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y - ASCENSION_VELOCITY, this.HitBox.Width, this.HitBox.Height + ASCENSION_VELOCITY);
                var collisions = this.CheckCollision(areaToCheck, true);
                var tile = GetCeilingTile(collisions);
                if (tile != null)
                {
                    this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - ASCENSION_VELOCITY, this.HitBox.Width, this.HitBox.Height);
                }
                _ascensionCount++;
                if (_ascensionCount == ASCENSIONS)
                {
                    this.PrepareMove();
                }
            }
            else //ascended high enough
            {
                // in this case the bug in the orginal episode will happen. We need to check if we are moving from an attack and if so,
                //hover down until we see a platform beneath us
                this.PrepareMove();
            }
        }

        private void Attack()
        {
            if (this.State != OrbatrixState.ATTACKING)
            {
                this.State = OrbatrixState.ATTACKING;
                this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                _bounceCount = 0;
                _horizontalVelocity = _direction == Enums.Direction.LEFT ? ATTACK_VELOCITY * -1 : ATTACK_VELOCITY;
                _verticalVelocity = INITIAL_VERTICAL_VELOCITY;
            }
            //TODO: Implement attack bounce
            int xOffset = _horizontalVelocity;
            int yOffset = _verticalVelocity;
            int xPosCheck = _horizontalVelocity < 0 ? this.HitBox.X + xOffset : this.HitBox.X;
            int yPosCheck = _verticalVelocity < 0 ? this.HitBox.Y + yOffset : this.HitBox.Y;

            Rectangle areaToCheck = new Rectangle(xPosCheck, yPosCheck, this.HitBox.Width + Math.Abs(_horizontalVelocity), this.HitBox.Height + Math.Abs(_verticalVelocity));
            var collisions = this.CheckCollision(areaToCheck, true);

            var horizontalTile = _horizontalVelocity < 0 ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            var verticalTile = _verticalVelocity < 0 ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);

            int newXPos = this.HitBox.X + xOffset;
            int newYPos = this.HitBox.Y + yOffset;
            int xDistFromCollision = -1;
            int yDistFromCollision = -1;

            if (horizontalTile != null)
            {
                newXPos = _horizontalVelocity < 0 ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                xDistFromCollision = _horizontalVelocity < 0 ? this.HitBox.Left - horizontalTile.HitBox.Right : horizontalTile.HitBox.Left - this.HitBox.Right;
            }

            if (verticalTile != null)
            {
                newYPos = _verticalVelocity < 0 ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                yDistFromCollision = _verticalVelocity < 0 ? this.HitBox.Top - verticalTile.HitBox.Bottom : this.HitBox.Bottom - verticalTile.HitBox.Top;
            }

            //in the event of collisions on both the horizontal and vertical plane, prioritize the one that is closer to the object
            //and update both x,y positions accordingly
            if (verticalTile != null && horizontalTile != null && horizontalTile != verticalTile)
            {
                if (xDistFromCollision < yDistFromCollision)
                {
                    newYPos = _verticalVelocity < 0 ? this.HitBox.Y - xDistFromCollision : this.HitBox.Y + xDistFromCollision;
                }
                else
                {
                    newXPos = _horizontalVelocity < 0 ? this.HitBox.X - yDistFromCollision : this.HitBox.X + yDistFromCollision;
                }
            }//priority to horizontal bounce
            else if (horizontalTile != null && verticalTile != null && horizontalTile == verticalTile)
            {
                newYPos = _verticalVelocity < 0 ? this.HitBox.Y - xDistFromCollision : this.HitBox.Y + xDistFromCollision;
            }

            if (horizontalTile != null || verticalTile != null)
            {
                _horizontalVelocity *= -1;
                if (_verticalVelocity > 0 && verticalTile != null)
                {
                    _verticalVelocity = MAX_VERTICAL_VELOCITY * -1;
                }
                else if (_verticalVelocity > 0)
                {
                    _verticalVelocity *= -1;
                }
                _bounceCount++;
            }
            else if (_verticalVelocity + GRAVITY_ACCELERATION <= MAX_VERTICAL_VELOCITY)
            {
                _verticalVelocity += GRAVITY_ACCELERATION;
            }
            else
            {
                _verticalVelocity = MAX_VERTICAL_VELOCITY;
            }

            //check for keen collisions
            CheckForKeenCollisions(_horizontalVelocity < 0, _verticalVelocity < 0, newXPos, newYPos);

            //update the location to the new x,y position
            this.HitBox = new Rectangle(newXPos, newYPos, this.HitBox.Width, this.HitBox.Height);

            this.UpdateSpriteByDelayBase(ref _currentAttackSpriteChangeDelayTick, ref _currentAttackSprite, ATTACK_SPRITE_CHANGE_DELAY, UpdateSprite);

            if (_bounceCount == BOUNCES)
            {
                this.Ascend();
            }
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK && c.HitBox.Bottom <= this.HitBox.Top && c.HitBox.Left <= this.HitBox.Right && c.HitBox.Right >= this.HitBox.Left).ToList();
            if (debugTiles.Any())
            {
                int maxBottom = debugTiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        protected override CollisionObject GetTopMostLandingTile(int currentFallVelocity)
        {
            CollisionObject topMostTile;
            Rectangle areaTocheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, currentFallVelocity);
            var items = this.CheckCollision(areaTocheck, true);

            var landingTiles = items.Where(h => h.HitBox.Top >= this.HitBox.Top
                && h.HitBox.Left <= this.HitBox.Right && h.HitBox.Right >= this.HitBox.Left);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile;
            var landingTiles = collisions.Where(h => (h.CollisionType == CollisionType.BLOCK || h.CollisionType == CollisionType.PLATFORM)
                && h.HitBox.Top >= this.HitBox.Top && h.HitBox.Left <= this.HitBox.Right && h.HitBox.Right >= this.HitBox.Left);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        private void CheckForKeenCollisions(bool isLeftDirection, bool isUpDirection, int newXPos, int newYPos)
        {
            int xMovement = Math.Abs(this.HitBox.X - newXPos);
            int yMovement = Math.Abs(this.HitBox.Y - newYPos);
            int xCheck = isLeftDirection ? this.HitBox.X - xMovement : this.HitBox.X;
            int yCheck = isUpDirection ? this.HitBox.Y - yMovement : this.HitBox.Y;
            Rectangle areaToCheckForKeen = new Rectangle(xCheck, yCheck, this.HitBox.Width + xMovement, this.HitBox.Height + yMovement);
            KillKeenIfColliding(areaToCheckForKeen);
        }

        protected void KillKeenIfColliding(Rectangle areaToCheck)
        {
            if (_keen.HitBox.Right >= areaToCheck.Left && _keen.HitBox.Left <= areaToCheck.Right)
            {
                double rise = areaToCheck.Height - this.HitBox.Height, run = areaToCheck.Width;
                double slope = rise / run;
                if (_verticalVelocity < 0)
                    slope *= -1.0;

                double xInputKeen = _keen.HitBox.Right - areaToCheck.Left;
                double yOutput = this.HitBox.Y + slope * xInputKeen;

                Rectangle killArea = new Rectangle(_keen.HitBox.Left, (int)yOutput, this.HitBox.Width, this.HitBox.Height);
                if (_keen.HitBox.IntersectsWith(killArea))
                {
                    _keen.Die();
                }
            }
        }

        private void PrepareAttack()
        {
            if (this.State != OrbatrixState.PREPARING_ATTACK)
            {
                this.State = OrbatrixState.PREPARING_ATTACK;
                _attackPrepTimeTick = 0;
                this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
            }

            if (_attackPrepTimeTick++ == ATTACK_PREP_TIME)
            {
                this.Attack();
            }
        }

        private void Move()
        {
            if (this.State != OrbatrixState.MOVING)
            {
                this.State = OrbatrixState.MOVING;
                _currentLookSprite = 0;
                if (IsOnEdge(this.Direction))
                {
                    this.BasicFall(1);
                }
            }
            //set up which direction to hover in
            var floorTile = GetTopMostLandingTile(TOP_VERTICAL_HOVER_DISTANCE + 4);
            if (floorTile != null)
            {
                _isMovingFromAttack = false;
                int distanceFromFloorTile = floorTile.HitBox.Top - this.HitBox.Bottom;
                if (distanceFromFloorTile >= TOP_VERTICAL_HOVER_DISTANCE)
                {
                    _hoverDirection = Enums.Direction.DOWN;
                }
                else if (distanceFromFloorTile <= BOTTOM_VERTICAL_HOVER_DISTANCE)
                {
                    _hoverDirection = Enums.Direction.UP;
                }
            }
            else if (!_isMovingFromAttack) //is on an edge
            {
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
                _hoverDirection = Enums.Direction.DOWN;
            }
            else
            {
                _hoverDirection = Enums.Direction.DOWN;
            }

            int lookVal = _random.Next(1, LOOK_CHANCE + 1);
            if (lookVal == LOOK_CHANCE)
            {
                this.Look();
                return;
            }

            //horizontal movement
            int xOffset = _direction == Enums.Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;
            int xPosCheck = _direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;

            Rectangle areaToCheck = new Rectangle(xPosCheck, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = _direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);

            if (tile != null)
            {
                int xCollidePos = _direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeHorizontalDirection(this.Direction);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            //vertical movement
            int yOffset = _hoverDirection == Enums.Direction.UP ? VERTICAL_HOVER_SPEED * -1 : VERTICAL_HOVER_SPEED;
            int yPosCheck = _hoverDirection == Enums.Direction.UP ? this.HitBox.Y + yOffset : this.HitBox.Y;
            Rectangle verticalAreaToCheck = new Rectangle(this.HitBox.X, yPosCheck, this.HitBox.Width, this.HitBox.Height + VERTICAL_HOVER_SPEED);

            var verticalTile = _hoverDirection == Enums.Direction.UP ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);
            if (verticalTile != null)
            {
                int yCollidePos = _hoverDirection == Enums.Direction.UP ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                this.HitBox = new Rectangle(this.HitBox.X, yCollidePos, this.HitBox.Width, this.HitBox.Height);
                _hoverDirection = this.ChangeVerticalDirection(_hoverDirection);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height);
            }

            this.UpdateSpriteByDelayBase(ref _currentLookSpriteChangeDelayTick, ref _currentLookSprite, LOOK_SPRITE_CHANGE_DELAY, UpdateSprite);

            if (IsKeenInAttackRange())
            {
                int attackVal = _random.Next(1, ATTACK_CHANCE + 1);
                if (attackVal == ATTACK_CHANCE)
                {
                    this.PrepareAttack();
                }
            }

        }

        private bool IsKeenInAttackRange()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X - HORIZONTAL_VISION_DISTANCE, this.HitBox.Y, this.HitBox.Width + (HORIZONTAL_VISION_DISTANCE * 2), this.HitBox.Height + VERTICAL_VISION_DISTANCE);
            bool inRange = _keen.HitBox.IntersectsWith(areaToCheck);
            return inRange;
        }

        protected override bool IsOnEdge(Direction directionToCheck, int edgeOffset = 0)
        {
            if (directionToCheck == Direction.LEFT)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.Left - this.HitBox.Width + edgeOffset, this.HitBox.Bottom, this.HitBox.Width, TOP_VERTICAL_HOVER_DISTANCE + 4);
                var tiles = this.CheckCollision(areaToCheck, true);
                return !tiles.Any();
            }
            else if (directionToCheck == Direction.RIGHT)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.Right - edgeOffset, this.HitBox.Bottom, this.HitBox.Width, TOP_VERTICAL_HOVER_DISTANCE + 4);
                var tiles = this.CheckCollision(areaToCheck, true);
                return !tiles.Any();
            }
            return false;
        }

        private void Look()
        {
            if (this.State != OrbatrixState.LOOKING)
            {
                this.State = OrbatrixState.LOOKING;

                if (_isMovingFromAttack)
                {
                    this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                    //check for initial collisions since the hitbox expands to the right
                    var initialCollisions = this.CheckCollision(this.HitBox, true);
                    //since the hitbox expands to the right, we will only collide right, so adjust hitbox accordingly
                    var collideTile = GetLeftMostRightTile(initialCollisions);
                    if (collideTile != null)
                    {
                        this.HitBox = new Rectangle(collideTile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                }
                _lookTimeTick = 0;
            }

            if (_lookTimeTick++ == LOOK_TIME)
            {
                this.Move();
            }
            else
            {
                this.UpdateSpriteByDelayBase(ref _currentLookSpriteChangeDelayTick, ref _currentLookSprite, LOOK_SPRITE_CHANGE_DELAY, UpdateSprite);
            }
        }

        public bool DeadlyTouch
        {
            get { return _state == OrbatrixState.ATTACKING; }
        }

        public void HandleHit(IProjectile projectile)
        {
            if (_state == OrbatrixState.PREPARING_ATTACK)
            {
                this.Look();
            }
            else if (_state == OrbatrixState.ATTACKING)
            {
                _horizontalVelocity = 0;
            }
        }

        public bool IsActive
        {
            get { return true; }
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

        OrbatrixState State
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

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image =>  _sprite;

        public Point Location => this.HitBox.Location;

        private void UpdateSprite()
        {
            switch (_state)
            {
                case OrbatrixState.LOOKING:
                    if (_currentLookSprite >= _lookSprites.Length)
                    {
                        _currentLookSprite = 0;
                    }
                    _sprite = _lookSprites[_currentLookSprite];
                    break;
                case OrbatrixState.MOVING:
                    var spriteSet = _direction == Enums.Direction.LEFT ? _lookLeftSprites : _lookRightSprites;
                    if (_currentLookSprite >= spriteSet.Length)
                    {
                        _currentLookSprite = 0;
                    }
                    _sprite = spriteSet[_currentLookSprite];
                    break;
                case OrbatrixState.PREPARING_ATTACK:
                case OrbatrixState.PREPARING_MOVE:
                    _sprite = Properties.Resources.keen6_orbatrix_close;
                    break;
                case OrbatrixState.ATTACKING:
                    if (_currentAttackSprite >= _attackSprites.Length)
                    {
                        _currentAttackSprite = 0;
                    }
                    _sprite = _attackSprites[_currentAttackSprite];
                    break;
                case OrbatrixState.ASCENDING:
                    _sprite = Properties.Resources.keen6_orbatrix_attack1;
                    break;
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_orbatrix_look_right1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum OrbatrixState
    {
        LOOKING,
        MOVING,
        PREPARING_ATTACK,
        ATTACKING,
        ASCENDING,
        PREPARING_MOVE
    }
}
