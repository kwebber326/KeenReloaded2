using KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEntities.Weapons;
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

        public static Checkpoint LastHitCheckPoint { get; private set; }
        public static KeenInventoryProfile KeenInventoryProfile { get; private set; }

        public static void UpdateLastHitCheckPoint(Checkpoint checkpoint, CommanderKeen keen)
        {
            LastHitCheckPoint = checkpoint;
            KeenInventoryProfile = new KeenInventoryProfile()
            {
                Points = keen.Points,
                Drops = keen.Drops,
                Lives = keen.Lives,
                Weapons = keen.Weapons
            };
        }

        public static void ClearCheckPointMarker()
        {
            LastHitCheckPoint = null;
            KeenInventoryProfile = null;
        }
    }

    public class KeenInventoryProfile
    {
        public int Lives { get; set; }
        public int Drops { get; set; }

        public long Points { get; set; }

        public List<NeuralStunner> Weapons { get; set; }
    }
}
