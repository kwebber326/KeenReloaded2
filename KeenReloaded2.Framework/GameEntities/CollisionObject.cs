using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Tiles;

namespace KeenReloaded.Framework
{
    public abstract class CollisionObject
    {
        protected bool debug = true;
        protected SpaceHashGrid _collisionGrid;
        protected HashSet<SpaceHashGridNode> _collidingNodes;
        protected Random _random = new Random(new Random().Next(0, int.MaxValue));

        public CollisionObject(SpaceHashGrid grid, Rectangle hitbox)
        {
            this.HitBox = hitbox;
            this._collisionGrid = grid;
            _collidingNodes = grid.GetCurrentHashes(this);
            foreach (SpaceHashGridNode node in _collidingNodes)
            {
                node.Objects.Add(this);
                AddIfTile(node);
                AddIfNotEnemy(node);
            }
        }

        protected virtual int GenerateRandomInteger(int min, int max)
        {
            int seed = new Random().Next(0, int.MaxValue);
            _random = new Random(seed);
            int retVal = _random.Next(min, max + 1);
            return retVal;
        }

        protected virtual void ResetRandomVariable()
        {
            Random seedR = new Random();
            var seed = seedR.Next(0, int.MaxValue);
            _random = new Random(seed);
        }

        private void AddIfNotEnemy(SpaceHashGridNode node)
        {
            //if (!(this is IEnemy))
            //{
            //    node.NonEnemies.Add(this);
            //}
        }

        private void AddIfTile(SpaceHashGridNode node)
        {
            //if (this is MaskedTile || (this is PlatformTile && !(this is MovingPlatformTile)) || this is PoleTile)
            //{
            //    node.Tiles.Add(this);
            //}
        }


        protected virtual void UpdateHitboxBasedOnStunnedImage(Image[] _stunnedSprites, ref int _currentStunnedSprite, ref int _currentSpriteChangeDelayTick, int spriteChangeDelay, Action updateAction)
        {
            var spriteIndex = _currentStunnedSprite;
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentStunnedSprite, spriteChangeDelay, updateAction);
            if (_currentStunnedSprite != spriteIndex)
            {
                try
                {
                    if (_currentStunnedSprite >= _stunnedSprites.Length)
                        _currentStunnedSprite = 0;

                    var image = _stunnedSprites[_currentStunnedSprite];
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - (image.Size.Height - this.HitBox.Height), image.Width, image.Height);
                }
                catch { }
            }
        }

        protected virtual void UpdateSpriteByDelayBase(ref int delayTicker, ref int spriteIndex, int delayThreshold, Action updateAction)
        {
            if (delayTicker++ == delayThreshold)
            {
                delayTicker = 0;
                spriteIndex++;
                updateAction();
            }
        }

        public bool CollidesWith(CollisionObject obj)
        {
            if (this.HitBox == null)
                return false;

            return this.HitBox.IntersectsWith(obj.HitBox);
        }

        //protected void KillKeenIfCollidingDiagonalMovement(Rectangle areaToCheck, CommanderKeen keen, Direction direction)
        //{
        //    if (keen.HitBox.Right >= areaToCheck.Left && keen.HitBox.Left <= areaToCheck.Right)
        //    {
        //        double rise = areaToCheck.Height - this.HitBox.Height, run = areaToCheck.Width;
        //        double slope = rise / run;
        //        if (IsUpDirection(direction))
        //            slope *= -1.0;

        //        double xInputKeen = keen.HitBox.Right - areaToCheck.Left;
        //        double yOutput = this.HitBox.Y + slope * xInputKeen;

        //        Rectangle killArea = new Rectangle(keen.HitBox.Left, (int)yOutput, this.HitBox.Width, this.HitBox.Height);
        //        if (keen.HitBox.IntersectsWith(killArea))
        //        {
        //            keen.Die();
        //        }
        //    }
        //}

        protected virtual CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var tiles = collisions.Where(c => c is MaskedTile && c.HitBox.Bottom <= this.HitBox.Top).ToList();
            if (tiles.Any())
            {
                int maxBottom = tiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        protected virtual bool IsNothingBeneath()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, 2);
            var items = this.CheckCollision(areaToCheck, true);
            return !items.Any();
        }

        //protected virtual void SetHorizontalDirectionFromKeenLocation(CommanderKeen keen, ref Direction direction)
        //{
        //    if (keen == null)
        //        throw new ArgumentNullException("keen cannot be null");

        //    if (keen.HitBox.Left < this.HitBox.Left)
        //    {
        //        direction = Direction.LEFT;
        //    }
        //    else if (keen.HitBox.Right > this.HitBox.Right)
        //    {
        //        direction = Direction.RIGHT;
        //    }
        //}

        //protected virtual void SetVerticalDirectionFromKeenLocation(CommanderKeen keen, ref Direction direction)
        //{
        //    if (keen == null)
        //        throw new ArgumentNullException("keen cannot be null");

        //    if (keen.HitBox.Bottom < this.HitBox.Top)
        //    {
        //        direction = Direction.UP;
        //    }
        //    else if (keen.HitBox.Bottom > this.HitBox.Bottom)
        //    {
        //        direction = Direction.DOWN;
        //    }
        //}

        //protected virtual void SetFullDirectionFromKeenLocation(CommanderKeen keen, ref Direction direction)
        //{
        //    if (keen == null)
        //        throw new ArgumentNullException("keen cannot be null");
        //    var initVertDir = Direction.LEFT;
        //    SetVerticalDirectionFromKeenLocation(keen, ref initVertDir);
        //    if (initVertDir == Direction.UP)
        //    {
        //        if (keen.HitBox.Left < this.HitBox.Left)
        //        {
        //            direction = Direction.UP_LEFT;
        //        }
        //        else if (keen.HitBox.Right > this.HitBox.Right)
        //        {
        //            direction = Direction.UP_RIGHT;
        //        }
        //        else
        //        {
        //            direction = Direction.UP;
        //        }
        //    }
        //    else if (initVertDir == Direction.DOWN)
        //    {
        //        if (keen.HitBox.Left < this.HitBox.Left)
        //        {
        //            direction = Direction.DOWN_LEFT;
        //        }
        //        else if (keen.HitBox.Right > this.HitBox.Right)
        //        {
        //            direction = Direction.DOWN_RIGHT;
        //        }
        //        else
        //        {
        //            direction = Direction.DOWN;
        //        }
        //    }
        //    else if (keen.HitBox.Left < this.HitBox.Left)
        //    {
        //        direction = Direction.LEFT;
        //    }
        //    else
        //    {
        //        direction = Direction.RIGHT;
        //    }
        //}

        protected bool IsDiagonalDirection(Direction direction)
        {
            return direction == Direction.DOWN_LEFT || direction == Direction.DOWN_RIGHT || direction == Direction.UP_LEFT || direction == Direction.UP_RIGHT;
        }

        protected bool IsUpDirection(Direction direction)
        {
            return direction == Direction.UP || direction == Direction.UP_LEFT || direction == Direction.UP_RIGHT;
        }

        protected bool IsDownDirection(Direction direction)
        {
            return direction == Direction.DOWN || direction == Direction.DOWN_LEFT || direction == Direction.DOWN_RIGHT;
        }

        protected bool IsLeftDirection(Direction direction)
        {
            return direction == Direction.LEFT || direction == Direction.UP_LEFT || direction == Direction.DOWN_LEFT;
        }

        protected bool IsRightDirection(Direction direction)
        {
            return direction == Direction.RIGHT || direction == Direction.UP_RIGHT || direction == Direction.DOWN_RIGHT;
        }

        protected virtual bool IsHorizontalDirection(Direction direction)
        {
            return direction == Direction.LEFT || direction == Direction.RIGHT;
        }

        protected virtual bool IsVerticalDirection(Direction direction)
        {
            return direction == Direction.UP || direction == Direction.DOWN;
        }

        protected virtual Direction ChangeVerticalDirection(Direction direction)
        {
            return direction == Direction.UP ? Direction.DOWN : Direction.UP;
        }

        protected virtual Direction ChangeHorizontalDirection(Direction direction)
        {
            return direction == Direction.LEFT ? Direction.RIGHT : Direction.LEFT;
        }

        protected virtual Direction TurnLeft(Direction direction)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    return Direction.RIGHT;
                case Direction.UP:
                    return Direction.LEFT;
                case Direction.LEFT:
                    return Direction.DOWN;
                case Direction.RIGHT:
                    return Direction.UP;
                case Direction.DOWN_LEFT:
                    return Direction.DOWN_RIGHT;
                case Direction.DOWN_RIGHT:
                    return Direction.UP_LEFT;
                case Direction.UP_LEFT:
                    return Direction.DOWN_LEFT;
                case Direction.UP_RIGHT:
                    return Direction.UP_LEFT;
            }
            return Direction.LEFT;
        }

        protected virtual Direction TurnRight(Direction direction)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    return Direction.LEFT;
                case Direction.UP:
                    return Direction.RIGHT;
                case Direction.LEFT:
                    return Direction.UP;
                case Direction.RIGHT:
                    return Direction.DOWN;
                case Direction.DOWN_LEFT:
                    return Direction.UP_LEFT;
                case Direction.DOWN_RIGHT:
                    return Direction.DOWN_LEFT;
                case Direction.UP_LEFT:
                    return Direction.UP_RIGHT;
                case Direction.UP_RIGHT:
                    return Direction.DOWN_RIGHT;
            }
            return Direction.RIGHT;
        }

        protected virtual bool IsOutOfBounds()
        {
            bool isOutOfBounds = (this.HitBox.X - this.HitBox.Width < 0)
                              || (this.HitBox.Right + this.HitBox.Width > _collisionGrid.Size.Width)
                              || (this.HitBox.Y - this.HitBox.Height) < 0
                              || (this.HitBox.Bottom + this.HitBox.Height > _collisionGrid.Size.Height);
            return isOutOfBounds;
        }

        protected virtual CollisionObject GetLeftMostRightTile(List<CollisionObject> collisions)
        {
            var MaskedTiles = collisions.OfType<MaskedTile>();
            if (collisions.Any() && MaskedTiles.Any())
            {
                var rightTiles = MaskedTiles.Where(c => c.HitBox.Left > this.HitBox.Left && c.HitBox.Top < this.HitBox.Bottom && c.HitBox.Bottom > this.HitBox.Top).ToList();
                if (rightTiles.Any())
                {
                    int minX = rightTiles.Select(t => t.HitBox.Left).Min();
                    CollisionObject obj = rightTiles.FirstOrDefault(x => x.HitBox.Left == minX);
                    return obj;
                }
            }
            return null;
        }

        protected virtual CollisionObject GetRightMostLeftTile(List<CollisionObject> collisions)
        {
            var MaskedTiles = collisions.OfType<MaskedTile>();
            if (collisions.Any() && MaskedTiles.Any())
            {
                var leftTiles = MaskedTiles.Where(c => c.HitBox.Left < this.HitBox.Left && c.HitBox.Top < this.HitBox.Bottom && c.HitBox.Bottom > this.HitBox.Top).ToList();
                if (leftTiles.Any())
                {
                    int maxX = leftTiles.Select(t => t.HitBox.Right).Max();
                    CollisionObject obj = leftTiles.FirstOrDefault(x => x.HitBox.Right == maxX);
                    return obj;
                }
            }
            return null;
        }

        //protected virtual CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        //{
        //    CollisionObject topMostTile = null;
        //    var landingTiles = collisions.Where(h => (h is MaskedTile || h is Platform || h is PlatformTile || h is PoleTile || h is Keen6Switch)
        //        && h.HitBox.Top >= this.HitBox.Top);

        //    if (!landingTiles.Any())
        //        return null;

        //    int minY = landingTiles.Select(c => c.HitBox.Top).Min();
        //    topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

        //    return topMostTile;
        //}

        protected virtual CollisionObject GetTopMostLandingTile(int currentFallVelocity)
        {
            CollisionObject topMostTile;
            Rectangle areaTocheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, currentFallVelocity);
            var items = this.CheckCollision(areaTocheck, true);

            var landingTiles = items.Where(h => h.HitBox.Top >= this.HitBox.Top);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        protected virtual bool IsOnEdge(Direction directionToCheck, int edgeOffset = 0)
        {
            if (directionToCheck == Direction.LEFT)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.Left - this.HitBox.Width + edgeOffset, this.HitBox.Bottom, this.HitBox.Width, 2);
                var tiles = this.CheckCollision(areaToCheck, true);
                return !tiles.Any();
            }
            else if (directionToCheck == Direction.RIGHT)
            {
                Rectangle areaToCheck = new Rectangle(this.HitBox.Right - edgeOffset, this.HitBox.Bottom, this.HitBox.Width, 2);
                var tiles = this.CheckCollision(areaToCheck, true);
                return !tiles.Any();
            }
            return false;
        }

        public virtual void DetachFromCollisionGrid()
        {
            foreach (var node in _collidingNodes)
            {
                if (node != null)
                {
                    if (node.Objects != null)
                        node.Objects.Remove(this);
                    if (node.Tiles != null)
                        node.Tiles.Remove(this);
                    if (node.NonEnemies != null)
                        node.NonEnemies.Remove(this);
                }
            }
        }

        protected virtual void BasicFall(int fallVelocity)
        {
            var landingTile = this.GetTopMostLandingTile(fallVelocity);
            if (landingTile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, landingTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + fallVelocity, this.HitBox.Width, this.HitBox.Height);
            }
        }

        protected virtual CollisionObject BasicFallReturnTile(int fallVelocity)
        {
            var landingTile = this.GetTopMostLandingTile(fallVelocity);
            if (landingTile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, landingTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + fallVelocity, this.HitBox.Width, this.HitBox.Height);
            }
            return landingTile;
        }

        public virtual void DetachFromObjects()
        {
            foreach (var node in _collidingNodes)
            {
                if (node != null && node.Tiles != null)
                    node.Objects.Remove(this);
            }
        }

        public virtual void DetachFromTiles()
        {
            foreach (var node in _collidingNodes)
            {
                if (node != null && node.Tiles != null)
                    node.Tiles.Remove(this);
            }
        }

        public virtual List<CollisionObject> CheckCollision(Rectangle areaToCheck, bool tilesOnly = false, bool includeEnemies = true)
        {
            HashSet<CollisionObject> collidingObjects = new HashSet<CollisionObject>();

            //check existing colliding nodes for any collisions first
            foreach (SpaceHashGridNode node in _collidingNodes)
            {
                if (node != null)
                {
                    //var nodeCollisions = !tilesOnly ? node.Objects.Where(n => n != this && n.HitBox.IntersectsWith(areaToCheck)) :
                    //    node.Tiles.Where(n => n != this && n.HitBox.IntersectsWith(areaToCheck));
                    //collidingObjects.AddRange(nodeCollisions);
                    AddNodeCollisions(areaToCheck, tilesOnly, includeEnemies, collidingObjects, node);
                }
            }
            //if any of the area to check is not covered by the current collision nodes, add all the necessary collision nodes to cover the entire area to check for collisions
            var extraCollidingNodes = GetAllNodesInAreaToCheck(areaToCheck);

            foreach (SpaceHashGridNode node in extraCollidingNodes)
            {
                if (node != null)
                {
                    //var nodeCollisions = !tilesOnly ? node.Objects.Where(n => n != this && n.HitBox.IntersectsWith(areaToCheck)) :
                    //    node.Tiles.Where(n => n != this && n.HitBox.IntersectsWith(areaToCheck));
                    //collidingObjects.AddRange(nodeCollisions);
                    AddNodeCollisions(areaToCheck, tilesOnly, includeEnemies, collidingObjects, node);
                }
            }

            return new List<CollisionObject>(collidingObjects);
        }

        private void AddNodeCollisions(Rectangle areaToCheck, bool tilesOnly, bool includeEnemies, HashSet<CollisionObject> collidingObjects, SpaceHashGridNode node)
        {
            var nodeCollisions = new HashSet<CollisionObject>();
            if (tilesOnly)
            {
                nodeCollisions = new HashSet<CollisionObject>(node.Tiles.Where(n => n != this && n.HitBox.IntersectsWith(areaToCheck)).ToList());
            }
            else
            {

                if (!includeEnemies)
                {
                    nodeCollisions = new HashSet<CollisionObject>(node.NonEnemies.Where(n => n != this && n.HitBox.IntersectsWith(areaToCheck)).ToList());
                }
                else
                {
                    nodeCollisions = new HashSet<CollisionObject>(node.Objects.Where(n => n != this && n.HitBox.IntersectsWith(areaToCheck)).ToList());
                }
            }
            foreach (var collision in nodeCollisions)
            {
                collidingObjects.Add(collision);
            }
        }

        /// <summary>
        /// Sets the vertical direction to go to based on the object's Y position
        /// </summary>
        /// <param name="obj">the object in question</param>
        /// <param name="chase">set to true to chase object, set to false to avoid</param>
        /// <returns></returns>
        protected virtual Direction SetDirectionFromObjectVertical(CollisionObject obj, bool chase)
        {
            if (obj == null)
                throw new ArgumentNullException("object to chase cannot be null");

            if (this.HitBox.Y < obj.HitBox.Y)
            {
                return chase ? Direction.DOWN : Direction.UP;
            }
            return chase ? Direction.UP : Direction.DOWN;
        }
        /// <summary>
        /// Sets the horizontal direction to go to based on the object's X position
        /// </summary>
        /// <param name="obj">the object in question</param>
        /// <param name="chase">set to true to chase object, set to false to avoid</param>
        /// <returns></returns>
        protected virtual Direction SetDirectionFromObjectHorizontal(CollisionObject obj, bool chase)
        {
            if (obj == null)
                throw new ArgumentNullException("object to chase cannot be null");

            if (this.HitBox.X < obj.HitBox.X)
            {
                return chase ? Direction.RIGHT : Direction.LEFT;
            }
            return chase ? Direction.LEFT : Direction.RIGHT;
        }

        private List<SpaceHashGridNode> GetAllNodesInAreaToCheck(Rectangle areaToCheck)
        {
            try
            {
                if (!_collidingNodes.Any())
                    return new List<SpaceHashGridNode>();

                int minX = _collidingNodes.Select(n => n.HashBox.X).Min();
                int maxX = _collidingNodes.Select(n => n.HashBox.Right).Max();
                int minY = _collidingNodes.Select(n => n.HashBox.Y).Min();
                int maxY = _collidingNodes.Select(n => n.HashBox.Y).Max();
                var extraCollidingNodes = new List<SpaceHashGridNode>();
                //if the area is too far to the left for the colliding nodes

                while (areaToCheck.Left < minX)
                {
                    var borderNodes = _collidingNodes.Where(n => n.HashBox.X == minX).ToList();
                    var extraNodes = extraCollidingNodes.Where(n => n.HashBox.X == minX).ToList();
                    borderNodes.AddRange(extraNodes);
                    if (!borderNodes.Any())
                        break;

                    foreach (var n in borderNodes)
                    {
                        if (n.Left != null)
                        {
                            extraCollidingNodes.Add(n.Left);
                            minX = n.Left.HashBox.X;
                        }
                        else
                        {
                            minX = int.MinValue;
                            break;
                        }
                    }
                }

                //if the area is too far to the right for the colliding nodes
                while (areaToCheck.Right > maxX)
                {
                    var borderNodes = _collidingNodes.Where(n => n.HashBox.Right == maxX).ToList();
                    var extraNodes = extraCollidingNodes.Where(n => n.HashBox.Right == maxX).ToList();
                    borderNodes.AddRange(extraNodes);
                    if (!borderNodes.Any())
                        break;

                    foreach (var n in borderNodes)
                    {
                        if (n.Right != null)
                        {
                            extraCollidingNodes.Add(n.Right);
                            maxX = n.Right.HashBox.Right;
                        }
                        else
                        {
                            maxX = int.MaxValue;
                            break;
                        }
                    }
                }

                //if the area is too far to the top for the colliding nodes
                while (areaToCheck.Top < minY)
                {
                    var borderNodes = _collidingNodes.Where(n => n.HashBox.Y == minY).ToList();
                    var extraNodes = extraCollidingNodes.Where(n => n.HashBox.Y == minY).ToList();
                    borderNodes.AddRange(extraNodes);
                    if (!borderNodes.Any())
                        break;

                    foreach (var n in borderNodes)
                    {
                        if (n.Up != null)
                        {
                            extraCollidingNodes.Add(n.Up);
                            minY = n.Up.HashBox.Y;
                        }
                        else
                        {
                            minY = int.MinValue;
                            break;
                        }
                    }
                }

                //if the area is too far to the bottom for the colliding nodes
                while (areaToCheck.Bottom > maxY)
                {
                    var borderNodes = _collidingNodes.Where(n => n.HashBox.Y == maxY).ToList();
                    var extraNodes = extraCollidingNodes.Where(n => n.HashBox.Y == maxY).ToList();
                    borderNodes.AddRange(extraNodes);
                    if (!borderNodes.Any())
                        break;

                    foreach (var n in borderNodes)
                    {
                        if (n.Down != null)
                        {
                            extraCollidingNodes.Add(n.Down);
                            maxY = n.Down.HashBox.Y;
                        }
                        else
                        {
                            maxY = int.MaxValue;
                            break;
                        }
                    }
                }
                return extraCollidingNodes;
            }
            catch
            {
                return new List<SpaceHashGridNode>();
            }

        }

        public virtual void UpdateCollisionNodes(Direction direction)
        {
            if (direction == Direction.UP_RIGHT)
            {
                UpdateCollisionNodes(Direction.UP);
                UpdateCollisionNodes(Direction.RIGHT);
                return;
            }

            if (direction == Direction.UP_LEFT)
            {
                UpdateCollisionNodes(Direction.UP);
                UpdateCollisionNodes(Direction.LEFT);
                return;
            }

            if (direction == Direction.DOWN_LEFT)
            {
                UpdateCollisionNodes(Direction.DOWN);
                UpdateCollisionNodes(Direction.LEFT);
                return;
            }

            if (direction == Direction.DOWN_RIGHT)
            {
                UpdateCollisionNodes(Direction.DOWN);
                UpdateCollisionNodes(Direction.RIGHT);
                return;
            }

            var newCollidingNodes = new HashSet<SpaceHashGridNode>();
            if (!_collidingNodes.Any())
            {
                _collidingNodes = _collisionGrid.GetCurrentHashes(this);
            }
            else
            {
                foreach (SpaceHashGridNode node in _collidingNodes)
                {
                    if (node != null)
                    {
                        if (!node.HashBox.IntersectsWith(this.HitBox))
                        {
                            node.Objects.Remove(this);
                            SpaceHashGridNode n = node;
                            AddNextNodeOver(direction, newCollidingNodes, n);
                        }
                        else
                        {
                            newCollidingNodes.Add(node);
                            AddCollidingNeighbors(newCollidingNodes, node);
                        }
                    }
                }

                _collidingNodes = newCollidingNodes;
                foreach (var node in _collidingNodes)
                {
                    node.Objects.Add(this);
                }
            }
        }

        private void AddCollidingNeighbors(HashSet<SpaceHashGridNode> newCollidingNodes, SpaceHashGridNode node)
        {
            //if on or spilling over right edge, add the right tile to new nodes
            var current = node;
            while (this.HitBox.Right >= current.HashBox.Right)
            {
                if (current.Right != null)
                {
                    newCollidingNodes.Add(current.Right);
                    current = current.Right;
                }
                else
                {
                    break;
                }
            }

            //if on or spilling over left edge, add the left tile to new nodes
            current = node;
            while (this.HitBox.Left <= current.HashBox.Left)
            {
                if (current.Left != null)
                {
                    newCollidingNodes.Add(current.Left);
                    current = current.Left;
                }
                else
                    break;
            }

            //if on or spilling over top edge, add the up tile to new nodes
            current = node;
            while (this.HitBox.Top <= current.HashBox.Top)
            {
                if (current.Up != null)
                {
                    newCollidingNodes.Add(current.Up);
                    current = current.Up;
                }
                else
                    break;
            }

            //if on or spilling over bottom edge, add the down tile to new nodes
            current = node;
            while (this.HitBox.Bottom >= current.HashBox.Bottom - 1)
            {
                if (current.Down != null)
                {
                    newCollidingNodes.Add(current.Down);
                    current = current.Down;
                }
                else
                    break;
            }

        }

        private void AddNextNodeOver(Direction direction, HashSet<SpaceHashGridNode> newCollidingNodes, SpaceHashGridNode n)
        {
            switch (direction)
            {
                case Direction.RIGHT:
                    while (!n.HashBox.IntersectsWith(this.HitBox) && n.Right != null)
                    {
                        n = n.Right;
                    }
                    break;

                case Direction.LEFT:
                    while (!n.HashBox.IntersectsWith(this.HitBox) && n.Left != null)
                    {
                        n = n.Left;
                    }
                    break;

                case Direction.UP:
                    while (!n.HashBox.IntersectsWith(this.HitBox) && n.Up != null)
                    {
                        n = n.Up;
                    }
                    break;

                case Direction.DOWN:
                    while (!n.HashBox.IntersectsWith(this.HitBox) && n.Down != null)
                    {
                        n = n.Down;
                    }
                    break;
            }
            if (n.HashBox.IntersectsWith(this.HitBox))
                newCollidingNodes.Add(n);
        }

        public virtual Rectangle HitBox
        {
            get;
            protected set;
        }
    }
}
