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
    public class TarPit : Hazard, IUpdatable
    {
        private List<LocatedImage> _sprites;
        private List<LocatedImage> _depthSprites;
        private int _lengths;
        private int _depths;
        private const int SPRITE_WIDTH = 64;
        private const int SPRITE_HEIGHT = 64;
        private const int HITBOX_VERTICAL_OFFSET = 32;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick;
        private Dictionary<int, int> _currentSpriteIndices;
        private Rectangle _area;

        public TarPit(Rectangle area, SpaceHashGrid grid, int zIndex, int lengths = 1, int depths = 0) 
            : base(grid, area, HazardType.KEEN4_TAR, zIndex)
        {
            if (lengths < 1)
                lengths = 1;

            if (depths < 0)
                depths = 0;

            _lengths = lengths;
            _depths = depths;
            _area = area;
            Initialize();
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

        private void Initialize()
        {
            _sprites = new List<LocatedImage>();
            _depthSprites = new List<LocatedImage>();
            _currentSpriteIndices = new Dictionary<int, int>();
            int currentX = this.HitBox.X;
            var images = SpriteSheet.SpriteSheet.Keen4TarPitImages;
            for (int i = 0; i < _lengths; i++)
            {
                LocatedImage p = new LocatedImage();
                p.Location = new Point(currentX, this.HitBox.Y);
                int randVal = _random.Next(0, images.Length);
                p.Image = images[randVal];

                _sprites.Add(p);
                _currentSpriteIndices.Add(i, randVal);

                for (int j = 0; j < _depths; j++)
                {
                    LocatedImage pDepth = new LocatedImage();
                    pDepth.Location = new Point(currentX, this.HitBox.Y + (SPRITE_HEIGHT * (j + 1)) - 2);
                    pDepth.Image = Properties.Resources.keen4_tar_depth;
                    _depthSprites.Add(pDepth);
                }
                currentX += SPRITE_WIDTH;
            }

            this.HitBox = new Rectangle(
                  this.HitBox.X//x
                , this.HitBox.Y + HITBOX_VERTICAL_OFFSET//y
                , SPRITE_WIDTH * _lengths//width
                , this.HitBox.Height - HITBOX_VERTICAL_OFFSET + (SPRITE_HEIGHT * _depths));//height

            this.DrawImage();
        }

        public void DrawImage()
        {
            var totalWidth = SPRITE_WIDTH * _lengths;
            var totalHeight = SPRITE_HEIGHT * (_depths + 1);
            Size canvas = new Size(totalWidth, totalHeight);
            Image[] images = this.Sprites.ToList().Union(this.DepthSprites)
                .Select(s => s.Image).ToArray();
            Point[] locations = this.Sprites.ToList().Union(this.DepthSprites)
                .Select(s => new Point(s.Location.X - _area.X, s.Location.Y - _area.Y)).ToArray();
            _sprite = BitMapTool.DrawImagesOnCanvas(canvas, null, images, locations);
        }

        public override Point Location => _area.Location;

        public void Update()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                for (int i = 0; i < _currentSpriteIndices.Count; i++)
                {
                    int spriteIndex = _currentSpriteIndices[i];
                    if (++spriteIndex >= SpriteSheet.SpriteSheet.Keen4TarPitImages.Length)
                    {
                        spriteIndex = 0;
                    }
                    _currentSpriteIndices[i] = spriteIndex;
                    _sprites[i].Image = SpriteSheet.SpriteSheet.Keen4TarPitImages[spriteIndex];
                }
                this.DrawImage();
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string key = nameof(Properties.Resources.keen4_tar1);
            var area = _area;
            return $"{key}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_lengths}{separator}{_depths}"; ;
        }
    }
}
