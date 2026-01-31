using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles
{
    public class Keen5PowerOmegamaticGenerator3 : Keen5PowerGenerator
    {
        private readonly int GLASS_Y_OFFSET1 = 56;
        private readonly int GLASS_Y_OFFSET2 = 122;
        private readonly int GLASS_Y_OFFSET3 = 188;

        public Keen5PowerOmegamaticGenerator3(Rectangle area, SpaceHashGrid grid, int zIndex, ObjectiveEventType eventType, IActivateable[] activateables)
            : base(area, grid, zIndex, eventType, activateables)
        {

        }

        protected override int[] YOffsets
        {
            get
            {
                return new int[] { GLASS_Y_OFFSET1, GLASS_Y_OFFSET2, GLASS_Y_OFFSET3 };
            }
        }

        protected override int LEFT_BLOCK_HORIZONTAL_OFFSET
        {
            get
            {
                return 6;
            }
        }

        protected override int RIGHT_BLOCK_HORIZONTAL_OFFSET
        {
            get
            {
                return 200;
            }
        }

        protected override int LEFT_BLOCK_WIDTH
        {
            get
            {
                return 100;
            }
        }

        protected override int RIGHT_BLOCK_WIDTH
        {
            get
            {
                return 70;
            }
        }

        protected override int GLASS_X_OFFSET => 128;

        public override CollisionType CollisionType => CollisionType.NONE;

        protected override List<ICrossBar> CrossBars => new List<ICrossBar>
        {
            new Generator3CrossBar()
        };

        protected override Image[] SpriteList => SpriteSheet.SpriteSheet.Keen5PowerGenerator3Images;

        protected override int SPRITE_CHANGE_DELAY => 2;

        protected override int BLOCK_VERTICAL_OFFSET => 28;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen5_omegamatic_third_machine1);
            string arrayItemSeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;
            string data = $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_eventType}";
            if (_activateables != null)
            {
                data += $"{separator}{arrayStart}{string.Join(arrayItemSeparator, _activateables.Select(a => a.ActivationID))}{arrayEnd}";
            }
            return data;
        }
    }

    class Generator3CrossBar : ICrossBar
    {
        public int HorizontalOffset => -38;

        public int VerticalOffset => 218;

        public int ZIndex => 201;

        public Image Image => Properties.Resources.keen5_omegamatic_third_machine_crossbar;

        public Point Location => new Point(HorizontalOffset, VerticalOffset);

        public bool CanUpdate => true;
    }
}
