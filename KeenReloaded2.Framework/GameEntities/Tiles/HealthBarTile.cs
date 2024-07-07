using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class HealthBarTile : MaskedTile, IHealthBar
    {
        private ProgressBar _healthBar;

        public HealthBarTile(SpaceHashGrid grid, Rectangle hitbox, int health, bool initallyVisible = false) : base(hitbox, grid, hitbox, null, 20)
        {
            _healthBar = new ProgressBar();
            _healthBar.Maximum = health;
            _healthBar.Value = health;
            _healthBar.Visible = initallyVisible;
            _healthBar.Location = new Point(hitbox.X - 100, hitbox.Y + (hitbox.Height / 2) + (hitbox.Height / 4));
        }

        public void SetHealthBarVisiblity(bool visible)
        {
            _healthBar.Visible = visible;
        }

        public void SetHealthBarValue(int val)
        {
            _healthBar.Value = val < 0 ? 0 : val;
        }

        public ProgressBar HealthBar => _healthBar;

        public override CollisionType CollisionType => CollisionType.NONE;
    }
}
