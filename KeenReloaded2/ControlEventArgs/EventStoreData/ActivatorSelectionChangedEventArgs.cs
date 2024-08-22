using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.ControlEventArgs.EventStoreData
{
    public class ActivatorSelectionChangedEventArgs : EventArgs
    {
       public List<IActivateable> CurrentActiveablesUnSelected { get; set; }

       public List<IActivateable> CurrentActivateablesSelected { get; set; }

        public List<IActivateable> OtherActiveablesUnSelected { get; set; }

        public List<IActivateable> OtherActivateablesSelected { get; set; }
    }
}
