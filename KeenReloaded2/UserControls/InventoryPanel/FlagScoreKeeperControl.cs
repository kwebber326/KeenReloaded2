using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Framework.GameEntities.Items;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class FlagScoreKeeperControl : UserControl
    {
        private List<Flag> _flags = new List<Flag>();
        private readonly GemColor _flagColor;
        public event EventHandler FlagsRemoved;

        public FlagScoreKeeperControl(GemColor color)
        {
            InitializeComponent();
            _flagColor = color;
            InitializeFlagColor();
        }

        private void InitializeFlagColor()
        {
            switch (_flagColor)
            {
                case GemColor.BLUE:
                    pbFlag.Image = Properties.Resources.Blue_Flag;
                    break;
                case GemColor.GREEN:
                    pbFlag.Image = Properties.Resources.Green_Flag;
                    break;
                case GemColor.RED:
                    pbFlag.Image = Properties.Resources.Red_Flag;
                    break;
                case GemColor.YELLOW:
                    pbFlag.Image = Properties.Resources.Yellow_Flag;
                    break;
            }
        }

        private void FlagScoreKeeperControl_Load(object sender, EventArgs e)
        {

        }

        public GemColor Color
        {
            get
            {
                return _flagColor;
            }
        }

        public void AddFlag(Flag flag)
        {
            if (flag == null)
                return;

            _flags.Add(flag);
            flag.FlagPointsChanged += Flag_FlagPointsChanged;
            flag.FlagCaptured += Flag_FlagCaptured;

            UpdateInventoryDisplay();
        }

        public void RemoveFlag(Flag flag)
        {
            if (flag == null)
                return;

            _flags.Remove(flag);
            flag.FlagPointsChanged -= Flag_FlagPointsChanged;
            flag.FlagCaptured -= Flag_FlagCaptured;
            UpdateInventoryDisplay();
        }

        private void UpdateInventoryDisplay()
        {
            int totalScore = _flags.Select(f => f.CurrentPointValue).Sum();
            lblPoints.Text = totalScore.ToString();
        }

        private void Flag_FlagCaptured(object sender, Framework.GameEventArgs.FlagCapturedEventArgs e)
        {
            if (e?.Flag != null)
                this.RemoveFlag(e.Flag);

            if (!_flags.Any())
            {
                this.FlagsRemoved?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Flag_FlagPointsChanged(object sender, Framework.GameEventArgs.FlagCapturedEventArgs e)
        {
            if (e?.Flag == null)
                return;

            this.UpdateInventoryDisplay();
        }
    }
}
