using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Pole : CollisionObject, ICreateRemove, IBiomeTile, ISprite, IUpdatable
    {
        private const int POLE_WIDTH = 12;
        private readonly string _objectKey;
        private int _zIndex;
        private PoleType _poleType;
        private CollisionType _collisionType = CollisionType.POLE;
        private string _biomeType;
        private Image _image;
        private Rectangle _area;
        private bool _updated;
        private const int MANHOLE_POLE_X_OFFSET = 40;
        private const int INVISIBLE_TILE_LEFT_WIDTH = 10;
        private const int INVISIBLE_TILE_RIGHT_WIDTH = 4;

        public Pole(Rectangle area, SpaceHashGrid grid, int zIndex, string objectKey, PoleType poleType, string biomeType)
            : base(grid, area)
        {
            _zIndex = zIndex;
            _biomeType = biomeType;
            _poleType = poleType;
            _area = area;
            _objectKey = objectKey;
            Initialize();
        }

        public PoleSprite Manhole { get; private set; }

        public PoleSprite ManholeFloor { get; private set; }

        private void Initialize()
        {
            this.Sprites = new List<ISprite>();
            int height = InferHeightFromPoleType(_poleType);
            this.HitBox = _area;
            if (_poleType == PoleType.MANHOLE || _poleType == PoleType.MANHOLE_FLOOR)
            {
                this.HitBox = new Rectangle(_area.X + MANHOLE_POLE_X_OFFSET, _area.Y, POLE_WIDTH, height);
                PoleSprite pManhole = new PoleSprite(PoleType.MANHOLE, _biomeType, _area.Location, _zIndex);
                this.Sprites.Add(pManhole);
                this.Manhole = pManhole;


                PoleSprite pManholeFloor = new PoleSprite(PoleType.MANHOLE_FLOOR, _biomeType, new Point(_area.X, pManhole.Location.Y + pManhole.Image.Height), _zIndex);
                this.Sprites.Add(pManholeFloor);
                this.ManholeFloor = pManholeFloor;

                if (_collisionGrid != null)
                {
                    pManhole.CollisionTile = new PoleTile(_collisionGrid, new Rectangle(pManhole.Location.X, pManhole.Location.Y + pManhole.Image.Height, pManhole.Image.Width, pManhole.Image.Height / 2), pManhole.Image, _zIndex);
                    ConstructCollisionTilesFromManholeCollisionTile(pManhole);
                }

                if (_biomeType.Contains("KEEN4"))
                {
                    Size canvas = new Size(pManhole.Image.Width, pManhole.Image.Height + pManholeFloor.Image.Height);
                    Point[] locations = new Point[] { new Point(0, pManhole.Image.Height) };
                    Image[] images = new Image[] { pManholeFloor.Image };

                    _image = BitMapTool.DrawImagesOnCanvas(canvas, pManhole.Image, images, locations);
                }
                else if (_biomeType == Biomes.BIOME_KEEN5_BLACK)
                {
                    _image = Properties.Resources.keen5_black_pole_manhole_full;
                }
                else if (_biomeType == Biomes.BIOME_KEEN5_RED)
                {
                    _image = Properties.Resources.keen5_red_pole_manhole_full;
                }
                else if (_biomeType == Biomes.BIOME_KEEN5_GREEN)
                {
                    _image = Properties.Resources.keen5_green_pole_manhole_full;
                }
                else if (_biomeType == Biomes.BIOME_KEEN6_DOME)
                {
                    _image = Properties.Resources.keen6_dome_pole_manhole_full;
                }
                else if (_biomeType == Biomes.BIOME_KEEN6_FOREST)
                {
                    _image = Properties.Resources.keen6_forest_pole_manhole_full;
                }
                else if (_biomeType == Biomes.BIOME_KEEN6_INDUSTRIAL)
                {
                    _image = Properties.Resources.keen6_industrial_pole_manhole_full;
                }
            }
            else if (_poleType == PoleType.TOP)
            {
                PoleSprite pTop = new PoleSprite(PoleType.TOP, _biomeType, new Point(this.HitBox.X, this.HitBox.Y), _zIndex);
                _image = pTop.Image;
            }
            else if (_poleType == PoleType.BOTTOM)
            {
                PoleSprite pBottom = new PoleSprite(PoleType.BOTTOM, _biomeType, new Point(this.HitBox.X, this.HitBox.Bottom - 28), _zIndex);
                this.Sprites.Add(pBottom);
                _image = pBottom.Image;
            }
            else
            {
                PoleSprite pMiddle = new PoleSprite(PoleType.MIDDLE, _biomeType, new Point(this.HitBox.X, this.HitBox.Bottom - 28), _zIndex);
                _image = pMiddle.Image;
            }

            if (_collisionGrid != null && _collidingNodes != null)
            {
                this.UpdateCollisionNodes(Direction.DOWN);
                ResetCollidingNodes();
            }
        }

        private void ConstructCollisionTilesFromManholeCollisionTile(PoleSprite pManhole)
        {
            int xLeft = pManhole.CollisionTile.HitBox.X;
            int y = pManhole.CollisionTile.HitBox.Y;
            int leftWidth = INVISIBLE_TILE_LEFT_WIDTH;
            int blockCollisionHeight = pManhole.CollisionTile.HitBox.Height;
            Rectangle leftArea = new Rectangle(xLeft, y, leftWidth, blockCollisionHeight);
            InvisibleTile leftTile = new InvisibleTile(_collisionGrid, leftArea);

            int rightWidth = INVISIBLE_TILE_RIGHT_WIDTH;
            int xRight = pManhole.CollisionTile.HitBox.Right - rightWidth;
            Rectangle rightArea = new Rectangle(xRight, y, rightWidth, blockCollisionHeight);
            InvisibleTile rightTile = new InvisibleTile(_collisionGrid, rightArea);
        }

        void pManholeFloor_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void pManholeFloor_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        private int InferHeightFromPoleType(PoleType type)
        {
            switch (type)
            {
                case PoleType.MANHOLE:
                case PoleType.MANHOLE_FLOOR:
                    return 59;
                case PoleType.MIDDLE:
                case PoleType.TOP:
                    return 32;
                case PoleType.BOTTOM:
                    return 28;
            }

            return 32;
        }

        private void ResetCollidingNodes()
        {
            _collidingNodes = _collisionGrid.GetCurrentHashes(this);
            foreach (SpaceHashGridNode node in _collidingNodes)
            {
                node.Objects.Remove(this);
                node.Objects.Add(this);
            }
        }


        public List<ISprite> Sprites
        {
            get;
            private set;
        }

        public PoleSprite ManHoleFloorSprite { get; private set; }

        public string Biome => _biomeType;

        public Point Location => _area.Location;

        public override CollisionType CollisionType => _collisionType;

        public int ZIndex => _zIndex;

        public Image Image => _image;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                Create(this, args);
            }
        }

        protected void OnRemove(ObjectEventArgs args)
        {
            if (this.Remove != null)
            {
                this.Remove(this, args);
            }
        }

        public virtual void ChangeBiome(string newBiome)
        {
            _biomeType = newBiome;
            var sprites = this.Sprites.OfType<IBiomeTile>();
            foreach (var sprite in sprites)
            {
                sprite.ChangeBiome(_biomeType);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var area = _area;
            return $"{_objectKey}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_objectKey}{separator}{_poleType}{separator}{_biomeType}";
        }

        public void Update()
        {
            if (_poleType != PoleType.MANHOLE && _poleType != PoleType.MANHOLE_FLOOR)
                return;

            if (!_updated)
            {
                try
                {
                    if (_biomeType.Contains("KEEN4"))
                    {
                        int x = this.ManholeFloor.Location.X;
                        int y = this.ManholeFloor.Location.Y;
                        int width = this.ManholeFloor.Image.Width;
                        int height = this.ManholeFloor.Image.Height - 12;
                        Rectangle areaRect = new Rectangle(x, y, width, height);
                        Image image = BitMapTool.CropImage(this.ManholeFloor.Image, new Rectangle(0, 0, areaRect.Width, areaRect.Height));
                        Background background = new Background(areaRect, image, true, _zIndex + 200);
                        ObjectEventArgs e = new ObjectEventArgs()
                        {
                            ObjectSprite = background
                        };
                        OnCreate(e);
                        _updated = true;
                    }
                    else 
                    {
                        int x = this.Location.X;
                        int y = this.Location.Y + 32;
                        int width = this.ManholeFloor.Image.Width;
                        int height = this.ManholeFloor.Image.Height;
                        Rectangle areaRect = new Rectangle(x, y, width, height);
                        Image image = this.ManholeFloor.Image;
                        Background background = new Background(areaRect, image, true, _zIndex + 200);
                        ObjectEventArgs e = new ObjectEventArgs()
                        {
                            ObjectSprite = background
                        };
                        OnCreate(e);
                        _updated = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }

    public class PoleSprite : ISprite, ICreateRemove, IBiomeTile
    {
        public const int MANHOLE_WIDTH = 96;
        private readonly int _zIndex;
        private readonly Point _originalLocation;
        private string _imageName;

        internal PoleSprite(PoleType type, string biomeType, Point p, int zIndex)
        {
            _zIndex = zIndex;
            _biome = biomeType;
            _poleType = type;
            _originalLocation = p;
            Initialize(type, biomeType);
        }

        public PoleTile CollisionTile { get; set; }

        public string ImageName
        {
            get
            {
                return _imageName;
            }
        }

        private void Initialize(PoleType poleType, string biomeType)
        {
            SetSprite(poleType, biomeType);
        }

        private void SetSprite(PoleType poleType, string biomeType)
        {
            switch (poleType)
            {
                case PoleType.BOTTOM:
                    SetBottomByBiome(biomeType);
                    break;
                case PoleType.MIDDLE:
                    SetMiddleByBiome(biomeType);
                    break;
                case PoleType.TOP:
                    SetTopByBiome(biomeType);
                    break;
                case PoleType.MANHOLE:
                    SetManholeByBiome(biomeType);
                    break;
                case PoleType.MANHOLE_FLOOR:
                    SetManholeFloorByBiome(biomeType);
                    break;
            }
        }

        private void SetManholeFloorByBiome(string biomeType)
        {
            switch (biomeType)
            {
                case Biomes.BIOME_KEEN4_FOREST:
                    _sprite = Properties.Resources.keen4_forest_pole_tile;
                    break;
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _sprite = Properties.Resources.keen4_pyramid_pole_manhole_floor;
                    break;
                case Biomes.BIOME_KEEN4_CAVE:
                    _sprite = Properties.Resources.keen4_cave_pole_manhole_floor;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _sprite = Properties.Resources.keen4_mirage_pole_manhole_floor;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _sprite = Properties.Resources.keen5_black_pole_manhole_bottom_half;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _sprite = Properties.Resources.keen5_red_pole_manhole_bottom_half;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _sprite = Properties.Resources.keen5_green_pole_manhole_bottom_half;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _sprite = Properties.Resources.keen6_forest_pole_manhole_bottom_half;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _sprite = Properties.Resources.keen6_industrial_pole_manhole_bottom_half;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _sprite = Properties.Resources.keen6_dome_pole_manhole_bottom_half;
                    break;
            }

        }

        private void SetManholeByBiome(string biomeType)
        {
            switch (biomeType)
            {
                case Biomes.BIOME_KEEN4_FOREST:
                    _sprite = Properties.Resources.keen4_forest_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _sprite = Properties.Resources.keen4_pyramid_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN4_CAVE:
                    _sprite = Properties.Resources.keen4_cave_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _sprite = Properties.Resources.keen4_mirage_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _sprite = Properties.Resources.keen5_black_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _sprite = Properties.Resources.keen5_red_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _sprite = Properties.Resources.keen5_green_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _sprite = Properties.Resources.keen6_forest_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _sprite = Properties.Resources.keen6_industrial_pole_manhole;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _sprite = Properties.Resources.keen6_dome_pole_manhole;
                    break;
            }
        }

        private void SetTopByBiome(string biomeType)
        {
            switch (biomeType)
            {
                case Biomes.BIOME_KEEN4_FOREST:
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _sprite = Properties.Resources.pole_top;
                    break;
                case Biomes.BIOME_KEEN4_CAVE:
                    _sprite = Properties.Resources.keen4_cave_pole_top;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _sprite = Properties.Resources.keen4_mirage_pole_top;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _sprite = Properties.Resources.keen5_black_pole_top;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _sprite = Properties.Resources.keen5_red_pole_top;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _sprite = Properties.Resources.keen5_green_pole_top;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _sprite = Properties.Resources.keen6_forest_pole_top;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _sprite = Properties.Resources.keen6_industrial_pole_top;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _sprite = Properties.Resources.keen6_dome_pole_top;
                    break;
                case Biomes.BIOME_KEEN6_FINAL:
                    _sprite = Properties.Resources.keen6_eyeball_pole;
                    break;
            }
        }

        private void SetMiddleByBiome(string biomeType)
        {
            switch (biomeType)
            {
                case Biomes.BIOME_KEEN4_FOREST:
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _sprite = Properties.Resources.pole_middle;
                    break;
                case Biomes.BIOME_KEEN4_CAVE:
                    _sprite = Properties.Resources.keen4_cave_pole_middle;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _sprite = Properties.Resources.keen4_mirage_pole_middle;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _sprite = Properties.Resources.keen5_black_pole_middle;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _sprite = Properties.Resources.keen5_red_pole_middle;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _sprite = Properties.Resources.keen5_green_pole_middle;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _sprite = Properties.Resources.keen6_forest_pole_middle;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _sprite = Properties.Resources.keen6_industrial_pole_middle;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _sprite = Properties.Resources.keen6_dome_pole_middle;
                    break;
                case Biomes.BIOME_KEEN6_FINAL:
                    _sprite = Properties.Resources.keen6_eyeball_pole;
                    break;
            }
        }

        private void SetBottomByBiome(string biomeType)
        {
            switch (biomeType)
            {
                case Biomes.BIOME_KEEN4_FOREST:
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _sprite = Properties.Resources.pole_bottom;
                    break;
                case Biomes.BIOME_KEEN4_CAVE:
                    _sprite = Properties.Resources.keen4_cave_pole_bottom;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _sprite = Properties.Resources.keen4_mirage_pole_bottom;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _sprite = Properties.Resources.keen5_black_pole_bottom;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _sprite = Properties.Resources.keen5_red_pole_bottom;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _sprite = Properties.Resources.keen5_green_pole_bottom;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _sprite = Properties.Resources.keen6_forest_pole_bottom;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _sprite = Properties.Resources.keen6_industrial_pole_bottom;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _sprite = Properties.Resources.keen6_dome_pole_bottom;
                    break;
                case Biomes.BIOME_KEEN6_FINAL:
                    _sprite = Properties.Resources.keen6_eyeball_pole;
                    break;
            }
        }



        private Image _sprite;
        private string _biome;
        private PoleType _poleType;

        public string Biome => _biome;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.CollisionTile?.Location ?? _originalLocation;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                Create(this, args);
            }
        }

        protected void OnRemove(ObjectEventArgs args)
        {
            if (this.Remove != null)
            {
                this.Remove(this, args);
            }
        }

        public void ChangeBiome(string newBiome)
        {
            _biome = newBiome;
            SetSprite(_poleType, _biome);
        }
    }
}
