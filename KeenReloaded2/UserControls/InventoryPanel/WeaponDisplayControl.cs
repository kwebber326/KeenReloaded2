using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Framework.GameEntities.Weapons;
using KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo;
using KeenReloaded.Framework.Utilities;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class WeaponDisplayControl : UserControl
    {
        private bool _selected;
        private Color _originalColor;
        private NeuralStunner _weapon;
        private int _ordinalPosition;


        protected int DIGIT_HORIZONTAL_OFFSET = 16;
        protected const int AMMO_VERTICAL_OFFSET = 6;
        protected const int AMMO_DIGIT_COUNT = 3;

        protected Dictionary<string, Image> _weaponImages = new Dictionary<string, Image>();
        protected Dictionary<char, Image> _digitLEDs = new Dictionary<char, Image>();
        protected List<Image> _ammoCountLEDImages = new List<Image>();

        public WeaponDisplayControl()
        {
            InitializeComponent();
            _originalColor = this.BackColor;
        }

        public WeaponDisplayControl(NeuralStunner weapon, int ordinalPosition, bool selected = false)
        {
            InitializeComponent();
            InitializeWeaponImages();
            InitializeLEDDictionary();
            _originalColor = this.BackColor;
            this.OrdinalPosition = ordinalPosition;
            this.Selected = selected;
            this.Weapon = weapon;

        }

        private void WeaponDisplayControl_Load(object sender, EventArgs e)
        {

        }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                BackColor = _selected ? Color.Gray : _originalColor;
            }
        }

        public int OrdinalPosition
        {
            get
            {
                return _ordinalPosition;
            }
            private set
            {
                _ordinalPosition = value;
                SetLedNumber(_ordinalPosition, pbWeaponNumber);
            }
        }

        public NeuralStunner Weapon
        {
            get
            {
                return _weapon;
            }
            private set
            {
                _weapon = value;
                if (_weapon != null)
                {
                    SetWeaponImage();
                    SetLedNumber(_weapon.Ammo, pbAmmoAmount);
                }
            }
        }

        public void UpdateWeaponDisplay(NeuralStunner weapon, bool selected)
        {
            this.Weapon = weapon;
            this.Selected = selected;
        }

        private void SetWeaponImage()
        {
            int margin = 4;
            pbWeapon.Image = _weaponImages[_weapon.GetType().Name];
            pbWeapon.Location = new Point(pbWeaponNumber.Right + margin, pbWeapon.Location.Y);
            label1.Location = new Point(pbWeapon.Right + margin, label1.Location.Y);
            pbAmmoAmount.Location = new Point(label1.Right + margin, pbAmmoAmount.Location.Y);
        }

        private void SetLedNumber(int number, PictureBox pictureBox)
        {
            string ordinalPositionStr = number.ToString();
            int verticalOffset = 2;
            int horizontalOffset = 2;
            int margin = 4;
            int ledDigitWidth = 12;
            int ledDigitHeight = 16;
            int x = horizontalOffset;
            int y = verticalOffset;
            List<ImageWithLocation> ledDigits = new List<ImageWithLocation>();
            for (int i = 0; i < ordinalPositionStr.Length; i++)
            {
                x = horizontalOffset + (i * (margin + ledDigitWidth));
                Point p = new Point(x, y);
                Image img = _digitLEDs[ordinalPositionStr[i]];
                ImageWithLocation ledDigit = new ImageWithLocation(img, p);
                ledDigits.Add(ledDigit);
            }

            int width = x + ledDigitWidth;
            int height = verticalOffset + ledDigitHeight;
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                foreach (var digit in ledDigits)
                {
                    g.DrawImage(digit.Image, digit.Location);
                }
            }
            pictureBox.Image = bmp;
        }

        protected virtual void InitializeWeaponImages()
        {
            _weaponImages.Add("NeuralStunner", Properties.Resources.neural_stunner1);
            _weaponImages.Add("ShotgunNeuralStunner", Properties.Resources.neural_stunner_shotgun);
            _weaponImages.Add("SMGNeuralStunner", Properties.Resources.neural_stunner_smg_1);
            _weaponImages.Add("RailgunNeuralStunner", Properties.Resources.neural_stunner_railgun1);
            _weaponImages.Add("RPGNeuralStunner", Properties.Resources.neural_stunner_rocket_launcher1);
            _weaponImages.Add("BoobusBombLauncher", Properties.Resources.keen_dreams_boobus_bomb2);
            _weaponImages.Add("BFG", Properties.Resources.BFG1);
            _weaponImages.Add("SnakeGun", Properties.Resources.snake_gun1);
        }

        protected virtual void InitializeLEDDictionary()
        {
            _digitLEDs.Add('x', Properties.Resources.scoreboard_default_LED);
            _digitLEDs.Add('0', Properties.Resources.scoreboard_LED_zero);
            _digitLEDs.Add('1', Properties.Resources.scoreboard_LED_one);
            _digitLEDs.Add('2', Properties.Resources.scoreboard_LED_two);
            _digitLEDs.Add('3', Properties.Resources.scoreboard_LED_three);
            _digitLEDs.Add('4', Properties.Resources.scoreboard_LED_four);
            _digitLEDs.Add('5', Properties.Resources.scoreboard_LED_five);
            _digitLEDs.Add('6', Properties.Resources.scoreboard_LED_six);
            _digitLEDs.Add('7', Properties.Resources.scoreboard_LED_seven);
            _digitLEDs.Add('8', Properties.Resources.scoreboard_LED_eight);
            _digitLEDs.Add('9', Properties.Resources.scoreboard_LED_nine);

            AddDefaultLEDs();
        }

        protected void AddDefaultLEDs()
        {
            for (int i = 0; i < AMMO_DIGIT_COUNT; i++)
            {
                _ammoCountLEDImages.Add(_digitLEDs['x']);
            }
        }

        private void PbWeaponNumber_Click(object sender, EventArgs e)
        {

        }
    }
}
