using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class Gem : Item
    {
        private GemColor _color;
        private bool _hasGravity;
        private bool _hasJumped;
        private const int FALL_VELOCITY = 30;
        private const int INITIAL_JUMP_VELOCITY = 10;
        private const int GRAVITY_ACCELERATION = 5;
        private const int MIN_AIR_RESISTANCE = 2;
        private int _currentAirResistance = GRAVITY_ACCELERATION;
        private int _currentVerticalVelocity = INITIAL_JUMP_VELOCITY * -1;
        public Gem(Rectangle area, SpaceHashGrid grid, string imageName, GemColor color, int zIndex, bool hasGravity = false)
            : base(area, imageName, grid, zIndex)
        {
            _color = color;
            _hasGravity = hasGravity;
            Initialize();
        }

        private void Initialize()
        {
            this.AcquiredSpriteList = SpriteSheet.SpriteSheet.GemAcquiredImages;
            switch (_color)
            {
                case GemColor.BLUE:
                    this.SpriteList = SpriteSheet.SpriteSheet.BlueGemImages;
                    break;
                case GemColor.GREEN:
                    this.SpriteList = SpriteSheet.SpriteSheet.GreenGemImages;
                    break;
                case GemColor.RED:
                    this.SpriteList = SpriteSheet.SpriteSheet.RedGemImages;
                    break;
                case GemColor.YELLOW:
                    this.SpriteList = SpriteSheet.SpriteSheet.YellowGemImages;
                    break;
            }
            this.Image = this.SpriteList.FirstOrDefault();
        }

        public GemColor Color
        {
            get
            {
                return _color;
            }
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
                if (this.Image != null && this.HitBox != null)
                {
                    if (_currentVerticalVelocity < 0)
                    {
                        this.UpdateCollisionNodes(Direction.UP);
                    }
                    else if (_currentVerticalVelocity > 0)
                    {
                        this.UpdateCollisionNodes(Direction.DOWN);
                    }
                }
            }
        }

        public override CollisionType CollisionType => CollisionType.ITEM;

        public override void Update()
        {
            base.Update();

            if (!this.IsAcquired && _hasGravity)
            {
                this.UpdateGravity();
            }
        }

        private void UpdateGravity()
        {
            if (!_hasJumped)
            {
                this.Jump();
            }
            else
            {
                this.Fall();
            }
        }

        private void Fall()
        {
            if (IsNothingBeneath())
            {

                var tile = this.BasicFallReturnTile(_currentVerticalVelocity);
                Rectangle areaToCheck;
                if (tile == null)
                {
                    areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + _currentVerticalVelocity);

                    if (_currentVerticalVelocity + GRAVITY_ACCELERATION < FALL_VELOCITY)
                    {
                        _currentVerticalVelocity += GRAVITY_ACCELERATION;
                    }
                    else
                    {
                        _currentVerticalVelocity = FALL_VELOCITY;
                    }
                }
                else
                {
                    _currentVerticalVelocity = 0;
                    areaToCheck = this.HitBox;
                }

                SetAcquiredIfCollidingWithKeen(areaToCheck);
            }
        }

        private void SetAcquiredIfCollidingWithKeen(Rectangle areaToCheck)
        {
            var collisions = this.CheckCollision(areaToCheck);
            //if (collisions.OfType<CommanderKeen>().Any())
            //{
            //    this.SetAcquired();
            //}
        }

        private void Jump()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height + Math.Abs(_currentVerticalVelocity));
            Rectangle keenCollisionCheck;
            var collisions = this.CheckCollision(areaToCheck, true);
            var tile = GetCeilingTile(collisions);
            if (tile != null)
            {
                _currentVerticalVelocity = 0;
                _hasJumped = true;
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                keenCollisionCheck = this.HitBox;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                keenCollisionCheck = areaToCheck;
                if (_currentVerticalVelocity + _currentAirResistance < 0)
                {
                    _currentVerticalVelocity += _currentAirResistance;
                    if (_currentAirResistance > MIN_AIR_RESISTANCE)
                        _currentAirResistance--;
                }
                else
                {
                    _currentVerticalVelocity = 0;
                    _hasJumped = true;
                }
            }
            SetAcquiredIfCollidingWithKeen(areaToCheck);
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + separator + this.Color.ToString() + separator + ZIndex + separator + _hasGravity.ToString();
        }
    }
}