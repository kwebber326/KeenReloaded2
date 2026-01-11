using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities.Hazards;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Wormmouth : DestructibleObject, IMoveable, IUpdatable, ISprite, IGravityObject, IEnemy, IZombieBountyEnemy
    {
        private Image _sprite;
        private Enums.Direction _direction;
        private WormmoutMoveState _state;
        private CommanderKeen _keen;
        private const int FALL_VELOCITY = 50;
        private const int MOVE_VELOCITY = 10;

        private const int PEEK_CHANCE = 30;//set this to a lower value for higher probability
        private const int PEEK_HOLD_TIME = 5;
        private const int PEEK_STATE_CHANGE_DELAY = 3;
        private int _peekStateChangeDelayTick = 0;
        private int _peekStateValue;

        private const int ATTACK_RANGE = 40;
        private const int ATTACK_STATE_CHANGE_DELAY = 1;
        private int _attackStateChangeDelayTick = 0;
        private int _currentAttackImage = 0;

        private int _currentStunImage = 0;
        private Image[] _stunImages = SpriteSheet.SpriteSheet.WormmouthStunSprites;
        private readonly int _zIndex;

        public Wormmouth(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            Initialize();
        }

        private void Initialize()
        {
            this.State = WormmoutMoveState.MOVING;
            this.Direction = this.GetRandomHorizontalDirection();
        }

        public override void Die()
        {
            this.State = WormmoutMoveState.STUNNED;
        }

        public void Update()
        {
            this.KillIfOutSideBoundsOfMap(_collisionGrid);
            if (_state != WormmoutMoveState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }
            switch (_state)
            {
                case WormmoutMoveState.FALLING:
                    this.Fall();
                    break;
                case WormmoutMoveState.MOVING:
                    this.Move();
                    break;
                case WormmoutMoveState.PEEKING:
                    this.Peek();
                    break;
                case WormmoutMoveState.ATTACKING:
                    this.Attack();
                    break;
                case WormmoutMoveState.STUNNED:
                    this.UpdateStunnedState();
                    break;
            }
        }

        private void UpdateStunnedState()
        {
            this.BasicFall(FALL_VELOCITY);
            _currentStunImage = _currentStunImage == _stunImages.Length - 1 ? 0 : _currentStunImage + 1;
            _sprite = _stunImages[_currentStunImage];
        }

        private void Attack()
        {
            if (this.State != WormmoutMoveState.ATTACKING)
            {
                this.State = WormmoutMoveState.ATTACKING;
                this.PublishSoundPlayEvent(
                    GeneralGameConstants.Sounds.WORMMOUTH_ATTACK);
            }

            if (_attackStateChangeDelayTick++ == ATTACK_STATE_CHANGE_DELAY)
            {
                _attackStateChangeDelayTick = 0;
                switch (_currentAttackImage)
                {
                    case 0:
                        int orginalWidth = _sprite.Width;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_attack_left1 : Properties.Resources.keen4_wormmouth_attack_right1;
                        int difference = _sprite.Width - orginalWidth;
                        SetVerticalPositionFromSpriteChange();
                        if (this.Direction == Enums.Direction.LEFT)
                            this.HitBox = new Rectangle(this.HitBox.X - difference, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        _currentAttackImage++;
                        TryKillKeen();
                        break;
                    case 1:
                        orginalWidth = _sprite.Width;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_attack_left2 : Properties.Resources.keen4_wormmouth_attack_right2;
                        difference = _sprite.Width - orginalWidth;
                        SetVerticalPositionFromSpriteChange();
                        if (this.Direction == Enums.Direction.LEFT)
                            this.HitBox = new Rectangle(this.HitBox.X - difference, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        _currentAttackImage++;
                        TryKillKeen();
                        break;
                    case 2:
                        orginalWidth = _sprite.Width;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_attack_left3 : Properties.Resources.keen4_wormmouth_attack_right3;
                        difference = _sprite.Width - orginalWidth;
                        SetVerticalPositionFromSpriteChange();
                        if (this.Direction == Enums.Direction.LEFT)
                            this.HitBox = new Rectangle(this.HitBox.X - difference, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        _currentAttackImage++;
                        TryKillKeen();
                        break;
                    case 3:
                        orginalWidth = _sprite.Width;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_attack_left2 : Properties.Resources.keen4_wormmouth_attack_right2;
                        difference = _sprite.Width - orginalWidth;
                        SetVerticalPositionFromSpriteChange();
                        if (this.Direction == Enums.Direction.LEFT)
                            this.HitBox = new Rectangle(this.HitBox.X - difference, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        _currentAttackImage++;
                        TryKillKeen();
                        break;
                    case 4:
                        orginalWidth = _sprite.Width;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_attack_left1 : Properties.Resources.keen4_wormmouth_attack_right1;
                        difference = _sprite.Width - orginalWidth;
                        SetVerticalPositionFromSpriteChange();
                        if (this.Direction == Enums.Direction.LEFT)
                            this.HitBox = new Rectangle(this.HitBox.X - difference, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        _currentAttackImage++;
                        TryKillKeen();
                        break;
                    case 5:
                        orginalWidth = _sprite.Width;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_attack_left1 : Properties.Resources.keen4_wormmouth_attack_right1;
                        difference = _sprite.Width - orginalWidth;
                        SetVerticalPositionFromSpriteChange();
                        if (this.Direction == Enums.Direction.LEFT)
                            this.HitBox = new Rectangle(this.HitBox.X - difference, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        _currentAttackImage = 0;
                        this.State = WormmoutMoveState.MOVING;
                        break;
                }
            }
        }

        private void TryKillKeen()
        {
            if (_keen.HitBox.IntersectsWith(this.HitBox))
            {
                _keen.Die();
            }
        }

        private void Peek()
        {
            if (this.State != WormmoutMoveState.PEEKING)
            {
                this.State = WormmoutMoveState.PEEKING;
            }

            switch (_peekStateValue)
            {
                case 0:
                    if (_peekStateChangeDelayTick++ == PEEK_STATE_CHANGE_DELAY)
                    {
                        _peekStateChangeDelayTick = 0;
                        _peekStateValue++;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_peek_left1 : Properties.Resources.keen4_wormmouth_peek_right1;
                        SetVerticalPositionFromSpriteChange();
                    }
                    break;
                case 1:
                    if (_peekStateChangeDelayTick++ == PEEK_STATE_CHANGE_DELAY)
                    {
                        _peekStateChangeDelayTick = 0;
                        _peekStateValue++;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_peek_left2 : Properties.Resources.keen4_wormmouth_peek_right2;
                    }
                    break;
                case 2:
                    if (_peekStateChangeDelayTick++ == PEEK_STATE_CHANGE_DELAY)
                    {
                        _peekStateChangeDelayTick = 0;
                        _peekStateValue++;
                        _sprite = Properties.Resources.keen4_wormmouth_move;
                        SwitchDirection();
                        SetVerticalPositionFromSpriteChange();
                    }
                    break;
                case 3:
                    if (_peekStateChangeDelayTick++ == PEEK_STATE_CHANGE_DELAY)
                    {
                        _peekStateChangeDelayTick = 0;
                        _peekStateValue++;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_peek_left1 : Properties.Resources.keen4_wormmouth_peek_right1;
                        SetVerticalPositionFromSpriteChange();
                    }
                    break;
                case 4:
                    if (_peekStateChangeDelayTick++ == PEEK_STATE_CHANGE_DELAY)
                    {
                        _peekStateChangeDelayTick = 0;
                        _peekStateValue++;
                        _sprite = _direction == Enums.Direction.LEFT ? Properties.Resources.keen4_wormmouth_peek_left2 : Properties.Resources.keen4_wormmouth_peek_right2;
                    }
                    break;
                case 5:
                    if (_peekStateChangeDelayTick++ == PEEK_STATE_CHANGE_DELAY)
                    {
                        _peekStateChangeDelayTick = 0;
                        _peekStateValue = 0;
                        _sprite = Properties.Resources.keen4_wormmouth_move;
                        SetDirectionFromKeenLocation();
                        this.State = WormmoutMoveState.MOVING;
                        SetVerticalPositionFromSpriteChange();
                    }
                    break;
            }
        }

        private void SetVerticalPositionFromSpriteChange()
        {
            this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
            var items = this.CheckCollision(new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + 10));
            var tile = GetTopMostLandingTile(items);
            if (tile != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, tile.HitBox.Top - this.HitBox.Height, _sprite.Width, _sprite.Height);
            }
            else
            {
                _peekStateValue = 0;
                this.Fall();
            }
        }

        public void Move()
        {
            int xOffset = this.Direction == Enums.Direction.LEFT ? MOVE_VELOCITY * -1 : MOVE_VELOCITY;
            Rectangle areaToCheck = new Rectangle(this.Direction == Enums.Direction.LEFT ? this.HitBox.X + xOffset : this.HitBox.X, this.HitBox.Y, this.HitBox.Width + MOVE_VELOCITY, this.HitBox.Height + 1);
            var collisions = this.CheckCollision(areaToCheck, true);

            List<CollisionObject> tilesBelow = GetTilesBelow(xOffset, collisions);
            if (!tilesBelow.Any())
            {
                this.State = WormmoutMoveState.FALLING;
                return;
            }
            else if (this.Direction == Enums.Direction.LEFT)
            {
                var minX = tilesBelow.Select(t => t.HitBox.Left).Min();
                if (minX >= this.HitBox.X)
                {
                    SwitchDirection();
                    return;
                }
            }
            else
            {
                var maxX = tilesBelow.Select(t => t.HitBox.Right).Max();
                if (maxX <= this.HitBox.Right)
                {
                    SwitchDirection();
                    return;
                }
            }

            var collisionTiles = collisions.Where(t => t.CollisionType == CollisionType.BLOCK && (t.HitBox.Bottom > this.HitBox.Top && t.HitBox.Top < this.HitBox.Bottom)).ToList();
            if (this.Direction == Enums.Direction.LEFT)
            {
                var items = collisionTiles.OfType<CollisionObject>().Where(t => t.HitBox.X < this.HitBox.X).ToList();
                CollisionObject obj = GetRightMostLeftTile(items);
                if (obj != null)
                {
                    SwitchDirection();
                    return;
                }
            }
            else
            {
                var items = collisionTiles.OfType<CollisionObject>().Where(t => t.HitBox.X > this.HitBox.X).ToList();
                CollisionObject obj = GetRightMostLeftTile(items);
                if (obj != null)
                {
                    SwitchDirection();
                    return;
                }
            }

            if (IsKeenInRange())
            {
                this.Attack();
                return;
            }

            this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            TryPeek();
        }

        private bool IsKeenInRange()
        {
            return ((this.Direction == Enums.Direction.LEFT && (_keen.HitBox.Right >= this.HitBox.X - ATTACK_RANGE && _keen.HitBox.Right < this.HitBox.Left))
                ||
                    (this.Direction == Enums.Direction.RIGHT && (_keen.HitBox.X <= this.HitBox.Right + ATTACK_RANGE && _keen.HitBox.X > this.HitBox.X)))
                && (_keen.HitBox.Bottom > this.HitBox.Top && _keen.HitBox.Top < this.HitBox.Bottom);
        }

        private void TryPeek()
        {
            int peekVal = _random.Next(0, PEEK_CHANCE);
            if (peekVal == 0)
            {
                this.Peek();
            }
        }

        private void SetDirectionFromKeenLocation()
        {
            if (_keen.HitBox.Right < this.HitBox.Left)
            {
                this.Direction = Enums.Direction.LEFT;
            }
            else if (_keen.HitBox.Left > this.HitBox.Right)
            {
                this.Direction = Enums.Direction.RIGHT;
            }
        }

        protected override CollisionObject GetRightMostLeftTile(List<CollisionObject> collisions)
        {
            var tiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK);
            if (tiles.Any())
            {
                return tiles.Where(t => t.HitBox.Top != this.HitBox.Bottom).OrderByDescending(c => c.HitBox.Right).FirstOrDefault();
            }
            return null;
        }

        protected override CollisionObject GetLeftMostRightTile(List<CollisionObject> collisions)
        {
            var tiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK);
            if (tiles.Any())
            {
                return tiles.Where(t => t.HitBox.Top != this.HitBox.Bottom).OrderBy(c => c.HitBox.Left).FirstOrDefault();
            }
            return null;
        }

        private List<CollisionObject> GetTilesBelow(int xOffset, List<CollisionObject> collisions)
        {
            var tiles = collisions.Where(c => !(c is MovingPlatformTile) && (c.CollisionType == CollisionType.BLOCK || c.CollisionType == CollisionType.PLATFORM || c.CollisionType == CollisionType.POLE_TILE) && c.HitBox.Top == this.HitBox.Bottom).ToList();
            return tiles;
        }

        private void SwitchDirection()
        {
            this.Direction = _direction == Enums.Direction.LEFT ? Enums.Direction.RIGHT : Enums.Direction.LEFT;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        private WormmoutMoveState State
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

        public override System.Drawing.Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (_collisionGrid != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(this.Direction);
                    if (this.State == WormmoutMoveState.FALLING)
                    {
                        this.UpdateCollisionNodes(Direction.DOWN);
                    }
                }
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
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            switch (_state)
            {
                case WormmoutMoveState.MOVING:
                    _sprite = Properties.Resources.keen4_wormmouth_move;
                    break;
                case WormmoutMoveState.PEEKING:
                    break;
                case WormmoutMoveState.ATTACKING:
                    break;
                case WormmoutMoveState.STUNNED:
                    _sprite = Properties.Resources.keen4_wormmouth_stun1;
                    SetVerticalPositionFromSpriteChange();
                    break;
            }
            this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
        }

        public void Jump()
        {
            
        }

        public bool CanJump
        {
            get { return false; }
        }

        public void Fall()
        {
            if (this.State != WormmoutMoveState.FALLING)
            {
                this.State = WormmoutMoveState.FALLING;
            }
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height + FALL_VELOCITY);
            var items = this.CheckCollision(areaToCheck, true);
            CollisionObject obj = GetTopMostLandingTile(items);
            if (obj != null)
            {
                this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Top - this.HitBox.Height, this.HitBox.Width, this.HitBox.Height);
                this.State = WormmoutMoveState.MOVING;
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + FALL_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            }
        }

        public bool DeadlyTouch
        {
            get { return this.State == WormmoutMoveState.ATTACKING; }
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
        }

        public bool IsActive
        {
            get { return this.State == WormmoutMoveState.PEEKING || this.State == WormmoutMoveState.ATTACKING; }
        }

        public PointItemType PointItem => PointItemType.KEEN4_JAWBREAKER;

        public override CollisionType CollisionType => CollisionType.ENEMY;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen4_wormmouth_move);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }

    enum WormmoutMoveState
    {
        MOVING,
        PEEKING,
        ATTACKING,
        STUNNED,
        FALLING
    }
}
