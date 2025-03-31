using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Utilities;
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
            _doors.RemoveAll(d => d is ExitDoor);
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            PublishSelectedDoorComplete();
            this.Close();
        }

        private void EditDestinationDoorForm_Load(object sender, EventArgs e)
        {
            if (_doors != null)
            {
                foreach (var door in _doors)
                {
                    lstDoors.Items.Add(door.Id);
                }
                if (_doorInQuestion?.DestinationDoorId != null)
                {
                    int destinationId = _doorInQuestion.DestinationDoorId.Value;
                    int? selectedItem = null;
                    foreach (var item in lstDoors.Items)
                    {
                        int? val = (int?)item;
                        if (val == destinationId)
                        {
                            selectedItem = val;
                        }
                    }
                    if (selectedItem != null)
                    {
                        lstDoors.SelectedItem = selectedItem;
                    }
                }
            }
        }

        private void LstDoors_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoorSelectionChangedEventArgs doorSelectionArgs = new DoorSelectionChangedEventArgs()
            {
                NewDoor = this.SelectedDoor
            };
            EventStore<DoorSelectionChangedEventArgs>.Publish(
                MapMakerConstants.EventStoreEventNames.EVENT_DOOR_SELECTION_CHANGED, doorSelectionArgs);
        }

        public Door SelectedDoor
        {
            get
            {
                if (_doors == null)
                    return null;

                if (lstDoors.SelectedIndex == -1)
                    return null;

                int selectedDoorId = (int)lstDoors.SelectedItem;
                Door door = _doors.FirstOrDefault(d => d.Id == selectedDoorId);
                return door;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            lstDoors.SelectedIndex = -1;
        }

        private void EditDestinationDoorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PublishSelectedDoorComplete();
        }

        private void PublishSelectedDoorComplete()
        {
            DoorSelectionChangedEventArgs selectionCompleteArgs = new DoorSelectionChangedEventArgs()
            {
                NewDoor = this.SelectedDoor
            };
            EventStore<DoorSelectionChangedEventArgs>.Publish(
               MapMakerConstants.EventStoreEventNames.EVENT_DOOR_SELECTION_COMPLETE,
               selectionCompleteArgs);
        }
    }
}
