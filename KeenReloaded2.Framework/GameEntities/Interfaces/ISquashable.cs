using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface ISquashable
    {
        void Squash();
        bool IsSquashed { get; }
        bool CanSquash { get; }
        event EventHandler<ObjectEventArgs> Squashed;
    }
}
