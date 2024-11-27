using KeenReloaded2.Entities.Statistics.HighScores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities.HighScoreFactory
{
    public interface IHighScoreUtility
    {
        bool WriteHighScore(Tuple<string, string> highScore, string mapName);

        List<Tuple<string, string>> ReadHighScores(string mapName);
    }
}
