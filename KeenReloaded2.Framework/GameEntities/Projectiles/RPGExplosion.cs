using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
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
    public class RPGExplosion : CollisionObject, IUpdatable, IExplodable, ISprite, ICreateRemove
    {
        protected Image _sprite;
        protected int _blastRadius;
        protected Enums.ExplosionState _state;
        protected int SPRITE_CHANGE_DELAY = 0;
        protected int _currentSpriteChangeDelayTick;
        protected int _blastRadiusIncreaseAmount;
        protected int _currentBlastRadiusAmount;
        protected int _damage;

        protected CollisionObject _leftTile;
        protected CollisionObject _rightTile;
        protected CollisionObject _upTile;
        protected CollisionObject _downTile;

        protected Image[] _explosionImages = SpriteSheet.SpriteSheet.RPGExplosionSprites;

        protected int _currentImage = 0;
        protected Rectangle _initialExplosionArea;
        private readonly Direction _direction;

        public RPGExplosion(SpaceHashGrid grid, Rectangle hitbox, int blastRadius, int damage, Direction direction)
            : base(grid, hitbox)
        {
            _blastRadius = blastRadius;
            _damage = damage;
            _blastRadiusIncreaseAmount =  _explosionImages.Length == 0 ? _blastRadius : (_blastRadius / _explosionImages.Length);
            _currentBlastRadiusAmount = _blastRadiusIncreaseAmount;
            _initialExplosionArea = new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height);
            _direction = direction;
            Initialize();
        }

        private void Initialize()
        {
            _sprite = _explosionImages[_currentImage];
            this.Explode();
        }

        protected virtual void HandleCollision(CollisionObject obj)
        {
            var destructoObject = obj as DestructibleObject;
            if (destructoObject != null && !destructoObject.IsDead())
            {
                destructoObject.TakeDamage(_damage);
            }
            else if (obj is IStunnable)
            {
                var stunnableObject = obj as IStunnable;
                stunnableObject.Stun();
            }
            else if (obj is IAlertable)
            {
                var alertable = (IAlertable)obj;
                if (!alertable.IsOnAlert)
                    alertable.Alert();
            }
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var tiles = collisions.Where(c => 
                c.CollisionType == CollisionType.BLOCK && c.HitBox.Bottom < this.HitBox.Bottom 
             && c.HitBox.Top < this.HitBox.Top
             && c.HitBox.Right > this.HitBox.Left
             && c.HitBox.Left < this.HitBox.Right).ToList();
            if (tiles.Any())
            {
                int maxBottom = tiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        private void ExplodeLeft()
        {
            int height = this.HitBox.Height;
            int width = this.HitBox.Width;

            int xPos = this.HitBox.X;
            int yPos = _initialExplosionArea.Y - (this.HitBox.Height / 2) + (_initialExplosionArea.Width / 2); 

            Rectangle areaToCheck = new Rectangle(xPos, yPos, width, height);
            var collisions = this.CheckCollision(areaToCheck);
            var barriers = collisions
                .Where(c => c.CollisionType == CollisionType.BLOCK
                ).ToList();

            var leftTile = this.GetRightMostLeftTile(barriers);
            if (leftTile != null && leftTile.HitBox.Top < _initialExplosionArea.Bottom && leftTile.HitBox.Bottom > _initialExplosionArea.Top)
            {
                xPos = leftTile.HitBox.Right + 1;
                width = this.HitBox.Right - xPos;
                if (width < 1)
                    width = 1;
            }

            var upTile = this.GetCeilingTile(barriers);
            if (upTile != null)
            {
                yPos = upTile.HitBox.Bottom + 1;
            }

            var downTile = this.GetTopMostLandingTile(barriers);
            if (downTile != null)
            {
                height = downTile.HitBox.Top - yPos - 1;
                if (height < 1)
                    height = 1;
            }

            this.HitBox = new Rectangle(xPos, yPos, width, height);
            HandleCollisions(collisions);
        }

        private void ExplodeRight()
        {
            int height = this.HitBox.Height;
            int width = this.HitBox.Width;

            int xPos = this.HitBox.X;
            int yPos = _initialExplosionArea.Y - this.HitBox.Height / 2;

            Rectangle areaToCheck = new Rectangle(xPos, yPos, width, height);
            var collisions = this.CheckCollision(areaToCheck);
            var barriers = collisions.Where(c => c.CollisionType == CollisionType.BLOCK 
                && c.HitBox.Top <= _initialExplosionArea.Bottom).ToList();

            var rightTile = this.GetLeftMostRightTile(barriers);
            if (rightTile != null && rightTile.HitBox.Top < _initialExplosionArea.Bottom && rightTile.HitBox.Bottom > _initialExplosionArea.Top)
            {
                width = this.HitBox.Left - xPos - 1;
                if (width < 1)
                    width = 1;
            }

            var upTile = this.GetCeilingTile(barriers);
            if (upTile != null)
            {
                yPos = upTile.HitBox.Bottom + 1;
            }

            var downTile = this.GetTopMostLandingTile(barriers);
            if (downTile != null)
            {
                height = downTile.HitBox.Top - yPos - 1;
                if (height < 1)
                    height = 1;
            }

            this.HitBox = new Rectangle(xPos, yPos, width, height);
            HandleCollisions(collisions);
        }

        private void ExplodeUp()
        {
            int height = this.HitBox.Height;
            int width = this.HitBox.Width;

            int xPos = _initialExplosionArea.X - (this.HitBox.Width / 2) + (_initialExplosionArea.Width / 2);
            int yPos = this.HitBox.Y;

            Rectangle areaToCheck = new Rectangle(xPos, yPos, width, height - (this.HitBox.Bottom - _initialExplosionArea.Bottom));
            var collisions = this.CheckCollision(areaToCheck);
            var barriers = collisions.Where(c => c.CollisionType == CollisionType.BLOCK).ToList();

            var leftTile = this.GetRightMostLeftTile(barriers);
            if (leftTile != null && leftTile.HitBox.Top < this.HitBox.Bottom && leftTile.HitBox.Bottom > this.HitBox.Top)
            {
                xPos = leftTile.HitBox.Right + 1;
            }

            var rightTile = this.GetLeftMostRightTile(barriers);
            if (rightTile != null && rightTile.HitBox.Top < this.HitBox.Bottom && rightTile.HitBox.Bottom > this.HitBox.Top)
            {
                width = this.HitBox.Left - xPos - 1;
                if (width < 1)
                    width = 1;
            }

            var upTile = this.GetCeilingTile(barriers);
            if (upTile != null)
            {
                yPos = upTile.HitBox.Bottom + 1;
                height = this.HitBox.Bottom - yPos;
                if (height < 1)
                    height = 1;
            }

            this.HitBox = new Rectangle(xPos, yPos, width, height);
            HandleCollisions(collisions);
        }

        private void ExplodeDown()
        {
            int height = this.HitBox.Height;
            int width = this.HitBox.Width;

            int xPos = _initialExplosionArea.X - (this.HitBox.Width / 2) + (_initialExplosionArea.Width / 2);
            int yPos = _initialExplosionArea.Y;

            Rectangle areaToCheck = new Rectangle(xPos, yPos, width, height);
            var collisions = this.CheckCollision(areaToCheck);
            var barriers = collisions.Where(c => c.CollisionType == CollisionType.BLOCK).ToList();

            var leftTile = this.GetRightMostLeftTile(barriers);
            if (leftTile != null && leftTile.HitBox.Top < this.HitBox.Bottom && leftTile.HitBox.Bottom > this.HitBox.Top)
            {
                xPos = leftTile.HitBox.Right + 1;
            }

            var rightTile = this.GetLeftMostRightTile(barriers);
            if (rightTile != null && rightTile.HitBox.Top < this.HitBox.Bottom && rightTile.HitBox.Bottom > this.HitBox.Top)
            {
                width = this.HitBox.Left - xPos - 1;
                if (width < 1)
                    width = 1;
            }

            var downTile = this.GetTopMostLandingTile(barriers);
            if (downTile != null)
            {
                height = downTile.HitBox.Top - yPos - 1;
                if (height < 1)
                    height = 1;
            }

            this.HitBox = new Rectangle(xPos, yPos, width, height);
            HandleCollisions(collisions);
        }

        public virtual void Explode()
        {
            if (this.ExplosionState != Enums.ExplosionState.EXPLODING)
            {
                this.ExplosionState = Enums.ExplosionState.EXPLODING;
            }

            if (_direction == Direction.RIGHT)
            {
                this.ExplodeRight();
                int x = this.HitBox.X;
                int y = this.HitBox.Y - _currentBlastRadiusAmount + _initialExplosionArea.Width / 2;
                int width = this.HitBox.Width + (_currentBlastRadiusAmount * 2);
                int height = this.HitBox.Height + (_currentBlastRadiusAmount * 2);
                this.HitBox = new Rectangle(x, y, width, height);
                UpdateSprite();
            }
            else if (_direction == Direction.LEFT)
            {
                this.ExplodeLeft();
                int x = this.HitBox.X - _currentBlastRadiusAmount * 2;
                int y = this.HitBox.Y - _currentBlastRadiusAmount;
                int width = this.HitBox.Width + (_currentBlastRadiusAmount * 2);
                int height = this.HitBox.Height + (_currentBlastRadiusAmount * 2);
                this.HitBox = new Rectangle(x, y, width, height);
                UpdateSprite();
            }
            else if (_direction == Direction.DOWN)
            {
                this.ExplodeDown();
                int x = this.HitBox.X - _currentBlastRadiusAmount;
                int y = this.HitBox.Y;
                int width = this.HitBox.Width + (_currentBlastRadiusAmount * 2);
                int height = this.HitBox.Height + (_currentBlastRadiusAmount);
                this.HitBox = new Rectangle(x, y, width, height);
                UpdateSprite();
            }
            else if (_direction == Direction.UP)
            {
                this.ExplodeUp();

                int x = this.HitBox.X - _currentBlastRadiusAmount + _initialExplosionArea.Width / 2;
                int y = this.HitBox.Y - _currentBlastRadiusAmount;
                int width = this.HitBox.Width + (_currentBlastRadiusAmount * 2);
                int height = this.HitBox.Height + (_currentBlastRadiusAmount);
                this.HitBox = new Rectangle(x, y, width, height);
                UpdateSprite();
            }


            if (_currentBlastRadiusAmount + _blastRadiusIncreaseAmount <= _blastRadius)
            {
                _currentBlastRadiusAmount += _blastRadiusIncreaseAmount;
            }
            else
            {
                _currentBlastRadiusAmount = _blastRadius;
            }

            if (_currentBlastRadiusAmount == _blastRadius)
            {
                this.EndExplosion();
            }
        }

        private void HandleCollisions(List<CollisionObject> collisions)
        {
            var destructoObjectsToKill = collisions.OfType<DestructibleObject>();
            var stunnableObjects = collisions.OfType<IStunnable>();

            if (destructoObjectsToKill.Any())
            {
                destructoObjectsToKill = destructoObjectsToKill.Where(o => o.HitBox.IntersectsWith(this.HitBox));
                if (destructoObjectsToKill.Any())
                {
                    foreach (var obj in destructoObjectsToKill)
                    {
                        this.HandleCollision(obj);
                    }
                }
            }
            if (stunnableObjects.Any())
            {
                foreach (var obj in stunnableObjects)
                {
                    var collideObj = obj as CollisionObject;
                    if (collideObj != null && collideObj.HitBox.IntersectsWith(this.HitBox))
                    {
                        this.HandleCollision(collideObj);
                    }
                }
            }
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile = null;
            var landingTiles = collisions.Where(h => (h.CollisionType == CollisionType.BLOCK)
                && h.HitBox.Top >= this.HitBox.Top);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        protected virtual void EndExplosion()
        {
            if (this.ExplosionState != Enums.ExplosionState.DONE)
            {
                this.ExplosionState = Enums.ExplosionState.DONE;
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
                if (_collidingNodes != null && _collidingNodes.Any())
                {
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        public void Update()
        {
            switch (_state)
            {
                case Enums.ExplosionState.EXPLODING:
                    if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
                    {
                        _currentSpriteChangeDelayTick = 0;
                        this.Explode();
                    }
                    break;
                case Enums.ExplosionState.CLEARIING:
                    this.UpdateSprite();
                    break;
                case Enums.ExplosionState.DONE:
                    OnRemove();
                    break;
            }
        }


        public ExplosionState ExplosionState
        {
            get { return _state; }
            protected set
            {
                _state = value;
                //UpdateSprite();
            }
        }

        public int ZIndex => 500;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.EXPLOSION;

        protected virtual void UpdateSprite()
        {
            if (++_currentImage >= _explosionImages.Length)
            {
                _currentImage = _explosionImages.Length - 1;
            }
            _sprite = BitMapTool.DrawImageAtLocationWithDimensions(_explosionImages[_currentImage], this.HitBox);
        }

        protected virtual void OnCreate()
        {
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = this
            };
            if (this.Create != null)
            {
                this.Create(this, e);
            }
        }
        protected virtual void OnRemove()
        {
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = this
            };

            foreach (var node in _collidingNodes)
            {
                node.Objects.Remove(this);
                node.NonEnemies.Remove(this);
            }

            if (this.Remove != null)
            {
                this.Remove(this, e);
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

    }
}
