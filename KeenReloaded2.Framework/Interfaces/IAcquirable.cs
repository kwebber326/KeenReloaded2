using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.Interfaces
{
    public interface IAcquirable
    {
        event EventHandler Acquired;
        bool IsAcquired { get; set; }
    }
}
