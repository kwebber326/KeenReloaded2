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
        HighScore _newHighScore;
        private string _gameMode;
        private readonly IHighScoreUtility _highScoreUtility;

        public HighScoreForm(string gameMode)
        {
            InitializeComponent();
            _gameMode = gameMode;
            _closeTimer.Interval = 2000;
            _closeTimer.Tick += _closeTimer_Tick;
            _highScoreUtility = HighScoreFactory.Generate(_gameMode);
        }

        public HighScoreForm(HighScore highScore, string gameMode)
        {
            InitializeComponent();
            _newHighScore = highScore;
            _gameMode = gameMode;
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
            
        }
    }
}
