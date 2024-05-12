using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KeenReloaded2.ControlEventArgs
{
    public class AreaControlEventArgs : EventArgs
    {
        public Rectangle Area { get; set; }
    }
}
