using KeenReloaded.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo
{
    public class BoobusBombLauncherAmmo : NeuralStunnerAmmo
    {
        public BoobusBombLauncherAmmo(SpaceHashGrid grid, Rectangle area, string imageName, int zIndex, int ammo)
            : base(grid, area, imageName, zIndex, ammo)
        {

        }
        protected override void Initialize()
        {
            this.SpriteList = new Image[]
            {
                Properties.Resources.keen_dreams_boobus_bomb1,
                Properties.Resources.keen_dreams_boobus_bomb2
            };

            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.keen_dreams_boobus_bomb_acquired
            };
            _sprite = Properties.Resources.keen_dreams_boobus_bomb2;
        }
    }
}
