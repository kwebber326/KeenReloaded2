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
using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.ControlEventArgs.EventStoreData;

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
            EventStore<int>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_SELECTED_INDEX_CHANGED, PathwayForm_SelectedIndex_Changed);
        }

        public void PathwayForm_SelectedIndex_Changed(object sender, ControlEventArgs<int> args)
        {
            var index = args.Data;
            if (index == _ordinalPosition)
            {
                this.BorderStyle = BorderStyle.Fixed3D;
                this.BackColor = Color.Gold;
                this.BringToFront();
            }
            else
            {
                this.BorderStyle = BorderStyle.None;
                this.BackColor = Color.Transparent;
            }
        }

        private void PointMarkerControl_Load(object sender, EventArgs e)
        {
            lblOrdinalPosition.Text = _ordinalPosition.ToString();
        }
    }
}
