using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class ForceField : DestructibleObject, ISprite, IEnemy
    {
        public const int GENERATOR_WIDTH = 32;
        public const int GENERATOR_HEIGHT = 64;
        private readonly int _zIndex;
        private bool _isActive;
        private ForceFieldTop _top;
        private ForceFieldBottom _bottom;
        private ForceFieldBarrier _barrier;
        private int _health;
        private Image _image;
        private Point _location;
        private Rectangle _area;

        public ForceField(Rectangle area, SpaceHashGrid grid, int zIndex, int health) : base(grid, area)
        {
            _area = area;
            _health = health < 1 ? 1 : health;
            _location = area.Location;
            _zIndex = zIndex;
            this.Activate();
        }

        public bool IsActive => _isActive;

        public ISprite Barrier => _barrier;

        public ISprite Top => _top;

        public ISprite Bottom => _bottom;

        public ProgressBar HealthBar => _barrier.HealthBar;

        public bool DeadlyTouch => false;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.DESTRUCTIBLE_BLOCK;

        public int ZIndex => _zIndex;

        public Image Image => _image;

        public Point Location => _location;

        public void Activate()
        {
            _isActive = true;

            int bottomX = _area.X;
            int bottomY = _area.Bottom - GENERATOR_HEIGHT;
            int barrierX = _area.X;
            int barrierY = _area.Y + GENERATOR_HEIGHT;
            int barrierWidth = _area.Width;
            int barrierHeight = _area.Height - (GENERATOR_HEIGHT * 2);

            Rectangle generatorAreaTop = new Rectangle(_area.X, _area.Y, GENERATOR_WIDTH, GENERATOR_HEIGHT);
            Rectangle generatorAreaBottom = new Rectangle(bottomX, bottomY, GENERATOR_WIDTH, GENERATOR_HEIGHT);
            Rectangle barrierArea = new Rectangle(barrierX, barrierY, barrierWidth, barrierHeight);
            string imageFileTop = FileIOUtility.GetResourcePathForMainProject() + @"\" + nameof(Properties.Resources.keen5_force_field_top) + ".png";
            string imageFileBottom = FileIOUtility.GetResourcePathForMainProject() + @"\" + nameof(Properties.Resources.keen5_force_field_top) + ".png";
            _top = new ForceFieldTop(_collisionGrid, generatorAreaTop, imageFileTop, _zIndex);
            _bottom = new ForceFieldBottom(_collisionGrid, generatorAreaBottom, imageFileBottom, _zIndex);
            _barrier = new ForceFieldBarrier(_collisionGrid, barrierArea, _health, _zIndex);
            _barrier.Killed += _barrier_Killed;
            _barrier.Redrawn += _barrier_Redrawn;

            this.DrawFullImage();
        }

        private void _barrier_Redrawn(object sender, EventArgs e)
        {
            this.DrawFullImage();
        }

        private void DrawFullImage()
        {
            if (_top == null || _bottom == null || _barrier == null)
                return;

            List<LocatedImage> locatedImages = new List<LocatedImage>()
            {
                new LocatedImage()
                {
                    Image = _top.Image,
                    Location = new Point(0, 0)
                },
                new LocatedImage()
                {
                    Image = _barrier.Image,
                    Location = new Point(0, _top.Image.Height)
                },
                new LocatedImage()
                {
                    Image = _bottom.Image,
                    Location = new Point(0, _area.Height - _bottom.Image.Height)
                }
            };

            Size canvas = _area.Size;
            Image[] images = locatedImages.Select(i => i.Image).ToArray();
            Point[] locations = locatedImages.Select(i => i.Location).ToArray();

            _image = BitMapTool.DrawImagesOnCanvas(canvas, null, images, locations);
        }

        private void _barrier_Killed(object sender, ObjectEventArgs e)
        {
            this.Deactivate();
        }

        public void Deactivate()
        {
            _isActive = false;
            this.DrawFullImage();
        }

        public override bool IsDead()
        {
            return !_isActive;
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

            return $"{nameof(Properties.Resources.keen5_force_field_bottom)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_health}";
        }

        public override void Die()
        {

        }

        public override void TakeDamage(int damage)
        {
            if (this.IsDead())
                return;

            _barrier.TakeDamage(damage);
        }

        public override void TakeDamage(IProjectile projectile)
        {
            if (this.IsDead())
                return;

            var collisionObject = (CollisionObject)projectile;
            if (collisionObject.HitBox.Bottom > _barrier.HitBox.Top && collisionObject.HitBox.Top < _barrier.HitBox.Bottom)
                _barrier.TakeDamage(projectile);
        }

        public void HandleHit(IProjectile projectile)
        {

        }


    }

    internal class ForceFieldBarrier : DestructibleObject, ISprite, ICreateRemove, IHealthBar
    {
        private HealthBarTile _collisionTile;
        private Timer _animationTimer;
        private bool _isDestroyed;

        private const int EXPLOSION_ANIMATIONS = 8;
        private int _currentExplosionAnimation;
        private Image _image;
        private Point _location;
        private readonly int _zIndex;
        private CollisionType _collisionType = CollisionType.BLOCK;
        public event EventHandler Redrawn;


        public ForceFieldBarrier(SpaceHashGrid grid, Rectangle hitbox, int health, int zIndex) : base(grid, hitbox)
        {
            _zIndex = zIndex;
            Initialize(grid, hitbox, health);
        }

        private void Initialize(SpaceHashGrid grid, Rectangle hitbox, int health)
        {
            DrawForceFieldImage(hitbox);
            this.Health = health;

            _collisionTile = new HealthBarTile(grid, hitbox, this.Health, false);
            OnCreate(_collisionTile);

            _animationTimer = new Timer();
            _animationTimer.Interval = 300;
            _animationTimer.Tick += _animationTimer_Tick;
            SetHealthBarValue();
        }


        private void DrawForceFieldImage(Rectangle hitbox)
        {
            string filePath = FileIOUtility.GetResourcePathForMainProject() + @"\" + nameof(Properties.Resources.keen5_laser_field_laser3) + ".png";
            int imgHeight = Properties.Resources.keen5_laser_field_laser3.Height;
            int imageCount = hitbox.Height / imgHeight;
            if (hitbox.Height % imgHeight != 0)
                imageCount++;

            var fileList = new string[imageCount];
            for (int i = 0; i < imageCount; i++)
            {
                fileList[i] = filePath;
            }
            _image = BitMapTool.CombineBitmap(fileList, 1, Color.White);
            Redrawn?.Invoke(this, EventArgs.Empty);
        }

        private void DrawExplosionImage(Rectangle hitbox)
        {
            int imgVal = _random.Next(1, 3);
            string filePath1 = FileIOUtility.GetResourcePathForMainProject() + @"\" + nameof(Properties.Resources.keen5_force_field_explosion1) + ".png";
            string filePath2 = FileIOUtility.GetResourcePathForMainProject() + @"\" + nameof(Properties.Resources.keen5_force_field_explosion2) + ".png";
            int imgHeight = Properties.Resources.keen5_force_field_explosion1.Height; //same height, can pick either one
            int imageCount = hitbox.Height / imgHeight;

            if (hitbox.Height % imgHeight != 0)
                imageCount++;

            var fileList = new string[imageCount];
            for (int i = 0; i < imageCount; i++)
            {
                fileList[i] = imgVal == 1 ? filePath1 : filePath2;
                imgVal = _random.Next(1, 3);
            }
            _image = BitMapTool.CombineBitmap(fileList, 1, Color.Transparent);
            Redrawn?.Invoke(this, EventArgs.Empty);
        }

        private void _animationTimer_Tick(object sender, EventArgs e)
        {
            if (!this.IsDead())
            {
                DrawForceFieldImage(this.HitBox);
                _animationTimer.Stop();
            }
            else if (++_currentExplosionAnimation < EXPLOSION_ANIMATIONS)
            {
                DrawExplosionImage(this.HitBox);
            }
            else
            {
                _animationTimer.Stop();
                _image = null;
                _isDestroyed = true;
                OnRemove(this);
                Redrawn?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetHealthBarValue()
        {
            _collisionTile?.SetHealthBarValue(this.Health);
        }

        private void SetHealthBarVisible()
        {
            _collisionTile?.SetHealthBarVisiblity(true);
        }

        public override void TakeDamage(IProjectile projectile)
        {
            base.TakeDamage(projectile);
            DrawExplosionImage(this.HitBox);
            ExecuteHitLogic();
            SetHealthBarValue();
            SetHealthBarVisible();
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            DrawExplosionImage(this.HitBox);
            ExecuteHitLogic();
            SetHealthBarValue();
            SetHealthBarVisible();
        }

        private void ExecuteHitLogic()
        {
            if (!this.IsDead())
                ExecuteHitAnimation();
            else if (!_isDestroyed)
            {
                DrawExplosionImage(this.HitBox);
                ExecuteDestructionAnimation();
            }
        }

        private void ExecuteHitAnimation()
        {
            _image = null;
            if (!_animationTimer.Enabled)
            {
                _animationTimer.Start();
            }
        }

        private void ExecuteDestructionAnimation()
        {
            _animationTimer.Interval = 100;
            if (!_animationTimer.Enabled)
            {
                _animationTimer.Start();
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
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public ProgressBar HealthBar => _collisionTile?.HealthBar;

        public override CollisionType CollisionType => _collisionType;

        public int ZIndex => _zIndex;

        public Image Image => _image;

        public Point Location => _location;

        public bool CanUpdate => true;

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;


        protected void OnCreate(ISprite obj)
        {
            ObjectEventArgs eventArgs = new ObjectEventArgs()
            {
                ObjectSprite = obj
            };
            this.Create?.Invoke(this, eventArgs);
        }

        protected void OnRemove(ISprite obj)
        {
            ObjectEventArgs eventArgs = new ObjectEventArgs()
            {
                ObjectSprite = obj
            };
            foreach (var node in _collidingNodes)
            {
                var collisionObj = obj as CollisionObject;
                if (collisionObj != null)
                {
                    node.Tiles.Remove(collisionObj);
                    node.NonEnemies.Remove(collisionObj);
                    node.Objects.Remove(collisionObj);
                }
            }
            this.Remove?.Invoke(this, eventArgs);
            _collisionTile = null;
        }

        public override void Die()
        {
            OnRemove(_collisionTile);
            _image = null;
            _collisionType = CollisionType.NONE;
        }
    }

    internal class ForceFieldTop : MaskedTile
    {

        public ForceFieldTop(SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex) : base(hitbox, grid, hitbox, imageFile, zIndex)
        {
            Initialize(hitbox);
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;

        private void Initialize(Rectangle hitbox)
        {

            _image = Properties.Resources.keen5_force_field_top;
        }
    }
    internal class ForceFieldBottom : MaskedTile
    {
        public ForceFieldBottom(SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex) : base(hitbox, grid, hitbox, imageFile, zIndex)
        {
            Initialize();
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;

        private void Initialize()
        {
            _image = Properties.Resources.keen5_force_field_bottom;
        }
    }
}
