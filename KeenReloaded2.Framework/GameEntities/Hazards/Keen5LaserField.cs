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
    public class Keen5LaserField : Hazard, IUpdatable
    {
        private LaserFieldState _state;
        private const int OFF_TIME = 20;
        private int _currentOffTimeTick;
        private const int STATE_CHANGE_DELAY = 2;
        private int _currentStateChangeDelayTick;

        private const int SKINNY_WIDTH = 6, MEDIUM_WIDTH = 14, FAT_WIDTH = 32;

        private const int WIDTH = 32, UP_HEIGHT = 44, Y_OFFSET = 4, DOWN_HEIGHT = 32;
        private Rectangle _area;

        public Keen5LaserField(Rectangle area, SpaceHashGrid grid, int zIndex, LaserFieldState initialState)
            : base(grid, area, Enums.HazardType.KEEN5_LASER_FIELD, zIndex)
        {
            this.HitBox = new Rectangle(area.X, area.Y, area.Width, area.Height);
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
                if (this.HitBox != null && _collisionGrid != null && _collidingNodes != null)
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
                return _state == LaserFieldState.PHASE2 || _state == LaserFieldState.PHASE3 || _state == LaserFieldState.PHASE4;
            }
        }

        protected InvisibleTile UpTile
        {
            get;
            private set;
        }

        protected InvisibleTile DownTile
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
            var upImg = Properties.Resources.keen5_laser_field_up;
            var downImg = Properties.Resources.keen5_laser_field_down;
            int xOffset = 0;
            List<LocatedImage> sprites = new List<LocatedImage>()
            {
                new LocatedImage()
                {
                    Image = downImg,
                    Location = new Point(0, 0)
                },
                new LocatedImage()
                {
                    Image = upImg,
                    Location = new Point(0, _area.Height - upImg.Height)
                }
            };
            Image middleImage = null;
            switch (_state)
            {
                case LaserFieldState.PHASE1:
                case LaserFieldState.PHASE5:
                    middleImage = Properties.Resources.keen5_laser_field_laser1;
                    xOffset = _area.Width / 2 - middleImage.Width / 2;
                    this.UpdateHitboxWidth(SKINNY_WIDTH);
                    break;
                case LaserFieldState.PHASE2:
                case LaserFieldState.PHASE4:
                    middleImage = Properties.Resources.keen5_laser_field_laser2;
                    xOffset = _area.Width / 2 - middleImage.Width / 2;
                    this.UpdateHitboxWidth(MEDIUM_WIDTH);
                    break;
                case LaserFieldState.PHASE3:
                    middleImage = Properties.Resources.keen5_laser_field_laser3;
                    xOffset = _area.Width / 2 - middleImage.Width / 2;
                    this.UpdateHitboxWidth(FAT_WIDTH);
                    break;
            }
            DrawLaserInLaserFieldArea(sprites, middleImage, xOffset);
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

        public override Point Location => _area.Location;

        public virtual void Update()
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
            switch (_state)
            {
                case LaserFieldState.OFF:
                    this.State = LaserFieldState.PHASE1;
                    break;
                case LaserFieldState.PHASE1:
                    if (_currentStateChangeDelayTick++ == STATE_CHANGE_DELAY)
                    {
                        _currentStateChangeDelayTick = 0;
                        this.State = LaserFieldState.PHASE2;
                    }
                    break;
                case LaserFieldState.PHASE2:
                    if (_currentStateChangeDelayTick++ == STATE_CHANGE_DELAY)
                    {
                        _currentStateChangeDelayTick = 0;
                        this.State = LaserFieldState.PHASE3;
                    }
                    TryKillKeen();
                    break;
                case LaserFieldState.PHASE3:
                    if (_currentStateChangeDelayTick++ == STATE_CHANGE_DELAY)
                    {
                        _currentStateChangeDelayTick = 0;
                        this.State = LaserFieldState.PHASE4;
                    }
                    TryKillKeen();
                    break;
                case LaserFieldState.PHASE4:
                    if (_currentStateChangeDelayTick++ == STATE_CHANGE_DELAY)
                    {
                        _currentStateChangeDelayTick = 0;
                        this.State = LaserFieldState.PHASE5;
                    }
                    TryKillKeen();
                    break;
                case LaserFieldState.PHASE5:
                    if (_currentStateChangeDelayTick++ == STATE_CHANGE_DELAY)
                    {
                        _currentStateChangeDelayTick = 0;
                        this.UpdateDoormantState();
                    }
                    break;
            }
        }

        private void UpdateHitboxWidth(int width)
        {
            if (width > WIDTH)
                width = WIDTH;
            this.HitBox = new Rectangle(UpTile.HitBox.X + ((WIDTH - width) / 2), this.HitBox.Y, width, this.HitBox.Height);
        }

        private void TryKillKeen()
        {
            if (this.IsDeadly)
            {
                var collisions = this.CheckCollision(this.HitBox);
                collisions = collisions.Where(c => c.CollisionType == Enums.CollisionType.PLAYER).ToList();
                if (collisions.Any())
                {
                    foreach (var keen in collisions)
                    {
                        var player = keen as CommanderKeen;
                        if (player != null)
                        {
                            player.Die();
                        }
                    }
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

            return $"{nameof(Properties.Resources.keen5_laser_field_up)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_state}";
        }
    }

    public enum LaserFieldState
    {
        OFF,
        PHASE1,
        PHASE2,
        PHASE3,
        PHASE4,
        PHASE5
    }
}
