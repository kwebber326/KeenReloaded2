using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IInteractiveLevelObjective : ILevelObjective
    {
        void UpdateSelf(params object[] objects);

        List<Guid> WorldMapActivationIds { get; }

        void UpdateGame();
    }
}
