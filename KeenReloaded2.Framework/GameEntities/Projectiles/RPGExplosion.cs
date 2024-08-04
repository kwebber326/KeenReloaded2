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

        public RPGExplosion(SpaceHashGrid grid, Rectangle hitbox, int blastRadius, int damage)
            : base(grid, hitbox)
        {
            _blastRadius = blastRadius;
            _damage = damage;
            _blastRadiusIncreaseAmount = _blastRadius / _explosionImages.Length == 0 ? _blastRadius : _explosionImages.Length;
            _currentBlastRadiusAmount = _blastRadiusIncreaseAmount;
            Initialize();
        }

        private void Initialize()
        {
            _sprite = _explosionImages[_currentImage];
            this.Explode();
        }

        protected void HandleCollision(CollisionObject obj)
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
        }

        public virtual void Explode()
        {
            if (this.ExplosionState != Enums.ExplosionState.EXPLODING)
            {
                this.ExplosionState = Enums.ExplosionState.EXPLODING;
            }
            //this should kill even keen if in blast radius
            List<DestructibleObject> destructoObjects = new List<DestructibleObject>();
            //tile collisions should limit explostion length
            List<CollisionObject> tiles = new List<CollisionObject>();

            //we need to continue expanding in other directions even if one direction finds a wall collisions
            //this means we store when we hit in a certain direction;
            int xPos = _leftTile != null ? _leftTile.HitBox.Right + 1 : this.HitBox.X - _currentBlastRadiusAmount;
            int yPos = _upTile != null ? _upTile.HitBox.Bottom + 1 : this.HitBox.Y - _currentBlastRadiusAmount;
            int width = _rightTile != null ? _rightTile.HitBox.Left - 1 - xPos : this.HitBox.Width + _currentBlastRadiusAmount * 2;
            int height = _downTile != null ? _downTile.HitBox.Top - 1 - yPos : this.HitBox.Height + _currentBlastRadiusAmount * 2;

            //area to check is the hitbox that represents a slightly
            Rectangle areaToCheck = new Rectangle(xPos, yPos, width, height);
            var collisions = this.CheckCollision(areaToCheck);
            //explode left
            int leftPos = areaToCheck.Left;
            if (_leftTile == null)//don't check again if we already hit a collision here
            {
                var leftTile = GetRightMostLeftTile(collisions);
                if (leftTile != null && leftTile.HitBox.Top < this.HitBox.Bottom && leftTile.HitBox.Bottom > this.HitBox.Top)
                {
                    leftPos = leftTile.HitBox.Right + 1;
                    _leftTile = leftTile;//store in memory when we hit something
                }
            }
            //explode right
            int rightPos = areaToCheck.Right;
            if (_rightTile == null)
            {
                var rightTile = GetLeftMostRightTile(collisions);

                if (rightTile != null && rightTile.HitBox.Top < this.HitBox.Bottom && rightTile.HitBox.Bottom > this.HitBox.Top)
                {
                    rightPos = rightTile.HitBox.Left - 1;
                    _rightTile = rightTile;
                }
            }
            //explode up
            int upPos = areaToCheck.Top;
            if (_upTile == null)
            {
                var upTile = GetCeilingTile(collisions);

                if (upTile != null)
                {
                    upPos = upTile.HitBox.Bottom + 1;
                    _upTile = upTile;
                }
            }
            //explode down
            int downPos = areaToCheck.Bottom;
            if (_downTile == null)
            {
                var downTile = GetTopMostLandingTile(_currentBlastRadiusAmount);

                if (downTile != null)
                {
                    downPos = downTile.HitBox.Top - 1;
                    _downTile = downTile;
                }
            }
            this.HitBox = new Rectangle(leftPos, upPos, rightPos - leftPos, downPos - upPos);
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

            UpdateSprite();

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
                UpdateSprite();
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
                _currentImage = 0;
            }
            _sprite = _explosionImages[_currentImage];
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
