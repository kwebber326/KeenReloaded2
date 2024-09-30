using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using System;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class HorizontalSlicestar : Slicestar
    {
        private int _startPointX;
        private int _endPointX;
        private bool _firstMove = true;
        public HorizontalSlicestar(Rectangle area, SpaceHashGrid grid, int zIndex, Direction direction,
            int startPointX, int endPointX)
            : base(grid, area, zIndex, direction)
        {
            _startPointX = startPointX;
            _endPointX = endPointX;

            _sprite = Properties.Resources.keen5_horizontal_slicestar;
            ValidateStartAndEndPoints();
        }

        private void ValidateStartAndEndPoints()
        {
            if (_startPointX >= _endPointX)
            {
                throw new ArgumentException("X position of end point must be greater than the X position of the start point for horizontal slicestars");
            }

            if (this.Direction != Enums.Direction.LEFT && this.Direction != Enums.Direction.RIGHT)
            {
                throw new ArgumentException("Direction must be set to 'left' or 'right' for horizontal slicestars");
            }
        }

        public override void Move()
        {
            if (_firstMove)
            {
                _sprite = Properties.Resources.keen5_slicestar;
                _firstMove = false;
            }
            int xOffset = this.Direction == Enums.Direction.LEFT ? VELOCITY * -1 : VELOCITY;
            int xPos = this.Direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xPos, this.HitBox.Y, this.HitBox.Width + VELOCITY, this.HitBox.Height);

            var collisions = this.CheckCollision(areaToCheck, true);

            var tile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (tile != null)
            {
                int collideXPos = this.Direction == Enums.Direction.LEFT ? tile.HitBox.Right + 1 : tile.HitBox.Left - this.HitBox.Width - 1;
                Rectangle collisionCheck = new Rectangle(this.Direction == Enums.Direction.LEFT ? collideXPos : this.HitBox.X,
                    this.HitBox.Y, this.HitBox.Width + Math.Abs(this.HitBox.X - collideXPos), this.HitBox.Height);
                this.KillCollidingPlayers(collisionCheck);

                this.HitBox = new Rectangle(collideXPos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Direction = ChangeHorizontalDirection(this.Direction);
            }
            else
            {
                //get the remaining distance to the endpoint
                int remainingXDistance = this.Direction == Enums.Direction.LEFT
                    ? this.HitBox.X - _startPointX
                    : _endPointX - this.HitBox.X;
                //how far to move is limited first by distance to end point, then by the slicestar's velocity
                int moveDistance = remainingXDistance < VELOCITY
                    ? this.Direction == Enums.Direction.LEFT
                        ? remainingXDistance * -1
                        : remainingXDistance
                    : xOffset;

                //kill keen if colliding
                this.KillCollidingPlayers(areaToCheck);
                if (remainingXDistance > 0)
                {
                    //finally, move the slicestar
                    this.HitBox = new Rectangle(this.HitBox.X + moveDistance, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    this.Direction = this.ChangeHorizontalDirection(this.Direction);
                }
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen5_horizontal_slicestar);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{this.Direction}{separator}{_startPointX}{separator}{_endPointX}";
        }
    }
}
