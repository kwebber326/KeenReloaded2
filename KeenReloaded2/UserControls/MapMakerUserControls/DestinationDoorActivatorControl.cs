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
    public partial class DestinationDoorActivatorControl : UserControl
    {
        public event EventHandler DoorSelect;
        public DestinationDoorActivatorControl()
        {
            InitializeComponent();
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            this.DoorSelect?.Invoke(this, e);
        }
    }
}
