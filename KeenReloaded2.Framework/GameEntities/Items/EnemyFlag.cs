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

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class EnemyFlag : Item, IFlag
    {
        #region constants
        private const int POINT_DEGRADATION_DELAY = 15;
        private const int FALL_VELOCITY = 50;
        private readonly CommanderKeen _keen;
        #endregion

        #region fields
        private int _pointsDegradedPerSecond;
        private int _pointDegradationDelayTick;
        private bool _isCaptured;
        private bool _executeGravity;
        private Point _origin;
        private IEnemy _enemyPossessingFlag;
        private Rectangle _enemyLocation;
        #endregion

        public EnemyFlag(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageName, int zIndex, CommanderKeen keen, int pointsDegradedPerSecond) 
            : base(area, imageName, grid, zIndex)
        {
            _pointsDegradedPerSecond = pointsDegradedPerSecond;
            _origin = new Point(hitbox.X, hitbox.Y);
            _collectable = false;
            _keen = keen;
            if (_keen == null)
                throw new ArgumentNullException("keen cannot be null");

            InitializeSprite();
        }

        public override Rectangle HitBox
        {
            get { return base.HitBox; }
            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public int PointsDegradedPerSecond => _pointsDegradedPerSecond;

        public bool IsCaptured => _isCaptured;

        public Point LocationOfOrigin => _origin;

        public override CollisionType CollisionType => CollisionType.ITEM;

        public event EventHandler<FlagCapturedEventArgs> FlagCaptured;
        public event EventHandler<FlagCapturedEventArgs> FlagPointsChanged;

        public void Capture()
        {
            _isCaptured = true;
            _executeGravity = true;
            this.UpdatePositionFromCapturingEnemy();
            OnCapture();
        }

        private void Drop()
        {
            _isCaptured = false;
            var tileCollisions = this.CheckCollision(this.HitBox, true, false);

            if (tileCollisions.Any())
                this.HitBox = new Rectangle(_enemyLocation.X, _enemyLocation.Y, this.HitBox.Width, this.HitBox.Height);

            _enemyPossessingFlag = null;
            _pointDegradationDelayTick = 0;
        }

        private void SubtractPointValue()
        {
            long currentScore = _keen.Points;
            if (currentScore == 0 || _keen.IsDead())
                return;

            if (currentScore < _pointsDegradedPerSecond)
            {
                _keen.GiveFlagPoints(currentScore * -1);
            }
            else
            {
                _keen.GiveFlagPoints(_pointsDegradedPerSecond * -1);
            }
            OnPointsDegraded();
        }

        public override void Update()
        {
            base.Update();
            if (_isCaptured)
            {
                this.UpdatePositionFromCapturingEnemy();

                if (_enemyPossessingFlag == null)
                    return;

                if (!_enemyPossessingFlag.IsActive)
                {
                    this.Drop();
                }
                else if (++_pointDegradationDelayTick >= POINT_DEGRADATION_DELAY)
                {
                    _pointDegradationDelayTick = 0;
                    SubtractPointValue();
                    this.UpdatePositionFromCapturingEnemy();
                }
                else
                {
                    this.UpdatePositionFromCapturingEnemy();
                }
            }
            else
            {
                _enemyPossessingFlag = GetClosestCollidingEnemy();
                if (_enemyPossessingFlag != null)
                {
                    this.Capture();
                }
                else if (_executeGravity)
                {
                    this.BasicFall(FALL_VELOCITY);
                    if (this.HitBox.Y > _collisionGrid.Size.Height)
                    {
                        ResetFlagToOriginState();
                    }
                }
            }
        }

        private void ResetFlagToOriginState()
        {
            _executeGravity = false;
            _isCaptured = false;
            this.HitBox = new Rectangle(_origin.X, _origin.Y, this.HitBox.Width, this.HitBox.Height);
        }

        private void UpdatePositionFromCapturingEnemy()
        {
            if (_enemyPossessingFlag != null)
            {
                UpdateLocationFromEnemy(_enemyPossessingFlag);
                this.HitBox = new Rectangle(_enemyLocation.X, _enemyLocation.Y - this.HitBox.Height - 8, this.HitBox.Width, this.HitBox.Height);
                if (this.HitBox.Y > _collisionGrid.Size.Height)
                {
                    Drop();
                    ResetFlagToOriginState();
                }
            }
        }

        private IEnemy GetClosestCollidingEnemy()
        {
            IEnemy enemy = null;
            var collisionItems = this.CheckCollision(this.HitBox);
            var enemies = collisionItems.OfType<IEnemy>();
            if (enemies.Any())
            {
                enemy = enemies.FirstOrDefault(e => e.IsActive && !(e is EnemySpawner) && ((e is DestructibleObject) || (e is IExplodable) || (e is ISquashable)));
            }

            UpdateLocationFromEnemy(enemy);

            return enemy;
        }

        private void UpdateLocationFromEnemy(IEnemy enemy)
        {
            if (enemy != null && enemy is CollisionObject)
            {
                var collisionObj = (CollisionObject)enemy;
                _enemyLocation = collisionObj.HitBox;
            }
        }

        protected virtual void OnCapture()
        {
            FlagCapturedEventArgs e = new FlagCapturedEventArgs()
            {
                EnemyFlag = this
            };
            this.FlagCaptured?.Invoke(this, e);
        }

        protected void OnPointsDegraded()
        {
            FlagCapturedEventArgs e = new FlagCapturedEventArgs()
            {
                EnemyFlag = this
            };
            this.FlagPointsChanged?.Invoke(this, e);

        }

        private void InitializeSprite()
        {
            var colorImages = SpriteSheet.SpriteSheet.CTFColors;
            this.SpriteList = new Image[] { Properties.Resources.Black_Flag };
            this.AcquiredSpriteList = new Image[] { Properties.Resources.Flag_Acquired };
            _sprite = this.SpriteList[0];
        }
    }
}
