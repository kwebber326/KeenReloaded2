using KeenReloaded2.Constants;
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
            PopulateListBoxWithCurrentItems();
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
    }
}
