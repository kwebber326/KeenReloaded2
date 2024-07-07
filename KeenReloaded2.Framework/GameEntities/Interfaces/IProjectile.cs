using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IProjectile : IMoveable
    {
        int Damage { get; }

        int Velocity { get; }

        int Pierce { get; }

        int Spread { get; }

        int BlastRadius { get; }

        int RefireDelay { get; }

        bool KillsKeen { get; }
    }
}
