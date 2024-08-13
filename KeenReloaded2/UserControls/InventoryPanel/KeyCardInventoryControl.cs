using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class KeyCardInventoryControl : UserControl
    {
        public KeyCardInventoryControl()
        {
            InitializeComponent();
        }

        private void KeyCardInventoryControl_Load(object sender, EventArgs e)
        {

        }

        public void AddKeyCard()
        {
            pbDisplay.Image = Properties.Resources.key_card_acquired;
        }

        public void RemoveKeyCard()
        {
            pbDisplay.Image = Properties.Resources.key_card_not_acquired;
        }
    }
}
