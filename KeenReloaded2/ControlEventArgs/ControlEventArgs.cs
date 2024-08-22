using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.ControlEventArgs
{
    public class ControlEventArgs<T> : EventArgs
    {
        public string EventName { get; set; }
        public T Data { get; set; }
    }
}
