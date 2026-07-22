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
        private bool _inGame = false;

        private const string NEW_GAME = "New Game";
        private const string LOAD_GAME = "Load Game";
        private const string SAVE_GAME = "Save Game";
        private const string CONFIGURE = "Configure";
        private const string QUIT = "Quit";

        private int _selectedMenuIndex = 0;

        public static event EventHandler GameQuit;
        public static event EventHandler GameStart;

        private WorldMapMenuOption[] _menuOptions
            = new WorldMapMenuOption[]
            {
                 new WorldMapMenuOption() { Name = NEW_GAME, YPos = 344 },
                 new WorldMapMenuOption() { Name = LOAD_GAME, YPos = 406 },
                 new WorldMapMenuOption() { Name = SAVE_GAME, YPos = 468 },
                 new WorldMapMenuOption() { Name = CONFIGURE, YPos = 530 },
                 new WorldMapMenuOption() { Name = QUIT, YPos = 592 },
            };

        private Dictionary<string, Action> _menuActions
            = new Dictionary<string, Action>()
        {
            { NEW_GAME, () => StartNewGame() },
            { LOAD_GAME, () => LoadGame() },
            { SAVE_GAME, () => SaveGame() },
            { CONFIGURE, () => Configure() },
            { QUIT, () => OnQuit() },
        };
        private bool _suppressSelection;

        private static void OnQuit()
        {
            GameQuit?.Invoke(null, EventArgs.Empty);
        }

        private static void Configure()
        {
            MessageBox.Show("Implementation in progress");
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

        public WorldMapModeMainMenu()
        {
            InitializeComponent();
        }

        public WorldMapModeMainMenu(string worldMapFile, bool inGame = false)
        {
            _inGame = inGame;
            _worldMapFile = worldMapFile;
            InitializeComponent();
            AttachMenuEvents();
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

            _suppressSelection = false;
            _inGame = true;
            var map = MapUtility.LoadMapData(_worldMapFile);
            WorldMapPlayerForm form = new WorldMapPlayerForm(map, false);
            this.DialogResult = DialogResult.Abort;
            this.Close();
            form.ShowDialog();
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

                var selectedOption = _menuOptions[_selectedMenuIndex];
                if (_menuActions.TryGetValue(selectedOption.Name, out Action action))
                {
                    action();
                }
            }
        }

        private void AttachMenuEvents()
        {
            GameQuit += WorldMapModeMainMenu_GameQuit;
            GameStart += WorldMapModeMainMenu_GameStart;
        }


        private void DetachMenuEvents()
        {
            GameQuit -= WorldMapModeMainMenu_GameQuit;
            GameStart -= WorldMapModeMainMenu_GameStart;
        }

        private void ExecuteQuitGameProtocol()
        {
            if (!_inGame)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            KeenReloadedYesNoDialogWindow keenReloadedYesNo =
                new KeenReloadedYesNoDialogWindow("Unsaved progress will be lost.\nQuit?", false);
            var dialogResult = keenReloadedYesNo.ShowDialog();
            if (dialogResult == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Abort;
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
            if (++_selectedMenuIndex >= _menuOptions.Length)
            {
                _selectedMenuIndex = 0;
            }

            MoveSelectorToSelectedOption();
        }

        private void CycleMenuUp()
        {
            if (--_selectedMenuIndex < 0)
            {
                _selectedMenuIndex = _menuOptions.Length - 1;
            }

            MoveSelectorToSelectedOption();
        }

        private void MoveSelectorToSelectedOption()
        {
            WorldMapMenuOption selectedOption = _menuOptions[_selectedMenuIndex];

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

    public struct WorldMapMenuOption
    {
        public string Name { get; set; }

        public int YPos { get; set; }
    }
}
