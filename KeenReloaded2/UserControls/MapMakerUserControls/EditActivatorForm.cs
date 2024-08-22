using KeenReloaded2.Framework.GameEntities.Interfaces;
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
        }

        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            TransferSelection(lstCurrent, lstRemaining);
            RefreshCurrentSelection();
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
                source.Items.Remove(item);
                target.Items.Add(item);
            }

        }
    }
}
