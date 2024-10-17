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
    public partial class PointSelectorUserControl : UserControl
    {
        public event EventHandler EditItemsClicked;
        public PointSelectorUserControl()
        {
            InitializeComponent();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            this.EditItemsClicked?.Invoke(this, e);
        }
    }
}
