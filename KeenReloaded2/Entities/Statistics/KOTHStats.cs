using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics
{
    public class KOTHStats
    {
        public int GamesPlayed { get; set; }
        public long TotalPointsFromHill { get; set; }

        public long BonusPointsFromMonsters { get; set; }

        public long TotalPointsScored { get; set; }

        public long GetAveragePointsPerGame()
        {
            if (GamesPlayed == 0)
                return 0;

            return TotalPointsScored / GamesPlayed;
        }
    }
}
