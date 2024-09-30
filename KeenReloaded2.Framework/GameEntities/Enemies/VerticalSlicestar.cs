using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using System;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class VerticalSlicestar : Slicestar
    {
        private int _startPointY;
        private int _endPointY;
        private bool _firstMove = true;
        public VerticalSlicestar(Rectangle area, SpaceHashGrid grid, int zIndex, Direction direction,
            int startPointY, int endPointY)
            : base(grid, area, zIndex, direction)
        {
            _startPointY = startPointY;
            _endPointY = endPointY;
            _sprite = Properties.Resources.keen5_vertical_slicestar;
            
            ValidateStartAndEndPoints();
        }

        private void ValidateStartAndEndPoints()
        {

            if (_startPointY >= _endPointY)
            {
                throw new ArgumentException("Y position of end point must be less than the Y position of the start point for vertical slicestars");
            }

            if (this.Direction != Enums.Direction.UP && this.Direction != Enums.Direction.DOWN)
            {
                throw new ArgumentException("Direction must be set to 'up' or 'down' for vertical slicestars");
            }
        }

        public override void Move()
        {
            if (_firstMove)
            {
                _sprite = Properties.Resources.keen5_slicestar;
                _firstMove = false;
            }
            int yOffset = this.Direction == Enums.Direction.UP ? VELOCITY * -1 : VELOCITY;
            int yPos = this.Direction == Enums.Direction.UP ? this.HitBox.Y + yOffset : this.HitBox.Y;
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, yPos, this.HitBox.Width, this.HitBox.Height + VELOCITY);

            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = this.Direction == Enums.Direction.UP ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);
            if (tile != null)
            {
                int collideYPos = this.Direction == Enums.Direction.UP ? tile.HitBox.Bottom + 1 : tile.HitBox.Top - this.HitBox.Height - 1;
                Rectangle collisionCheck = new Rectangle(this.HitBox.X,
                    this.Direction == Enums.Direction.UP ? collideYPos : this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + Math.Abs(this.HitBox.Y - collideYPos));
                this.KillCollidingPlayers(collisionCheck);

                this.HitBox = new Rectangle(this.HitBox.X, collideYPos, this.HitBox.Width, this.HitBox.Height);
                this.Direction = this.ChangeVerticalDirection(this.Direction);
            }
            else
            {
                //get the remaining distance to the endpoint
                int remainingYDistance = this.Direction == Enums.Direction.UP
                    ? this.HitBox.Y - _startPointY
                    : _endPointY - this.HitBox.Y;
                //how far to move is limited first by distance to end point, then by the slicestar's velocity
                int moveDistance = remainingYDistance < VELOCITY
                    ? this.Direction == Enums.Direction.UP
                        ? remainingYDistance * -1
                        : remainingYDistance
                    : yOffset;

                //kill keen if colliding
                this.KillCollidingPlayers(areaToCheck);

                if (remainingYDistance > 0)
                {
                    //finally, move the slicestar
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + moveDistance, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    //change direction if we reached the end
                    this.Direction = this.ChangeVerticalDirection(this.Direction);
                }
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_vertical_slicestar);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{this.Direction}{separator}{_startPointY}{separator}{_endPointY}";
        }
    }
}
