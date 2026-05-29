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

        public WorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Rectangle hitbox, Image sprite) 
            : base(grid, hitbox)
        {
            _zIndex = zIndex;
            _sprite = sprite;
            _area = area;
            _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            _objectKey = _sprite?.Tag?.ToString() ?? "unknown_level";
            _objectKey = FileIOUtility.ExtractFileNameFromPath(_objectKey);
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public virtual bool CanUpdate => false;

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public override string ToString()
        {
            return $"{_objectKey}{_separator}"+ 
                $"{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}"+
                $"{_separator}{_zIndex}{_separator}{this.HitBox.X}{_separator}{this.HitBox.Y}" + 
                $"{_separator}{this.HitBox.Width}{_separator}{this.HitBox.Height}";
        }
    }
}
