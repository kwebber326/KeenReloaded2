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
    public partial class FlagInventoryBoard : UserControl
    {
        private List<Flag> _flags = new List<Flag>();
        private Dictionary<GemColor, Image> _flagColorImageMapping = new Dictionary<GemColor, Image>()
        {
            
        };

        public FlagInventoryBoard()
        {
            InitializeComponent();
        }

        private void FlagInventoryBoard_Load(object sender, EventArgs e)
        {

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

            _flags.Add(flag);
            flag.FlagPointsChanged -= FlagInventoryBoard_Load;
            flag.FlagCaptured -= Flag_FlagCaptured;
            UpdateInventoryDisplay();
        }

        private void UpdateInventoryDisplay()
        {
            var flagGroupings = _flags.GroupBy(f => f.Color);
            foreach (var grouping in flagGroupings)
            {
                int totalScore = grouping.Select(f => f.CurrentPointValue).Sum();
                GemColor color = grouping.Key;
            }
        }

        private void Flag_FlagCaptured(object sender, Framework.GameEventArgs.FlagCapturedEventArgs e)
        {
            if (e?.Flag != null)
                this.RemoveFlag(e.Flag);
        }

        private void Flag_FlagPointsChanged(object sender, Framework.GameEventArgs.FlagCapturedEventArgs e)
        {
            if (e?.Flag == null)
                return;

            this.UpdateInventoryDisplay();
        }

    }
}
