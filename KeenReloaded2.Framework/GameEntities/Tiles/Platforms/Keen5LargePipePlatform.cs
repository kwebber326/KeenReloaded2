using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class Keen5LargePipePlatform : MaskedTile
    {
        public Keen5LargePipePlatform(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(area, grid, area, null, zIndex)
        {
            _downwardCollisionOffset = 32;
            SetSprite();
            this.AdjustHitboxBasedOnOffsets();
        }

        protected void SetSprite()
        {
            _image = Properties.Resources.keen5_pipe_platform;
        }

        public override CollisionType CollisionType => CollisionType.PLATFORM;

        public override string ToString()
        {
            return $"{nameof(Properties.Resources.keen5_pipe_platform)}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}";
        }
    }
}
