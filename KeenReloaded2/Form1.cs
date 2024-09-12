using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.AltCharacters;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Utilities;
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
    public partial class Form1 : Form
    {
        private readonly string _gameMode;
        private Timer _gameUpdateTimer = new Timer();
        private CommanderKeenGame _game;
        private bool _paused;
        private CommanderKeen _keen;
        private readonly string _selectedCharacter;
        private int _currentVisionOffset;
        private int _maxVisionY;
        private int _maxVisionX;
        private const int VIEW_RADIUS = 400;
        private const int MAX_VISION_OFFSET = 10;
        private const int VISION_OFFSET_COEFFICIENT = 10;
        private const int HIGH_SCORE_LENGTH = 8;
        private bool _levelCompleted;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string gameMode, MapMakerData data)
        {
            InitializeComponent();
            _gameMode = gameMode;
            _game = new CommanderKeenGame(data);
            var gameObjects = data.MapData.Select(d => d.GameObject);
            _keen = gameObjects.OfType<CommanderKeen>().FirstOrDefault();
          

            string characterName = FileIOUtility.LoadSavedCharacterSelection();
            if (characterName != MainMenuConstants.Characters.COMMANDER_KEEN)
            {
                _game.ChangeKeenSkin(characterName, out CommanderKeen keen);
                _keen = keen;
            }
            CurrentPlayerList.Players.Clear();
            CurrentPlayerList.Players.Add(_keen);
            inventoryPanel1.Keen = _keen;
            inventoryPanel1.ShowFlagInventory = gameMode == MainMenuConstants.OPTION_LABEL_CTF_MODE;
            _maxVisionY = _game.Map.MapSize.Height - VIEW_RADIUS;
            _maxVisionX = _game.Map.MapSize.Width - VIEW_RADIUS;
        }

        private void InitializeGameState()
        {
            _gameUpdateTimer = new Timer();
            _gameUpdateTimer.Interval = 50;
            _gameUpdateTimer.Tick += _gameUpdateTimer_Tick;
            _gameUpdateTimer.Start();
            _keen.KeenMoved += _keen_KeenMoved;
            UpdateViewRectangle();
            pnlGameWindow.AutoScroll = true;
            pnlGameWindow.MouseWheel += PnlGameWindow_MouseWheel;
            this.MouseWheel += PnlGameWindow_MouseWheel;
        }

        private void PnlGameWindow_MouseWheel(object sender, MouseEventArgs e)
        {
            this.UpdateViewRectangle();
        }

        private void _keen_KeenMoved(object sender, EventArgs e)
        {
            UpdateViewRectangle();
        }

        private void _gameUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!_paused && !_levelCompleted)
            {
                pbGameImage.Image = _game.UpdateGame();
                if (_keen.IsLookingUp)
                {
                    if (_currentVisionOffset * -1 < MAX_VISION_OFFSET)
                    {
                        _currentVisionOffset--;
                        int x = _keen.HitBox.X - VIEW_RADIUS;
                        int y = _keen.HitBox.Y - VIEW_RADIUS + (_currentVisionOffset * VISION_OFFSET_COEFFICIENT);
                        pnlGameWindow.AutoScrollPosition = new Point(x, y);
                    }
                }
                else if (_keen.IsLookingDown)
                {
                    if (_currentVisionOffset < MAX_VISION_OFFSET)
                    {
                        _currentVisionOffset++;
                        int x = _keen.HitBox.X - VIEW_RADIUS;
                        int y = (_keen.HitBox.Y - VIEW_RADIUS < 0 ? 0 : _keen.HitBox.Y - VIEW_RADIUS) + (_currentVisionOffset * VISION_OFFSET_COEFFICIENT);
                        if (Math.Abs(y) > _maxVisionY)
                            y = y < 0 ? _maxVisionY * -1 : _maxVisionY;
                        pnlGameWindow.AutoScrollPosition = new Point(x, y);
                    }
                }
                else if (_currentVisionOffset != 0)
                {
                    _currentVisionOffset = _currentVisionOffset < 0 ? _currentVisionOffset + 1 : _currentVisionOffset - 1;
                    int x = _keen.HitBox.X - VIEW_RADIUS;
                    int y = (_keen.HitBox.Y - VIEW_RADIUS < 0 ? 0 : _keen.HitBox.Y - VIEW_RADIUS) + (_currentVisionOffset * VISION_OFFSET_COEFFICIENT);

                    pnlGameWindow.AutoScrollPosition = new Point(x, y);
                }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return base.IsInputKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    KeyEventArgs args = new KeyEventArgs(keyData);
                    base.OnKeyDown(args);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGameState();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameUpdateTimer.Stop();
            _gameUpdateTimer.Tick -= _gameUpdateTimer_Tick;
            _gameUpdateTimer.Dispose();
            _game.Dispose();
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _game.SetKeyPressed(e.KeyCode.ToString(), true);
            if (e.KeyCode == Keys.Alt)
            {
                e.SuppressKeyPress = true;
                this.Focus();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                OpenDialog(e.KeyCode);
            }
            else
            {
                _game.SetKeyPressed(e.KeyCode.ToString(), false);
            }
        }

        private void OpenDialog(Keys key)
        {

            switch (key)
            {
                case Keys.Escape:
                    //_paused = true;
                    //var messageWindow = new KeenReloadedYesNoDialogWindow("Are you sure you want to quit?", false);

                    //var dialogResult = messageWindow.ShowDialog();
                    //if (dialogResult == DialogResult.No)
                    //{
                    //    _paused = false;
                    //}
                    //else
                    //{
                    //    this.Close();
                    //}
                    break;
            }
        }

        private void UpdateViewRectangle()
        {
            if (!_keen.IsDead())
            {
               // pnlGameWindow.AutoScroll = false;
                int x = _keen.HitBox.X - VIEW_RADIUS;
                int y = (_keen.HitBox.Y - VIEW_RADIUS < 0 ? 0 : _keen.HitBox.Y - VIEW_RADIUS) + (_currentVisionOffset * VISION_OFFSET_COEFFICIENT);

                if (Math.Abs(x) > _maxVisionX)
                    x = x < 0 ? _maxVisionX * -1 : _maxVisionX;
                if (Math.Abs(y) > _maxVisionY)
                    y = y < 0 ? _maxVisionY * -1 : _maxVisionY;

                pnlGameWindow.AutoScrollPosition = new Point(x, y);
            }
        }
    }
}
