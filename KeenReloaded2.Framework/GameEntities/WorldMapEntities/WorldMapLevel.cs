using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class WorldMapLevel : CollisionObject, ISprite
    {
        protected readonly int _zIndex;
        protected Image _sprite;
        protected readonly Rectangle _area;
        protected string _separator;
        protected string _objectKey;
        protected readonly string _levelName;
        protected readonly string _levelEntryText;

        public WorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText) 
            : base(grid, area)
        {
            _zIndex = zIndex;
            _sprite = sprite;
            _area = area;
            _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            _objectKey = _sprite?.Tag?.ToString() ?? "unknown_level";
            _objectKey = FileIOUtility.ExtractFileNameFromPath(_objectKey);
            _levelName = levelName;
            _levelEntryText = levelEntryText;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public virtual bool CanUpdate => false;

        public override CollisionType CollisionType => CollisionType.NONE;

        public override string ToString()
        {
            return $"{_objectKey}{_separator}"+ 
                $"{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}"+
                $"{_separator}{_zIndex}" + 
                $"{_separator}{_levelName}{_separator}{_levelEntryText}";
        }
    }
}
