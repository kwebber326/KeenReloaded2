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

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class LifeDropInventoryControl : UserControl
    {
        const int VERTICAL_OFFSET = 24;
        const int HORIZONTAL_OFFSET = 51;
        const int LED_WIDTH = 14;
        const int MARGIN = 4;
        private Dictionary<char, Image> _digitLEDs = new Dictionary<char, Image>();
        private readonly Image _originalImage;

        public LifeDropInventoryControl()
        {
            InitializeComponent();
            _originalImage = pbDisplay.Image;
        }

        private void LifeDropInventoryControl_Load(object sender, EventArgs e)
        {
            InitializeLEDDictionary();
            SetLifeDropCount(0);
        }

        public void SetLifeDropCount(int count)
        {
            string countStr = count < 0 ? "0" : count.ToString();
            int x = HORIZONTAL_OFFSET;
            int y = VERTICAL_OFFSET;
            List<Image> images = new List<Image>();
            List<Point> locations = new List<Point>();

            if (count > 99)
            {
                Point p1 = new Point(x, y);
                x += MARGIN + LED_WIDTH;
                Point p2 = new Point(x, y);

                locations.Add(p1);
                locations.Add(p2);
                images.Add(_digitLEDs['9']);
                images.Add(_digitLEDs['9']);
            }
            else
            {
                for (int i = 0; i < countStr.Length; i++)
                {
                    
                    Point p = new Point(x, y);
                    locations.Add(p);
                    Image img = _digitLEDs[countStr[i]];
                    images.Add(img);
                    x += (MARGIN + LED_WIDTH);
                }
            }

           pbDisplay.Image = BitMapTool.DrawImagesOnCanvas(pbDisplay.Size, _originalImage, images.ToArray(), locations.ToArray());
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
