using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class KeenRailgunShot : KeenStunShot
    {
        public KeenRailgunShot(SpaceHashGrid grid, Rectangle hitbox,
            int damage, int velocity, int pierce, int spread, int blastRadius, int refireDelay, Direction direction)
            : base(grid, hitbox, damage, velocity, pierce, spread, blastRadius, refireDelay, direction)
        {
            this.CheckInitialCollision();
        }

        protected override CollisionObject GetTopMostLandingTile(int currentFallVelocity)
        {
            CollisionObject topMostTile;
            Rectangle areaTocheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + currentFallVelocity);
            var items = this.CheckCollision(areaTocheck, true);

            var landingTiles = items.Where(h => h.HitBox.Top >= this.HitBox.Top);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        protected void CheckInitialCollision()
        {
            var collisionsToHandle = new List<CollisionObject>();
            switch (this.Direction)
            {

                case Direction.DOWN:
                    Rectangle areaToCheck = new Rectangle(this.HitBox.Location, this.HitBox.Size);
                    var collisions = this.CheckCollision(areaToCheck);
                    if (collisions != null && collisions.Any())
                    {
                        collisions = collisions.OrderBy(o => o.HitBox.Top).ToList();

                        this.HandleInitialCollisions(collisions, this.Direction);
                    }
                    break;
                case Direction.UP:
                    areaToCheck = new Rectangle(this.HitBox.Location, this.HitBox.Size);
                    collisions = this.CheckCollision(areaToCheck);
                    if (collisions != null && collisions.Any())
                    {
                        collisions = collisions.OrderByDescending(o => o.HitBox.Bottom).ToList();

                        this.HandleInitialCollisions(collisions, this.Direction);
                    }
                    break;
                case Direction.LEFT:
                    areaToCheck = new Rectangle(this.HitBox.Location, this.HitBox.Size);
                    collisions = this.CheckCollision(areaToCheck);
                    if (collisions != null && collisions.Any())
                    {
                        collisions = collisions.OrderByDescending(o => o.HitBox.Right).ToList();

                        this.HandleInitialCollisions(collisions, this.Direction);
                    }
                    break;
                case Direction.RIGHT:
                    areaToCheck = new Rectangle(this.HitBox.Location, this.HitBox.Size);
                    collisions = this.CheckCollision(areaToCheck);
                    if (collisions != null && collisions.Any())
                    {
                        collisions = collisions.OrderBy(o => o.HitBox.Left).ToList();

                        this.HandleInitialCollisions(collisions, this.Direction);
                    }
                    break;
            }
        }

        private void HandleInitialCollisions(List<CollisionObject> collisions, Direction direction)
        {
            foreach (var collision in collisions)
            {
                if (collision.CollisionType == CollisionType.BLOCK)
                {
                    switch (direction)
                    {
                        case Direction.DOWN:
                            this.HitBox = new Rectangle(this.HitBox.X, collision.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                            break;
                        case Direction.UP:
                            this.HitBox = new Rectangle(this.HitBox.X, collision.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                            break;
                        case Direction.LEFT:
                            this.HitBox = new Rectangle(collision.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                            break;
                        case Direction.RIGHT:
                            this.HitBox = new Rectangle(collision.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                            break;
                    }
                    this.Stop();
                    break;
                }
                this.HandleCollision(collision);
            }
        }

        protected override void InitializeSprites()
        {
            base.InitializeSprites();
            _shotCompleteSprites = new Image[0];
            Size spriteSize = new Size();
            Point spriteLocation = new Point();
            if (this.Direction == Enums.Direction.LEFT || this.Direction == Enums.Direction.RIGHT)
            {
                spriteSize = new Size(_sprite.Width * 10, _sprite.Height / 2);
                spriteLocation= new Point(Direction == Enums.Direction.LEFT ? this.Location.X - (spriteSize.Width) : this.Location.X, this.Location.Y);
            }
            else if (this.Direction == Enums.Direction.UP || this.Direction == Enums.Direction.DOWN)
            {
                spriteSize = new Size(_sprite.Width / 2, _sprite.Height * 10);
                spriteLocation = new Point(this.Location.X + _sprite.Width / 2, this.Direction == Enums.Direction.UP ? this.Location.Y - spriteSize.Height : this.Location.Y);
            }
            this.HitBox = new Rectangle(spriteLocation, spriteSize);
            _sprite = BitMapTool.DrawImageAtLocationWithDimensions(_sprite, this.HitBox);
        }
    }
}
