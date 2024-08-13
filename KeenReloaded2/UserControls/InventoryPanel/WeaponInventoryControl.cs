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
using KeenReloaded2.Framework.GameEntities.Weapons;

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

        public void AddNewWeapon(NeuralStunner weapon, bool makeSelected = true)
        {
            var existingWeapons = pnlWeapons.Controls.OfType<WeaponDisplayControl>();
           
            if (existingWeapons.Any())
            {
                if (makeSelected)
                {
                    foreach (var ctrl in existingWeapons)
                    {
                        ctrl.Selected = false;
                    }
                }
                int ordinalPosition = existingWeapons.Count() + 1;
                var lastweapon = existingWeapons.LastOrDefault();
                WeaponDisplayControl control = new WeaponDisplayControl(weapon, ordinalPosition, makeSelected);
                if (lastweapon != null)
                {
                    control.Location = new Point(0, lastweapon.Bottom + 1);
                }
                pnlWeapons.Controls.Add(control);
            }
            else
            {
                WeaponDisplayControl control = new WeaponDisplayControl(weapon, 1, true);
                pnlWeapons.Controls.Add(control);
            }
        }

        public void ChangeSelection(NeuralStunner weapon)
        {
            var weapons = pnlWeapons.Controls.OfType<WeaponDisplayControl>();
            if (!weapons.Any())
                return;

            var previousSelection = weapons.FirstOrDefault(w => w.Selected);
            if (previousSelection != null)
            {
                previousSelection.Selected = false;
            }

            var newSelectedWeapon = weapons.FirstOrDefault(f => f.Weapon == weapon);
            if (newSelectedWeapon != null)
            {
                newSelectedWeapon.Selected = true;
            }
            else
            {
                this.AddNewWeapon(weapon);
            }
        }

        public void UpdateWeapon(NeuralStunner weapon, bool selected = true)
        {
            var weapons = pnlWeapons.Controls.OfType<WeaponDisplayControl>();
            if (!weapons.Any())
                return;

            var selectedWeapon = weapons.FirstOrDefault(f => f.Weapon == weapon);
            if (selectedWeapon != null)
            {
                if (selected)
                {
                    this.ChangeSelection(weapon);
                }
                selectedWeapon.UpdateWeaponDisplay(weapon, selected);
            }
            else
            {
                this.AddNewWeapon(weapon, selected);
            }
        }
    }
}
