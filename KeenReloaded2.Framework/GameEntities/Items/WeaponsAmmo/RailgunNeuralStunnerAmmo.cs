using KeenReloaded.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo
{
    public class RailgunNeuralStunnerAmmo : NeuralStunnerAmmo
    {
        public RailgunNeuralStunnerAmmo(SpaceHashGrid grid, Rectangle area, string imageName, int zIndex, int ammo)
            : base(grid, area, imageName, zIndex, ammo)
        {

        }

        protected override void Initialize()
        {
            this.SpriteList = new Image[]
            {
                Properties.Resources.neural_stunner_railgun1,
                Properties.Resources.neural_stunner_railgun2
            };

            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.neural_stunner_railgun_acquired
            };
            _sprite = Properties.Resources.neural_stunner_railgun1;
        }
    }
}
