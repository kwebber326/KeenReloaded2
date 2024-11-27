using KeenReloaded2.Entities.Statistics.HighScores;
using KeenReloaded2.Utilities.HighScoreFactory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2
{
    public partial class HighScoreForm : Form
    {
        private bool _canClose = false;
        Timer _closeTimer = new Timer();
        IHighScore _newHighScore;
        private readonly string _gameMode;
        private readonly string _mapName;
        private readonly IHighScoreUtility _highScoreUtility;

        public HighScoreForm(string gameMode, string mapName)
        {
            InitializeComponent();
            _gameMode = gameMode;
            _mapName = mapName;
            _closeTimer.Interval = 2000;
            _closeTimer.Tick += _closeTimer_Tick;
            _highScoreUtility = HighScoreFactory.Generate(_gameMode);
        }

        public HighScoreForm(IHighScore highScore, string gameMode, string mapName)
        {
            InitializeComponent();
            _newHighScore = highScore;
            _gameMode = gameMode;
            _mapName = mapName;
            _closeTimer.Interval = 2000;
            _closeTimer.Tick += _closeTimer_Tick;
            _highScoreUtility = HighScoreFactory.Generate(_gameMode);
        }

        private void _closeTimer_Tick(object sender, EventArgs e)
        {
            _canClose = true;
        }

        private void HighScoreForm_Load(object sender, EventArgs e)
        {
            WriteHighScoresToBoard();
            this.Size = pbHighScoreImage.Size;
            _closeTimer.Start();
        }

        private void HighScoreForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (_canClose)
            {
                this.Close();
            }
        }

        private void WriteHighScoresToBoard()
        {
            try
            {
                //get high score list
                var highScores = _highScoreUtility.ReadHighScores(_mapName);
                if (_newHighScore != null)
                {
                    //insert new high score to list and determine if the player achieved a high score
                    highScores.Add(_newHighScore);
                    var min = highScores.Min(h => h.Value);
                    var firstMin = highScores.FirstOrDefault(h => h.Value == min);
                    highScores.Remove(firstMin);
                    //if the player achieved a high score, prompt the user to enter their name
                    if (highScores.Contains(_newHighScore))
                    {
                        //TODO: use prompt to get their name and write the new list to the scores
                    }
                    //TODO: write player stats
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                MessageBox.Show("Error retrieving high scores");
            }
        }
    }
}
