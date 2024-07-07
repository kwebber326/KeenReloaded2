using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    interface IFireable
    {
        void Fire();

        bool IsFiring { get; }

        int Ammo { get; }
    }
}
