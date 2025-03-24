using KeenReloaded2.Framework.GameEntities.Interfaces;
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
        private static List<ILevelObjective> _levelObjectives = new List<ILevelObjective>();

        public static bool TryAddTileObjective(ILevelObjective objective)
        {
            if (objective == null || objective.EventType != Enums.ObjectiveCompleteEvent.LEVEL_EXIT)
                return false;

            if (_levelObjectives.Any(t => t.Equals(objective)))
                return false;

            _levelObjectives.Add(objective);
            return true;
        }

        public static bool AreAllTileObjectivesComplete()
        {
            return _levelObjectives.All(t => t.ObjectiveComplete);
        }

        public static void ClearAll()
        {
            _levelObjectives.Clear();
        }
    }
}
