using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles.WorldMap
{
    public class WorldMapInteractiveTile : CollisionObject, ISprite
    {
        protected readonly Rectangle _area;
        protected int _zIndex;
        protected Image _sprite;
        protected readonly WorldMapInteractiveTileAction _action;
        protected readonly string _tag;

        public WorldMapInteractiveTile(Rectangle area, SpaceHashGrid grid, int zIndex, WorldMapInteractiveTileAction action, Image sprite) 
            : base(grid, area)
        {
            _area = area;
            _zIndex = zIndex;
            _sprite = sprite;
            if (_sprite?.Tag == null)
                throw new ArgumentNullException("Sprite and or sprite tag is null");

            _tag = sprite.Tag.ToString();
            _action = action;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public virtual bool CanUpdate => false;

        public WorldMapInteractiveTileAction Action => _action;

        public override CollisionType CollisionType => CollisionType.NONE;

        protected string GetImageFileName(string fullPath)
        {
            string altPathExtension = fullPath?.Substring(fullPath.LastIndexOf('.')) ?? ".png";
            string imageName = FileIOUtility.ExtractFileNameFromPath(fullPath);
            return imageName + altPathExtension;
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = FileIOUtility.ExtractFileNameFromPath(_tag);
            string imageFileName = GetImageFileName(_tag);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{ZIndex}{separator}{_action.ToString()}{separator}{(imageFileName)}";
        }
    }
}
