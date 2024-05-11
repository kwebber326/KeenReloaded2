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
        private readonly bool _locationEditEnabled;
        private readonly bool _sizeEditEnabled;

        public AreaControl()
        {
            InitializeComponent();
        }

        public AreaControl(Rectangle initialArea, bool locationEditEnabled, bool sizeEditEnabled)
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

        private void TxtXLocation_TextChanged(object sender, EventArgs e)
        {
            InputValidation.SanitizeStringForIntegerNumerics(txtXLocation.Text);
        }

        private void TxtYLocation_TextChanged(object sender, EventArgs e)
        {
            InputValidation.SanitizeStringForIntegerNumerics(txtYLocation.Text);
        }

        private void TxtWidth_TextChanged(object sender, EventArgs e)
        {
            InputValidation.SanitizeStringForIntegerNumerics(txtWidth.Text);
        }

        private void TxtHeight_TextChanged(object sender, EventArgs e)
        {
            InputValidation.SanitizeStringForIntegerNumerics(txtHeight.Text);
        }
    }
}
