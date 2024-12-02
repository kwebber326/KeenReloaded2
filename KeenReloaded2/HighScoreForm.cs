using KeenReloaded.Framework.Utilities;
using KeenReloaded2.DialogWindows;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Entities.Statistics.HighScores;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Utilities.HighScoreFactory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2
{
    public partial class HighScoreForm : Form
    {
        private bool _canClose = false;
        Timer _closeTimer = new Timer();
        IHighScore _newHighScore;
        private readonly string _gameMode;
        private readonly string _mapName;
        private readonly IHighScoreUtility _highScoreUtility;
        private const int INITIAL_VERTICAL_OFFSET = 100;
        private const int VERTICAL_OFFSET = 30;
        private const int PLAYER_NAME_HORIZONTAL_OFFSET = 64;
        private const int SCORE_HORIZONTAL_OFFSET = 408;
        private const int HIGH_SCORE_MAX_NUMBER_OF_ENTRIES = 8;

        public HighScoreForm(string gameMode, string mapName)
        {
            InitializeComponent();
            _gameMode = gameMode;
            _mapName = mapName;
            _closeTimer.Interval = 2000;
            _closeTimer.Tick += _closeTimer_Tick;
            _highScoreUtility = HighScoreFactory.Generate(_gameMode);
        }

        public HighScoreForm(IHighScore highScore, string gameMode, string mapName)
        {
            InitializeComponent();
            _newHighScore = highScore;
            _gameMode = gameMode;
            _mapName = mapName;
            _closeTimer.Interval = 2000;
            _closeTimer.Tick += _closeTimer_Tick;
            _highScoreUtility = HighScoreFactory.Generate(_gameMode);
        }

        private void _closeTimer_Tick(object sender, EventArgs e)
        {
            _canClose = true;
        }

        private void HighScoreForm_Load(object sender, EventArgs e)
        {
            WriteHighScoresToBoard();
            this.Size = pbHighScoreImage.Size;
            _closeTimer.Start();
        }

        private void HighScoreForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (_canClose)
            {
                this.Close();
            }
        }

        private void WriteHighScoresToBoard()
        {
            try
            {
                //get high score list
                var highScores = _highScoreUtility.ReadHighScores(_mapName)
                    .Take(HIGH_SCORE_MAX_NUMBER_OF_ENTRIES).ToList();
                if (_newHighScore != null)
                {
                    //insert new high score to list and determine if the player achieved a high score
                    highScores.Add(_newHighScore);
                    var min = highScores.Min(h => h.Value);
                    if (highScores.Count > HIGH_SCORE_MAX_NUMBER_OF_ENTRIES)
                    {
                        var firstMin = highScores.FirstOrDefault(h => h.Value?.ToString() == min?.ToString());
                        highScores.Remove(firstMin);
                    }
                   
                    //if the player achieved a high score, prompt the user to enter their name
                    if (highScores.Contains(_newHighScore))
                    {
                        //use prompt to get their name and write the new list to the scores
                        KeenReloadedTextInputDialog highScoreUserNameDialog = new KeenReloadedTextInputDialog("You've achieved a high score!\n Enter your name here:");
                        highScoreUserNameDialog.ShowDialog();
                        _newHighScore.PlayerName = highScoreUserNameDialog.UserNameText;
                        _highScoreUtility.WriteHighScores(highScores, _mapName);
                    }
                    //TODO: write player stats
                }
                highScores = _highScoreUtility.GetSortedList(highScores)
                    .Take(HIGH_SCORE_MAX_NUMBER_OF_ENTRIES).ToList();
                //use image writing utility to write the image version of the names/scores of every player
                if (_highScoreUtility.WriteHighScores(highScores, _mapName))
                {
                    WriteHighScoresOnBoard(highScores);
                }
                else
                {
                    MessageBox.Show("Error writing high scores", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                MessageBox.Show("Error retrieving high scores");
            }
        }

        private void WriteHighScoresOnBoard(List<IHighScore> scores)
        {
            int x1 = PLAYER_NAME_HORIZONTAL_OFFSET, x2 = SCORE_HORIZONTAL_OFFSET;
            int y = INITIAL_VERTICAL_OFFSET;
            List<LocatedImage> characters = new List<LocatedImage>();

            var characterMapping = ImageToCharacterFactory.CharacterImageMapping;
            for (int i = 0; i < scores.Count; i++)
            {
                Point p1 = new Point(x1, y);
                Point p2 = new Point(x2, y);

                if (!string.IsNullOrEmpty(scores[i].PlayerName))
                {
                    AddCharactersFromStartingPosition(p1, scores[i].PlayerName, characters, characterMapping);
                }
                string scoreStr = scores[i].Value?.ToString();
                if (!string.IsNullOrEmpty(scoreStr))
                {
                    AddCharactersFromStartingPosition(p2, scoreStr, characters, characterMapping);
                }

                y += VERTICAL_OFFSET;
            }
            Size canvasSize = pbHighScoreImage.Size;
            Image[] extraImages = characters.Select(c => c.Image).ToArray();
            Point[] locations = characters.Select(c => c.Location).ToArray();

            pbHighScoreImage.Image = BitMapTool.DrawImagesOnCanvas(canvasSize, pbHighScoreImage.Image, canvasSize, extraImages, locations);
        }

        private void AddCharactersFromStartingPosition(Point startPos, string value, List<LocatedImage> characters, Dictionary<char, Image> characterMapping)
        {
            int letterMargin = 2;
            int x = startPos.X;
            int y = startPos.Y;
            if (string.IsNullOrEmpty(value))
                return;

            if (!string.IsNullOrEmpty(value))
            {
                foreach (char c in value)
                {
                    if (characterMapping.TryGetValue(c, out Image image))
                    {
                        LocatedImage character = new LocatedImage()
                        {
                            Location = startPos,
                            Image = image
                        };
                        characters.Add(character);
                        x = x + (image?.Width ?? 16) + letterMargin;
                        startPos = new Point(x, y);
                    }
                }
            }
        }
    }
}
