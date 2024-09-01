using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
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
    public class AcidPool : Hazard, IUpdatable
    {
        private int _floorCollisionVerticalOffset;
        private int _poisonCollisionVerticalOffset;
        private const int FLOOR_LEFT_WIDTH = 32, FLOOR_RIGHT_WIDTH = 38;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;
        private int _lengths;
        private Rectangle _area;
        private Image _referenceSprite = Properties.Resources.keen4_poison_pool1;

        private CollisionObject _rightFloor, _leftFloor, _middleFloor;

        private Image[] _leftSprites = SpriteSheet.SpriteSheet.Keen4PoisonPoolLeftImages;
        private Image[] _rightSprites = SpriteSheet.SpriteSheet.Keen4PoisonPoolRightImages;
        private Image[] _middleSprites = SpriteSheet.SpriteSheet.Keen4PoisonPoolMiddleImages;

        private List<LocatedImage> _sprites;
        public AcidPool(Rectangle area, SpaceHashGrid grid, int zIndex, int lengths = 1) 
            : base(grid, area, HazardType.KEEN4_POISON_POOL, zIndex)
        {
            if (lengths < 1)
                lengths = 1;

            _lengths = lengths;
            this.HitBox = area;
            _area = area;
            Initialize();
        }

        public IEnumerable<LocatedImage> Sprites
        {
            get
            {
                return _sprites;
            }
            private set
            {
                _sprites = value as List<LocatedImage>;
            }
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
                if (value != null && _collisionGrid != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        public override Point Location
        {
            get
            {
                return _area.Location;
            }
        }

        private void Initialize()
        {
            _floorCollisionVerticalOffset = this.HitBox.Height / 2;
            _poisonCollisionVerticalOffset = _floorCollisionVerticalOffset + 2;

            List<LocatedImage> sprites = new List<LocatedImage>();

            LocatedImage pLeft = new LocatedImage();
            pLeft.Location = new Point(this.HitBox.X, this.HitBox.Y);
            pLeft.Image = Properties.Resources.keen4_poison_pool1_edge_left;

            sprites.Add(pLeft);

            _leftFloor = new InvisibleTile(_collisionGrid, new Rectangle(
              this.HitBox.Left,
              this.HitBox.Y + _floorCollisionVerticalOffset,
              FLOOR_LEFT_WIDTH,
              this.HitBox.Height - _floorCollisionVerticalOffset));

            int newRight = pLeft.Right;
            int middleWidth = 0;
            for (int i = 0; i < _lengths; i++)
            {
                LocatedImage pMiddle = new LocatedImage();
                pMiddle = new LocatedImage();
                pMiddle.Location = new Point(newRight, pLeft.Top);
                pMiddle.Image = Properties.Resources.keen4_poison_pool1_middle;
                middleWidth = pMiddle.Width;
                sprites.Add(pMiddle);
                newRight += pMiddle.Width;
            }

            LocatedImage pRight = new LocatedImage();
            pRight.Location = new Point(newRight, this.HitBox.Y);
            pRight.Image = Properties.Resources.keen4_poison_pool1_edge_right;

            sprites.Add(pRight);

            _middleFloor = new InvisibleTile(_collisionGrid, new Rectangle(
                 _leftFloor.HitBox.Right
               , this.HitBox.Y + _poisonCollisionVerticalOffset + 1
               , middleWidth * _lengths
               , this.HitBox.Height - _floorCollisionVerticalOffset - 1));

            _rightFloor = new InvisibleTile(_collisionGrid, new Rectangle(
                 newRight
               , this.HitBox.Y + _floorCollisionVerticalOffset
               , FLOOR_RIGHT_WIDTH
               , this.HitBox.Height - _floorCollisionVerticalOffset));
            this.HitBox = new Rectangle(
               _leftFloor.HitBox.Right
               , this.HitBox.Y + _poisonCollisionVerticalOffset
               , middleWidth * _lengths
               , this.HitBox.Height - _floorCollisionVerticalOffset);

            this.Sprites = sprites;
            this.DrawImage();
        }

        public void DrawImage()
        {
            var totalWidth = this.Sprites.Select(s => s.Width).Sum();
            var totalHeight = _referenceSprite.Height;
            Size canvas = new Size(totalWidth, totalHeight);
            Image[] images = this.Sprites.Select(s => s.Image).ToArray();
            Point[] locations = this.Sprites.Select(s => new Point(s.Location.X - _area.X, 0)).ToArray();
            _sprite = BitMapTool.DrawImagesOnCanvas(canvas, null, images, locations);
        }

        public void Update()
        {
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSprite, SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        private void UpdateSprite()
        {
            if (_currentSprite >= _middleSprites.Length)
            {
                _currentSprite = 0;
            }
            _sprites[0].Image = _leftSprites[_currentSprite];
            for (int i = 1; i < _sprites.Count - 1; i++)
            {
                _sprites[i].Image = _middleSprites[_currentSprite];
            }
            _sprites[_sprites.Count - 1].Image = _rightSprites[_currentSprite];
            this.DrawImage();
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string key = nameof(Properties.Resources.keen4_poison_pool1);
            var area = _area;
            return $"{key}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_lengths}"; ;
        }
    }
}
