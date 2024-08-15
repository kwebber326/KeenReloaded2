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
        private const int HORIZONTAL_OFFSET = 2;
        private const int VERTICAL_OFFSET = 24;
        private const int MARGIN = 4;

        private List<FlagScoreKeeperControl> _flagControls = new List<FlagScoreKeeperControl>();

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

            var flagControl = _flagControls.FirstOrDefault(f => f.Color == flag.Color);
            if (flagControl != null)
            {
                flagControl.AddFlag(flag);
            }
            else
            {
                FlagScoreKeeperControl newControl = new FlagScoreKeeperControl(flag.Color);
                _flagControls.Add(newControl);
                newControl.FlagsRemoved += FlagScoreKeeper_FlagsRemoved;
                newControl.AddFlag(flag);
            }
            AlignFlagControls();
        }

        public void RemoveFlag(Flag flag)
        {
            if (flag == null)
                return;

            var flagControl = _flagControls.FirstOrDefault(f => f.Color == flag.Color);
            if (flagControl != null)
            {
                flagControl.RemoveFlag(flag);
                AlignFlagControls();
            }
        }

        private void FlagScoreKeeper_FlagsRemoved(object sender, EventArgs e)
        {
            var flagControl = sender as FlagScoreKeeperControl;
            if (flagControl != null)
            {
                flagControl.FlagsRemoved -= FlagScoreKeeper_FlagsRemoved;
                _flagControls.Remove(flagControl);
                AlignFlagControls();
            }
        }

        private void AlignFlagControls()
        {
            for (int i = 0; i < _flagControls.Count; i++)
            {
                int x = (i + 1) % 2 != 0 ? HORIZONTAL_OFFSET : HORIZONTAL_OFFSET + MARGIN + _flagControls[i].Width;
                int y = (i + 1) <= 2 ? VERTICAL_OFFSET : VERTICAL_OFFSET + (MARGIN + _flagControls[i].Height * (i / 2));
                _flagControls[i].Location = new Point(x, y);
                _flagControls[i].BringToFront();
            }
        }
    }
}
