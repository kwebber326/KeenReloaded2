using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEventArgs
{
    public class ObjectEventArgs : EventArgs
    {
        public ISprite ObjectSprite { get; set; }
    }
}
