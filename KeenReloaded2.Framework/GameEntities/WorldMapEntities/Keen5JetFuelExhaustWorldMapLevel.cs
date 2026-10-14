using KeenReloaded.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class Keen5JetFuelExhaustWorldMapLevel : AnimatedForegroundMultiHitboxWorldMapLevel
    {
        public Keen5JetFuelExhaustWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Guid activationId, Rectangle[] entryPoints, Rectangle[] hitboxes, Rectangle[] foregroundAreas, string imagesPath, int animationDelay, int animationStartIndex, string key) 
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, activationId, entryPoints, hitboxes, foregroundAreas, imagesPath, animationDelay, animationStartIndex, key)
        {
        }
    }
}
