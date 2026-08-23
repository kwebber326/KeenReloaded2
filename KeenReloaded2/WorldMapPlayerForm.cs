using KeenReloaded2.Constants;
using KeenReloaded2.DialogWindows;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.Statistics.HighScores;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Utilities;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KeenReloaded2
{
    public partial class WorldMapPlayerForm : Form
    {
        private WorldMapPlayer _player;
        private CommanderKeen _playerState;
        private CommanderKeenGame _game;
        private const int VIEW_RADIUS = GeneralGameConstants.VIEW_RADIUS;
        private readonly bool _mapMakerMode;
        private int _maxVisionY;
        private int _maxVisionX;
        private bool _paused;
        private Timer _gameUpdateTimer;
        private KeenReloadedLoadingWindow _loadingWindow;
        private WorldMapObjectiveManager _worldMapObjectiveData
            = new WorldMapObjectiveManager();

        private WorldMapMenuOptionDecision? _menuDecision;

        public WorldMapMenuOptionDecision? MenuDecision => _menuDecision;

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

        public WorldMapPlayerForm(MapMakerData data, bool mapMakerMode, WorldMapObjectiveManager objectiveData = null)
        {
            InitializeComponent();
            _worldMapObjectiveData = objectiveData ?? new WorldMapObjectiveManager();
            _loadingWindow = new KeenReloadedLoadingWindow();
            _loadingWindow.WorldMapLoadError += _loadingWindow_WorldMapLoadError;
            if (MapUtility.LoadWorldMapMusic(data.MapName, out string music))
            {
                _loadingWindow.ChangeSong(music);
            }

            InitializeGameData(data);
            _mapMakerMode = mapMakerMode;
            pbBackgroundImage.SendToBack();
            pbGameImage.Parent = pbBackgroundImage;
            pbBackgroundImage.Image = _game.BackGroundImage;
            pbGameImage.Location = new Point(0, 0);
            _worldMapObjectiveData.GameBeaten += _worldMapObjectiveData_GameBeaten;

        }

        private void _worldMapObjectiveData_GameBeaten(object sender, EventArgs e)
        {
            _worldMapObjectiveData.GameBeaten -= _worldMapObjectiveData_GameBeaten;
            if (!_mapMakerMode)
            {
                IHighScore score = new WorldModeHighScore(string.Empty, _game.Map.MapName, _playerState?.Points ?? 0);
                HighScoreForm highScoreForm = score != null
                    ? new HighScoreForm(score, MainMenuConstants.OPTION_LABEL_WORLD_MODE, _game?.Map?.MapName)
                    : new HighScoreForm(MainMenuConstants.OPTION_LABEL_WORLD_MODE, _game?.Map?.MapName);
                highScoreForm.ShowDialog();
            }
            this.Close();
        }

        private void _loadingWindow_WorldMapLoadError(object sender, EventArgs e)
        {
            HandleMapError("Error loading level");
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
            if (level == null || !_loadingWindow.IsMapValid(level.LevelName))
            {
                HandleMapError("Error loading level");
                return;
            }

            string levelName = level.LevelName;
            string levelEntryText = level.LevelEntryText;
            string episode = level.Episode;
            string music = level.Music;
            _player.ClearAllKeyPressStates();

            _loadingWindow.LoadLevel(levelName, episode, levelEntryText);
            if (_loadingWindow.DialogResult == DialogResult.OK)
            {
                Form1 form1 = new Form1(
                   MainMenuConstants.OPTION_LABEL_NORMAL_MODE,
                   _loadingWindow.MapData, true, true, _playerState, music);
                form1.WorldMapPath = _game.Map.MapPath;
                form1.KeenStateChanged += Form1_KeenStateChanged;
                form1.ShowDialog();
                form1.KeenStateChanged -= Form1_KeenStateChanged;
                _loadingWindow.UnmuteMusic();
                if (form1.LevelCompleted)
                {
                    ProcessLevelCompletion(level, levelName, form1.LoadedMapData);
                    _gameUpdateTimer.Start();
                }
                else if (form1.GameOver)
                {
                    if (form1.MenuDecision == WorldMapMenuOptionDecision.START_NEW)
                    {
                        _menuDecision = WorldMapMenuOptionDecision.START_NEW;
                        this.Close();
                    }
                    else if (form1.MenuDecision == WorldMapMenuOptionDecision.QUIT)
                    {
                        this.Close();
                    }
                    else if (form1.MenuDecision == null)
                    {
                        //TODO: Insert game over animation here and await the closing of the 
                        //gameover animation form before closing
                        this.Close();
                    }
                }  
                else
                {
                    _gameUpdateTimer.Start();
                }
            }
        }

        private void ProcessLevelCompletion(IWorldMapLevel level, string levelName, MapMakerData mapData)
        {
            _game.MarkLevelComplete(level);
            var worldMapGameObjects = _game.Map.MapData.Select(d => d.GameObject);
            var levelGameObjects = mapData.MapData.Select(d => d.GameObject);

            //activators
            var activatorObjectives = levelGameObjects.Where(g => 
            {
                if (!(g is ILevelObjective))
                    return false;

                if (g is IActivator)
                {
                    var act = g as IActivator;
                    return !act.IsActive;
                }

                return true;
            }).ToList();
            var relevantActivateables = worldMapGameObjects.OfType<IActivateable>()?.ToList();
            _worldMapObjectiveData.UpdateWorldMapObjectives(levelName, _player, activatorObjectives, relevantActivateables);

            //items
            var itemObjectives = levelGameObjects.Where(l =>   l is IItemLevelObjective
                && ((IItemLevelObjective)l).ObjectiveComplete).OfType<ISprite>(); 
            if (itemObjectives.Any())
            {
                _worldMapObjectiveData.UpdateWorldMapObjectives(levelName, _player,
                   itemObjectives.ToList());
            }
            //end goals
            var endGoalObjectives = levelGameObjects.OfType<ILevelObjective>();
            if (endGoalObjectives.Any())
            {
                var endGoalObjects = endGoalObjectives.Where(
                    e => e.EventType == Framework.Enums.ObjectiveEventType.LEVEL_EXIT
                     && e.ObjectiveComplete)
                    .OfType<ISprite>();
                if (endGoalObjects.Any())
                {
                    _worldMapObjectiveData.UpdateWorldMapObjectives(levelName, _player,
                        endGoalObjects.ToList());
                }
            }
        }

        private void HandleMapError(string message)
        {
            _player.ClearAllKeyPressStates();

            EventStore<SoundPlayEventArgs>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
              new SoundPlayEventArgs()
              {
                  SenderPosition = null,
                  Sound = GeneralGameConstants.Sounds.KEEN_ACCESS_DENIED
              });

            KeenReloadedErrorMessageWindow errorMessageWindow =
                new KeenReloadedErrorMessageWindow(message);

            errorMessageWindow.ShowDialog();

            if (!_gameUpdateTimer.Enabled)
                _gameUpdateTimer.Start();
        }

        private void Form1_KeenStateChanged(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {
            _playerState = e?.ObjectSprite as CommanderKeen;
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

            _worldMapObjectiveData.GameBeaten -= _worldMapObjectiveData_GameBeaten;
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

            if (_player == null)
            { 
                this.Close();
                return;
            }

            _player.Moved += _keen_KeenMoved;
            _player.PlayerError += _player_PlayerError;
            UpdateViewRectangle();
            pnlGameWindow.AutoScroll = true;
            pnlGameWindow.MouseWheel += PnlGameWindow_MouseWheel;
            this.MouseWheel += PnlGameWindow_MouseWheel;
        }

        private void _player_PlayerError(object sender, string e)
        {
            HandleMapError(e);
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
            _loadingWindow.KillMusicPlayer();
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
            if (_paused)
            {
                WorldMapModeMainMenu menu = new WorldMapModeMainMenu(_game.Map.MapPath, true);
                var result = menu.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    _paused = false;
                }
                else if (result == DialogResult.Abort)
                {
                    _menuDecision = menu.MenuDecision;
                    this.Close();
                }
            }
        }
    }
}
