using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Framework.GameEntities.Interfaces;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class ActivatorListControl : UserControl
    {
        public event EventHandler EditItemsClicked;
        public ActivatorListControl()
        {
            InitializeComponent();
        }

        public List<IActivateable> Activateables
        {
            get;set;
        }

        private void ActivatorListControl_Load(object sender, EventArgs e)
        {

        }

        private void BtnEditItems_Click(object sender, EventArgs e)
        {
            this.EditItemsClicked?.Invoke(this, e);
        }
    }
}
