using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;


namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class DropPlatform : Platform
    {
        protected int _fallDistanceLimit;
        protected int _currentFallDistance;
        protected Rectangle _originalLocation;

        public DropPlatform(Rectangle area, SpaceHashGrid grid, int zIndex, PlatformType type, int maxDrop)
            : base(grid, area, type, zIndex, Guid.NewGuid())
        {
            _fallDistanceLimit = maxDrop;
            _originalLocation = new Rectangle(this.HitBox.Location, this.HitBox.Size);
        }

        protected virtual void Fall()
        {
            if (_currentFallDistance < _fallDistanceLimit)
            {
                _direction = Direction.DOWN;
                var landingTile = GetTopMostLandingTile(_currentVerticalVelocity);
                if (landingTile != null)
                {
                    _currentFallDistance += landingTile.HitBox.Top - this.HitBox.Bottom - 1;
                    this.HitBox = new Rectangle(this.HitBox.X, landingTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                    _currentVerticalVelocity = 0;
                    UpdateKeenVerticalPosition();
                }
                else
                {
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                    _currentFallDistance += _currentVerticalVelocity;
                    if (_currentVerticalVelocity + _acceleration <= MAX_GRAVITY_SPEED)
                    {
                        _currentVerticalVelocity += _acceleration;
                    }
                    UpdateKeenVerticalPosition();
                }

            }
        }

        public override void Activate()
        {

        }

        public override void Deactivate()
        {

        }

        public override void Update()
        {
            _keen = this.GetClosestAlivePlayer();
            bool keenStandingOnPlatform = KeenIsStandingOnThis();
            if (keenStandingOnPlatform && _currentFallDistance < _fallDistanceLimit)
            {
                if (_currentVerticalVelocity < 0)
                    _currentVerticalVelocity = 0;

                this.Fall();
            }
            else if (!keenStandingOnPlatform && _currentFallDistance > 0)
            {
                if (_currentVerticalVelocity > 0)
                {
                    _currentVerticalVelocity = 0;
                }
                this.Rise();
            }
            this.UpdateCollisionNodes(Direction.UP);
            this.UpdateCollisionNodes(Direction.DOWN);
        }

        protected virtual void Rise()
        {
            _direction = Direction.UP;
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck);
            var ceilingTile = GetCeilingTile(collisions);
            if (ceilingTile != null)
            {
                _currentVerticalVelocity = 0;
                _currentFallDistance -= ceilingTile.HitBox.Bottom - 1 - this.HitBox.Top;
                this.HitBox = new Rectangle(this.HitBox.X, ceilingTile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
            }
            else
            {
                if (_currentFallDistance + _currentVerticalVelocity < 0)
                {
                    this.HitBox = new Rectangle(this.HitBox.X, _originalLocation.Y, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                }
                _currentFallDistance += _currentVerticalVelocity;
                if (_currentVerticalVelocity - _acceleration >= MAX_GRAVITY_SPEED * -1)
                {
                    _currentVerticalVelocity -= _acceleration;
                }
                if (_currentFallDistance < 0)
                {
                    _currentFallDistance = 0;
                }
            }
            if (this.CollidesWithPlayer(collisions) && !KeenIsStandingOnThis() && _currentVerticalVelocity <= (MAX_GRAVITY_SPEED / 2) * -1)
            {
                UpdateKeenVerticalPosition();
            }
        }

        private bool CollidesWithPlayer(List<CollisionObject> collisions)
        {
            return collisions?.Contains(_keen) ?? false;
        }

        protected override string GetImageNameFromType()
        {
            switch (_type)
            {
                case PlatformType.KEEN4:
                    return "keen4_drop_platform";
                case PlatformType.KEEN5_PINK:
                    return "keen5_pink_drop_platform";
                case PlatformType.KEEN5_ORANGE:
                    return "keen5_orange_drop_platform";
                case PlatformType.KEEN6:
                    return "keen6_drop_platform";
            }

            return nameof(Properties.Resources.keen6_bip_platform);
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = this.GetImageNameFromType();
           
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_type}{separator}{_fallDistanceLimit}";
        }
    }
}
