using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.Interfaces
{
    public interface ICreateRemove
    {
        event EventHandler<ObjectEventArgs> Create;
        event EventHandler<ObjectEventArgs> Remove;
    }
}
