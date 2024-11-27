using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics.HighScores
{
    public interface IHighScore
    {
        string PlayerName { get; }

        string MapName { get; }

        object Value { get; }
    }
}
