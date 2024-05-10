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
        public const string BIOME_KEEN4_FOREST = "KEEN4_GREEN";
        public const string BIOME_KEEN4_MIRAGE = "KEEN4_MIRAGE";
        public const string BIOME_KEEN4_CAVE = "KEEN4_CAVE";
        public const string BIOME_KEEN4_PYRAMID = "KEEN4_PYRAMID";

        public const string BIOME_KEEN5_BLACK = "KEEN5_BLACK";
        public const string BIOME_KEEN5_RED = "KEEN5_RED";
        public const string BIOME_KEEN5_GREEN = "KEEN5_GREEN";

        public const string BIOME_KEEN6_DOME = "KEEN6_DOME";
        public const string BIOME_KEEN6_FOREST = "KEEN6_FOREST";
        public const string BIOME_KEEN6_INDUSTRIAL = "KEEN6_INDUSTRIAL";
        public const string BIOME_KEEN6_FINAL = "KEEN6_FINAL";

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
