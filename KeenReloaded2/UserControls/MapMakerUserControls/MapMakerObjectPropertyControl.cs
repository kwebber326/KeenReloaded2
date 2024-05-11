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

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class MapMakerObjectPropertyControl : UserControl
    {
        private MapMakerObjectProperty _mapMakerObjectProperty;
        private TextBox _txtValue = new TextBox();
        private ComboBox _cmbValue = new ComboBox();
        private CheckBox _chkValue = new CheckBox();
        private AreaControl _areaValue = new AreaControl();

        private Control _selectedControl;
        private bool _selectedControlAdded = false;

        public MapMakerObjectPropertyControl()
        {
            InitializeComponent();
        }

        public MapMakerObjectPropertyControl(MapMakerObjectProperty mapMakerObjectProperty)
        {
            InitializeComponent();
            _mapMakerObjectProperty = mapMakerObjectProperty;
        }

        public object Value
        {
            get; private set;
        }

        private void MapMakerObjectPropertyControl_Load(object sender, EventArgs e)
        {
            _txtValue.TextChanged += _txtValue_TextChanged;
            _cmbValue.SelectedIndexChanged += _cmbValue_SelectedIndexChanged;
            _cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
            _chkValue.CheckedChanged += _chkValue_CheckedChanged;
            UpdateControl(_mapMakerObjectProperty);
        }

        private void _chkValue_CheckedChanged(object sender, EventArgs e)
        {
            this.Value = _chkValue.Checked;
        }

        private void _cmbValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Value = _cmbValue.SelectedItem;
        }

        private void _txtValue_TextChanged(object sender, EventArgs e)
        {
            this.Value = _txtValue.Text;
        }

        public void UpdateControl(MapMakerObjectProperty objectProperty)
        {
            _mapMakerObjectProperty = objectProperty;
            lblPropertyName.Text = _mapMakerObjectProperty.DisplayName;
            if (_mapMakerObjectProperty.DataType == typeof(Enum)
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
