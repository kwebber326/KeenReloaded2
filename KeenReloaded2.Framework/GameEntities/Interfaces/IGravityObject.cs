using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IGravityObject
    {
        void Jump();

        bool CanJump { get; }

        void Fall();
    }
}
