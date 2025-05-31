using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints
{
    public class FlippingCheckpointFlag : CheckPointFlag
    {
        private readonly Image[] _flippingSprites = new Image[]
        {
            Properties.Resources.keen_flipping_flag1,
            Properties.Resources.keen_flipping_flag2,
            Properties.Resources.keen_flipping_flag3,
            Properties.Resources.keen_flipping_flag4
        };

        private readonly Image[] _landingSprites = new Image[]
        {
            Properties.Resources.keen_landing_flag1,
            Properties.Resources.keen_landing_flag2,
        };

        private readonly Image[] _wavingSprites = new Image[]
        {
            Properties.Resources.keen_flag_waving1,
            Properties.Resources.keen_flag_waving2,
            Properties.Resources.keen_flag_waving3,
            Properties.Resources.keen_flag_waving4,
        };

        public FlippingCheckpointFlag(SpaceHashGrid grid, Rectangle hitbox, int zIndex) : base(grid, hitbox, zIndex, CheckPointFlagState.FLIPPING)
        {   
           
        }

        protected override Image[] FlippingSprites => _flippingSprites;

        protected override Image[] LandingSprites => _landingSprites;

        protected override Image[] WavingSprites => _wavingSprites;
    }
}
