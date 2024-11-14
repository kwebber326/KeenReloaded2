using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;
using IActivator = KeenReloaded2.Framework.GameEntities.Interfaces.IActivator;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class RandomWeaponGenerator : CollisionObject, IUpdatable, ISprite, ICreateRemove
    {
        private Image[] _sprites;
        private Image[] _weaponSprites;
        private const int SPRITE_CHANGE_DELAY = 4;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;
        private Image _sprite;

        private CommanderKeen _keen;
        private readonly int _cost;
        private readonly int _zIndex;
        private const int WEAPON_SPRITE_CHANGE_DELAY = 1;
        private const int WEAPON_DECISION_TIME = 100;
        private int _currentWeaponDecisionTimeTick;
        private int _currentWeaponSpriteChangeDelayTick;
        private int _currentWeaponSprite;

        private RandomWeaponGeneratorSwitch _switch;
        private RandomWeaponGeneratorState _state;
        private bool _closingDoor;
        private string _weaponType;
        private const int WEAPON_IMAGE_WIDTH = 64;
        private const int WEAPON_IMAGE_HEIGHT = 32;

        private const string STUNNER_NEURAL = "NeuralStunner";
        private const string STUNNER_SHOTGUN = "Shotgun";
        private const string STUNNER_SMG = "SMG";
        private const string STUNNER_RPG = "RPG";
        private const string STUNNER_BOOBUS_BOMB = "BoobusBomb";
        private const string STUNNER_RAILGUN = "Railgun";
        private const string BFG = "BFG";
        private const string SNAKE_GUN = "SnakeGun";

        private const int AMMO_MAX_STUNNER_NEURAL = 50;
        private const int AMMO_MIN_STUNNER_NEURAL = 5;
        private const int AMMO_MAX_STUNNER_SHOTGUN = 100;
        private const int AMMO_MIN_STUNNER_SHOTGUN = 5;
        private const int AMMO_MAX_STUNNER_SMG = 150;
        private const int AMMO_MIN_STUNNER_SMG = 10;
        private const int AMMO_MAX_STUNNER_RPG = 25;
        private const int AMMO_MIN_STUNNER_RPG = 1;
        private const int AMMO_MAX_STUNNER_BOOBUS_BOMB = 50;
        private const int AMMO_MIN_STUNNER_BOOBUS_BOMB = 5;
        private const int AMMO_MAX_STUNNER_RAILGUN = 20;
        private const int AMMO_MIN_STUNNER_RAILGUN = 1;
        private const int AMMO_MAX_BFG = 10;
        private const int AMMO_MIN_BFG = 1;
        private const int AMMO_MAX_SNAKE_GUN = 30;
        private const int AMMO_MIN_SNAKE_GUN = 1;
        private const int BFG_PROBABILITY_DENOMINATOR = 100;//the higher the value, the lower the chance
        private const int SNAKE_GUN_PROBABILITY_DENOMINATOR = 100;//

        private Image _costLabel;


        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        public Image WeaponSprite
        {
            get; private set;
        }

        RandomWeaponGeneratorState State
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
                case RandomWeaponGeneratorState.OFF:
                case RandomWeaponGeneratorState.CHOOSING_WEAPON:
                    _currentSprite = 0;
                    break;
                case RandomWeaponGeneratorState.DISPENSING_WEAPON:
                    _currentSprite = 1;
                    break;

            }
            if (_state == RandomWeaponGeneratorState.OFF)
            {
                this.WeaponSprite = null;
            }
            _sprite = _sprites[_currentSprite];
        }

        public RandomWeaponGenerator(Rectangle area, SpaceHashGrid grid, int zIndex, int cost) : base(grid, area)
        {
            if (_cost < 0)
                throw new ArgumentNullException("Cost for random weapon generator must be greater than or equal to zero");

            _zIndex = zIndex;
            _cost = cost;
            Initialize();
        }

        private void Initialize()
        {
            _sprites = SpriteSheet.SpriteSheet.WeaponGeneratorImages;
            _weaponSprites = SpriteSheet.SpriteSheet.WeaponImages;
            _sprite = _sprites[_currentSprite];
            _switch = new RandomWeaponGeneratorSwitch(_collisionGrid, new Rectangle(this.HitBox.Right - 16, this.HitBox.Bottom - 32, 12, 28), _cost, _keen);
            _switch.Toggled += _switch_Toggled;
            //this.WeaponSprite.Location =
            //    new Point((this.HitBox.X + this.HitBox.Width / 2) - this.WeaponSprite.Width / 2
            //    , this.HitBox.Y - WEAPON_IMAGE_HEIGHT - 8);

            //_costLabel = new Label();
            //_costLabel.Text = _cost.ToString();
            //_costLabel.ForeColor = Color.Red;
            //_costLabel.Location = new Point(
            //    this.HitBox.Right + 2,//x
            //    this.HitBox.Bottom - 64);//y

            ResetRandomVariable();
        }

        public Image CostLabel
        {
            get
            {
                return _costLabel;
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.NONE;

        public string FileUtility { get; private set; }

        private void _switch_Toggled(object sender, ToggleEventArgs e)
        {
            if (e.IsActive)
            {
                ChooseWeapon();
            }
            else
            {
                this.State = RandomWeaponGeneratorState.OFF;
            }
        }

        private void OnCreated(ObjectEventArgs e)
        {
            Create?.Invoke(this, e);
        }

        private void OnRemoved(ObjectEventArgs e)
        {
            Remove?.Invoke(this, e);
        }

        public void Update()
        {
            switch (_state)
            {
                case RandomWeaponGeneratorState.CHOOSING_WEAPON:
                    ChooseWeapon();
                    break;
                case RandomWeaponGeneratorState.DISPENSING_WEAPON:
                    DispenseWeapon();
                    break;
            }
        }

        private void ChooseWeapon()
        {
            if (this.State != RandomWeaponGeneratorState.CHOOSING_WEAPON)
            {
                this.State = RandomWeaponGeneratorState.CHOOSING_WEAPON;
                _currentWeaponDecisionTimeTick = 0;
                ChooseRandomWeaponType();

            }

            if (_currentWeaponDecisionTimeTick++ == WEAPON_DECISION_TIME)
            {
                _currentWeaponDecisionTimeTick = 0;
                SetWeaponImageBasedOnType();
                this.DispenseWeapon();
            }
            else
            {
                this.UpdateSpriteByDelayBase(ref _currentWeaponSpriteChangeDelayTick, ref _currentWeaponSprite, WEAPON_SPRITE_CHANGE_DELAY, UpdateWeaponSprite);
            }
        }

        private void UpdateWeaponSprite()
        {
            if (_currentWeaponSprite >= _weaponSprites.Length)
            {
                _currentWeaponSprite = 0;
            }
            if (_currentWeaponSprite < 0)
                _currentWeaponSprite = 0;

            this.WeaponSprite = _weaponSprites[_currentWeaponSprite];
        }

        private void DispenseWeapon()
        {
            if (this.State != RandomWeaponGeneratorState.DISPENSING_WEAPON)
            {
                this.State = RandomWeaponGeneratorState.DISPENSING_WEAPON;
                _currentSpriteChangeDelayTick = 0;
                _currentSprite = 1;
            }

            if (_currentSprite > 0)
            {
                //Update the timer by delay
                if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
                {
                    //set ticker to zero after each sprite update
                    _currentSpriteChangeDelayTick = 0;
                    //have a boolean to show whether or not we are closing the door again
                    //if not closing, run the below functionality
                    if (!_closingDoor)
                    {
                        //Increment the current sprite. if we reach the peak of the sprite list, we start moving back down. set closing to true
                        if (++_currentSprite >= _sprites.Length)
                        {
                            _currentSprite = _sprites.Length - 2;
                            _closingDoor = true;
                            //create weapon
                            CreateWeapon();
                        }
                    }
                    else
                    {
                        _currentSprite--;
                    }//else, decrement the current sprite

                    //set the current sprite
                    _sprite = _sprites[_currentSprite];
                }

            }
            else
            {//set back to an "off" state
                _closingDoor = false;

                this._switch.Deactivate();
            }
        }

        private void ChooseRandomWeaponType()
        {
            //choose a random number between 0 and length of weapon images - 1
            int imageIndex = _random.Next(0, _sprites.Length);//exclusive upper bound

            //set _weaponType field to indicate the type of the weapon
            switch (imageIndex)
            {
                case 0:
                    _weaponType = STUNNER_NEURAL;
                    break;
                case 1:
                    _weaponType = STUNNER_SHOTGUN;
                    break;
                case 2:
                    _weaponType = STUNNER_SMG;
                    break;
                case 3:
                    _weaponType = STUNNER_RPG;
                    break;
                case 4:
                    _weaponType = STUNNER_BOOBUS_BOMB;
                    break;
                case 5:
                    _weaponType = STUNNER_RAILGUN;
                    break;
            }

            int bfgVal = _random.Next(1, BFG_PROBABILITY_DENOMINATOR + 1);
            if (bfgVal == BFG_PROBABILITY_DENOMINATOR)//can be any value between 1 and probability denominator
            {
                _weaponType = BFG;//this will override the previous decision
            }

            int snakeGunVal = _random.Next(1, SNAKE_GUN_PROBABILITY_DENOMINATOR + 1);
            if (snakeGunVal == SNAKE_GUN_PROBABILITY_DENOMINATOR)
            {
                _weaponType = SNAKE_GUN;//final override
            }
        }

        private void SetWeaponImageBasedOnType()
        {
            switch (_weaponType)
            { //have decision login on random weapon ammo amount
                case STUNNER_NEURAL:
                    _currentWeaponSprite = 0;
                    break;
                case STUNNER_SHOTGUN:
                    _currentWeaponSprite = 1;
                    break;
                case STUNNER_SMG:
                    _currentWeaponSprite = 2;
                    break;
                case STUNNER_RPG:
                    _currentWeaponSprite = 3;
                    break;
                case STUNNER_BOOBUS_BOMB:
                    _currentWeaponSprite = 4;
                    break;
                case STUNNER_RAILGUN:
                    _currentWeaponSprite = 5;
                    break;
                case BFG:
                    _currentWeaponSprite = -1;
                    break;
                case SNAKE_GUN:
                    _currentWeaponSprite = -2;
                    break;
            }
            if (_currentWeaponSprite == -1)//we don't want the BFG image to show in the lottery turn because it is a surprise lol
            {
                this.WeaponSprite = Properties.Resources.BFG1;//override if lucky
            }
            else if (_currentWeaponSprite == -2)
            {
                this.WeaponSprite = Properties.Resources.snake_gun1;//final override is snake gun
            }
            else
            {
                //set the current weapon image to the image in the weapon image list corresponding with the randomly generated index
                this.WeaponSprite = _weaponSprites[_currentWeaponSprite];
            }
        }

        private void CreateWeapon()
        {
            GameEntities.Items.Item weapon = null;
            int ammoAmount = 0;
            string filePath = Path.Combine(System.Environment.CurrentDirectory, MapMakerConstants.MAP_MAKER_FOLDER, MapMakerConstants.Categories.OBJECT_CATEGORY_WEAPONS);
            //switch between the type of weapon to generate
            //construct an ammo object of the corresponding weapon type
            switch (_weaponType)
            { //have decision login on random weapon ammo amount
                case STUNNER_NEURAL:
                    ammoAmount = _random.Next(AMMO_MIN_STUNNER_NEURAL, AMMO_MAX_STUNNER_NEURAL + 1);
                    string imageNamePistol = Path.Combine(filePath, nameof(Properties.Resources.neural_stunner1) + ".png");
                    weapon = new NeuralStunnerAmmo(_collisionGrid, new Rectangle(
                        (this.HitBox.X + this.HitBox.Width / 2) - 15,//x
                        (this.HitBox.Bottom - this.HitBox.Height / 2) + 11,//y
                        30,//width
                        22//height
                        ), imageNamePistol, _zIndex + 1, ammoAmount);
                    break;
                case STUNNER_SHOTGUN:
                    ammoAmount = _random.Next(AMMO_MIN_STUNNER_SHOTGUN, AMMO_MAX_STUNNER_SHOTGUN + 1);

                    //round to nearest multiple of 5 (shots per fire)
                    if (ammoAmount % 5 >= 3)
                    {
                        ammoAmount += (5 - (ammoAmount % 5));
                    }
                    else
                    {
                        ammoAmount /= 5;
                        ammoAmount *= 5;
                    }
                    string imageNameShotty = Path.Combine(filePath, nameof(Properties.Resources.neural_stunner_shotgun) + ".png");
                    weapon = new ShotgunNeuralStunnerAmmo(_collisionGrid, new Rectangle(
                      (this.HitBox.X + this.HitBox.Width / 2) - 26,//x
                      (this.HitBox.Bottom - this.HitBox.Height / 2) + 11,//y
                      53,//width
                      22//height
                      ), imageNameShotty, _zIndex + 1, ammoAmount);
                    break;
                case STUNNER_SMG:
                    ammoAmount = _random.Next(AMMO_MIN_STUNNER_SMG, AMMO_MAX_STUNNER_SMG + 1);
                    string imageNameSMG = Path.Combine(filePath, nameof(Properties.Resources.neural_stunner_smg_1) + ".png");
                    weapon = new SMGNeuralStunnerAmmo(_collisionGrid, new Rectangle(
                        (this.HitBox.X + this.HitBox.Width / 2) - 23,//x
                        (this.HitBox.Bottom - this.HitBox.Height / 2) + 11,//y
                        46,//width
                        22//height
                        ), imageNameSMG, _zIndex + 1, ammoAmount);
                    break;
                case STUNNER_RPG:
                    ammoAmount = _random.Next(AMMO_MIN_STUNNER_RPG, AMMO_MAX_STUNNER_RPG + 1);
                    string imageNameRPG = Path.Combine(filePath, nameof(Properties.Resources.neural_stunner_rpg1) + ".png");
                    weapon = new RPGNeuralStunnerAmmo(_collisionGrid, new Rectangle(
                        (this.HitBox.X + this.HitBox.Width / 2) - 27,//x
                        (this.HitBox.Bottom - this.HitBox.Height / 2) + 13,//y
                        55,//width
                        26//height
                        ), imageNameRPG, _zIndex + 1, ammoAmount);
                    break;
                case STUNNER_BOOBUS_BOMB:
                    ammoAmount = _random.Next(AMMO_MIN_STUNNER_BOOBUS_BOMB, AMMO_MAX_STUNNER_BOOBUS_BOMB + 1);
                    string imageNameGrenade = Path.Combine(filePath, nameof(Properties.Resources.keen_dreams_boobus_bomb1) + ".png");
                    weapon = new BoobusBombLauncherAmmo(_collisionGrid, new Rectangle(
                        (this.HitBox.X + this.HitBox.Width / 2) - 15,//x
                        (this.HitBox.Bottom - this.HitBox.Height / 2) + 15,//y
                        30,//width
                        30//height
                        ), imageNameGrenade, _zIndex + 1, ammoAmount);
                    break;
                case STUNNER_RAILGUN:
                    ammoAmount = _random.Next(AMMO_MIN_STUNNER_RAILGUN, AMMO_MAX_STUNNER_RAILGUN + 1);
                    string imageNameRailgun = Path.Combine(filePath, nameof(Properties.Resources.neural_stunner_railgun1) + ".png");
                    weapon = new RailgunNeuralStunnerAmmo(_collisionGrid, new Rectangle(
                        (this.HitBox.X + this.HitBox.Width / 2) - 26,//x
                        (this.HitBox.Bottom - this.HitBox.Height / 2) + 11,//y
                        52,//width
                        22//height
                        ), imageNameRailgun, _zIndex + 1, ammoAmount);
                    break;
                case BFG:
                    ammoAmount = _random.Next(AMMO_MIN_BFG, AMMO_MAX_BFG + 1);
                    string imageNameBFG = Path.Combine(filePath, nameof(Properties.Resources.BFG1) + "png");
                    weapon = new BFGAmmo(_collisionGrid, new Rectangle(
                        (this.HitBox.X + this.HitBox.Width / 2) - 30,//x
                        (this.HitBox.Bottom - this.HitBox.Height / 2) + 12,//y
                        61,//width
                        24//height
                        ), imageNameBFG, _zIndex + 1, ammoAmount);
                    break;
                case SNAKE_GUN:
                    ammoAmount = _random.Next(AMMO_MIN_SNAKE_GUN, AMMO_MAX_SNAKE_GUN + 1);
                    string imageNameSnakeGun = Path.Combine(filePath, nameof(Properties.Resources.snake_gun1) + ".png");
                    weapon = new SnakeGunAmmo(_collisionGrid, new Rectangle(
                      (this.HitBox.X + this.HitBox.Width / 2) - 28,//x
                      (this.HitBox.Bottom - this.HitBox.Height / 2) + 10,//y
                      57,//width
                      20//height
                      ), imageNameSnakeGun, _zIndex + 1, ammoAmount);
                    break;
            }
            //add removal event so removal event is registered to remove item when acquired
            weapon.Remove += W_Remove;
            //call OnCreate method passing event args containing the newly constructed object
            OnCreated(new ObjectEventArgs() { ObjectSprite = weapon });
            //if it collides with keen out of the door, have keen collect the item
            if (_keen.HitBox.IntersectsWith(weapon.HitBox))
            {
                weapon.SetAcquired();
                _keen.HandleItemCollection(weapon);
            }
        }

        private void W_Remove(object sender, ObjectEventArgs e)
        {
            OnRemoved(e);
        }
    }

    enum RandomWeaponGeneratorState
    {
        OFF,
        CHOOSING_WEAPON,
        DISPENSING_WEAPON
    }

    public class RandomWeaponGeneratorSwitch : CollisionObject, IActivator
    {
        private bool _isActive;
        private CommanderKeen _keen;
        private readonly int _cost;

        public RandomWeaponGeneratorSwitch(SpaceHashGrid grid, Rectangle hitbox, int cost, CommanderKeen keen) : base(grid, hitbox)
        {
            if (keen == null)
                throw new ArgumentNullException("Keen was not properly set");
            if (_cost < 0)
                throw new ArgumentNullException("Cost for random weapon generator must be greater than or equal to zero");

            _keen = keen;
            _cost = cost;
        }

        public List<IActivateable> ToggleObjects => throw new NotImplementedException();

        public bool IsActive => _isActive;

        public override CollisionType CollisionType => CollisionType.TOGGLE_SWITCH;

        public event EventHandler<ToggleEventArgs> Toggled;

        internal void Deactivate()
        {
            if (_isActive)
                OnToggled(this, new ToggleEventArgs() { IsActive = false });
            _isActive = false;
        }

        public void Toggle()
        {
            if (_keen.Points >= _cost && !_isActive)
            {
                _keen.Points -= _cost;
                OnToggled(this, new ToggleEventArgs() { IsActive = true });
                _isActive = true;
            }
        }

        private void OnToggled(object sender, ToggleEventArgs e)
        {
            Toggled?.Invoke(sender, e);
        }
    }
}
