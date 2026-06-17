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

        public KeenReloadedMessageWindow()
        {
            InitializeComponent();
        }

        public KeenReloadedMessageWindow(string messageText)
        {
            InitializeComponent();
            _messageText = messageText;
        }

        protected virtual void KeenReloadedMessageWindow_Load(object sender, EventArgs e)
        {
            lblText.Text = _messageText;
        }
        protected virtual void KeenReloadedMessageWindow_KeyUp(object sender, KeyEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
