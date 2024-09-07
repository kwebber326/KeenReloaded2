using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen6LaserField : Hazard, IUpdatable
    {
        private LaserFieldState _state;
        private const int OFF_TIME = 20;
        private int _currentOffTimeTick;
        private const int STATE_CHANGE_DELAY = 6;
        private int _currentStateChangeDelayTick;

        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;
        private Image[] _laserSprites;
        private Rectangle _area;

        private const int WIDTH = 32, UP_HEIGHT = 24, DOWN_HEIGHT = 32;

        public Keen6LaserField(Rectangle area, SpaceHashGrid grid, int zIndex, LaserFieldState initialState)
            : base(grid, area, Enums.HazardType.KEEN5_LASER_FIELD, zIndex)
        {
            this.HitBox = new Rectangle(area.X, area.Y, area.Width, area.Height);
            _laserSprites = SpriteSheet.SpriteSheet.Keen6LaserFieldImages;

            UpTile = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X, this.HitBox.Bottom - UP_HEIGHT, WIDTH, UP_HEIGHT));
            DownTile = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X, this.HitBox.Top, WIDTH, DOWN_HEIGHT));
            _area = area;
            this.State = initialState;
        }

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public override bool IsDeadly
        {
            get
            {
                return _state == LaserFieldState.PHASE1;
            }
        }

        public InvisibleTile UpTile
        {
            get;
            private set;
        }

        public InvisibleTile DownTile
        {
            get;
            private set;
        }

        LaserFieldState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            Image middleImage = null;
            var imgTop = Properties.Resources.keen6_laser_field_top;
            var imgBottom = Properties.Resources.keen6_laser_field_bottom;
            List<LocatedImage> sprites = new List<LocatedImage>()
            {
                new LocatedImage()
                {
                    Image = imgTop,
                    Location = new Point(0, 0)
                },
                new LocatedImage()
                {
                    Image = imgBottom,
                    Location = new Point(0, _area.Height - imgBottom.Height)
                }
            };
            switch (_state)
            {
                case LaserFieldState.PHASE1:
                    if (_currentSprite >= _laserSprites.Length)
                    {
                        _currentSprite = 0;
                    }
                    middleImage = _laserSprites[_currentSprite];
                    break;
            }
            this.DrawLaserInLaserFieldArea(sprites, middleImage, 0);
        }

        private void DrawLaserInLaserFieldArea(List<LocatedImage> sprites, Image middleImage, int xOffset)
        {
            if (middleImage != null)
            {
                int currentY = sprites[0].Bottom;
                int maxY = sprites[1].Top - 1;
                int laserHeight = middleImage.Height;

                while (currentY < maxY)
                {
                    int bottom = currentY + laserHeight;
                    if (bottom <= maxY)
                    {
                        sprites.Add(new LocatedImage()
                        {
                            Image = middleImage,
                            Location = new Point(xOffset, currentY)
                        });
                    }
                    else
                    {
                        int cropHeight = laserHeight - (bottom - maxY);
                        Rectangle subSection = new Rectangle(0, 0, middleImage.Width, cropHeight);
                        Image img = BitMapTool.CropImage(middleImage, subSection);
                        LocatedImage lImg = new LocatedImage()
                        {
                            Image = img,
                            Location = new Point(xOffset, currentY)
                        };
                        sprites.Add(lImg);
                    }

                    currentY += laserHeight;
                }
            }

            Image[] images = sprites.Select(i => i.Image).ToArray();
            Point[] locations = sprites.Select(i => i.Location).ToArray();

            _sprite = BitMapTool.DrawImagesOnCanvas(_area.Size, null, images, locations);
        }

        public void Update()
        {
            switch (_state)
            {
                case LaserFieldState.OFF:
                    this.UpdateDoormantState();
                    break;
                default:
                    this.UpdateLaserPhase();
                    break;
            }
        }

        private void UpdateLaserPhase()
        {
            this.State = LaserFieldState.PHASE1;
            if (_currentStateChangeDelayTick++ == STATE_CHANGE_DELAY)
            {
                _currentStateChangeDelayTick = 0;
                this.UpdateDoormantState();
                return;
            }
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSprite, SPRITE_CHANGE_DELAY, UpdateSprite);
            TryKillKeen();
        }

        private void UpdateHitboxWidth(int width)
        {
            if (width > WIDTH)
                width = WIDTH;
            this.HitBox = new Rectangle(UpTile.HitBox.X + ((WIDTH - width) / 2), this.HitBox.Y, width, this.HitBox.Height);
        }

        private void TryKillKeen()
        {
            var collisions = this.CheckCollision(this.HitBox);
            var keens = collisions.OfType<CommanderKeen>();
            if (keens.Any())
            {
                foreach (var _keen in keens)
                {
                    if (_keen.HitBox.IntersectsWith(this.HitBox))
                        _keen.Die();
                }
            }
        }

        private void UpdateDoormantState()
        {
            if (this.State != LaserFieldState.OFF)
            {
                this.State = LaserFieldState.OFF;
                _currentOffTimeTick = 0;
            }
            if (_currentOffTimeTick++ == OFF_TIME)
            {
                this.UpdateLaserPhase();
            }
        }
        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

            return $"{nameof(Properties.Resources.keen6_laser_field_bottom)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_state}";
        }
    }
}
