using KeenReloaded2.Constants;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace KeenReloaded2
{
    public partial class MapMaker : Form
    {
        private Dictionary<string, string> _episodeFileFolderDict = new Dictionary<string, string>();
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
            InitializeBiomeComboBox();
            SetObjectContainer();
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
            _episodeFileFolderDict.Add(GeneralGameConstants.Episodes.EPISODE_4, "Keen4");
            _episodeFileFolderDict.Add(GeneralGameConstants.Episodes.EPISODE_5, "Keen5");
            _episodeFileFolderDict.Add(GeneralGameConstants.Episodes.EPISODE_6, "Keen6");
            cmbEpisode.SelectedIndexChanged += CmbEpisode_SelectedIndexChanged;
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

        private void InitializeBiomeComboBox()
        {
            cmbBiome.SelectedIndexChanged += CmbBiome_SelectedIndexChanged;
        }

        private void PopulateBiomes()
        {
            cmbBiome.Items.Clear();
            if (cmbEpisode.SelectedItem != null &&
                Biomes.BiomeRepository.TryGetValue(cmbEpisode.SelectedItem?.ToString(), out List<string> biomes))
            {
                foreach (var biome in biomes)
                {
                    cmbBiome.Items.Add(biome);
                }
            }
            cmbBiome.SelectedIndex = 0;
        }

        #endregion

        #region helper methods 
        private void SetObjectContainer()
        {
            string mapMakerFolder = MapMakerConstants.MAP_MAKER_FOLDER;
            string categoryFolder = cmbCategory.SelectedItem?.ToString();
            string selectedEpisode = cmbEpisode.SelectedItem?.ToString() ?? string.Empty;
            _episodeFileFolderDict.TryGetValue(selectedEpisode, out string episodeFolder);

            string path = Path.Combine(System.Environment.CurrentDirectory, mapMakerFolder, categoryFolder, episodeFolder);

            string[] files = Directory.GetFiles(path);

            mapObjectContainer1.DisplayImageFiles(files);
        }
        #endregion

        #region event handlers

        private void BtnDefaultDimensions_Click(object sender, EventArgs e)
        {
            SetDimensionsToDefaultValues();
        }

        private void CmbEpisode_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateBiomes();
        }

        private void CmbBiome_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        #endregion
    }
}
