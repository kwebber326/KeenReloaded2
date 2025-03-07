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
    public partial class EditDestinationNodeForm : Form
    {
        List<EnemyTransporter> _nodes = new List<EnemyTransporter>();
        EnemyTransporter _nodeInQuestion;

        public EditDestinationNodeForm(List<EnemyTransporter> nodes, EnemyTransporter nodeInQuestion)
        {
            InitializeComponent();
            _nodes = nodes;
            _nodeInQuestion = nodeInQuestion;
            _nodes.Remove(_nodeInQuestion);
        }


        private void BtnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            DoorSelectionChangedEventArgs selectionCompleteArgs = new DoorSelectionChangedEventArgs()
            {
                NewDoor = this.SelectedNode
            };
            EventStore<DoorSelectionChangedEventArgs>.Publish(
                MapMakerConstants.EventStoreEventNames.EVENT_NODE_SELECTION_COMPLETE,
                selectionCompleteArgs);

            this.Close();
        }

        private void EditDestinationDoorForm_Load(object sender, EventArgs e)
        {
            if (_nodes != null)
            {
                foreach (var node in _nodes)
                {
                    lstDoors.Items.Add(node.Id);
                }
                if (_nodeInQuestion?.DestinationNodeId != null)
                {
                    int destinationId = _nodeInQuestion.DestinationNodeId.Value;
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
            DoorSelectionChangedEventArgs nodeSelectionArgs = new DoorSelectionChangedEventArgs()
            {
                NewDoor = this.SelectedNode
            };
            EventStore<DoorSelectionChangedEventArgs>.Publish(
                MapMakerConstants.EventStoreEventNames.EVENT_NODE_SELECTION_CHANGED, nodeSelectionArgs);
        }

        public EnemyTransporter SelectedNode
        {
            get
            {
                if (_nodes == null)
                    return null;

                if (lstDoors.SelectedIndex == -1)
                    return null;

                int selectedDoorId = (int)lstDoors.SelectedItem;
                EnemyTransporter node = _nodes.FirstOrDefault(d => d.Id == selectedDoorId);
                return node;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            lstDoors.SelectedIndex = -1;
        }
    }
}
