using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Pole : CollisionObject, ICreateRemove, IBiomeTile
    {
        private const int POLE_WIDTH = 12;
        private int _zIndex;
        public Pole(SpaceHashGrid grid, Rectangle hitbox, int zIndex, PoleType poleType, string biomeType, int addedLengths = 0)
            : base(grid, hitbox)
        {
            _zIndex = zIndex;
            _biomeType = biomeType;
            _poleType = poleType;
            _addedLengths = addedLengths;
            Initialize();
        }

        public PoleSprite Manhole { get; private set; }

        public PoleSprite ManholeFloor { get; private set; }

        private void Initialize()
        {
            this.Sprites = new List<ISprite>();
            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y, POLE_WIDTH, 59 + (32 * _addedLengths));
            if (_poleType == PoleType.MANHOLE || _poleType == PoleType.MANHOLE_FLOOR)
            {
                int widthDiff = PoleSprite.MANHOLE_WIDTH - this.HitBox.Width;

                PoleSprite pTop = new PoleSprite(PoleType.TOP, _biomeType, new Point(this.HitBox.X, this.HitBox.Y), _zIndex);
                this.Sprites.Add(pTop);

                PoleSprite pManhole = new PoleSprite(PoleType.MANHOLE, _biomeType, new Point(this.HitBox.X - (widthDiff / 2), pTop.CollisionTile.HitBox.Bottom), _zIndex);
                this.Manhole = pManhole;
                PoleSprite pManholeFloor = new PoleSprite(PoleType.MANHOLE_FLOOR, _biomeType,
                    new Point(this.HitBox.X - (widthDiff / 2), pManhole.CollisionTile.HitBox.Bottom), _zIndex);
                this.ManholeFloor = pManholeFloor;

                pManholeFloor.CollisionTile = new PoleTile(_collisionGrid, new Rectangle(pManholeFloor.Location, 
                    pManholeFloor.CollisionTile.HitBox.Size), pManholeFloor.Image, _zIndex);
                pManholeFloor.Create += new EventHandler<ObjectEventArgs>(pManholeFloor_Create);
                pManholeFloor.Remove += new EventHandler<ObjectEventArgs>(pManholeFloor_Remove);

                OnCreate(new ObjectEventArgs() { ObjectSprite = pManholeFloor.CollisionTile });

                this.Sprites.Add(pManhole);
                this.Sprites.Add(pManholeFloor);
                ManHoleFloorSprite = pManholeFloor;
            }
            else
            {
                PoleSprite pTop = new PoleSprite(PoleType.TOP, _biomeType, new Point(this.HitBox.X, this.HitBox.Y), _zIndex);
                this.Sprites.Add(pTop);
            }
            for (int i = 1; i <= _addedLengths; i++)
            {
                PoleSprite Pmiddle = new PoleSprite(PoleType.MIDDLE, _biomeType, new Point(this.HitBox.X, this.HitBox.Y + (32 * i)), _zIndex);
                this.Sprites.Add(Pmiddle);
            }
            PoleSprite pBottom = new PoleSprite(PoleType.BOTTOM, _biomeType, new Point(this.HitBox.X, this.HitBox.Bottom - 28), _zIndex);
            this.Sprites.Add(pBottom);
            this.UpdateCollisionNodes(Direction.DOWN);
            ResetCollidingNodes();
        }

        void pManholeFloor_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void pManholeFloor_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
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
        protected int _addedLengths;
        private PoleType _poleType;
        private string _biomeType;

        public List<ISprite> Sprites
        {
            get;
            private set;
        }

        public PoleSprite ManHoleFloorSprite { get; private set; }

        public string Biome => _biomeType;

        public Point SpriteLocation => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.POLE;

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
    }

    public class PoleSprite : ISprite, ICreateRemove, IBiomeTile
    {
        public const int MANHOLE_WIDTH = 96;
        private readonly int _zIndex;

        internal PoleSprite(PoleType type, string biomeType, Point p, int zIndex)
        {
            _zIndex = zIndex;
            _biome = biomeType;
            _poleType = type;
            Initialize(type, biomeType, p);
        }

        public PoleTile CollisionTile { get; set; }

        private void Initialize(PoleType poleType, string biomeType, Point p)
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
                    _sprite = Properties.Resources.keen5_black_pole_manhole_floor;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _sprite = Properties.Resources.keen5_red_pole_manhole_floor;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _sprite = Properties.Resources.keen5_green_pole_manhole_floor;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _sprite = Properties.Resources.keen6_forest_pole_manhole_floor;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _sprite = Properties.Resources.keen6_industrial_pole_manhole_floor;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _sprite = Properties.Resources.keen6_dome_pole_manhole_floor;
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

        public Point Location => this.CollisionTile.Location; 

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
