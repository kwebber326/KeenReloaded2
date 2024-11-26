using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics
{
    public class ZombieModeStats : PlayerStats
    {
        public int Kills { get; set; }

        public int GamesPlayed { get; set; }

        public long TotalPointsScored { get; set; }

        public long GetAveragePointsPerGame()
        {
            if (GamesPlayed == 0)
                return 0;

            return TotalPointsScored / GamesPlayed;
        }

        public string GetKDRatio()
        {
            double kills = (double)this.Kills;
            double deaths = (double)this.Deaths;

            if (deaths == 0)
                return "infinite";

            if (kills == 0)
                return $"0/{this.Deaths}";

            double kdRatio = kills / deaths;
            return $"{Math.Round(kdRatio, 4)}/1";
        }
    }
}
