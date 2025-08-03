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
    public abstract class ResizableLiquidHazard : Hazard, IUpdatable
    {
        protected List<LocatedImage> _sprites;
        protected List<LocatedImage> _depthSprites;
        protected int _lengths;
        protected int _depths;

        protected readonly int HITBOX_VERTICAL_OFFSET = 32;
        protected const int SPRITE_CHANGE_DELAY = 1;
        protected int _currentSpriteChangeDelayTick;
        protected Dictionary<int, int> _currentSpriteIndices;
        protected Rectangle _area;
        protected readonly Image[] _spriteSheet;
        protected readonly Image _depthSprite;

        public ResizableLiquidHazard(Rectangle area, SpaceHashGrid grid, int zIndex, HazardType hazardType, Image[] spriteSheet, Image depthSprite, int lengths = 1, int depths = 0)
            : base(grid, area, hazardType, zIndex)
        {
            if (lengths < 1)
                lengths = 1;

            if (depths < 0)
                depths = 0;

            _lengths = lengths;
            _depths = depths;
            _area = area;
            _spriteSheet = spriteSheet;
            _depthSprite = depthSprite;
            if (_depthSprite == null)
                throw new ArgumentNullException("Need a depth sprite for a hazard");


            HITBOX_VERTICAL_OFFSET = depthSprite.Height / 2;
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

        protected virtual Image GetRandomImageFromSpriteSheet(out int randVal)
        {
            randVal = _random.Next(0, _spriteSheet.Length);
            return _spriteSheet[randVal];
        }

        protected void Initialize()
        {
            _sprites = new List<LocatedImage>();
            _depthSprites = new List<LocatedImage>();
            _currentSpriteIndices = new Dictionary<int, int>();
            int currentX = this.HitBox.X;
            var images = _spriteSheet;
            for (int i = 0; i < _lengths; i++)
            {
                LocatedImage p = new LocatedImage();
                p.Location = new Point(currentX, this.HitBox.Y);

                p.Image = GetRandomImageFromSpriteSheet(out int randVal);

                _sprites.Add(p);
                _currentSpriteIndices.Add(i, randVal);

                for (int j = 0; j < _depths; j++)
                {
                    LocatedImage pDepth = new LocatedImage();
                    pDepth.Location = new Point(currentX, this.HitBox.Y + (_depthSprite.Height * (j + 1)) - 2);
                    pDepth.Image = _depthSprite;
                    _depthSprites.Add(pDepth);
                }
                currentX += _depthSprite.Width;
            }

            this.HitBox = new Rectangle(
                  this.HitBox.X//x
                , this.HitBox.Y + HITBOX_VERTICAL_OFFSET//y
                , _depthSprite.Width * _lengths//width
                , this.HitBox.Height - HITBOX_VERTICAL_OFFSET + (_depthSprite.Height * _depths));//height

            this.DrawImage();
        }

        public void DrawImage()
        {
            var totalWidth = _depthSprite.Width * _lengths;
            var totalHeight = _depthSprite.Height * (_depths + 1);
            Size canvas = new Size(totalWidth, totalHeight);
            Image[] images = this.Sprites.ToList().Union(this.DepthSprites)
                .Select(s => s.Image).ToArray();
            Point[] locations = this.Sprites.ToList().Union(this.DepthSprites)
                .Select(s => new Point(s.Location.X - _area.X, s.Location.Y - _area.Y)).ToArray();
            _sprite = BitMapTool.DrawImagesOnCanvas(canvas, null, images, locations);
        }

        public override Point Location => _area.Location;

        public virtual void Update()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                for (int i = 0; i < _currentSpriteIndices.Count; i++)
                {
                    if (_sprites[i].Image == _depthSprite)
                        continue;

                    int spriteIndex = _currentSpriteIndices[i];
                    if (++spriteIndex >= _spriteSheet.Length)
                    {
                        spriteIndex = 0;
                    }
                    _currentSpriteIndices[i] = spriteIndex;
                    _sprites[i].Image = _spriteSheet[spriteIndex];
                }
                this.DrawImage();
            }
        }
    }
}
