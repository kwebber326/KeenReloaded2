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

        public static Tuple<string, string> GenerateHighScoreInput(string gameMode, HighScore highScore)
        {
            try
            {
                switch (gameMode)
                {
                    case MainMenuConstants.OPTION_LABEL_NORMAL_MODE:
                        var normalScore = (NormalModeHighScore)highScore;
                        return new Tuple<string, string>(normalScore.PlayerName, normalScore.Time.ToString());
                    case MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE:
                        var zombieScore = (ZombieModeHighScore)highScore;
                        return new Tuple<string, string>(zombieScore.PlayerName, zombieScore.Score.ToString());
                    case MainMenuConstants.OPTION_LABEL_CTF_MODE:
                        var ctfScore = (CTFHighScore)highScore;
                        return new Tuple<string, string>(ctfScore.PlayerName, ctfScore.Score.ToString());
                    case MainMenuConstants.OPTION_LABEL_KOTH_MODE:
                        var kothScore = (KOTHHighScore)highScore;
                        return new Tuple<string, string>(kothScore.PlayerName, kothScore.Score.ToString());
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
