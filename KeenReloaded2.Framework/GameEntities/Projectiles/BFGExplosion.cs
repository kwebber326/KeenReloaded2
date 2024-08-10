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

        public BFGExplosion(SpaceHashGrid grid, Rectangle hitbox, int blastRadius, int damage, BFGProjectile callingObject, Direction direction) : base(grid, hitbox, blastRadius, damage, direction)
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
                this.UpdateSprite();
            }

            this.HitBox = new Rectangle(this.HitBox.X - _blastRadiusIncreaseAmount, this.HitBox.Y - _blastRadiusIncreaseAmount, 
                this.HitBox.Width + _blastRadiusIncreaseAmount * 2, this.HitBox.Height + _blastRadiusIncreaseAmount * 2);

            this.UpdateSprite();

            var collisions = this.CheckCollision(this.HitBox);
            HandleDestructibleObjectCollisions(collisions);
            

            _explosionbreadthComplete = _sprite.Width / 2 >= _blastRadius;
            if (_explosionbreadthComplete)
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
            if (destructoObject != null && !destructoObject.IsDead() && (destructoObject.CollisionType != Enums.CollisionType.PLAYER))
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
