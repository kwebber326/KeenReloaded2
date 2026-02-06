using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles
{
    public class Keen5PowerOmegamaticGenerator4 : Keen5PowerGenerator
    {
        private List<ICrossBar> _crossbars = new List<ICrossBar>();
        private readonly int GLASS_Y_OFFSET1 = 56;
        private readonly int GLASS_Y_OFFSET2 = 122;
        private readonly int GLASS_Y_OFFSET3 = 188;
        private readonly int GLASS_Y_OFFSET4 = 254;
        private readonly Generator4Spinner _spinner;

        public Keen5PowerOmegamaticGenerator4(Rectangle area, SpaceHashGrid grid, int zIndex, ObjectiveEventType eventType, IActivateable[] activateables) : base(area, grid, zIndex, eventType, activateables)
        {
            Generator4Crossbar crossbar = new Generator4Crossbar();
            _spinner = new Generator4Spinner();
            _crossbars = new List<ICrossBar>() { crossbar };
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        protected override int GLASS_X_OFFSET => 172;

        protected override List<ICrossBar> CrossBars => _crossbars;

        protected override Image[] SpriteList => SpriteSheet.SpriteSheet.Keen5PowerGenerator4Images;

        protected override int SPRITE_CHANGE_DELAY => 2;

        protected override int BLOCK_VERTICAL_OFFSET => 28;

        protected override int[] YOffsets => new int[] { GLASS_Y_OFFSET1, GLASS_Y_OFFSET2, GLASS_Y_OFFSET3, GLASS_Y_OFFSET4 };

        protected override int LEFT_BLOCK_HORIZONTAL_OFFSET => 2;

        protected override int RIGHT_BLOCK_HORIZONTAL_OFFSET => 226;

        protected override int LEFT_BLOCK_WIDTH => 138;

        protected override int RIGHT_BLOCK_WIDTH => 76;

        protected override void DrawCombinedImage(bool initialDrawing = false)
        {
            List<Point> points = new List<Point>() { new Point(0, 0) };
            List<Image> images = new List<Image>() { _sprite };
            for (int i = 0; i < this.YOffsets.Length; i++)
            {
                int yOffset = this.YOffsets[i];
                Image newImg = initialDrawing
                ? SpriteSheet.SpriteSheet.Keen5GlassGeneratorSprites.FirstOrDefault()
                : _glassObjects?[i]?.Image;

                points.Add(new Point(GLASS_X_OFFSET, yOffset));
                images.Add(newImg);

                if (_spinner != null)
                {
                    points.Add(new Point(_spinner.Location.X, _spinner.Location.Y));
                    images.Add(_spinner.Image);
                }
            }

            _sprite = BitMapTool.DrawImagesOnCanvas(_area.Size,
                null, images.ToArray(), points.ToArray());
        }


        public override void Update()
        {
            base.Update();
            _spinner.Update();
            this.DrawCombinedImage();
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen5_omegamatic_fourth_machine1);
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

    class Generator4Spinner : ICrossBar, IUpdatable
    {
        private readonly Image[] _sprites = SpriteSheet.SpriteSheet.Keen5PowerGenerator4SpinnerImages;
        private const int SPRITE_CHANGE_DELAY = 2;
        private int _currentSpriteChangeDelayTick;
        private int _currentSpriteIndex;
        private Image _sprite;
        public Generator4Spinner()
        {
            _sprite = _sprites[_currentSpriteIndex];
        }

        public int HorizontalOffset => 0;

        public int VerticalOffset => 96;

        public int ZIndex => 201;

        public Image Image => _sprite;

        public Point Location => new Point(HorizontalOffset, VerticalOffset);

        public bool CanUpdate => true;

        public void Update()
        {
            if (_currentSpriteChangeDelayTick++ >= SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                if (++_currentSpriteIndex >= _sprites.Length)
                    _currentSpriteIndex = 0;
            }
            _sprite = _sprites[_currentSpriteIndex];
        }
    }

    class Generator4Crossbar : ICrossBar
    {
        public int HorizontalOffset => -40;

        public int VerticalOffset => 290;

        public int ZIndex => 201;

        public Image Image => Properties.Resources.keen5_omegamatic_fourth_machine_crossbar;

        public Point Location => new Point(HorizontalOffset, VerticalOffset);

        public bool CanUpdate => true;
    }
}
