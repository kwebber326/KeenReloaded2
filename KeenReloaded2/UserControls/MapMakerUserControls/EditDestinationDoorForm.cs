using KeenReloaded2.Framework.GameEntities.Constructs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class EditDestinationDoorForm : Form
    {
        List<Door> _doors = new List<Door>();
        public EditDestinationDoorForm(List<Door> doors)
        {
            InitializeComponent();
            _doors = doors;
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {

        }

        private void EditDestinationDoorForm_Load(object sender, EventArgs e)
        {

        }
    }
}
