using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IFlag
    {
        int PointsDegradedPerSecond { get; }

        bool IsCaptured { get; }

        Point LocationOfOrigin { get; }

        event EventHandler<FlagCapturedEventArgs> FlagCaptured;
        event EventHandler<FlagCapturedEventArgs> FlagPointsChanged;

        void Capture();
    }
}
