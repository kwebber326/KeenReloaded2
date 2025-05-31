using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints
{
    public class SpinningCheckpointFlag : CheckPointFlag
    {
        private readonly Image[] _flippingSprites = new Image[]
        {
        };

        private readonly Image[] _landingSprites = new Image[]
        {
        };

        private readonly Image[] _wavingSprites = new Image[]
        {
            Properties.Resources.keen5_flag1,
            Properties.Resources.keen5_flag2,
            Properties.Resources.keen5_flag3,
            Properties.Resources.keen5_flag4,
        };

        public SpinningCheckpointFlag(SpaceHashGrid grid, Rectangle hitbox, int zIndex, CheckPointFlagState initialState) : base(grid, hitbox, zIndex, initialState)
        {
        }

        protected override Image[] FlippingSprites => _flippingSprites;

        protected override Image[] LandingSprites => _landingSprites;

        protected override Image[] WavingSprites => _wavingSprites;
    }
}
