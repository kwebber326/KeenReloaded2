using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Hazards;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class BFGExplosion : RPGExplosion
    {
        private BFGProjectile _callingObject;

        private readonly int _initalLeft, _initialRight, _initialUp, _initialDown, _initialWidth, _initialHeight;
        private Direction _direction;
        private bool _explosionbreadthComplete;

        public BFGExplosion(SpaceHashGrid grid, Rectangle hitbox, int blastRadius, int damage, BFGProjectile callingObject) : base(grid, hitbox, blastRadius, damage)
        {
            _callingObject = callingObject;
            if (_callingObject == null)
            {
                throw new ArgumentNullException("must pass projectile object that caused this explosion");
            }
            _initalLeft = _callingObject.HitBox.Left;
            _initialRight = _callingObject.HitBox.Right;
            _initialUp = _callingObject.HitBox.Top;
            _initialWidth = _initialRight - _initalLeft;
            _initialHeight = _initialDown - _initialUp;
            _initialDown = _callingObject.HitBox.Bottom;
            _direction = _callingObject.Direction;
            _blastRadiusIncreaseAmount = 30;
        }

        public override void Explode()
        {

            if (this.ExplosionState != Enums.ExplosionState.EXPLODING)
            {
                this.ExplosionState = Enums.ExplosionState.EXPLODING;
            }


            if (this.IsHorizontalDirection(_direction))
            {
                ExplodeHorizontally();
            }
            else
            {
                ExplodeVertically();
            }
        }

        private void ExplodeVertically()
        {
            //this should kill even keen if in blast radius
            List<DestructibleObject> destructoObjects = new List<DestructibleObject>();
            //tile collisions should limit explostion length
            List<CollisionObject> tiles = new List<CollisionObject>();
            //we need to continue expanding in other directions even if one direction finds a wall collisions
            //this means we store when we hit in a certain direction;
            int xPos = _leftTile != null ? _leftTile.HitBox.Right + 1 : this.HitBox.X - _blastRadiusIncreaseAmount;
            int yPos = _upTile != null ? _upTile.HitBox.Bottom + 1 : _initialDown - (this.HitBox.Y - _blastRadiusIncreaseAmount) > _blastRadius * 2 ? _initialDown - (_blastRadius * 2) : this.HitBox.Y - _blastRadiusIncreaseAmount;
            int width = _rightTile != null ? _rightTile.HitBox.Left - 1 - xPos : this.HitBox.Width + _blastRadiusIncreaseAmount * 2;
            int height = _downTile != null ? (_downTile.HitBox.Top - 1 - yPos) > _blastRadius * 2 ? _blastRadius * 2 : (_downTile.HitBox.Top - 1 - yPos)
                : (this.HitBox.Height + _blastRadiusIncreaseAmount * 2) > _blastRadius * 2 ? _blastRadius * 2 : (this.HitBox.Height + _blastRadiusIncreaseAmount * 2);

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
            if (_upTile == null && _rightTile == null && _leftTile == null)
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
                var downTile = GetTopMostLandingTile(collisions);

                if (downTile != null && _rightTile == null && _leftTile == null)
                {
                    downPos = downTile.HitBox.Top - 1;
                    _downTile = downTile;
                }
            }
            this.HitBox = new Rectangle(leftPos, upPos, rightPos - leftPos, downPos - upPos);
            HandleDestructibleObjectCollisions(collisions);

            UpdateSprite();

            if ((_leftTile != null || this.HitBox.Left <= 0) && (_rightTile != null || this.HitBox.Right >= _collisionGrid.Size.Width))
            {
                _explosionbreadthComplete = true;
            }

            if (_currentBlastRadiusAmount + _blastRadiusIncreaseAmount <= _blastRadius)
            {
                _currentBlastRadiusAmount += _blastRadiusIncreaseAmount;
            }
            else
            {
                _currentBlastRadiusAmount = _blastRadius;
            }

            if (_currentBlastRadiusAmount == _blastRadius && _explosionbreadthComplete)
            {
                this.EndExplosion();
            }
        }

        private void ExplodeHorizontally()
        {
            //this should kill even keen if in blast radius
            List<DestructibleObject> destructoObjects = new List<DestructibleObject>();
            //tile collisions should limit explostion length
            List<CollisionObject> tiles = new List<CollisionObject>();
            //we need to continue expanding in other directions even if one direction finds a wall collisions
            //this means we store when we hit in a certain direction;
            int xPos = _leftTile != null ? _leftTile.HitBox.Right + 1 : _initialRight - (this.HitBox.X - _blastRadiusIncreaseAmount) > _blastRadius * 2 ? _initialRight - (_blastRadius * 2) : this.HitBox.X - _blastRadiusIncreaseAmount;
            int yPos = _upTile != null ? _upTile.HitBox.Bottom + 1 : this.HitBox.Y - _blastRadiusIncreaseAmount;
            int width = _rightTile != null ? (_rightTile.HitBox.Left - 1 - xPos) > _blastRadius * 2 ? _blastRadius * 2 : (_rightTile.HitBox.Left - 1 - xPos)
                : (this.HitBox.Width + _blastRadiusIncreaseAmount * 2) > _blastRadius * 2 ? _blastRadius * 2 : (this.HitBox.Width + _blastRadiusIncreaseAmount * 2);
            int height = _downTile != null ? _downTile.HitBox.Top - 1 - yPos : this.HitBox.Height + _blastRadiusIncreaseAmount * 2;

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
                var downTile = GetTopMostLandingTile(collisions);

                if (downTile != null)
                {
                    downPos = downTile.HitBox.Top - 1;
                    _downTile = downTile;
                }
            }
            this.HitBox = new Rectangle(leftPos, upPos, rightPos - leftPos, downPos - upPos);
            HandleDestructibleObjectCollisions(collisions);

            UpdateSprite();

            if ((_upTile != null || this.HitBox.Top <= 0) && (_downTile != null || this.HitBox.Bottom >= _collisionGrid.Size.Height))
            {
                _explosionbreadthComplete = true;
            }

            if (_currentBlastRadiusAmount + _blastRadiusIncreaseAmount <= _blastRadius)
            {
                _currentBlastRadiusAmount += _blastRadiusIncreaseAmount;
            }
            else
            {
                _currentBlastRadiusAmount = _blastRadius;
            }

            if (_currentBlastRadiusAmount == _blastRadius && _explosionbreadthComplete)
            {
                this.EndExplosion();
            }
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile;
            var landingTiles = collisions.Where(h => (h.CollisionType == Enums.CollisionType.BLOCK)
                && h.HitBox.Top >= this.HitBox.Top);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        private void HandleDestructibleObjectCollisions(List<CollisionObject> collisions)
        {
            var destructoObjectsToKill = collisions.OfType<DestructibleObject>();
            var stunnableObjects = collisions.OfType<IStunnable>();
            var explodableObjects = collisions.OfType<IExplodable>();

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
            if (explodableObjects.Any())
            {
                foreach (var obj in explodableObjects)
                {
                    var collideObj = obj as CollisionObject;
                    if (collideObj != null && collideObj.HitBox.IntersectsWith(this.HitBox))
                    {
                        this.HandleCollision(collideObj);
                    }
                }
            }
        }

        protected void HandleCollision(CollisionObject obj)
        {
            var destructoObject = obj as DestructibleObject;
            if (destructoObject != null && !destructoObject.IsDead() && !(destructoObject.CollisionType == Enums.CollisionType.PLAYER))
            {
                destructoObject.TakeDamage(_damage);
            }
            else if (obj is IStunnable)
            {
                var stunnableObject = obj as IStunnable;
                stunnableObject.Stun();
            }
            else if (obj is IExplodable && ((obj is IEnemy) || (obj is Hazard)))
            {
                var explodableObject = obj as IExplodable;
                explodableObject.Explode();
            }
        }

        public new event EventHandler<ObjectEventArgs> Create;

        public new event EventHandler<ObjectEventArgs> Remove;

        public int InitialLeft
        {
            get
            {
                return _initalLeft;
            }
        }

        public int InitialRight
        {
            get
            {
                return _initialRight;
            }
        }

        public int InitialUp
        {
            get
            {
                return _initialUp;
            }
        }

        public int InitialDown
        {
            get
            {
                return _initialDown;
            }
        }

        protected void OnCreate(object sender, ObjectEventArgs e)
        {
            this.Create?.Invoke(this, e);
        }
        protected void OnRemove(object sender, ObjectEventArgs e)
        {
            this.Remove?.Invoke(this, e);
        }

        protected override void OnCreate()
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
        protected override void OnRemove()
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
    }
}
