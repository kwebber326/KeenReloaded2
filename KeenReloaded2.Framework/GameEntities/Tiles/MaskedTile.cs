using KeenReloaded.Framework;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using KeenReloaded2.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public abstract class MaskedTile : CollisionObject, ISprite
    {
        protected readonly int _zIndex;
        protected readonly string _initialImageName;
        protected readonly string _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
        protected int _upwardCollisionOffset = 0, _downwardCollisionOffset = 0, _leftwardCollisionOffset = 0, _rightwardCollisionOffset = 0;

        protected Image _image;
        protected Rectangle _area;


        public MaskedTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex) : base(grid, hitbox)
        {
            _zIndex = zIndex;
            try
            {
                _area = area;
                _image = Image.FromFile(imageFile);
                this.HitBox = area;
                _initialImageName = FileIOUtility.ExtractFileNameFromPath(imageFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error constructing {this.GetType().Name}:\n{ex}");
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _image;

        public Point Location => _area.Location;

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        protected virtual void AdjustHitboxBasedOnOffsets()
        {
            var newWidth = this.HitBox.Width - _leftwardCollisionOffset - _rightwardCollisionOffset;
            var newHeight = this.HitBox.Height - _upwardCollisionOffset - _downwardCollisionOffset;
            if (newWidth <= 0 || newHeight <= 0)
                return;

            this.HitBox = new Rectangle(
                this.HitBox.Location.X + _leftwardCollisionOffset,
                this.HitBox.Location.Y + _downwardCollisionOffset,
                newWidth, newHeight);
        }

        public override string ToString()
        {
            return $"{_initialImageName}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}";
        }
    }
}
