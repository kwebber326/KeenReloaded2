using KeenReloaded2.Constants;
using KeenReloaded2.Entities.Statistics.HighScores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities.HighScoreFactory
{
    public class CTFHighScoreUtility : IHighScoreUtility
    {
        public List<IHighScore> GetSortedList(List<IHighScore> highScores)
        {
            return highScores.OrderByDescending(v => v.Value).ToList();
        }

        public List<IHighScore> ReadHighScores(string mapName)
        {
            var items = FileIOUtility.ReadHighScoresByGameModeAndLevel(MainMenuConstants.OPTION_LABEL_CTF_MODE, mapName);
            List<IHighScore> highScores = items
                .Select(i => (IHighScore)new CTFHighScore(i.Item1, mapName, long.Parse(i.Item2)))
                .ToList();
            return highScores;
        }

        public bool WriteHighScores(List<IHighScore> highScores, string mapName)
        {
            try
            {
                var data = highScores
                    .Select(h => new Tuple<string, string>(h.PlayerName, h.Value?.ToString()))
                    .ToList();
                FileIOUtility.WriteHighScoresByGameMode(MainMenuConstants.OPTION_LABEL_CTF_MODE, mapName, data);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
