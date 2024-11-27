using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics.HighScores
{
    public abstract class HighScore
    {
        public string PlayerName { get; set; }

        public string MapName { get; set; }
    }
}
