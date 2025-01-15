using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Walls
{
    public class WallToPlatformTile : CollisionObject, ISprite, IBiomeTile
    {
        private Image _sprite;
        private string _biome;
        private Direction _direction;
        private InvisibleTile _floorTile;
        private InvisiblePlatformTile _platformTile;
        private string _initialImageName;
        private readonly string _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

        private int VERTICAL_OFFSET = 32, EDGE_HORIZONTAL_OFFSET_LEFT = 0, EDGE_HORIZONTAL_OFFSET_RIGHT = 0, WALL_WIDTH_RIGHT = 6, WALL_WIDTH_LEFT = 4, WALL_HEIGHT = 64;
        private readonly int _zIndex;

        public WallToPlatformTile(Rectangle area, SpaceHashGrid grid, int zIndex, string biome, Direction direction)
            : base(grid, area)
        {
            if (direction != Direction.LEFT && direction != Direction.RIGHT)
                throw new ArgumentException("Direction must be left or right for wall-to-platform tile type");

            _direction = direction;
            _biome = biome;
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {

            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_MIRAGE:
                default:
                    _sprite = _direction == Direction.LEFT
                        ? Properties.Resources.keen4_mirage_wall_to_platform_left
                        : Properties.Resources.keen4_mirage_wall_to_platform_right;
                    _initialImageName = _direction == Direction.LEFT
                        ? nameof(Properties.Resources.keen4_mirage_wall_to_platform_left)
                        : nameof(Properties.Resources.keen4_mirage_wall_to_platform_right);
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _direction = Direction.LEFT;
                    _sprite = Properties.Resources.keen5_red_wall_to_platform_left;
                    _initialImageName = nameof(Properties.Resources.keen5_red_wall_to_platform_left);
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _direction = Direction.LEFT;
                    _sprite = Properties.Resources.keen5_black_wall_to_platform_left;
                    _initialImageName = nameof(Properties.Resources.keen5_black_wall_to_platform_left);
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _direction = Direction.LEFT;
                    _sprite = Properties.Resources.keen5_green_wall_to_platform_left;
                    _initialImageName = nameof(Properties.Resources.keen5_green_wall_to_platform_left);
                    VERTICAL_OFFSET = 36;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _direction = Direction.LEFT;
                    _sprite = Properties.Resources.keen6_dome_wall_to_platform;
                    _initialImageName = nameof(Properties.Resources.keen6_dome_wall_to_platform);
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _direction = Direction.LEFT;
                    _sprite = Properties.Resources.keen6_industrial_wall_to_platform_tile;
                    _initialImageName = nameof(Properties.Resources.keen6_industrial_wall_to_platform_tile);
                    break;
            }

            if (_direction == Direction.LEFT)
            {
                _platformTile = new InvisiblePlatformTile(_collisionGrid, new Rectangle(this.HitBox.X + WALL_WIDTH_LEFT + EDGE_HORIZONTAL_OFFSET_LEFT, this.HitBox.Y + VERTICAL_OFFSET, this.HitBox.Width - WALL_WIDTH_LEFT - EDGE_HORIZONTAL_OFFSET_LEFT, this.HitBox.Height - VERTICAL_OFFSET));
                _floorTile = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X, this.HitBox.Y, WALL_WIDTH_LEFT, this.HitBox.Height));
            }
            else
            {
                _floorTile = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.Right - WALL_WIDTH_RIGHT + EDGE_HORIZONTAL_OFFSET_RIGHT, this.HitBox.Y, WALL_WIDTH_RIGHT - EDGE_HORIZONTAL_OFFSET_RIGHT, this.HitBox.Height));
                _platformTile = new InvisiblePlatformTile(_collisionGrid, new Rectangle(this.HitBox.X + EDGE_HORIZONTAL_OFFSET_RIGHT, this.HitBox.Y + VERTICAL_OFFSET, this.HitBox.Width - WALL_WIDTH_RIGHT - EDGE_HORIZONTAL_OFFSET_RIGHT, this.HitBox.Height - VERTICAL_OFFSET));
            }
        }

        public string Biome
        {
            get { return _biome; }
        }

        public Direction Direction
        {
            get
            {
                return _direction;
            }
        }

        public void ChangeBiome(string newBiome)
        {
            _biome = newBiome;
            Initialize();
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => false;

        public override string ToString()
        {
            var _area = this.HitBox;
            return $"{_initialImageName}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}{_separator}{_biome}{_separator}{_direction}";
        }
    }
}
