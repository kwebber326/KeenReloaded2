using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.UserControls;
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
    public partial class MainMenu : Form
    {
        private const int VERTICAL_OFFSET = 16;
        private const int HORIZONTAL_OFFSET = 8;
        private const int CHARACTER_POINT_VERTICAL_OFFSET = 36;
        private MainMenuOption _selectedOption;
        private int _lastSelectedIndex = 0;
        private bool _settingsInitialized;
        private string _selectedSong = string.Empty;
        private List<MainMenuOption> _options = new List<MainMenuOption>()
        {
            new MainMenuOption(MainMenuConstants.OPTION_LABEL_NORMAL_MODE, () => OpenMapPlayerInGameMode(MainMenuConstants.OPTION_LABEL_NORMAL_MODE) , true),
            new MainMenuOption(MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE, () => OpenMapPlayerInGameMode(MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE)),
            new MainMenuOption(MainMenuConstants.OPTION_LABEL_KOTH_MODE, () => OpenMapPlayerInGameMode(MainMenuConstants.OPTION_LABEL_KOTH_MODE)),
            new MainMenuOption(MainMenuConstants.OPTION_LABEL_CTF_MODE, () => OpenMapPlayerInGameMode(MainMenuConstants.OPTION_LABEL_CTF_MODE)),
            new MainMenuOption(MainMenuConstants.OPTION_LABEL_MAP_MAKER_MODE, () => OpenMapMaker())
        };
        public MainMenu()
        {
            InitializeComponent();
            EventStore<string>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_SELECTED_SONG_CHANGED,
                SelectedSong_Changed);

            musicSelectControl1.Visible = chkMusic.Checked;
        }

        #region helper methods
        private void InitializeAudioSettings()
        {
            AudioSettings settings = FileIOUtility.LoadAudioSettings();
            chkSounds.Checked = settings.Sounds;
            chkMusic.Checked = settings.Music;
            _selectedSong = settings.SelectedSong;
            _settingsInitialized = true;
            musicSelectControl1.Visible = settings.Music;
        }

        private void InitializeCharacterSelect()
        {
            pbCharacter.Image = characterSelectControl1.SelectedImage;
            characterSelectControl1.SelectedCharacterChanged += CharacterSelectControl1_SelectedCharacterChanged;
            pbCharacter.Location = new Point(pbCharacter.Location.X, _options[0].Location.Y + CHARACTER_POINT_VERTICAL_OFFSET);
        }

        private void ConstructMenu()
        {
            int x = HORIZONTAL_OFFSET, y = VERTICAL_OFFSET;
            for (int i = 0; i < _options.Count; i++)
            {
                var option = _options[i];
                pnlMenuOptions.Controls.Add(option);
                option.Location = new Point(x, y);
                option.MenuItemSelected += Option_MenuItemSelected;
                option.MenuItemDeselected += Option_MenuItemDeselected;
                y = VERTICAL_OFFSET + ((i + 1) * option.Height);
            }
        }

        private void ShiftSelectionUp()
        {
            if (_lastSelectedIndex != -1)
            {
                if (_lastSelectedIndex == 0)
                {
                    _lastSelectedIndex = _options.Count - 1;
                }
                else
                {
                    _lastSelectedIndex--;
                }
                _selectedOption = _options[_lastSelectedIndex];
                _selectedOption.SelectOption();
            }
        }

        private void ShiftSelectionDown()
        {
            if (_lastSelectedIndex != -1)
            {
                if (_lastSelectedIndex == _options.Count - 1)
                {
                    _lastSelectedIndex = 0;
                }
                else
                {
                    _lastSelectedIndex++;
                }
                _selectedOption = _options[_lastSelectedIndex];
                _selectedOption.SelectOption();
            }
        }

        private void ExecuteActionForSelectedMenuOption()
        {
            _selectedOption?.ExecuteAction();
        }

        private static void OpenMapPlayerInGameMode(string gameMode)
        {
            MapLoader mapPlayer = new MapLoader(gameMode);
            mapPlayer.ShowDialog();
        }

        private static void OpenMapMaker()
        {
            MapMaker mapMaker = new MapMaker();
            mapMaker.ShowDialog();
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
                case Keys.Enter:
                    KeyEventArgs args = new KeyEventArgs(keyData);
                    base.OnKeyDown(args);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SaveAudioSettings()
        {
            AudioSettings settings = new AudioSettings()
            {
                Sounds = chkSounds.Checked,
                Music = chkMusic.Checked,
                SelectedSong = _selectedSong
            };
            FileIOUtility.SaveAudioSettings(settings);
        }

        #endregion

        #region events

        private void SelectedSong_Changed(object sender, ControlEventArgs<string> e)
        {
            _selectedSong = e.Data;

            SaveAudioSettings();
        }

        private void ChkSounds_CheckedChanged(object sender, EventArgs e)
        {
            if (_settingsInitialized)
                SaveAudioSettings();
        }

        private void ChkMusic_CheckedChanged(object sender, EventArgs e)
        {
            if (_settingsInitialized)
            {
                musicSelectControl1.Visible = chkMusic.Checked;
                SaveAudioSettings();
            }
        }

        private void BtnRandomCharacter_Click(object sender, EventArgs e)
        {

        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            ConstructMenu();
            InitializeCharacterSelect();
            InitializeAudioSettings();
        }

        private void Option_MenuItemSelected(object sender, ControlEventArgs.MainMenuOptionEventArgs e)
        {
            var option = sender as MainMenuOption;
            if (option != null)
            {
                _selectedOption = option;
                _lastSelectedIndex = _options.IndexOf(option);
                pbCharacter.Location = new Point(pbCharacter.Location.X, _selectedOption.Location.Y + CHARACTER_POINT_VERTICAL_OFFSET);
                var deselectOptions = _options.Where(o => o != option).ToList();
                foreach (var menuItem in deselectOptions)
                {
                    menuItem.DeselectOption();
                }
            }
        }

        private void Option_MenuItemDeselected(object sender, ControlEventArgs.MainMenuOptionEventArgs e)
        {
            if (!_options.Any(o => o.IsSelected))
            {
                _selectedOption = null;
            }
        }

        private void CharacterSelectControl1_SelectedCharacterChanged(object sender, ControlEventArgs.CharacterSelectControlEventArgs e)
        {
            pbCharacter.Image = characterSelectControl1.SelectedImage;
            SaveCharacterSelection();
        }

        private void MainMenu_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    this.ExecuteActionForSelectedMenuOption();
                    break;
                case Keys.Up:
                    this.ShiftSelectionUp();
                    break;
                case Keys.Down:
                    this.ShiftSelectionDown();
                    break;
            }
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCharacterSelection();
        }

        private void SaveCharacterSelection()
        {
            string characterName = characterSelectControl1.SelectedCharacterName;
            if (!string.IsNullOrWhiteSpace(characterName))
                FileIOUtility.SaveCharacterSelection(characterName);
        }



        #endregion

    }
}
