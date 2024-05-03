using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Constants;

namespace KeenReloaded2.Framework.ReferenceDataClasses
{
    public static class Biomes
    {
        public const string BIOME_KEEN4_FOREST = "Keen 4 Forest";
        public const string BIOME_KEEN4_MIRAGE = "Keen 4 Mirage";
        public const string BIOME_KEEN4_CAVE = "Keen 4 Cave";
        public const string BIOME_KEEN4_PYRAMID = "Keen 4 Pyramid";

        public const string BIOME_KEEN5_BLACK = "Keen 5 Black";
        public const string BIOME_KEEN5_RED = "Keen 5 Red";
        public const string BIOME_KEEN5_GREEN = "Keen 5 Green";

        public const string BIOME_KEEN6_DOME = "Keen 6 Dome";
        public const string BIOME_KEEN6_FOREST = "Keen 6 Forest";
        public const string BIOME_KEEN6_INDUSTRIAL = "Keen 6 Industrial";
        public const string BIOME_KEEN6_FINAL = "Keen 6 Final";

        public static readonly Dictionary<string, List<string>> BiomeRepository
            = new Dictionary<string, List<string>>()
        {
                {
                    GeneralGameConstants.Episodes.EPISODE_4, new List<string>()
                    {
                        BIOME_KEEN4_CAVE,
                        BIOME_KEEN4_FOREST,
                        BIOME_KEEN4_MIRAGE,
                        BIOME_KEEN4_PYRAMID
                    }
                },
                {
                    GeneralGameConstants.Episodes.EPISODE_5, new List<string>()
                    {
                        BIOME_KEEN5_BLACK,
                        BIOME_KEEN5_GREEN,
                        BIOME_KEEN5_RED
                    }
                },
                {
                    GeneralGameConstants.Episodes.EPISODE_6, new List<string>()
                    {
                        BIOME_KEEN6_DOME,
                        BIOME_KEEN6_FINAL,
                        BIOME_KEEN6_FOREST,
                        BIOME_KEEN6_INDUSTRIAL
                    }
                }
        };
    }
}
