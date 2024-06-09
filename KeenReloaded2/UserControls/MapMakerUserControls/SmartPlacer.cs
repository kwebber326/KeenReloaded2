using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Enums;
using KeenReloaded.Framework;
using KeenReloaded2.Entities;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class SmartPlacer : UserControl
    {
        private readonly Size _defaultSize;
        private const int VERTICAL_LABEL_MARGIN = 2;
        private Graphics _graphics;

        public SmartPlacer()
        {
            InitializeComponent();
            _defaultSize = new Size(this.Size.Width, this.Size.Height);
        }

        public void DrawAdjacent(Size rectangleSize, GameObjectMapping gameObject, Direction direction)
        {
            if (gameObject != null && gameObject.Image != null)
            {
                this.Visible = true;
                int width = rectangleSize.Width;
                int height = rectangleSize.Height;
                this.Size = new Size(width, height);
                int x = gameObject.Location.X;
                int y = gameObject.Location.Y;

                int drawX = 0;
                int drawY = 0;

                if (direction == Direction.UP)
                {
                    y = gameObject.Bottom;
                    this.Location = new Point(x, y);
                }
                else if (direction == Direction.DOWN)
                {
                    y = gameObject.Top - this.Height;
                    this.Location = new Point(x, y);
                }
                else if (direction == Direction.LEFT)
                {
                    x = gameObject.Right;
                    this.Location = new Point(x, y);
                }
                else if (direction == Direction.RIGHT)
                {
                    x = gameObject.Left - this.Width;
                    this.Location = new Point(x, y);
                }
                else
                {
                    return;
                }

                _graphics = this.CreateGraphics();
                using (Pen p = new Pen(Color.HotPink, 4))
                {
                    _graphics.DrawRectangle(p, new Rectangle(drawX, drawY, rectangleSize.Width, rectangleSize.Height));
                }
            }
        }

        public void RemoveDrawing()
        {
            this.Invalidate();
            _graphics?.Dispose();
            this.Visible = false;
        }

        public GameObjectMapping FindClosestBlockOfSameType(List<GameObjectMapping> gameObjects, GameObjectMapping gameObject, out Direction? direction)
        {
            direction = null;
            int closeDistanceThreshold = 32;
            if (gameObject != null && gameObject.Image != null && gameObjects != null && gameObjects.Any())
            {
                var gameObjectRect = new Rectangle(gameObject.Location.X, gameObject.Location.Y, gameObject.Image.Width, gameObject.Image.Height);
                var collisionObjects = gameObjects.Select(c => c.GameObject).OfType<CollisionObject>();
                if (!collisionObjects.Any())
                    return null;
                var sameTypeObjects = collisionObjects.Where(g => (g.CollisionType == CollisionType.BLOCK || g.CollisionType == CollisionType.PLATFORM));
                if (!sameTypeObjects.Any())
                    return null;



                var objectsToTheLeft = gameObjects.Where(o =>
                    o != gameObject &&
                    o.Right < gameObjectRect.Left &&
                   Math.Abs(o.Location.Y - gameObjectRect.Y) <= closeDistanceThreshold).ToList();

                var objectsToTheRight = gameObjects.Where(o =>
                    o != gameObject &&
                    o.Left > gameObjectRect.Right &&
                    Math.Abs(o.Location.Y - gameObjectRect.Y) <= closeDistanceThreshold).ToList();

                var objectsAbove = gameObjects.Where(o =>
                     o != gameObject &&
                     o.Bottom < gameObjectRect.Top &&
                     Math.Abs(o.Location.X - gameObjectRect.X) <= closeDistanceThreshold).ToList();

                var objectsBelow = gameObjects.Where(o =>
                    o != gameObject &&
                    o.Top > gameObjectRect.Bottom &&
                    Math.Abs(o.Location.X - gameObjectRect.X) <= closeDistanceThreshold).ToList();

                int closestLeftDistance = objectsToTheLeft.Any() ? objectsToTheLeft.Select(o => gameObjectRect.Left - o.Right).Min() : closeDistanceThreshold + 1;
                int closestRightDistance = objectsToTheRight.Any() ? objectsToTheRight.Select(o => o.Left - gameObjectRect.Right).Min() : closeDistanceThreshold + 1;
                int closestAboveDistance = objectsAbove.Any() ? objectsAbove.Select(o => gameObjectRect.Top - o.Bottom).Min() : closeDistanceThreshold + 1;
                int closestBelowDistance = objectsBelow.Any() ? objectsBelow.Select(o => o.Top - gameObjectRect.Bottom).Min() : closeDistanceThreshold + 1;

                int min = closeDistanceThreshold + 1;
                if (closestLeftDistance < min)
                {
                    min = closestLeftDistance;
                    direction = Direction.LEFT;
                }
                if (closestRightDistance < min)
                {
                    min = closestRightDistance;
                    direction = Direction.RIGHT;
                }
                if (closestAboveDistance < min)
                {
                    min = closestAboveDistance;
                    direction = Direction.UP;
                }
                if (closestBelowDistance < min)
                {
                    min = closestBelowDistance;
                    direction = Direction.DOWN;
                }

                if (direction != null)
                {
                    GameObjectMapping retVal;
                    switch (direction)
                    {
                        case Direction.LEFT:
                            retVal = objectsToTheLeft.FirstOrDefault(o => gameObjectRect.Left - o.Right == min);
                            return retVal;
                        case Direction.RIGHT:
                            retVal = objectsToTheRight.FirstOrDefault(o => o.Left - gameObjectRect.Right == min);
                            return retVal;
                        case Direction.UP:
                            retVal = objectsAbove.FirstOrDefault(o => gameObjectRect.Top - o.Bottom == min);
                            return retVal;
                        case Direction.DOWN:
                            retVal = objectsBelow.FirstOrDefault(o => o.Top - gameObjectRect.Bottom == min);
                            return retVal;
                    }
                }
            }
            return null;
        }

        private void SmartPlacer_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
