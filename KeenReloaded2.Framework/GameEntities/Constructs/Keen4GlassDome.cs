using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Keen4GlassDome : CollisionObject, ISprite
    {
        private readonly int _zindex;
        private readonly int _extraWidths;
        private readonly int _extraHeights;
        private readonly Keen4GlassDomeLeftArchwayType _leftSideArchway;
        private readonly Keen4GlassDomeRightArchwayType _rightSideArchway;
        private Image _sprite;

        public Keen4GlassDome(Rectangle area, SpaceHashGrid grid, int zIndex, int extraWidths, int extraHeights, Keen4GlassDomeLeftArchwayType leftSide, Keen4GlassDomeRightArchwayType rightSide) 
            : base(grid, area)
        {
            _zindex = zIndex;
            _extraWidths = extraWidths;
            _extraHeights = extraHeights;
            _leftSideArchway = leftSide;
            _rightSideArchway = rightSide;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            int currentX = 0, currentY = 0;
            //top
            Image topLeft = Properties.Resources.keen4_glass_dome_top_left;
            int totalWidth = topLeft.Width, totalHeight = topLeft.Height;
            List<LocatedImage> locatedImages = new List<LocatedImage>()
            {
                new LocatedImage() { Image = topLeft, Location = new Point(currentX, currentY) }
            };
            //top left corner collision
            if (_collisionGrid != null)
            {
                ConstructTopLeftCornerCollisionArch();
            }
            //
            for (int i = 0; i < _extraWidths; i++)
            {
                Image img = Properties.Resources.keen4_glass_dome_extra_top_width;
                int x = (topLeft.Width + 1) + (i * img.Width);
                int y = currentY;
                var locatedImage = new LocatedImage() { Image = img, Location = new Point(x, y) };
                locatedImages.Add(locatedImage);
                totalWidth += img.Width;
                if (_collisionGrid != null && _collidingNodes != null)
                {
                    var middleCeilingTile = new InvisibleTile(_collisionGrid, new Rectangle(x + this.HitBox.X, y + this.HitBox.Y, img.Width, 2));
                }
            }
            Image topRight = Properties.Resources.keen4_glass_dome_top_right;         
            totalWidth += topRight.Width;
            locatedImages.Add(new LocatedImage() { Image = topRight, Location = new Point(totalWidth - topRight.Width, currentY) });
            if (_collisionGrid != null)
            {
                ConstructTopRightCornerCollisionArch(this.HitBox.X + (totalWidth - topRight.Width));
            }
            //middle
            currentY = topLeft.Height + 1;
            for (int i = 0; i < _extraHeights; i++)
            {
                Image middleLeft = Properties.Resources.keen4_glass_dome_middle_left;
                Image middleRight = Properties.Resources.keen4_glass_dome_middle_right;
                int middleLeftX = 0, middleRightX = totalWidth - middleRight.Width;
                int y = (topRight.Height) + (i * middleLeft.Height);
                LocatedImage imgLeft = new LocatedImage() { Image = middleLeft, Location = new Point(middleLeftX, y) };
                LocatedImage imgRight = new LocatedImage() { Image = middleRight, Location = new Point(middleRightX, y) };
                locatedImages.Add(imgLeft);
                locatedImages.Add(imgRight);
                totalHeight += middleRight.Height;
                if (_collisionGrid != null && _collidingNodes != null)
                {
                    var leftWallTile = new InvisibleTile(_collisionGrid, new Rectangle(middleLeftX + this.HitBox.X, y + this.HitBox.Y, 2, imgLeft.Height));
                    var rightWallTile = new InvisibleTile(_collisionGrid, new Rectangle(middleRightX + this.HitBox.X + imgRight.Width - 8, y + this.HitBox.Y, 2, imgRight.Height));
                }
            }
            currentY = totalHeight;
            //bottom
            Image bottomLeft = GetBottomLeftImage();
            Image bottomRight = GetBottomRightImage();
            
            totalHeight += bottomLeft.Height;

            LocatedImage bottomLeftImg = new LocatedImage() { Image = bottomLeft, Location = new Point(0, currentY) };
            LocatedImage bottomRightImg = new LocatedImage() { Image = bottomRight, Location = new Point(totalWidth - bottomRight.Width, currentY) };
            locatedImages.Add(bottomLeftImg);
            locatedImages.Add(bottomRightImg);

            if (_collisionGrid != null && _leftSideArchway == Keen4GlassDomeLeftArchwayType.NONE)
            {
                InvisibleTile leftEntryBarrier = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X, this.HitBox.Y + bottomLeftImg.Location.Y, 2, bottomLeftImg.Height));
            }
            if (_collisionGrid != null && _rightSideArchway == Keen4GlassDomeRightArchwayType.NONE)
            {
                InvisibleTile rightEntryBarrier = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + totalWidth - 8, this.HitBox.Y + bottomRightImg.Location.Y, 10, bottomRightImg.Height));
            }

            for (int i = 0; i < _extraWidths; i++)
            {
                Image middleBottom = Properties.Resources.keen4_glass_dome_extra_bottom_width;
                int x = (bottomLeftImg.Width + 1) + (i * middleBottom.Width);
                int y = totalHeight - middleBottom.Height;
                LocatedImage middleBottomImg = new LocatedImage() { Image = middleBottom, Location = new Point(x, y) };
                locatedImages.Add(middleBottomImg);
            }

            Size canvas = new Size(totalWidth, totalHeight);
            Image[] images = locatedImages.Select(i => i.Image).ToArray();
            Point[] locations = locatedImages.Select(i => i.Location).ToArray();
            _sprite = BitMapTool.DrawImagesOnCanvas(canvas, null, images, locations);
        }

        private void ConstructTopRightCornerCollisionArch(int startX)
        {
            InvisibleTile t1 = new InvisibleTile(_collisionGrid, new Rectangle(startX, this.HitBox.Y, 16, 2));
            InvisibleTile t2 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 16, this.HitBox.Y + 2, 8, 2));
            InvisibleTile t3 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 24, this.HitBox.Y + 4, 6, 2));
            InvisibleTile t4 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 30, this.HitBox.Y + 6, 4, 2));
            InvisibleTile t5 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 34, this.HitBox.Y + 8, 4, 2));
            InvisibleTile t6 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 38, this.HitBox.Y + 10, 2, 2));
            InvisibleTile t7 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 40, this.HitBox.Y + 12, 4, 2));
            InvisibleTile t8 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 44, this.HitBox.Y + 14, 4, 2));
            InvisibleTile t9 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 48, this.HitBox.Y + 16, 2, 2));
            InvisibleTile t10 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 50, this.HitBox.Y + 18, 2, 2));
            InvisibleTile t11 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 52, this.HitBox.Y + 20, 2, 2));
            InvisibleTile t12 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 54, this.HitBox.Y + 22, 2, 4));
            InvisibleTile t13 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 58, this.HitBox.Y + 26, 2, 2));
            InvisibleTile t14 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 60, this.HitBox.Y + 28, 2, 4));
            InvisibleTile t15 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 62, this.HitBox.Y + 32, 2, 2));
            InvisibleTile t16 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 64, this.HitBox.Y + 34, 2, 4));
            InvisibleTile t17 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 66, this.HitBox.Y + 38, 2, 6));
            InvisibleTile t18 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 66, this.HitBox.Y + 44, 2, 8));
            InvisibleTile t19 = new InvisibleTile(_collisionGrid, new Rectangle(startX + 66, this.HitBox.Y + 52, 2, 42));

        }

        private void ConstructTopLeftCornerCollisionArch()
        {
            InvisibleTile t1 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X, this.HitBox.Y + 50, 4, 42));
            InvisibleTile t2 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 2, this.HitBox.Y + 42, 4, 12));
            InvisibleTile t3 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 4, this.HitBox.Y + 36, 4, 10));
            InvisibleTile t4 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 6, this.HitBox.Y + 32, 6, 4));
            InvisibleTile t5 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 8, this.HitBox.Y + 30, 6, 2));
            InvisibleTile t6 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 10, this.HitBox.Y + 26, 6, 4));
            InvisibleTile t7 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 12, this.HitBox.Y + 24, 8, 2));
            InvisibleTile t8 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 14, this.HitBox.Y + 22, 6, 2));
            InvisibleTile t9 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 16, this.HitBox.Y + 20, 6, 4));
            InvisibleTile t10 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 18, this.HitBox.Y + 18, 10, 2));
            InvisibleTile t11 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 22, this.HitBox.Y + 14, 10, 2));
            InvisibleTile t12 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 24, this.HitBox.Y + 12, 10, 2));
            InvisibleTile t13 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 26, this.HitBox.Y + 10, 10, 2));
            InvisibleTile t14 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 30, this.HitBox.Y + 8, 10, 2));
            InvisibleTile t15 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 32, this.HitBox.Y + 6, 14, 2));
            InvisibleTile t16 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 36, this.HitBox.Y + 4, 18, 2));
            InvisibleTile t17 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 42, this.HitBox.Y + 2, 18, 2));
            InvisibleTile t18 = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + 50, this.HitBox.Y, 12, 2));
        }

        private Image GetBottomLeftImage()
        {
            switch (_leftSideArchway)
            {
                case Keen4GlassDomeLeftArchwayType.BLUE:
                    return Properties.Resources.keen4_glass_dome_bottom_left_blue_door;
                case Keen4GlassDomeLeftArchwayType.YELLOW:
                    return Properties.Resources.keen4_glass_dome_bottom_left_yellow_door;
                case Keen4GlassDomeLeftArchwayType.NONE:
                default:
                    return Properties.Resources.keen4_glass_dome_bottom_left_no_door;
            }
        }

        private Image GetBottomRightImage()
        {
            switch (_rightSideArchway)
            {
                case Keen4GlassDomeRightArchwayType.ARCHWAY:
                    return Properties.Resources.keen4_glass_dome_bottom_right_with_door;
                case Keen4GlassDomeRightArchwayType.NONE:
                default:
                    return Properties.Resources.keen4_glass_dome_bottom_right_no_door;
            }
        }

        public int ZIndex => _zindex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.NONE;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string key = nameof(Properties.Resources.keen4_glass_dome_top_left);
            var area = this.HitBox;
            return $"{key}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zindex}{separator}{_extraWidths}{separator}{_extraHeights}{separator}{_leftSideArchway}{separator}{_rightSideArchway}";
        }
    }

    public enum Keen4GlassDomeLeftArchwayType
    {
        NONE,
        YELLOW,
        BLUE
    }
    public enum Keen4GlassDomeRightArchwayType
    {
        NONE,
        ARCHWAY
    }
}
