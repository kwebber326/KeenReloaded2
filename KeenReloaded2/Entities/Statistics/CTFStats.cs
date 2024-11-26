using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics
{
    public class CTFStats : PlayerStats
    {
        public int TotalFlagsCaptured { get; set; }

        public int TotalFlagsAcquired { get; set; }

        public long TotalPointsFromFlags { get; set; }

        public long TotalPointsDeductedFromEnemyFlagCapture { get; set; }

        public long TotalPointsDeductedFromFlagUncaptured { get; set; }

        public int EnemyBlackFlagForcedFumbles { get; set; }

        public long GetAveragePointsPerFlag()
        {
            if (TotalFlagsCaptured == 0)
                return 0;

            return TotalPointsFromFlags / TotalFlagsCaptured;
        }
    }
}
