using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class MapEdgeTile : MaskedTile, IUpdatable
    {
        private readonly MapEdgeBehavior _behavior;

        public MapEdgeTile(Rectangle area, SpaceHashGrid grid, int zIndex, MapEdgeBehavior behavior)
            : base(area, grid, area, null, zIndex)
        {
            _image = CommonGameFunctions.DrawImage(area, Properties.Resources.edge_of_map_tile_debug);
            _behavior = behavior;
        }

        public MapEdgeBehavior Behavior
        {
            get
            {
                return _behavior;
            }
        }

        private CollisionType GetCollisionTypeFromBehavior()
        {
            switch (_behavior)
            {
                case MapEdgeBehavior.DEATH:
                    return CollisionType.HAZARD;
                case MapEdgeBehavior.EXIT:
                case MapEdgeBehavior.BARRIER:
                default:
                    return CollisionType.BLOCK;
            }
        }

        public override CollisionType CollisionType => GetCollisionTypeFromBehavior();

        public override string ToString()
        {
            return $"{nameof(Properties.Resources.edge_of_map_tile_debug)}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}{_separator}{_behavior}";
        }

        public void Update()
        {
            if (_image != null)
                _image = null;
        }
    }
}
