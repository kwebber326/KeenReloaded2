﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities.Statistics.HighScores
{
    public interface IHighScore
    {
        string PlayerName { get; set; }

        string MapName { get; }

        object Value { get; }
    }
}
