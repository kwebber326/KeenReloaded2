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
    public class MoveActionControl : UserControl, IActionControl
    {
        private ActionCommandControl actionCommandControl1;
        private Label lblOldLocation;
        private Label label1;
        private TextBox txtX;
        private Label label2;
        private TextBox txtY;
        private Label label3;

        private List<GameObjectMapping> _previousChanges;
        private List<GameObjectMapping> _currentChanges;
        protected List<GameObjectMapping> SelectedObjects { get; set; }

        Point _originalLocation = new Point();

        public MoveActionControl()
        {
            InitializeComponent();
        }

        public void CancelAction(object parameter)
        {
            if (parameter != null && parameter is AdvancedToolsActions?
               && ((AdvancedToolsActions?)parameter).Value != AdvancedToolsActions.MOVE)
            {
                return;
            }
            if (_previousChanges != null && _previousChanges.Any())
            {
                EventStore<AdvancedToolsEventArgs>
                   .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_CANCEL,
                    BuildEventData(_previousChanges));
            }

            Rectangle r1 = AdvancedToolsCommonFunctions.GetExtensionArea(this.SelectedObjects);
            Rectangle r2 = new Rectangle(_originalLocation.X, _originalLocation.Y, r1.Width, r1.Height);
            MoveSelectionToSpecifiedLocation(r1, r2, false);

            _currentChanges = new List<GameObjectMapping>();
            _previousChanges = new List<GameObjectMapping>();
        }

        public void CommitAction(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT,
                 BuildEventData(_currentChanges));

            _originalLocation = AdvancedToolsCommonFunctions.GetExtensionArea(this.SelectedObjects).Location;
            _previousChanges = new List<GameObjectMapping>();
            _currentChanges = new List<GameObjectMapping>();
        }

        public void PreviewAction(object parameter)
        {
            if (_previousChanges != null && _previousChanges.Any())
            {
                this.Undo(parameter);
            }

            //show move Preview
            Rectangle r1 = AdvancedToolsCommonFunctions.GetExtensionArea(this.SelectedObjects);
            int newX = Convert.ToInt32(txtX.Text);
            int newY = Convert.ToInt32(txtY.Text);
            Rectangle r2 = new Rectangle(newX, newY, r1.Width, r1.Height);
            MoveSelectionToSpecifiedLocation(r1, r2, true);

            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW,
                 BuildEventData(_currentChanges));

            _previousChanges = _currentChanges;

        }

        private void MoveSelectionToSpecifiedLocation(Rectangle r1, Rectangle r2, bool isPreview)
        {
            if (this.SelectedObjects == null)
                return;

            _currentChanges = new List<GameObjectMapping>();

            int xDiff = r2.X - r1.X;
            int yDiff = r2.Y - r1.Y;
            if (isPreview)
                _originalLocation = new Point(r1.Location.X, r1.Location.Y);
            else if (_originalLocation.X == 0 && _originalLocation.Y == 0)
                _originalLocation = AdvancedToolsCommonFunctions.GetExtensionArea(this.SelectedObjects).Location;

            foreach (var item in this.SelectedObjects)
            {
                var areaProperty = item.MapMakerObject.ConstructorParameters
                   .FirstOrDefault(prop => prop.PropertyName == GeneralGameConstants.AREA_PROPERTY_NAME);
                if (areaProperty != null)
                {
                    Rectangle propertyRect = (Rectangle)areaProperty.Value;
                    propertyRect = new Rectangle(item.GameObject.Location.X + xDiff, item.GameObject.Location.Y + yDiff, propertyRect.Width, propertyRect.Height);
                    areaProperty.Value = propertyRect;
                }
                item.GameObject = (ISprite)item.MapMakerObject.Construct();
                item.Image = item.GameObject.Image;
                item.SizeMode = PictureBoxSizeMode.AutoSize;
                item.Location = item.GameObject.Location;
                _currentChanges.Add(item);
            }
        }

        public void Undo(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_UNDO,
                 BuildEventData(_previousChanges));

            Rectangle r1 = AdvancedToolsCommonFunctions.GetExtensionArea(this.SelectedObjects);
            Rectangle r2 = new Rectangle(_originalLocation.X, _originalLocation.Y, r1.Width, r1.Height);
            MoveSelectionToSpecifiedLocation(r1, r2, false);

            _previousChanges = new List<GameObjectMapping>();
            _currentChanges = new List<GameObjectMapping>();
        }

        public void UpdateSelection(object selection)
        {
            this.SelectedObjects = selection as List<GameObjectMapping>;
            UpdateLocationText();
        }

        public bool ValidateControl()
        {
            bool isValid = this.SelectedObjects != null && this.SelectedObjects.Any()
                 && int.TryParse(txtX.Text, out int x) && x >= 0
                 && int.TryParse(txtY.Text, out int y) && y >= 0;
            if (!isValid)
            {
                MessageBox.Show("At least one object must be selected and X and Y coordinates must be non-negative integers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isValid;
        }

        private void InitializeComponent()
        {
            this.actionCommandControl1 = new KeenReloaded2.UserControls.AdvancedTools.ActionCommandControl();
            this.lblOldLocation = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // actionCommandControl1
            // 
            this.actionCommandControl1.CancelCommand = null;
            this.actionCommandControl1.CommitCommand = null;
            this.actionCommandControl1.Location = new System.Drawing.Point(3, 104);
            this.actionCommandControl1.Name = "actionCommandControl1";
            this.actionCommandControl1.PreviewCommand = null;
            this.actionCommandControl1.Size = new System.Drawing.Size(297, 71);
            this.actionCommandControl1.TabIndex = 0;
            // 
            // lblOldLocation
            // 
            this.lblOldLocation.AutoSize = true;
            this.lblOldLocation.Location = new System.Drawing.Point(4, 4);
            this.lblOldLocation.Name = "lblOldLocation";
            this.lblOldLocation.Size = new System.Drawing.Size(142, 20);
            this.lblOldLocation.TabIndex = 1;
            this.lblOldLocation.Text = "Old Location: (0, 0)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "New Location: (";
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(133, 37);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(100, 26);
            this.txtX.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(240, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = ",";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(259, 37);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(100, 26);
            this.txtY.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(365, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = ")";
            // 
            // MoveActionControl
            // 
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblOldLocation);
            this.Controls.Add(this.actionCommandControl1);
            this.Name = "MoveActionControl";
            this.Size = new System.Drawing.Size(530, 175);
            this.Load += new System.EventHandler(this.MoveActionControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private AdvancedToolsEventArgs BuildEventData(List<GameObjectMapping> eventData)
        {
            AdvancedToolsEventArgs e = new AdvancedToolsEventArgs();
            e.ChangeData = new AdvancedToolsChangeData()
            {
                Action = AdvancedToolsActions.MOVE,
                ChangedData = eventData
            };
            e.SelectedObjects = this.SelectedObjects;

            return e;
        }

        private void MoveActionControl_Load(object sender, EventArgs e)
        {
            actionCommandControl1.PreviewCommand = new AdvancedToolsCommand(PreviewAction,
               (x) => ValidateControl());
            actionCommandControl1.CancelCommand = new AdvancedToolsCommand(CancelAction);
            actionCommandControl1.CommitCommand = new AdvancedToolsCommand(CommitAction,
                (x) => ValidateControl());

            EventStore<AdvancedToolsEventArgs>.Subscribe(
                MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED,
                AdvancedForm_SelectionChanged);

            this.VisibleChanged += MoveActionControl_VisibleChanged;
        }

        private void MoveActionControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                UpdateLocationText();
            }
        }

        private void AdvancedForm_SelectionChanged(object sender, ControlEventArgs<AdvancedToolsEventArgs> e)
        {
            this.SelectedObjects = e?.Data?.SelectedObjects;
            UpdateLocationText();
        }

        private void UpdateLocationText()
        {
            Rectangle extensionArea = AdvancedToolsCommonFunctions.GetExtensionArea(this.SelectedObjects);
            lblOldLocation.Text = $"Old Location: ({extensionArea.X}, {extensionArea.Y})";
            txtX.Text = extensionArea.X.ToString();
            txtY.Text = extensionArea.Y.ToString();
        }
    }
}
