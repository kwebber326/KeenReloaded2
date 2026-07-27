using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IWorldMapLevel : IActivateable
    {
        event EventHandler WorldMapEntered;

        string LevelName { get; }

        string LevelEntryText { get; }

        string Episode { get; }

        string Music { get; }
    }
}
