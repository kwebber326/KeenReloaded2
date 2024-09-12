using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class BlueEagleEgg : DestructibleObject, IUpdatable, IEnemy, ISprite, ICreateRemove
    {
        private Image _sprite;
        private bool _hatched;
        private bool _animationComplete;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _spriteChangeDelayTick;
        private int _currentSprite = 0;
        private const int FALL_VELOCITY = 30;
        private int _zIndex;

        public BlueEagleEgg(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            this.Health = 1;
            _sprite = Properties.Resources.keen4_blue_eagle_egg;

        }

        protected override void HandleCollision(CollisionObject obj)
        {

        }

        public override void Die()
        {
            this.Hatch();
        }

        private void Hatch()
        {
            if (!_hatched)
            {
                _hatched = true;
                BlueEagle e = new BlueEagle(new Rectangle(this.HitBox.X, this.HitBox.Bottom - 62, 60, 62), _collisionGrid, _zIndex);
                OnCreate(e);
            }
        }

        private CollisionObject GetTopMostLandingTile()
        {
            CollisionObject topMostTile;
            Rectangle areaTocheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + 2);
            var items = this.CheckCollision(areaTocheck, true);

            if (!items.Any())
                return null;

            int minY = items.Select(c => c.HitBox.Top).Min();
            topMostTile = items.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        private void Fall()
        {
            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + FALL_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            this.UpdateCollisionNodes(Direction.DOWN);
        }

        private void Land(CollisionObject obj)
        {
            if (obj != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                this.UpdateCollisionNodes(Direction.DOWN);
            }
        }

        private void UpdateHatchedSprite()
        {
            if (_spriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _spriteChangeDelayTick = 0;
                switch (_currentSprite)
                {
                    case 0:
                        _currentSprite++;
                        _sprite = Properties.Resources.keen4_blue_eagle_egg_hatch1;
                        this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                        this.UpdateCollisionNodes(Direction.DOWN);
                        break;
                    case 1:
                        _currentSprite++;
                        _sprite = Properties.Resources.keen4_blue_eagle_egg_hatch2;
                        this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
                        this.UpdateCollisionNodes(Direction.DOWN);
                        _animationComplete = true;
                        break;
                }
            }
        }

        private void ExecuteGravity()
        {
            CollisionObject obj = GetTopMostLandingTile();
            if (obj != null)
            {
                this.Land(obj);
            }
            else
            {
                this.Fall();
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
            }
        }

        public bool DeadlyTouch
        {
            get { return false; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return !_hatched; }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public void Update()
        {
            var collisions = this.CheckCollision(this.HitBox);
            var keens = collisions.OfType<CommanderKeen>();
            if (!_hatched && keens.Any())
            {
                this.Hatch();
            }
            else if (_hatched && !_animationComplete)
            {
                UpdateHatchedSprite();
            }
            ExecuteGravity();
        }

        protected void OnCreate(ISprite obj)
        {
            if (Create != null)
            {
                Create(this, new ObjectEventArgs() { ObjectSprite = obj });
            }
        }

        protected void OnRemove(ISprite obj)
        {
            if (Remove != null)
            {
                Remove(this, new ObjectEventArgs() { ObjectSprite = obj });
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_blue_eagle_egg);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
