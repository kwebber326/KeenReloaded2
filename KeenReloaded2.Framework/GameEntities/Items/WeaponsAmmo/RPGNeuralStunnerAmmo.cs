using KeenReloaded.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo
{
    public class RPGNeuralStunnerAmmo : NeuralStunnerAmmo
    {
        public RPGNeuralStunnerAmmo(SpaceHashGrid grid, Rectangle area, string imageName, int zIndex, int ammo)
            : base(grid, area, imageName, zIndex, ammo)
        {

        }

        protected override void Initialize()
        {
            this.SpriteList = new Image[]
            {
                Properties.Resources.neural_stunner_rocket_launcher1,
                Properties.Resources.neural_stunner_rocket_launcher2
            };

            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.neural_stunner_rocket_launcher_acquired
            };
           _sprite = Properties.Resources.neural_stunner_rocket_launcher1;
        }
    }
}
