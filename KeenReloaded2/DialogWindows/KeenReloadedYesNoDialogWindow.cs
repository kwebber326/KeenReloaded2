using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows.Forms;
using System.Drawing;

namespace KeenReloaded2.DialogWindows
{
  
    public class KeenReloadedYesNoDialogWindow : KeenReloadedMessageWindow
    {
        private KeenOptionButton _yesButton, _noButton;

        public KeenReloadedYesNoDialogWindow(string text, bool defaultYes) : base(text)
        {
            _yesButton = new KeenOptionButton("Yes", defaultYes);
            _yesButton.Location = new Point(180, 100);
            _yesButton.Click += _yesButton_Click;

            _noButton = new KeenOptionButton("No", !defaultYes);
            _noButton.Location = new Point(280, 100);
            _noButton.Click += _noButton_Click;

            this.Controls.Add(_noButton);
            this.Controls.Add(_yesButton);
            _noButton.BringToFront();
            _yesButton.BringToFront();

            this.KeyDown += KeenReloadedYesNoDialogWindow_KeyDown;
            this.KeyUp -= KeenReloadedMessageWindow_KeyUp;
        }
        private void KeenReloadedYesNoDialogWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (_noButton.Selected)
                    {
                        _noButton.Selected = false;
                        _yesButton.Selected = true;
                    }
                    break;
                case Keys.Right:
                    if (_yesButton.Selected)
                    {
                        _noButton.Selected = true;
                        _yesButton.Selected = false;
                    }
                    break;
                case Keys.Enter:
                    if (_yesButton.Selected)
                    {
                        _yesButton_Click(this, EventArgs.Empty);
                    }
                    else
                    {
                        _noButton_Click(this, EventArgs.Empty);
                    }
                    break;
            }
        }

        private void _noButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void _yesButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void KeenReloadedYesNoDialogWindow_Load(object sender, EventArgs e)
        {

        }

        protected override bool IsInputKey(Keys keyData)
        {
            return base.IsInputKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Enter:
                    KeyEventArgs args = new KeyEventArgs(keyData);
                    base.OnKeyDown(args);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
