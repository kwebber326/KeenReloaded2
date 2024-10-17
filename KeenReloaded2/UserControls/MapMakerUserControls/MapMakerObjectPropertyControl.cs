using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Constants;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class MapMakerObjectPropertyControl : UserControl
    {
        private MapMakerObjectProperty _mapMakerObjectProperty;
        private readonly MapMakerObject _mapMakerObject;
        private TextBox _txtValue = new TextBox();
        private ComboBox _cmbValue = new ComboBox();
        private CheckBox _chkValue = new CheckBox();
        private AreaControl _areaValue = new AreaControl();
        private ActivatorListControl _activatorControl = new ActivatorListControl();
        private DestinationDoorActivatorControl _doorControl = new DestinationDoorActivatorControl();
        private PointSelectorUserControl _pointSelectorControl = new PointSelectorUserControl();

        private Control _selectedControl;
        private bool _selectedControlAdded = false;
        private List<IActivateable> _totalActivateables = new List<IActivateable>();
        private List<Door> _doors = new List<Door>();

        public MapMakerObjectPropertyControl()
        {
            InitializeComponent();
        }

        public MapMakerObjectPropertyControl(MapMakerObjectProperty mapMakerObjectProperty, MapMakerObject mapMakerObject, List<IActivateable> totalActivateables, List<Door> doors)
        {
            InitializeComponent();
            _mapMakerObjectProperty = mapMakerObjectProperty;
            _mapMakerObject = mapMakerObject;
            _totalActivateables = totalActivateables;
            _doors = doors;
        }

        public object Value
        {
            get
            {
                return _mapMakerObjectProperty?.Value;
            }
        }

        public MapMakerObjectProperty ObjectProperty
        {
            get
            {
                return _mapMakerObjectProperty;
            }
        }

        private void MapMakerObjectPropertyControl_Load(object sender, EventArgs e)
        {
            _txtValue.TextChanged += _txtValue_TextChanged;
            _cmbValue.SelectedIndexChanged += _cmbValue_SelectedIndexChanged;
            _cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbValue.Width = 200;
            _chkValue.CheckedChanged += _chkValue_CheckedChanged;
            _areaValue.AreaChanged += _areaValue_AreaChanged;
            _activatorControl.EditItemsClicked += _activatorControl_EditItemsClicked;
            _doorControl.DoorSelect += _doorControl_DoorSelect;
            _pointSelectorControl.EditItemsClicked += _pointSelectorControl_EditItemsClicked;
            UpdateControl(_mapMakerObjectProperty);
        }

        private void _pointSelectorControl_EditItemsClicked(object sender, EventArgs e)
        {
            List<Point> points = _mapMakerObjectProperty.Value as List<Point>;
            if (points != null)
            {
                PathwayCreatorForm pathwayCreatorForm = new PathwayCreatorForm(points);
                if (pathwayCreatorForm.ShowDialog() == DialogResult.OK)
                {
                    _mapMakerObjectProperty.Value = pathwayCreatorForm.PathWayPoints;
                }
            }
        }

        private void _doorControl_DoorSelect(object sender, EventArgs e)
        {
            var doorIdProperty = _mapMakerObject.ConstructorParameters.FirstOrDefault(d => d.PropertyName == GeneralGameConstants.DOOR_ID_PROPERTY_NAME);
            if (doorIdProperty != null)
            {
                int doorId = (int)doorIdProperty.Value;
                bool duplicateDoorIds = _doors.GroupBy(g => g.Id).Any(g1 => g1.Count() > 1);
                if (duplicateDoorIds)
                {
                    MessageBox.Show("There are duplicate Door numbers. Make sure each door number is unique before proceeding.", "Duplicate Door IDs", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var otherDoors = _doors.Where(d => d.Id != doorId).ToList();
                var thisDoor = _doors.FirstOrDefault(d => d.Id == doorId);
                if (thisDoor == null)
                {
                    MessageBox.Show("No Door found with this ID.", "Door not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                EditDestinationDoorForm doorForm = new EditDestinationDoorForm(otherDoors, thisDoor);
                if (doorForm.ShowDialog() == DialogResult.OK)
                {
                    _mapMakerObjectProperty.Value = doorForm.SelectedDoor?.Id;
                }
            }
        }

        private void _activatorControl_EditItemsClicked(object sender, EventArgs e)
        {
            var currentActivators = (IActivateable[])_mapMakerObjectProperty.Value;
            EditActivatorForm form = new EditActivatorForm(currentActivators.ToList(), _totalActivateables);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _activatorControl.Activateables = form.ChosenActivateables;
                _mapMakerObjectProperty.Value = form.ChosenActivateables.ToArray();
            }
        }

        private void _areaValue_AreaChanged(object sender, ControlEventArgs.AreaControlEventArgs e)
        {
            _mapMakerObjectProperty.Value = e.Area;
        }

        private void _chkValue_CheckedChanged(object sender, EventArgs e)
        {
            _mapMakerObjectProperty.Value = _chkValue.Checked;
        }

        private void _cmbValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            _mapMakerObjectProperty.Value = _cmbValue.SelectedItem;
            if (_mapMakerObjectProperty.DataType?.IsEnum ?? false)
            {
                var value = Enum.Parse(_mapMakerObjectProperty.DataType, _cmbValue.SelectedItem?.ToString());
                _mapMakerObjectProperty.Value = value;
            }
        }

        private void _txtValue_TextChanged(object sender, EventArgs e)
        {
            _mapMakerObjectProperty.Value = _txtValue.Text;
            if (_mapMakerObjectProperty.DataType == typeof(int))
            {
                bool parseSuccess = int.TryParse(_txtValue.Text, out int val);
                _mapMakerObjectProperty.Value = parseSuccess ? val : 0;
            }
            else if (_mapMakerObjectProperty.DataType == typeof(Guid))
            {
                Guid guid = Guid.Parse(_txtValue.Text);
                _mapMakerObjectProperty.Value = guid;
            }
        }

        public void UpdateControl(MapMakerObjectProperty objectProperty)
        {
            _mapMakerObjectProperty = objectProperty;
            lblPropertyName.Text = _mapMakerObjectProperty.DisplayName;
            if (_mapMakerObjectProperty.DataType.IsEnum
             || _mapMakerObjectProperty.DataType == typeof(string[])
             || _mapMakerObjectProperty.DataType == typeof(List<string>))
            {
                _selectedControl = _cmbValue;
                SetValuesForComboBox();
            }
            else if (_mapMakerObjectProperty.DataType == typeof(bool))
            {
                _selectedControl = _chkValue;
                SetBooleanValueForCheckbox();
            }
            else if (_mapMakerObjectProperty.DataType == typeof(Rectangle))
            {
                _selectedControl = _areaValue;
                SetAreaValueForAreaControl();
            }
            else if (_mapMakerObjectProperty.DataType == typeof(Point))
            {
                _selectedControl = _areaValue;
                SetLocationValueForAreaControl();
            }
            else if (_mapMakerObjectProperty.DataType == typeof(IActivateable[]))
            {
                _selectedControl = _activatorControl;
                SetValuesForActivatorControl();
            }
            else if (_mapMakerObjectProperty.IsDoorSelectionProperty)
            {
                _selectedControl = _doorControl;
            }
            else if (_mapMakerObjectProperty.DataType == typeof(List<Point>))
            {
                _selectedControl = _pointSelectorControl;
            }
            else
            {
                _selectedControl = _txtValue;
                SetTextValueForDefaultControl();
            }

            _selectedControl.Visible = true;
            _selectedControl.Enabled = !_mapMakerObjectProperty.Readonly;
            _selectedControl.Location = new Point(lblPropertyName.Right + 2, lblPropertyName.Top);
            if (!_selectedControlAdded)
            {
                this.Controls.Add(_selectedControl);
                _selectedControlAdded = true;
            }
        }

        private void SetValuesForActivatorControl()
        {
            var val = ((IActivateable[])_mapMakerObjectProperty.Value).ToList();
            _activatorControl.Activateables = val;
        }

        private void SetLocationValueForAreaControl()
        {
            try
            {
                Point location = (Point)_mapMakerObjectProperty.Value;
                Rectangle area = new Rectangle(location, Size.Empty);
                _areaValue.Area = area;
                _areaValue.CanModifySize = false;
            }
            catch
            {
                MessageBox.Show("Error Parsing location for " + _mapMakerObjectProperty.PropertyName);
            }
        }

        private void SetTextValueForDefaultControl()
        {
            try
            {
                _txtValue.Text = _mapMakerObjectProperty.Value?.ToString();
            }
            catch
            {
                MessageBox.Show("Error Parsing text for " + _mapMakerObjectProperty.PropertyName);
            }
        }

        private void SetAreaValueForAreaControl()
        {
            try
            {
                Rectangle area = (Rectangle)_mapMakerObjectProperty.Value;
                _areaValue.Area = area;
            }
            catch
            {
                MessageBox.Show("Error Parsing area for " + _mapMakerObjectProperty.PropertyName);
            }
        }

        private void SetBooleanValueForCheckbox()
        {
            try
            {
                _chkValue.Checked = (bool)_mapMakerObjectProperty.Value;
            }
            catch
            {
                MessageBox.Show("Error Parsing boolean value for " + _mapMakerObjectProperty.PropertyName);
            }
        }

        private void SetValuesForComboBox()
        {
            _cmbValue.Items.Clear();
            try
            {
                string value = _mapMakerObjectProperty.Value?.ToString();
                foreach (var name in _mapMakerObjectProperty.PossibleValues)
                {
                    _cmbValue.Items.Add(name);
                }

                if (!string.IsNullOrEmpty(value))
                {
                    int index = _mapMakerObjectProperty.PossibleValues.ToList().IndexOf(value);
                    if (index != -1)
                        _cmbValue.SelectedIndex = index;
                    else
                        _cmbValue.SelectedIndex = 0;
                }
                else
                {
                    _cmbValue.SelectedIndex = 0;
                }
            }
            catch
            {
                MessageBox.Show("Error Parsing Enum for " + _mapMakerObjectProperty.PropertyName);
            }
        }
    }
}
