using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Framework.GameEntities.Tiles.CustomTiles
{
    public class CustomStructure : CollisionObject, ISprite
    {
        private readonly CollisionType _collisionType;
        private readonly Rectangle[] _hitBoxes;
        private readonly Rectangle _area;
        private readonly string _imageFile;
        private readonly string _imageName;
        private readonly List<CollisionObject> _collisionTiles;
        private readonly int _zIndex;
        private Image _sprite;

        public CustomStructure(Rectangle area, SpaceHashGrid grid, int zIndex,
            CollisionType collisionType, string imageFile, Rectangle[] hitBoxes) : base(grid, area)
        {
            _collisionType = collisionType;
            _hitBoxes = hitBoxes;
            _area = area;
            _imageFile = imageFile;
            _imageName = FileIOUtility.ExtractFileNameFromPath(imageFile);
            _zIndex = zIndex;
            _sprite = Image.FromFile(imageFile);
            if (_collisionGrid != null && _collidingNodes != null)
            {
                _collisionTiles = _hitBoxes.Select(r => 
                _collisionType == CollisionType.BLOCK ?
                    new InvisibleTile(grid, 
                    new Rectangle(_area.X + r.X, _area.Y + r.Y, r.Width, r.Height)) :
                    new InvisiblePlatformTile(grid,
                    new Rectangle(_area.X + r.X, _area.Y + r.Y, r.Width, r.Height))
                        ).OfType<CollisionObject>().ToList();
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public virtual bool CanUpdate => false;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string hitBoxArrStr = BuildRectangleArrayStringRepresentation(_hitBoxes);
            return $"{_imageName}{separator}{_area.X}{separator}{_area.Y}{separator}" +
                   $"{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}" +
                   $"{_collisionType}{separator}{_imageFile}{separator}{hitBoxArrStr}";
        }
    }
}
