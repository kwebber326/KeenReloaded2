using KeenReloaded2.Framework.GameEntities.Items;
using KeenReloaded2.Framework.GameEntities.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities
{
    public class WorldMapPlayerInventoryState
    {
        public long PlayerPoints { get; set; }

        public int PlayerLives { get; set; }

        public List<Gem> PlayerGems { get; set; }

        public List<NeuralStunner> PlayerWeapons { get; set; }

        public bool HasKeyCard { get; set; }
    }
}
