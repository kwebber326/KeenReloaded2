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
        PictureBox _headerTextImage = new PictureBox()
        {
            SizeMode = PictureBoxSizeMode.AutoSize,
            BackColor = Color.Transparent
        };
        private const int LETTER_SPACING = 2;
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
            //TODO: ADD the number of lives the player has
            //TODO: Show in sequential pictoral format the number of items obtained/objectives completed.
        }

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
    }
}
