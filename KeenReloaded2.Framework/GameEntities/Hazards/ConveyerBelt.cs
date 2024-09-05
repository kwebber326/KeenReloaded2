using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class ConveyerBelt : Hazard, IUpdatable
    {

        private readonly Direction _direction;
        private readonly int _addedLengths;
        private List<ConveyerBeltPart> _parts = new List<ConveyerBeltPart>();
        private Rectangle _area;


        public ConveyerBelt(Rectangle area, SpaceHashGrid grid, int zIndex, Direction conveyerBeltDirection, int addedLengths = 0)
            : base(grid, area, HazardType.KEEN6_CONVEYER_BELT, zIndex)
        {
            _area = area;
            _direction = conveyerBeltDirection == Direction.LEFT ? Direction.LEFT : Direction.RIGHT;
            _addedLengths = addedLengths < 0 ? 0 : addedLengths;
            Initialize();
        }

        public List<ISprite> Sprites
        {
            get
            {
                return _parts.OfType<ISprite>().ToList();
            }
        }

        private void Initialize()
        {
            int currentX = this.HitBox.X;
            int currentY = this.HitBox.Y;

            Rectangle leftPartRect = new Rectangle(currentX, currentY, ConveyerBeltPart.LEFT_EDGE_WIDTH, ConveyerBeltPart.PART_HEIGHT);
            var leftPart = new ConveyerBeltPart(_collisionGrid, leftPartRect, _zIndex, ConveyerBeltPartType.LEFT, _direction);
            _parts.Add(leftPart);

            for (int i = 0; i < _addedLengths; i++)
            {
                currentX += ConveyerBeltPart.MIDDLE_WIDTH;
                Rectangle middlePartRect = new Rectangle(currentX, currentY, ConveyerBeltPart.MIDDLE_WIDTH, ConveyerBeltPart.PART_HEIGHT);
                ConveyerBeltPart part = new ConveyerBeltPart(_collisionGrid, middlePartRect, _zIndex, ConveyerBeltPartType.MIDDLE, _direction);
                _parts.Add(part);
            }
            currentX += ConveyerBeltPart.MIDDLE_WIDTH;
            Rectangle rightPartRect = new Rectangle(currentX, currentY, ConveyerBeltPart.RIGHT_EDGE_WIDTH, ConveyerBeltPart.PART_HEIGHT);
            var rightPart = new ConveyerBeltPart(_collisionGrid, rightPartRect, _zIndex, ConveyerBeltPartType.RIGHT, _direction);
            _parts.Add(rightPart);

            this.DrawSprite();
        }

        public override bool IsDeadly => false;

        private void DrawSprite()
        {
            int width = _parts.Select(p => p.Image?.Width ?? 0).Sum();
            int height = _parts.FirstOrDefault()?.Image?.Height ?? 0;
            Size canvas = new Size(width, height);
            Image[] images = _parts.Select(p => p.Image).ToArray();
            List<Point> points = new List<Point>();
            int currentX = 0;
            foreach (var part in _parts)
            {
                Point p = new Point(currentX, 0);
                points.Add(p);
                currentX += part.Image?.Width ?? 0;
            }
            _sprite = BitMapTool.DrawImagesOnCanvas(canvas, null, images, points.ToArray());
        }

        protected override void SetSpriteFromType(HazardType type)
        {
           
        }

        public void Update()
        {
            foreach (var part in _parts)
            {
                part.Update();
            }
            this.DrawSprite();
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{nameof(Properties.Resources.keen6_conveyer_belt_whole)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_direction}{separator}{_addedLengths}";
        }
    }

    class ConveyerBeltPart : MaskedTile, IUpdatable
    {
        private const int VERTICAL_LANDING_OFFSET = 32;
        private const int VERTICAL_COLLISION_LENGTH = 64;
        private const int SPRITE_CHANGE_DELAY = 1;
        private const int PUSH_STRENGTH = 10;
        private int _currentSpriteChangeDelayTick;
        private int _currentSpriteIndex;

        private readonly ConveyerBeltPartType _partType;
        private readonly Direction _direction;
        private Image[] _sprites;
        List<CommanderKeen> _collidingKeens = new List<CommanderKeen>();

        public const int LEFT_EDGE_WIDTH = 128;
        public const int RIGHT_EDGE_WIDTH = 126;
        public const int MIDDLE_WIDTH = 64;
        public const int PART_HEIGHT = 96;

        public ConveyerBeltPart(SpaceHashGrid grid, Rectangle hitbox, int zIndex, ConveyerBeltPartType partType, Direction direction) 
            : base(hitbox, grid, hitbox, null, zIndex)
        {
            _area = hitbox;
            _partType = partType;
            _direction = direction;
            Initialize();
        }

        private void Initialize()
        {
            switch (_partType)
            {
                case ConveyerBeltPartType.LEFT:
                    _sprites = SpriteSheet.SpriteSheet.Keen6ConveyerBeltLeftSprites;
                    break;
                case ConveyerBeltPartType.RIGHT:
                    _sprites = SpriteSheet.SpriteSheet.Keen6ConveyerBeltRightSprites;
                    break;
                case ConveyerBeltPartType.MIDDLE:
                    _sprites = SpriteSheet.SpriteSheet.Keen6ConveyerBeltMiddleSprites;
                    break;
            }
            _currentSpriteIndex = _direction == Direction.RIGHT ? 0 : _sprites.Length - 1;
            _image = _sprites[_currentSpriteIndex];

            this.HitBox = new Rectangle(_area.Location.X, _area.Location.Y + VERTICAL_LANDING_OFFSET, _image.Width, VERTICAL_COLLISION_LENGTH);
            if (_collisionGrid != null && _collidingNodes != null)
            {
                this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                this.UpdateCollisionNodes(Direction.UP_RIGHT);
            }
        }

        public ConveyerBeltPartType PartType
        {
            get => _partType;
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public override bool Hangable => false;

        public void Update()
        {
            if (_currentSpriteChangeDelayTick++ >= SPRITE_CHANGE_DELAY)
            {
                var collisions = this.CheckCollision(_area);
                _collidingKeens = collisions.OfType<CommanderKeen>()?.ToList() ?? new List<CommanderKeen>();
                _currentSpriteChangeDelayTick = 0;
                if (_direction == Direction.RIGHT)
                    RotateClockwise();
                else
                    RotateCounterClockwise();

                if (_collidingKeens.Any())
                {
                    foreach (var keen in _collidingKeens)
                    {
                        if (this.IsKeenStandingOnThis(keen))
                        {
                            this.UpdateKeenHorizontalPosition(keen);
                        }
                    }
                }
            }
        }

        private void RotateClockwise()
        {
            if (++_currentSpriteIndex >= _sprites.Length)
            {
                _currentSpriteIndex = 0;
            }
            _image = _sprites[_currentSpriteIndex];
        }

        private void RotateCounterClockwise()
        {
            if (--_currentSpriteIndex < 0)
            {
                _currentSpriteIndex = _sprites.Length - 1;
            }
            _image = _sprites[_currentSpriteIndex];
        }

        private void UpdateKeenHorizontalPosition(CommanderKeen _keen)
        {
            if (!_keen.IsDead())
                _keen.GetMovedHorizontally(this, _direction, PUSH_STRENGTH);
        }

        private bool IsKeenStandingOnThis(CommanderKeen _keen)
        {
            bool isStanding = _keen.HitBox.Bottom == this.HitBox.Top - 1 || _keen.HitBox.Bottom == this.HitBox.Top;
            return isStanding;
        }
    }

    enum ConveyerBeltPartType
    {
        LEFT,
        RIGHT,
        MIDDLE
    }
}
