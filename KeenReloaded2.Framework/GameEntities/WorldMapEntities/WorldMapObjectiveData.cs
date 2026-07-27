using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class WorldMapObjectiveData
    {
        Dictionary<Guid, List<IActivator>> _objectiveObjectMapping =
            new Dictionary<Guid, List<IActivator>>();

        public IEnumerable<ILevelObjective> LevelObjectives { get; set; }

        public Func<bool> LevelObjectivesComplete { get; set; }

        public List<IActivator> ToggleObjects =>
            _objectiveObjectMapping.Values.SelectMany(s => s).ToList();

        public event EventHandler GameBeaten;
        public event EventHandler<ToggleEventArgs> Toggled;

        public void CompleteWorldMapObjective(Guid activationId)
        {
            if (_objectiveObjectMapping.TryGetValue(activationId, out List<IActivator> activationSet))
            {
                foreach (var activateableObject in activationSet)
                {
                    activateableObject.Toggle();
                }
            }

            if (this.LevelObjectivesComplete?.Invoke() ?? false)
            {
                this.GameBeaten?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
