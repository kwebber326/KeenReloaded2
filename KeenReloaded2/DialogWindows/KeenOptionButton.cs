using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Utilities;

namespace KeenReloaded2.DialogWindows
{
    public partial class KeenOptionButton : UserControl
    {
        private bool _selected;
        private int _currentImage;
        private Image[] _images = UserControlSprites.KeenOptionButtonImages;

        private Timer _timer;
        public KeenOptionButton(string text, bool selected)
        {
            InitializeComponent();
            _timer = new Timer();
            _timer.Interval = 300;
            _timer.Tick += _timer_Tick;
            this.Selected = selected;
            lblText.Text = text;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                if (_selected)
                {
                    _timer.Start();
                }
                else
                {
                    this.BackgroundImage = null;
                    _timer.Stop();
                }
            }
        }

        private void KeenOptionButton_Click(object sender, EventArgs e)
        {
            this.Selected = true;
        }

        private void KeenOptionButton_Load(object sender, EventArgs e)
        {


        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_currentImage >= _images.Length)
            {
                _currentImage = 0;
            }
            this.BackgroundImage = _images[_currentImage++];
        }
    }
}
