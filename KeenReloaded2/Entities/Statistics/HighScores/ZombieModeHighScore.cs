using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics.HighScores
{
    public class ZombieModeHighScore : IHighScore
    {
        public ZombieModeHighScore(string playerName, string mapName, long score)
        {
            this.Score = score;
            this.MapName = mapName;
            this.PlayerName = playerName;
        }
        public long Score { get; private set; }
        public string PlayerName { get; set; }
        public string MapName { get; private set; }
        public object Value
        {
            get
            {
                return this.Score;
            }
        }
    }
}
