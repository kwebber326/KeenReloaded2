using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.DataStructures;
using KeenReloaded2.UserControls.AdvancedTools;
using KeenReloaded2.UserControls.AdvancedTools.ActionControls;
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
        private Dictionary<AdvancedToolsActions, IActionControl> _actionToFormMapping = new Dictionary<AdvancedToolsActions, IActionControl>();
        private List<string> _originalList;

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
            PopulateComboBoxForSortCriteria();
            InitializeActionFormControls();

            EventStore<AdvancedToolsActions>.Subscribe(
                MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTED_ACTION_CHANGED,
                SelectedAction_Changed);

            EventStore<AdvancedToolsEventArgs>.Subscribe(
                MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT,
                Action_Committed);
        }

        private void InitializeActionFormControls()
        {
            //extend
            ExtendActionControl extendActionControl = new ExtendActionControl();
            extendActionControl.Location = new Point(lstMapObjects.Location.X, lstMapObjects.Bottom + 2);
            extendActionControl.Visible = false;
            this.Controls.Add(extendActionControl);
            _actionToFormMapping.Add(AdvancedToolsActions.EXTEND, extendActionControl);
            //copy
            CopyActionControl copyActionControl = new CopyActionControl();
            copyActionControl.Location = new Point(lstMapObjects.Location.X, lstMapObjects.Bottom + 2);
            copyActionControl.Visible = false;
            this.Controls.Add(copyActionControl);
            _actionToFormMapping.Add(AdvancedToolsActions.COPY, copyActionControl);
            //move
            MoveActionControl moveActionControl = new MoveActionControl();
            moveActionControl.Location = new Point(lstMapObjects.Location.X, lstMapObjects.Bottom + 2);
            moveActionControl.Visible = false;
            this.Controls.Add(moveActionControl);
            _actionToFormMapping.Add(AdvancedToolsActions.MOVE, moveActionControl);
            //delete
            DeleteActionControl deleteActionControl = new DeleteActionControl();
            deleteActionControl.Location = new Point(lstMapObjects.Location.X, lstMapObjects.Bottom + 2);
            deleteActionControl.Visible = false;
            this.Controls.Add(deleteActionControl);
            _actionToFormMapping.Add(AdvancedToolsActions.DELETE, deleteActionControl);
        }

        private void PopulateListBoxWithCurrentItems()
        {
            lstMapObjects.Items.Clear();
            if (_gameObjects == null)
                return;

            _originalList = _gameObjects.Select(g => g.ToString()).ToList();
            foreach (var obj in _gameObjects)
            {
                lstMapObjects.Items.Add(obj.ToString());
            }
        }

        private void SelectedAction_Changed(object sender, ControlEventArgs<AdvancedToolsActions> selectedAction)
        {
            foreach (var control in _actionToFormMapping.Values)
            {
                var ctrl = control as Control;
                if (ctrl != null)
                {
                    ctrl.Visible = false;
                }
            }
            if (_actionToFormMapping.TryGetValue(selectedAction.Data, out IActionControl actionControl))
            {
                var ctrl = actionControl as Control;
                if (ctrl != null)
                {
                    var selection = BuildSelectionOfMappingObjects().Values.ToList();
                    actionControl.UpdateSelection(selection);
                    ctrl.Visible = true;
                }
            }
        }

        private void Action_Committed(object sender, ControlEventArgs<AdvancedToolsEventArgs> e)
        {
            var data = e.Data.ChangeData.ChangedData as List<GameObjectMapping>;
            var action = e.Data.ChangeData.Action;
            if (data != null && (action == AdvancedToolsActions.EXTEND || action == AdvancedToolsActions.COPY))
            {
                foreach (var item in data)
                {
                    lstMapObjects.Items.Add(item.ToString());
                }
            }
        }

        private void LstMapObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentObjectsSelected = BuildSelectionOfMappingObjects();

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

        private Dictionary<int, GameObjectMapping> BuildSelectionOfMappingObjects()
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
                            var key = obj.GetHashCode();
                            if (!dictionary.ContainsKey(key))
                                dictionary.Add(obj.GetHashCode(), obj);
                        }
                    }
                }
            }

            return dictionary;
        }

        private AdvancedToolsActions? GetSelectedAction()
        {
            var radioButtons = advancedToolsActionRadioList1.Controls.OfType<RadioButton>();
            if (radioButtons.Any())
            {
                var selectedActionText = radioButtons.FirstOrDefault(f => f.Checked)?.Text;
                if (string.IsNullOrEmpty(selectedActionText))
                    return null;

                try
                {
                    AdvancedToolsActions selectedAction = (AdvancedToolsActions)Enum.Parse(typeof(AdvancedToolsActions), selectedActionText);
                    return selectedAction;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }
            return null;
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
                var selectedAction = GetSelectedAction();
                foreach (var item in _actionToFormMapping.Values)
                {
                    if (selectedAction != AdvancedToolsActions.MOVE)
                        item.CancelAction(selectedAction);
                }
            }

            EventStore<AdvancedToolsActions>.UnSubscribe(
               MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTED_ACTION_CHANGED,
               SelectedAction_Changed);

            EventStore<AdvancedToolsEventArgs>.UnSubscribe(
               MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT,
               Action_Committed);
        }

        private void CmbSortCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSortCriteria.SelectedItem == null)
                return;

            string selectedOption = cmbSortCriteria.SelectedItem.ToString();
            AdvancedToolsSortCriteria sortCriteria = FromDisplayText(selectedOption);
            SortObjectListByCriteria(sortCriteria);
        }

        private void SortObjectListByCriteria(AdvancedToolsSortCriteria criteria)
        {
            //clear selection
            var selection = BuildSelectionOfMappingObjects().Values.ToList();
            AdvancedToolsEventArgs eventData = new AdvancedToolsEventArgs()
            {
                SelectedObjects = selection,
                ChangeData = new AdvancedToolsChangeData()
                {
                    ChangedData = selection,
                    ChangeMetaData = false
                }
            };
            EventStore<AdvancedToolsEventArgs>.Publish(
                MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, eventData);

            selection.Clear();
            eventData.ChangeData.ChangeMetaData = true;
            EventStore<AdvancedToolsEventArgs>.Publish(
              MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, eventData);

            //sort the list
            lstMapObjects.Items.Clear();
            if (_gameObjects == null)
                return;

            var sortedObjects = _gameObjects.ToList();
            switch (criteria)
            {
                case AdvancedToolsSortCriteria.NONE:
                default:
                    break;
                case AdvancedToolsSortCriteria.TYPE_ASCENDING:
                    sortedObjects = sortedObjects.OrderBy(s => s.GameObject.GetType().Name).ToList();
                    break;
                case AdvancedToolsSortCriteria.TYPE_DESCENDING:
                    sortedObjects = sortedObjects.OrderByDescending(s => s.GameObject.GetType().Name).ToList();
                    break;
                case AdvancedToolsSortCriteria.LOCATION_LEFT_TO_RIGHT:
                    sortedObjects = sortedObjects
                        .OrderBy(s => s.GameObject.Location.X)
                        .ThenBy(s1 => s1.GameObject.Location.Y).ToList();
                    break;
                case AdvancedToolsSortCriteria.LOCATION_TOP_TO_BOTTOM:
                    sortedObjects = sortedObjects
                        .OrderBy(s => s.GameObject.Location.Y)
                        .ThenBy(s1 => s1.GameObject.Location.X).ToList();
                    break;
            }

            foreach (var obj in sortedObjects)
            {
                lstMapObjects.Items.Add(obj.ToString());
            }
        }

        private void PopulateComboBoxForSortCriteria()
        {
            //populate the object list with a newly sorted list
            var values = Enum.GetValues(typeof(AdvancedToolsSortCriteria)).OfType<AdvancedToolsSortCriteria>();
            cmbSortCriteria.Items.Clear();
            foreach (var value in values)
            {
                string displayText = ToDisplayText(value);
                cmbSortCriteria.Items.Add(displayText);
            }
            cmbSortCriteria.SelectedIndex = 0;
        }

        private AdvancedToolsSortCriteria FromDisplayText(string displayText)
        {
            string value = displayText.ToUpper().Replace(" ", "_");
            AdvancedToolsSortCriteria result = (AdvancedToolsSortCriteria)Enum.Parse(typeof(AdvancedToolsSortCriteria), value);
            return result;
        }

        private string ToDisplayText(AdvancedToolsSortCriteria sortCriteria)
        {
            string lowerCaseWord = sortCriteria.ToString().ToLower().Replace("_", " ");
            string[] words = lowerCaseWord.Split(' ');
            List<string> finalWords = new List<string>();
            foreach (var word in words)
            {
                string firstLetter = word.Substring(0, 1).ToUpper();
                string restOfWord = word.Substring(1);
                string finalWord = firstLetter + restOfWord;
                finalWords.Add(finalWord);
            }
            string result = string.Join(" ", finalWords);
            return result;
        }
    }

    enum AdvancedToolsSortCriteria
    {
        NONE,
        TYPE_ASCENDING,
        TYPE_DESCENDING,
        LOCATION_TOP_TO_BOTTOM,
        LOCATION_LEFT_TO_RIGHT
    }
}
