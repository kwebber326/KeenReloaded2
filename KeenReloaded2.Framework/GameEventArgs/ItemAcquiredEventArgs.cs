using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Framework.GameEntities.Items;

namespace KeenReloaded2.Framework.GameEventArgs
{
    public class ItemAcquiredEventArgs : EventArgs
    {
        public Item Item { get; set; }
    }
}
