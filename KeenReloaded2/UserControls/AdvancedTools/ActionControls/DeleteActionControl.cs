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
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Interfaces;

namespace KeenReloaded2.UserControls.AdvancedTools.ActionControls
{
    public class DeleteActionControl : UserControl, IActionControl
    {
        private ActionCommandControl actionCommandControl1;
        private List<GameObjectMapping> _previousChanges;
        private List<GameObjectMapping> _currentChanges;
        protected List<GameObjectMapping> SelectedObjects { get; set; }

        public DeleteActionControl()
        {
            InitializeComponent();
        }

        public void CancelAction(object parameter)
        {
            if (parameter != null && parameter is AdvancedToolsActions?
               && ((AdvancedToolsActions?)parameter).Value != AdvancedToolsActions.DELETE)
            {
                return;
            }
            if (_previousChanges != null && _previousChanges.Any())
            {
                EventStore<AdvancedToolsEventArgs>
                   .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_CANCEL,
                    BuildEventData(_previousChanges));
            }

            _currentChanges = new List<GameObjectMapping>();
            _previousChanges = new List<GameObjectMapping>();
        }

        public void CommitAction(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT,
                 BuildEventData(this.SelectedObjects));

            _previousChanges = new List<GameObjectMapping>();
            _currentChanges = new List<GameObjectMapping>();
        }

        public void PreviewAction(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW,
                 BuildEventData(this.SelectedObjects));

            _previousChanges = this.SelectedObjects;
        }

        public void Undo(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
               .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_UNDO,
                BuildEventData(_previousChanges));

            _previousChanges = new List<GameObjectMapping>();
            _currentChanges = new List<GameObjectMapping>();
        }

        public void UpdateSelection(object selection)
        {
            this.SelectedObjects = selection as List<GameObjectMapping> ?? new List<GameObjectMapping>();
        }

        public bool ValidateControl()
        {
            bool isValid = this.SelectedObjects != null && this.SelectedObjects.Any();

            if (!isValid)
            {
                MessageBox.Show("At least one object must be selected.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isValid;
        }

        private AdvancedToolsEventArgs BuildEventData(List<GameObjectMapping> eventData)
        {
            AdvancedToolsEventArgs e = new AdvancedToolsEventArgs();
            e.ChangeData = new AdvancedToolsChangeData()
            {
                Action = AdvancedToolsActions.DELETE,
                ChangedData = eventData
            };
            e.SelectedObjects = this.SelectedObjects;

            return e;
        }

        private void InitializeComponent()
        {
            this.actionCommandControl1 = new KeenReloaded2.UserControls.AdvancedTools.ActionCommandControl();
            this.SuspendLayout();
            // 
            // actionCommandControl1
            // 
            this.actionCommandControl1.CancelCommand = null;
            this.actionCommandControl1.CommitCommand = null;
            this.actionCommandControl1.Location = new System.Drawing.Point(3, 3);
            this.actionCommandControl1.Name = "actionCommandControl1";
            this.actionCommandControl1.PreviewCommand = null;
            this.actionCommandControl1.Size = new System.Drawing.Size(297, 71);
            this.actionCommandControl1.TabIndex = 0;
            this.actionCommandControl1.Load += new System.EventHandler(this.ActionCommandControl1_Load);
            // 
            // DeleteActionControl
            // 
            this.Controls.Add(this.actionCommandControl1);
            this.Name = "DeleteActionControl";
            this.Size = new System.Drawing.Size(316, 77);
            this.Load += new System.EventHandler(this.DeleteActionControl_Load);
            this.ResumeLayout(false);

        }

        private void ActionCommandControl1_Load(object sender, EventArgs e)
        {

        }

        private void DeleteActionControl_Load(object sender, EventArgs e)
        {
            actionCommandControl1.PreviewCommand = new AdvancedToolsCommand(PreviewAction,
                (x) => ValidateControl());
            actionCommandControl1.CancelCommand = new AdvancedToolsCommand(CancelAction);
            actionCommandControl1.CommitCommand = new AdvancedToolsCommand(CommitAction,
                (x) => ValidateControl());

            EventStore<AdvancedToolsEventArgs>.Subscribe(
                MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED,
                AdvancedForm_SelectionChanged);
        }

        private void AdvancedForm_SelectionChanged(object sender, ControlEventArgs<AdvancedToolsEventArgs> e)
        {
            this.SelectedObjects = e?.Data?.SelectedObjects;
        }
    }
}
