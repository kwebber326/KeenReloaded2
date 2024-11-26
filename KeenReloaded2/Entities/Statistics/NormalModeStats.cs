using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics
{
    public class NormalModeStats : PlayerStats
    {
        public int LevelsCleared { get; set; }

        public int GameOvers { get; set; }

        public string GetAverageTimeToCompletion()
        {
            if (this.FinishTimes == null  || !this.FinishTimes.Any())
            {
                return "never completed";
            }

           double average = this.FinishTimes.Average(t => (double)t.Ticks);

            TimeSpan timeSpan = new TimeSpan((long)average);
            return timeSpan.ToString();
        }

        public string GetFastestTime()
        {
            if (this.FinishTimes == null || !this.FinishTimes.Any())
            {
                return "never completed";
            }

            long lowestTicks = this.FinishTimes.Min(t => t.Ticks);
            TimeSpan fastest = this.FinishTimes.FirstOrDefault(s => s.Ticks == lowestTicks);

            return fastest.ToString();
        }

        public string GetSlowestTime()
        {
            if (this.FinishTimes == null || !this.FinishTimes.Any())
            {
                return "never completed";
            }

            long mostTicks = this.FinishTimes.Max(t => t.Ticks);
            TimeSpan fastest = this.FinishTimes.FirstOrDefault(s => s.Ticks == mostTicks);

            return fastest.ToString();
        }


        List<TimeSpan> FinishTimes { get; set; }
    }
}
