using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen5SpinningBurnPlatform : Hazard, IUpdatable, ICreateRemove
    {
        private Image[] _sprites = SpriteSheet.SpriteSheet.SpinningBurnPlatformImages;
        private readonly int _startIndex;
        private readonly string _startingImageName;
        private bool _hasBurner;
        private int _currentIndex;
        private const int UPDATE_DELAY = 2;
        private int _currentUpdateDelayTick;
        private Keen5PlatformBurner _burner;
        private Rectangle _area;

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;


        public Keen5SpinningBurnPlatform(Rectangle area, SpaceHashGrid grid, string startingImageName, int zIndex, bool hasBurner = true, int startIndex = 6)
            : base(grid, area, Enums.HazardType.KEEN5_SPINNING_BURN_PLATFORM, zIndex)
        {
            _startIndex = startIndex;
            _area = area;
            _startingImageName = startingImageName;
            _hasBurner = hasBurner;
            if (_hasBurner)
            {
                _currentIndex = startIndex;
                _burner = new Keen5PlatformBurner(_collisionGrid, new Rectangle(area.Location, area.Size), _zIndex);
                _burner.SpinSequence = _currentIndex;
                OnCreate(new ObjectEventArgs() { ObjectSprite = _burner });
            }
            this.Tile = new InvisiblePlatformTile(grid, new Rectangle(area.Location, area.Size));
            _sprite = _sprites[_startIndex];
        }

        public InvisiblePlatformTile Tile
        {
            get;
            private set;
        }

        public int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
        }

        public override bool IsDeadly
        {
            get
            {
                return false;
            }
        }

        public void Update()
        {
            if (_hasBurner && _currentUpdateDelayTick++ == UPDATE_DELAY)
            {
                UpdateSprite();
                UpdateDeadlyBurnerPosition();
            }
        }

        private void UpdateDeadlyBurnerPosition()
        {
            int nextSpriteHeight = _sprites[_currentIndex].Height;
            int referenceHeight = _sprites[6].Height;
            int yOffset = nextSpriteHeight < referenceHeight ? 0 : nextSpriteHeight - referenceHeight;
            if (_currentIndex >= 3 && _currentIndex <= 5)
            {
                Rectangle newHitbox = new Rectangle(new Point(_burner.HitBox.X, Tile.HitBox.Bottom), new Size(Tile.HitBox.Width, Math.Abs(yOffset)));
                _burner.UpdateLocation(newHitbox);
            }
            else
            {
                Rectangle newHitbox = new Rectangle(new Point(_burner.HitBox.X, Tile.HitBox.Y - Math.Abs(yOffset)), new Size(Tile.HitBox.Width, Math.Abs(yOffset)));
                _burner.UpdateLocation(newHitbox);
            }
            _burner.SpinSequence = _currentIndex;
            KillCollidingKeens();
        }

        private void KillCollidingKeens()
        {
            var collisions = this.CheckCollision(_burner.HitBox);
            var keens = collisions.Where(c => c.CollisionType == Enums.CollisionType.PLAYER);
            if (keens.Any())
            {
                foreach (var keen in keens)
                {
                    var player = keen as CommanderKeen;
                    if (player != null)
                    {
                        player.Die();
                    }
                }
            }
        }

        public override Point Location =>_area.Location;

        private void UpdateSprite()
        {
            _currentUpdateDelayTick = 0;
            int nextSpriteIndex = _currentIndex + 1;
            if (nextSpriteIndex >= _sprites.Length)
            {
                nextSpriteIndex = 0;
            }

            int nextSpriteHeight = _sprites[nextSpriteIndex].Height;
            int referenceHeight = _sprites[6].Height;
            int yOffset = nextSpriteHeight < referenceHeight ? 0 : nextSpriteHeight - referenceHeight;
            if (nextSpriteIndex < 3 || nextSpriteIndex == 7)
            {
                _area.Location = new Point(_area.Location.X, Tile.HitBox.Y - yOffset);
            }
            _sprite = _sprites[nextSpriteIndex];
            _currentIndex = nextSpriteIndex;
        }



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
                if (args.ObjectSprite == this)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                    }
                }
                this.Remove(this, args);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

            return $"{_startingImageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_startingImageName}{separator}{_zIndex}{separator}{_hasBurner}{separator}{_startIndex}";
        }
    }
}
