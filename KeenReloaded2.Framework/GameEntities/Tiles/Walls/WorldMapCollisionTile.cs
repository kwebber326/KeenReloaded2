using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Walls
{
    public class WorldMapCollisionTile : CollisionObject, ISprite
    {
        private readonly Rectangle _area;
        private int _zIndex;
        private Image _sprite;
        private readonly string _imageName;

        public WorldMapCollisionTile(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite) : base(grid, area)
        {
            _area = area;
            try
            {
                _sprite = sprite;
                _zIndex = zIndex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => false;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string altFilePath = _sprite.Tag?.ToString();
            string altPathExtension = altFilePath?.Substring(altFilePath.LastIndexOf('.')) ?? ".png";
            string imageName = FileIOUtility.ExtractFileNameFromPath(altFilePath);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{ZIndex}{separator}{(imageName + altPathExtension)}";
        }
    }
}
