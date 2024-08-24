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
        Door _doorInQuestion;
        public EditDestinationDoorForm(List<Door> doors, Door doorInQuestion)
        {
            InitializeComponent();
            _doors = doors;
            _doorInQuestion = doorInQuestion;
            _doors.Remove(_doorInQuestion);
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void EditDestinationDoorForm_Load(object sender, EventArgs e)
        {
            foreach (var door in _doors)
            {
                lstDoors.Items.Add(door.Id);
            }
        }

        private void LstDoors_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public Door SelectedDoor
        {
            get
            {
                if (lstDoors.SelectedIndex == -1)
                    return null;

                int selectedDoorId = (int)lstDoors.SelectedItem;
                Door door = _doors.FirstOrDefault(d => d.Id == selectedDoorId);
                return door;
            }
        }
    }
}
