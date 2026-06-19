using KeenReloaded2.Constants;
using KeenReloaded2.DialogWindows;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using KeenReloaded2.UserControls.InventoryPanel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace KeenReloaded2
{
    public partial class WorldMapPlayerForm : Form
    {
        private WorldMapPlayer _player;
        private CommanderKeen _playerState;
        private CommanderKeenGame _game;
        private const int VIEW_RADIUS = GeneralGameConstants.VIEW_RADIUS;
        private const int MAX_VISION_OFFSET = 10;
        private const int VISION_OFFSET_COEFFICIENT = 10;
        private readonly bool _mapMakerMove;
        private int _maxVisionY;
        private int _maxVisionX;
        private bool _paused;
        private Timer _gameUpdateTimer;
        private KeenReloadedLoadingWindow _loadingWindow;

        private void UpdateViewRectangle()
        {
            // pnlGameWindow.AutoScroll = false;
            int x = _player.HitBox.X - VIEW_RADIUS;
            int y = (_player.HitBox.Y - VIEW_RADIUS < 0 ? 0 : _player.HitBox.Y - VIEW_RADIUS);

            if (Math.Abs(x) > _maxVisionX)
                x = x < 0 ? _maxVisionX * -1 : _maxVisionX;
            if (Math.Abs(y) > _maxVisionY)
                y = y < 0 ? _maxVisionY * -1 : _maxVisionY;

            pnlGameWindow.AutoScrollPosition = new Point(x, y);
        }

        private Rectangle GetViewRectangle()
        {
            int x = pnlGameWindow.DisplayRectangle.X * -1;
            int y = pnlGameWindow.DisplayRectangle.Y * -1;
            int width = pnlGameWindow.Width;
            int height = pnlGameWindow.Height;

            return new Rectangle(x, y, width, height);
        }

        private void _game_BackgroundImageRedrawn(object sender, EventArgs e)
        {
            pbBackgroundImage.Image = _game.BackGroundImage;
        }

        private void _gameUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!_paused && !_game.IsDisposed)
            {
                var rectangle = GetViewRectangle();

                pbGameImage.Image = _game.UpdateGame(rectangle);
            }
        }

        public WorldMapPlayerForm()
        {
            InitializeComponent();
        }

        public WorldMapPlayerForm(MapMakerData data, bool mapMakerMode)
        {
            InitializeComponent();
            _loadingWindow = new KeenReloadedLoadingWindow();
            InitializeGameData(data);
            _mapMakerMove = mapMakerMode;
            pbBackgroundImage.SendToBack();
            pbGameImage.Parent = pbBackgroundImage;
            pbBackgroundImage.Image = _game.BackGroundImage;
            pbGameImage.Location = new Point(0, 0);
           
        }

        private void WorldMapPlayerForm_Load(object sender, EventArgs e)
        {
            InitializeGameState();
        }

        private void _keen_KeenMoved(object sender, EventArgs e)
        {
            UpdateViewRectangle();
        }

        private void PnlGameWindow_MouseWheel(object sender, MouseEventArgs e)
        {
            this.UpdateViewRectangle();
        }

        private void InitializeGameData(MapMakerData data)
        {
            if (_game != null)
            {
                _game.BackgroundImageRedrawn -= _game_BackgroundImageRedrawn;
                _game.LevelEntered -= _game_LevelEntered;
                _game.Dispose();
            }

            _game = new CommanderKeenGame(data);
            pbBackgroundImage.Image = _game.BackGroundImage;
            _game.BackgroundImageRedrawn += _game_BackgroundImageRedrawn;
            _game.LevelEntered += _game_LevelEntered;
            var gameObjects = data.MapData.Select(d => d.GameObject);

            _player = gameObjects.OfType<WorldMapPlayer>().FirstOrDefault();

            _maxVisionY = _game.Map.MapSize.Height - VIEW_RADIUS;
            _maxVisionX = _game.Map.MapSize.Width - VIEW_RADIUS;
        }

        private void _game_LevelEntered(object sender, EventArgs e)
        {
            _gameUpdateTimer.Stop();
            IWorldMapLevel level = sender as IWorldMapLevel;
            if (level == null)
            {
                KeenReloadedErrorMessageWindow errorMessageWindow =
                    new KeenReloadedErrorMessageWindow("Level could not be loaded");
                errorMessageWindow.ShowDialog();
                _player.ClearAllKeyPressStates();
                _gameUpdateTimer.Start();
                return;
            }

            string levelName = level.LevelName;
            string levelEntryText = level.LevelEntryText;
            string episode = level.Episode;
            _player.ClearAllKeyPressStates();


           _loadingWindow.LoadLevel(levelName, episode, levelEntryText);
            if (_loadingWindow.DialogResult == DialogResult.OK)
            {
                Form1 form1 = new Form1(
                    MainMenuConstants.OPTION_LABEL_NORMAL_MODE,
                    _loadingWindow.MapData, true, true);
                form1.ShowDialog();
                _gameUpdateTimer.Start();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return base.IsInputKey(keyData);
        }

        private void DetachEvents()
        {
            if (_player != null)
            {
                _player.Moved -= _keen_KeenMoved;
            }
            if (_gameUpdateTimer != null)
            {
                _gameUpdateTimer.Tick -= _gameUpdateTimer_Tick;
            }
            pnlGameWindow.MouseWheel -= PnlGameWindow_MouseWheel;
            this.MouseWheel -= PnlGameWindow_MouseWheel;

            if (_game != null && !_game.IsDisposed)
            {
                _game.BackgroundImageRedrawn -= _game_BackgroundImageRedrawn;
                _game.LevelEntered -= _game_LevelEntered;
            }
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

        private void DisposeBitmaps()
        {
            // Only dispose if the image hasn't already been disposed by another component
            try
            {
                var tmp = pbGameImage.Image;
                pbGameImage.Image = null;
                if (tmp != null && !IsImageDisposed(tmp))
                    tmp.Dispose();
            }
            catch { /* Already disposed */ }

            try
            {
                var tmp2 = pbBackgroundImage.Image;
                pbBackgroundImage.Image = null;
                if (tmp2 != null && !IsImageDisposed(tmp2))
                    tmp2.Dispose();
            }
            catch { /* Already disposed */ }
        }

        private bool IsImageDisposed(Image image)
        {
            try
            {
                // Try to access a property - will throw if disposed
                var _ = image.Width;
                return false;
            }
            catch
            {
                return true;
            }
        }

        private void InitializeGameState()
        {
            // Dispose old timer if it exists
            if (_gameUpdateTimer != null)
            {
                _gameUpdateTimer.Stop();
                _gameUpdateTimer.Dispose();
            }

            _gameUpdateTimer = new Timer();
            _gameUpdateTimer.Interval = 50;
            _gameUpdateTimer.Tick += _gameUpdateTimer_Tick;
            _gameUpdateTimer.Start();
            _player.Moved += _keen_KeenMoved;
            UpdateViewRectangle();
            pnlGameWindow.AutoScroll = true;
            pnlGameWindow.MouseWheel += PnlGameWindow_MouseWheel;
            this.MouseWheel += PnlGameWindow_MouseWheel;
        }

        private void WorldMapPlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop timers
            if (_gameUpdateTimer != null)
            {
                _gameUpdateTimer.Stop();
                _gameUpdateTimer.Dispose();
            }

            // Detach all event handlers
            DetachEvents();

            //dispose sound player
           
        }

        private void WorldMapPlayerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DisposeBitmaps();

            if (_game != null && !_game.IsDisposed)
                _game.Dispose();
        }

        private void WorldMapPlayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            _game.SetKeyPressed(e.KeyCode.ToString(), true);
            if (e.KeyCode == Keys.Alt)
            {
                e.SuppressKeyPress = true;
                this.Focus();
            }
        }

        private void WorldMapPlayerForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                OpenGameIODialog();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                OpenGameStateDialog();
            }
            else
            {
                _game.SetKeyPressed(e.KeyCode.ToString(), false);
            }
        }

        private void OpenGameStateDialog()
        {
            //TODO: make game state dialog window and display it here
        }

        private void OpenGameIODialog()
        {
            _paused = !_paused;
            //TODO: Make the save/load/new game dialog and display it here
        }
    }
}
