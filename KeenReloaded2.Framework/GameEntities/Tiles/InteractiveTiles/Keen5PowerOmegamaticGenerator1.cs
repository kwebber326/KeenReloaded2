using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles
{
    public class Keen5PowerOmegamaticGenerator1 : Keen5PowerGenerator
    {
        private const int GLASS_Y_OFFSET = 56;
        private InvisibleTile _metalPipeCollisionArea;

        public Keen5PowerOmegamaticGenerator1(Rectangle area, SpaceHashGrid grid, int zIndex, ObjectiveEventType eventType, IActivateable[] activateables) 
            : base(area, grid, zIndex, eventType, activateables)
        {
            AddMetalPipeHitbox();
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        protected override int GLASS_X_OFFSET => 172;

        protected override List<ICrossBar> CrossBars => new List<ICrossBar>
        {
            new Generator1CrossBar()
        };

        protected override Image[] SpriteList => SpriteSheet.SpriteSheet.Keen5PowerGenerator1Images;

        protected override int SPRITE_CHANGE_DELAY => 4;

        protected override int BLOCK_VERTICAL_OFFSET => 28;

        protected override int[] YOffsets => new int[] { GLASS_Y_OFFSET };

        protected override int LEFT_BLOCK_HORIZONTAL_OFFSET => 32;

        protected override int RIGHT_BLOCK_HORIZONTAL_OFFSET => 228;

        protected override int LEFT_BLOCK_WIDTH => 124;

        protected override int RIGHT_BLOCK_WIDTH => 70;

        private void AddMetalPipeHitbox()
        {
            //metal pipe hitbox
            Rectangle metalPipeHitbox = new Rectangle(_area.X, _area.Y + 74, 26, 46);
            _metalPipeCollisionArea = new InvisibleTile(_collisionGrid, metalPipeHitbox, true);
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen5_omegamatic_first_machine1);
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

    class Generator1CrossBar : ICrossBar
    {
        public int HorizontalOffset => -22;

        public int VerticalOffset => 164;

        public int ZIndex => 201;

        public Image Image => Properties.Resources.keen5_omegamatic_first_machine_crossbar;

        public Point Location => new Point(HorizontalOffset, VerticalOffset);

        public bool CanUpdate => true;
    }
}
