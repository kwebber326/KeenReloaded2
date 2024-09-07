using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
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
    public class Keen6ElectricRodHazard : Hazard, IUpdatable
    {
        private const int LEFT_TILE_WIDTH = 22, LEFT_TILE_HEIGHT = 82;
        private const int RIGHT_TILE_WIDTH = 8, RIGHT_TILE_HEIGHT = 82;
        private const int DEATH_ZONE_WIDTH = 98, DEATH_ZONE_HEIGHT = 82;
        private const int BOTTOM_TILE_WIDTH = 128, BOTTOM_TILE_HEIGHT = 14;

        private const int SPRITE_CHANGE_DELAY = 2;
        private int _currentSpriteChangeDelayTick;
        private int _currentSpriteIndex;

        private InvisibleTile _leftTile;
        private InvisibleTile _rightTile;
        private InvisibleTile _bottomTile;

        private Rectangle _area;

        private readonly Image[] _sprites = SpriteSheet.SpriteSheet.Keen6ElectricRodSprites;
        public Keen6ElectricRodHazard(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area, HazardType.KEEN6_ELECTRIC_RODS, zIndex)
        {
            _area = new Rectangle(area.X, area.Y, area.Width, area.Height);
            if (grid != null && _collidingNodes != null)
            {
                InitializeCollisionTiles();
            }
        }

        private void InitializeCollisionTiles()
        {
            _leftTile = new InvisibleTile(_collisionGrid, new Rectangle(_area.X, _area.Y, LEFT_TILE_WIDTH, LEFT_TILE_HEIGHT));
            _rightTile = new InvisibleTile(_collisionGrid, new Rectangle(_area.Right - RIGHT_TILE_WIDTH, _area.Y, RIGHT_TILE_WIDTH, RIGHT_TILE_HEIGHT));
            _bottomTile = new InvisibleTile(_collisionGrid, new Rectangle(_area.X, _area.Bottom - BOTTOM_TILE_HEIGHT, BOTTOM_TILE_WIDTH, BOTTOM_TILE_HEIGHT));
            //death zone
            this.HitBox = new Rectangle(_area.X + LEFT_TILE_WIDTH + 1, _area.Y, DEATH_ZONE_WIDTH, DEATH_ZONE_HEIGHT);
            this.UpdateCollisionNodes(Direction.DOWN_LEFT);
            this.UpdateCollisionNodes(Direction.UP_RIGHT);
        }

        public override Point Location => _area.Location;    

        public void Update()
        {
            this.UpdateSpriteByDelayBase(
                ref _currentSpriteChangeDelayTick
              , ref _currentSpriteIndex
              , SPRITE_CHANGE_DELAY
              , UpdateSprite);
        }

        private void UpdateSprite()
        {
            if (_currentSpriteIndex >= _sprites.Length)
                _currentSpriteIndex = 0;

            _sprite = _sprites[_currentSpriteIndex];
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

            return $"{nameof(Properties.Resources.keen6_electric_rods1)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
