using KeenReloaded2.Framework.GameEntities.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities
{
    public static class LevelCompleteObjectives
    {
        private static List<DestructibleCollisionTile> _tileObjectives = new List<DestructibleCollisionTile>();

        public static bool TryAddTileObjective(DestructibleCollisionTile objective)
        {
            if (objective == null || objective.EventType != Enums.TileDestroyedEventType.LEVEL_EXIT)
                return false;

            if (_tileObjectives.Any(t => t.Equals(objective)))
                return false;

            _tileObjectives.Add(objective);
            return true;
        }

        public static bool AreAllTileObjectivesComplete()
        {
            return _tileObjectives.All(t => t.IsDead());
        }

        public static void ClearAll()
        {
            _tileObjectives.Clear();
        }
    }
}
