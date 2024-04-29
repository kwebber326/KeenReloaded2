using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.ControlEventArgs;

namespace KeenReloaded2.UserControls
{
    public partial class MainMenuOption : UserControl
    {
        private readonly string _optionText;
        private bool _isSelected;
        private Action _action;
        private Color _originalColor;
        public event EventHandler<MainMenuOptionEventArgs> MenuItemSelected;
        public event EventHandler<MainMenuOptionEventArgs> MenuItemDeselected;
        public MainMenuOption()
        {
            InitializeComponent();
        }

        public MainMenuOption(string optionText, Action menuItemAction, bool selected = false)
        {
            InitializeComponent();
            _optionText = optionText;
            _isSelected = selected;
            _action = menuItemAction;
        }

        private void MainMenuOption_Load(object sender, EventArgs e)
        {
            lblOptionText.Text = _optionText;
            _originalColor = lblOptionText.ForeColor;
            SetLabelForeColor();
        }

        public void SelectOption()
        {
            _isSelected = true;
            SetLabelForeColor();
            this.OnMainMenuOptionSelected();
        }

        public void DeselectOption()
        {
            _isSelected = false;
            SetLabelForeColor();
            this.OnMainMenuOptionDeselected();
        }

        public void ExecuteAction()
        {
            _action?.Invoke();
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
        }

        protected void OnMainMenuOptionSelected()
        {
            MainMenuOptionEventArgs args = new MainMenuOptionEventArgs()
            {
                OptionText = _optionText
            };
            this.MenuItemSelected?.Invoke(this, args);
        }
        protected void OnMainMenuOptionDeselected()
        {
            MainMenuOptionEventArgs args = new MainMenuOptionEventArgs()
            {
                OptionText = _optionText
            };
            this.MenuItemDeselected?.Invoke(this, args);
        }

        private void SetLabelForeColor()
        {
            lblOptionText.ForeColor = _isSelected ? Color.Red : _originalColor;
        }

        private void MainMenuOption_Click(object sender, EventArgs e)
        {
            this.SelectOption();
            this.ExecuteAction();
        }

        private void LblOptionText_Click(object sender, EventArgs e)
        {
            this.SelectOption();
            this.ExecuteAction();
        }

        private void LblOptionText_MouseEnter(object sender, EventArgs e)
        {
            this.SelectOption();
        }

        private void LblOptionText_MouseLeave(object sender, EventArgs e)
        {
            this.DeselectOption();
        }
    }
}
