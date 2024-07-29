using KeenReloaded.Framework;
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
                if (collision is MaskedTile)
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
            _shotCompleteSprites = _shotSprites;
            //this.Sprite.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            //if (this.Direction == Enums.Direction.LEFT || this.Direction == Enums.Direction.RIGHT)
            //{
            //    this.Sprite.Size = new Size(this.Sprite.Size.Width * 10, this.Sprite.Height / 2);
            //    this.Sprite.Location = new Point(Direction == Enums.Direction.LEFT ? this.Sprite.Location.X - (this.Sprite.Width) : this.Sprite.Location.X, this.Sprite.Location.Y + this.Sprite.Height / 2);
            //}
            //else if (this.Direction == Enums.Direction.UP || this.Direction == Enums.Direction.DOWN)
            //{
            //    this.Sprite.Size = new Size(this.Sprite.Size.Width / 2, this.Sprite.Height * 10);
            //    this.Sprite.Location = new Point(this.Sprite.Location.X + this.Sprite.Width / 2, this.Direction == Enums.Direction.UP ? this.Sprite.Location.Y - this.Sprite.Height : this.Sprite.Location.Y);
            //}
            this.HitBox = new Rectangle(this.Location, _sprite.Size);
        }
    }
}
