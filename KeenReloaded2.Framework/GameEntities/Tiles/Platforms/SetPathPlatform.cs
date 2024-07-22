using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Players;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class SetPathPlatform : Platform
    {
        private bool _isActive;
        private int _currentSprite;
        private bool _isKeenStandingOnPlatform;
        private bool _wasStandingOnPlatform;
        List<Point> _pathwayPoints;
        int _currentPathwayPointIndex = 1;
        int _horizontalMoveKeenVal = 0;
        private bool _currentLocationReached;

        public SetPathPlatform(SpaceHashGrid grid, Rectangle hitbox, PlatformType type, CommanderKeen keen, List<Point> locations, Guid activationId, bool initiallyActive = false)
            : base(grid, hitbox, type, keen, activationId)
        {
            if (locations == null)
                throw new ArgumentNullException("Pathway Platforms need a pathway to move");

            _pathwayPoints = locations;
            _isActive = initiallyActive;
            if (_pathwayPoints.Any())
            {
                _pathwayPoints.Insert(0, this.HitBox.Location);
                SetDirectionBasedOnNextNode(_currentPathwayPointIndex);
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
                if (value != null)
                {
                    this.UpdateCollisionNodes(_direction);
                }
            }
        }

        public override void Activate()
        {
            _isActive = true;
        }

        public override void Deactivate()
        {
            _isActive = false;
        }


        protected override bool IsHorizontalDirection(Direction direction)
        {
            return direction == Direction.LEFT || direction == Direction.RIGHT || IsDiagonalDirection(direction);
        }

        protected override bool IsVerticalDirection(Direction direction)
        {
            return direction == Direction.UP || direction == Direction.DOWN || IsDiagonalDirection(direction);
        }

        public Direction Direction
        {
            get
            {
                return _direction;
            }
        }

        public override CollisionType CollisionType => CollisionType.PLATFORM;

        protected override void UpdateKeenHorizontalPosition()
        {
            if (!_keen.IsDead())
                _keen.GetMovedHorizontally(this, _direction, _horizontalMoveKeenVal);
        }

        public override void Update()
        {
            if (!_pathwayPoints.Any())
                return;

            if (_isActive)
            {
                _isKeenStandingOnPlatform = IsKeenStandingOnPlatform();
                int yOffset = _direction == Direction.UP ? _moveVelocity * -1 : _moveVelocity;
                int yLocation = _direction == Direction.UP ? this.HitBox.Y + yOffset : this.HitBox.Y;
                int originalX = this.HitBox.X;
                Move();

                if (_keen.IsLookingDown && _wasStandingOnPlatform)
                {
                    this.UpdateKeenVerticalPosition();
                    if (this.HitBox.X != originalX)
                        this.UpdateKeenHorizontalPosition();
                }
                else
                {
                    if (_isKeenStandingOnPlatform)
                    {
                        UpdateKeenPosition(originalX);
                    }
                    else if (this.IsUpDirection(_direction))
                    {
                        Rectangle r = new Rectangle(this.HitBox.X, this.HitBox.Y - _moveVelocity, this.HitBox.Width, this.HitBox.Height + _moveVelocity);
                        if ((_keen.MoveState == MoveState.STANDING || _keen.MoveState == MoveState.RUNNING || _keen.MoveState == MoveState.FALLING) &&
                            _keen.HitBox.IntersectsWith(r) && _keen.HitBox.Bottom < this.HitBox.Bottom && !_keen.IsDead())
                        {
                            UpdateKeenPosition(originalX);
                        }
                        else
                        {
                            _wasStandingOnPlatform = false;
                        }
                    }
                    else
                    {
                        _wasStandingOnPlatform = false;
                    }
                }
            }
        }

        private void UpdateKeenPosition(int originalX)
        {
            this.UpdateKeenVerticalPosition();
            if (this.HitBox.X != originalX)
                this.UpdateKeenHorizontalPosition();
            _wasStandingOnPlatform = true;
        }

        private void Move()
        {
            _horizontalMoveKeenVal = 0;
            if (_currentLocationReached)
            {
                _currentLocationReached = false;
                //determine next node index to approach if we reached the goal node
                int nextIndex = _currentPathwayPointIndex == _pathwayPoints.Count - 1 ? 0 : _currentPathwayPointIndex + 1;
                //set direction based on next node
                SetDirectionBasedOnNextNode(nextIndex);
                _currentPathwayPointIndex = nextIndex;
            }
            else
            {
                SetDirectionBasedOnNextNode(_currentPathwayPointIndex);
            }

            int xOffset = 0, yOffset = 0;
            int xPosCheck = this.HitBox.X, yPosCheck = this.HitBox.Y;
            int previousX = this.HitBox.X, previousY = this.HitBox.Y;

            bool isHorizontal = IsHorizontalDirection(_direction);
            bool isVertical = IsVerticalDirection(_direction);
            bool isLeftDirection = IsLeftDirection(_direction);
            bool isUpDirection = IsUpDirection(_direction);
            if (isHorizontal)
            {
                xOffset = isLeftDirection ? _moveVelocity * -1 : _moveVelocity;
                if (isLeftDirection)
                {
                    xPosCheck = this.HitBox.X + xOffset;
                }
            }

            if (isVertical)
            {

                yOffset = isUpDirection ? _moveVelocity * -1 : _moveVelocity;
                if (isUpDirection)
                {
                    yPosCheck = this.HitBox.Y + yOffset;
                }
            }
            Rectangle areaToCheck = new Rectangle(xPosCheck, yPosCheck, this.HitBox.Width + Math.Abs(xOffset), this.HitBox.Height + Math.Abs(yOffset));
            var collisions = this.CheckCollision(areaToCheck, true);

            CollisionObject verticalTile = null;
            CollisionObject horizontalTile = null;
            if (isVertical)
            {
                verticalTile = isUpDirection ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);
            }

            if (isHorizontal)
            {
                horizontalTile = isLeftDirection ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            }

            int xDistanceToNextPoint = Math.Abs(this.HitBox.X - _pathwayPoints[_currentPathwayPointIndex].X);
            int yDistanceToNextPoint = Math.Abs(this.HitBox.Y - _pathwayPoints[_currentPathwayPointIndex].Y);
            int nextXPoint = this.HitBox.X, nextYPoint = this.HitBox.Y;
            if (yOffset != 0)
            {
                if (verticalTile == null)
                {
                    if (Math.Abs(yOffset) <= yDistanceToNextPoint)
                    {
                        this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + yOffset, this.HitBox.Width, this.HitBox.Height);
                    }
                    else
                    {
                        if (isUpDirection)
                        {
                            yDistanceToNextPoint *= -1;
                        }
                        this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + yDistanceToNextPoint, this.HitBox.Width, this.HitBox.Height);
                    }
                }
                else if (Math.Abs(yOffset) <= yDistanceToNextPoint)
                {
                    int yCollidePos = isUpDirection ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                    this.HitBox = new Rectangle(this.HitBox.X, yCollidePos, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    if (isUpDirection)
                    {
                        yDistanceToNextPoint *= -1;
                    }
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + yDistanceToNextPoint, this.HitBox.Width, this.HitBox.Height);
                }
            }

            if (xOffset != 0)
            {
                if (horizontalTile == null)
                {
                    if (Math.Abs(xOffset) <= xDistanceToNextPoint)
                    {
                        _horizontalMoveKeenVal = Math.Abs(xOffset);
                        this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                    else
                    {
                        int xDistanceToTravel = /*Math.Abs(xOffset) - */xDistanceToNextPoint;
                        _horizontalMoveKeenVal = xDistanceToTravel;
                        if (isLeftDirection)
                        {
                            xDistanceToTravel *= -1;
                        }

                        this.HitBox = new Rectangle(this.HitBox.X + xDistanceToTravel, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                }
                else if (Math.Abs(xOffset) <= xDistanceToNextPoint)
                {
                    int xCollidePos = isLeftDirection ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                    _horizontalMoveKeenVal = Math.Abs(this.HitBox.X - xCollidePos);
                    this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    _horizontalMoveKeenVal = xDistanceToNextPoint;
                    if (isLeftDirection)
                    {
                        xDistanceToNextPoint *= -1;
                    }
                    this.HitBox = new Rectangle(this.HitBox.X + xDistanceToNextPoint, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
            }
            else
            {
                _horizontalMoveKeenVal = 0;
            }

            if (this.HitBox.Location.Equals(_pathwayPoints[_currentPathwayPointIndex]))
            {
                _currentLocationReached = true;
            }
        }

        private void SetDirectionBasedOnNextNode(int nextIndex)
        {
            Point current = this.HitBox.Location;
            Point next = _pathwayPoints[nextIndex];

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
                        _direction = Enums.Direction.UP;
                        break;
                    case 1:
                        _direction = Enums.Direction.UP_RIGHT;
                        break;
                    case -1:
                        _direction = Enums.Direction.UP_LEFT;
                        break;
                }
            }
            else if (current.Y < next.Y)
            {
                yVal = 1;
                switch (xVal)
                {
                    case 0:
                        _direction = Enums.Direction.DOWN;
                        break;
                    case 1:
                        _direction = Enums.Direction.DOWN_RIGHT;
                        break;
                    case -1:
                        _direction = Enums.Direction.DOWN_LEFT;
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
                        _direction = Enums.Direction.RIGHT;
                        break;
                    case -1:
                        _direction = Enums.Direction.LEFT;
                        break;
                }
            }
        }
    }
}
