using KeenReloaded.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo
{
    public class BFGAmmo : NeuralStunnerAmmo
    {
        public BFGAmmo(SpaceHashGrid grid, Rectangle area, string imageName, int zIndex, int ammo)
            : base(grid, area, imageName, zIndex, ammo)
        {

        }

        protected override void Initialize()
        {
            this.SpriteList = new Image[]
            {
                Properties.Resources.BFG1,
                Properties.Resources.BFG2
            };

            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.BFG_acquired
            };
            _sprite = this.SpriteList.FirstOrDefault();
        }
    }
}
