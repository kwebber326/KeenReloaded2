using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.DataStructures;
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

namespace KeenReloaded2
{
    public partial class AdvancedToolsForm : Form
    {
        private OrderedList<GameObjectMapping> _gameObjects;
        private Dictionary<int, GameObjectMapping> _previousAdvancedToolsSelection = new Dictionary<int, GameObjectMapping>();

        public AdvancedToolsForm()
        {
            InitializeComponent();
        }

        public AdvancedToolsForm(OrderedList<GameObjectMapping> gameObjects)
        {
            InitializeComponent();
            _gameObjects = gameObjects;
        }

        private void AdvancedToolsForm_Load(object sender, EventArgs e)
        {
            lstMapObjects.DrawMode = DrawMode.OwnerDrawFixed;
            lstMapObjects.DrawItem += new DrawItemEventHandler(listBox_DrawItem);
            lstMapObjects.SelectedIndexChanged += LstMapObjects_SelectedIndexChanged;
            PopulateListBoxWithCurrentItems();

            EventStore<AdvancedToolsActions>.Subscribe(
                MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTED_ACTION_CHANGED,
                SelectedAction_Changed);
        }

        private void PopulateListBoxWithCurrentItems()
        {
            lstMapObjects.Items.Clear();
            if (_gameObjects == null)
                return;
            foreach (var obj in _gameObjects)
            {
                lstMapObjects.Items.Add(obj.ToString());
            }
        }

        private void SelectedAction_Changed(object sender, ControlEventArgs<AdvancedToolsActions> selectedAction)
        {

        }

        private void LstMapObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentObjectsSelected = BuildSelctionOfMappingObjects();

            List<GameObjectMapping> newlySelected = currentObjectsSelected
                .Values.Where(k => !_previousAdvancedToolsSelection.ContainsKey(k.GetHashCode())).ToList();
            List<GameObjectMapping> deselected = _previousAdvancedToolsSelection
                .Values.Where(o => !currentObjectsSelected.ContainsKey(o.GetHashCode())).ToList();
               
            if (newlySelected.Any())
            {
                AdvancedToolsEventArgs eventData = new AdvancedToolsEventArgs()
                {
                    SelectedObjects = currentObjectsSelected.Values.ToList(),
                    ChangeData = new AdvancedToolsChangeData()
                    {
                        ChangedData = newlySelected,
                        ChangeMetaData = true
                    }
                };
                EventStore<AdvancedToolsEventArgs>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, eventData);
            }

            if (deselected.Any())
            {
                AdvancedToolsEventArgs eventData = new AdvancedToolsEventArgs()
                {
                    SelectedObjects = currentObjectsSelected.Values.ToList(),
                    ChangeData = new AdvancedToolsChangeData()
                    {
                        ChangedData = deselected,
                        ChangeMetaData = false
                    }
                };
                EventStore<AdvancedToolsEventArgs>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, eventData);
            }

            _previousAdvancedToolsSelection = currentObjectsSelected;
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            // Draw the background
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.LightBlue), e.Bounds);
            }
            else if (ItemSearchMatch(e.Index))
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            }

            // Draw the text
            e.Graphics.DrawString(lstMapObjects.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);

            // Draw the focus rectangle if the ListBox has focus
            e.DrawFocusRectangle();
        }

        private Dictionary<int, GameObjectMapping> BuildSelctionOfMappingObjects()
        {
            var dictionary = new Dictionary<int, GameObjectMapping>();

            if (lstMapObjects.SelectedItems != null && lstMapObjects.SelectedItems.Count > 0)
            {
                foreach (var item in lstMapObjects.SelectedItems)
                {
                    string data = item?.ToString();
                    if (!string.IsNullOrEmpty(data))
                    {
                        GameObjectMapping obj = _gameObjects.FirstOrDefault(o => o.ToString() == data);
                        if (obj != null)
                        {
                            dictionary.Add(obj.GetHashCode(), obj);
                        }
                    }
                }
            }

            return dictionary;
        }

        private bool ItemSearchMatch(int itemIndex)
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                lstMapObjects.Items[itemIndex].ToString().ToLower().Contains(txtSearch.Text.ToLower());
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            lstMapObjects.BorderStyle = BorderStyle.Fixed3D;
            lstMapObjects.BorderStyle = BorderStyle.None;
            for (int i = 0; i < lstMapObjects.Items.Count; i++)
            {
                if (ItemSearchMatch(i))
                {
                    lstMapObjects.SetScrollY(i);
                    return;
                }
            }
        }

        private void AdvancedToolsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.Cancel 
                && (_previousAdvancedToolsSelection?.Any() ?? false))
            {
                var selection = _previousAdvancedToolsSelection.Values.ToList();
                AdvancedToolsEventArgs eventData = new AdvancedToolsEventArgs()
                {
                    SelectedObjects = selection,
                    ChangeData = new AdvancedToolsChangeData()
                    {
                        ChangedData = selection,
                        ChangeMetaData = false
                    }
                };
                EventStore<AdvancedToolsEventArgs>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, eventData);
            }
        }
    }
}
