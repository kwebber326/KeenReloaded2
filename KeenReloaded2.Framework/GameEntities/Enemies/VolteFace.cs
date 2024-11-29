using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class VolteFace : CollisionObject, IUpdatable, ISprite, IEnemy, IStunnable
    {
        private const int STUN_TIME = 100;
        private int _stunTimeTick;

        private const int UPDATE_SPRITE_DELAY = 1;
        private int _updateSpriteDelayTick;

        private const int VELOCITY = 20;

        private bool _isStunned;
        private Image _sprite;
        private readonly int _zIndex;
        private List<Point> _locationNodes;
        private bool _movingForward = true;
        private int _currentNodeIndex;

        private int _currentMoveSprite;
        private Image[] _moveSprites = new Image[]
        {
            Properties.Resources.keen5_volte_face1,
            Properties.Resources.keen5_volte_face2,
            Properties.Resources.keen5_volte_face3,
            Properties.Resources.keen5_volte_face4
        };
        private bool _currentLocationReached;

        public VolteFace(Rectangle area, SpaceHashGrid grid, int zIndex, List<Point> locationNodes)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            _locationNodes = locationNodes != null && locationNodes.Any() 
                ? locationNodes 
                : new List<Point>() { area.Location };
            Initialize();
        }

        private void Initialize()
        {
            _sprite = _moveSprites[_currentMoveSprite];
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
                }
            }
        }

        public bool DeadlyTouch
        {
            get { return !_isStunned; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.Stun();
        }

        public bool IsActive
        {
            get { return !_isStunned; }
        }

        public void Stun()
        {
            if (!this._isStunned)
            {
                _isStunned = true;
                UpdateSprite();
                _stunTimeTick++;
            }
            else if (_stunTimeTick++ >= STUN_TIME)
            {
                _stunTimeTick = 0;
                _isStunned = false;
                _currentMoveSprite = 0;
                _updateSpriteDelayTick = UPDATE_SPRITE_DELAY;
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            if (_isStunned)
                _sprite = Properties.Resources.keen5_volte_face_stunned;
            else
            {
                if (_updateSpriteDelayTick++ == UPDATE_SPRITE_DELAY)
                {
                    _updateSpriteDelayTick = 0;
                    if (_currentMoveSprite >= _moveSprites.Length)
                    {
                        _currentMoveSprite = 0;
                    }
                    _sprite = _moveSprites[_currentMoveSprite++];
                }
            }

        }

        public bool IsStunned
        {
            get { return _isStunned; }
        }

        protected Enums.Direction Direction { get; set; }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public void Update()
        {
            if (!_isStunned)
            {
                this.Move();
            }
            else
            {
                this.Stun();
            }
        }

        private void Move()
        {
            //update sprite;
            UpdateSprite();
            //determine next node index to approach if we reached the goal node
            int nextIndex = GetNextLocationIndex();

            //set direction based on next node
            SetDirectionBasedOnNextNode(nextIndex);

            //get x and y offsets for collision detection based on direction
            int xOffset = 0, yOffset = 0, xPos = this.HitBox.X, yPos = this.HitBox.Y;
            bool isUpDirection = this.IsUpDirection(this.Direction),
                 isDownDirection = this.IsDownDirection(this.Direction),
                 isLeftDirection = this.IsLeftDirection(this.Direction),
                 isRightDirection = this.IsRightDirection(this.Direction);
            bool reachedXDest = false, reachedYDest = false;
            Point next = _locationNodes[nextIndex];
            if (isUpDirection)
            {
                if (this.HitBox.Y - next.Y > VELOCITY)
                    yOffset = VELOCITY * -1;
                else
                {
                    yOffset = (this.HitBox.Y - next.Y) * -1; reachedYDest = true;
                }
                yPos += yOffset;
            }
            else if (isDownDirection)
            {
                if (next.Y - this.HitBox.Y > VELOCITY)
                    yOffset = VELOCITY;
                else
                {
                    yOffset = next.Y - this.HitBox.Y; reachedYDest = true;
                }
            }
            else
            {
                reachedYDest = true;
            }

            if (isLeftDirection)
            {
                if (this.HitBox.X - next.X > VELOCITY)
                    xOffset = VELOCITY * -1;
                else
                {
                    xOffset = (this.HitBox.X - next.X) * -1; reachedXDest = true;
                }
                xPos += xOffset;
            }
            else if (isRightDirection)
            {
                if (next.X - this.HitBox.X > VELOCITY)
                    xOffset = VELOCITY;
                else
                {
                    xOffset = next.X - this.HitBox.X; reachedXDest = true;
                }
            }
            else
            {
                reachedXDest = true;
            }
            //check collision
            Rectangle areaToCheck = new Rectangle(xPos, yPos, this.HitBox.Width + Math.Abs(xOffset), this.HitBox.Height + Math.Abs(yOffset));
            var collisions = this.CheckCollision(areaToCheck, true);
            //get collision tiles
            CollisionObject verticalTile = null, horizontalTile = null;
            if (isUpDirection)
            {
                verticalTile = GetCeilingTile(collisions);
            }
            else if (isDownDirection)
            {
                verticalTile = GetTopMostLandingTile(collisions);
            }

            if (isLeftDirection)
            {
                horizontalTile = GetRightMostLeftTile(collisions);
            }
            else if (isRightDirection)
            {
                horizontalTile = GetLeftMostRightTile(collisions);
            }
            //get moving ability
            if (verticalTile == null && horizontalTile == null)
            {
                this.KillCollidingPlayers(areaToCheck);
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                _movingForward = !_movingForward;
                _currentLocationReached = true;
                if (verticalTile != null)
                {
                    yPos = isUpDirection ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                }
                else
                {
                    yPos = this.HitBox.Y + yOffset;
                }
                if (horizontalTile != null)
                {
                    xPos = isLeftDirection ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                }
                else
                {
                    xPos = this.HitBox.X + xOffset;
                }
                //Rectangle killArea = new Rectangle(
                //    isLeftDirection ? this.HitBox.X + xOffset : this.HitBox.X, //x
                //    isUpDirection ? this.HitBox.Y + yOffset : this.HitBox.Y, //y
                //    this.HitBox.Width + Math.Abs(xOffset), //width
                //    this.HitBox.Height + Math.Abs(yOffset));//height
                //this.KillCollidingPlayers(killArea);
                this.HitBox = new Rectangle(xPos, yPos, this.HitBox.Width, this.HitBox.Height);
                this.KillCollidingPlayers();
            }

            //register to get next node if we reached the current node
            if (reachedXDest && reachedYDest)
            {
                _currentLocationReached = true;
            }
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile;
            var landingTiles = collisions.Where(h => !(h.CollisionType == CollisionType.PLATFORM)
                && h.HitBox.Top >= this.HitBox.Top
                && h.HitBox.Right >= this.HitBox.Left
                && h.HitBox.Left <= this.HitBox.Right);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => !(c.CollisionType == CollisionType.PLATFORM)
                && c.HitBox.Bottom <= this.HitBox.Top
                && c.HitBox.Right >= this.HitBox.Left
                && c.HitBox.Left <= this.HitBox.Right).ToList();
            if (debugTiles.Any())
            {
                int maxBottom = debugTiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        private int GetNextLocationIndex()
        {
            int nextIndex = _currentNodeIndex;
            if (_currentLocationReached)
            {
                _currentLocationReached = false;

                if (_movingForward)
                {
                    if (_currentNodeIndex == _locationNodes.Count - 1)
                    {
                        _currentNodeIndex = 0;
                        nextIndex = 0;
                    }
                    else
                    {
                        nextIndex = ++_currentNodeIndex;
                    }
                }
                else
                {
                    if (_currentNodeIndex == 0)
                    {
                        _currentNodeIndex = _locationNodes.Count - 1;
                        nextIndex = _currentNodeIndex;
                    }
                    else
                    {
                        nextIndex = --_currentNodeIndex;
                    }
                }
            }
            return nextIndex;
        }

        private void SetDirectionBasedOnNextNode(int nextIndex)
        {
            Point current = this.HitBox.Location;
            Point next = _locationNodes[nextIndex];

            int xVal = 0, yVal = 0;
            if (current.X > next.X)
            {
                xVal = -1;
            }
            else if (current.X < next.X)
            {
                xVal = 1;
            }
            if (current.Y > next.Y)
            {
                yVal = -1;
                switch (xVal)
                {
                    case 0:
                        this.Direction = Enums.Direction.UP;
                        break;
                    case 1:
                        this.Direction = Enums.Direction.UP_RIGHT;
                        break;
                    case -1:
                        this.Direction = Enums.Direction.UP_LEFT;
                        break;
                }
            }
            else if (current.Y < next.Y)
            {
                yVal = 1;
                switch (xVal)
                {
                    case 0:
                        this.Direction = Enums.Direction.DOWN;
                        break;
                    case 1:
                        this.Direction = Enums.Direction.DOWN_RIGHT;
                        break;
                    case -1:
                        this.Direction = Enums.Direction.DOWN_LEFT;
                        break;
                }
            }
            else
            {
                switch (xVal)
                {
                    case 0:
                        _currentLocationReached = true;
                        break;
                    case 1:
                        this.Direction = Enums.Direction.RIGHT;
                        break;
                    case -1:
                        this.Direction = Enums.Direction.LEFT;
                        break;
                }
            }
        }

        public override string ToString()
        {
            string pathArray = MapMakerConstants.MAP_MAKER_ARRAY_START;
            foreach (var node in _locationNodes)
            {
                string item = node.X + MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR + node.Y + (node == _locationNodes.Last() 
                    ? "" 
                    : MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR);
                pathArray += item;
            }
            pathArray += MapMakerConstants.MAP_MAKER_ARRAY_END;

            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_volte_face1);

            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{pathArray}";
        }
    }
}
