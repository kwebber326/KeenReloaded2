using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Hazards;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class KeenStunShot : CollisionObject, IProjectile, IMoveable, IUpdatable, ISprite
    {
        public KeenStunShot(SpaceHashGrid grid, Rectangle hitbox,
            int damage, int velocity, int pierce, int spread, int blastRadius, int refireDelay, Direction direction)
            : base(grid, hitbox)
        {
            _damage = damage;
            _velocity = velocity;
            _pierce = pierce;
            _spread = spread;
            _blastRadius = blastRadius;
            _refireDelay = refireDelay;
            this.Direction = direction;
            InitializeSprites();
        }

        private HashSet<IEnemy> _hitObjects = new HashSet<IEnemy>();
        private HashSet<CommanderKeen> _hitKeens = new HashSet<CommanderKeen>();

        private bool _updatedOnDeflection;

        protected void HandleCollision(CollisionObject obj)
        {
            if (obj.CollisionType == CollisionType.BLOCK)
            {
                StopAtCollisionObject(obj);
            }
            else if (obj is IEnemy && !(obj is ForceField))
            {
                var enemy = (IEnemy)obj;
                if (enemy.IsActive)
                {
                    if (obj is IDeflector && !_deflected)
                    {
                        var deflector = (IDeflector)obj;

                        if (deflector.DeflectsHorizontally && this.IsHorizontalDirection(this.Direction))
                        {
                            if (!_updatedOnDeflection)
                            {
                                SetLocationByCollision(obj);
                                this.Direction = ChangeHorizontalDirection(this.Direction);
                                _deflected = true;
                                this.Update();
                                this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                                this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                                _updatedOnDeflection = true;
                                return;
                            }
                        }
                        else if (deflector.DeflectsVertically && this.IsVerticalDirection(this.Direction))
                        {
                            if (!_updatedOnDeflection)
                            {
                                SetLocationByCollision(obj);
                                this.Direction = this.ChangeVerticalDirection(this.Direction);
                                _deflected = true;
                                this.Update();
                                this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                                this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                                return;
                            }
                        }
                    }
                    else if (obj is IDeflector && _deflected)
                    {
                        var deflector = (IDeflector)obj;
                        if (deflector.DeflectsHorizontally && this.IsHorizontalDirection(this.Direction))
                        {
                            if (!_updatedOnDeflection)
                            {
                                this.Update();
                                this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                                this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                                _updatedOnDeflection = true;
                                return;
                            }
                            else
                            {
                                _updatedOnDeflection = false;
                                return;
                            }
                        }
                        else if (deflector.DeflectsVertically && this.IsVerticalDirection(this.Direction))
                        {
                            if (!_updatedOnDeflection)
                            {
                                this.Update();
                                this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                                this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                                _updatedOnDeflection = true;
                                return;
                            }
                            else
                            {
                                _updatedOnDeflection = false;
                                return;
                            }
                        }
                    }
                    if (!_hitObjects.Contains(enemy))
                    {
                        enemy.HandleHit(this);
                        _hitObjects.Add(enemy);
                        if (--_pierce < 0)
                        {
                            StopAtCollisionObject(obj);
                        }
                    }
                }
            }
            else if (obj is ICancellableProjectile)
            {
                var t = (ICancellableProjectile)obj;
                t.Cancel();
                if (--_pierce < 0)
                {
                    StopAtCollisionObject(obj);
                }
            }
            else if (_deflected && obj is CommanderKeen)
            {
                var keen = (CommanderKeen)obj;
                keen.Stun();
                _hitKeens.Add(keen);
                if (--_pierce < 0)
                {
                    StopAtCollisionObject(obj);
                }
            }
            else if (obj is DestructibleObject && !(obj is CommanderKeen))
            {
                var destructoObject = (DestructibleObject)obj;
                destructoObject.TakeDamage(this);
            }
        }

        private void StopAtCollisionObject(CollisionObject obj)
        {
            SetLocationByCollision(obj);
            this.Stop();
        }

        protected void SetLocationByCollision(CollisionObject obj)
        {
            switch (Direction)
            {
                case Enums.Direction.LEFT:
                    if (_spreadOffset == 0 || (obj.HitBox.Top < this.HitBox.Bottom && obj.HitBox.Bottom > this.HitBox.Top))
                    {
                        this.HitBox = new Rectangle(obj.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                    else if (_spreadOffset < 0)
                    {
                        int heightDiff = obj.HitBox.Bottom - this.HitBox.Top;
                        this.HitBox = new Rectangle(this.HitBox.X - heightDiff, obj.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                    }
                    else if (_spreadOffset > 0)
                    {
                        int heightDiff = this.HitBox.Bottom - obj.HitBox.Top;
                        this.HitBox = new Rectangle(this.HitBox.X - heightDiff, obj.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                    }
                    break;
                case Enums.Direction.RIGHT:
                    if (_spreadOffset == 0 || (obj.HitBox.Top < this.HitBox.Bottom && obj.HitBox.Bottom > this.HitBox.Top))
                    {
                        this.HitBox = new Rectangle(obj.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                    else if (_spreadOffset < 0)
                    {
                        int heightDiff = obj.HitBox.Bottom - this.HitBox.Top;
                        this.HitBox = new Rectangle(this.HitBox.X + heightDiff, obj.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                    }
                    else if (_spreadOffset > 0)
                    {
                        int heightDiff = this.HitBox.Bottom - obj.HitBox.Top;
                        this.HitBox = new Rectangle(this.HitBox.X + heightDiff, obj.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                    }
                    break;
                case Enums.Direction.UP:
                    if (obj.HitBox.Right > this.HitBox.Left && obj.HitBox.Left < this.HitBox.Right && obj.HitBox.Bottom <= this.HitBox.Top)
                    {
                        this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                    }
                    else if (_spreadOffset < 0 || obj.HitBox.Left < this.HitBox.Left)
                    {
                        int widthDiff = obj.HitBox.Right - this.HitBox.Left;
                        this.HitBox = new Rectangle(obj.HitBox.Right + 1, this.HitBox.Y - widthDiff, this.HitBox.Width, this.HitBox.Height);
                    }
                    else if (_spreadOffset > 0 || obj.HitBox.Right > this.HitBox.Right)
                    {
                        int widthDiff = obj.HitBox.Left - this.HitBox.Right;
                        this.HitBox = new Rectangle(obj.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y - widthDiff, this.HitBox.Width, this.HitBox.Height);
                    }
                    break;
                case Enums.Direction.DOWN:
                    if (obj.HitBox.Right > this.HitBox.Left && obj.HitBox.Left < this.HitBox.Right && obj.HitBox.Top >= this.HitBox.Bottom)
                    {
                        this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                    }
                    else if (_spreadOffset == 0)
                    {
                        this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                    }
                    else if (_spreadOffset < 0 || obj.HitBox.Left < this.HitBox.Left)
                    {
                        int widthDiff = obj.HitBox.Right - this.HitBox.Left;
                        this.HitBox = new Rectangle(obj.HitBox.Right + 1, this.HitBox.Y + widthDiff, this.HitBox.Width, this.HitBox.Height);
                    }
                    else
                    {
                        int widthDiff = obj.HitBox.Left - this.HitBox.Right;
                        this.HitBox = new Rectangle(obj.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y + widthDiff, this.HitBox.Width, this.HitBox.Height);
                    }
                    break;
            }
        }
        private int _damage;
        private int _velocity;
        private int _pierce;
        private int _spread;
        private int _spreadOffset;
        private bool _spreadApplied;
        private int _blastRadius;
        protected Image _sprite;
        private int _refireDelay;

        protected Image[] _shotSprites;
        protected Image[] _shotCompleteSprites;

        private int _currentCompleteSprite = 0;
        private int _currentShootSprite = 0;
        private const int UPDATE_SPRITE_DELAY = 1;
        private int _currentSpriteDelay = 0;

        protected bool _shotComplete;
        protected bool _deflected;
        private Point _location;

        public event EventHandler ObjectComplete;

        protected void OnObjectComplete()
        {
            if (ObjectComplete != null)
                ObjectComplete(this, null);
        }

        public bool KillsKeen
        {
            get
            {
                return false;
            }
        }

        public int Damage
        {
            get { return _damage; }

        }

        public int Velocity
        {
            get { return _velocity; }

        }

        public int Pierce
        {
            get { return _pierce; }

        }

        public int Spread
        {
            get { return _spread; }
        }

        public int BlastRadius
        {
            get { return _blastRadius; }

        }

        public virtual void Stop()
        {
            _shotComplete = true;
            UpdateSprite();
        }

        protected virtual void UpdateSprite()
        {
            if (_currentSpriteDelay == UPDATE_SPRITE_DELAY)
            {
                if (!_shotComplete)
                {
                    if (_currentShootSprite < _shotSprites.Length - 1)
                    {
                        _currentShootSprite++;
                    }
                    else
                    {
                        _currentShootSprite = 0;
                    }
                    _sprite = _shotSprites[_currentShootSprite];
                }
                else if (_currentCompleteSprite <= _shotCompleteSprites.Length - 1)
                {
                    _sprite = _shotCompleteSprites[_currentCompleteSprite++];
                }
                else
                {
                    _sprite = null;
                    if (_collidingNodes != null)
                    {
                        foreach (var node in _collidingNodes)
                        {
                            node.Objects.Remove(this);
                            node.NonEnemies.Remove(this);
                        }
                    }
                    OnObjectComplete();
                }

                _currentSpriteDelay = 0;
            }
            else
            {
                _currentSpriteDelay++;
            }
        }

        protected virtual void InitializeSprites()
        {

            _shotSprites = new Image[]
            {
                Properties.Resources.keen_stun_shot1,
                Properties.Resources.keen_stun_shot2,
                Properties.Resources.keen_stun_shot3,
                Properties.Resources.keen_stun_shot4
            };

            _shotCompleteSprites = new Image[]
            {
                Properties.Resources.keen_stun_shot_hit1,
                Properties.Resources.keen_stun_shot_hit2
            };

            _sprite = _shotSprites[_currentShootSprite];
        }

        public Enums.MoveState MoveState
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
            get;
            set;
        }

        public virtual void Move()
        {
            int x = this.HitBox.X;
            int y = this.HitBox.Y;
            Rectangle newLocation = new Rectangle(x, y, this.HitBox.Width, this.HitBox.Height);

            switch (this.Direction)
            {
                case Enums.Direction.LEFT:
                    x -= _velocity;
                    newLocation = new Rectangle(x, y + _spreadOffset, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    x += _velocity;
                    newLocation = new Rectangle(x, y + _spreadOffset, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    y -= _velocity;
                    newLocation = new Rectangle(x + _spreadOffset, y, this.HitBox.Width, this.HitBox.Height);
                    break;
                case Enums.Direction.DOWN:
                    y += _velocity;
                    newLocation = new Rectangle(x + _spreadOffset, y, this.HitBox.Width, this.HitBox.Height);
                    break;
            }

            this.HitBox = newLocation;
            UpdateSprite();
            this.UpdateCollisionNodes(this.Direction);
        }

        void IMoveable.Stop()
        {
            throw new NotImplementedException();
        }

        Enums.MoveState IMoveable.MoveState
        {
            get;
            set;
        }

        Enums.Direction IMoveable.Direction
        {
            get;
            set;
        }

        private void GetRandomSpreadOffset()
        {
            this.ResetRandomVariable();
            _spreadOffset = _random.Next(_spread * -1, _spread + 1);
        }

        public virtual void Update()
        {
            if (!_shotComplete)
            {
                if (!_spreadApplied)
                {
                    GetRandomSpreadOffset();
                    _spreadApplied = true;
                }
                var areaToCheck = GetAreaToCheckForCollision();
                var collisionObjects = this.CheckCollision(areaToCheck);
                var debugTiles = collisionObjects.Where(c => c.CollisionType == CollisionType.BLOCK);
                var enemies = collisionObjects.OfType<IEnemy>().Where(i => i.IsActive).ToList();
                var cancellables = collisionObjects.OfType<ICancellableProjectile>().OfType<CollisionObject>().ToList();
                var itemsToCheck = new List<CollisionObject>();
                itemsToCheck.AddRange(debugTiles);
                itemsToCheck.AddRange(cancellables);
                if (_deflected)
                {
                    var keens = collisionObjects.OfType<CommanderKeen>();
                    if (keens.Any())
                    {
                        foreach (var keen in keens)
                        {
                            if (!_hitKeens.Contains(keen))
                            {
                                itemsToCheck.Add(keen);

                            }
                        }
                    }

                }
                foreach (var enemy in enemies)
                {
                    if (enemy is IEnemy)
                    {
                        var item = enemy as CollisionObject;
                        if (item != null && !_hitObjects.Contains(enemy))
                            itemsToCheck.Add(item);
                    }
                }
                if (itemsToCheck.Any())
                {
                    HandleCollisionByDirection(collisionObjects);
                }
                else
                {
                    this.Move();
                }
            }
            else
            {
                UpdateSprite();
            }
        }

        private void HandleCollisionByDirection(IEnumerable<CollisionObject> collisions)
        {
            switch (Direction)
            {
                case Enums.Direction.DOWN:
                    collisions = collisions.OrderBy(c => c.HitBox.Top).ToList();
                    break;
                case Enums.Direction.UP:
                    collisions = collisions.OrderByDescending(c => c.HitBox.Bottom).ToList();
                    break;
                case Enums.Direction.LEFT:
                    collisions = collisions.OrderByDescending(c => c.HitBox.Right).ToList();
                    break;
                case Enums.Direction.RIGHT:
                    collisions = collisions.OrderBy(c => c.HitBox.Left).ToList();
                    break;
            }
            bool handledDebugTileCollision = false;
            foreach (var collision in collisions)
            {
                bool handle = !_shotComplete && ((!handledDebugTileCollision && collision.CollisionType == CollisionType.BLOCK) || collision.CollisionType != CollisionType.BLOCK);
                if (handle)
                {
                    if (collision.CollisionType == CollisionType.BLOCK)
                        handledDebugTileCollision = true;
                    this.HandleCollision(collision);
                }
            }
            UpdateCollisionNodes(this.Direction);
        }

        private Rectangle GetAreaToCheckForCollision()
        {
            int x = this.HitBox.X;
            int y = this.HitBox.Y;
            Rectangle areaToCheck = new Rectangle(x, y, this.HitBox.Width, this.HitBox.Height);

            switch (this.Direction)
            {
                case Enums.Direction.LEFT:
                    areaToCheck = new Rectangle(x - _velocity, y + _spreadOffset, this.HitBox.Width + _velocity, this.HitBox.Height);
                    break;
                case Enums.Direction.RIGHT:
                    areaToCheck = new Rectangle(x, y + _spreadOffset, this.HitBox.Width + _velocity, this.HitBox.Height);
                    break;
                case Enums.Direction.UP:
                    areaToCheck = new Rectangle(x + _spreadOffset, y - _velocity, this.HitBox.Width, this.HitBox.Height + _velocity);
                    break;
                case Enums.Direction.DOWN:
                    areaToCheck = new Rectangle(x + _spreadOffset, y, this.HitBox.Width, this.HitBox.Height + _velocity);
                    break;
            }

            return areaToCheck;
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
                _location = value.Location;
            }
        }

        public int RefireDelay
        {
            get { return _refireDelay; }
        }

        public override CollisionType CollisionType => CollisionType.FRIENDLY_PROJECTILE;

        public int ZIndex => 200;

        public Image Image => _sprite;

        public Point Location => _location;
    }
}
