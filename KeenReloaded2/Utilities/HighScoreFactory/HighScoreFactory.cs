using KeenReloaded2.Constants;
using KeenReloaded2.Entities.Statistics.HighScores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities.HighScoreFactory
{
    public static class HighScoreFactory
    {
        public static IHighScoreUtility Generate(string gameMode)
        {
            switch (gameMode)
            {
                case MainMenuConstants.OPTION_LABEL_NORMAL_MODE:
                    return new NormalModeHighScoreUtility();
                case MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE:
                    return new ZombieModeHighScoreUtility();
                case MainMenuConstants.OPTION_LABEL_CTF_MODE:
                    return new CTFHighScoreUtility();
                case MainMenuConstants.OPTION_LABEL_KOTH_MODE:
                    return new KOTHHighScoreUtility();
            }

            return null;
        }
    }
}
