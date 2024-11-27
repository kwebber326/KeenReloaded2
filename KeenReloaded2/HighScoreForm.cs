using KeenReloaded2.Entities.Statistics.HighScores;
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
        List<HighScore> _highScores = new List<HighScore>();

        public HighScoreForm()
        {
            InitializeComponent();
            _closeTimer.Interval = 2000;
            _closeTimer.Tick += _closeTimer_Tick;
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
    }
}
