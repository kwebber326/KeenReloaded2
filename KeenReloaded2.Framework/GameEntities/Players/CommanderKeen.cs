using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Animations;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Hazards;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Items;
using KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEntities.Weapons;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Framework.GameEntities.Players
{
    public class CommanderKeen
        : DestructibleObject, IFireable, IGravityObject, IMoveable, IStunnable, ISprite, IUpdatable, ICreateRemove
    {
        public CommanderKeen(Rectangle area, SpaceHashGrid grid, Direction direction)
            : base(grid, area)
        {
            this.HitBox = area;
            Initialize(direction);
        }

        public CommanderKeen(Rectangle area, SpaceHashGrid grid, Direction direction, int lives, long points)
           : base(grid, area)
        {
            this.Lives = lives;
            this.Points = points;
            this.HitBox = area;
            Initialize(direction);
        }

        private void Initialize(Direction direction)
        {
            _keysPressed = new Dictionary<string, bool>();
            _keysPressed.Add(KEY_LEFT, false);
            _keysPressed.Add(KEY_RIGHT, false);
            _keysPressed.Add(KEY_UP, false);
            _keysPressed.Add(KEY_DOWN, false);
            _keysPressed.Add(KEY_CTRL, false);
            _keysPressed.Add(KEY_SPACE, false);
            _keysPressed.Add(KEY_1, false);
            _keysPressed.Add(KEY_2, false);
            _keysPressed.Add(KEY_3, false);
            _keysPressed.Add(KEY_4, false);
            _keysPressed.Add(KEY_5, false);
            _keysPressed.Add(KEY_6, false);
            _keysPressed.Add(KEY_7, false);
            _keysPressed.Add(KEY_8, false);
            _keysPressed.Add(KEY_ENTER, false);
            _keysPressed.Add(KEY_SHIFT, false);
            _keysPressed.Add(KEY_SHIELD, false);
            this.Health = 1;
            _moveState = Enums.MoveState.STANDING;
            InitializeDirection(direction);
            UpdateSprite();
            InitializeWeapons();
            InitializeGemList();
            InitializeFlagList();
        }



        #region event handlers

        public event EventHandler<WeaponEventArgs> KeenAcquiredWeapon;
        public event EventHandler<ItemAcquiredEventArgs> KeenAcquiredItem;
        public event EventHandler KeenMoved;

        protected void OnKeenAcquiredWeapon(WeaponEventArgs e)
        {
            if (KeenAcquiredWeapon != null)
                KeenAcquiredWeapon(this, e);
        }

        protected void OnKeenAcquiredItem(ItemAcquiredEventArgs e)
        {
            if (KeenAcquiredItem != null)
                KeenAcquiredItem(this, e);
        }

        protected void OnKeenMoved(EventArgs e)
        {
            if (this.KeenMoved != null)
                KeenMoved(this, e);
        }

        #endregion


        #region helper methods

        private void InitializeGemList()
        {
            _gems = new List<Gem>();
        }

        private void InitializeFlagList()
        {
            _flags = new List<Flag>();
        }

        public void InitializeWeapons()
        {
            _weapons = new List<NeuralStunner>()
            {
                new NeuralStunner(this._collisionGrid, this.HitBox)
            };
            this.CurrentWeapon = _weapons[0];
            RegisterProjectileEvents(this.CurrentWeapon);
            OnKeenAcquiredWeapon(new WeaponEventArgs() { Weapon = _currentWeapon });
        }

        public void InitializeWeapons(List<NeuralStunner> weaponSet)
        {
            _weapons = new List<NeuralStunner>();
            for (int i = 0; i < weaponSet.Count; i++)
            {
                if (weaponSet[i].GetType().Name == typeof(NeuralStunner).Name)
                {
                    weaponSet[i] = new NeuralStunner(_collisionGrid, new Rectangle(weaponSet[i].HitBox.Location, weaponSet[i].HitBox.Size), weaponSet[i].Ammo);
                }
                else if (weaponSet[i].GetType().Name == typeof(ShotgunNeuralStunner).Name)
                {
                    weaponSet[i] = new ShotgunNeuralStunner(_collisionGrid, new Rectangle(weaponSet[i].HitBox.Location, weaponSet[i].HitBox.Size), weaponSet[i].Ammo);
                }
                else if (weaponSet[i].GetType().Name == typeof(SMGNeuralStunner).Name)
                {
                    weaponSet[i] = new SMGNeuralStunner(_collisionGrid, new Rectangle(weaponSet[i].HitBox.Location, weaponSet[i].HitBox.Size), weaponSet[i].Ammo);
                }
                else if (weaponSet[i].GetType().Name == typeof(RPGNeuralStunner).Name)
                {
                    weaponSet[i] = new RPGNeuralStunner(_collisionGrid, new Rectangle(weaponSet[i].HitBox.Location, weaponSet[i].HitBox.Size), weaponSet[i].Ammo);
                }
                else if (weaponSet[i].GetType().Name == typeof(BoobusBombLauncher).Name)
                {
                    weaponSet[i] = new BoobusBombLauncher(_collisionGrid, new Rectangle(weaponSet[i].HitBox.Location, weaponSet[i].HitBox.Size), weaponSet[i].Ammo);
                }
                else if (weaponSet[i].GetType().Name == typeof(RailgunNeuralStunner).Name)
                {
                    weaponSet[i] = new RailgunNeuralStunner(_collisionGrid, new Rectangle(weaponSet[i].HitBox.Location, weaponSet[i].HitBox.Size), weaponSet[i].Ammo);
                }
                else if (weaponSet[i].GetType().Name == typeof(BFG).Name)
                {
                    weaponSet[i] = new BFG(_collisionGrid, new Rectangle(weaponSet[i].HitBox.Location, weaponSet[i].HitBox.Size), weaponSet[i].Ammo);
                }
                else if (weaponSet[i].GetType().Name == typeof(SnakeGun).Name)
                {
                    weaponSet[i] = new SnakeGun(_collisionGrid, new Rectangle(weaponSet[i].HitBox.Location, weaponSet[i].HitBox.Size), weaponSet[i].Ammo);
                }
                RegisterProjectileEvents(weaponSet[i]);

                _weapons.Add(weaponSet[i]);
            }
            this.CurrentWeapon = _weapons[0];
        }

        private void RegisterProjectileEvents(NeuralStunner weaponSet)
        {
            if (weaponSet == null)
                return;

            weaponSet.CreatedObject += CommanderKeen_CreatedProjectile;
            weaponSet.RemovedObject += CommanderKeen_RemovedProjectile;
        }

        private void CommanderKeen_RemovedProjectile(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        private void CommanderKeen_CreatedProjectile(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        private void ContinueDeathSequence()
        {
            if (_collidingNodes != null && _collidingNodes.Any())
            {
                if (_deathFalling)
                {
                    if (_fallVelocity <= MAX_FALL_VELOCITY - FALL_ACCELLERATION)
                        _fallVelocity += FALL_ACCELLERATION;

                    if (this.Direction == Enums.Direction.RIGHT)
                    {
                        this.HitBox = new Rectangle(new Point(this.HitBox.X + 15, this.HitBox.Y + _fallVelocity), this.HitBox.Size);
                    }
                    else
                    {
                        this.HitBox = new Rectangle(new Point(this.HitBox.X - 15, this.HitBox.Y + _fallVelocity), this.HitBox.Size);
                    }
                    this.UpdateCollisionNodes(Direction.DOWN);

                    var items = this.CheckCollision(this.HitBox);
                    if (items.OfType<Hazard>().Any())
                    {
                        this.Die();
                    }

                }
                else if (_currentDeathJump < _maxDeathJump)
                {
                    int deathJumpAmount = 10;
                    _currentDeathJump += deathJumpAmount;
                    if (this.Direction == Enums.Direction.RIGHT)
                    {
                        this.HitBox = new Rectangle(new Point(this.HitBox.X + 15, this.HitBox.Y - deathJumpAmount), this.HitBox.Size);
                    }
                    else
                    {
                        this.HitBox = new Rectangle(new Point(this.HitBox.X - 15, this.HitBox.Y - deathJumpAmount), this.HitBox.Size);
                    }
                    this.UpdateCollisionNodes(Direction.DOWN);
                }
                else
                {
                    _deathFalling = true;
                }
            }
        }

        private void AbortClimb()
        {
            //if climbing, reset location
            if (_hangTile != null)
            {
                if (this.Direction == Enums.Direction.RIGHT)
                {
                    this.HitBox = new Rectangle(new Point(_hangTile.HitBox.Right + 1, this.HitBox.Location.Y), this.HitBox.Size);
                }
                else if (this.Direction == Enums.Direction.LEFT)
                {
                    this.HitBox = new Rectangle(new Point(_hangTile.HitBox.X - this.HitBox.Width - 1, this.HitBox.Location.Y), this.HitBox.Size);
                }
                _hangTile = null;
            }
            _currentClimbDelayTicks = 0;
            _climbReady = false;
            //fall
            this.Fall();
        }

        protected void UpdateSprite()
        {
            if (!IsDead())
            {
                if (_isLookingUp)
                {
                    _sprite = _keenLookUp;
                    if (IsFiring)
                        UpdateShootSprite();
                }
                else if (_isLookingDown)
                {
                    _sprite = _keenLookDownSprites[_currentLookDownSprite];
                }
                else if (_togglingSwitch)
                {
                    _sprite = _keenEnterDoor1;
                }
                else
                {
                    if (_isFiring)
                    {
                        UpdateShootSprite();
                        return;
                    }

                    if (this.MoveState == Enums.MoveState.ENTERING_DOOR)
                    {
                        _sprite = _keenDoorEnterSprites[_currentDoorWalkState];
                        return;
                    }
                    if (this.MoveState == Enums.MoveState.STUNNED)
                    {
                        _sprite = _keenStunned;
                        return;
                    }

                    switch (this.Direction)
                    {
                        case Enums.Direction.RIGHT:
                            if (this.MoveState == MoveState.STANDING)
                            {
                                _sprite = _keenStandright;
                            }
                            else if (this.MoveState == Enums.MoveState.RUNNING)
                            {
                                _sprite = _keenRunRightSprites[_currentRunImage];
                                _currentRunImage = _currentRunImage == _keenRunRightSprites.Length - 1 ? 0 : _currentRunImage + 1;
                            }
                            else if (this.MoveState == Enums.MoveState.FALLING)
                            {
                                _sprite = _keenFallRight;
                            }
                            else if (this.MoveState == Enums.MoveState.JUMPING)
                            {
                                _sprite = _keenJumpRight1;
                            }
                            else if (this.MoveState == Enums.MoveState.HANGING)
                            {
                                _sprite = _keenHangRight;
                            }
                            else if (this.MoveState == Enums.MoveState.CLIMBING)
                            {
                                _sprite = _keenClimbRightSprites[_currentClimbSprite];
                            }
                            else if (this.MoveState == Enums.MoveState.ON_POLE)
                            {
                                if (this.PoleDirection == Enums.Direction.UP)
                                {
                                    int spriteIndex = _currentPoleClimbSprite > _keenClimbUpPoleLeftSprites.Length - 1 ? 0 : _currentPoleClimbSprite;
                                    _sprite = _keenClimbUpPoleLeftSprites[spriteIndex];
                                }
                                else
                                {
                                    _sprite = _keenClimbDownPoleSprites[_currentPoleClimbSprite];
                                }
                            }
                            break;
                        case Enums.Direction.LEFT:
                            if (this.MoveState == MoveState.STANDING)
                                _sprite = _keenStandLeft;
                            else if (this.MoveState == Enums.MoveState.RUNNING)
                            {
                                _sprite = _keenRunLeftSprites[_currentRunImage];
                                _currentRunImage = _currentRunImage == _keenRunLeftSprites.Length - 1 ? 0 : _currentRunImage + 1;
                            }
                            else if (this.MoveState == Enums.MoveState.FALLING)
                            {
                                _sprite = _keenFallLeft;
                            }
                            else if (this.MoveState == Enums.MoveState.JUMPING)
                            {
                                _sprite = _keenJumpLeft1;
                            }
                            else if (this.MoveState == Enums.MoveState.HANGING)
                            {
                                _sprite = _keenHangLeft;
                            }
                            else if (this.MoveState == Enums.MoveState.CLIMBING)
                            {
                                _sprite = _keenClimbLeftSprites[_currentClimbSprite];
                            }
                            else if (this.MoveState == Enums.MoveState.ON_POLE)
                            {
                                if (this.PoleDirection == Enums.Direction.UP)
                                {
                                    int spriteIndex = _currentPoleClimbSprite > _keenClimbUpPoleRightSprites.Length - 1 ? 0 : _currentPoleClimbSprite;
                                    _sprite = _keenClimbUpPoleRightSprites[spriteIndex];
                                }
                                else
                                {
                                    _sprite = _keenClimbDownPoleSprites[_currentPoleClimbSprite];
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void InitializeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.DOWN_LEFT:
                case Direction.UP_LEFT:
                case Direction.LEFT:
                    _direction = Direction.LEFT;
                    break;
                case Direction.DOWN_RIGHT:
                case Direction.UP_RIGHT:
                case Direction.RIGHT:
                    _direction = Direction.RIGHT;
                    break;
                default:
                    _direction = Direction.RIGHT;
                    break;
            }
        }

        private bool IsLandingOnPlaform(List<CollisionObject> collisionObjects)
        {
            foreach (var item in collisionObjects)
            {
                if (item.CollisionType == CollisionType.BLOCK)
                    return true;
                else if ((item.CollisionType == CollisionType.KEEN_ONLY_PLATFORM || item.CollisionType == CollisionType.PLATFORM || item.CollisionType == CollisionType.POLE_TILE) && item.HitBox.Top >= this.HitBox.Bottom)//PLATFORM CODE
                {
                    return true;
                }
            }
            return false;
        }

        private int GetTopMostLandingPlatformYPos(List<CollisionObject> collisionObjects)
        {
            List<CollisionObject> items = collisionObjects  //added bounder
                .Where(c => (c.CollisionType == CollisionType.KEEN_ONLY_PLATFORM && c.HitBox.Top > this.HitBox.Bottom - 16)
                        || c.CollisionType == CollisionType.BLOCK || ((c.CollisionType == CollisionType.PLATFORM
                        || c.CollisionType == CollisionType.POLE_TILE) && c.HitBox.Top >= this.HitBox.Bottom)//PLATFORM CODE
                        || (c.CollisionType == CollisionType.KEEN6_SWITCH && ((Keen6Switch)c).IsActive))//keen6 switch code
                .ToList();
            if (!items.Any())
                return -1;

            return items.Select(c => c.HitBox.Top).Min();
        }

        private bool IsNothingBeneathKeen()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.Location.X, this.HitBox.Bottom + 1, this.HitBox.Width, 1);
            var collisionObjectsBelow = this.CheckCollision(areaToCheck);
            if (!IsLandingOnPlaform(collisionObjectsBelow)
                && this.MoveState != Enums.MoveState.JUMPING
                && this.MoveState != Enums.MoveState.ON_POLE)
            {
                return true;
            }
            var tile = GetTopMostLandingTile(collisionObjectsBelow);
            if (tile != null && tile is MapEdgeTile)
            {
                HandleCollision(tile);
            }
            else if (tile != null && tile is MovingPlatformTile)
            {
                var movingTile = ((MovingPlatformTile)tile);
                if (movingTile.Keen != this)
                {
                    movingTile.AssignKeen(this);
                }
            }
            return false;
        }

        private bool IsPlatformBeneathKeen()
        {
            Rectangle areaToCheck = new Rectangle(this.HitBox.Location.X, this.HitBox.Bottom + 1, this.HitBox.Width, 1);
            var collisionObjectsBelow = this.CheckCollision(areaToCheck);
            bool platforms = collisionObjectsBelow.Any(t => (t.CollisionType == CollisionType.PLATFORM || t.CollisionType == CollisionType.KEEN_ONLY_PLATFORM) && t.HitBox.Top <= this.HitBox.Bottom + 1);//PLATFORM CODE
            return platforms;
        }

        #endregion

        #region private fields

        private Dictionary<string, bool> _keysPressed;
        private const string KEY_LEFT = "Left";
        private const string KEY_RIGHT = "Right";
        private const string KEY_UP = "Up";
        private const string KEY_DOWN = "Down";
        private const string KEY_CTRL = "ControlKey";
        private const string KEY_SPACE = "Space";
        private const string KEY_1 = "D1";
        private const string KEY_2 = "D2";
        private const string KEY_3 = "D3";
        private const string KEY_4 = "D4";
        private const string KEY_5 = "D5";
        private const string KEY_6 = "D6";
        private const string KEY_7 = "D7";
        private const string KEY_8 = "D8";
        private const string KEY_ENTER = "Return";
        private const string KEY_SHIFT = "ShiftKey";
        private const string KEY_SHIELD = "S";

        private const int WEAPON_ROTATE_DELAY = 2;
        private int _currentWeaponRotateDelayTick = WEAPON_ROTATE_DELAY;
        private const int STUN_TIME = 15;
        private int _stunTimeTick;

        private bool _failedExitDoorAccess;
        private const int FAILED_ACCESS_WAIT_TIME = 10;
        private int _failedAccessSoundDelayTick;

        private const int VELOCITY = 15;
        private const int FALL_ACCELLERATION = 15;
        private const int INITIAL_FALL_VELOCITY = 0;
        private const int MAX_FALL_VELOCITY = 45;
        private const int SHOT_DELAY = 125;

        private int _fallVelocity = INITIAL_FALL_VELOCITY;

        private const int JUMP_VELOCITY = 30;
        private const int MAX_JUMP_HEIGHT = 200;
        private const int MAX_POLE_JUMP_HEIGHT = 50;
        private const int POLE_CLIMB_SPEED = 3;
        private const int POLE_SHIMMY_SPEED = 7;
        private const int POLE_HANG_DELAY = 2;
        private int _currentPoleHangDelayTick = POLE_HANG_DELAY;
        private bool _jumpFromPole = false;
        private int _currentJumpHeight = 0;
        private bool _jumpReady = true;
        private const int POLE_CLIMB_CHANGE_SPRITE_DELAY = 1;
        private int _currentPoleClimbChangeSpriteTick = 0;

        private MoveState _moveState;
        private Direction _direction;
        private Direction _hangAndClimbDirection;
        private Direction _poleDirection = Direction.UP;
        private Pole _currentPole;

        private Image _sprite;
        private IHangableTile _hangTile;

        private int _currentClimbSprite = 0;
        private const int CLIMB_DELAY_TICKS = 2;
        private int _currentClimbDelayTicks = 0;
        private const int START_CLIMB_TICKS = 2;
        private int _currentStartClimbDelayTick = 0;
        private bool _climbReady = false;

        private bool _isLookingUp = false;
        private bool _isLookingDown = false;
        private Rectangle? _originalStandingHitbox = null;

        private bool _beingPushedLeft = false;
        private bool _beingPushedRight = false;
        private HashSet<CollisionObject> _leftPushingObjects = new HashSet<CollisionObject>();
        private HashSet<CollisionObject> _rightPushingObjects = new HashSet<CollisionObject>();

        private int _currentRunImage = 0;
        protected Image[] _keenRunRightSprites = new Image[]
        {
            Properties.Resources.keen_run_right1,
            Properties.Resources.keen_run_right2,
            Properties.Resources.keen_run_right3,
            Properties.Resources.keen_run_right4
        };

        protected Image[] _keenRunLeftSprites = new Image[]
        {
            Properties.Resources.keen_run_left1,
            Properties.Resources.keen_run_left2,
            Properties.Resources.keen_run_left3,
            Properties.Resources.keen_run_left4
        };

        protected Image[] _keenClimbRightSprites = new Image[]
        {
            Properties.Resources.keen_climb_right1,
            Properties.Resources.keen_climb_right2,
            Properties.Resources.keen_climb_right3,
            Properties.Resources.keen_climb_right4
        };

        protected Image[] _keenClimbLeftSprites = new Image[]
        {
            Properties.Resources.keen_climb_left1,
            Properties.Resources.keen_climb_left2,
            Properties.Resources.keen_climb_left3,
            Properties.Resources.keen_climb_left4
        };

        protected Image[] _keenClimbUpPoleLeftSprites = new Image[]
        {
            Properties.Resources.keen_pole_left1,
            Properties.Resources.keen_pole_left2,
            Properties.Resources.keen_pole_left3
        };

        protected Image[] _keenClimbUpPoleRightSprites = new Image[]
        {
            Properties.Resources.keen_pole_right1,
            Properties.Resources.keen_pole_right2,
            Properties.Resources.keen_pole_right3
        };

        protected Image[] _keenClimbDownPoleSprites = new Image[]
        {
            Properties.Resources.keen_shimmy_down1,
            Properties.Resources.keen_shimmy_down2,
            Properties.Resources.keen_shimmy_down3,
            Properties.Resources.keen_shimmy_down4,
        };

        protected Image[] _keenLookDownSprites = new Image[]
        {
             Properties.Resources.keen_look_down1,
             Properties.Resources.keen_look_down2
        };

        protected Image[] _keenDoorEnterSprites = new Image[]
        {
            Properties.Resources.keen_enter_door1,
            Properties.Resources.keen_enter_door2,
            Properties.Resources.keen_enter_door3,
            Properties.Resources.keen_enter_door4,
            Properties.Resources.keen_enter_door5
        };

        #region solo image fields
        protected Image _keenShootUpAerial = Properties.Resources.keen_shoot_up_aerial;
        protected Image _keenShootDownAerial = Properties.Resources.keen_shoot_down_aerial;
        protected Image _keenShootRightAerial = Properties.Resources.keen_shoot_right_aerial;
        protected Image _keenShootLeftAerial = Properties.Resources.keen_shoot_left_aerial;

        protected Image _keenShootUp = Properties.Resources.keen_shoot_up;
        protected Image _keenShootLeft = Properties.Resources.keen_shoot_left_standing;
        protected Image _keenShootRight = Properties.Resources.keen_shoot_right_standing;

        protected Image _keenShootDownRightPole = Properties.Resources.keen_shoot_down_pole_right;
        protected Image _keenShootUpRightPole = Properties.Resources.keen_shoot_up_pole_right;
        protected Image _keenShootDownLeftPole = Properties.Resources.keen_shoot_down_pole_left;
        protected Image _keenShootUpLeftPole = Properties.Resources.keen_shoot_up_pole_left;
        protected Image _keenShootPoleLeft = Properties.Resources.keen_shoot_left_pole;
        protected Image _keenShootPoleRight = Properties.Resources.keen_shoot_right_pole;

        protected Image _keenLookUp = Properties.Resources.keen_look_up;
        protected Image _keenEnterDoor1 = Properties.Resources.keen_enter_door1;
        protected Image _keenStunned = Properties.Resources.keen_stunned;
        protected Image _keenStandright = Properties.Resources.keen_stand_right;
        protected Image _keenStandLeft = Properties.Resources.keen_stand_left;
        protected Image _keenFallRight = Properties.Resources.keen_fall_right;
        protected Image _keenFallLeft = Properties.Resources.keen_fall_left;
        protected Image _keenJumpLeft1 = Properties.Resources.keen_jump_left1;
        protected Image _keenJumpRight1 = Properties.Resources.keen_jump_right1;
        protected Image _keenHangLeft = Properties.Resources.keen_hang_left;
        protected Image _keenHangRight = Properties.Resources.keen_hang_right;

        protected Image _keenDead1 = Properties.Resources.keen_dead1;
        protected Image _keenDead2 = Properties.Resources.keen_dead2;

        #endregion

        List<NeuralStunner> _weapons;
        List<Gem> _gems;
        List<Flag> _flags;

        private Animation _keenRunRightAnimation;
        private Animation _keenRunLeftAnimation;
        private Animation _keenClimbLeftAnimation;
        private Animation _keenClimbRightAnimation;
        private Animation _currentAnimation;

        private bool _deathFalling = false;
        private int _maxDeathJump = 50;
        private int _currentDeathJump = 0;
        private bool _isFiring;
        private int _currentPoleClimbSprite;
        private int _currentLookDownSprite;

        private const int MAX_SHOT_DELAY = 4;
        private int _currentShotDelay = 0;
        private NeuralStunner _currentWeapon;
        private Door _currentDoor;
        private int _currentDoorWalkState;

        private const int ENTER_DOOR_SPRITE_CHANGE_DELAY = 0;
        private int _currentDoorSpriteDelayTick = 0;
        private bool _togglingSwitch;

        private int _drops = 0;
        private int _lives = 3;
        private const int DROPS_TO_EXTRA_LIFE = 100;
        private long _points;
        private long _pointsToNextExtraLife = 20000;
        private bool _hasKeyCard;
        private const int MAX_SHIELD_TOGGLE_DELAY = 3;
        private int _currentShieldToggleDelay = MAX_SHIELD_TOGGLE_DELAY;
        private Shield _shield;
        private bool _disappearDeath;


        #endregion

        #region DestructibleObject Implementation

        public override void TakeDamage(int damage)
        {
            bool isDead = this.IsDead();
            this.Health -= damage;
            if (this.Health <= 0)
            {
                if (!isDead)
                {
                    this.Lives--;
                    OnKeenDied(new ObjectEventArgs() { ObjectSprite = this });
                }

                this.Die();
            }
        }



        public override void Die()
        {
            if (!(_shield != null && _shield.IsActive) || this.HitBox.Top > _collisionGrid.Size.Height || _disappearDeath)
            {
                if (!this.IsDead())
                {
                    this.Lives--;
                    OnKeenDied(new ObjectEventArgs() { ObjectSprite = this });
                }
                this.Health = 0;
                _fallVelocity = 0;
                _currentDeathJump = 0;
                _deathFalling = false;
                SetDeathSprite();
                if (_shield != null)
                {
                    _shield.Depleted -= _shield_Depleted;
                }
                if (!_disappearDeath)
                {
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.KEEN_DIE);
                }
            }
        }

        private void SetDeathSprite()
        {
            Random random = new Random();
            int imgNum = random.Next(1, 3);

            if (imgNum == 1)
            {
                _sprite = _keenDead1; //Properties.Resources.keen_dead1;
            }
            else
            {
                _sprite = _keenDead2;//Properties.Resources.keen_dead2;
            }
        }

        protected override void HandleCollision(CollisionObject obj)
        {
            //base.HandleCollision(obj);
            if (obj.CollisionType == CollisionType.BLOCK)
            {
                if (obj is MapEdgeTile)
                {
                    var tile = (MapEdgeTile)obj;
                    if (tile.Behavior == MapEdgeBehavior.DEATH)
                    {
                        //kill keen if death tile
                        this.Die();
                    }
                    else if (tile.Behavior == MapEdgeBehavior.EXIT)
                    {
                        //raise keen passed level event
                        this.OnKeenLevelCompleted(new ObjectEventArgs() { ObjectSprite = this });
                    }
                }
                if (this.MoveState == Enums.MoveState.FALLING)
                {
                    //perform hang only if falling and colliding a wall keen is a higher y position than
                    if (obj.HitBox.Y > this.HitBox.Y)
                    {
                        int xOffsetWallCollide = this.Direction == Direction.LEFT
                            ? obj.HitBox.Right - this.HitBox.Width : obj.HitBox.Left + 1/*this.HitBox.Width*/;
                        Rectangle areaToCheck = new Rectangle(xOffsetWallCollide, obj.HitBox.Y - this.HitBox.Height, this.HitBox.Width, this.HitBox.Height);
                        var collisionWalls = obj.CheckCollision(areaToCheck).Where(c => c.CollisionType == CollisionType.BLOCK);
                        //we can't hang on a wall if there are floors below at a lower Y distance than keen's height 
                        Rectangle areaToCheck2 = this.Direction == Enums.Direction.RIGHT ?
                            new Rectangle(obj.HitBox.X - this.HitBox.Width, obj.HitBox.Y, this.HitBox.Width, this.HitBox.Height) :
                            new Rectangle(obj.HitBox.Right + this.HitBox.Width, obj.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                        var bottomCollisions = obj.CheckCollision(areaToCheck2).Where(c => c.CollisionType == CollisionType.BLOCK);
                        //check for overhead collisions
                        Rectangle overheadArea = this.Direction == Direction.RIGHT ?
                            new Rectangle(obj.HitBox.Left - 32, obj.HitBox.Y - 32, 32, 64) :
                            new Rectangle(obj.HitBox.Right + 32, obj.HitBox.Y - 32, 32, 64);
                        var overheadCollisions = this.CheckCollision(overheadArea, true).Where(c => c.CollisionType == CollisionType.BLOCK);

                        if (!collisionWalls.Any() && !bottomCollisions.Any() && !overheadCollisions.Any())
                        {
                            this.Hang(obj as IHangableTile);
                        }
                    }
                }
                else if (this.MoveState == Enums.MoveState.HANGING)
                {
                    if (_hangTile != null && obj.HitBox.Location == _hangTile.HitBox.Location)
                    {
                        if (_currentStartClimbDelayTick == START_CLIMB_TICKS)
                        {
                            this.Climb();
                            _currentStartClimbDelayTick = 0;
                        }
                    }
                }
            }
            else if (obj.CollisionType == CollisionType.HAZARD)
            {
                if (obj is MapEdgeTile)
                {
                    Die();
                    return;
                }
                var hazard = (Hazard)obj;
                if (hazard.IsDeadly)
                {
                    Die();
                    if (obj is Mine)
                    {
                        var mine = (Mine)obj;
                        if (!mine.IsExploded)
                        {
                            mine.Explode();
                        }
                    }
                }
            }
            else if (obj is Item)
            {
                var item = (Item)obj;
                if (!item.IsAcquired && item.CollectibleByPlayer)
                {
                    item.SetAcquired();
                    HandleItemCollection(item);
                }
            }
            else if (obj is ThunderCloud)
            {
                var cloud = (ThunderCloud)obj;
                if (cloud.IsLethal && cloud.MotionState == ThunderCloudMoveState.DORMANT)
                {
                    cloud.PursueKeen(this);
                }
            }
            else if (obj is IProjectile)
            {
                var trajectory = (IProjectile)obj;
                if (trajectory.KillsKeen)
                {
                    this.Die();
                }
            }
            else if (obj is IEnemy)
            {
                var enemy = (IEnemy)obj;
                if (enemy.DeadlyTouch)
                {
                    this.Die();
                }
            }
            else if (obj is GemPlaceHolder)
            {
                var placeHolder = obj as GemPlaceHolder;
                if ((this.MoveState == Enums.MoveState.STANDING || this.MoveState == Enums.MoveState.RUNNING
                    || this.MoveState == MoveState.FALLING)
                    && CanOpenGemGate(placeHolder))
                {
                    Gem gemToRemove = _gems.FirstOrDefault(g => g.Color == placeHolder.Color);
                    _gems.Remove(gemToRemove);
                    OnItemLost(new ItemAcquiredEventArgs() { Item = gemToRemove });
                    placeHolder.Toggle();
                }
            }
            else if (obj is Keen6Switch)
            {
                Keen6Switch keen6Switch = (Keen6Switch)obj;

                keen6Switch.OnCollide(this);
            }
            else if (obj is CTFDestination)
            {
                CTFDestination destination = (CTFDestination)obj;

                var flags = new List<Flag>(_flags.Where(f => f.Color == destination.Color));
                if (flags.Any())
                {
                    foreach (var flag in flags)
                    {
                        flag.Capture();
                        _flags.Remove(flag);
                        this.GiveFlagPoints(flag.CurrentPointValue);
                        OnItemLost(new ItemAcquiredEventArgs() { Item = flag });
                    }
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.FLAG_CAPTURED);
                }
            }
        }

        private bool CanOpenGemGate(GemPlaceHolder p)
        {
            if (!p.IsActive)
            {
                bool canOpen = _gems.Any(g => g.Color == p.Color);
                return canOpen;
            }
            return false;
        }

        public void HandleItemCollection(Item item)
        {
            if (item is NeuralStunnerAmmo)
            {
                var weapon = _weapons.FirstOrDefault(i => i.GetType().Name == item.GetType().Name.Replace("Ammo", "")) as NeuralStunner;
                int ammoAmmount = ((NeuralStunnerAmmo)item).AmmoAmount;
                if (weapon != null)
                {
                    weapon.Ammo += ammoAmmount;
                    OnWeaponChanged(new WeaponEventArgs() { Weapon = weapon });
                }
                else
                {
                    CreateWeaponAndSetToCurrentWeapon(ammoAmmount, item);
                }
                GenerateWeaponAcquiredSound(weapon);
            }
            else if (item is Gem)
            {
                var gem = item as Gem;
                _gems.Add(gem);
                OnKeenAcquiredItem(new ItemAcquiredEventArgs() { Item = gem });
                if (!this.IsDead())
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.KEEN_GEM_ACQUIRED);
            }
            else if (item is Flag)
            {
                var flag = item as Flag;
                _flags.Add(flag);
                OnKeenAcquiredItem(new ItemAcquiredEventArgs() { Item = flag });
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.FLAG_ACQUIRED);
            }
            else if (item is IDropCollector)
            {
                IDropCollector drop = item as IDropCollector;
                this.Drops += drop.DropVal;
                if (!this.IsDead())
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.KEEN_LIFE_DROP_ACQUIRED);
                if (_drops >= DROPS_TO_EXTRA_LIFE)
                {
                    this.Drops = 0;
                    GiveExtraLife();
                }
            }
            else if (item is ExtraLife)
            {
                GiveExtraLife();
            }
            else if (item is PointItem)
            {
                PointItem bonus = item as PointItem;
                GivePoints(bonus.PointValue);
                if (!this.IsDead())
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.KEEN_POINT_ACQUIRED);
            }
            else if (item is KeyCard)
            {
                this.HasKeyCard = true;
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.KEEN_KEY_CARD_ACQUIRED);
            }
            else if (item is Shield)
            {
                AcquireShield(item);
                OnShieldAcquired(new ObjectEventArgs() { ObjectSprite = _shield });
                _shield.Depleted += _shield_Depleted;
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.MAJOR_ITEM_ACQUIRED);
            }
        }

        private void GenerateWeaponAcquiredSound(NeuralStunner weapon)
        {
            if (!this.IsDead())
            {
                if ((weapon != null && weapon.IsSpecialWeapon)
                    || (weapon == null && CurrentWeapon != null && CurrentWeapon.IsSpecialWeapon))
                {
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.MAJOR_ITEM_ACQUIRED);
                }
                else
                {
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.KEEN_WEAPON_ACQUIRED);
                }
            }
        }

        private void _shield_Depleted(object sender, EventArgs e)
        {
            if (!this.HasShield || _shield.Duration <= 0)
                this.CheckSelfForDeathCollision();
        }

        private void AcquireShield(Item item)
        {
            var shield = (Shield)item;
            if (this.HasShield)
            {
                _shield.AddShieldToCurrent(shield);
                _shield.UpdateSpriteToSurroundKeen(this);
                shield.Deactivate();
            }
            else
            {
                _shield = shield;
                _shield.Deactivate();
            }
        }

        private void CreateWeaponAndSetToCurrentWeapon(int ammoAmmount, Item AmmoType)
        {
            if (AmmoType is ShotgunNeuralStunnerAmmo)
            {
                ShotgunNeuralStunner stunner = new ShotgunNeuralStunner(_collisionGrid, this.HitBox, ammoAmmount);
                RegisterNewWeapon(stunner);
            }
            else if (AmmoType is SMGNeuralStunnerAmmo)
            {
                SMGNeuralStunner stunner = new SMGNeuralStunner(_collisionGrid, this.HitBox, ammoAmmount);
                RegisterNewWeapon(stunner);
            }
            else if (AmmoType is RailgunNeuralStunnerAmmo)
            {
                RailgunNeuralStunner stunner = new RailgunNeuralStunner(_collisionGrid, this.HitBox, ammoAmmount);
                RegisterNewWeapon(stunner);
            }
            else if (AmmoType is RPGNeuralStunnerAmmo)
            {
                RPGNeuralStunner stunner = new RPGNeuralStunner(_collisionGrid, this.HitBox, ammoAmmount);
                RegisterNewWeapon(stunner);
            }
            else if (AmmoType is BoobusBombLauncherAmmo)
            {
                BoobusBombLauncher stunner = new BoobusBombLauncher(_collisionGrid, this.HitBox, ammoAmmount);
                RegisterNewWeapon(stunner);
            }
            else if (AmmoType is BFGAmmo)
            {
                BFG stunner = new BFG(_collisionGrid, this.HitBox, ammoAmmount);
                RegisterNewWeapon(stunner);
            }
            else if (AmmoType is SnakeGunAmmo)
            {
                SnakeGun stunner = new SnakeGun(_collisionGrid, this.HitBox, ammoAmmount);
                RegisterNewWeapon(stunner);
            }
            else
            {
                NeuralStunner stunner = new NeuralStunner(_collisionGrid, this.HitBox, ammoAmmount);
                _weapons.Add(stunner);
                this.CurrentWeapon = stunner;
            }
        }

        private void RegisterNewWeapon(NeuralStunner stunner)
        {
            _weapons.Add(stunner);
            RegisterProjectileEvents(stunner);
            this.CurrentWeapon = stunner;
            OnKeenAcquiredWeapon(new WeaponEventArgs() { Weapon = _currentWeapon });
        }

        public void GivePoints(long value)
        {
            this.Points += value;
            if (this.Points >= _pointsToNextExtraLife)
            {
                _pointsToNextExtraLife *= 2;
                GiveExtraLife();
            }
        }

        public void GiveFlagPoints(long points)
        {
            if (points < _pointsToNextExtraLife)
                this.GivePoints(points);
            else
            {
                this.Points += points;
                while (this.Points >= _pointsToNextExtraLife)
                {
                    this.GiveExtraLife();
                    _pointsToNextExtraLife *= 2;
                }
            }
        }

        public void MoveKeenToPosition(Point p, CollisionObject collidingObject)
        {
            if (this.MoveState == MoveState.ENTERING_DOOR)
                return;

            Rectangle areaToCheck = new Rectangle(p, this.HitBox.Size);
            Point previousDir = this.HitBox.Location;
            var collisions = this.CheckCollision(areaToCheck, true);
            collisions = collisions.Where(c => !(c.CollisionType == CollisionType.PLATFORM) && c != collidingObject).ToList();
            if (!collisions.Any())
            {
                Rectangle previousPos = this.HitBox;
                this.HitBox = areaToCheck;
                //update original standing hitbox of looking down
                if (_originalStandingHitbox != null)
                {
                    _originalStandingHitbox = new Rectangle(this.HitBox.X, _originalStandingHitbox.Value.Y, _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height);
                    _originalStandingHitbox = new Rectangle(_originalStandingHitbox.Value.X, _originalStandingHitbox.Value.Y - (previousDir.Y - this.HitBox.Y), _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height);
                }

                if (p.X < previousPos.X)
                {
                    this.UpdateCollisionNodes(Enums.Direction.LEFT);
                }
                else if (p.X > this.HitBox.X)
                {
                    this.UpdateCollisionNodes(Enums.Direction.RIGHT);
                }

                if (p.Y < previousPos.Y)
                {
                    this.UpdateCollisionNodes(Enums.Direction.UP);
                }
                else if (p.Y > previousPos.Y)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN);
                }
                var newCollisions = this.CheckCollision(this.HitBox);
                foreach (var collision in newCollisions)
                {
                    this.HandleCollision(collision);
                }
            }
        }

        public void GetMovedVertically(CollisionObject obj)
        {
            if (this.MoveState == MoveState.ENTERING_DOOR)
                return;

            Point previousDir = this.HitBox.Location;

            int distanceMoved = Math.Abs(this.HitBox.Bottom - obj.HitBox.Top);
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Y - (this.HitBox.Bottom - obj.HitBox.Top), this.HitBox.Width, this.HitBox.Height + distanceMoved);
            var collisions = this.CheckCollision(areaToCheck);
            if (this.HitBox.Bottom < obj.HitBox.Top)
            {
                int landingYPos = GetTopMostLandingPlatformYPos(collisions);
                if (landingYPos != -1)
                {
                    //set keen to stand one pixel higher than the top of the floor he fell onto
                    this.HitBox = new Rectangle(new Point(this.HitBox.Location.X, landingYPos - this.HitBox.Height - 1), this.HitBox.Size);
                    //update collision nodes
                    this.UpdateCollisionNodes(Enums.Direction.DOWN);
                    //update original standing hitbox of looking down
                    if (_originalStandingHitbox != null)
                    {
                        _originalStandingHitbox = new Rectangle(_originalStandingHitbox.Value.X, _originalStandingHitbox.Value.Y + (this.HitBox.Y - previousDir.Y), _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height);
                    }
                    //handle collisions
                    foreach (var item in collisions)
                    {
                        if (item.HitBox.Y < landingYPos)
                            this.HandleCollision(item);
                    }
                    return;
                }
            }
            else
            {
                //get lowest ceiling tile
                var ceilingTile = GetCeilingTile(collisions);
                if (ceilingTile != null)//if we hit something
                {
                    //update position
                    this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Bottom + 1, this.HitBox.Width, this.HitBox.Height);
                    //update collisions
                    this.UpdateCollisionNodes(Enums.Direction.UP);
                    //update original standing hitbox of looking down
                    if (_originalStandingHitbox != null)
                    {
                        _originalStandingHitbox = new Rectangle(_originalStandingHitbox.Value.X, _originalStandingHitbox.Value.Y - (previousDir.Y - this.HitBox.Y), _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height);
                    }
                    //handle collisions
                    foreach (var collision in collisions)
                    {
                        if (collision.HitBox.Bottom > ceilingTile.HitBox.Bottom)
                        {
                            this.HandleCollision(collision);
                        }
                    }
                    return;
                }
            }
            foreach (var collision in collisions)
            {
                this.HandleCollision(collision);
            }
            //if no collision, keep going as previously implemented
            this.HitBox = new Rectangle(this.HitBox.X, obj.HitBox.Top - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
            if (this.HitBox.Y < previousDir.Y)
            {
                if (_originalStandingHitbox != null)
                {
                    _originalStandingHitbox = new Rectangle(_originalStandingHitbox.Value.X, _originalStandingHitbox.Value.Y - (previousDir.Y - this.HitBox.Y), _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height);
                }
                this.UpdateCollisionNodes(Enums.Direction.UP);
            }
            else if (this.HitBox.Y > previousDir.Y)
            {
                if (_originalStandingHitbox != null)
                {
                    _originalStandingHitbox = new Rectangle(_originalStandingHitbox.Value.X, _originalStandingHitbox.Value.Y + (this.HitBox.Y - previousDir.Y), _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height);
                }
                this.UpdateCollisionNodes(Enums.Direction.DOWN);
            }

        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => (c.CollisionType == CollisionType.BLOCK || (c.CollisionType == CollisionType.KEEN6_SWITCH && !((Keen6Switch)c).IsActive)) && c.HitBox.Bottom < this.HitBox.Bottom - 2).ToList();
            if (debugTiles.Any())
            {
                int maxBottom = debugTiles.Select(c => c.HitBox.Bottom).Max();
                CollisionObject obj = collisions.FirstOrDefault(c => c.HitBox.Bottom == maxBottom);
                return obj;
            }
            return null;
        }


        public void GetMovedHorizontally(CollisionObject obj, Direction direction, int speed)
        {
            if (this.MoveState == MoveState.ENTERING_DOOR)
            {
                ClearPushState();
                return;
            }
            bool isLeftDirection = IsLeftDirection(direction);
            int xOffset = isLeftDirection ? speed * -1 : speed;
            int xLocation = isLeftDirection ? this.HitBox.X - speed : this.HitBox.X;
            Rectangle areaToCheck = new Rectangle(xLocation, this.HitBox.Y, this.HitBox.Width + speed, this.HitBox.Height);
            var collisions = this.CheckCollision(areaToCheck);
            var hitTile = isLeftDirection ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
            if (hitTile == null)
            {
                this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                foreach (var collision in collisions)
                {
                    this.HandleCollision(collision);
                }
            }
            else
            {
                int xCollidePos = isLeftDirection ? hitTile.HitBox.Right + 1 : hitTile.HitBox.Left - this.HitBox.Width - 1;
                foreach (var collision in collisions)
                {
                    if (isLeftDirection && collision.HitBox.Right > xCollidePos)
                    {
                        this.HandleCollision(collision);
                    }
                    else if (!isLeftDirection && collision.HitBox.Left < xCollidePos)
                    {
                        this.HandleCollision(collision);
                    }
                }
                this.HitBox = isLeftDirection ?
                    new Rectangle(hitTile.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height) :
                    new Rectangle(hitTile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
            }
            if (xOffset < 0)
            {
                this.UpdateCollisionNodes(Enums.Direction.LEFT);
            }
            else if (xOffset > 0)
            {
                this.UpdateCollisionNodes(Enums.Direction.RIGHT);
            }
            if (_originalStandingHitbox != null)
            {
                _originalStandingHitbox = new Rectangle(this.HitBox.X, _originalStandingHitbox.Value.Y, _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height);
            }
        }

        private void GiveExtraLife()
        {
            this.Lives++;
            if (!this.IsDead())
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.KEEN_1UP);
        }

        private void Climb()
        {
            if (_currentClimbDelayTicks == CLIMB_DELAY_TICKS)
            {
                _currentClimbDelayTicks = 0;
                this.MoveState = Enums.MoveState.CLIMBING;
                int lengthLimit = this.Direction == Enums.Direction.RIGHT ? _keenClimbRightSprites.Length - 1
                    : _keenClimbLeftSprites.Length - 1;

                if (_currentClimbSprite < 2)
                    this.HitBox = new Rectangle(new Point(_direction == Enums.Direction.RIGHT ? _hangTile.HitBox.X : _hangTile.HitBox.Right - this.HitBox.Width, _hangTile.HitBox.Y - 40),
                       this.HitBox.Size);
                else if (_currentClimbSprite == 2)
                    this.HitBox = new Rectangle(new Point(_direction == Enums.Direction.RIGHT ? _hangTile.HitBox.X : _hangTile.HitBox.Right - this.HitBox.Width, _hangTile.HitBox.Y - 50),
                      this.HitBox.Size);
                else
                    this.HitBox = new Rectangle(new Point(_direction == Enums.Direction.RIGHT ? _hangTile.HitBox.X : _hangTile.HitBox.Right - this.HitBox.Width, _hangTile.HitBox.Y - this.HitBox.Height - 1),
                        this.HitBox.Size);

                UpdateAndCheckCollision();
                if (_currentClimbSprite == lengthLimit)
                {
                    _currentClimbSprite = 0;
                    this.MoveState = Enums.MoveState.STANDING;
                    _climbReady = false;
                }
                else
                {
                    _currentClimbSprite++;
                    UpdateSprite();
                }
            }
            else
            {
                _currentClimbDelayTicks++;
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
                if (_currentWeapon != null)
                {
                    _currentWeapon.HitBox = this.HitBox;
                }
                if (_shield != null)
                {
                    _shield.UpdateSpriteToSurroundKeen(this);
                }
                OnKeenMoved(new EventArgs());
            }
        }

        #endregion

        #region IFireable Implementation
        public void Fire()
        {
            if (_shield == null || !_shield.IsActive)
            {
                if (!_isFiring || (_currentWeapon != null && _currentWeapon.IsAuto))
                {
                    this.IsFiring = true;
                    _currentShotDelay = _currentWeapon != null ? _currentWeapon.RefireDelay : 0;
                    if (_currentWeapon != null && _currentWeapon.Ammo > 0)
                    {
                        _currentWeapon.Fire();
                        if (_currentWeapon.Ammo <= 0)
                            SetBackupWeapon();
                    }
                    else if (_currentWeapon != null)
                    {
                        SetBackupWeapon();
                    }
                    OnWeaponChanged(new WeaponEventArgs() { Weapon = _currentWeapon });
                }
            }
        }

        private void SetBackupWeapon()
        {
            OnWeaponChanged(new WeaponEventArgs() { Weapon = this.CurrentWeapon });
            var backupWeapon = _weapons.LastOrDefault(w => w.Ammo > 0);
            if (backupWeapon == null)
            {
                this.CurrentWeapon = _weapons.FirstOrDefault();
            }
            else
            {
                this.CurrentWeapon = backupWeapon;
            }
        }

        public bool IsFiring
        {
            get { return _isFiring; }
            private set
            {
                _isFiring = value;
                UpdateSprite();
            }
        }
        #endregion

        #region IGravityObject Implementation

        public void Jump()
        {
            //1. set movestate to jumping to update sprites
            this.MoveState = Enums.MoveState.JUMPING;
            //2. check for collisions above
            Rectangle areaToCheck = new Rectangle(this.HitBox.Left, this.HitBox.Top - JUMP_VELOCITY, this.HitBox.Width, JUMP_VELOCITY);
            var collisionItems = this.CheckCollision(areaToCheck);
            //3. If any collision nodes, take the one with the lowest bottom y pos and set the position of keen equal to that bottom plus one
            var collisionWalls = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK);
            var collisionKeen6Switches = collisionItems.OfType<Keen6Switch>()?.Where(s => !s.IsActive) ?? new List<Keen6Switch>();
            if (collisionWalls.Any() || collisionKeen6Switches.Any())
            {
                //get lowest bottom position colliding with keen
                int lowestBottomDebugTiles = collisionWalls.Any() ? collisionWalls.Select(c => c.HitBox.Bottom).Max() : -1;
                int lowestBottomKeen6Switches = collisionKeen6Switches.Any() ? collisionKeen6Switches.Select(c => c.HitBox.Bottom).Max() : -1;

                int lowestBottom = lowestBottomDebugTiles > lowestBottomKeen6Switches ? lowestBottomDebugTiles : lowestBottomKeen6Switches;

                //set keen to be one pixel lower than the bottom of the colliding block
                this.HitBox = new Rectangle(new Point(this.HitBox.Location.X, lowestBottom + 1), this.HitBox.Size);
                //set jump height to 0 and set keen to falling state
                SetToFallStateFromJumpState();
                //update collision nodes
                this.UpdateCollisionNodes(Enums.Direction.UP);
                //handle collisions
                foreach (var item in collisionItems)
                {
                    if (item.HitBox.Bottom > lowestBottom)
                        this.HandleCollision(item);
                    else if (item.HitBox.Bottom == lowestBottomDebugTiles && item is MapEdgeTile)
                        this.HandleCollision(item);
                }
                //toggle the keen 6 switch if hit
                if (lowestBottom == lowestBottomKeen6Switches && collisionKeen6Switches.Any())
                {
                    var objSwitch = collisionKeen6Switches.FirstOrDefault(s => s.HitBox.Bottom == lowestBottomKeen6Switches);
                    if (objSwitch != null)
                    {
                        this.HandleCollision(objSwitch);
                    }
                }
            }
            else
            {
                int maxJumpHeight = _jumpFromPole ? MAX_POLE_JUMP_HEIGHT : MAX_JUMP_HEIGHT;
                //increment jump height by jump velocity if we haven't reached max jump height.
                //Else, set jump height to zero and set keen to a falling state
                if (_currentJumpHeight < maxJumpHeight)
                {
                    //4. If no collisions, keep the jumping state and increase the jump height by the jump velocity
                    this.HitBox = new Rectangle(new Point(this.HitBox.Location.X, this.HitBox.Location.Y - JUMP_VELOCITY), this.HitBox.Size);
                    _currentJumpHeight += JUMP_VELOCITY;
                }
                else
                {
                    SetToFallStateFromJumpState();
                }
                //update collision nodes
                this.UpdateCollisionNodes(Enums.Direction.UP);
                foreach (var item in collisionItems)
                {
                    this.HandleCollision(item);
                }
            }
        }

        public bool CanJump
        {
            get
            {
                return (this.MoveState == Enums.MoveState.STANDING
                    || this.MoveState == Enums.MoveState.RUNNING
                    || this.MoveState == Enums.MoveState.ON_PLATFORM
                    || this.MoveState == Enums.MoveState.ON_POLE
                    || this.MoveState == Enums.MoveState.JUMPING)
                    && !(_isFiring && (this.MoveState == Enums.MoveState.RUNNING || this.MoveState == Enums.MoveState.STANDING || this.MoveState == Enums.MoveState.ON_POLE || this.MoveState == Enums.MoveState.ON_PLATFORM))
                    && !_isLookingDown;
            }
        }

        public bool CanShoot
        {
            get
            {
                return this.MoveState != Enums.MoveState.CLIMBING && this.MoveState != Enums.MoveState.HANGING && !this.IsStunned
                    && !_isLookingDown && _currentShotDelay == 0;
            }
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            CollisionObject topMostTile = null;
            var landingTiles = collisions.Where(h => (h.CollisionType == CollisionType.BLOCK
            || h.CollisionType == CollisionType.PLATFORM
            || h.CollisionType == CollisionType.KEEN_ONLY_PLATFORM
            || h.CollisionType == CollisionType.POLE_TILE
            || (h.CollisionType == CollisionType.KEEN6_SWITCH && ((Keen6Switch)h).IsActive))
                && h.HitBox.Top >= this.HitBox.Top);

            if (!landingTiles.Any())
                return null;

            int minY = landingTiles.Select(c => c.HitBox.Top).Min();
            topMostTile = landingTiles.FirstOrDefault(t => t.HitBox.Top == minY);

            return topMostTile;
        }

        public void Fall()
        {
            _jumpReady = false;
            _currentJumpHeight = 0;
            //SET MOVESTATE TO FALLING
            this.MoveState = Enums.MoveState.FALLING;
            //1. Take Current Fall velocity variable and create an area to check just beneath the foot of the character 
            //1.A current fall velocity is the height of the area to check and width is the width of the character 
            //1.B x = character xpos and y is the the bottom of the characters hitbox plus one (this is to avoid screwing up left/right collision detection)
            Rectangle areaToCheck = new Rectangle(this.HitBox.Left, this.HitBox.Bottom, this.HitBox.Width, _fallVelocity);
            //2. Check collision nodes for colliding objects
            var collisionItems = this.CheckCollision(areaToCheck);
            //3. If any collision nodes, take the one with the highest y pos and set the position of keen equal to that y plus the height of keen's hitbox plus one

            //get min y position colliding with keen
            int minY = GetTopMostLandingPlatformYPos(collisionItems);
            if (minY != -1) //if we are going to land on something
            {
                //if it is a moving platform tile, update the tile's status
                var landingMovingPlatformTile = GetTopMostLandingTile(collisionItems);
                if (landingMovingPlatformTile != null && landingMovingPlatformTile is MovingPlatformTile)
                {
                    ((MovingPlatformTile)landingMovingPlatformTile).AssignKeen(this);
                }
                //set keen to stand one pixel higher than the top of the floor he fell onto
                this.HitBox = new Rectangle(new Point(this.HitBox.Location.X, minY - this.HitBox.Height - 1), this.HitBox.Size);
                //stop keen from falling
                _fallVelocity = INITIAL_FALL_VELOCITY;
                this.MoveState = Enums.MoveState.STANDING;
                //update collision nodes
                this.UpdateCollisionNodes(Enums.Direction.DOWN);
                //handle collisions
                foreach (var item in collisionItems)
                {
                    if (item.HitBox.Y < minY)
                        this.HandleCollision(item);
                    else if (item is MapEdgeTile)
                    {
                        this.HandleCollision(item);
                    }
                    else if (item is Keen6Switch)
                    {
                        var keen6Switch = (Keen6Switch)item;
                        if (keen6Switch.IsActive && item.HitBox.Y == minY)
                        {
                            this.HandleCollision(item);
                        }
                    }
                }
                //squash things that can be squashed
                var squashables = collisionItems.OfType<ISquashable>();
                if (squashables.Any())
                {
                    foreach (ISquashable i in squashables)
                    {
                        if (i.CanSquash)
                        {
                            CollisionObject obj = i as CollisionObject;
                            if (obj != null && (obj.HitBox.Y < minY || minY == -1))
                                i.Squash();
                        }
                    }
                }

                //gem placeholders
                var gemPlaceholders = collisionItems.OfType<GemPlaceHolder>();
                if (gemPlaceholders.Any())
                {
                    foreach (var placeholder in gemPlaceholders)
                    {
                        this.HandleCollision(placeholder);
                    }
                }
            }
            else
            {
                //4. If no collisions, keep the falling state and increment the fall velocity by the constant fall acceleration
                //set location to new Y point 
                this.HitBox = new Rectangle(new Point(this.HitBox.Location.X, this.HitBox.Location.Y + _fallVelocity), this.HitBox.Size);
                //increment fall velocity by constant rate of acceleration
                if (_fallVelocity + FALL_ACCELLERATION <= MAX_FALL_VELOCITY)
                    _fallVelocity += FALL_ACCELLERATION;
                //update collision nodes
                this.UpdateCollisionNodes(Enums.Direction.DOWN);
                foreach (var item in collisionItems)
                {
                    this.HandleCollision(item);
                }
            }
        }

        #endregion

        #region IMoveable Implementation
        public void Move()
        {

        }

        public void Stop() { }

        public Enums.MoveState MoveState
        {
            get { return _moveState; }

            set
            {
                _moveState = value;
                if (!this.CanLookUp)
                    StopLookUp();
                if (!this.CanLookDown)
                {
                    StopLookUp();
                }

                //SetCorrectTimersFromMoveState();
                UpdateSprite();
                //SetMiscellaneousMovementVariables();
            }
        }

        public Direction Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;

                UpdateSprite();
            }
        }

        private void UpdateWeaponFireDirectionAndLocation()
        {
            if (_currentWeapon != null)
            {
                if (IsKeyPressed(KEY_UP))
                {
                    _currentWeapon.Direction = Enums.Direction.UP;
                }
                else if (IsKeyPressed(KEY_DOWN))
                {
                    _currentWeapon.Direction = Enums.Direction.DOWN;
                }
                else
                {
                    _currentWeapon.Direction = this.Direction;
                }
                if (_currentWeapon is BoobusBombLauncher)
                {
                    var launcher = (BoobusBombLauncher)_currentWeapon;
                    launcher.KeenStandDirection = this.Direction;
                }

                _currentWeapon.HitBox = this.HitBox;
            }
        }

        public Direction PoleDirection
        {
            get
            {
                return _poleDirection;
            }
            set
            {
                _poleDirection = value;
                UpdateSprite();
            }
        }

        #endregion

        #region IStunnable Implementation

        public void Stun()
        {
            _stunTimeTick = 0;
            this.MoveState = Enums.MoveState.STUNNED;
        }

        public bool IsStunned
        {
            get { return this.MoveState == Enums.MoveState.STUNNED; }
        }
        #endregion

        #region ISprite Implementation
        public Image Sprite
        {
            get { return _sprite; }
        }

        public bool CanUpdate => true;
        #endregion

        public NeuralStunner CurrentWeapon
        {
            get
            {
                return _currentWeapon;
            }
            private set
            {
                _currentWeapon = value;
                WeaponEventArgs e = new WeaponEventArgs()
                {
                    Weapon = _currentWeapon
                };
                OnWeaponChanged(e);
            }
        }

        public bool HasKeyCard
        {
            get
            {
                return _hasKeyCard;
            }
            private set
            {
                if (value != _hasKeyCard)
                {
                    _hasKeyCard = value;
                    ObjectEventArgs e = new ObjectEventArgs()
                    {
                        ObjectSprite = this
                    };
                    OnKeyCardAcquisitionStateChanged(e);
                }
            }
        }

        public bool HasCollisionNodes
        {
            get
            {
                return _collidingNodes != null && _collidingNodes.Any();
            }
        }

        public bool HasShield
        {
            get
            {
                return _shield != null;
            }
        }

        public List<Gem> Gems
        {
            get
            {
                return _gems;
            }
        }

        public List<NeuralStunner> Weapons
        {
            get
            {
                return _weapons;
            }
        }

        public bool IsKeenMoving
        {
            get;
            set;
        }

        public bool IsLookingDown
        {
            get
            {
                return _isLookingDown;
            }
        }

        public bool IsLookingUp
        {
            get
            {
                return _isLookingUp;
            }
        }

        public Direction HangClimbDirection
        {
            get
            {
                return _hangAndClimbDirection;
            }
        }

        private void Hang(IHangableTile tile)
        {
            if (tile != null && tile.Hangable)
            {
                int newX = this.Direction == Enums.Direction.RIGHT ? tile.HitBox.Left - this.HitBox.Width - 1 : tile.HitBox.Right + 1;
                this.HitBox = new Rectangle(new Point(newX, tile.HitBox.Y), this.HitBox.Size);
                this.UpdateCollisionNodes(this.Direction);
                this.MoveState = Enums.MoveState.HANGING;
                _hangTile = tile;
            }
        }

        private List<string> GetWeaponKeys()
        {
            return new List<string>() { KEY_1, KEY_2, KEY_3, KEY_4, KEY_5, KEY_6, KEY_7, KEY_8 };
        }

        private int ParseWeaponNumberFromKey(string key)
        {
            try
            {
                return Convert.ToInt32(key.Substring(1));
            }
            catch
            {
                return 0;
            }
        }

        private void SetWeaponByKey()
        {
            var keys = GetWeaponKeys();
            foreach (string key in keys)
            {
                if (IsKeyPressed(key))
                {
                    int weaponKey = ParseWeaponNumberFromKey(key) - 1;
                    if (weaponKey < _weapons.Count && _weapons[weaponKey] != null && _weapons[weaponKey].Ammo > 0)
                    {
                        this.CurrentWeapon = _weapons[weaponKey];
                    }
                }
            }
        }

        public void ResetKeenAfterDeath(int lives, int drops, long points)
        {
            this.Lives = lives;
            this.Points = points;
            this.Drops = drops;
            _disappearDeath = false;

            OnItemLost(new ItemAcquiredEventArgs() { Item = _shield });
            _shield = null;
            _currentShieldToggleDelay = MAX_SHIELD_TOGGLE_DELAY;
            SetNextExtraLifePointGoal();
            this.HasKeyCard = false;
            InitializeWeapons();
            while (_gems.Any())
            {
                var gemToRemove = _gems.FirstOrDefault();
                _gems.Remove(gemToRemove);
                OnItemLost(new ItemAcquiredEventArgs() { Item = gemToRemove });
            }
        }

        public void ResetKeenAfterDeath(int lives, int drops, long points, List<NeuralStunner> weaponSet, Shield shield)
        {
            this.Lives = lives;
            this.Points = points;
            this.Drops = drops;
            _disappearDeath = false;
            SetNextExtraLifePointGoal();
            this.HasKeyCard = false;
            _currentShieldToggleDelay = MAX_SHIELD_TOGGLE_DELAY;
            _shield = shield;
            if (shield != null)
                this.HandleItemCollection(shield);

            InitializeWeapons(weaponSet);
            while (_gems.Any())
            {
                var gemToRemove = _gems.FirstOrDefault();
                _gems.Remove(gemToRemove);
                OnItemLost(new ItemAcquiredEventArgs() { Item = gemToRemove });
            }
        }

        public void Update()
        {
            if (!this.IsDead())
            {
                if (_failedExitDoorAccess && _failedAccessSoundDelayTick++ >= FAILED_ACCESS_WAIT_TIME)
                {
                    _failedExitDoorAccess = false;
                    _failedAccessSoundDelayTick = 0;
                }
                if (this.HitBox.Top > _collisionGrid.Size.Height)
                {
                    this.Die();
                    return;
                }
                SetWeaponByKey();
                CheckForSwitchWeaponButtonPress();
                CheckForShieldToggle();
                if (this.MoveState != Enums.MoveState.CLIMBING && this.MoveState != Enums.MoveState.ENTERING_DOOR)
                {
                    bool isNothingBeneath = IsNothingBeneathKeen();
                    if (isNothingBeneath
                        && (this.MoveState != Enums.MoveState.JUMPING && this.MoveState != Enums.MoveState.HANGING))
                    {
                        this.Fall();
                    }
                    else if (!isNothingBeneath && this.MoveState == Enums.MoveState.FALLING)
                    {
                        this.MoveState = Enums.MoveState.STANDING;
                    }

                    if (this.MoveState == Enums.MoveState.HANGING)
                    {
                        if (_currentStartClimbDelayTick < START_CLIMB_TICKS)
                        {
                            _currentStartClimbDelayTick++;
                        }
                    }

                    if (!this.IsStunned)
                    {
                        if (IsKeyPressed(KEY_RIGHT) && !IsKeyPressed(KEY_LEFT))
                        {
                            TryMoveRight();
                        }
                        else if (IsKeyPressed(KEY_LEFT) && !IsKeyPressed(KEY_RIGHT))
                        {
                            TryMoveLeft();
                        }
                        else if ((!IsKeyPressed(KEY_LEFT) && !IsKeyPressed(KEY_RIGHT))
                              || (IsKeyPressed(KEY_LEFT) && IsKeyPressed(KEY_RIGHT)))
                        {
                            if (this.MoveState == Enums.MoveState.RUNNING)
                            {
                                this.MoveState = Enums.MoveState.STANDING;
                            }
                        }
                    }
                    else if (_stunTimeTick++ == STUN_TIME)
                    {
                        this.MoveState = Enums.MoveState.STANDING;
                        // _sprite.Location = this.HitBox.Location;
                    }
                    else if (_stunTimeTick == 1)
                    {
                        int spriteHeight = _sprite.Height;
                        int hitboxHeight = this.HitBox.Height;
                        // _sprite.Location = new Point(_sprite.Location.X, this.HitBox.Y + (hitboxHeight - spriteHeight));
                    }

                    if (IsKeyPressed(KEY_SPACE) && !this.IsStunned)
                    {
                        TryShoot();
                    }
                    else if (this.IsFiring)
                    {
                        this.IsFiring = false;
                    }
                    //update weapon delays even when not in use
                    foreach (var weapon in _weapons)
                    {
                        weapon.Update();
                    }
                    //set the weapon delay to the current weapons delay tick
                    _currentShotDelay = _currentWeapon != null ? _currentWeapon.CurrentDelayTick : 0;

                    if (IsKeyPressed(KEY_CTRL))
                    {
                        if (_isLookingDown && IsPlatformBeneathKeen())
                        {
                            StopLookDown();
                            this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + 20, this.HitBox.Width, this.HitBox.Height);
                            UpdateCollisionNodes(Enums.Direction.DOWN);
                            UpdateSprite();
                            this.Fall();
                        }
                        else
                        {
                            TryJump();
                        }
                    }
                    else if (this.MoveState == Enums.MoveState.JUMPING && !IsKeyPressed(KEY_CTRL))
                    {
                        SetToFallStateFromJumpState();
                    }
                    else if ((this.MoveState == Enums.MoveState.STANDING || this.MoveState == Enums.MoveState.RUNNING || this.MoveState == Enums.MoveState.ON_POLE)
                             && !IsKeyPressed(KEY_CTRL)
                             && this.CanJump)
                    {
                        _jumpReady = true;
                        _jumpFromPole = this.MoveState == Enums.MoveState.ON_POLE;
                    }

                    if (!IsKeyPressed(KEY_UP) && _togglingSwitch)
                    {
                        _togglingSwitch = false;
                        UpdateSprite();
                    }

                    if (IsKeyPressed(KEY_UP) && !IsKeyPressed(KEY_DOWN))
                    {
                        if (CanKeenGrabPole())
                        {
                            if (this.MoveState != Enums.MoveState.ON_POLE)
                                HangOntoPole();
                            else
                                MoveUpPole();
                        }
                        else if (this.CanLookUp)
                        {
                            var door = GetFirstOrCollidingDoor();
                            var toggleSwitch = GetFirstCollidingSwitch();
                            if (door != null && toggleSwitch == null)
                            {
                                TryEnterDoor(door);
                            }
                            else if (toggleSwitch != null && !(toggleSwitch is GemPlaceHolder))
                            {
                                if (!_togglingSwitch)
                                {
                                    _togglingSwitch = true;
                                    toggleSwitch.Toggle();
                                }
                                UpdateSprite();
                            }
                            else
                            {
                                TryLookUp();
                            }
                        }
                    }
                    else if (IsKeyPressed(KEY_DOWN) && !IsKeyPressed(KEY_UP))
                    {
                        if (this.MoveState == Enums.MoveState.ON_POLE)
                        {
                            this.PoleDirection = Enums.Direction.DOWN;
                            if (!CanKeenGrabPole())
                            {
                                ResetPoleMovement();
                                this.Fall();
                            }
                            else
                            {
                                MoveDownPole();
                            }
                        }
                        else if (this.CanLookDown)
                        {
                            TryLookDown();
                        }
                        else
                        {
                            StopLookDown();
                        }
                    }
                    else if (this.MoveState == Enums.MoveState.ON_POLE)
                    {
                        ResetPoleMovement();
                    }
                    else if (_isLookingUp)
                    {
                        StopLookUp();
                    }
                    else if (_isLookingDown)
                    {
                        StopLookDown();
                    }
                    UpdateWeaponFireDirectionAndLocation();
                }
                else if (this.MoveState == Enums.MoveState.ENTERING_DOOR)
                {
                    if (_currentDoor is ExitDoor)
                    {
                        WalkThroughExit();
                    }
                    else
                    {
                        WalkThroughDoor();
                    }
                }
                else
                {
                    this.Climb();
                }
            }
            else
            {
                ContinueDeathSequence();
            }
        }

        private void CheckForShieldToggle()
        {
            if (++_currentShieldToggleDelay >= MAX_SHIELD_TOGGLE_DELAY)
            {
                _currentShieldToggleDelay = MAX_SHIELD_TOGGLE_DELAY;
                if (IsKeyPressed(KEY_SHIELD) && _shield != null)
                {
                    _currentShieldToggleDelay = 0;
                    if (_shield.IsActive)
                    {
                        _shield.Deactivate();
                        CheckSelfForDeathCollision();
                    }
                    else
                    {
                        _shield.TryActivate();
                    }
                }
            }
        }

        private void CheckSelfForDeathCollision()
        {
            var collisions = this.CheckCollision(this.HitBox);
            if (collisions.OfType<IEnemy>().Any(e => e.DeadlyTouch && e.IsActive)
              || collisions.OfType<Hazard>().Any(h => h.IsDeadly))
            {
                this.Die();
            }
        }

        private void CheckForSwitchWeaponButtonPress()
        {
            if (IsKeyPressed(KEY_SHIFT) && _currentWeaponRotateDelayTick == WEAPON_ROTATE_DELAY)
            {
                RotateWeaponForward();
            }
            else if (IsKeyPressed(KEY_ENTER) && _currentWeaponRotateDelayTick == WEAPON_ROTATE_DELAY)
            {
                RotateWeaponBackward();
            }
            else if (_currentWeaponRotateDelayTick < WEAPON_ROTATE_DELAY)
            {
                _currentWeaponRotateDelayTick++;
            }
        }

        private void RotateWeaponBackward()
        {
            if (this.CurrentWeapon != null && _weapons.Any(w => w.Ammo > 0))
            {
                int index = _weapons.IndexOf(this.CurrentWeapon);
                do
                {
                    if (index == 0)
                    {
                        index = _weapons.Count - 1;
                    }
                    else
                    {
                        index--;
                    }
                    this.CurrentWeapon = _weapons[index];
                } while (_weapons[index].Ammo <= 0);
                _currentWeaponRotateDelayTick = 0;
            }
        }

        private void RotateWeaponForward()
        {
            if (this.CurrentWeapon != null && _weapons.Any(w => w.Ammo > 0))
            {

                int index = _weapons.IndexOf(this.CurrentWeapon);
                do
                {
                    if (index == _weapons.Count - 1)
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }
                    this.CurrentWeapon = _weapons[index];
                } while (_weapons[index].Ammo <= 0);
                _currentWeaponRotateDelayTick = 0;
            }
        }

        private void WalkThroughDoor()
        {
            if (_currentDoorSpriteDelayTick++ == ENTER_DOOR_SPRITE_CHANGE_DELAY)
            {
                _currentDoorSpriteDelayTick = 0;
                if (_currentDoorWalkState == _keenDoorEnterSprites.Length - 1)
                {
                    if (_currentDoor != null && _currentDoor.DestinationDoor != null)
                    {
                        var door = _currentDoor.DestinationDoor;
                        this.HitBox = new Rectangle(door.HitBox.X + ((door.HitBox.Width / 2) - (this.HitBox.Width / 2)),
                            door.HitBox.Bottom - this.HitBox.Height - 1, this.HitBox.Width, this.HitBox.Height);
                        _currentDoorWalkState = 0;
                        this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                        this.UpdateCollisionNodes(Direction.UP_RIGHT);
                        _fallVelocity = 0;
                        this.MoveState = Enums.MoveState.STANDING;
                        if (IsNothingBeneathKeen())
                        {
                            this.Fall();
                        }
                    }
                }
                else
                {
                    _currentDoorWalkState++;
                }
                UpdateSprite();
            }
        }

        private void WalkThroughExit()
        {
            var exit = _currentDoor as ExitDoor;
            if (exit != null && !exit.IsOpening)
            {
                exit.Open();
            }

            if (exit != null && exit.IsOpened)
            {
                if (_currentDoorSpriteDelayTick++ == ENTER_DOOR_SPRITE_CHANGE_DELAY)
                {
                    _currentDoorSpriteDelayTick = 0;
                    if (_currentDoorWalkState == _keenDoorEnterSprites.Length - 1)
                    {
                        //raise keen pass level event
                        this.OnKeenLevelCompleted(new ObjectEventArgs() { ObjectSprite = this });
                    }
                    else
                    {
                        _currentDoorWalkState++;
                    }
                    UpdateSprite();
                }
            }
        }

        private void TryEnterDoor(Door door)
        {
            _currentDoor = door;

            if (_isFiring)
                return;

            if (_isLookingDown)
                return;

            if (this.MoveState != Enums.MoveState.ENTERING_DOOR && _currentDoor != null
                && Math.Abs(this.HitBox.Bottom - _currentDoor.HitBox.Bottom) <= 3)
            {
                if (_currentDoor.DestinationDoor != null || (_hasKeyCard && _currentDoor is ExitDoor))
                {
                    this.HitBox = new Rectangle(door.HitBox.X + ((door.HitBox.Width / 2) - (this.HitBox.Width / 2)), door.HitBox.Bottom - this.HitBox.Height,
                        this.HitBox.Width, this.HitBox.Height);
                    this.UpdateCollisionNodes(this.Direction);
                    this.MoveState = Enums.MoveState.ENTERING_DOOR;
                }
                else if ((_currentDoor is ExitDoor) && !_hasKeyCard)
                {
                    if (!_failedExitDoorAccess)
                    {
                        EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                            GeneralGameConstants.Sounds.KEEN_ACCESS_DENIED);
                        _failedExitDoorAccess = true;
                    }
                }
            }
            else if (CanKeenGrabPole())
            {
                if (this.MoveState != Enums.MoveState.ON_POLE)
                    HangOntoPole();
                else
                    MoveUpPole();
            }
            else if (this.CanLookUp)
            {
                TryLookUp();
            }
        }

        private Door GetFirstOrCollidingDoor()
        {
            var items = this.CheckCollision(this.HitBox);
            var doors = items.OfType<Door>();
            return doors.FirstOrDefault();
        }

        private IActivator GetFirstCollidingSwitch()
        {
            var items = this.CheckCollision(this.HitBox);
            var switches = items.OfType<IActivator>();
            return switches.FirstOrDefault();
        }

        private void TryShoot()
        {
            if (this.CanShoot)
                this.Fire();
        }

        private void UpdateShootSprite()
        {
            switch (this.MoveState)
            {
                case Enums.MoveState.JUMPING:
                case Enums.MoveState.FALLING:
                    if (IsKeyPressed(KEY_UP))
                    {
                        _sprite = _keenShootUpAerial;
                    }
                    else if (IsKeyPressed(KEY_DOWN))
                    {
                        _sprite = _keenShootDownAerial;
                    }
                    else
                    {
                        _sprite = this.Direction == Enums.Direction.LEFT
                            ? _keenShootLeftAerial
                            : _keenShootRightAerial;
                    }
                    break;
                case Enums.MoveState.RUNNING:
                case Enums.MoveState.STANDING:
                    if (_isLookingUp)
                    {
                        _sprite = _keenShootUp;
                    }
                    else
                    {
                        _sprite = this.Direction == Enums.Direction.LEFT
                            ? _keenShootLeft
                            : _keenShootRight;
                    }
                    break;
                case Enums.MoveState.ON_POLE:
                    if (IsKeyPressed(KEY_DOWN))
                    {
                        _sprite = this.Direction == Enums.Direction.LEFT
                            ? _keenShootDownLeftPole
                            : _keenShootDownRightPole;
                    }
                    else if (IsKeyPressed(KEY_UP))
                    {
                        _sprite = this.Direction == Enums.Direction.LEFT
                            ? _keenShootUpLeftPole
                            : _keenShootUpRightPole;
                    }
                    else
                    {
                        _sprite = this.Direction == Enums.Direction.LEFT
                            ? _keenShootPoleLeft
                            : _keenShootPoleRight;
                    }
                    break;
            }

        }

        private void TryLookDown()
        {
            if (this.CanLookDown)
            {
                LookDown();
            }
        }

        private void LookDown()
        {
            if (_originalStandingHitbox == null)
                _originalStandingHitbox = this.HitBox;

            var standingTile = GetTopMostLandingTile(2);
            if (standingTile != null && standingTile.CollisionType == CollisionType.POLE_TILE && CanKeenGrabPole(false))
            {
                _originalStandingHitbox = null;
                HangOntoPole();
                return;
            }

            _isLookingDown = true;
            _isLookingUp = false;
            SetNewLocationAndHitboxLookingDown();
            UpdateSprite();
            if (_currentLookDownSprite < _keenLookDownSprites.Length - 1)
                _currentLookDownSprite++;
        }

        private void SetNewLocationAndHitboxLookingDown()
        {
            if (_originalStandingHitbox != null)
            {
                switch (_currentLookDownSprite)
                {
                    case 0:
                        this.HitBox = new Rectangle(_originalStandingHitbox.Value.X, _originalStandingHitbox.Value.Y + 10, _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height - 10);
                        break;
                    case 1:
                        this.HitBox = new Rectangle(_originalStandingHitbox.Value.X, _originalStandingHitbox.Value.Y + 21, _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height - 21);
                        break;
                }
                UpdateCollisionNodes(this.Direction);
            }
        }

        private void SetNewLocationAndHitboxStopLookingDown()
        {
            if (_originalStandingHitbox != null)
            {
                switch (_currentLookDownSprite)
                {
                    case 0:
                        this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y + 10, _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height - 10);
                        break;
                    case 1:
                        this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - 21, _originalStandingHitbox.Value.Width, _originalStandingHitbox.Value.Height);
                        _originalStandingHitbox = null;
                        break;
                }
                UpdateCollisionNodes(this.Direction);
                //if standing up makes us collide with tile, fall
                var collisionTiles = this.CheckCollision(this.HitBox, true, false);
                var debugTilesAboveKeen = collisionTiles.Where(t => t.CollisionType == CollisionType.BLOCK && t.HitBox.Bottom < this.HitBox.Bottom).ToList();
                if (debugTilesAboveKeen.Any())
                {
                    var lowestTile = debugTilesAboveKeen.OrderByDescending(t => t.HitBox.Bottom).FirstOrDefault();
                    int newY = lowestTile.HitBox.Bottom + 1;
                    this.HitBox = new Rectangle(this.HitBox.X, newY, this.HitBox.Width, this.HitBox.Height);
                    this.UpdateCollisionNodes(Direction.DOWN);

                    //if we bump our head on the ceiling while riding on a conveyer belt,
                    //we are essentially sandwiched between the converyer belt, so it should
                    //result in death
                    var squashDebugTiles = this.CheckCollision(this.HitBox);
                    bool isSquashed = squashDebugTiles
                        .Any(c => c.CollisionType == CollisionType.CONVEYOR_BELT
                        && c.HitBox.Top < this.HitBox.Bottom - 2);
                    if (isSquashed)
                    {
                        this.Die();
                    }
                    //alternatively, we should unassign the player from moving platform tiles
                    //in the event of a squish between a block and said platform tile
                    var movingPlatformTiles = squashDebugTiles.OfType<MovingPlatformTile>();
                    if (movingPlatformTiles.Any())
                    {
                        foreach (var tile in movingPlatformTiles)
                        {
                            tile.UnassignKeen(this);
                        }
                    }
                }
            }
        }

        private void TryLookUp()
        {
            if (this.CanLookUp)
            {
                Lookup();
            }
        }

        private void Lookup()
        {
            _isLookingUp = true;
            _isLookingDown = false;
            if (_originalStandingHitbox != null)
                this.HitBox = _originalStandingHitbox.Value;
            UpdateSprite();
        }

        private void StopLookUp()
        {
            _isLookingUp = false;
            _togglingSwitch = false;

            UpdateSprite();
        }

        private void StopLookDown()
        {
            SetNewLocationAndHitboxStopLookingDown();
            if (_currentLookDownSprite == _keenLookDownSprites.Length - 1)
            {
                _currentLookDownSprite--;
            }
            else
            {
                _isLookingDown = false;
            }
            UpdateSprite();
        }

        private void HangOntoPole()
        {
            SetPoleHangHorizontalLocation();
            this.PoleDirection = Enums.Direction.UP;
            this.MoveState = Enums.MoveState.ON_POLE;
            _currentJumpHeight = 0;
        }

        private void SetPoleHangHorizontalLocation()
        {
            if (_currentPole != null)
            {
                var areaToCheck = new Rectangle();
                if (this.Direction == Enums.Direction.RIGHT)
                {
                    areaToCheck = new Rectangle(new Point(_currentPole.HitBox.Left - 18, this.HitBox.Y), this.HitBox.Size);
                }
                else
                {
                    areaToCheck = new Rectangle(new Point(_currentPole.HitBox.Right - 10, this.HitBox.Y), this.HitBox.Size);
                }
                var items = this.CheckCollision(areaToCheck);
                if (!items.Any(c => c.CollisionType == CollisionType.BLOCK))
                {
                    this.HitBox = areaToCheck;
                }
                UpdateAndCheckCollision();
            }
        }

        private void UpdateAndCheckCollision()
        {
            UpdateCollisionNodes(this.Direction);
            var collisionItems = this.CheckCollision(this.HitBox);
            if (_hangTile != null && collisionItems.Any(c => c.CollisionType == CollisionType.BLOCK
                && c.HitBox.Y != _hangTile.HitBox.Y))
            {
                var climbCollisions = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK
                    && c.HitBox.Y != _hangTile.HitBox.Y).ToList();
                if (this.Direction == Direction.LEFT)
                {
                    int maxX = climbCollisions.Select(c => c.HitBox.Right).Max();
                    var closestTile = climbCollisions.FirstOrDefault(c => c.HitBox.Right == maxX);
                    if (closestTile != null)
                    {
                        this.HitBox = new Rectangle(closestTile.HitBox.Right + 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                }
                else if (this.Direction == Direction.RIGHT)
                {
                    int minX = climbCollisions.Select(c => c.HitBox.Left).Min();
                    var closestTile = climbCollisions.FirstOrDefault(c => c.HitBox.Left == minX);
                    if (closestTile != null)
                    {
                        this.HitBox = new Rectangle(closestTile.HitBox.Left - this.HitBox.Width - 1, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                    }
                }
            }
            foreach (var item in collisionItems)
            {
                this.HandleCollision(item);
            }
        }

        private void ResetPoleMovement()
        {
            _currentPoleClimbSprite = 0;
            this.PoleDirection = Enums.Direction.UP;
        }

        private void MoveUpPole()
        {
            _originalStandingHitbox = null;
            if (_isFiring)
                return;

            if (_isLookingDown)
                return;

            if (_currentPole != null)
            {
                this.PoleDirection = Enums.Direction.UP;
                if (CanMove(Enums.Direction.UP, POLE_CLIMB_SPEED) && this.HitBox.Y >= _currentPole.HitBox.Top - 20)
                {
                    this.HitBox = new Rectangle(new Point(this.HitBox.X, this.HitBox.Y - POLE_CLIMB_SPEED), this.HitBox.Size);
                    UpdateAndCheckCollision();

                }
                int lengthLimit = 0;
                if (this.Direction == Enums.Direction.LEFT)
                {
                    lengthLimit = _keenClimbUpPoleRightSprites.Length - 1;
                }
                else
                {
                    lengthLimit = _keenClimbUpPoleLeftSprites.Length - 1;
                }
                UpdatePoleClimbSprite(lengthLimit);
            }
        }

        private void UpdatePoleClimbSprite(int lengthLimit)
        {
            if (_currentPoleClimbChangeSpriteTick++ == POLE_CLIMB_CHANGE_SPRITE_DELAY)
            {
                _currentPoleClimbChangeSpriteTick = 0;
                if (_currentPoleClimbSprite >= lengthLimit)
                {
                    _currentPoleClimbSprite = 0;
                }
                else
                {
                    _currentPoleClimbSprite++;
                }
            }
            UpdateSprite();
        }

        private void MoveDownPole()
        {
            _originalStandingHitbox = null;
            if (_isFiring)
                return;

            if (_isLookingDown)
                StopLookDown();
            if (CanMove(Enums.Direction.DOWN, POLE_SHIMMY_SPEED))
            {
                this.PoleDirection = Enums.Direction.DOWN;
                this.HitBox = new Rectangle(new Point(this.HitBox.X, this.HitBox.Y + POLE_SHIMMY_SPEED), this.HitBox.Size);
                UpdateAndCheckCollision();

                int lengthLimit = _keenClimbDownPoleSprites.Length - 1;
                UpdatePoleClimbSprite(lengthLimit);
            }
            else
            {
                this.MoveState = IsNothingBeneathKeen() ? Enums.MoveState.FALLING : Enums.MoveState.STANDING;
                _currentPole = null;
                return;
            }

            if (!CanKeenGrabPole())
            {
                this.MoveState = Enums.MoveState.FALLING;
                if (IsNothingBeneathKeen())
                {
                    this.Fall();
                }
                else
                {
                    this.MoveState = Enums.MoveState.STANDING;
                }
            }
        }

        private bool CanMove(Direction direction, int velocity)
        {
            bool canMove = true;
            Rectangle areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Top - velocity, this.HitBox.Width, velocity);
            switch (direction)
            {
                case Enums.Direction.UP:
                    areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Top - velocity, this.HitBox.Width, velocity);
                    break;
                case Enums.Direction.DOWN:
                    areaToCheck = new Rectangle(this.HitBox.X, this.HitBox.Bottom, this.HitBox.Width, velocity);
                    break;
                case Enums.Direction.RIGHT:
                    areaToCheck = new Rectangle(this.HitBox.Right, this.HitBox.Y, velocity, this.HitBox.Height);
                    break;
                case Enums.Direction.LEFT:
                    areaToCheck = new Rectangle(this.HitBox.Left - velocity, this.HitBox.Y, velocity, this.HitBox.Height);
                    break;
            }

            var collisionItems = this.CheckCollision(areaToCheck);

            if (direction == Direction.DOWN && collisionItems.Any(c => c.CollisionType == CollisionType.PLATFORM))
            {
                return false;
            }

            canMove = !collisionItems.Any(c => c.CollisionType == CollisionType.BLOCK);
            return canMove;
        }

        private bool CanKeenGrabPole(bool updateDelay = true)
        {
            bool canGrabPole = false;
            if (_isLookingDown)
                return false;
            if (_currentPoleHangDelayTick == POLE_HANG_DELAY || this.MoveState == Enums.MoveState.ON_POLE || !updateDelay)
            {
                if (this.MoveState == Enums.MoveState.FALLING
                 || this.MoveState == Enums.MoveState.RUNNING
                 || this.MoveState == Enums.MoveState.STANDING
                 || this.MoveState == Enums.MoveState.JUMPING
                 || this.MoveState == Enums.MoveState.ON_POLE)
                {
                    _currentPoleHangDelayTick = this.MoveState == Enums.MoveState.ON_POLE ? POLE_HANG_DELAY : 0;
                    Rectangle areaToCheck = this.HitBox;
                    var collisionItems = this.CheckCollision(areaToCheck).OfType<Pole>();

                    if (collisionItems.Any())
                    {
                        collisionItems = PoleDirection == Direction.UP
                            ? collisionItems.OrderBy(c => c.HitBox.Top).ToList()
                            : collisionItems.OrderByDescending(c => c.HitBox.Top).ToList();
                        _currentPole = collisionItems.FirstOrDefault();
                    }
                    else
                    {
                        _currentPole = null;
                    }

                    canGrabPole = _currentPole != null;
                }
            }
            else if (updateDelay)
            {
                _currentPoleHangDelayTick++;
            }
            return canGrabPole;
        }

        private void TryJump()
        {
            int maxJumpHeight = _jumpFromPole ? MAX_POLE_JUMP_HEIGHT : MAX_JUMP_HEIGHT;
            if (_jumpFromPole && this.MoveState != Enums.MoveState.JUMPING)
                _currentJumpHeight = 0;
            if (this.CanJump && _currentJumpHeight < maxJumpHeight)
            {
                if (_jumpReady)
                    this.Jump();
            }
            else if (this.MoveState == Enums.MoveState.JUMPING && _currentJumpHeight >= maxJumpHeight)
            {
                SetToFallStateFromJumpState();
            }
        }

        private void SetToFallStateFromJumpState()
        {
            _currentJumpHeight = 0;
            this.MoveState = Enums.MoveState.FALLING;
            _jumpReady = false;
        }

        public void SetKeenPushState(Direction direction, bool isBeingPushed, CollisionObject pushingObject)
        {
            if (this.MoveState == MoveState.ENTERING_DOOR)
            {
                ClearPushState();
                return;
            }

            if (direction == Enums.Direction.LEFT)
            {
                if (isBeingPushed)
                {
                    _leftPushingObjects.Add(pushingObject);
                }
                else
                {
                    _leftPushingObjects.Remove(pushingObject);
                }
                _beingPushedLeft = _leftPushingObjects.Any();
            }
            else if (direction == Enums.Direction.RIGHT)
            {
                if (isBeingPushed)
                {
                    _rightPushingObjects.Add(pushingObject);
                }
                else
                {
                    _rightPushingObjects.Remove(pushingObject);
                }
                _beingPushedRight = _rightPushingObjects.Any();
            }
        }

        private void ClearPushState()
        {
            _leftPushingObjects.Clear();
            _rightPushingObjects.Clear();
            return;
        }

        private void TryMoveLeft()
        {
            if (_isLookingUp)
                return;

            if (_isLookingDown)
                return;

            if (_isFiring && this.MoveState != Enums.MoveState.JUMPING && this.MoveState != Enums.MoveState.FALLING)
                return;

            this.Direction = Enums.Direction.LEFT;
            if (this.MoveState != Enums.MoveState.ON_POLE)
            {
                Point p = new Point(this.HitBox.Location.X - VELOCITY, this.HitBox.Location.Y);
                Rectangle areaToCheck = new Rectangle(p, this.HitBox.Size);
                var collisionItems = this.CheckCollision(areaToCheck);
                UpdateMoveStateUponMove();
                var wallTiles = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK);
                int checkCollideXLimit = -1;
                if (wallTiles.Any())
                {
                    int xPos = wallTiles.Select(c => c.HitBox.Right).Max() + 1;
                    checkCollideXLimit = xPos;
                    Point collideP = new Point(xPos, this.HitBox.Y);
                    this.HitBox = new Rectangle(collideP, this.HitBox.Size);
                }
                else if (!_beingPushedRight)
                {
                    this.HitBox = areaToCheck;
                    UpdateCollisionNodes(this.Direction);
                    if (this.MoveState == Enums.MoveState.HANGING)
                    {
                        if (_hangTile != null && _hangTile.HitBox.X > this.HitBox.X)
                        {
                            AbortClimb();
                        }
                    }
                }
                var itemsToCheck = checkCollideXLimit == -1 ? collisionItems : collisionItems.Where(c => c.HitBox.Right >= checkCollideXLimit || c.CollisionType == CollisionType.BLOCK).ToList();
                foreach (var item in itemsToCheck)
                {
                    this.HandleCollision(item);
                }
            }
            else
            {
                SetPoleHangHorizontalLocation();
                UpdateAndCheckCollision();
            }
        }

        private void TryMoveRight()
        {
            if (_isLookingUp)
                return;

            if (_isLookingDown)
                return;

            if (_isFiring && this.MoveState != Enums.MoveState.JUMPING && this.MoveState != Enums.MoveState.FALLING)
                return;

            this.Direction = Enums.Direction.RIGHT;
            if (this.MoveState != Enums.MoveState.ON_POLE)
            {
                Point p = new Point(this.HitBox.Location.X + VELOCITY, this.HitBox.Location.Y);
                Rectangle areaToCheck = new Rectangle(p, this.HitBox.Size);
                var collisionItems = this.CheckCollision(areaToCheck);
                UpdateMoveStateUponMove();
                var wallTiles = collisionItems.Where(c => c.CollisionType == CollisionType.BLOCK);
                int collideCheckXLimit = -1;
                if (wallTiles.Any())
                {
                    int xPos = wallTiles.Select(c => c.HitBox.Left).Min() - this.HitBox.Width - 1;
                    collideCheckXLimit = xPos;
                    Point collideP = new Point(xPos, this.HitBox.Y);
                    this.HitBox = new Rectangle(collideP, this.HitBox.Size);
                }
                else if (!_beingPushedLeft)
                {
                    this.HitBox = areaToCheck;
                    UpdateCollisionNodes(this.Direction);
                    if (this.MoveState == Enums.MoveState.HANGING)
                    {
                        if (_hangTile != null && _hangTile.HitBox.X < this.HitBox.X)
                        {
                            AbortClimb();
                        }
                    }
                }
                var itemsToCheck = collideCheckXLimit == -1 ? collisionItems : collisionItems.Where(x => x.HitBox.X <= collideCheckXLimit || x.CollisionType == CollisionType.BLOCK).ToList();
                foreach (var item in itemsToCheck)
                {
                    this.HandleCollision(item);
                }
            }
            else
            {
                SetPoleHangHorizontalLocation();
                UpdateAndCheckCollision();
            }
        }

        private void UpdateMoveStateUponMove()
        {
            if (this.MoveState == MoveState.STANDING)
            {
                this.MoveState = MoveState.RUNNING;
            }
        }

        private void SetNextExtraLifePointGoal()
        {
            if (_pointsToNextExtraLife < 20000)
            {
                _pointsToNextExtraLife = 20000;
            }
            while (_pointsToNextExtraLife <= _points)
            {
                _pointsToNextExtraLife *= 2;
            }
        }
        public void SetKeyPressed(string key, bool isPressed)
        {
            if (_keysPressed.ContainsKey(key))
            {
                _keysPressed[key] = isPressed;
            }
        }

        public bool IsKeyPressed(string key)
        {
            bool value = false;
            _keysPressed.TryGetValue(key, out value);
            return value;
        }

        public bool CanLookUp
        {
            get
            {
                return this.MoveState == Enums.MoveState.STANDING || this.MoveState == Enums.MoveState.RUNNING;
            }
        }

        public bool CanLookDown
        {
            get
            {
                return (this.MoveState == Enums.MoveState.STANDING || this.MoveState == MoveState.RUNNING) && !_togglingSwitch;
            }
        }

        public int Drops
        {
            get
            {
                return _drops;
            }
            private set
            {
                _drops = value;
                OnLifeDropsChanged(new ObjectEventArgs() { ObjectSprite = this });
            }
        }

        public int Lives
        {
            get
            {
                return _lives;
            }
            private set
            {
                _lives = value;
                OnLivesChanged(new ObjectEventArgs() { ObjectSprite = this });
            }
        }

        public long Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                OnScoreChanged(new ObjectEventArgs() { ObjectSprite = this });
            }
        }



        public int Ammo
        {
            get { return _currentWeapon != null ? _currentWeapon.Ammo : 0; }
        }

        #region custom events

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        public event EventHandler<ObjectEventArgs> ScoreChanged;

        public event EventHandler<WeaponEventArgs> WeaponChanged;

        public event EventHandler<ObjectEventArgs> LivesChanged;

        public event EventHandler<ItemAcquiredEventArgs> ItemLost;

        public event EventHandler<ObjectEventArgs> LifeDropsChanged;

        public event EventHandler<ObjectEventArgs> KeyCardAcquiredChanged;

        public event EventHandler<ObjectEventArgs> KeenLevelCompleted;

        public event EventHandler<ObjectEventArgs> KeenDied;

        public event EventHandler<ObjectEventArgs> ShieldAcquired;

        protected void OnItemLost(ItemAcquiredEventArgs e)
        {
            if (this.ItemLost != null)
            {
                this.ItemLost(this, e);
            }
        }

        protected void OnCreate(ObjectEventArgs e)
        {
            if (this.Create != null)
            {
                this.Create(this, e);
            }
        }

        protected void OnRemove(ObjectEventArgs e)
        {
            if (this.Remove != null)
            {
                this.Remove(this, e);
            }
        }

        protected void OnScoreChanged(ObjectEventArgs e)
        {
            if (this.ScoreChanged != null)
            {
                this.ScoreChanged(this, e);
            }
        }

        protected void OnWeaponChanged(WeaponEventArgs e)
        {
            if (this.WeaponChanged != null)
            {
                this.WeaponChanged(this, e);
            }
        }

        protected void OnLivesChanged(ObjectEventArgs e)
        {
            if (this.LivesChanged != null)
            {
                this.LivesChanged(this, e);
            }
        }

        protected void OnLifeDropsChanged(ObjectEventArgs e)
        {
            if (this.LifeDropsChanged != null)
            {
                this.LifeDropsChanged(this, e);
            }
        }

        protected void OnKeyCardAcquisitionStateChanged(ObjectEventArgs e)
        {
            if (this.KeyCardAcquiredChanged != null)
            {
                this.KeyCardAcquiredChanged(this, e);
            }
        }

        protected void OnKeenLevelCompleted(ObjectEventArgs e)
        {
            if (this.KeenLevelCompleted != null)
            {
                this.KeenLevelCompleted(this, e);
            }
        }

        protected void OnKeenDied(ObjectEventArgs e)
        {
            if (this.KeenDied != null)
            {
                this.KeenDied(this, e);
            }
        }

        protected void OnShieldAcquired(ObjectEventArgs e)
        {
            this.ShieldAcquired?.Invoke(this, e);
        }

        #endregion

        public void DisappearDeath()
        {
            foreach (var node in _collidingNodes)
            {
                node.Objects.Remove(this);
            }
            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = this
            };
            OnRemove(e);
            if (_shield != null)
            {
                _shield.Deactivate();
            }
            this.Die();
            _disappearDeath = true;
        }

        public override string ToString()
        {
            string key = this.IsLeftDirection(this.Direction)
                ? nameof(Properties.Resources.keen_stand_left)
                : nameof(Properties.Resources.keen_stand_right);
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{key}{separator}{this.Location.X}{separator}{this.Location.Y}{separator}{this.HitBox.Width}{separator}{this.HitBox.Height}{separator}{this.Direction.ToString()}";
        }
        public override CollisionType CollisionType => CollisionType.PLAYER;

        public int ZIndex => 20;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;
    }
}
