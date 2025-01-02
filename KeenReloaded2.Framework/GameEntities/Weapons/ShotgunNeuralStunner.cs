using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Weapons
{
    public class ShotgunNeuralStunner : NeuralStunner
    {
        public ShotgunNeuralStunner(SpaceHashGrid grid, Rectangle hitbox, int ammo = 25)
            : base(grid, hitbox, ammo)
        {
            VELOCITY = 90;
            SPREAD = 10;
            REFIRE_DELAY = 10;
            SHOTS_PER_FIRE = 5;
            _fire_sound = GeneralGameConstants.Sounds.THUNDER;
        }
    }
}
