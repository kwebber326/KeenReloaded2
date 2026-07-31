using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class WorldMapObjectiveManager
    {
        public static string GetKeyFromLevelObjective(ISprite item, string levelName)
        {
            string name = item.GetType().Name;
            Point location = item?.Location ?? new Point(0, 0);
            string key = levelName + "_" + name + "_" + location.ToString();
            return key;
        }

        public WorldMapObjectiveData LevelObjectives { get; set; } = new WorldMapObjectiveData();
        public bool ObjectivesComplete()
        {
            return LevelObjectives.EndGoals.Values.All(l => l.Completed);
        }

        public event EventHandler GameBeaten;

        public void AddOrUpdateObjectives(List<ISprite> objectives, string levelName, WorldMapLevelObjectiveType type, params object[] argData)
        {
            foreach (var objective in objectives)
            {
                var key = GetKeyFromLevelObjective(objective, levelName);
                switch (type)
                {
                    case WorldMapLevelObjectiveType.ACTIVATOR:
                        if (this.LevelObjectives.Activators.TryGetValue(key, out ActivatorWorldMapObjective activator))
                        {
                            this.LevelObjectives.Activators[key] = WorldMapObjectiveFactory.BuildWorldMapObjective(type, argData) as ActivatorWorldMapObjective;
                        }
                        else
                        {
                            this.LevelObjectives.Activators.Add(key, WorldMapObjectiveFactory.BuildWorldMapObjective(type, argData) as ActivatorWorldMapObjective);
                        }
                        break;
                    case WorldMapLevelObjectiveType.END_GOAL:
                        if (this.LevelObjectives.EndGoals.TryGetValue(key, out EndGoalWorldMapObjective endGoal))
                        {
                            this.LevelObjectives.EndGoals[key] = WorldMapObjectiveFactory.BuildWorldMapObjective(type, argData) as EndGoalWorldMapObjective;
                        }
                        else
                        {
                            this.LevelObjectives.EndGoals.Add(key, WorldMapObjectiveFactory.BuildWorldMapObjective(type, argData) as EndGoalWorldMapObjective);
                        }
                        break;

                    case WorldMapLevelObjectiveType.ITEM:
                        if (this.LevelObjectives.Items.TryGetValue(key, out ItemWorldMapObjective item))
                        {
                            this.LevelObjectives.Items[key] = WorldMapObjectiveFactory.BuildWorldMapObjective(type, argData) as ItemWorldMapObjective;
                        }
                        else
                        {
                            this.LevelObjectives.Items.Add(key, WorldMapObjectiveFactory.BuildWorldMapObjective(type, argData) as ItemWorldMapObjective);
                        }
                        break;
                }

            }
        }

        public void UpdateWorldMapObjectives(string levelName, WorldMapPlayer player = null, List<ISprite> relevantObjectives = null, List<IActivateable> activateables = null)
        {
            if (relevantObjectives == null)
                return;


            foreach (var objective in relevantObjectives)
            {
                var key = GetKeyFromLevelObjective(objective, levelName);
                if (this.LevelObjectives.EndGoals.TryGetValue(key, out var endGoal))
                {
                    endGoal.Completed = true;
                }
                if (this.LevelObjectives.Activators.TryGetValue(key, out var activator))
                {
                    activator.Completed = true;
                    if (activateables != null)
                    {
                        var relevantActivateables = activateables.Where(
                            g => activator.WorldMapComponents.Contains(g.ActivationID)).ToList();
                        foreach (var activateable in relevantActivateables)
                        {
                            activateable.Deactivate();
                        }
                    }
                }
                if (player != null && this.LevelObjectives.Items.TryGetValue(key, out var item))
                {
                    //TODO: change world map player to have items
                }
            }

            if (this.ObjectivesComplete())
            {
                this.GameBeaten?.Invoke(this, EventArgs.Empty);
            }
        }

        public static WorldMapObjectiveManager FromString(string data)
        {
            try
            {
                WorldMapObjectiveData worldMapData =
                    JsonConvert.DeserializeObject<WorldMapObjectiveData>(data);

                WorldMapObjectiveManager mapObjectiveData = new WorldMapObjectiveManager();
                mapObjectiveData.LevelObjectives = worldMapData;

                return mapObjectiveData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this.LevelObjectives);
        }
    }

    #region Helper Types

    public class WorldMapObjectiveData
    {
        public Dictionary<string, EndGoalWorldMapObjective> EndGoals { get; set; } = new Dictionary<string, EndGoalWorldMapObjective>();

        public Dictionary<string, ActivatorWorldMapObjective> Activators { get; set; } = new Dictionary<string, ActivatorWorldMapObjective>();

        public Dictionary<string, ItemWorldMapObjective> Items { get; set; } = new Dictionary<string, ItemWorldMapObjective>();
    }

    public static class WorldMapObjectiveFactory
    {
        public static IWorldMapObjective BuildWorldMapObjective(WorldMapLevelObjectiveType type, params object[] data)
        {
            switch (type)
            {
                case WorldMapLevelObjectiveType.ACTIVATOR:
                    var activator = new ActivatorWorldMapObjective();
                    activator.Build(data);
                    return activator;
                case WorldMapLevelObjectiveType.ITEM:
                    var item = new ItemWorldMapObjective();
                    item.Build(data);
                    return item;
                case WorldMapLevelObjectiveType.END_GOAL:
                    var endGoal = new EndGoalWorldMapObjective();
                    endGoal.Build(data);
                    return endGoal;
            }

            return null;
        }
    }

    public interface IWorldMapObjective
    {
        WorldMapLevelObjectiveType Type { get; }

        bool Completed { get; set; }

        void Build(params object[] args);
    }

    public class ActivatorWorldMapObjective : IWorldMapObjective
    {
        public WorldMapLevelObjectiveType Type => WorldMapLevelObjectiveType.ACTIVATOR;

        public List<Guid> WorldMapComponents { get; set; }
        public bool Completed { get; set; }

        public void Build(params object[] args)
        {
            if (!args.Any() || !(args[0] is IEnumerable<Guid>))
            {
                throw new ArgumentException("Invalid data given. Need a collection of guids");
            }

            IEnumerable<Guid> guids = args[0] as IEnumerable<Guid>;
            this.WorldMapComponents = guids.ToList();
        }
    }

    public class ItemWorldMapObjective : IWorldMapObjective
    {
        public WorldMapLevelObjectiveType Type => WorldMapLevelObjectiveType.ITEM;

        public bool Completed { get; set; }

        public WorldMapItemType ItemType { get; set; }

        public void Build(params object[] args)
        {
            if (!args.Any() || !(args[0] is WorldMapItemType))
                throw new ArgumentException("Invalid data.  Need WorldMapItemType");

            this.ItemType = (WorldMapItemType)args[0];
        }
    }

    public class EndGoalWorldMapObjective : IWorldMapObjective
    {
        public WorldMapLevelObjectiveType Type => WorldMapLevelObjectiveType.END_GOAL;

        public bool Completed { get; set; }

        public void Build(params object[] args)
        {

        }
    }

    public enum WorldMapItemType
    {
        SWIMSUIT,
        SANDWICH
    }

    public enum WorldMapLevelObjectiveType
    {
        ACTIVATOR,
        ITEM,
        END_GOAL
    }

    #endregion
}
