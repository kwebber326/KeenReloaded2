using KeenReloaded2.Entities.Statistics.HighScores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities.HighScoreFactory
{
    public class ZombieModeHighScoreUtility : IHighScoreUtility
    {
        public List<Tuple<string, string>> ReadHighScores(string mapName)
        {
            throw new NotImplementedException();
        }

        public bool WriteHighScore(Tuple<string, string> highScore, string mapName)
        {
            throw new NotImplementedException();
        }
    }
}
