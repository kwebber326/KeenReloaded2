using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.ControlEventArgs.EventStoreData
{
    public class DoorSelectionChangedEventArgs : EventArgs
    {
       public ISprite NewDoor { get; set; }
    }
}
