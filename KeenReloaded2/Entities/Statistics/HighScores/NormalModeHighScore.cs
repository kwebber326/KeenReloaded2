using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics.HighScores
{
    public class NormalModeHighScore : IHighScore
    {
        public NormalModeHighScore(string playerName, string mapName, TimeSpan time)
        {
            this.Time = time;
            this.MapName = mapName;
            this.PlayerName = playerName;
        }
        public TimeSpan Time { get; private set; }
        public string PlayerName { get; set; }
        public string MapName { get; private set; }
        public object Value
        {
            get
            {
                return this.Time;
            }
        }
    }
}
