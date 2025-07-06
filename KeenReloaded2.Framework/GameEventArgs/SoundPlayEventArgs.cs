using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEventArgs
{
    public class SoundPlayEventArgs : EventArgs
    {
        public string Sound { get; set; }

        public Point? SenderPosition { get; set; }
    }
}
