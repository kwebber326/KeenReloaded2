using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class ShikadiMasterEnergyBall : CollisionObject, IUpdatable, ISprite, IProjectile, ICreateRemove
    {
        private Image _sprite;
        private Direction _direction;
        private bool _shotHit;

        private const int GRAVITY_ACCELERATION = 5;
        private const int AIR_RESISTANCE = 5;
        private const int KINETIC_IMPACT_DENOMINATOR = 5;
        private const int MAX_VERTICAL_VELOCITY = 80;
        private const int MIN_HORIZONTAL_VELOCITY = 25;
        private int _currentHorizontalVelocity, _currentVerticalVelocity;

        private const int SPRITE_CHANGE_DELAY = 2;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;

        public ShikadiMasterEnergyBall(SpaceHashGrid grid, Rectangle hitbox, Direction direction)
            : base(grid, hitbox)
        {
            _direction = direction;
            Initialize();
        }

        private void Initialize()
        {
            _sprite = SpriteSheet.SpriteSheet.ShikadiMasterEnergyBallImages[_currentSprite];
            _currentHorizontalVelocity = _direction == Enums.Direction.LEFT ? this.Velocity * -1 : this.Velocity;
            var collisions = this.CheckCollision(this.HitBox, true);
            if (collisions.Any())
            {
                this.Stop();
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
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_RIGHT);
                }
            }
        }

        public void Update()
        {
            this.Move();
        }

        public int Damage
        {
            get { return int.MaxValue; }
        }

        public int Velocity
        {
            get { return 40; }
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
            get { return -1; }
        }

        public bool KillsKeen
        {
            get { return true; }
        }

        private void UpdateSpriteByDelay()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                if (++_currentSprite >= SpriteSheet.SpriteSheet.ShikadiMasterEnergyBallImages.Length)
                {
                    _currentSprite = 0;
                }
                _sprite = SpriteSheet.SpriteSheet.ShikadiMasterEnergyBallImages[_currentSprite];
            }
        }
        private void CreateShockWaves()
        {
            int width = 56, height = 14;
            int xPos1 = this.HitBox.X + width / 2, yPos1 = this.HitBox.Bottom - height;
            ShikadiMasterShockwave shockwave1 = new ShikadiMasterShockwave(_collisionGrid, new Rectangle(xPos1, yPos1, width, height), _direction);
            shockwave1.Create += new EventHandler<ObjectEventArgs>(shockwave_Create);
            shockwave1.Remove += new EventHandler<ObjectEventArgs>(shockwave_Remove);

            OnCreate(new ObjectEventArgs() { ObjectSprite = shockwave1 });

            //right wave
            int xPos2 = this.HitBox.Right + 1, yPos2 = this.HitBox.Bottom - height;
            Rectangle areaToCheck2 = new Rectangle(xPos2, yPos2, width, height);
            var collisions2 = this.CheckCollision(areaToCheck2, true);
            if (!collisions2.Any())
            {
                ShikadiMasterShockwave shockwave2 = new ShikadiMasterShockwave(_collisionGrid, new Rectangle(xPos2, yPos2, width, height), Enums.Direction.RIGHT);
                shockwave2.Create += new EventHandler<ObjectEventArgs>(shockwave_Create);
                shockwave2.Remove += new EventHandler<ObjectEventArgs>(shockwave_Remove);

                OnCreate(new ObjectEventArgs() { ObjectSprite = shockwave2 });
            }
        }

        void shockwave_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void shockwave_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        public void Move()
        {
            int velocity = _direction == Enums.Direction.LEFT ? _currentHorizontalVelocity * -1 : _currentHorizontalVelocity;
            Rectangle areaToCheck = this.Direction == Enums.Direction.LEFT ?
                new Rectangle(this.HitBox.Left + _currentHorizontalVelocity, _currentVerticalVelocity < 0 ? this.HitBox.Y + _currentVerticalVelocity : this.HitBox.Y, this.HitBox.Width + velocity, this.HitBox.Height + Math.Abs(_currentVerticalVelocity)) :
                new Rectangle(this.HitBox.X, _currentVerticalVelocity < 0 ? this.HitBox.Y + _currentVerticalVelocity : this.HitBox.Y, this.HitBox.Width + _currentHorizontalVelocity, this.HitBox.Height + Math.Abs(_currentVerticalVelocity));
            var collisions = this.CheckCollision(areaToCheck, true);

            CollisionObject tile = this.Direction == Enums.Direction.LEFT ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            CollisionObject groundTile = GetTopMostLandingTile(_currentVerticalVelocity);

            UpdateSpriteByDelay();

            List<CommanderKeen> collidingPlayers = GetCollidingPlayers();

            if (groundTile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, groundTile.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                KillCollidingPlayers();
                this.Stop();
                CreateShockWaves();
                return;
            }

            if (tile != null)
            {
                if (_direction == Enums.Direction.LEFT)
                {
                    this.HitBox = new Rectangle(tile.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
                else
                {
                    this.HitBox = new Rectangle(tile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                }
                _currentHorizontalVelocity = GetImpactVelocityHorizontal(_currentHorizontalVelocity, true);
                _direction = this.ChangeHorizontalDirection(_direction);
            }
            else if (collidingPlayers.Any(k => k.HitBox.IntersectsWith(this.HitBox)))
            {
                var keen = this.GetClosestPlayer();
                keen?.Die();
                this.HitBox = new Rectangle(keen.HitBox.Location, this.HitBox.Size);
                this.Stop();
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + _currentHorizontalVelocity, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                AccelerateGravity();
                DecelerateHorizontalVelocityByWindResistance();
            }
        }

        private void DecelerateHorizontalVelocityByWindResistance()
        {
            if (_currentHorizontalVelocity < 0)
            {
                if (_currentHorizontalVelocity + AIR_RESISTANCE <= MIN_HORIZONTAL_VELOCITY * -1)
                {
                    _currentHorizontalVelocity += AIR_RESISTANCE;
                }
                else
                {
                    _currentHorizontalVelocity = MIN_HORIZONTAL_VELOCITY * -1;
                }
            }
            else
            {
                if (_currentHorizontalVelocity - AIR_RESISTANCE >= MIN_HORIZONTAL_VELOCITY)
                {
                    _currentHorizontalVelocity -= AIR_RESISTANCE;
                }
                else
                {
                    _currentHorizontalVelocity = MIN_HORIZONTAL_VELOCITY;
                }
            }
        }

        private void AccelerateGravity()
        {
            if (_currentVerticalVelocity + GRAVITY_ACCELERATION <= MAX_VERTICAL_VELOCITY)
            {
                _currentVerticalVelocity += GRAVITY_ACCELERATION;
            }
            else
            {
                _currentVerticalVelocity = MAX_VERTICAL_VELOCITY;
            }
        }

        private int GetImpactVelocityHorizontal(int velocity, bool switchDirection)
        {
            double kineticLoss = velocity / KINETIC_IMPACT_DENOMINATOR; /*(INITIAL_MOVE_VELOCITY - Math.Abs(velocity)) / INITIAL_MOVE_VELOCITY;*/ // velocity / VELOCITY_DECREASE;  //((double)velocity) / ((double)INITIAL_MOVE_VELOCITY) * 100.0;
            int kLossInt = Convert.ToInt32(kineticLoss);
            // if (kLossInt < velocity)
            velocity -= kLossInt;
            //else 
            //  velocity = 0;
            if (switchDirection)
                velocity *= -1;
            return velocity;
        }

        public void Stop()
        {
            OnRemove(new ObjectEventArgs() { ObjectSprite = this });
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
            get
            {
                return _direction;
            }
            set
            {

            }
        }

        public override CollisionType CollisionType => CollisionType.ENEMY_PROJECTILE;

        public int ZIndex => 200;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
    

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
                if (args.ObjectSprite == this)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                        node.NonEnemies.Remove(this);
                    }
                }
                this.Remove(this, args);
            }
        }
    }
}
