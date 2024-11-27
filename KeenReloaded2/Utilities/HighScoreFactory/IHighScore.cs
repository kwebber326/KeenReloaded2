using KeenReloaded2.Entities.Statistics.HighScores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities.HighScoreFactory
{
    public interface IHighScore<T> where T : HighScore
    {
        bool WriteHighScore(T highScore);

        List<T> ReadHighScores();
    }
}
