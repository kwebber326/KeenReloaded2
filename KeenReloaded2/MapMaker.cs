using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.GameEntities.Interfaces;
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
        private List<GameObjectMapping> _mapMakerObjects = new List<GameObjectMapping>();
        private GameObjectMapping _selectedGameObjectMapping;
        public MapMaker()
        {
            InitializeComponent();
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
            mapObjectContainer1.ObjectClicked += MapObjectContainer1_ObjectClicked;
            mapMakerObjectPropertyListControl1.PlaceObjectClicked += MapMakerObjectPropertyListControl1_PlaceObjectClicked;
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
        private void ClearSelectedMapItem()
        {
            if (_selectedGameObjectMapping != null)
            {
                _selectedGameObjectMapping.BorderStyle = BorderStyle.None;
                _selectedGameObjectMapping = null;
            }
        }
        private void SetObjectContainer()
        {
            string categoryFolder = cmbCategory.SelectedItem?.ToString();
            string selectedEpisode = cmbEpisode.SelectedItem?.ToString() ?? string.Empty;
            string selectedBiome = cmbBiome.SelectedItem?.ToString();
            _episodeFileFolderDict.TryGetValue(selectedEpisode, out string episodeFolder);

            if (string.IsNullOrEmpty(categoryFolder)
             || string.IsNullOrEmpty(episodeFolder)
             || string.IsNullOrEmpty(selectedBiome))
                return;


            string path = ImageToObjectCreationFactory.GetImageDirectory(categoryFolder, episodeFolder, selectedBiome);
            if (categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_WEAPONS
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_PLAYER
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_GEMS)
            {
                cmbBiome.Visible = false;
                cmbEpisode.Visible = false;
            }
            else if (categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_TILES)
            {
                cmbEpisode.Visible = true;
                cmbBiome.Visible = true;
            }
            else
            {
                cmbEpisode.Visible = true;
                cmbBiome.Visible = false;
            }

            string[] files = Directory.GetFiles(path);

            mapObjectContainer1.DisplayImageFiles(files);
        }
        #endregion

        #region event handlers

        private void MapMakerObjectPropertyListControl1_PlaceObjectClicked(object sender, ControlEventArgs.MapMakerObjectEventArgs e)
        {
            if (e?.MapMakerObject?.ImageControl == null)
                return;

            object mapObj = new object();
            try
            {
                mapObj = e.MapMakerObject.Construct();
            }
            catch
            {
                MessageBox.Show("Error constructing the selected object. It will not be placed on the map.");
                return;
            }

            try
            {
                //parse and copy map maker objects
                ISprite placedItem = (ISprite)mapObj;
                var mapMakerObjectCopy = new MapMakerObject(
                    e.MapMakerObject.ObjectType,
                    e.MapMakerObject.ImageControl.ImageLocation,
                    e.MapMakerObject.IsManualPlacement,
                    e.MapMakerObject.ConstructorParameters);

                //remove existing item if it is there
                if (_selectedGameObjectMapping != null)
                {
                    pnlMapCanvas.Controls.Remove(_selectedGameObjectMapping);
                    _mapMakerObjects.Remove(_selectedGameObjectMapping);
                }
                //add new item
                //construct mapping
                GameObjectMapping gameObjectMapping = new GameObjectMapping()
                {
                    GameObject = placedItem,
                    MapMakerObject = mapMakerObjectCopy
                };
                gameObjectMapping.Location = placedItem.Location;
                gameObjectMapping.SizeMode = PictureBoxSizeMode.AutoSize;
                gameObjectMapping.Image = placedItem.Image;
                gameObjectMapping.Click += GameObjectMapping_Click;

                //add to collections
                _mapMakerObjects.Add(gameObjectMapping);
                pnlMapCanvas.Controls.Add(gameObjectMapping);

                //redraw grid
                var orderedByZindexObjects = _mapMakerObjects.OrderBy(o => o.GameObject.ZIndex);
                foreach (var obj in orderedByZindexObjects)
                {
                    obj.MapMakerObject.ImageControl.BringToFront();
                }

                //replace existing selection if we have one
                if (_selectedGameObjectMapping != null)
                {
                    _selectedGameObjectMapping = gameObjectMapping;
                }
            }
            catch
            {
                MessageBox.Show("Error placing object on the map. It is not in the standard sprite format.");
            }
        }

        private void GameObjectMapping_Click(object sender, EventArgs e)
        {
            ClearSelectedMapItem();
            _selectedGameObjectMapping = sender as GameObjectMapping;
            if (_selectedGameObjectMapping != null)
            {
                mapObjectContainer1.ClearSelection();
                _selectedGameObjectMapping.BorderStyle = BorderStyle.Fixed3D;
                mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject);
            }
        }
        private void MapMaker_Load(object sender, EventArgs e)
        {
            InitializeMapMaker();

        }
        private void MapObjectContainer1_ObjectClicked(object sender, ControlEventArgs.MapMakerObjectEventArgs e)
        {
            ClearSelectedMapItem();
            mapMakerObjectPropertyListControl1.SetProperties(e.MapMakerObject);
        }

        private void BtnDefaultDimensions_Click(object sender, EventArgs e)
        {
            SetDimensionsToDefaultValues();
        }

        private void CmbEpisode_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateBiomes();
            SetObjectContainer();
        }

        private void CmbBiome_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetObjectContainer();
        }

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetObjectContainer();
        }

        private void MapMaker_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    mapObjectContainer1.ClearSelection();
                    mapMakerObjectPropertyListControl1.SetProperties(null);
                    ClearSelectedMapItem();
                    break;
                case Keys.Delete:
                    if (_selectedGameObjectMapping != null)
                    {
                        _mapMakerObjects.Remove(_selectedGameObjectMapping);
                        pnlMapCanvas.Controls.Remove(_selectedGameObjectMapping);
                        mapObjectContainer1.ClearSelection();
                        mapMakerObjectPropertyListControl1.SetProperties(null);
                        ClearSelectedMapItem();
                    }
                    break;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
