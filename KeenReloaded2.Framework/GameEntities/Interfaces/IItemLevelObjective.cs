using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IItemLevelObjective : ILevelObjective
    {
        WorldMapItemType ItemType { get; }

        void Acquire();
    }
}
