using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Constants;

namespace KeenReloaded2.Framework.ReferenceDataClasses
{
    public static class Enemies
    {
        public static readonly Dictionary<string, string> EnemyToEpisodeMapping = new Dictionary<string, string>()
        {
            //keen 4 enemies
            { typeof(PoisonSlug).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(Lick).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(Bounder).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(Wormmouth).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(MadMushroom).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(Berkeloid).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(SkyPest).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(Mimrock).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(BlueEagleEgg).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(BlueEagle).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(Dopefish).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(ThunderCloud).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(Arachnut).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(Keen4Sprite).Name, GeneralGameConstants.Episodes.EPISODE_4 },
            { typeof(GnosticeneAncient).Name, GeneralGameConstants.Episodes.EPISODE_4 },

             //keen 5 enemies
            { typeof(Sparky).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(LittleAmpton).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(DiagonalSlicestar).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(HorizontalSlicestar).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(VerticalSlicestar).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(Shikadi).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(Shockshund).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(ShikadiMine).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(ShikadiMaster).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(Spirogrip).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(Spindred).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(RoboRed).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(VolteFace).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(Sphereful).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(KorathInhabitant).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            { typeof(Shelley).Name, GeneralGameConstants.Episodes.EPISODE_5 },
            //keen 6 enemies
            { typeof(Bloog).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Blooglet).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Blooguard).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Blorb).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Babobba).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Bobba).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Ceilick).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Gix).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Orbatrix).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Fleex).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Flect).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(NoSpike).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(BipShip).Name, GeneralGameConstants.Episodes.EPISODE_6 },
            { typeof(Bip).Name, GeneralGameConstants.Episodes.EPISODE_6 },
        };
    }
}
