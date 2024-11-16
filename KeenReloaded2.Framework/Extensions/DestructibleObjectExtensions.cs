using KeenReloaded.Framework;
using KeenReloaded2.Framework.GameEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.Extensions
{
    public static class DestructibleObjectExtensions
    {
        private static bool IsOutSideOfMap(this DestructibleObject destructibleObject, SpaceHashGrid collisionGrid)
        {
            bool isOutSideLeft = destructibleObject.HitBox.Right < 0;
            bool isOutSideRight = destructibleObject.HitBox.Left > collisionGrid.Size.Width;
            bool isOutSideTop = destructibleObject.HitBox.Bottom < -200;//use lesser value than 0 since we can jump to the edge of the map bounds
            bool isOutSideBottom = destructibleObject.HitBox.Top > collisionGrid.Size.Height;
            return isOutSideLeft || isOutSideRight || isOutSideTop || isOutSideBottom;
        }
        public static void KillIfOutSideBoundsOfMap(this DestructibleObject destructibleObject, SpaceHashGrid collisionGrid)
        {
            if (destructibleObject == null || collisionGrid == null)
                return;

            if (IsOutSideOfMap(destructibleObject, collisionGrid))
            {
                destructibleObject.Die();
            }
        }
    }
}
