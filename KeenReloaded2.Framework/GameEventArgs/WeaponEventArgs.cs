using KeenReloaded2.Framework.GameEntities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEventArgs
{
    public class WeaponEventArgs : EventArgs
    {
        public NeuralStunner Weapon { get; set; }
    }
}
