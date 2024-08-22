using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.ControlEventArgs.EventStoreData
{
    public class ActivatorSelectionCompletedEventArgs : EventArgs
    {
        public List<IActivateable> Activateables { get; set; }
    }
}
