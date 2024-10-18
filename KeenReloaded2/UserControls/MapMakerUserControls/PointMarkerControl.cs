using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class PointMarkerControl : UserControl
    {
        int _ordinalPosition;
        public PointMarkerControl()
        {
            InitializeComponent();
        }

        public PointMarkerControl(int ordinalPosition)
        {
            _ordinalPosition = ordinalPosition;
            InitializeComponent();
        }

        private void PointMarkerControl_Load(object sender, EventArgs e)
        {
            lblOrdinalPosition.Text = _ordinalPosition.ToString();
        }
    }
}
