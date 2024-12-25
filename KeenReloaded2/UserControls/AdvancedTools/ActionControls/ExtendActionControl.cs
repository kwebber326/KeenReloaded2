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
using KeenReloaded2.Framework.GameEntities.Interfaces;

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

            //show extension Preview
            ExtendSelectionInSelectedDirection();

            EventStore<AdvancedToolsEventArgs>
                .Publish(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW,
                 BuildEventData(_currentChanges));

            _previousChanges = _currentChanges;
        }

        private void ExtendSelectionInSelectedDirection()
        {
            _currentChanges = new List<GameObjectMapping>();
            var direction = GetDirection();
            int lengths = int.Parse(txtLengths.Text);
            Rectangle extensionArea = GetExtensionArea();
            int xOffset = 0, yOffset = 0;
            switch (direction)
            {
                case Direction.LEFT:
                case Direction.DOWN_LEFT:
                case Direction.UP_LEFT:
                    xOffset = extensionArea.Width * -1;
                    break;
                case Direction.RIGHT:
                case Direction.DOWN_RIGHT:
                case Direction.UP_RIGHT:
                    xOffset = extensionArea.Width;
                    break;
            }

            switch (direction)
            {
                case Direction.DOWN:
                case Direction.DOWN_LEFT:
                case Direction.DOWN_RIGHT:
                    yOffset = extensionArea.Height;
                    break;
                case Direction.UP:
                case Direction.UP_LEFT:
                case Direction.UP_RIGHT:
                    yOffset = extensionArea.Height * -1;
                    break;
            }
            int x = extensionArea.X, y = extensionArea.Y;
            for (int i = 1; i <= lengths; i++)
            {
                List<GameObjectMapping> currentObjects = this.SelectedObjects;
                List<GameObjectMapping> tmp = new List<GameObjectMapping>();
                for (int j = 0; j < currentObjects.Count; j++)
                {
                    GameObjectMapping mapping = new GameObjectMapping();
                    var obj = currentObjects[j];
                    int newX = obj.Location.X + (xOffset * i);
                    int newY = obj.Location.Y + (yOffset * i);
                    Point p1 = new Point(newX, newY);
                    mapping.Location = p1;

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
                    tmp.Add(mapping);
                }
                currentObjects = tmp;
                tmp = new List<GameObjectMapping>();
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

        private Direction GetDirection()
        {
            string directionName = cmbDirection.SelectedItem.ToString();
            Direction direction = (Direction)Enum.Parse(typeof(Direction), directionName);
            return direction;
        }

        private Rectangle GetExtensionArea()
        {
            if (this.SelectedObjects == null || !this.SelectedObjects.Any())
                return new Rectangle();

            int minLeft = this.SelectedObjects.Select(o => o.GameObject.Location.X).Min();
            int maxRight = this.SelectedObjects.Select(o => o.GameObject.Location.X + o.Image.Width).Max();
            int minTop = this.SelectedObjects.Select(o => o.GameObject.Location.Y).Min();
            int maxBottom = this.SelectedObjects.Select(o => o.GameObject.Location.Y + o.GameObject.Image.Height).Max();

            int width = maxRight - minLeft;
            int height = maxBottom - minTop;

            return new Rectangle(minLeft, minTop, width, height);
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
