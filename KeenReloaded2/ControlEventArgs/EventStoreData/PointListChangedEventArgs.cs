using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.ControlEventArgs.EventStoreData
{
    public class PointListChangedEventArgs : EventArgs
    {
        public List<Point> NewPoints { get; set; }
    }
}
