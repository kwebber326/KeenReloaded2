using KeenReloaded2.Constants;
using KeenReloaded2.DialogWindows;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2
{
    public partial class WorldMapModeMainMenu : Form
    {
        private readonly string _worldMapFile;
        private const int SELECTOR_X_POS = 350;
        private const int TOGGLE_IMAGE_X_POS = 600;
        private bool _inGame = false;

        private const string NEW_GAME = "New Game";
        private const string LOAD_GAME = "Load Game";
        private const string SAVE_GAME = "Save Game";
        private const string CONFIGURE = "Configure";
        private const string QUIT = "Quit";

        private const string SOUND = "Sound";
        private const string MUSIC = "Music";
        private const string BACK = "Back";

        private int _selectedMenuIndex = 0;

        public static event EventHandler GameQuit;
        public static event EventHandler GameStart;
        public static event EventHandler GameConfigure;
        public static event EventHandler GameGoBack;
        public static event EventHandler GameSoundToggle;
        public static event EventHandler GameMusicToggle;

        private bool _suppressSelection;
        private WorldMapMenuOptionDecision? _menuDecision;

        private WorldMapMenuOption[] _currentMenu;
        private readonly string _songOverride;
        private AudioSettings _settings;
        private WorldMapMenuOption[] _mainMenuOptions
            = new WorldMapMenuOption[]
            {
                 new WorldMapMenuOption() { Name = NEW_GAME, YPos = 344 },
                 new WorldMapMenuOption() { Name = LOAD_GAME, YPos = 406 },
                 new WorldMapMenuOption() { Name = SAVE_GAME, YPos = 468 },
                 new WorldMapMenuOption() { Name = CONFIGURE, YPos = 530 },
                 new WorldMapMenuOption() { Name = QUIT, YPos = 592 },
            };

        private WorldMapMenuOption[] _configureMenuOptions = new WorldMapMenuOption[]
        {
            new WorldMapMenuOption() { Name = MUSIC, YPos = 376 },
            new WorldMapMenuOption() { Name = SOUND, YPos = 448 },
            new WorldMapMenuOption() { Name = BACK, YPos = 518 }
        };

        private Dictionary<string, Action> _menuActions
            = new Dictionary<string, Action>()
        {
            { NEW_GAME, () => StartNewGame() },
            { LOAD_GAME, () => LoadGame() },
            { SAVE_GAME, () => SaveGame() },
            { CONFIGURE, () => Configure() },
            { QUIT, () => OnQuit() },
            { SOUND, () => OnSoundToggle() },
            { MUSIC, () => OnMusicToggle() },
            { BACK, () => OnGoBack() }
        };

        public WorldMapModeMainMenu()
        {
            InitializeComponent();
        }

        public WorldMapModeMainMenu(string worldMapFile, bool inGame = false, string songOverride = null)
        {
            _inGame = inGame;
            _worldMapFile = worldMapFile;
            _currentMenu = _mainMenuOptions;
            _songOverride = songOverride;
            InitializeComponent();
            InitializeAudioSettings();
            AttachMenuEvents();
        }

        private void InitializeAudioSettings()
        {
            _settings = FileIOUtility.LoadAudioSettings();
            if (_songOverride != null)
            {
                _settings.SelectedSong = _songOverride;
            }
            _configureMenuOptions[1].PictureBox = new PictureBox()
            {
                Image = _settings.Sounds
               ? Properties.Resources.keen_menu_option_on
               : Properties.Resources.keen_menu_option_off,
                Location = new Point(TOGGLE_IMAGE_X_POS, _configureMenuOptions[1].YPos)
            };
            _configureMenuOptions[0].PictureBox = new PictureBox()
            {
                Image = _settings.Music
               ? Properties.Resources.keen_menu_option_on
               : Properties.Resources.keen_menu_option_off,
                Location = new Point(TOGGLE_IMAGE_X_POS, _configureMenuOptions[0].YPos)
            };
            foreach (var option in _configureMenuOptions)
            {
                if (option.PictureBox != null)
                {
                    option.PictureBox.BackColor = Color.Transparent;
                    option.PictureBox.Parent = pbScreen;
                    option.PictureBox.Visible = false;
                    pbScreen.Controls.Add(option.PictureBox);
                }
            }
        }

        public WorldMapMenuOptionDecision? MenuDecision => _menuDecision;

        private static void OnGoBack()
        {
            GameGoBack?.Invoke(null, EventArgs.Empty);
        }

        private static void OnSoundToggle()
        {
           GameSoundToggle?.Invoke(null, EventArgs.Empty);
        }

        private static void OnMusicToggle()
        {
            GameMusicToggle?.Invoke(null, EventArgs.Empty);
        }

        private static void OnQuit()
        {
            GameQuit?.Invoke(null, EventArgs.Empty);
        }

        private static void Configure()
        {
            GameConfigure?.Invoke(null, EventArgs.Empty);
        }

        private static void SaveGame()
        {
            MessageBox.Show("Implementation in progress");
        }

        private static void LoadGame()
        {
            MessageBox.Show("Implementation in progress");
        }

        private static void StartNewGame()
        {
            GameStart?.Invoke(null, EventArgs.Empty);
        }

        private void WorldMapModeMainMenu_GameStart(object sender, EventArgs e)
        {
            if (_inGame)
            {
                KeenReloadedYesNoDialogWindow keenReloadedYesNoDialog =
                    new KeenReloadedYesNoDialogWindow("You're in a game. \nStart a new one?", false);
                var result = keenReloadedYesNoDialog.ShowDialog();
                if (result == DialogResult.No)
                {
                    _suppressSelection = true;
                    return;
                }
            }

            _menuDecision = WorldMapMenuOptionDecision.START_NEW;
            _suppressSelection = false;
            _inGame = true;
          
            this.DialogResult = DialogResult.Abort;
            this.Close();
            
        }

        private void WorldMapModeMainMenu_GameQuit(object sender, EventArgs e)
        {
            ExecuteQuitGameProtocol();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_SYSMENU = 0x80000; // Flag for system menu
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU; // Remove system menu
                return cp;
            }
        }

        private void WorldMapModeMainMenu_Load(object sender, EventArgs e)
        {
            MoveSelectorToSelectedOption();
        }

        private void WorldMapModeMainMenu_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void WorldMapModeMainMenu_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (!_inGame)
                    ExecuteQuitGameProtocol();
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                MoveMenuOptions(e.KeyCode);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (_suppressSelection)
                {
                    _suppressSelection = false;
                    return;
                }

                var selectedOption = _currentMenu[_selectedMenuIndex];
                if (_menuActions.TryGetValue(selectedOption.Name, out Action action))
                {
                    action();
                }
            }
        }

        private void DetachMenuEvents()
        {
            GameQuit -= WorldMapModeMainMenu_GameQuit;
            GameStart -= WorldMapModeMainMenu_GameStart;
            GameConfigure -= WorldMapModeMainMenu_GameConfigure;
            GameGoBack -= WorldMapModeMainMenu_GameGoBack;
            GameSoundToggle -= WorldMapModeMainMenu_GameSoundToggle;
            GameMusicToggle -= WorldMapModeMainMenu_GameMusicToggle;
        }

        private void AttachMenuEvents()
        {
            GameQuit += WorldMapModeMainMenu_GameQuit;
            GameStart += WorldMapModeMainMenu_GameStart;
            GameConfigure += WorldMapModeMainMenu_GameConfigure;
            GameGoBack += WorldMapModeMainMenu_GameGoBack;
            GameSoundToggle += WorldMapModeMainMenu_GameSoundToggle;
            GameMusicToggle += WorldMapModeMainMenu_GameMusicToggle;
        }

        private void WorldMapModeMainMenu_GameMusicToggle(object sender, EventArgs e)
        {
            _settings.Music = !_settings.Music;
            FileIOUtility.SaveAudioSettings(_settings);
            _configureMenuOptions[0].PictureBox.Image = _settings.Music
                 ? Properties.Resources.keen_menu_option_on
                 : Properties.Resources.keen_menu_option_off;
            EventStore<AudioSettings>.Publish(
                MapMakerConstants.EventStoreEventNames.EVENT_AUDIO_SETTINGS_CHANGED,
                _settings);
        }

        private void WorldMapModeMainMenu_GameSoundToggle(object sender, EventArgs e)
        {
            _settings.Sounds = !_settings.Sounds;
            FileIOUtility.SaveAudioSettings(_settings);
            _configureMenuOptions[1].PictureBox.Image = _settings.Sounds
                ? Properties.Resources.keen_menu_option_on
                : Properties.Resources.keen_menu_option_off;
            EventStore<AudioSettings>.Publish(
                MapMakerConstants.EventStoreEventNames.EVENT_AUDIO_SETTINGS_CHANGED,
                _settings);
        }

        private void WorldMapModeMainMenu_GameGoBack(object sender, EventArgs e)
        {
            if (_currentMenu == _configureMenuOptions)
            {
                pbScreen.Image = Properties.Resources.keen_main_menu;
                _currentMenu = _mainMenuOptions;
                _selectedMenuIndex = 0;
                pbSelector.Location = new Point(
                    pbSelector.Location.X, _currentMenu[_selectedMenuIndex].YPos);
                _configureMenuOptions[0].PictureBox.Visible = false;
                _configureMenuOptions[1].PictureBox.Visible = false;
            }
        }

        private void WorldMapModeMainMenu_GameConfigure(object sender, EventArgs e)
        {
            pbScreen.Image = Properties.Resources.keen_configure_menu;
            _currentMenu = _configureMenuOptions;
            _selectedMenuIndex = 0;
            pbSelector.Location = new Point(
                pbSelector.Location.X, _currentMenu[_selectedMenuIndex].YPos);
            _configureMenuOptions[0].PictureBox.Visible = true;
            _configureMenuOptions[1].PictureBox.Visible = true;
        }

        private void ExecuteQuitGameProtocol()
        {
            if (!_inGame)
            {
                this.DialogResult = DialogResult.Abort;
                _menuDecision = WorldMapMenuOptionDecision.QUIT;
                this.Close();
                return;
            }

            KeenReloadedYesNoDialogWindow keenReloadedYesNo =
                new KeenReloadedYesNoDialogWindow("Unsaved progress will be lost.\nQuit?", false);
            var dialogResult = keenReloadedYesNo.ShowDialog();
            if (dialogResult == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Abort;
                _menuDecision = WorldMapMenuOptionDecision.QUIT;
                this.Close();
            }
            else
            {
                _suppressSelection = true;
            }
        }

        private void MoveMenuOptions(Keys keyCode)
        {
            if (keyCode == Keys.Up)
            {
                CycleMenuUp();
            }
            else
            {
                CycleMenuDown();
            }
        }

        private void CycleMenuDown()
        {
            if (++_selectedMenuIndex >= _currentMenu.Length)
            {
                _selectedMenuIndex = 0;
            }

            MoveSelectorToSelectedOption();
        }

        private void CycleMenuUp()
        {
            if (--_selectedMenuIndex < 0)
            {
                _selectedMenuIndex = _currentMenu.Length - 1;
            }

            MoveSelectorToSelectedOption();
        }

        private void MoveSelectorToSelectedOption()
        {
            WorldMapMenuOption selectedOption = _currentMenu[_selectedMenuIndex];

            pbSelector.Location = new Point(SELECTOR_X_POS, selectedOption.YPos);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void WorldMapModeMainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            DetachMenuEvents();
        }
    }

    public enum WorldMapMenuOptionDecision
    {
        START_NEW,
        QUIT,
        LOAD_EXISTING
    }

    public struct WorldMapMenuOption
    {
        public string Name { get; set; }

        public int YPos { get; set; }

        public PictureBox PictureBox { get; set; }
    }
}
