using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Framework.GameEntities.Interfaces;
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
    public partial class EditActivatorForm : Form
    {
        private List<IActivateable> _current = new List<IActivateable>();
        private List<IActivateable> _remaining = new List<IActivateable>();
        private Dictionary<Guid, IActivateable> _total = new Dictionary<Guid, IActivateable>();
        private bool _selectionChangeFromCode;

        public EditActivatorForm()
        {
            InitializeComponent();
        }

        public EditActivatorForm(List<IActivateable> current, List<IActivateable> total)
        {
            InitializeComponent();
            _current = current;
            _remaining = total.Except(current, new ActivatorEqualityComparer()).ToList();
            _total = total.ToDictionary(a => a.ActivationID);
            _current = _current.Where(c => _total.ContainsKey(c.ActivationID)).ToList();
        }

        public List<IActivateable> ChosenActivateables
        {
            get
            {
                return _current;
            }
        }

        private void EditActivatorForm_Load(object sender, EventArgs e)
        {
            InitializeControlState();
            PublishSelectionChanged();
        }

        private void InitializeControlState()
        {
            lstRemaining.Items.Clear();
            foreach (var item in _remaining)
            {
                lstRemaining.Items.Add(item.ActivationID);
            }
            lstCurrent.Items.Clear();
            foreach (var item in _current)
            {
                lstCurrent.Items.Add(item.ActivationID);
            }
        }

        class ActivatorEqualityComparer : IEqualityComparer<IActivateable>
        {
            public bool Equals(IActivateable x, IActivateable y)
            {
                return x.ActivationID.ToString() == y.ActivationID.ToString();
            }

            public int GetHashCode(IActivateable obj)
            {
                return -1;
            }
        }

        private void BtnAddSelection_Click(object sender, EventArgs e)
        {
            TransferSelection(lstRemaining, lstCurrent);
            RefreshCurrentSelection();
            PublishSelectionChanged();
        }

        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            TransferSelection(lstCurrent, lstRemaining);
            RefreshCurrentSelection();
            PublishSelectionChanged();
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void RefreshCurrentSelection()
        {
            _current.Clear();
            var currentGuids = lstCurrent.Items.OfType<Guid>();
            if (currentGuids.Any())
            {
                foreach (var guid in currentGuids)
                {
                    if (_total.TryGetValue(guid, out IActivateable item))
                    {
                        _current.Add(item);
                    }
                }
            }

            _remaining.Clear();
            var remainingGuids = lstRemaining.Items.OfType<Guid>();
            if (remainingGuids.Any())
            {
                foreach (var guid in remainingGuids)
                {
                    if (_total.TryGetValue(guid, out IActivateable item))
                    {
                        _remaining.Add(item);
                    }
                }
            }
        }

        private void TransferSelection(ListBox source, ListBox target)
        {
            if (source.SelectedItems == null || source.SelectedItems.Count == 0)
                return;

            if (source == target)
                return;

            List<Guid> selectedItems = new List<Guid>();
            foreach (var item in source.SelectedItems)
            {
                selectedItems.Add((Guid)item);
            }
            foreach (var item in selectedItems)
            {
                _selectionChangeFromCode = true;
                source.Items.Remove(item);
                target.Items.Add(item);
                _selectionChangeFromCode = false;
            }

        }

        private Tuple<List<IActivateable>, List<IActivateable>> GetSelectedAndUnselectedInListBox(ListBox listBox, List<IActivateable> activateablesStore)
        {
            Tuple<List<IActivateable>, List<IActivateable>> retVal = new Tuple<List<IActivateable>, List<IActivateable>>
                    (new List<IActivateable>(), new List<IActivateable>());

            var selectedGuids = listBox.SelectedItems.OfType<Guid>();
            List<IActivateable> selected = new List<IActivateable>();
            if (selectedGuids.Any())
            {
                selected = selectedGuids.Select(g => _total[g]).ToList();
            }
            List<IActivateable> unselected = activateablesStore.Except(selected,
                   new ActivatorEqualityComparer()).ToList();
            retVal = new Tuple<List<IActivateable>, List<IActivateable>>(selected, unselected);
            return retVal;
        }

        private ActivatorSelectionChangedEventArgs BuildEventDataForSelectionChangedEvent()
        {
            var currentSelectionGrouping = GetSelectedAndUnselectedInListBox(lstCurrent, _current);
            var otherSelectionGroup = GetSelectedAndUnselectedInListBox(lstRemaining, _remaining);
            ActivatorSelectionChangedEventArgs e = new ActivatorSelectionChangedEventArgs()
            {
                CurrentActivateablesSelected = currentSelectionGrouping.Item1,
                CurrentActiveablesUnSelected = currentSelectionGrouping.Item2,
                OtherActivateablesSelected = otherSelectionGroup.Item1,
                OtherActiveablesUnSelected = otherSelectionGroup.Item2
            };
            return e;
        }

        private void PublishSelectionChanged()
        {
            var eventData = BuildEventDataForSelectionChangedEvent();
            EventStore<ActivatorSelectionChangedEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_CHANGED, eventData);
        }

        private void PublishSelectionComplete()
        {
            var eventData = new ActivatorSelectionCompletedEventArgs()
            {
                Activateables = _total.Values.ToList()
            };
            EventStore<ActivatorSelectionCompletedEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_COMPLETE, eventData);
        }

        private void LstRemaining_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_selectionChangeFromCode)
                PublishSelectionChanged();
        }

        private void LstCurrent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_selectionChangeFromCode)
                PublishSelectionChanged();
        }

        private void EditActivatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PublishSelectionComplete();
        }
    }
}
