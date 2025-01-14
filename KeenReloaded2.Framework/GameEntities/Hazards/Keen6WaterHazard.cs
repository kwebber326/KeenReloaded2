using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen6WaterHazard : Hazard, IUpdatable
    {
        private readonly int _addedWidth;
        private readonly int _addedDepth;
        private Rectangle _area;
        private Image[] _leftSideImages = SpriteSheet.SpriteSheet.Keen6WaterHazardLeftImages;
        private Image[] _rightSideImages = SpriteSheet.SpriteSheet.Keen6WaterHazardRightImages;
        private const int LEFT_DEPTH_X_OFFSET = 24;
        private const int RIGHT_DEPTH_X_OFFSET = 8;
        private const int SPRITE_HEIGHT = 64;
        private readonly int DEPTH_HEIGHT = Properties.Resources.keen6_water_hazard_depth.Height;
        private const int HITBOX_VERTICAL_OFFSET = 32;
        private const int HITBOX_HORIZONTAL_OFFSET = 16;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick;
        private List<LocatedImage> _sprites;
        private List<LocatedImage> _depthSprites;
        private Image _leftSprite;
        private Image _rightSprite;
        private Image _middleSprite;
        private int _currentSpriteIndex = 0;

        public Keen6WaterHazard(Rectangle area, SpaceHashGrid grid, int zIndex, int addedWidth, int addedDepth)
            : base(grid, area, HazardType.KEEN6_WATER_HAZARD, zIndex)
        {
            _addedWidth = addedWidth;
            _addedDepth = addedDepth;
            _area = area;
            Initialize();
        }

        private void Initialize()
        {
            _sprites = new List<LocatedImage>();
            _depthSprites = new List<LocatedImage>();
            _leftSprite = Properties.Resources.keen6_water_hazard_left1;
            _middleSprite = Properties.Resources.keen6_water_hazard_middle;
            _rightSprite = Properties.Resources.keen6_water_hazard_right1;
            var depthSprite = Properties.Resources.keen6_water_hazard_depth;
            _sprites.Add(new LocatedImage() { Image = _leftSprite, Location = new Point(this.HitBox.X, this.HitBox.Y) });
            int currentX = this.HitBox.X + _leftSprite.Width;
            var totalWidth = (_leftSprite.Width + _rightSprite.Width) + (_middleSprite.Width * _addedWidth);
            for (int i = 0; i < _addedWidth; i++)
            {
                LocatedImage p = new LocatedImage();
                p.Location = new Point(currentX, this.HitBox.Y);
                p.Image = Properties.Resources.keen6_water_hazard_middle;

                _sprites.Add(p);

                //depth logic
                
                for (int j = 0; j < _addedDepth; j++)
                {
                    //left side
                    int yPos = this.HitBox.Bottom + (DEPTH_HEIGHT * j);
                    LocatedImage pLeft = new LocatedImage()
                    {
                        Location = new Point(this.HitBox.X + LEFT_DEPTH_X_OFFSET, yPos),
                        Image = depthSprite
                    };
                    _depthSprites.Add(pLeft);
                    //middle
                    LocatedImage pDepth = new LocatedImage();
                    pDepth.Location = new Point(currentX, yPos);
                    pDepth.Image = depthSprite;
                    _depthSprites.Add(pDepth);

                    //right side
                    LocatedImage pRight = new LocatedImage()
                    {
                        Location = new Point(this.HitBox.X + totalWidth - _middleSprite.Width - RIGHT_DEPTH_X_OFFSET, yPos),
                        Image = depthSprite
                    };
                    _depthSprites.Add(pRight);

                    if (j == 0 || (j + 1) % (SPRITE_HEIGHT / DEPTH_HEIGHT) == 0)
                    {
                        LocatedImage leftWallImg = new LocatedImage()
                        {
                            Image = Properties.Resources.keen6_forest_wall_edge_right,
                            Location = new Point(this.HitBox.X, yPos)
                        };
                        _sprites.Add(leftWallImg);

                        LocatedImage rightWallImg = new LocatedImage()
                        {
                            Image = Properties.Resources.keen6_water_edge_right,
                            Location = new Point(this.HitBox.X + totalWidth - RIGHT_DEPTH_X_OFFSET, yPos)
                        };
                        _sprites.Add(rightWallImg);
                    }
                }
                currentX += _middleSprite.Width;
            }

            _sprites.Add(new LocatedImage()
            {
                Image = _rightSprite,
                Location = new Point(currentX, this.HitBox.Y)
            });

            this.HitBox = new Rectangle(
                  this.HitBox.X + HITBOX_HORIZONTAL_OFFSET//x
                , this.HitBox.Y + HITBOX_VERTICAL_OFFSET//y
                , totalWidth - HITBOX_HORIZONTAL_OFFSET //width
                , this.HitBox.Height - HITBOX_VERTICAL_OFFSET + (_middleSprite.Height + (DEPTH_HEIGHT * _addedDepth)));//height

            this.DrawImage();
        }

        public IEnumerable<LocatedImage> Sprites
        {
            get
            {
                return _sprites;
            }
        }

        public IEnumerable<LocatedImage> DepthSprites
        {
            get
            {
                return _depthSprites;
            }
        }

        public void DrawImage()
        {
            var totalWidth = (_leftSprite.Width + _rightSprite.Width) + (_middleSprite.Width  * _addedWidth);
            var totalHeight = SPRITE_HEIGHT + (DEPTH_HEIGHT * _addedDepth);
            Size canvas = new Size(totalWidth, totalHeight);
            Image[] images = this.Sprites.ToList().Union(this.DepthSprites)
                .Select(s => s.Image).ToArray();
            Point[] locations = this.Sprites.ToList().Union(this.DepthSprites)
                .Select(s => new Point(s.Location.X - _area.X, s.Location.Y - _area.Y)).ToArray();
            _sprite = BitMapTool.DrawImagesOnCanvas(canvas, null, images, locations);
        }
        public override Rectangle HitBox
        {
            get => base.HitBox;
            protected set
            {
                base.HitBox = value;
                if (value != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        public override Point Location => _area.Location;

        public void Update()
        {
            UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSpriteIndex, SPRITE_CHANGE_DELAY,
                () =>
                {
                    if (_currentSpriteIndex >= _leftSideImages.Length || _currentSpriteIndex >= _rightSideImages.Length)
                        _currentSpriteIndex = 0;

                    _leftSprite = _leftSideImages[_currentSpriteIndex];
                    _rightSprite = _rightSideImages[_currentSpriteIndex];
                    _sprites[0].Image = _leftSprite;
                    _sprites[_sprites.Count - 1].Image = _rightSprite;
                    this.DrawImage();
                });
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string key = nameof(Properties.Resources.keen6_water_hazard);
            var area = _area;
            return $"{key}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_addedWidth}{separator}{_addedDepth}"; ;
        }
    }
}
