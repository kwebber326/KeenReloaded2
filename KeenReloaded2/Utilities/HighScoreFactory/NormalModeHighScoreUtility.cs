using KeenReloaded2.Entities.Statistics.HighScores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities.HighScoreFactory
{
    public class NormalModeHighScoreUtility : IHighScore<NormalModeHighScore>
    {
        public NormalModeHighScoreUtility()
        {
        }

        public List<NormalModeHighScore> ReadHighScores()
        {
            return new List<NormalModeHighScore>();
        }

        public bool WriteHighScore(NormalModeHighScore highScore)
        {
            return false;
        }
    }
}
