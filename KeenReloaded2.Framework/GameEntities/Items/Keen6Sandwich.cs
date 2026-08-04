using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class Keen6Sandwich : CollisionObject, ISprite, IItemLevelObjective
    {
        private readonly int _zIndex;
        private bool _acquired;

        public Keen6Sandwich(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
        }

        public override CollisionType CollisionType => CollisionType.EXIT;

        public int ZIndex => _zIndex;

        public Image Image => Properties.Resources.keen6_sandwich;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => false;

        public ObjectiveEventType EventType => ObjectiveEventType.LEVEL_EXIT;

        public bool ObjectiveComplete => _acquired;

        public void Acquire()
        {
            _acquired = true;
        }

        public WorldMapItemType ItemType => WorldMapItemType.SANDWICH;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string name = nameof(Properties.Resources.keen6_sandwich);
            return $"{name}{separator}{this.Location.X}{separator}{this.Location.Y}{separator}" +
                   $"{this.HitBox.Width}{separator}{this.HitBox.Height}{separator}{_zIndex}";
        }
    }
}
