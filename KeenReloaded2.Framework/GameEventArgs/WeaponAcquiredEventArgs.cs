using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEventArgs
{
    public class WeaponAcquiredEventArgs : EventArgs
    {
        public object Weapon { get; set; }
    }
}
