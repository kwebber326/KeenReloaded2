using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class MapEdgeTile : MaskedTile
    {
        private readonly MapEdgeBehavior _behavior;

        public MapEdgeTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, MapEdgeBehavior behavior)
            : base(area, grid, hitbox, imageFile, zIndex)
        {
            _image = Properties.Resources.edge_of_map_tile_debug;
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
                case MapEdgeBehavior.EXIT:
                    return CollisionType.EXIT;
                case MapEdgeBehavior.DEATH:
                    return CollisionType.HAZARD;
                case MapEdgeBehavior.BARRIER:
                default:
                    return CollisionType.BLOCK;
            }
        }

        public override CollisionType CollisionType => GetCollisionTypeFromBehavior();
    }
}
