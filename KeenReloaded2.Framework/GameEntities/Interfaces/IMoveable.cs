using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IMoveable
    {
        void Move();

        void Stop();

        MoveState MoveState { get; set; }

        Direction Direction { get; set; }

    }
}
