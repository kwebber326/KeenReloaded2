using KeenReloaded2.Framework.GameEntities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEventArgs
{
    public class FlagCapturedEventArgs : EventArgs
    {
        public Flag Flag { get; set; }

        public EnemyFlag EnemyFlag { get; set; }
    }
}
