using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class Dart : CollisionObject, IProjectile, IMoveable, IUpdatable, ISprite, ICreateRemove
    {
        public Dart(Rectangle area, SpaceHashGrid grid, Direction direction)
            : base(grid, area)
        {
            Initialize(direction);
        }

        private Image[] _images;

        private void Initialize(Direction direction)
        {
            this.Direction = direction;
            switch (direction)
            {
                case Enums.Direction.DOWN:
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_dart_down1,
                        Properties.Resources.keen4_dart_down2
                    };
                    break;

                case Enums.Direction.UP:
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_dart_up1,
                        Properties.Resources.keen4_dart_up2
                    };
                    break;

                case Enums.Direction.RIGHT:
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_dart_right1,
                        Properties.Resources.keen4_dart_right2
                    };
                    break;

                case Enums.Direction.LEFT:
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_dart_left1,
                        Properties.Resources.keen4_dart_left2
                    };
                    break;
            }
            if (_images != null && _images.Any())
            {
                Random random = new Random();
                _currentSprite = random.Next(0, _images.Length);
                _sprite = _images[_currentSprite];
            }

            OnCreated();
        }

        public bool KillsKeen
        {
            get
            {
                return true;
            }
        }

        public int Damage
        {
            get { return int.MaxValue; }
        }

        public int Velocity
        {
            get { return 70; }
        }

        public int Pierce
        {
            get { return 0; }
        }

        public int Spread
        {
            get { return 0; }
        }

        public int BlastRadius
        {
            get { return 0; }
        }

        public int RefireDelay
        {
            get { return 10; }
        }

        public void Move()
        {
            Rectangle areaToCheck = new Rectangle();
            switch (_direction)
            {
                case Enums.Direction.DOWN:
                    areaToCheck = new Rectangle(this.HitBox.Location.X, this.HitBox.Location.Y, this.HitBox.Width, this.HitBox.Height + Velocity);
                    break;
                case Enums.Direction.UP:
                    areaToCheck = new Rectangle(this.HitBox.Location.X, this.HitBox.Location.Y - Velocity, this.HitBox.Width, this.HitBox.Height + Velocity);
                    break;
                case Enums.Direction.LEFT:
                    areaToCheck = new Rectangle(this.HitBox.Location.X - Velocity, this.HitBox.Location.Y, this.HitBox.Width + Velocity, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    areaToCheck = new Rectangle(this.HitBox.Location.X, this.HitBox.Location.Y, this.HitBox.Width + Velocity, this.HitBox.Height);
                    break;
            }
            var collisionItems = this.CheckCollision(areaToCheck);
            var keens = collisionItems.OfType<CommanderKeen>();
            var walls = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK);

            if (walls.Any())
            {
                HitWallByDirection(walls, keens);
                this.Stop();
            }
            else
            {
                KillKeens(keens, areaToCheck);
                AdvancePositionByDirection();
            }
        }

        private void KillKeens(IEnumerable<CommanderKeen> keens, Rectangle areaToCheck)
        {
            if (keens.Any())
            {
                foreach (var keen in keens)
                {
                    if (keen.HitBox.IntersectsWith(areaToCheck))
                        keen.Die();
                }
            }
        }

        private void UpdateSprite()
        {
            if (_images != null && _images.Any())
            {
                if (_currentSprite < _images.Length)
                    _currentSprite++;
                else
                    _currentSprite = 0;

                this._sprite = _images[_currentSprite];
            }
        }

        private void AdvancePositionByDirection()
        {
            switch (_direction)
            {
                case Enums.Direction.DOWN:
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + Velocity, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - Velocity, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    this.HitBox = new Rectangle(this.HitBox.X + Velocity, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.LEFT:
                    this.HitBox = new Rectangle(this.HitBox.X - Velocity, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    break;
            }
        }

        private void HitWallByDirection(IEnumerable<CollisionObject> walls, IEnumerable<CommanderKeen> keens)
        {
            switch (_direction)
            {
                case Enums.Direction.DOWN:
                    int yMin = walls.Select(w => w.HitBox.Top).Min();
                    this.HitBox = new Rectangle(this.HitBox.X, yMin - this.HitBox.Height, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    int yMax = walls.Select(w => w.HitBox.Bottom).Max();
                    this.HitBox = new Rectangle(this.HitBox.X, yMax, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    int xMin = walls.Select(w => w.HitBox.Left).Min();
                    this.HitBox = new Rectangle(xMin - this.HitBox.Width, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.LEFT:
                    int xMax = walls.Select(w => w.HitBox.Right).Max();
                    this.HitBox = new Rectangle(xMax, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    break;
            }
            KillKeens(keens, this.HitBox);
        }

        public void Stop()
        {
            _hitWall = true;
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
                if (_sprite != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(this.Direction);
                }
            }
        }

        Enums.MoveState IMoveable.MoveState
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Enums.Direction Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }

        public void Update()
        {
            if (!_hitWall && !IsOutOfBounds(this.Direction))
            {
                this.Move();
            }
            else
            {
                OnRemoved();
            }
        }

        private void OnRemoved()
        {
            if (Remove != null)
            {
                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                if (_collidingNodes != null && _collidingNodes.Any())
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                        node.NonEnemies.Remove(this);
                    }
                }
                Remove(this, args);
            }
        }

        private void OnCreated()
        {
            if (Create != null)
            {
                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                Create(this, args);
            }
        }

        public int ZIndex => 20;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.ENEMY_PROJECTILE;

        private Direction _direction;
        private Image _sprite;
        private bool _hitWall = false;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private int _currentSprite;
    }
}
