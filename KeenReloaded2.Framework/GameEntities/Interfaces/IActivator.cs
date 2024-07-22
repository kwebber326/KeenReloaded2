using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IActivator
    {
        List<IActivateable> ToggleObjects { get; }

        bool IsActive { get; }

        void Toggle();

        event EventHandler<ToggleEventArgs> Toggled;
    }
}
