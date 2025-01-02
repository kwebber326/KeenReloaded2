using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IHangableTile
    {
        bool Hangable { get; }

        Rectangle HitBox { get; }
    }
}
