using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Interfaces
{
    public interface IBiomeTile
    {
        string Biome { get; }

        void ChangeBiome(string biome);

        event EventHandler<ObjectEventArgs> BiomeChanged;
    }
}
