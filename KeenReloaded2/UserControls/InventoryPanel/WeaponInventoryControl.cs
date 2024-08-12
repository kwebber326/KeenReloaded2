using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Framework.GameEntities.Players;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class WeaponInventoryControl : UserControl
    {
        public WeaponInventoryControl()
        {
            InitializeComponent();
        }

        private void WeaponInventoryControl_Load(object sender, EventArgs e)
        {

        }

        public void SetWeaponInventory(CommanderKeen keen)
        {
            pnlWeapons.Controls.Clear();
           
            if (keen == null)
                return;

            if (keen.CurrentWeapon != null)
            {
                lblSelectedWeapon.Text = keen.CurrentWeapon.GetType().Name;
            }

            int y = 0;
            for (int i = 0; i < keen.Weapons.Count; i++)
            {
                var weapon = keen.Weapons[i];
                WeaponDisplayControl control = new WeaponDisplayControl(weapon, i + 1, weapon == keen.CurrentWeapon);
                control.Location = new Point(0, y);
                pnlWeapons.Controls.Add(control);
                y = control.Bottom + 1;
            }
        }
    }
}
