using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using KeenReloaded.Framework;

namespace KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo
{
    public class ShotgunNeuralStunnerAmmo : NeuralStunnerAmmo
    {
        public ShotgunNeuralStunnerAmmo(SpaceHashGrid grid, Rectangle area, string imageName, int zIndex, int ammo)
            : base(grid, area, imageName, zIndex, ammo)
        {

        }

        protected override void Initialize()
        {
            this.SpriteList = new Image[]
            {
                Properties.Resources.neural_stunner_shotgun,
                Properties.Resources.neural_stunner_shotgun2
            };

            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.neural_stunner_shotgun_acquired
            };
            _sprite = Properties.Resources.neural_stunner_shotgun;
        }
    }
}
