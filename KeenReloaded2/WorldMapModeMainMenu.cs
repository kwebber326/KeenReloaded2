using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.DialogWindows;
using KeenReloaded2.Entities;
using KeenReloaded2.Utilities;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace KeenReloaded2
{
    public partial class WorldMapModeMainMenu : Form
    {
        private readonly string _worldMapFile;
        private readonly string _levelFile;
        private const int TOGGLE_IMAGE_X_POS = 600;
        private const int IO_MENU_SELECTION_COUNT = 8;
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
        public static event EventHandler GameSaveMenu;
        public static event EventHandler GameLoadMenu;

        private bool _suppressSelection;
        private WorldMapMenuOptionDecision? _menuDecision;

        private WorldMapMenuOption[] _currentMenu;
        private readonly string _songOverride;
        private WorldMapSaveState _worldState;
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

        private SavedGameMenuOption[] _loadMenuOptions = new SavedGameMenuOption[IO_MENU_SELECTION_COUNT];
        private SavedGameMenuOption[] _saveMenuOptions = new SavedGameMenuOption[IO_MENU_SELECTION_COUNT];

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
        private bool _isItemFocused;

        public WorldMapModeMainMenu()
        {
            InitializeComponent();
        }

        public WorldMapModeMainMenu(string worldMapFile, bool inGame = false, string songOverride = null, WorldMapSaveState saveState = null)
        {
            _inGame = inGame;
            _worldMapFile = worldMapFile;
            _currentMenu = _mainMenuOptions;
            _songOverride = songOverride;
            _worldState = saveState;
            InitializeComponent();
            InitializeAudioSettings();
            AttachMenuEvents();
            InitializeLoadSaveSelections();
        }

        public WorldMapModeMainMenu(string worldMapFile, string levelFile, bool inGame = false, string songOverride = null, WorldMapSaveState saveState = null)
        {
            _inGame = inGame;
            _worldMapFile = worldMapFile;
            _levelFile = levelFile;
            _currentMenu = _mainMenuOptions;
            _songOverride = songOverride;
            _worldState = saveState;
            InitializeComponent();
            InitializeAudioSettings();
            AttachMenuEvents();
            InitializeLoadSaveSelections();
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

        private void InitializeLoadSaveSelections()
        {
            int x = WorldMapMenuOption.SELECTOR_X_POS - 96,
                y = SavedGameMenuOption.VERTICAL_OFFSET;
            const int VERTICAL_MARGIN = 4;
            Image img = Properties.Resources.menu_named_selection;
            int width = img.Width,
                height = img.Height;

            for (int i = 0; i < _loadMenuOptions.Length; i++)
            {
                SavedGameMenuOption option = new SavedGameMenuOption(x, y);
                _loadMenuOptions[i] = option;
                _saveMenuOptions[i] = option;
                option.Name = $"savedGame_{i}";
                _menuActions.Add(option.Name, () => ExecuteActionForSavedGame(option));
                y = (height + VERTICAL_MARGIN) * (i + 1)
                    + SavedGameMenuOption.VERTICAL_OFFSET;
            }
        }

        public WorldMapMenuOptionDecision? MenuDecision => _menuDecision;

        #region helper methods

        private void ExecuteActionForSavedGame(SavedGameMenuOption option)
        {
            if (_currentMenu == _saveMenuOptions)
            {
                if (!option.IsSelected)
                    option.Select();
                else
                    option.Deselect();

                _isItemFocused = option.IsSelected;
            }
            else
            {

            }
        }

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
            GameSaveMenu?.Invoke(null, EventArgs.Empty);
        }

        private static void LoadGame()
        {
            GameLoadMenu?.Invoke(null, EventArgs.Empty);
        }

        private static void StartNewGame()
        {
            GameStart?.Invoke(null, EventArgs.Empty);
        }

        private void DetachMenuEvents()
        {
            GameQuit -= WorldMapModeMainMenu_GameQuit;
            GameStart -= WorldMapModeMainMenu_GameStart;
            GameConfigure -= WorldMapModeMainMenu_GameConfigure;
            GameGoBack -= WorldMapModeMainMenu_GameGoBack;
            GameSoundToggle -= WorldMapModeMainMenu_GameSoundToggle;
            GameMusicToggle -= WorldMapModeMainMenu_GameMusicToggle;
            GameLoadMenu -= WorldMapModeMainMenu_GameLoadMenu;
            GameSaveMenu -= WorldMapModeMainMenu_GameSaveMenu;
        }

        private void AttachMenuEvents()
        {
            GameQuit += WorldMapModeMainMenu_GameQuit;
            GameStart += WorldMapModeMainMenu_GameStart;
            GameConfigure += WorldMapModeMainMenu_GameConfigure;
            GameGoBack += WorldMapModeMainMenu_GameGoBack;
            GameSoundToggle += WorldMapModeMainMenu_GameSoundToggle;
            GameMusicToggle += WorldMapModeMainMenu_GameMusicToggle;
            GameLoadMenu += WorldMapModeMainMenu_GameLoadMenu;
            GameSaveMenu += WorldMapModeMainMenu_GameSaveMenu;
        }

        private void ReturnToMainMenu()
        {
            ClearLoadSaveOptions();
            pbScreen.Image = Properties.Resources.keen_main_menu;
            _currentMenu = _mainMenuOptions;
            _selectedMenuIndex = 0;
            pbSelector.Location = new Point(
                pbSelector.Location.X, _currentMenu[_selectedMenuIndex].YPos);
            _configureMenuOptions[0].PictureBox.Visible = false;
            _configureMenuOptions[1].PictureBox.Visible = false;
            MoveSelectorToSelectedOption();
            _isItemFocused = false;
        }

        private void ClearLoadSaveOptions()
        {
            _isItemFocused = false;
            foreach (var option in _loadMenuOptions)
            {
                this.Controls.Remove(option.PictureBox);
            }
            foreach (var option in _saveMenuOptions)
            {
                this.Controls.Remove(option.PictureBox);
                if (option.IsSelected)
                {
                    option.Deselect();
                }
            }
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
            if (_isItemFocused)
                return;

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

            pbSelector.Location = new Point(selectedOption.XPos, selectedOption.YPos);
            pbSelector.BringToFront();
        }

        #endregion

        #region Event Handlers

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
            var selectedOption = _currentMenu[_selectedMenuIndex];
            if (e.KeyCode == Keys.Escape)
            {
                if (selectedOption is SavedGameMenuOption)
                {
                    var option = (SavedGameMenuOption)selectedOption;
                    if (option.IsSelected)
                    {
                        _isItemFocused = false;
                        option.Deselect(true);
                        return;
                    }
                }

                if (_currentMenu == _loadMenuOptions
                    || _currentMenu == _saveMenuOptions)
                {
                    ReturnToMainMenu();
                }
                else if (!_inGame)
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

               
                if (_menuActions.TryGetValue(selectedOption.Name, out Action action))
                {
                    action();
                }
            }
            else
            {
                if (!(selectedOption is SavedGameMenuOption))
                    return;

                var savedGameSelection = (SavedGameMenuOption)selectedOption;
                if (!savedGameSelection.IsSelected)
                    return;

                if (e.KeyCode == Keys.Back)
                {
                   savedGameSelection.RemoveLastChar();
                }
                else
                {
                    char c = e.KeyCode.ToString()[0];
                    savedGameSelection.AddCharacter(c);
                }
            }
        }

        private void WorldMapModeMainMenu_GameSaveMenu(object sender, EventArgs e)
        {
            if (!_inGame)
                return;

            pbScreen.Image = Properties.Resources.keen_save_menu;
            _currentMenu = _saveMenuOptions;

            foreach (var option in _saveMenuOptions)
            {
                this.Controls.Add(option.PictureBox);
                option.PictureBox.BringToFront();
            }
            _selectedMenuIndex = 0;
            MoveSelectorToSelectedOption();
        }

        private void WorldMapModeMainMenu_GameLoadMenu(object sender, EventArgs e)
        {
            pbScreen.Image = Properties.Resources.keen_load_menu;
            _currentMenu = _loadMenuOptions;

            foreach (var option in _loadMenuOptions)
            {
                this.Controls.Add(option.PictureBox);
                option.PictureBox.BringToFront();
            }
            _selectedMenuIndex = 0;
            MoveSelectorToSelectedOption();
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
                ReturnToMainMenu();
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

        private void WorldMapModeMainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            DetachMenuEvents();
        }

        #endregion
    }

    public enum WorldMapMenuOptionDecision
    {
        START_NEW,
        QUIT,
        LOAD_EXISTING
    }

    public class WorldMapMenuOption
    {
        public static readonly int SELECTOR_X_POS = 350;
        public string Name { get; set; }

        public int YPos { get; set; }

        public int XPos { get; set; } = SELECTOR_X_POS;

        public PictureBox PictureBox { get; set; }
    }

    public class SavedGameMenuOption : WorldMapMenuOption
    {
        public static readonly int VERTICAL_OFFSET = 320;
        private const int CHARACTER_HORIZONTAL_OFFSET = 10;
        private const int CHARACTER_VERTICAL_OFFSET = 6;
        private bool _borderToggle;
        private bool _selected;
        private Timer _selectionImageToggleTimer;
        private string _saveNameText = string.Empty;
        private Dictionary<char, Rectangle> _characterLocationMapping = new Dictionary<char, Rectangle>()
        {
            { 'A', new Rectangle(56, 124, 31, 39) }
        };

        public SavedGameMenuOption(int xPos, int yPos)
        {
            this.XPos = xPos;
            this.YPos = yPos;
            _selectionImageToggleTimer = new Timer();
            _selectionImageToggleTimer.Interval = 400;
            _selectionImageToggleTimer.Tick += _selectionImageToggleTimer_Tick;
            this.PictureBox = new PictureBox();
            this.PictureBox.Location = new Point(xPos, yPos);
            this.PictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            this.PictureBox.Image = Properties.Resources.menu_named_selection;
            this.PictureBox.BackColor = ColorTranslator.FromHtml("#353535");
            this.YPos += this.PictureBox.Height / 2 - 7;
            this.XPos -= 17;
        }

        public bool IsSelected
        {
            get
            {
                return _selected;
            }
        }

        private void _selectionImageToggleTimer_Tick(object sender, EventArgs e)
        {
            DrawFrame();

            _borderToggle = !_borderToggle;
        }

        private void DrawFrame()
        {
            this.PictureBox.Image = _borderToggle ?
                Properties.Resources.menu_named_selection_border :
                Properties.Resources.menu_named_selection;

            List<Image> characterImages = new List<Image>();
            List<Point> points = new List<Point>();
            int x = CHARACTER_HORIZONTAL_OFFSET, bottom = this.PictureBox.Height - CHARACTER_VERTICAL_OFFSET;
            const int TEXT_MARGIN = 2;
            foreach (char c in _saveNameText)
            {
                if (_characterLocationMapping.TryGetValue(c, out Rectangle area))
                {
                    var image = BitMapTool.CropImage(Properties.Resources.keen_main_menu_font_sheet,
                        area);
                    characterImages.Add(image);
                    int y = bottom - image.Height;
                    points.Add(new Point(x, y));
                    x += area.Width + TEXT_MARGIN; 
                }
            }

            this.PictureBox.Image = BitMapTool.DrawImagesOnCanvas(this.PictureBox.Size,
                this.PictureBox.Image, characterImages.ToArray(), points.ToArray());
        }

        private bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(_saveNameText);
        }

        public void AddCharacter(char c)
        {
            _saveNameText += c;
            DrawFrame();
        }

        public void RemoveLastChar()
        {
            if (_saveNameText.Length == 0)
                return;

           _saveNameText = _saveNameText.Remove(_saveNameText.Length - 1);
            DrawFrame();
        }

        public void Select()
        {
            _selected = true;
            _selectionImageToggleTimer.Start();
        }

        public void Deselect(bool force = false)
        {
            if (!IsValid() && !force)
                return;

            _selected = false;
            _selectionImageToggleTimer.Stop();
            _borderToggle = false;
            DrawFrame();
        }
    }
}
