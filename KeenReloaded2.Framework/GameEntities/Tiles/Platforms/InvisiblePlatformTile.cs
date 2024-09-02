using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class InvisiblePlatformTile : InvisibleTile
    {
        public InvisiblePlatformTile(SpaceHashGrid grid, Rectangle hitbox) : base(grid, hitbox)
        {
        }

        public override CollisionType CollisionType => CollisionType.PLATFORM;
    }
}
