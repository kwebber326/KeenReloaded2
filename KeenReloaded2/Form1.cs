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
using KeenReloaded2.DialogWindows;
using KeenReloaded2.Entities.Statistics.HighScores;
using System.Diagnostics;

namespace KeenReloaded2
{
    public partial class Form1 : Form
    {
        private readonly string _gameMode;
        private readonly bool _inMapMakerMode;
        private Timer _gameUpdateTimer = new Timer();
        private Stopwatch _levelCompletionTimer = new Stopwatch();
        private CommanderKeenGame _game;
        private bool _paused;
        private CommanderKeen _keen;
        private int _currentVisionOffset;
        private int _maxVisionY;
        private int _maxVisionX;
        private const int VIEW_RADIUS = 440;
        private const int MAX_VISION_OFFSET = 10;
        private const int VISION_OFFSET_COEFFICIENT = 10;
        private bool _levelCompleted;

        private const int INITIAL_VIEW_RECT_UPDATES = 3;
        private int _rectUpdates;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string gameMode, MapMakerData data, bool inMapMakerMode)
        {
            InitializeComponent();
            _gameMode = gameMode;
            _inMapMakerMode = inMapMakerMode;
            InitializeGameData(gameMode, data, false);
            //unsubscribe cannot execute on form closing due to changing a callback collection during iteration
            //Here, we will clean out any previous subscriptions before subscribing
            EventStore<bool>.UnSubscribe(
               MapMakerConstants.EventStoreEventNames.KEEN_DISAPPEAR_DEATH,
               Keen_Disappear_Death);
            EventStore<bool>.Subscribe(
                MapMakerConstants.EventStoreEventNames.KEEN_DISAPPEAR_DEATH,
                Keen_Disappear_Death);
            pnlGameWindow.VerticalScroll.Maximum = 280;
            pnlGameWindow.HorizontalScroll.Maximum = 800;
        }

        private void InitializeGameData(string gameMode, MapMakerData data, bool isReset)
        {
            if (_game != null)
            {
                _game.BackgroundImageRedrawn -= _game_BackgroundImageRedrawn;
                _game.Dispose();
            }

            _game = new CommanderKeenGame(data);
            pbBackgroundImage.Image = _game.BackGroundImage;
            _game.BackgroundImageRedrawn += _game_BackgroundImageRedrawn;
            var gameObjects = data.MapData.Select(d => d.GameObject);

            int lives = _keen?.Lives ?? 0;
            int drops = _keen?.Drops ?? 0;
            long points = _keen?.Points ?? 0;
            var weapons = _gameMode != MainMenuConstants.OPTION_LABEL_NORMAL_MODE && isReset ? _keen.Weapons : null;
            var shield = inventoryPanel1.Shield;
            _keen = gameObjects.OfType<CommanderKeen>().FirstOrDefault();

            string characterName = FileIOUtility.LoadSavedCharacterSelection();
            if (characterName != MainMenuConstants.Characters.COMMANDER_KEEN)
            {
                _game.ChangeKeenSkin(characterName, out CommanderKeen keen);
                _keen = keen;
            }

            if (isReset)
            {
                if (_gameMode != MainMenuConstants.OPTION_LABEL_NORMAL_MODE)
                {
                    _keen.ResetKeenAfterDeath(lives, drops, points, weapons, shield);
                }
                else
                {
                    _keen.ResetKeenAfterDeath(lives, drops, 0);
                }
                inventoryPanel1.Reset();
            }

            CurrentPlayerList.Players.Clear();
            CurrentPlayerList.Players.Add(_keen);

            inventoryPanel1.Keen = _keen;
            inventoryPanel1.ShowFlagInventory = gameMode == MainMenuConstants.OPTION_LABEL_CTF_MODE;
            _maxVisionY = _game.Map.MapSize.Height - VIEW_RADIUS;
            _maxVisionX = _game.Map.MapSize.Width - VIEW_RADIUS;

            if (_gameMode == MainMenuConstants.OPTION_LABEL_NORMAL_MODE && !isReset)
            {
                _levelCompletionTimer.Start();
            }
        }

        private void InitializeGameState()
        {
            _gameUpdateTimer = new Timer();
            _gameUpdateTimer.Interval = 50;
            _gameUpdateTimer.Tick += _gameUpdateTimer_Tick;
            _gameUpdateTimer.Start();
            _keen.KeenMoved += _keen_KeenMoved;
            _keen.KeenDied += _keen_KeenDied;
            _keen.KeenLevelCompleted += _keen_KeenLevelCompleted;
            UpdateViewRectangle();
            pnlGameWindow.AutoScroll = true;
            pnlGameWindow.MouseWheel += PnlGameWindow_MouseWheel;
            this.MouseWheel += PnlGameWindow_MouseWheel;
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            if (_gameMode == MainMenuConstants.OPTION_LABEL_NORMAL_MODE)
            {
                lblStopwatch.Visible = true;
                SetTimerLabel();
            }
            else
            {
                lblStopwatch.Visible = false;
            }
        }

        private void SetTimerLabel()
        {
            var timerStr = _levelCompletionTimer.Elapsed.ToString();
            string mapName = _game?.Map?.MapName ?? string.Empty;
            string displayText = $"{mapName} - {timerStr}";
            lblStopwatch.Text = displayText;
        }

        private void Keen_Disappear_Death(object sender, ControlEventArgs.ControlEventArgs<bool> e)
        {
            ProcessPlayerDeath();
        }

        private void _keen_KeenLevelCompleted(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {
            _gameUpdateTimer.Stop();
            if (!_levelCompleted)
            {
                _levelCompleted = true;
                _levelCompletionTimer.Stop();
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.KEEN4_EXIT);
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.KEEN_LEVEL_COMPLETE,
                    string.Empty);
                RemoveKeenFromGame();
                KeenReloadedMessageWindow window = new KeenReloadedMessageWindow("Level Completed!");
                window.ShowDialog();
                ExecuteShutdownProtocol();
            }
        }

        private void RemoveKeenFromGame()
        {
            var keenObject = _game.Map.MapData.FirstOrDefault(o => o.GameObject == _keen);
            if (keenObject != null)
            {
                _game.Map.MapData.Remove(keenObject);
                _game.UpdateGame(GetViewRectangle());
            }
        }

        private void ResetGame()
        {
            DetachEvents();
            pbGameImage.Image = null;
            var mapMakerData = MapUtility.LoadMapData(_game.Map.MapPath);
            InitializeGameData(_gameMode, mapMakerData, true);
            InitializeGameState();

        }

        private void DetachEvents()
        {
            _keen.KeenDied -= _keen_KeenDied;
            _keen.KeenLevelCompleted -= _keen_KeenLevelCompleted;
            _keen.KeenMoved -= _keen_KeenMoved;
            _gameUpdateTimer.Tick -= _gameUpdateTimer_Tick;
            pnlGameWindow.MouseWheel -= PnlGameWindow_MouseWheel;
            this.MouseWheel -= PnlGameWindow_MouseWheel;
        }

        private void _game_BackgroundImageRedrawn(object sender, EventArgs e)
        {
            pbBackgroundImage.Image = _game.BackGroundImage;
        }

        private void _keen_KeenDied(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {

        }

        private void ShowRestartDialogPrompt()
        {
            KeenReloadedYesNoDialogWindow yesNoDialogWindow = new KeenReloadedYesNoDialogWindow("You failed. Try again?", true);
            var dialogResult = yesNoDialogWindow.ShowDialog();
            if (dialogResult == DialogResult.Yes)
            {
                ResetGame();
            }
            else
            {
                ExecuteShutdownProtocol();
            }
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

            if (_rectUpdates < INITIAL_VIEW_RECT_UPDATES)
            {
                UpdateViewRectangle();
                _rectUpdates++;
            }

            if (!_paused && !_levelCompleted)
            {
                var rectangle = GetViewRectangle();
                if (_gameMode == MainMenuConstants.OPTION_LABEL_NORMAL_MODE)
                    SetTimerLabel();

                pbGameImage.Image = _game.UpdateGame(rectangle);

                if (_keen.IsDead())
                {
                    if (IsKeenOutOfVisibleRange())
                    {
                        ProcessPlayerDeath();
                    }
                    return;
                }

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

        private void ProcessPlayerDeath()
        {
            _gameUpdateTimer.Stop();
            if (_keen.Lives >= 0)
                ShowRestartDialogPrompt();
            else
                InitiateGameOverProtocol();
        }

        private void InitiateGameOverProtocol()
        {
            KeenReloadedMessageWindow messageWindow = new KeenReloadedMessageWindow("Game Over");
            messageWindow.ShowDialog();
            ExecuteShutdownProtocol();
        }

        private void ShowHighScoreBoard()
        {
            IHighScore score = this.GetPlayerScore();
            HighScoreForm highScoreForm = score != null
                ? new HighScoreForm(score, _gameMode, _game?.Map?.MapName)
                : new HighScoreForm(_gameMode, _game?.Map?.MapName);
            highScoreForm.ShowDialog();
        }

        private IHighScore GetPlayerScore()
        {
            string mapName = _game?.Map?.MapName;
            switch (_gameMode)
            {
                case MainMenuConstants.OPTION_LABEL_CTF_MODE:
                    return new CTFHighScore(string.Empty, mapName, _keen.Points);
                case MainMenuConstants.OPTION_LABEL_KOTH_MODE:
                    return new KOTHHighScore(string.Empty, mapName, _keen.Points);
                case MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE:
                    return new ZombieModeHighScore(string.Empty, mapName, _keen.Points);
                case MainMenuConstants.OPTION_LABEL_NORMAL_MODE:
                    return _levelCompleted ?
                        new NormalModeHighScore(string.Empty, mapName, _levelCompletionTimer.Elapsed)
                       : null;
            }
            return null;
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
            pbBackgroundImage.SendToBack();
            pbGameImage.Parent = pbBackgroundImage;
            pbBackgroundImage.Image = _game.BackGroundImage;
            pbGameImage.Location = new Point(0, 0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameUpdateTimer.Tick -= _gameUpdateTimer_Tick;
            _gameUpdateTimer.Stop();
            if (_game != null && !_game.IsDisposed)
            {
                _game.BackgroundImageRedrawn -= _game_BackgroundImageRedrawn;
                _game.Dispose();
            }
            soundPlayer1.StopMusic();
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
                    _paused = true;
                    _levelCompletionTimer.Stop();
                    var messageWindow = new KeenReloadedYesNoDialogWindow("Are you sure you want to quit?", false);

                    var dialogResult = messageWindow.ShowDialog();
                    if (dialogResult == DialogResult.No)
                    {
                        _paused = false;
                        _levelCompletionTimer.Start();
                    }
                    else
                    {
                        ExecuteShutdownProtocol();
                    }
                    break;
            }
        }

        private void ExecuteShutdownProtocol()
        {
            _levelCompletionTimer.Stop();
            if (!_inMapMakerMode)
            {
                ShowHighScoreBoard();
            }
            this.Close();
        }

        private bool IsKeenOutOfVisibleRange()
        {
            return _keen.HitBox.X >= _game.Map.MapSize.Width || _keen.HitBox.Right <= 0
                || _keen.HitBox.X >= (pnlGameWindow.Width + Math.Abs(pnlGameWindow.AutoScrollPosition.X))
                || _keen.HitBox.Right <= (Math.Abs(pnlGameWindow.AutoScrollPosition.X) - 32)
                || _keen.HitBox.Y >= (pnlGameWindow.Height + Math.Abs(pnlGameWindow.AutoScrollPosition.Y))
                || _keen.HitBox.Y >= _game.Map.MapSize.Height;
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

        private Rectangle GetViewRectangle()
        {
            int x = pnlGameWindow.DisplayRectangle.X * -1;
            int y = pnlGameWindow.DisplayRectangle.Y * -1;
            int width = pnlGameWindow.Width;
            int height = pnlGameWindow.Height;

            return new Rectangle(x, y, width, height);
        }
    }
}
