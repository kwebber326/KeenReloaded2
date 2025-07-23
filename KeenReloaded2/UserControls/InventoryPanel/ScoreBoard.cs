using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Utilities;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.GameEntities.Players;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class ScoreBoard : UserControl
    {
        Dictionary<char, Image> _digitLEDs = new Dictionary<char, Image>();
        private CommanderKeen _keen;

        private const int INITIAL_SCORE_HORIZONTAL_OFFSET = 12;
        private const int SCORE_VERITICAL_OFFSET = 7;
        private const int DISTANCE_BETWEEN_LED_DIGITS = 4;
        private const int SCORE_BOARD_DIGIT_COUNT = 9;
        private const int LED_DIGIT_WIDTH = 12;

        private const int INITIAL_LIFE_HORIZONTAL_OFFSET = 44;
        private const int LIFE_VERTICAL_OFFSET = 38;
        private const int LIFE_DIGIT_COUNT = 2;

        private const int INITIAL_AMMO_HORIZONTAL_OFFSET = 124;
        private const int AMMO_VERTICAL_OFFSET = 38;
        private const int AMMO_DIGIT_COUNT = 2;

        public ScoreBoard()
        {
            InitializeComponent();
            InitializeLEDDictionary();
        }

        private void ScoreBoard_Load(object sender, EventArgs e)
        {
           
        }

        public void UpdateScoreBoard()
        {
            Image backgroundImage = Properties.Resources.scoreboard_blank;

            Size imageSize = pbScoreBoard.Size.Width == 0 ? backgroundImage.Size : pbScoreBoard.Size;
            ImageWithLocation[] data = GetLEDImagesFromKeen();
            Image[] images = data.Select(d => d.Image).ToArray();
            Point[] locations = data.Select(d => d.Location).ToArray();

            pbScoreBoard.Image = BitMapTool.DrawImagesOnCanvas(imageSize, backgroundImage, images, locations);
        }

        private ImageWithLocation[] GetLEDImagesFromKeen()
        {
            ImageWithLocation[] scoreImages = GetScoreImages();
            ImageWithLocation[] ammoAmountImages = GetWeaponAmmoAmountImages();
            ImageWithLocation[] lifeAmountImages = GetLifeAmountImages();

            List<ImageWithLocation> images = new List<ImageWithLocation>();
            images.AddRange(scoreImages);
            images.AddRange(ammoAmountImages);
            images.AddRange(lifeAmountImages);
            
            return images.ToArray();
        }

        private ImageWithLocation[] GetScoreImages()
        {
            ImageWithLocation[] images = new ImageWithLocation[SCORE_BOARD_DIGIT_COUNT];
            int x = INITIAL_SCORE_HORIZONTAL_OFFSET;
            int y = SCORE_VERITICAL_OFFSET;
            //score
            long score = _keen.Points;
            if (score >= 999999999)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    Image img = _digitLEDs['9'];
                    x = INITIAL_SCORE_HORIZONTAL_OFFSET + (i * (DISTANCE_BETWEEN_LED_DIGITS + LED_DIGIT_WIDTH));
                    Point location = new Point(x, y);
                    images[i] = new ImageWithLocation(img, location);
                }
            }
            else
            {
                string scoreString = score.ToString();
                int currentDigit = 0;
                for (int i = 0; i < SCORE_BOARD_DIGIT_COUNT; i++)
                {
                    Image img;
                    if (SCORE_BOARD_DIGIT_COUNT - i > scoreString.Length)
                    {
                        img = _digitLEDs['x'];
                    }
                    else
                    {
                        img = _digitLEDs[scoreString[currentDigit++]];
                    }
                    x = INITIAL_SCORE_HORIZONTAL_OFFSET + (i * (DISTANCE_BETWEEN_LED_DIGITS + LED_DIGIT_WIDTH));
                    Point location = new Point(x, y);
                    images[i] = new ImageWithLocation(img, location);
                }
            }
            return images;
        }

        private ImageWithLocation[] GetWeaponAmmoAmountImages()
        {
            ImageWithLocation[] imageWithLocations = new ImageWithLocation[0];
            var selectedWeapon = _keen.CurrentWeapon;
            if (selectedWeapon == null)
                return imageWithLocations;

            string ammoString = selectedWeapon.Ammo.ToString();

            imageWithLocations = new ImageWithLocation[AMMO_DIGIT_COUNT];
            int x = INITIAL_AMMO_HORIZONTAL_OFFSET;
            int y = AMMO_VERTICAL_OFFSET;
            if (selectedWeapon.Ammo > 99)
            {
                Image img = _digitLEDs['9'];
                for (int i = 0; i < imageWithLocations.Length; i++)
                {
                    x = INITIAL_AMMO_HORIZONTAL_OFFSET + (i * (DISTANCE_BETWEEN_LED_DIGITS + LED_DIGIT_WIDTH));
                    Point p = new Point(x, y);
                    imageWithLocations[i] = new ImageWithLocation(img, p);
                }
            }
            else
            {
                int currentDigit = 0;
                for (int i = 0; i < AMMO_DIGIT_COUNT; i++)
                {
                    Image img;
                    if (AMMO_DIGIT_COUNT - i > ammoString.Length)
                    {
                        img = _digitLEDs['x'];
                    }
                    else
                    {
                        img = _digitLEDs[ammoString[currentDigit++]];
                    }
                    x = INITIAL_AMMO_HORIZONTAL_OFFSET + (i * (DISTANCE_BETWEEN_LED_DIGITS + LED_DIGIT_WIDTH));
                    Point location = new Point(x, y);
                    imageWithLocations[i] = new ImageWithLocation(img, location);
                }
            }

            return imageWithLocations;
        }

        private ImageWithLocation[] GetLifeAmountImages()
        {
            ImageWithLocation[] imageWithLocations = new ImageWithLocation[0];
            if (Keen.Lives < 0)
            {
                Image img = _digitLEDs['0'];
                Point location = new Point(INITIAL_LIFE_HORIZONTAL_OFFSET, LIFE_VERTICAL_OFFSET);
                ImageWithLocation imageWithLocation = new ImageWithLocation(img, location);
                return new ImageWithLocation[] { imageWithLocation };
            }
            string livesString = _keen.Lives.ToString();

            imageWithLocations = new ImageWithLocation[LIFE_DIGIT_COUNT];
            int x = INITIAL_LIFE_HORIZONTAL_OFFSET;
            int y = LIFE_VERTICAL_OFFSET;
            if (_keen.Lives > 99)
            {
                Image img = _digitLEDs['9'];
                for (int i = 0; i < imageWithLocations.Length; i++)
                {
                    x = INITIAL_LIFE_HORIZONTAL_OFFSET + (i * (DISTANCE_BETWEEN_LED_DIGITS + LED_DIGIT_WIDTH));
                    Point p = new Point(x, y);
                    imageWithLocations[i] = new ImageWithLocation(img, p);
                }
            }
            else
            {
                int currentDigit = 0;
                for (int i = 0; i < LIFE_DIGIT_COUNT; i++)
                {
                    Image img;
                    if (LIFE_DIGIT_COUNT - i > livesString.Length)
                    {
                        img = _digitLEDs['x'];
                    }
                    else
                    {
                        img = _digitLEDs[livesString[currentDigit++]];
                    }
                    x = INITIAL_LIFE_HORIZONTAL_OFFSET + (i * (DISTANCE_BETWEEN_LED_DIGITS + LED_DIGIT_WIDTH));
                    Point location = new Point(x, y);
                    imageWithLocations[i] = new ImageWithLocation(img, location);
                }
            }

            return imageWithLocations;
        }

        public CommanderKeen Keen
        {
            get
            {
                return _keen;
            }
            set
            {
                _keen = value;
                if (_keen != null)
                {
                    UpdateScoreBoard();
                }
            }
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

    struct ImageWithLocation
    {
        public ImageWithLocation(Image image, Point location)
        {
            this.Image = image;
            this.Location = location;
        }
        public Image Image { get; }

        public Point Location { get; }
    }
}
