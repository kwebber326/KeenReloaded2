using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class BiomeChanger : DestructibleObject, ISprite, IUpdatable, ICreateRemove, IFireable, IEnemy
    {
        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private int _fireDelay;
        private Image _sprite;
        private bool _isFiring;
        private int _maxVelocity;
        private int _currentFireDelayTick;

        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;

        private const int FIRE_TIME = 3;
        private int _currentFireTimeTick;

        private Image[] _sprites;

        private bool _hitAnimation;
        private const int HIT_ANIMATION_TIME = 3;
        private int _hitAnimationTimeTick;
        private int _maxDelay;
        private readonly int _zIndex;
        private int _minDelay;

        public BiomeChanger(Rectangle area, SpaceHashGrid grid, int zIndex, int minDelay, int maxDelay, int maxVelocty, int health)
            : base(grid, area)
        {
            this.HitBox = area;
            this.Health = health;
            _zIndex = zIndex;
            _minDelay = minDelay;
            _maxDelay = maxDelay < minDelay ? minDelay : maxDelay;
            _fireDelay = _random.Next(_minDelay, _maxDelay + 1);
            _maxVelocity = maxVelocty;
            Initialize();
        }

        private void Initialize()
        {
            _sprites = SpriteSheet.SpriteSheet.BiomeChangerImages;
            _currentSprite = _random.Next(0, _sprites.Length);
            _sprite = _sprites[_currentSprite];
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
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public override void Die()
        {
            _sprite = Properties.Resources.keen5_quantum_dynamo_sphere_destroyed;
            ResetHitAnimation();
        }

        public void Update()
        {
            if (!this.IsDead())
            {
                if (_hitAnimation && _hitAnimationTimeTick++ == HIT_ANIMATION_TIME)
                {
                    ResetHitAnimation();
                }

                if (_isFiring)
                {
                    this.Fire();
                }
                else if (_currentFireDelayTick++ == _fireDelay)
                {
                    _currentFireDelayTick = 0;
                    this.Fire();
                }

                this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSprite, SPRITE_CHANGE_DELAY, UpdateSprite);
            }
        }

        private void ResetHitAnimation()
        {
            _hitAnimationTimeTick = 0;
            _hitAnimation = false;
        }

        private void UpdateSprite()
        {
            if (++_currentSprite >= _sprites.Length)
            {
                _currentSprite = 0;
            }
            _sprite = _sprites[_currentSprite];
            if (_hitAnimation)
            {
                _sprite = this.GetCurrentSpriteWithWhiteBackground(_sprite);
            }
        }

        public void Fire()
        {
            if (!_isFiring)
                _isFiring = true;

            if (_currentFireTimeTick++ == 0)
            {
                _sprite = this.GetCurrentSpriteWithBackgroundColor(_sprite, Color.Red);
                //fire shot
                CreateBiomeShot();
            }
            else if (_currentFireTimeTick == FIRE_TIME)
            {
                _sprite = _sprites[_currentSprite];
                _currentFireTimeTick = 0;
                _isFiring = false;
                _fireDelay = _random.Next(_minDelay, _maxDelay);
            }
        }

        private void CreateBiomeShot()
        {
            //random velocity
            int randHorizontalVelocity = _random.Next(_maxVelocity * -1, _maxVelocity + 1);
            if (randHorizontalVelocity == 0)
                randHorizontalVelocity = 1;//don't divide by zero, genius
            _random = new Random();
            int randVerticalVelocity = _random.Next(_maxVelocity * -1, _maxVelocity + 1);
            if (randVerticalVelocity == 0)
                randVerticalVelocity = 1;//don't divide by zero, genius
            //random biome
            var constants = CommonGameFunctions.GetConstants(typeof(Biomes));
            int biomeCount = constants.Count;
            int randVal = _random.Next(0, biomeCount);
            string tileType = constants[randVal].GetRawConstantValue()?.ToString();

            //block projectile should begin at a point on the sphere that is indicative of its intended trajectory
            int xPos = randHorizontalVelocity < 0 ? this.HitBox.X - 48 : this.HitBox.Right + 48;
            double yPosOffset = ((double)this.HitBox.Height / 2 / (double)randVerticalVelocity);
            int yOffsetVal = (int)((this.HitBox.Height / 2) * yPosOffset);
            int yPos = this.HitBox.Y + (this.HitBox.Height / 2) + yOffsetVal;
            xPos = randVerticalVelocity < 0 ? xPos + Math.Abs(yOffsetVal) : xPos - Math.Abs(yOffsetVal);

            //instantiate projectile
            BiomeProjectile biomeChangerProjectile = new BiomeProjectile(_collisionGrid, new Rectangle(xPos, yPos, 48, 64), randVerticalVelocity, randHorizontalVelocity, tileType);
            biomeChangerProjectile.Create += new EventHandler<ObjectEventArgs>(biomeChangerProjectile_Create);
            biomeChangerProjectile.Remove += new EventHandler<ObjectEventArgs>(biomeChangerProjectile_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = biomeChangerProjectile });
        }

        void biomeChangerProjectile_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void biomeChangerProjectile_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public bool IsFiring
        {
            get { return _isFiring; }
        }

        public int Ammo
        {
            get { return -1; }
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
                this.Remove(this, args);
            }
        }

        public bool DeadlyTouch
        {
            get { return false; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);

            if (this.Health > 0)
            {
                _hitAnimation = true;
                UpdateSprite();
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (!this.IsDead())
            {
                _hitAnimation = true;
                UpdateSprite();
            }
        }

        public bool IsActive
        {
            get { return !this.IsDead(); }
        }

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            int minDelay = _minDelay;
            int maxDelay = _maxDelay;
            int maxVelocty = _maxVelocity;
            int health = this.Health;
            string imageName = nameof(Properties.Resources.keen5_quantum_dynamo_sphere1);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{minDelay}{separator}{maxDelay}{separator}{maxVelocty}{separator}{health}";
        }
    }
}
