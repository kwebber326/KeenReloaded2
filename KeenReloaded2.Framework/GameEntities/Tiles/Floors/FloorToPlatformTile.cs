using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Floors
{
    public class FloorToPlatformTile : CollisionObject, ISprite, IBiomeTile
    {
        private readonly int _zIndex;
        private string _biome;
        private Image _sprite;
        private InvisibleTile _floorTile;
        private InvisiblePlatformTile _platformTile;
        private string _initialImageName;

        private const int PLATFORM_VERTICAL_OFFSET = 32, FLOOR_VERTICAL_OFFSET = 32, EDGE_HORIZONTAL_OFFSET_LEFT = 0, EDGE_HORIZONTAL_OFFSET_RIGHT = 0, FLOOR_WIDTH = 12;

        public event EventHandler<ObjectEventArgs> BiomeChanged;

        public FloorToPlatformTile(Rectangle area, SpaceHashGrid grid, int zIndex, string biome)
            : base(grid, area)
        {
            _zIndex = zIndex;
            _biome = biome;
            this.HitBox = area;
            Initialize();
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
                if (_collidingNodes != null && _collisionGrid != null)
                {
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        private void Initialize()
        {
            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_MIRAGE:
                default:
                    _sprite = Properties.Resources.keen4_mirage_floor_to_platform_left;
                    _initialImageName = nameof(Properties.Resources.keen4_mirage_floor_to_platform_left);
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _sprite = Properties.Resources.keen5_black_floor_to_platform_left;
                    _initialImageName = nameof(Properties.Resources.keen5_black_floor_to_platform_left);
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _sprite = Properties.Resources.keen5_red_floor_to_platform_left;
                    _initialImageName = nameof(Properties.Resources.keen5_red_floor_to_platform_left);
                    break;
                    
            }

            _floorTile = new InvisibleTile(_collisionGrid, new Rectangle(this.HitBox.X + EDGE_HORIZONTAL_OFFSET_LEFT, this.HitBox.Y + FLOOR_VERTICAL_OFFSET, FLOOR_WIDTH, this.HitBox.Height - FLOOR_VERTICAL_OFFSET));
            _platformTile = new InvisiblePlatformTile(_collisionGrid, new Rectangle(_floorTile.HitBox.Right + 1, this.HitBox.Y + PLATFORM_VERTICAL_OFFSET, this.HitBox.Width - FLOOR_WIDTH - EDGE_HORIZONTAL_OFFSET_LEFT - EDGE_HORIZONTAL_OFFSET_RIGHT, this.HitBox.Height - PLATFORM_VERTICAL_OFFSET));
        }

        public void ChangeBiome(string newBiome)
        {
            _biome = newBiome;
            Initialize();
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = this
            };
            BiomeChanged?.Invoke(this, e);
        }

        public string Biome => _biome;

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => false;

        public override string ToString()
        {
            var _area = this.HitBox;
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{_initialImageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_biome}";
        }
    }
}
