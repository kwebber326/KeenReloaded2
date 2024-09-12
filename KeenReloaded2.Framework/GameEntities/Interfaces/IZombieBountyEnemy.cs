using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IZombieBountyEnemy
    {
        PointItemType PointItem { get; }

        event EventHandler<ObjectEventArgs> Killed;
    }
}
