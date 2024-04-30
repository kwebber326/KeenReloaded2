using KeenReloaded2.Constants;
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
    public partial class MapMaker : Form
    {
        public MapMaker()
        {
            InitializeComponent();
        }

        private void MapMaker_Load(object sender, EventArgs e)
        {
            InitializeMapMaker();
        }

        #region Initialization Methods

        private void InitializeMapMaker()
        {
            InitializeGameModeList();
            InitializeEpisodeList();
            InitializeDimensionValues();
            InitializeCategories();
        }

        private void InitializeGameModeList()
        {
            cmbGameMode.Items.Add(MainMenuConstants.OPTION_LABEL_NORMAL_MODE);
            cmbGameMode.Items.Add(MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE);
            cmbGameMode.Items.Add(MainMenuConstants.OPTION_LABEL_KOTH_MODE);
            cmbGameMode.Items.Add(MainMenuConstants.OPTION_LABEL_CTF_MODE);
            cmbGameMode.SelectedIndex = 0;
        }

        private void InitializeEpisodeList()
        {
            cmbEpisode.Items.Add(GeneralGameConstants.Episodes.EPISODE_4);
            cmbEpisode.Items.Add(GeneralGameConstants.Episodes.EPISODE_5);
            cmbEpisode.Items.Add(GeneralGameConstants.Episodes.EPISODE_6);
            cmbEpisode.SelectedIndex = 0;
        }

        private void InitializeDimensionValues()
        {
            for (int i = 500; i <= 5000; i += 100)
            {
                cmbWidth.Items.Add(i);
                cmbHeight.Items.Add(i);
            }
            SetDimensionsToDefaultValues();
        }

        private void SetDimensionsToDefaultValues()
        {
            cmbWidth.SelectedItem = MapMakerConstants.DEFAULT_MAP_WIDTH;
            cmbHeight.SelectedItem = MapMakerConstants.DEFAULT_MAP_HEIGHT;
        }

        private void InitializeCategories()
        {
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_BACKGROUNDS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_TILES);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_CONSTRUCTS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_PLAYER);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_POINTS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_LIVES);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_GEMS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_WEAPONS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_HAZARDS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_ENEMIES);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_FOREGROUNDS);
            cmbCategory.SelectedIndex = 0;
        }

        #endregion

        private void BtnDefaultDimensions_Click(object sender, EventArgs e)
        {
            SetDimensionsToDefaultValues();
        }
    }
}
