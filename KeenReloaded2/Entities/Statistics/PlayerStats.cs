using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics
{
    public abstract class PlayerStats
    {
        public string PlayerName { get; set; }

        public string MapName { get; set; }

        public int Deaths { get; set; }

        public int Quits { get; set; }
    }
}
