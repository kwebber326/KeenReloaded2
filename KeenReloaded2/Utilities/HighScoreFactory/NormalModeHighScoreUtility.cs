using KeenReloaded2.Entities.Statistics.HighScores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities.HighScoreFactory
{
    public class NormalModeHighScoreUtility : IHighScoreUtility
    {
        public List<IHighScore> ReadHighScores(string mapName)
        {
            return new List<IHighScore>()
            {
                new NormalModeHighScore("test1", mapName, TimeSpan.FromMinutes(2.2)),
                new NormalModeHighScore("test2", mapName, TimeSpan.FromMinutes(5.0))
            };
        }

        public bool WriteHighScores(List<IHighScore> highScores, string mapName)
        {
            return false;
        }
    }
}
