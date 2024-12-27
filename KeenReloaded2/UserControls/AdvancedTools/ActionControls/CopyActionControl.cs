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
    public partial class CopyActionControl : UserControl, IActionControl
    {
        public CopyActionControl()
        {
            InitializeComponent();
        }

        private List<GameObjectMapping> _previousChanges;
        private List<GameObjectMapping> _currentChanges;
        protected List<GameObjectMapping> SelectedObjects { get; set; }

        public void CancelAction(object parameter)
        {
            if (parameter != null && parameter is AdvancedToolsActions?
               && ((AdvancedToolsActions?)parameter).Value != AdvancedToolsActions.COPY)
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
                  BuildEventData(_currentChanges));

            _previousChanges = new List<GameObjectMapping>();
            _currentChanges = new List<GameObjectMapping>();
        }

        public void PreviewAction(object parameter)
        {
            if (_previousChanges != null && _previousChanges.Any())
            {
                this.Undo(parameter);
            }

            //show copy Preview
            PastSelectionInSpecifiedLocation();

            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW,
                 BuildEventData(_currentChanges));

            _previousChanges = _currentChanges;
        }

        private void PastSelectionInSpecifiedLocation()
        {
            Rectangle r1 = AdvancedToolsCommonFunctions.GetExtensionArea(this.SelectedObjects);
            int x = Convert.ToInt32(txtX.Text);
            int y = Convert.ToInt32(txtY.Text);
            Rectangle r2 = new Rectangle(x, y, r1.Width, r1.Height);
            int xOffset = r2.X - r1.X;
            int yOffset = r2.Y - r1.Y;

            _currentChanges = new List<GameObjectMapping>();
            foreach (var obj in this.SelectedObjects)
            {
                GameObjectMapping mapping = new GameObjectMapping();
                mapping.Location = new Point(obj.Location.X + xOffset, obj.Location.Y + yOffset);
                mapping.MapMakerObject = obj.MapMakerObject.Clone();
                var areaProperty = mapping.MapMakerObject.ConstructorParameters
                    .FirstOrDefault(prop => prop.PropertyName == GeneralGameConstants.AREA_PROPERTY_NAME);
                if (areaProperty != null)
                {
                    Rectangle propertyRect = (Rectangle)areaProperty.Value;
                    propertyRect = new Rectangle(mapping.Location.X, mapping.Location.Y, propertyRect.Width, propertyRect.Height);
                    areaProperty.Value = propertyRect;
                }
                mapping.GameObject = (ISprite)mapping.MapMakerObject.Construct();
                mapping.Image = mapping.GameObject.Image;
                mapping.SizeMode = PictureBoxSizeMode.AutoSize;
                _currentChanges.Add(mapping);
            }
        }

        public void Undo(object parameter)
        {
            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_UNDO,
                 BuildEventData(_previousChanges));

            _previousChanges = new List<GameObjectMapping>();
            _currentChanges = new List<GameObjectMapping>();
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

        private void CopyActionControl_Load(object sender, EventArgs e)
        {
            actionCommandControl1.PreviewCommand = new AdvancedToolsCommand(PreviewAction,
                (x) => ValidateControl());
            actionCommandControl1.CancelCommand = new AdvancedToolsCommand(CancelAction);
            actionCommandControl1.CommitCommand = new AdvancedToolsCommand(CommitAction,
                (x) => ValidateControl());

            EventStore<AdvancedToolsEventArgs>.Subscribe(
                MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED,
                AdvancedForm_SelectionChanged);

            this.VisibleChanged += CopyActionControl_VisibleChanged;
        }

        private void CopyActionControl_VisibleChanged(object sender, EventArgs e)
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

        private AdvancedToolsEventArgs BuildEventData(List<GameObjectMapping> eventData)
        {
            AdvancedToolsEventArgs e = new AdvancedToolsEventArgs();
            e.ChangeData = new AdvancedToolsChangeData()
            {
                Action = AdvancedToolsActions.COPY,
                ChangedData = eventData
            };
            e.SelectedObjects = this.SelectedObjects;

            return e;
        }

        private void UpdateLocationText()
        {
            Rectangle extensionArea = AdvancedToolsCommonFunctions.GetExtensionArea(this.SelectedObjects);
            lblOldLocation.Text = $"Old Location: ({extensionArea.X}, {extensionArea.Y})";
            txtX.Text = extensionArea.X.ToString();
            txtY.Text = extensionArea.Y.ToString();
        }

        private void LblOldLocation_Click(object sender, EventArgs e)
        {

        }

        public void UpdateSelection(object selection)
        {
            this.SelectedObjects = selection as List<GameObjectMapping>;
            UpdateLocationText();
        }
    }
}
