using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.DialogWindows
{
    public partial class KeenReloadedMessageWindow : Form
    {
        protected string _messageText;
        private int _messageCloseDelay;
        private bool _canClose = true;
        Timer _delayTimer;

        public KeenReloadedMessageWindow()
        {
            InitializeComponent();
        }

        public KeenReloadedMessageWindow(string messageText, int messageCloseDelayMs = -1)
        {
            InitializeComponent();
            _messageCloseDelay = messageCloseDelayMs;
            if (_messageCloseDelay > 0)
            {
                _delayTimer = new Timer();
                _delayTimer.Interval = _messageCloseDelay;
                _delayTimer.Tick += _delayTimer_Tick;
                _canClose = false;
                _delayTimer.Start();
            }
            _messageText = messageText;
        }

        private void _delayTimer_Tick(object sender, EventArgs e)
        {
            _canClose = true;
            _delayTimer.Stop();
        }

        protected virtual void KeenReloadedMessageWindow_Load(object sender, EventArgs e)
        {
            lblText.Text = _messageText;
        }
        protected virtual void KeenReloadedMessageWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (_canClose)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
