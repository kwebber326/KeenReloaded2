﻿using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class SetPathPlatform : Platform, IActivateable
    {
        protected bool _isActive;
        protected List<Point> _pathwayPoints;
        int _currentPathwayPointIndex = 0;
        int _horizontalMoveKeenVal = 0;
        private bool _currentLocationReached;

        public SetPathPlatform(Rectangle area, SpaceHashGrid grid, int zIndex, PlatformType type, List<Point> locations, Guid activationId, bool initiallyActive = false)
            : base(grid, area, type, zIndex, activationId)
        {
            if (locations == null)
                throw new ArgumentNullException("Pathway Platforms need a pathway to move");

            _pathwayPoints = locations;
            _isActive = initiallyActive;
            if (_pathwayPoints.Any())
            {
                SetDirectionBasedOnNextNode(0);
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

        public virtual Direction Direction
        {
            get
            {
                return _direction;
            }
            protected set
            {
                _direction = value;
            }
        }

        public Guid ActivationID => _activationId;

        public override bool CanUpdate => true;

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
               Move();
            }
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
                        this.Move(new Point(this.HitBox.X, this.HitBox.Y + yOffset));
                    }
                    else
                    {
                        if (isUpDirection)
                        {
                            yDistanceToNextPoint *= -1;
                        }
                        this.Move(new Point(this.HitBox.X, this.HitBox.Y + yDistanceToNextPoint));
                    }
                }
                else if (Math.Abs(yOffset) <= yDistanceToNextPoint)
                {
                    int yCollidePos = isUpDirection ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                    this.Move(new Point(this.HitBox.X, yCollidePos));
                }
                else
                {
                    if (isUpDirection)
                    {
                        yDistanceToNextPoint *= -1;
                    }
                    this.Move(new Point(this.HitBox.X, this.HitBox.Y + yDistanceToNextPoint));
                }
            }

            if (xOffset != 0)
            {
                if (horizontalTile == null)
                {
                    if (Math.Abs(xOffset) <= xDistanceToNextPoint)
                    {
                        _horizontalMoveKeenVal = Math.Abs(xOffset);
                        this.Move(new Point(this.HitBox.X + xOffset, this.HitBox.Y));
                    }
                    else
                    {
                        int xDistanceToTravel = xDistanceToNextPoint;
                        _horizontalMoveKeenVal = xDistanceToTravel;
                        if (isLeftDirection)
                        {
                            xDistanceToTravel *= -1;
                        }

                        this.Move(new Point(this.HitBox.X + xDistanceToTravel, this.HitBox.Y));
                    }
                }
                else if (Math.Abs(xOffset) <= xDistanceToNextPoint)
                {
                    int xCollidePos = isLeftDirection ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                    _horizontalMoveKeenVal = Math.Abs(this.HitBox.X - xCollidePos);
                    this.Move(new Point(xCollidePos, this.HitBox.Y));
                }
                else
                {
                    _horizontalMoveKeenVal = xDistanceToNextPoint;
                    if (isLeftDirection)
                    {
                        xDistanceToNextPoint *= -1;
                    }
                    this.Move(new Point(this.HitBox.X + xDistanceToNextPoint, this.HitBox.Y));
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
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = GetImageNameFromType();
            string pathArray = MapMakerConstants.MAP_MAKER_ARRAY_START;
            for (int i = 0; i < _pathwayPoints.Count; i++)
            {
                var node = _pathwayPoints[i];
                string item = node.X + MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR + node.Y + ((i == _pathwayPoints.Count - 1)
                    ? ""
                    : MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR);
                pathArray += item;
            }
            pathArray += MapMakerConstants.MAP_MAKER_ARRAY_END;
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_type}{separator}{pathArray}{separator}{_activationId}{separator}{_isActive}";
        }
    }
}
