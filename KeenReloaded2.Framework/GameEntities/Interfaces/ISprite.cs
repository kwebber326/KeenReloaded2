using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface ISprite
    {
        int ZIndex { get; }

        Image Image { get; }

        Point Location { get; }

        bool CanUpdate { get; }
    }
}
