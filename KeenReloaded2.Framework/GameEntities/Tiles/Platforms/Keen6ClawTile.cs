using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class Keen6ClawTile : SinglePlatform
    {
        private const int EDGE_HORIZONTAL_OFFSET_LEFT = 32;
        private const int VERTICAL_OFFSET = 64;
        private const int EDGE_HORIZONTAL_OFFSET_RIGHT = 32;
        private const int ARM_LENGTH = 62;
        private const int ARM_WIDTH = 64;
        private const int ARM_HORIZONTAL_OFFSET = 28;

        private readonly int _addedLengths;

        public Keen6ClawTile(Rectangle area, SpaceHashGrid grid, int zIndex, int addedLengths = 0)
            : base(area, grid, area, null, zIndex, Biomes.BIOME_KEEN6_FINAL)
        {
            _addedLengths = addedLengths;
            Initialize();
        }

        public List<LocatedImage> ArmLengths
        {
            get; private set;
        }

        private void AddArmLengths()
        {
            this.ArmLengths = new List<LocatedImage>();
            int addedHeight = Properties.Resources.keen6_claw_platform_arm.Height;
            int totalHeight = _image.Height + (addedHeight * _addedLengths);
            Rectangle totalArea = new Rectangle(0, 0, _image.Width, totalHeight);
            for (int i = 0; i < _addedLengths; i++)
            {
                int offsetY = i * ARM_LENGTH;
                LocatedImage p = new LocatedImage();
                p.Image = Properties.Resources.keen6_claw_platform_arm;
                int x = ARM_HORIZONTAL_OFFSET;
                int y = _image.Height + offsetY;
                p.Location = new Point(x, y);
                this.ArmLengths.Add(p);
            }
            var images = this.ArmLengths.Select(s => s.Image).ToArray();
            var locations = this.ArmLengths.Select(s => s.Location).ToArray();
            _image = BitMapTool.DrawImagesOnCanvas(totalArea.Size, _image, images, locations);
        }

        protected void SetSprite()
        {
            _image = Properties.Resources.keen6_claw_platform;
            _initialImageName = nameof(Properties.Resources.keen6_claw_platform);
        }

        protected void Initialize()
        {
            SetSprite();

            if (_collisionGrid != null && _collidingNodes != null)
            {
                this.HitBox = new Rectangle(
                    this.HitBox.X + EDGE_HORIZONTAL_OFFSET_LEFT //x
                  , this.HitBox.Y + VERTICAL_OFFSET   //y
                  , this.HitBox.Width - EDGE_HORIZONTAL_OFFSET_LEFT - EDGE_HORIZONTAL_OFFSET_RIGHT //width
                  , this.HitBox.Height - VERTICAL_OFFSET); //height
            }

            AddArmLengths();
        }

        public override string ToString()
        {
            return $"{_initialImageName}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}{_separator}{this.ArmLengths?.Count ?? 0}";
        }
    }
}

