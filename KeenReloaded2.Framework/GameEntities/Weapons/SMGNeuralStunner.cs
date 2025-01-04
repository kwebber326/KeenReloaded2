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
    public class SMGNeuralStunner : NeuralStunner
    {
        public SMGNeuralStunner(SpaceHashGrid grid, Rectangle hitbox, int ammo = 10)
            : base(grid, hitbox, ammo)
        {
            SPREAD = 12;
            PIERCE = 1;
            DAMAGE = 1;
            REFIRE_DELAY = 3;
            IS_AUTO = true;
            _fire_sound = GeneralGameConstants.Sounds.SMG_ROUND;
        }
    }
}
