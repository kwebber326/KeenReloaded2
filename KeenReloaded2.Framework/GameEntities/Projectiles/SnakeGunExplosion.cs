using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class SnakeGunExplosion : RPGExplosion
    {
        private const int MAX_EXPLOSIIONS = 10;

        private readonly SnakeGunShot _callingObject;
        private readonly Rectangle _lastCollision;
        private readonly int _currentExplosionNum;
        private Direction _direction;
        private CollisionObject _borderTile;
        private readonly Guid _objectId = Guid.NewGuid();

        public SnakeGunExplosion(SpaceHashGrid grid, Rectangle hitbox, int blastRadius, int damage, SnakeGunShot callingObject, Rectangle lastCollision, Direction direction, int currentExplosionNum = 1, SnakeGunExplosion previous = null, CollisionObject borderObject = null) : base(grid, hitbox, blastRadius, damage, direction)
        {
            _callingObject = callingObject;
            _lastCollision = lastCollision;
            _borderTile = borderObject;
            this.Previous = previous;
            if (_callingObject == null)
            {
                throw new ArgumentNullException("must pass projectile object that caused this explosion");
            }
            if (currentExplosionNum < 1)
                currentExplosionNum = 1;

            _currentExplosionNum = currentExplosionNum;
            _direction = direction;

            if (_currentExplosionNum > 1)
            {
                _callingObject.RegisterExplosionEvents(this);
            }
            SPRITE_CHANGE_DELAY = 1;
            this.Explode();
        }

        private void SetInitialHitbox()
        {
            switch (_direction)
            {
                case Direction.DOWN:
                    int collisionBottom = _callingObject.HitBox.Bottom;
                    int centerOfHorizontalCollision = _callingObject.HitBox.Left + (_callingObject.HitBox.Width / 2);
                    int x = centerOfHorizontalCollision - (_blastRadius / 2);
                    int y = collisionBottom - _blastRadius;
                    this.HitBox = new Rectangle(x, y, _blastRadius, _blastRadius);
                    break;
                case Direction.UP:
                    int collisionTop = _callingObject.HitBox.Top;
                    centerOfHorizontalCollision = _callingObject.HitBox.Left + (_callingObject.HitBox.Width / 2);
                    x = centerOfHorizontalCollision - (_blastRadius / 2);
                    y = collisionTop;
                    this.HitBox = new Rectangle(x, y, _blastRadius, _blastRadius);
                    break;
                case Direction.LEFT:
                    int collisionLeft = _callingObject.HitBox.Left;
                    int centerOfVerticalCollision = _callingObject.HitBox.Top + (_callingObject.HitBox.Height / 2);
                    x = collisionLeft;
                    y = centerOfVerticalCollision - (_blastRadius / 2);
                    this.HitBox = new Rectangle(x, y, _blastRadius, _blastRadius);
                    break;
                case Direction.RIGHT:
                    int collisionRight = _callingObject.HitBox.Right;
                    centerOfVerticalCollision = _callingObject.HitBox.Top + (_callingObject.HitBox.Height / 2);
                    x = collisionRight - _blastRadius;
                    y = centerOfVerticalCollision - (_blastRadius / 2);
                    this.HitBox = new Rectangle(x, y, _blastRadius, _blastRadius);
                    break;
            }

            UpdateHitboxForExplosionCollisionsInitialChain();
        }

        private void SetSubsequentHitboxes()
        {
            SetHitboxBasedOnDirectionSubsequentChains();
            UpdateHitboxForExplosionCollisionsSubsequentChains();
        }

        private void SetHitboxBasedOnDirectionSubsequentChains()
        {
            if (_borderTile == null)
                return;

            switch (_direction)
            {
                case Direction.DOWN:
                    int collisionBottom = _lastCollision.Bottom;
                    int x = _borderTile.HitBox.Right < this.HitBox.Left ? _borderTile.HitBox.Right + 1 : _borderTile.HitBox.Left - _blastRadius - 1;//_lastCollision.X;
                    int y = collisionBottom + 1;
                    this.HitBox = new Rectangle(x, y, _blastRadius, _blastRadius);
                    break;
                case Direction.UP:
                    int collisionTop = _lastCollision.Top;
                    x = _borderTile.HitBox.Right < this.HitBox.Left ? _borderTile.HitBox.Right + 1 : _borderTile.HitBox.Left - _blastRadius - 1; //_lastCollision.X;
                    y = collisionTop - _blastRadius - 1;
                    this.HitBox = new Rectangle(x, y, _blastRadius, _blastRadius);
                    break;
                case Direction.LEFT:
                    int collisionLeft = _lastCollision.Left;
                    x = collisionLeft - _blastRadius - 1;
                    y = _borderTile.HitBox.Top > this.HitBox.Bottom ? _borderTile.HitBox.Top - _blastRadius - 1 : _borderTile.HitBox.Bottom + 1;
                    this.HitBox = new Rectangle(x, y, _blastRadius, _blastRadius);
                    break;
                case Direction.RIGHT:
                    int collisionRight = _lastCollision.Right;
                    x = collisionRight + 1;
                    y = _borderTile.HitBox.Top > this.HitBox.Bottom ? _borderTile.HitBox.Top - _blastRadius - 1 : _borderTile.HitBox.Bottom + 1;
                    this.HitBox = new Rectangle(x, y, _blastRadius, _blastRadius);
                    break;
            }
        }

        private void UpdateHitboxForExplosionCollisionsSubsequentChains()
        {
            var collisions = this.CheckCollision(this.HitBox);
            _upTile = (!this.IsVerticalDirection(_direction) && _currentExplosionNum > 1) ? null : this.GetCeilingTileForSubsequentExplosions(collisions);
            _downTile = (!this.IsVerticalDirection(_direction) && _currentExplosionNum > 1) ? null : this.GetTopMostLandingTile(collisions);
            _leftTile = (!this.IsHorizontalDirection(_direction) && _currentExplosionNum > 1) ? null : this.GetRightMostLeftTileSubsequentExplosions(collisions);
            _rightTile = (!this.IsHorizontalDirection(_direction) && _currentExplosionNum > 1) ? null : this.GetLeftMostRightTileSubsequentExplosions(collisions);
            switch (_direction)
            {
                case Direction.DOWN:
                    int xPos = this.HitBox.X;
                    int yPos = this.Previous.HitBox.Bottom + 1;
                    int width = this.HitBox.Width;
                    int height = this.HitBox.Height;
                    if (_downTile != null)
                    {
                        _borderTile = _downTile;
                        yPos = _borderTile.HitBox.Y - _blastRadius - 1;
                        this.HitBox = new Rectangle(xPos, yPos, width, height);
                        if (IsWallLleft(false))
                        {
                            _direction = this.TurnLeft(_direction);
                        }
                        else
                        {
                            _direction = this.TurnRight(_direction);
                        }
                    }
                    else
                    {
                        this.HitBox = new Rectangle(xPos, yPos, width, height);
                        if (IsWallLleft(true, out CollisionObject previousTile) && !IsWallLleft(false))
                        {
                            _borderTile = previousTile;
                            _direction = this.TurnRight(_direction);
                        }
                        else if (IsWallLRight(true, out previousTile) && !IsWallLRight(false))
                        {
                            _borderTile = previousTile;
                            _direction = this.TurnLeft(_direction);
                        }
                    }


                    break;
                case Direction.UP:
                    xPos = this.HitBox.X;
                    yPos = this.Previous.HitBox.Top - this.HitBox.Height - 1;
                    width = this.HitBox.Width;
                    height = this.HitBox.Height;
                    if (_upTile != null)
                    {
                        _borderTile = _upTile;

                        yPos = _upTile.HitBox.Bottom + 1;
                        this.HitBox = new Rectangle(xPos, yPos, width, height);
                        if (IsWallLleft(false))
                        {
                            _direction = this.TurnRight(_direction);
                        }
                        else if (IsWallLRight(false))
                        {
                            _direction = this.TurnLeft(_direction);
                        }
                    }
                    else
                    {
                        this.HitBox = new Rectangle(xPos, yPos, width, height);
                        if (IsWallLleft(true, out CollisionObject previousTile) && !IsWallLleft(false))
                        {
                            _borderTile = previousTile;
                            _direction = this.TurnLeft(_direction);
                        }
                        else if (IsWallLRight(true, out previousTile) && !IsWallLRight(false))
                        {
                            _borderTile = previousTile;
                            _direction = this.TurnRight(_direction);
                        }
                    }
                    break;
                case Direction.RIGHT:
                    xPos = this.Previous.HitBox.Right + 1;
                    yPos = this.HitBox.Y;
                    width = this.HitBox.Width;
                    height = this.HitBox.Height;
                    if (_rightTile != null)
                    {
                        _borderTile = _rightTile;
                        xPos = _rightTile.HitBox.Left - _blastRadius - 1;
                        this.HitBox = new Rectangle(xPos, yPos, width, height);

                        if (IsWallLUp(false))
                        {
                            _direction = this.TurnRight(_direction);
                        }
                        else if (IsWallLDown(false))
                        {
                            _direction = this.TurnLeft(_direction);
                        }
                    }
                    else
                    {
                        this.HitBox = new Rectangle(xPos, yPos, width, height);

                        if (this.IsWallLUp(true, out CollisionObject previousTile) && !this.IsWallLUp(false))
                        {
                            _borderTile = previousTile;
                            _direction = this.TurnLeft(_direction);
                        }
                        else if (this.IsWallLDown(true, out previousTile) && !this.IsWallLDown(false))
                        {
                            _borderTile = previousTile;
                            _direction = this.TurnRight(_direction);
                        }
                    }

                    break;
                case Direction.LEFT:
                    xPos = this.Previous.HitBox.Left - this.HitBox.Width - 1;
                    yPos = _borderTile.HitBox.Top > this.HitBox.Bottom ? _borderTile.HitBox.Top - _blastRadius - 1 : _borderTile.HitBox.Bottom + 1;  //this.HitBox.Y;
                    width = this.HitBox.Width;
                    height = this.HitBox.Height;
                    if (_leftTile != null)
                    {
                        _borderTile = _leftTile;

                        xPos = _leftTile.HitBox.Right + 1;
                        this.HitBox = new Rectangle(xPos, yPos, width, height);

                        if (IsWallLUp(false))
                        {
                            _direction = this.TurnLeft(_direction);
                        }
                        else if (IsWallLDown(false))
                        {
                            _direction = this.TurnRight(_direction);
                        }
                    }
                    else
                    {
                        this.HitBox = new Rectangle(xPos, yPos, width, height);

                        if (IsWallLDown(true, out CollisionObject previousTile) && !IsWallLDown(false))
                        {
                            _borderTile = previousTile;
                            _direction = this.TurnLeft(_direction);
                        }
                        else if (IsWallLUp(true, out previousTile) && !IsWallLUp(false))
                        {
                            _borderTile = previousTile;
                            _direction = this.TurnRight(_direction);
                        }
                    }

                    break;
            }

        }

        private CollisionObject GetRightMostLeftTileSubsequentExplosions(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.OfType<MaskedTile>();
            if (collisions.Any() && debugTiles.Any())
            {
                var leftTiles = debugTiles.Where(c => c.HitBox.Right <= this.HitBox.Right && c.HitBox.Right >= this.HitBox.Left && c.HitBox.Top < this.HitBox.Bottom && c.HitBox.Bottom > this.HitBox.Top).ToList();
                if (leftTiles.Any())
                {
                    int maxX = leftTiles.Select(t => t.HitBox.Right).Max();
                    CollisionObject obj = leftTiles.FirstOrDefault(x => x.HitBox.Right == maxX);
                    return obj;
                }
            }
            return null;
        }

        private CollisionObject GetLeftMostRightTileSubsequentExplosions(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.OfType<MaskedTile>();
            if (collisions.Any() && debugTiles.Any())
            {
                var rightTiles = debugTiles.Where(c => c.HitBox.Right >= this.HitBox.Right && c.HitBox.Left <= this.HitBox.Right && c.HitBox.Top < this.HitBox.Bottom && c.HitBox.Bottom > this.HitBox.Top).ToList();
                if (rightTiles.Any())
                {
                    int minX = rightTiles.Select(t => t.HitBox.Left).Min();
                    CollisionObject obj = rightTiles.FirstOrDefault(x => x.HitBox.Left == minX);
                    return obj;
                }
            }
            return null;
        }

        private CollisionObject GetCeilingTileForSubsequentExplosions(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK && c.HitBox.Bottom <= this.HitBox.Top).ToList();
            if (debugTiles.Any())
            {
                int maxBottom = debugTiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        private void UpdateHitboxForExplosionCollisionsInitialChain()
        {
            var collisions = this.CheckCollision(this.HitBox);
            _upTile = (_direction == Direction.UP && _currentExplosionNum == 1) ? null : this.GetCeilingTile(collisions);
            _downTile = _direction == Direction.DOWN && _currentExplosionNum == 1 ? null : this.GetTopMostLandingTile(collisions);
            _leftTile = _direction == Direction.LEFT && _currentExplosionNum == 1 ? null : this.GetRightMostLeftTile(collisions);
            _rightTile = _direction == Direction.RIGHT && _currentExplosionNum == 1 ? null : this.GetLeftMostRightTile(collisions);
            int xPos = this.HitBox.X, yPos = this.HitBox.Y, width = this.HitBox.Width, height = this.HitBox.Height;
            //reduce hitbox if it collides with tiles
            if (_downTile != null)
            {
                if (_direction == Direction.DOWN)
                {
                    _borderTile = _downTile;
                }
                yPos = _downTile.HitBox.Top - height - 1;
                if (yPos < this.HitBox.Y)
                {
                    height = height - (this.HitBox.Y - yPos);
                }
            }
            if (_upTile != null && _upTile != _downTile)
            {
                if (_direction == Direction.UP)
                {
                    _borderTile = _upTile;
                }
                yPos = _upTile.HitBox.Bottom + 1;
                if (yPos > this.HitBox.Y)
                {
                    height = height - (yPos - this.HitBox.Y);
                }
                if (_downTile != null)
                {
                    height = (_downTile.HitBox.Top - 1) - (_upTile.HitBox.Bottom + 1);//if we are squashed verically, we set the height to the space in between
                    if (height < 0)
                        height = 0;
                }
            }
            if (_rightTile != null)
            {
                if (_direction == Direction.RIGHT)
                {
                    _borderTile = _rightTile;
                }
                xPos = _rightTile.HitBox.Left - width - 1;
                if (xPos < this.HitBox.X)
                {
                    width = width - (this.HitBox.X - xPos);
                }
            }
            if (_leftTile != null)
            {
                if (_direction == Direction.LEFT)
                {
                    _borderTile = _leftTile;
                }
                xPos = _leftTile.HitBox.Right + 1;
                if (xPos > this.HitBox.X)
                {
                    width = width - (xPos - this.HitBox.X);
                }
                if (_rightTile != null)
                {
                    width = (_rightTile.HitBox.Left - 1) - (_leftTile.HitBox.Right + 1);//if we are squashed horizontally, we set the space in between
                    if (width < 0)
                        width = 0;
                }
            }
            this.HitBox = new Rectangle(xPos, yPos, width, height);
        }

        public override void Explode()
        {
            if (this.ExplosionState != Enums.ExplosionState.EXPLODING && _callingObject != null)
            {
                this.ExplosionState = Enums.ExplosionState.EXPLODING;
                if (_currentExplosionNum == 1)
                {
                    SetInitialHitbox();
                }
                else
                {
                    SetSubsequentHitboxes();
                }
                HandleCurrentCollisions();
            }

            if (_currentImage == _explosionImages.Length / 2 && _currentExplosionNum < MAX_EXPLOSIIONS)
            {
                CreateCascadingExplosions();
            }

            UpdateSprite();
        }

        private void CreateCascadingExplosions()
        {
            if (_currentExplosionNum == 1)
            {
                //first explosion creates two neighboring explosions perpendicular to the projectile that initiated the explosion
                BeginExplosionChain();
            }
            else
            {
                //subsequent chain explosions will move along the border of the pathway the initial explosion set
                //until the maximum explosions are reached
                ContinueExplosionChangeInDirection();
            }
        }

        private void BeginExplosionChain()
        {
            if (this.IsVerticalDirection(_direction))
            {
                //create the chain horizontally if the projectile hit from a vertical trajectory
                int leftX = this.HitBox.Left - _blastRadius - 1;
                int rightX = this.HitBox.Right + 1;

                if (_direction == Direction.UP && IsWallLUp(false, out CollisionObject tile))
                {
                    _borderTile = tile;
                }
                else if (_direction == Direction.DOWN && IsWallLDown(false, out CollisionObject tileDown))
                {
                    _borderTile = tileDown;
                }

                if (_borderTile != null)
                {
                    int y = _direction == Direction.UP ? _borderTile.HitBox.Bottom + 1 : _borderTile.HitBox.Top - _blastRadius - 1;

                    Rectangle newLastCollision = new Rectangle(this.HitBox.Location, this.HitBox.Size);
                    //left explosion neighbor
                    Rectangle initialCollisionAreaLeftExplosion = new Rectangle(leftX, y, _blastRadius, _blastRadius);
                    SnakeGunExplosion explosionLeft = new SnakeGunExplosion(_collisionGrid, initialCollisionAreaLeftExplosion, _blastRadius, _damage, _callingObject, newLastCollision, Direction.LEFT, _currentExplosionNum + 1, this, _borderTile);
                    //right explosion neighbor
                    Rectangle initialCollisionAreaRightExplosion = new Rectangle(rightX, y, _blastRadius, _blastRadius);
                    SnakeGunExplosion explosionRight = new SnakeGunExplosion(_collisionGrid, initialCollisionAreaRightExplosion, _blastRadius, _damage, _callingObject, newLastCollision, Direction.RIGHT, _currentExplosionNum + 1, this, _borderTile);
                }
            }
            else
            {
                //create the chain vertically if the projectile hit from a horizontal trajectory
                int upY = this.HitBox.Top - _blastRadius - 1;
                int downY = this.HitBox.Bottom + 1;

                if (_direction == Direction.RIGHT && IsWallLRight(false, out CollisionObject tile))
                {
                    _borderTile = tile;
                }
                else if (_direction == Direction.LEFT && IsWallLleft(false, out CollisionObject tileLeft))
                {
                    _borderTile = tileLeft;
                }

                if (_borderTile != null)
                {
                    Rectangle newLastCollision = new Rectangle(this.HitBox.Location, this.HitBox.Size);

                    //up explosion neighbor
                    Rectangle initialCollisionAreaUpExplosion = new Rectangle(_lastCollision.X, upY, _blastRadius, _blastRadius);
                    SnakeGunExplosion explosionUp = new SnakeGunExplosion(_collisionGrid, initialCollisionAreaUpExplosion, _blastRadius, _damage, _callingObject, newLastCollision, Direction.UP, _currentExplosionNum + 1, this, _borderTile);

                    //down explosion neighbor
                    Rectangle initialCollisionAreaDownExplosion = new Rectangle(_lastCollision.X, downY, _blastRadius, _blastRadius);
                    SnakeGunExplosion explosionDown = new SnakeGunExplosion(_collisionGrid, initialCollisionAreaDownExplosion, _blastRadius, _damage, _callingObject, newLastCollision, Direction.DOWN, _currentExplosionNum + 1, this, _borderTile);
                }
            }
        }


        private void ContinueExplosionChangeInDirection()
        {
            //continue explosion direction
            int x = this.HitBox.X, y = this.HitBox.Y;
            switch (_direction)
            {
                case Direction.UP:
                    y = this.HitBox.Top - _blastRadius - 1;
                    break;
                case Direction.DOWN:
                    y = this.HitBox.Bottom + 1;
                    break;
                case Direction.LEFT:
                    x = this.HitBox.Left - _blastRadius - 1;
                    break;
                case Direction.RIGHT:
                    x = this.HitBox.Right + 1;
                    break;
            }

            Rectangle newLastCollision = new Rectangle(this.HitBox.Location, this.HitBox.Size);
            Rectangle initialCollisionAreaUpExplosion = new Rectangle(x, y, _blastRadius, _blastRadius);
            SnakeGunExplosion explosion = new SnakeGunExplosion(_collisionGrid, initialCollisionAreaUpExplosion, _blastRadius, _damage, _callingObject, newLastCollision, _direction, _currentExplosionNum + 1, this, _borderTile);
        }

        private void HandleCurrentCollisions()
        {
            var collisions = this.CheckCollision(this.HitBox);
            foreach (var collision in collisions)
            {
                this.HandleCollision(collision);
            }
        }

        protected override void UpdateSprite()
        {
            if (++_currentImage >= _explosionImages.Length)
            {
                this.EndExplosion();
            }
            else
            {
                _sprite = BitMapTool.DrawImageAtLocationWithDimensions(_explosionImages[_currentImage], this.HitBox);
            }
        }

        private bool IsWallLleft(bool checkPrevious)
        {

            Rectangle hitbox = checkPrevious && this.Previous != null ? this.Previous.HitBox : this.HitBox;
            Rectangle areaToCheck = new Rectangle(hitbox.X - 2, hitbox.Y, hitbox.Width, hitbox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            return collisions.Any();
        }

        private bool IsWallLleft(bool checkPrevious, out CollisionObject tile)
        {

            Rectangle hitbox = checkPrevious && this.Previous != null ? this.Previous.HitBox : this.HitBox;
            Rectangle areaToCheck = new Rectangle(hitbox.X - 2, hitbox.Y, hitbox.Width, hitbox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            tile = collisions.OrderByDescending(c => c.HitBox.Right).FirstOrDefault();  //this.GetRightMostLeftTile(collisions);
            return collisions.Any();
        }

        private bool IsWallLRight(bool checkPrevious)
        {
            Rectangle hitbox = checkPrevious && this.Previous != null ? this.Previous.HitBox : this.HitBox;
            Rectangle areaToCheck = new Rectangle(hitbox.X, hitbox.Y, hitbox.Width + 2, hitbox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            return collisions.Any();
        }

        private bool IsWallLRight(bool checkPrevious, out CollisionObject tile)
        {
            Rectangle hitbox = checkPrevious && this.Previous != null ? this.Previous.HitBox : this.HitBox;
            Rectangle areaToCheck = new Rectangle(hitbox.X, hitbox.Y, hitbox.Width + 2, hitbox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            tile = collisions.OrderBy(c => c.HitBox.Left).FirstOrDefault();//this.GetLeftMostRightTile(collisions);
            return collisions.Any();
        }

        private bool IsWallLUp(bool checkPrevious)
        {
            Rectangle hitbox = checkPrevious && this.Previous != null ? this.Previous.HitBox : this.HitBox;
            Rectangle areaToCheck = new Rectangle(hitbox.X, hitbox.Y - 2, hitbox.Width, hitbox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            return collisions.Any();
        }

        private bool IsWallLUp(bool checkPrevious, out CollisionObject tile)
        {
            Rectangle hitbox = checkPrevious && this.Previous != null ? this.Previous.HitBox : this.HitBox;
            Rectangle areaToCheck = new Rectangle(hitbox.X, hitbox.Y - 2, hitbox.Width, hitbox.Height);
            var collisions = this.CheckCollision(areaToCheck, true);
            tile = collisions?.OrderByDescending(c => c.HitBox.Bottom).FirstOrDefault();
            return collisions.Any();
        }

        private bool IsWallLDown(bool checkPrevious)
        {
            Rectangle hitbox = checkPrevious && this.Previous != null ? this.Previous.HitBox : this.HitBox;
            Rectangle areaToCheck = new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height + 2);
            var collisions = this.CheckCollision(areaToCheck, true);
            return collisions.Any();
        }

        private bool IsWallLDown(bool checkPrevious, out CollisionObject tile)
        {
            Rectangle hitbox = checkPrevious && this.Previous != null ? this.Previous.HitBox : this.HitBox;
            Rectangle areaToCheck = new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height + 2);
            var collisions = this.CheckCollision(areaToCheck, true);
            tile = collisions.Where(h => h.HitBox.Top >= this.HitBox.Top).OrderBy(c => c.HitBox.Top).FirstOrDefault();
            return collisions.Any();
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => (c.CollisionType == CollisionType.BLOCK || c.CollisionType == CollisionType.PLATFORM)
                    && c.HitBox.Bottom >= this.HitBox.Top && c.HitBox.Bottom <= this.HitBox.Bottom /*&& c.HitBox.Top < this.HitBox.Top*/).ToList();
            if (debugTiles.Any())
            {
                int maxBottom = debugTiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = debugTiles.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        public new event EventHandler<ObjectEventArgs> Create;

        public new event EventHandler<ObjectEventArgs> Remove;

        public CollisionObject UpTile
        {
            get
            {
                return _upTile;
            }
        }

        public CollisionObject DownTile
        {
            get
            {
                return _downTile;
            }
        }
        public CollisionObject LeftTile
        {
            get
            {
                return _upTile;
            }
        }
        public CollisionObject RightTile
        {
            get
            {
                return _rightTile;
            }
        }

        public SnakeGunExplosion Previous
        {
            get; private set;
        }

        protected void OnCreate(object sender, ObjectEventArgs e)
        {
            this.Create?.Invoke(this, e);
        }
        protected void OnRemove(object sender, ObjectEventArgs e)
        {
            this.Remove?.Invoke(this, e);
        }

        protected override void OnCreate()
        {
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = this
            };
            if (this.Create != null)
            {
                this.Create(this, e);
            }
        }
        protected override void OnRemove()
        {
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = this
            };

            foreach (var node in _collidingNodes)
            {
                node.Objects.Remove(this);
                node.NonEnemies.Remove(this);
            }

            if (this.Remove != null)
            {
                this.Remove(this, e);
            }
        }
    }
}
