using KeenReloaded2.Framework.GameEntities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities
{
    public static class CurrentPlayerList
    {
        private static List<CommanderKeen> _players;

        public static List<CommanderKeen> Players
        {
            get
            {
                if (_players == null)
                {
                    _players = new List<CommanderKeen>();
                }
                return _players;
            }
        }
    }
}
