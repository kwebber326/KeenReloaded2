using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.GameEntities.Items;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class ShieldInventoryControl : UserControl
    {
        const int VERTICAL_OFFSET = 10;
        const int HORIZONTAL_OFFSET = 2;
        const int LED_WIDTH = 14;
        const int MARGIN = 4;
        const string ACTIVE_TEXT = "Shield (Active):";
        const string INACTIVE_TEXT = "Shield (Inactive):";
        readonly Color _activeColor = Color.Green;
        readonly Color _inactiveColor = Color.Red;

        private Dictionary<char, Image> _digitLEDs = new Dictionary<char, Image>();
        private Shield _shield;

        public ShieldInventoryControl()
        {
            InitializeComponent();
            InitializeLEDDictionary();
        }

        private void ShieldInventoryControl_Load(object sender, EventArgs e)
        {
            SetShieldCount(0);
        }

        public Shield Shield
        {
            get
            {
                return _shield;
            }
            set
            {
                if (_shield != null && value == null)
                {
                    _shield.ShieldDurationChanged -= _shield_ShieldDurationChanged;
                    SetShieldActiveStatus(false);
                    SetShieldCount(0);
                }
                _shield = value;
                if (_shield != null)
                {
                    _shield.ShieldDurationChanged += _shield_ShieldDurationChanged;
                }
            }
        }

        private void _shield_ShieldDurationChanged(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {
            var shieldCount = _shield?.Duration ?? 0;
            this.SetShieldCount(shieldCount);
            if (shieldCount <= 0)
            {
                SetShieldActiveStatus(false);
            }
        }

        public void SetShieldActiveStatus(bool isActive)
        {
            lblShieldStatus.Text = isActive ? ACTIVE_TEXT : INACTIVE_TEXT;
            lblShieldStatus.ForeColor = isActive ? _activeColor : _inactiveColor;
        }

        public void SetShieldCount(int count)
        {
            string countStr = count < 0 ? "0" : count.ToString();
            int x = HORIZONTAL_OFFSET;
            int y = VERTICAL_OFFSET;
            List<Image> images = new List<Image>();
            List<Point> locations = new List<Point>();

            if (count > 9999)
            {
                for (int i = 0; i < 4; i++)
                {
                    Point p = new Point(x, y);
                    locations.Add(p);
                    images.Add(_digitLEDs['9']);
                }
            }
            else
            {
                for (int i = countStr.Length - 1; i >= 0; i--)
                {
                    x = HORIZONTAL_OFFSET + (i * (MARGIN + LED_WIDTH));
                    Point p = new Point(x, y);
                    locations.Add(p);
                    Image img = _digitLEDs[countStr[i]];
                    images.Add(img);
                }
            }

            pbShieldCount.Image = BitMapTool.DrawImagesOnCanvas(pbShieldCount.Size, null, images.ToArray(), locations.ToArray());
        }

        private void InitializeLEDDictionary()
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
        }
    }
}
