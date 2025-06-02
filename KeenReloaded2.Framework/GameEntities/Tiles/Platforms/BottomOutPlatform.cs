using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class BottomOutPlatform : DropPlatform, ICreateRemove
    {
        private int _bottomOutDistance;
        private bool _bottomedOut;
        public BottomOutPlatform(Rectangle area, SpaceHashGrid grid, int zIndex, PlatformType type, int bottomOutDistance)
            : base(area, grid, zIndex, type, int.MaxValue)
        {
            _bottomOutDistance = bottomOutDistance;
        }

        protected override void Fall()
        {
            //Retain falling without vertical collision detection for self
            if (KeenIsStandingOnThis() || _bottomedOut)
            {
                _direction = Direction.DOWN;
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                bool hitGround = false;

                if (_keen != null)
                {
                    //since this falls through platforms, only update keen Y pos when keen hits floor tiles
                    Rectangle areaToCheck = new Rectangle(_keen.HitBox.X, _keen.HitBox.Y, _keen.HitBox.Width, _keen.HitBox.Height + _currentVerticalVelocity);
                    var collisions = _keen.CheckCollision(areaToCheck, true);
                    if (!collisions.Any() && (_keen.MoveState == MoveState.STANDING || _keen.MoveState == MoveState.RUNNING))
                        UpdateKeenVerticalPosition();
                    else if (collisions.Any() && !_bottomedOut)
                    {
                        var collisionTile = collisions.OrderBy(c => c.HitBox.Top).FirstOrDefault();
                        if (collisionTile != null)
                        {
                            this.HitBox = new Rectangle(this.HitBox.X, collisionTile.HitBox.Y - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                            UpdateKeenVerticalPosition();
                            hitGround = true;
                        }
                    }
                }

                if (!hitGround)
                    _currentFallDistance += _currentVerticalVelocity;

                //update speed
                if (_currentVerticalVelocity + _acceleration <= MAX_GRAVITY_SPEED)
                {
                    _currentVerticalVelocity += _acceleration;
                }

                //know when to keep falling without rising
                if (_currentFallDistance >= _bottomOutDistance)
                {
                    _bottomedOut = true;
                }
                //know when to remove self from map
                if (_collidingNodes == null || !_collidingNodes.Any())
                {
                    OnRemove();
                }
            }
        }

        public override void Update()
        {
            _keen = this.GetClosestAlivePlayer();
            bool keenStandingOnPlatform = KeenIsStandingOnThis();
            if (keenStandingOnPlatform || _bottomedOut)
            {
                if (_currentVerticalVelocity < 0)
                    _currentVerticalVelocity = 0;

                this.Fall();
            }
            else if (!keenStandingOnPlatform && _currentFallDistance > 0 && !_bottomedOut)
            {
                if (_currentVerticalVelocity > 0)
                {
                    _currentVerticalVelocity = 0;
                }
                this.Rise();
            }
        }

        protected void OnCreate()
        {
            if (Create != null)
                this.Create(this, null);
        }

        protected void OnRemove()
        {
            if (Remove != null)
            {
                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                DetachFromObjects();
                Remove(this, args);
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        protected override string GetImageNameFromType()
        {
            switch (_type)
            {
                case PlatformType.KEEN4:
                    return "keen4_bottom_out_platform";
                case PlatformType.KEEN5_PINK:
                    return "keen5_pink_bottom_out_platform";
                case PlatformType.KEEN5_ORANGE:
                    return "keen5_orange_bottom_out_platform";
                case PlatformType.KEEN6:
                    return "keen6_bottom_out_platform";
            }

            return nameof(Properties.Resources.keen6_bip_platform);
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = this.GetImageNameFromType();

            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_type}{separator}{_bottomOutDistance}";
        }
    }
}
