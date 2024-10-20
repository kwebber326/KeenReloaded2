using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class BabobbaFireBall : CollisionObject, IUpdatable, ISprite, IProjectile, ICreateRemove
    {
        private const int GRAVITY_ACCELERATION = 5;
        private const int AIR_RESISTANCE = 5;
        private const int KINETIC_IMPACT_DENOMINATOR_HORIZONTAL = 10;
        private const int KINETIC_IMPACT_DENOMINATOR_VERTICAL = 2;
        private const int MAX_VERTICAL_VELOCITY = 40;
        private const int MIN_HORIZONTAL_VELOCITY = 25;
        private int _currentHorizontalVelocity, _currentVerticalVelocity;
        private Enums.Direction _direction;
        private Image _sprite;
        private BabobbaFireBallState _state;
        private bool _stoppedHorizontalMovement;
        private int _lastVerticalImpact;
        private bool _stoppedVerticalMovement;
        private const int INITIAL_MOVE_VELOCITY = 30;
        private const int MIN_VERTICAL_VELOCITY = 5;

        private const int STOP_STAGE_1_TIME = 30;
        private int _stage1Tick;

        private const int STOP_STAGE_2_TIME = 25;
        private int _stage2Tick;

        private const int STOPPED_SPRITE_CHANGE_DELAY1 = 1;
        private const int STOPPED_SPRITE_CHANGE_DELAY2 = 2;
        private int _spriteChangeTick;

        public BabobbaFireBall(SpaceHashGrid grid, Rectangle hitbox, Direction direction)
            : base(grid, hitbox)
        {
            _direction = direction;
            Initialize();
        }

        private void Initialize()
        {
            _sprite = Properties.Resources.keen6_babobba_fire_ball1;

            _currentHorizontalVelocity = _direction == Direction.LEFT ? INITIAL_MOVE_VELOCITY * -1 : INITIAL_MOVE_VELOCITY;
            _currentVerticalVelocity = 0;
            _lastVerticalImpact = int.MaxValue;
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK && c.HitBox.Bottom <= this.HitBox.Top && c.HitBox.Left <= this.HitBox.Right && c.HitBox.Right >= this.HitBox.Left).ToList();
            if (debugTiles.Any())
            {
                int maxBottom = debugTiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile;
            var landingTiles = collisions.Where(h => (h.CollisionType == CollisionType.BLOCK || h.CollisionType == CollisionType.PLATFORM)
                && h.HitBox.Top >= this.HitBox.Top && h.HitBox.Left <= this.HitBox.Right && h.HitBox.Right >= this.HitBox.Left);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        public void Update()
        {
            switch (_state)
            {
                case BabobbaFireBallState.MOVING:
                    this.Move();
                    break;
                case BabobbaFireBallState.STOPPED_STAGE_1:
                    this.Stop();
                    break;
                case BabobbaFireBallState.STOPPED_STAGE_2:
                    this.StopStage2();
                    break;
            }
        }

        private void StopStage2()
        {
            if (this.State != BabobbaFireBallState.STOPPED_STAGE_2)
            {
                this.State = BabobbaFireBallState.STOPPED_STAGE_2;
                _stage2Tick = 0;
                _spriteChangeTick = 0;
            }

            if (_stage2Tick++ != STOP_STAGE_2_TIME)
            {
                if (_spriteChangeTick++ == STOPPED_SPRITE_CHANGE_DELAY2)
                {
                    if (_currentStopSprite == 0)
                    {
                        _currentStopSprite = 1;
                    }
                    else
                    {
                        _currentStopSprite = 0;
                    }
                    _spriteChangeTick = 0;
                    UpdateSprite();
                }
            }
            else
            {
                OnRemove(new ObjectEventArgs() { ObjectSprite = this });
            }
        }

        private int GetImpactVelocityHorizontal(int velocity, bool switchDirection)
        {
            double kineticLoss = velocity / KINETIC_IMPACT_DENOMINATOR_HORIZONTAL; /*(INITIAL_MOVE_VELOCITY - Math.Abs(velocity)) / INITIAL_MOVE_VELOCITY;*/ // velocity / VELOCITY_DECREASE;  //((double)velocity) / ((double)INITIAL_MOVE_VELOCITY) * 100.0;
            int kLossInt = Convert.ToInt32(kineticLoss);
            // if (kLossInt < velocity)
            velocity -= kLossInt;
            //else 
            //  velocity = 0;
            if (switchDirection)
                velocity *= -1;
            return velocity;
        }

        private int GetImpactVelocityVertical(int velocity)
        {
            double kineticLoss = velocity / KINETIC_IMPACT_DENOMINATOR_VERTICAL; //(((double)velocity + VELOCITY_DECREASE) - (double)velocity) / ((double)velocity) * 100.0;
            int kLossInt = Convert.ToInt32(kineticLoss);
            if (kLossInt < velocity)
            {
                velocity -= kLossInt;
            }
            else
            {
                velocity = 0;
            }
            velocity *= -1;
            return velocity;
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
                    this.UpdateCollisionNodes(this.Direction);
                    if (_currentVerticalVelocity < 0)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.UP);
                    }
                    else if (_currentVerticalVelocity > 0)
                    {
                        this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    }
                }
            }
        }

        BabobbaFireBallState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            switch (_state)
            {
                case BabobbaFireBallState.MOVING:
                case BabobbaFireBallState.STOPPED_STAGE_1:
                    if (_currentStopSprite == 0)
                    {
                        _sprite = Properties.Resources.keen6_babobba_fire_ball1;
                    }
                    else
                    {
                        _sprite = Properties.Resources.keen6_babobba_fire_ball2;
                    }
                    break;
                case BabobbaFireBallState.STOPPED_STAGE_2:
                    if (_currentStopSprite == 0)
                    {
                        _sprite = Properties.Resources.keen6_babobba_fire_ball2;
                    }
                    else
                    {
                        _sprite = null;
                    }
                    break;
            }
        }

        public int Damage
        {
            get { return 1; }
        }

        public int Velocity
        {
            get { return 60; }
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

        public void Move()
        {
            if (_stoppedHorizontalMovement && _stoppedVerticalMovement)
            {
                this.Stop();
                return;
            }

            Rectangle areaToCheck = new Rectangle(
                  _currentHorizontalVelocity < 0 ? this.HitBox.X + _currentHorizontalVelocity : this.HitBox.X //X
                , _currentVerticalVelocity < 0 ? this.HitBox.Y + _currentVerticalVelocity : this.HitBox.Y //Y
                , this.HitBox.Width + Math.Abs(_currentHorizontalVelocity)//width
                , this.HitBox.Height + Math.Abs(_currentVerticalVelocity));//height

            var collisions = this.CheckCollision(areaToCheck, true);

            var horizontalTile = _currentHorizontalVelocity < 0 ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            var verticalTile = _currentVerticalVelocity < 0 ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);

            if (horizontalTile != null)
            {
                int collisionXPos = _currentHorizontalVelocity < 0 ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(collisionXPos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                GetHorizontalImpact();
                this.KillCollidingPlayers();
                DecelerateHorizontalMovement();
            }
            else
            {
                Rectangle areaToCheckToKillKeen = new Rectangle(
                    _currentHorizontalVelocity < 0 ? this.HitBox.X + _currentHorizontalVelocity : this.HitBox.X,
                    this.HitBox.Y, this.HitBox.Width + Math.Abs(_currentHorizontalVelocity), this.HitBox.Height);
                this.KillCollidingPlayers(areaToCheckToKillKeen);
                this.HitBox = new Rectangle(this.HitBox.X + _currentHorizontalVelocity, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                DecelerateHorizontalMovement();
            }

            if (verticalTile != null)
            {
                int _collisionYPos = _currentVerticalVelocity < 0 ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                this.HitBox = new Rectangle(this.HitBox.X, _collisionYPos, this.HitBox.Width, this.HitBox.Height);
                GetVerticalImpact();
                this.KillCollidingPlayers();
            }
            else
            {
                Rectangle areaToCheckToKillKeen = new Rectangle(
                   this.HitBox.X,
                   _currentVerticalVelocity < 0 ? this.HitBox.Y + _currentVerticalVelocity : this.HitBox.Y
                   , this.HitBox.Width, this.HitBox.Height + Math.Abs(_currentVerticalVelocity));

                this.KillCollidingPlayers(areaToCheckToKillKeen);
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + _currentVerticalVelocity, this.HitBox.Width, this.HitBox.Height);
                AccelerateVerticalMovement();
                if (_currentHorizontalVelocity == 0 && Math.Abs(_lastVerticalImpact) <= MIN_VERTICAL_VELOCITY)
                {
                    _stoppedVerticalMovement = true;
                    _stoppedHorizontalMovement = true;
                }
            }
        }

        public void Stop()
        {
            if (this.State != BabobbaFireBallState.STOPPED_STAGE_1)
            {
                this.State = BabobbaFireBallState.STOPPED_STAGE_1;
                _spriteChangeTick = 0;
                _stage1Tick = 0;
            }

            if (_stage1Tick++ != STOP_STAGE_1_TIME)
            {
                if (_spriteChangeTick++ == STOPPED_SPRITE_CHANGE_DELAY1)
                {
                    if (_currentStopSprite == 0)
                    {
                        _currentStopSprite = 1;
                    }
                    else
                    {
                        _currentStopSprite = 0;
                    }
                    _spriteChangeTick = 0;
                    UpdateSprite();
                }
            }
            else
            {
                this.StopStage2();
            }
        }

        private void GetVerticalImpact()
        {
            if (Math.Abs(_currentVerticalVelocity) <= MIN_HORIZONTAL_VELOCITY && _currentHorizontalVelocity == 0)
            {
                _currentVerticalVelocity = 0;
                _stoppedVerticalMovement = true;
            }
            else
            {
                _currentVerticalVelocity = GetImpactVelocityVertical(_currentVerticalVelocity);
                _lastVerticalImpact = _currentVerticalVelocity;
                _currentHorizontalVelocity = GetImpactVelocityHorizontal(_currentHorizontalVelocity, false);
            }
        }

        private void GetHorizontalImpact()
        {
            if (Math.Abs(_currentHorizontalVelocity) <= MIN_HORIZONTAL_VELOCITY)
            {
                _currentHorizontalVelocity = 0;
                _stoppedHorizontalMovement = true;
            }
            else
            {
                _currentHorizontalVelocity = GetImpactVelocityHorizontal(_currentHorizontalVelocity, true);
            }
        }

        private void AccelerateVerticalMovement()
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

        private void DecelerateHorizontalMovement()
        {
            if (_currentHorizontalVelocity < 0)
            {
                if (_currentHorizontalVelocity + AIR_RESISTANCE <= 0)
                {
                    _currentHorizontalVelocity += AIR_RESISTANCE;
                }
                else
                {
                    _currentHorizontalVelocity = 0;
                    //end horizontal movement
                    _stoppedHorizontalMovement = true;
                }
            }
            else if (_currentHorizontalVelocity > 0)
            {
                if (_currentHorizontalVelocity - AIR_RESISTANCE >= 0)
                {
                    _currentHorizontalVelocity -= AIR_RESISTANCE;
                }
                else
                {
                    _currentHorizontalVelocity = 0;
                    //end horizontalMovement;
                    _stoppedHorizontalMovement = true;
                }
            }
            else
            {
                _stoppedHorizontalMovement = true;
            }
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
                _direction = value;
            }
        }

        public int ZIndex => 200;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.ENEMY_PROJECTILE;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private int _currentStopSprite;



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

    enum BabobbaFireBallState
    {
        MOVING,
        STOPPED_STAGE_1,
        STOPPED_STAGE_2
    }
}
