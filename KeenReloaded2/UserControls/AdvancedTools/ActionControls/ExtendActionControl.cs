using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.UserControls.AdvancedTools.ActionControls.Entities;
using KeenReloaded2.Utilities;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.UserControls.AdvancedTools
{
    public partial class ExtendActionControl : UserControl, IActionControl
    {
        public ExtendActionControl()
        {
            InitializeComponent();
            actionCommandControl1.PreviewCommand = new AdvancedToolsCommand(PreviewAction,
                (x) => ValidateControl());
            actionCommandControl1.CancelCommand = new AdvancedToolsCommand(CancelAction);
            actionCommandControl1.CommitCommand = new AdvancedToolsCommand(CommitAction,
                (x) => ValidateControl());

            EventStore<AdvancedToolsEventArgs>.Subscribe(
                MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED,
                AdvancedForm_SelectionChanged);

            var directions = Enum.GetNames(typeof(Direction));
            cmbDirection.Items.AddRange(directions);
            cmbDirection.SelectedIndex = 0;
        }
        private List<GameObjectMapping> _previousChanges;
        private List<GameObjectMapping> _currentChanges;
        protected List<GameObjectMapping> SelectedObjects { get; set; }

        public void CancelAction(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
               .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_CANCEL,
                BuildEventData(_previousChanges));
        }

        public void CommitAction(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT,
                 BuildEventData(_currentChanges));
        }

        public void PreviewAction(object parameter)
        {
            if (_previousChanges != null && _previousChanges.Any())
            {
                this.Undo(parameter);
            }

            _currentChanges = new List<GameObjectMapping>();

            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW,
                 BuildEventData(_currentChanges));

            _previousChanges = _currentChanges;
        }

        public void Undo(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_UNDO,
                 BuildEventData(_previousChanges));
        }

        public bool ValidateControl()
        {
            bool isValid = cmbDirection.SelectedItem != null && 
                (this.SelectedObjects?.Any() ?? false) &&
                int.TryParse(txtLengths.Text, out int result) &&
                result > 0 && result <= 50;
            if (!isValid)
            {
                MessageBox.Show("Invalid settings: \n\nEnsure that at least one object is selected and the lengths is an integer > 0 and <= 50",
                    "Invalid Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return isValid;
        }

        private AdvancedToolsEventArgs BuildEventData(List<GameObjectMapping> eventData)
        {
            AdvancedToolsEventArgs e = new AdvancedToolsEventArgs();
            e.ChangeData = new AdvancedToolsChangeData()
            {
                Action = AdvancedToolsActions.EXTEND,
                ChangedData = eventData
            };
            e.SelectedObjects = this.SelectedObjects;

            return e;
        }

        private void ExtendActionControl_Load(object sender, EventArgs e)
        {

        }

        private void AdvancedForm_SelectionChanged(object sender, ControlEventArgs<AdvancedToolsEventArgs> e)
        {
            this.SelectedObjects = e?.Data?.SelectedObjects;
        }
    }
}
