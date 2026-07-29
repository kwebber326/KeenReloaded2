using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.VisualStyles;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class WorldMapObjectiveData
    {
        public static string GetKeyFromLevelObjective(ILevelObjective item, string levelName)
        {
            string name = item.GetType().Name;
            Point location = (item as ISprite)?.Location ?? new Point(0, 0);
            string key = levelName + "_" + name + "_" + location.ToString();
            return key;
        }

        public Dictionary<string, ILevelObjective> LevelObjectives { get; set; } = new Dictionary<string, ILevelObjective>();
        public Func<bool> LevelObjectivesComplete { get; set; }

        public List<IActivator> ToggleObjects =>
            this.LevelObjectives.Values.Select(s => s as IActivator).Where(a => a != null).ToList();

        public event EventHandler GameBeaten;
        public event EventHandler<ToggleEventArgs> Toggled;

        public void AddOrUpdateObjectives(List<ILevelObjective> objectives, string levelName, List<IActivateable> activateables = null)
        {
            foreach (var objective in objectives)
            {
                var key = GetKeyFromLevelObjective(objective, levelName);
                if (this.LevelObjectives.TryGetValue(key, out  ILevelObjective obj))
                {
                    this.LevelObjectives[key] = objective;
                }
                else
                {
                    this.LevelObjectives.Add(key, objective);
                }

                var activator = objective as IInteractiveLevelObjective;
                if (activator != null && activateables != null)
                {
                    activator.UpdateSelf(activateables);
                }
            }
        }

        public void UpdateWorldMapObjectives()
        {
            foreach (var item in LevelObjectives.Values)
            {
                if (item.ObjectiveComplete)
                {
                    var activator = item as IActivator;
                    if (activator != null)
                    {
                        activator.Toggle();
                    }
                }
            }

            if (this.LevelObjectivesComplete?.Invoke() ?? false)
            {
                this.GameBeaten?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
