using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class HorizontalTrickPlatform : Platform
    {
        private const int VERTICAL_VISION = 100;
        private const int VERTICAL_VISION_OFFSET = 100;
        private const int QUICK_MOVE_SPEED = 50;
        private const int SLOW_MOVE_SPEED = 10;
        private const int MAX_LUNGE_DISTANCE = 250;

        private TrickPlatformState _state;
        private int _currentLungeDistance;
        private Direction _returnDirection;
        private int _originalX;

        public HorizontalTrickPlatform(Rectangle area, SpaceHashGrid grid, int zIndex, PlatformType type)
            : base(grid, area, type, zIndex, Guid.Empty)
        {
            _direction = GetRandomHorizontalDirection();
            _state = TrickPlatformState.WAITING;
            _originalX = this.HitBox.X;
            _moveVelocity = SLOW_MOVE_SPEED;
        }

        private Direction GetDirectionFromKeenLocation()
        {
            if (_keen.HitBox.X < this.HitBox.X + this.HitBox.Width / 2)
            {
                return Direction.RIGHT;
            }
            return Direction.LEFT;
        }

        public override void Activate()
        {

        }

        public override void Deactivate()
        {

        }

        public override void Update()
        {
            switch (_state)
            {
                case TrickPlatformState.WAITING:
                    this.Wait();
                    break;
                case TrickPlatformState.LUNGING:
                    this.Lunge();
                    break;
                case TrickPlatformState.RETURNING:
                    this.ReturnToOriginalLocation();
                    break;
            }
        }

        private void ReturnToOriginalLocation()
        {
            if (_state != TrickPlatformState.RETURNING)
            {
                _state = TrickPlatformState.RETURNING;
                _currentLungeDistance = 0;
                _returnDirection = _direction == Direction.LEFT ? Direction.RIGHT : Direction.LEFT;
                _moveVelocity = SLOW_MOVE_SPEED;
            }
            int originalX = this.HitBox.X;
            bool returned = _returnDirection == Direction.LEFT ? this.HitBox.X - SLOW_MOVE_SPEED <= _originalX : this.HitBox.X + SLOW_MOVE_SPEED >= _originalX;
            if (returned)
            {
                this.HitBox = new Rectangle(_originalX, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.Wait();
                return;
            }

            int xOffset = _returnDirection == Direction.LEFT ? SLOW_MOVE_SPEED * -1 : SLOW_MOVE_SPEED;
            int xLocation = _returnDirection == Direction.LEFT ? this.HitBox.X - QUICK_MOVE_SPEED : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xLocation, this.HitBox.Y, this.HitBox.Width + SLOW_MOVE_SPEED, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck);
            var hitTile = _returnDirection == Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (hitTile == null)
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                if (IsKeenStandingOnPlatform() && (this.HitBox.X != originalX))
                    UpdateKeenHorizontalPosition(_returnDirection);
            }
        }

        private void UpdateKeenHorizontalPosition(Direction direction)
        {
            if (!_keen.IsDead())
                _keen.GetMovedHorizontally(this, direction, _moveVelocity);
        }

        private void Lunge()
        {
            if (_state != TrickPlatformState.LUNGING)
            {
                _state = TrickPlatformState.LUNGING;
                _moveVelocity = QUICK_MOVE_SPEED;
            }
            int originalX = this.HitBox.X;
            int xOffset = _direction == Direction.LEFT ? QUICK_MOVE_SPEED * -1 : QUICK_MOVE_SPEED;
            int xLocation = _direction == Direction.LEFT ? this.HitBox.X - QUICK_MOVE_SPEED : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xLocation, this.HitBox.Y, this.HitBox.Width + QUICK_MOVE_SPEED, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck);
            var hitTile = _direction == Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (hitTile != null)
            {
                int newXLocation = _direction == Direction.LEFT ? hitTile.HitBox.Right + 1 : hitTile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(newXLocation, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.ReturnToOriginalLocation();
            }
            else if (_currentLungeDistance + QUICK_MOVE_SPEED <= MAX_LUNGE_DISTANCE)
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                _currentLungeDistance += QUICK_MOVE_SPEED;
            }
            else
            {
                int moveDistance = MAX_LUNGE_DISTANCE - _currentLungeDistance;
                int newXLocation = _direction == Direction.LEFT ? this.HitBox.X - moveDistance : this.HitBox.X + moveDistance;
                this.HitBox = new Rectangle(newXLocation, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.ReturnToOriginalLocation();
            }
            if (IsKeenStandingOnPlatform() && (this.HitBox.X != originalX))
                UpdateKeenHorizontalPosition(_direction);
        }

        private void Wait()
        {
            if (_state != TrickPlatformState.WAITING)
            {
                _state = TrickPlatformState.WAITING;
            }
            _keen = this.GetClosestPlayer();

            _direction = GetDirectionFromKeenLocation();

            bool keenInVision = IsKeenInVision();
            bool keenStandingOnPlatform = IsKeenStandingOnPlatform();
            if (keenInVision && !keenStandingOnPlatform && (_keen.MoveState == MoveState.FALLING || _keen.MoveState == MoveState.JUMPING))
            {
                _direction = GetDirectionFromKeenLocation();
                this.Lunge();
            }
        }

        private bool IsKeenInVision()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Top - VERTICAL_VISION - VERTICAL_VISION_OFFSET, this.HitBox.Width, VERTICAL_VISION);
            bool inVision = _keen.HitBox.IntersectsWith(areaToCheck);
            return inVision;
        }

        protected override string GetImageNameFromType()
        {
            switch (_type)
            {
                case PlatformType.KEEN4:
                    return "keen4_trick_platform";
                case PlatformType.KEEN5_PINK:
                    return "keen5_pink_trick_platform";
                case PlatformType.KEEN5_ORANGE:
                    return "keen5_orange_trick_platform";
                case PlatformType.KEEN6:
                    return "keen6_trick_platform";
            }

            return nameof(Properties.Resources.keen6_bip_platform);
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = this.GetImageNameFromType();

            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_type}";
        }
    }

    enum TrickPlatformState
    {
        WAITING,
        LUNGING,
        RETURNING
    }
}
