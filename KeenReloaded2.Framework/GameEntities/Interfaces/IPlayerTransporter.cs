using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IPlayerTransporter
    {
        int TransportSpeed { get; }

        int PlayerZIndexInFlight { get; }

        Point Destination { get; }

        void TransportPlayer(WorldMapPlayer player);

        bool IsActive { get; }

        WorldMapPlayerMoveState DesiredPlayerMoveState { get; }
    }
}
