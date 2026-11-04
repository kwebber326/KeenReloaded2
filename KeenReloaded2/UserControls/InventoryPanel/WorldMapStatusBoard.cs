using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class WorldMapStatusBoard : UserControl
    {
        Dictionary<char, Image> _characterMapping = ImageToCharacterFactory.CharacterImageMapping;
        Dictionary<char, Image> _digitLEDMapping = ImageToCharacterFactory.LEDDigitMapping;
        PictureBox _headerTextImage = new PictureBox()
        {
            SizeMode = PictureBoxSizeMode.AutoSize,
            BackColor = Color.Transparent
        };

        PictureBox _keensTextImage = new PictureBox()
        {
            SizeMode = PictureBoxSizeMode.AutoSize,
            BackColor = Color.Transparent
        };

        PictureBox _scoreTextImage = new PictureBox()
        {
            SizeMode = PictureBoxSizeMode.AutoSize,
            BackColor = Color.Transparent
        };
        private const int LETTER_SPACING = 2;
        private const int NUMBER_SPACING = 4;
        private const int WHITE_SPACE_DISTANCE = 16;

        private readonly Rectangle _headerTextRectangle = new Rectangle(120, 120, 600, 32);
        public WorldMapStatusBoard()
        {
            InitializeComponent();

        }

        private void WorldMapStatusBoard_KeyUp(object sender, KeyEventArgs e)
        {

        }

        public void WritePlayerStatus(string worldMapName, WorldMapObjectiveData objectiveData, CommanderKeen player)
        {
            //Add the name of the world map in the header text box
            WriteWorldMapNameHeader(worldMapName);
            //ADD the number of lives/score the player has
            WritePlayerLives(player?.Lives ?? 3);
            WritePlayerScore(player?.Points ?? 0);
            //TODO: Show in sequential pictoral format the number of items obtained/objectives completed.
        }

        #region writing map title
        private void WriteWorldMapNameHeader(string headerText)
        {
            pbState.Controls.Remove(_headerTextImage);
            if (string.IsNullOrWhiteSpace(headerText))
                return;


            int x = 0;
            int y = 0;
            int totalWidth = 0;
            int maxHeight = 0;
            List<Image> images = new List<Image>();
            List<Point> locations = new List<Point>();
            //gather images from dictionary and combine them sequentially into one image

            foreach (char character in headerText)
            {
                if (_characterMapping.TryGetValue(character, out Image img))
                {
                    if (img == null)
                    {
                        x += WHITE_SPACE_DISTANCE;
                        totalWidth += WHITE_SPACE_DISTANCE;
                        continue;
                    }
                    Point p = new Point(x, y);
                    images.Add(img);
                    if (img.Height > maxHeight)
                    {
                        maxHeight = img.Height;
                    }
                    locations.Add(p);
                    int characterWidth = (img.Width + LETTER_SPACING);
                    x += characterWidth;
                    totalWidth += characterWidth;
                }
            }

            Image finalImage = BitMapTool.DrawImagesOnCanvas(
                new Size(totalWidth, maxHeight), null,
                images.ToArray(), locations.ToArray());
            _headerTextImage.Image = finalImage;
            _headerTextImage.BackColor = Color.Transparent;
            // center the image
            _headerTextImage.Location = GetHeaderCenterPointBasedOnImage(finalImage);
            pbState.Controls.Add(_headerTextImage);
            _headerTextImage.BringToFront();

        }

        private Point GetHeaderCenterPointBasedOnImage(Image image)
        {
            int startX = _headerTextRectangle.X;
            int startY = _headerTextRectangle.Y;

            int x = startX + (_headerTextRectangle.Width / 2) - (image.Width / 2);
            int y = startY + (_headerTextRectangle.Height / 2) - (image.Height / 2);

            return new Point(x, y);
        }
        #endregion

        #region Player Stats

        private void WritePlayerLives(int livesCount)
        {
            if (livesCount > 99)
                livesCount = 99;

            int x = 482;
            int y = 207;
            var position = new Point(x, y);
            WriteLEDImagesFromStartPosition(livesCount, position, _keensTextImage, new Size(20, 26));
        }

        private void WritePlayerScore(long score)
        {
            if (score > 99999999)
                score = 99999999;

            int x = 350;
            int y = 208;
            var position = new Point(x, y);
            WriteLEDImagesFromStartPosition(score, position, _scoreTextImage, new Size(20, 28));
        }

        private void WriteLEDImagesFromStartPosition(long number, Point p, PictureBox imageContainer, Size characterSize)
        {
            string text = number.ToString();
            int digits = text.Length;
            int totalWidth = 0;
            int maxHeight = 0;

            pbState.Controls.Remove(imageContainer);
            List<Image> images = new List<Image>();
            List<Point> locations = new List<Point>();
            //capture digit images
            for (int i = digits - 1; i >= 0; i--)
            {
                char key = text[i];
                Image img = _digitLEDMapping.TryGetValue(key, out img)
                    ? img : _digitLEDMapping['x'];

                img = BitMapTool.ResizeImage(img, characterSize.Width, characterSize.Height);

                if (img.Height > maxHeight)
                {
                    maxHeight = img.Height;
                }
                images.Add(img);
                totalWidth += img.Width + NUMBER_SPACING;
            }
            //calculate x/y position withing picture box
            int x = totalWidth;
            for (int i = images.Count - 1; i >= 0; i--)
            {
                int movement = images[i].Width + NUMBER_SPACING;
                x -= movement;
                p = new Point(p.X - movement, p.Y);
                locations.Add(new Point(x, 0));
            }

            Image blackBackground = BitMapTool.ResizeImage(
                Properties.Resources.keen4_tar_depth, totalWidth, maxHeight);

            Image finalImage = BitMapTool.DrawImagesOnCanvas(
                new Size(totalWidth, maxHeight),
                blackBackground, images.ToArray(), locations.ToArray());

            imageContainer.Location = p;
            imageContainer.Image = finalImage;
            pbState.Controls.Add(imageContainer);
        }

        #endregion
    }
}
