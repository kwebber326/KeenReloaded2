using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Keen5GeneratorWall : CollisionObject, IUpdatable, ISprite
    {
        private readonly int _zIndex;
        private Image _sprite;
        private InvisibleTile _leftTile;
        private InvisibleTile _rightTile;
        private InvisibleTile _centerTile;
        private const int LEFT_TILE_HORIZONTAL_COLLISION_WIDTH = 36;
        private const int RIGHT_TILE_HORIZONTAL_COLLISION_WIDTH = 10;
        private const int GLASS_X_OFFSET = 64;
        private const int GLASS_Y_OFFSET = 26;

        private const string _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
        private Keen5GeneratorGlass _glass;
        private bool _stopUpdating = false;

        public Keen5GeneratorWall(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            _zIndex = zIndex;
            _sprite = Properties.Resources.keen5_red_generator_wall;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            if (_collidingNodes != null && _collisionGrid != null)
            {
                Rectangle leftTileHitbox = new Rectangle(this.HitBox.X, this.HitBox.Y,
                    LEFT_TILE_HORIZONTAL_COLLISION_WIDTH, this.HitBox.Height);
                _leftTile = new InvisibleTile(_collisionGrid, leftTileHitbox);

                Rectangle rightTileHitbox = new Rectangle(this.HitBox.Right - RIGHT_TILE_HORIZONTAL_COLLISION_WIDTH,
                    this.HitBox.Y, RIGHT_TILE_HORIZONTAL_COLLISION_WIDTH, this.HitBox.Height);
                _rightTile = new InvisibleTile(_collisionGrid, rightTileHitbox);


                Rectangle glassHitbox = new Rectangle(this.HitBox.X + GLASS_X_OFFSET, this.HitBox.Y + GLASS_Y_OFFSET,
                    Keen5GeneratorGlass.IMAGE_WIDTH, Keen5GeneratorGlass.IMAGE_HEIGHT);
                _glass = new Keen5GeneratorGlass(_collisionGrid, glassHitbox, _zIndex);

                Rectangle centerTileHitbox = new Rectangle(leftTileHitbox.Right,
                    this.HitBox.Y + GLASS_Y_OFFSET, glassHitbox.X - leftTileHitbox.Right, Keen5GeneratorGlass.IMAGE_HEIGHT);
                _centerTile = new InvisibleTile(_collisionGrid, centerTileHitbox);
            }
            DrawCombinedImage(true);
        }

        private void DrawCombinedImage(bool initialDrawing = false)
        {
            Image newImg = initialDrawing
                ? SpriteSheet.SpriteSheet.Keen5GlassGeneratorSprites.FirstOrDefault()
                : _glass?.Image;

            _sprite = BitMapTool.DrawImagesOnCanvas(this.HitBox.Size,
                null, new Image[] { Properties.Resources.keen5_red_generator_wall, newImg },
                new Point[] { new Point(0, 0), new Point(GLASS_X_OFFSET, GLASS_Y_OFFSET) });
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public void Update()
        {
            if (!_stopUpdating)
            {
                _glass.Update();
                DrawCombinedImage();
                if (_glass.IsDead())
                {
                    _centerTile.RemoveTileFromGrid();
                    _stopUpdating = true;
                }
            }
        }

        public override string ToString()
        {
            var initialImageName = nameof(Properties.Resources.keen5_red_generator_wall);
            var area = this.HitBox;
            return $"{initialImageName}{_separator}{area.X}{_separator}{area.Y}{_separator}{area.Width}{_separator}{area.Height}{_separator}{_zIndex}";
        }
    }
}
