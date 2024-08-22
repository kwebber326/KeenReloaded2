using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IActivateable : ISprite
    {
        void Activate();
        void Deactivate();
        bool IsActive { get; }

        Guid ActivationID { get; }
    }
}
