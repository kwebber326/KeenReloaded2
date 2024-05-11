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

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class AreaControl : UserControl
    {
        private Rectangle _area;
        private bool _locationEditEnabled;
        private bool _sizeEditEnabled;

        public AreaControl(Rectangle initialArea = default(Rectangle), bool locationEditEnabled = true, bool sizeEditEnabled = true)
        {
            _area = initialArea;
            _locationEditEnabled = locationEditEnabled;
            _sizeEditEnabled = sizeEditEnabled;
            InitializeComponent();
        }

        private void AreaControl_Load(object sender, EventArgs e)
        {
            txtXLocation.Enabled = _locationEditEnabled;
            txtYLocation.Enabled = _locationEditEnabled;
            txtHeight.Enabled = _sizeEditEnabled;
            txtWidth.Enabled = _sizeEditEnabled;

            txtXLocation.Text = _area.X.ToString();
            txtYLocation.Text = _area.Y.ToString();
            txtWidth.Text = _area.Width.ToString();
            txtHeight.Text = _area.Height.ToString();
        }

        public bool CanModifyLocation
        {
            get
            {
                return _locationEditEnabled;
            }
            set
            {
                _locationEditEnabled = value;
                txtXLocation.Text = _area.X.ToString();
                txtYLocation.Text = _area.Y.ToString();
            }
        }

        public bool CanModifySize
        {
            get
            {
                return _sizeEditEnabled;
            }
            set
            {
                _sizeEditEnabled = value;
                txtHeight.Enabled = _sizeEditEnabled;
                txtWidth.Enabled = _sizeEditEnabled;
            }
        }

        public  Rectangle Area
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
                txtXLocation.Text = _area.X.ToString();
                txtYLocation.Text = _area.Y.ToString();
                txtWidth.Text = _area.Width.ToString();
                txtHeight.Text = _area.Height.ToString();
            }
        }

        private void TxtXLocation_TextChanged(object sender, EventArgs e)
        {
            txtXLocation.Text = InputValidation.SanitizeStringForIntegerNumerics(txtXLocation.Text);
            if (int.TryParse(txtXLocation.Text, out int x))
                _area = new Rectangle(x, _area.Y, _area.Width, _area.Height);
        }

        private void TxtYLocation_TextChanged(object sender, EventArgs e)
        {
            txtYLocation.Text = InputValidation.SanitizeStringForIntegerNumerics(txtYLocation.Text);
            if (int.TryParse(txtYLocation.Text, out int y))
                _area = new Rectangle(_area.X, y, _area.Width, _area.Height);
        }

        private void TxtWidth_TextChanged(object sender, EventArgs e)
        {
            txtWidth.Text = InputValidation.SanitizeStringForIntegerNumerics(txtWidth.Text);
            if (int.TryParse(txtWidth.Text, out int width))
                _area = new Rectangle(_area.X, _area.Y, width, _area.Height);
        }

        private void TxtHeight_TextChanged(object sender, EventArgs e)
        {
            txtHeight.Text = InputValidation.SanitizeStringForIntegerNumerics(txtHeight.Text);
            if (int.TryParse(txtHeight.Text, out int height))
                _area = new Rectangle(_area.X, _area.Y, _area.Width, height);
        }
    }
}
