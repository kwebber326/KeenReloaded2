using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.UserControls.AdvancedTools
{
    public class AdvancedToolsActionRadioList : UserControl
    {
        private Dictionary<string, AdvancedToolsActions> _actionBank = new Dictionary<string, AdvancedToolsActions>();
        private const int VERTICAL_RADIO_BUTTON_MARGIN = 8;
        public AdvancedToolsActionRadioList()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AdvancedToolsActionRadioList
            // 
            this.AutoScroll = true;
            this.Name = "AdvancedToolsActionRadioList";
            this.Size = new System.Drawing.Size(212, 371);
            this.Load += new System.EventHandler(this.AdvancedToolsActionRadioList_Load);
            this.ResumeLayout(false);

        }

        public AdvancedToolsActions? SelectedAction
        {
            get
            {
                var radioButtons = this.Controls.OfType<RadioButton>();
                if (radioButtons.Any(r => r.Checked))
                {
                    var selectedControl = radioButtons.FirstOrDefault(r => r.Checked);
                    if (_actionBank.TryGetValue(selectedControl.Text, out AdvancedToolsActions action))
                    {
                        return action;
                    }
                }
                return null;
            }
        }

        private void AdvancedToolsActionRadioList_Load(object sender, EventArgs e)
        {
            InitializeRadioButtonListControl();
        }

        private void InitializeRadioButtonListControl()
        {
            string[] keys = Enum.GetNames(typeof(AdvancedToolsActions));
            AdvancedToolsActions[] values = Enum.GetValues(typeof(AdvancedToolsActions))
                .OfType<AdvancedToolsActions>().ToArray();

            int currentY = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                _actionBank.Add(keys[i], values[i]);
                Point ctrlLocation = new Point(0, currentY);
                RadioButton radioButton = new RadioButton();
                radioButton.Text = keys[i];
                radioButton.Location = ctrlLocation;
                radioButton.CheckedChanged += RadioButton_CheckedChanged;
                this.Controls.Add(radioButton);
                currentY = radioButton.Bottom + VERTICAL_RADIO_BUTTON_MARGIN;
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            var selectedAction = this.SelectedAction;
            if (selectedAction != null)
                EventStore<AdvancedToolsActions>.Publish(
                    MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTED_ACTION_CHANGED
                    , selectedAction.Value);
        }
    }
}
