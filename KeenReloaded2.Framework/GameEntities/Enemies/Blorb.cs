using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Blorb : CollisionObject, IUpdatable, ISprite, IEnemy
    {
        private Image _sprite;
        private Image[] _sprites;
        private const int FIRST_DELAY = 8, SECOND_DELAY = 3;
        private int SPRITE_CHANGE_DELAY = 5;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;
        private const int VELOCITY = 5;

        public Blorb(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
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

        private readonly int _zIndex;

        private void Initialize()
        {
            _sprites = SpriteSheet.SpriteSheet.BlorbImages;
            _sprite = _sprites[_currentSprite];
           this.Direction = GetRandomDiagonalDirection();
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile = null;
            var landingTiles = collisions.Where(h => (h.CollisionType == CollisionType.BLOCK 
            || h.CollisionType == CollisionType.PLATFORM 
            || h.CollisionType == CollisionType.POLE_TILE 
            || (h.CollisionType == CollisionType.KEEN6_SWITCH && ((Keen6Switch)h).IsActive && this.IsDownDirection(this.Direction)))
            && h.HitBox.Top > this.HitBox.Top && h.HitBox.Left < this.HitBox.Right && h.HitBox.Right > this.HitBox.Left);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        protected override CollisionObject GetTopMostLandingTile(int currentFallVelocity)
        {
            CollisionObject topMostTile;
            Rectangle areaTocheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, currentFallVelocity);
            var items = this.CheckCollision(areaTocheck, true);

            var landingTiles = items.Where(h =>  h.HitBox.Top > this.HitBox.Top 
                && h.HitBox.Left < this.HitBox.Right && h.HitBox.Right > this.HitBox.Left);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        public void Update()
        {
            //will determine how/if we should bounce (default no bounce)
            bool bounceHorizontal = false, bounceVertical = false;
            //determine which diagonal direction we are moving in
            bool isLeftDirection = IsLeftDirection(this.Direction);
            bool isUpDirection = IsUpDirection(this.Direction);

            //get the velocities on horizontal and vertical plane
            int xOffset = isLeftDirection ? VELOCITY * -1 : VELOCITY;
            int yOffset = isUpDirection ? VELOCITY * -1 : VELOCITY;

            //get the distance away from hitbox to check for
            int xPosCheck = isLeftDirection ? this.HitBox.X + xOffset : this.HitBox.X;
            int yPosCheck = isUpDirection ? this.HitBox.Y + yOffset : this.HitBox.Y;

            //get collisions
            Rectangle areaToCheck = new Rectangle(xPosCheck, yPosCheck, this.HitBox.Width + VELOCITY, this.HitBox.Height + VELOCITY);
            var collisions = this.CheckCollision(areaToCheck, true);

            var horizontalTile = isLeftDirection ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            var verticalTile = isUpDirection ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);

            //determine if we hit anything
            //by default, our new positions reflect a non collision
            int newXPos = this.HitBox.X + xOffset;
            int newYPos = this.HitBox.Y + yOffset;
            int xDistFromCollision = -1;
            int yDistFromCollision = -1;
            //set x,y coordinates from collisions
            if (horizontalTile != null)
            {
                newXPos = isLeftDirection ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                xDistFromCollision = isLeftDirection ? this.HitBox.Left - horizontalTile.HitBox.Right : horizontalTile.HitBox.Left - this.HitBox.Right;
                if (verticalTile == null)
                    bounceHorizontal = true;
            }
            if (verticalTile != null && !IsThroughPlatformTileVertically(verticalTile, isUpDirection))
            {
                newYPos = isUpDirection ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                yDistFromCollision = isUpDirection ? this.HitBox.Top - verticalTile.HitBox.Bottom : this.HitBox.Bottom - verticalTile.HitBox.Top;
                if (horizontalTile == null)
                    bounceVertical = true;
            }
            //in the event of collisions on both the horizontal and vertical plane, prioritize the one that is closer to the object
            //and update both x,y positions accordingly
            if (verticalTile != null && horizontalTile != null && horizontalTile != verticalTile && !IsThroughPlatformTileVertically(verticalTile, isUpDirection))
            {
                if (xDistFromCollision < yDistFromCollision)
                {
                    newYPos = isUpDirection ? this.HitBox.Y - xDistFromCollision : this.HitBox.Y + xDistFromCollision;
                    bounceHorizontal = true;
                }
                else
                {
                    newXPos = isLeftDirection ? this.HitBox.X - yDistFromCollision : this.HitBox.X + yDistFromCollision;
                    bounceVertical = true;
                }
            }//priority to horizontal bounce
            else if (horizontalTile != null && verticalTile != null && horizontalTile == verticalTile && !IsThroughPlatformTileVertically(verticalTile, isUpDirection))
            {
                newYPos = isUpDirection ? this.HitBox.Y - xDistFromCollision : this.HitBox.Y + xDistFromCollision;
                bounceHorizontal = true;
                bounceVertical = false;
            }

            //check for keen collisions
            CheckForKeenCollisions(isLeftDirection, isUpDirection, newXPos, newYPos);

            //update the location to the new x,y position
            this.HitBox = new Rectangle(newXPos, newYPos, this.HitBox.Width, this.HitBox.Height);


            //bounce if applicable to the situation (priority to horizontal)
            if (bounceHorizontal)
            {
                BounceHorizontal();
            }
            else if (bounceVertical)
            {
                BounceVertical();
            }

            //update sprite
            SPRITE_CHANGE_DELAY = _currentSprite == 0 ? SECOND_DELAY : FIRST_DELAY;
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSprite, SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        private void CheckForKeenCollisions(bool isLeftDirection, bool isUpDirection, int newXPos, int newYPos)
        {
            int xMovement = Math.Abs(this.HitBox.X - newXPos);
            int yMovement = Math.Abs(this.HitBox.Y - newYPos);
            int xCheck = isLeftDirection ? this.HitBox.X - xMovement : this.HitBox.X;
            int yCheck = isUpDirection ? this.HitBox.Y - yMovement : this.HitBox.Y;
            Rectangle areaToCheckForKeen = new Rectangle(xCheck, yCheck, this.HitBox.Width + xMovement, this.HitBox.Height + yMovement);
            KillCollidingPlayers(areaToCheckForKeen);
        }

        private void BounceHorizontal()
        {
            switch (this.Direction)
            {
                case Enums.Direction.DOWN_LEFT:
                    this.Direction = Enums.Direction.DOWN_RIGHT;
                    break;
                case Enums.Direction.DOWN_RIGHT:
                    this.Direction = Enums.Direction.DOWN_LEFT;
                    break;
                case Enums.Direction.UP_LEFT:
                    this.Direction = Enums.Direction.UP_RIGHT;
                    break;
                case Enums.Direction.UP_RIGHT:
                    this.Direction = Enums.Direction.UP_LEFT;
                    break;
            }
        }

        private void BounceVertical()
        {
            switch (this.Direction)
            {
                case Enums.Direction.DOWN_LEFT:
                    this.Direction = Enums.Direction.UP_LEFT;
                    break;
                case Enums.Direction.DOWN_RIGHT:
                    this.Direction = Enums.Direction.UP_RIGHT;
                    break;
                case Enums.Direction.UP_LEFT:
                    this.Direction = Enums.Direction.DOWN_LEFT;
                    break;
                case Enums.Direction.UP_RIGHT:
                    this.Direction = Enums.Direction.DOWN_RIGHT;
                    break;
            }
        }

        private void UpdateSprite()
        {
            if (_currentSprite >= _sprites.Length)
            {
                _currentSprite = 0;
            }
            _sprite = _sprites[_currentSprite];
        }

        private bool IsThroughPlatformTileVertically(CollisionObject platformTile, bool isupDirection)
        {
            if (platformTile == null)
                return false;

            if (this.HitBox.Bottom < platformTile.HitBox.Top && !isupDirection)
                return false;
            if (this.HitBox.Top > platformTile.HitBox.Bottom && isupDirection)
                return false;

            bool isPlatformTile = platformTile.CollisionType == CollisionType.PLATFORM;
            return isPlatformTile;
        }

        public bool DeadlyTouch
        {
            get { return true; }
        }

        public void HandleHit(IProjectile projectile)
        {

        }

        public bool IsActive
        {
            get { return false; }
        }

        public Enums.Direction Direction { get; set; }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_blorb1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
