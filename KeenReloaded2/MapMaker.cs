using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private GameObjectMapping _cursorItem;
        private Timer _cursorUpdateTimer = new Timer();
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
            _cursorUpdateTimer.Interval = 10;
            _cursorUpdateTimer.Tick += _cursorUpdateTimer_Tick;
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

        private Size GetMapSize()
        {
            int width = (int)cmbWidth.SelectedItem;
            int height = (int)cmbHeight.SelectedItem;

            Size mapSize = new Size(width, height);

            return mapSize;
        }

        private GameObjectMapping GenerateMappingObjectFromMapMakerData(MapMakerObject mapMakerObject)
        {
            MapMakerObject obj = new MapMakerObject(
                  mapMakerObject.ObjectType,
                  mapMakerObject.ImageControl.ImageLocation,
                  mapMakerObject.IsManualPlacement,
                  mapMakerObject.CloneParameterList());
            ISprite placeableObject = (ISprite)obj.Construct();
            GameObjectMapping mapping = new GameObjectMapping()
            {
                MapMakerObject = obj,
                GameObject = placeableObject
            };
            mapping.SizeMode = PictureBoxSizeMode.AutoSize;
            mapping.Location = placeableObject.Location;
            mapping.Image = placeableObject.Image;

            return mapping;
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
                    e.MapMakerObject.CloneParameterList());

                var objectArea = new Rectangle(placedItem.Location, placedItem.Image.Size);
                var mapSize = this.GetMapSize();
                var mapArea = new Rectangle(new Point(0, 0), mapSize);
                if (!MapUtility.Validation.ValidateObjectPlacement(objectArea, mapArea))
                {
                    MessageBox.Show($"This object would be placed outside the bounds of the map. Please ensure the location and size of the object does not place the object outside the bounds of the map", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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
                    obj.BringToFront();
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
            if (e.MapMakerObject == null)
            {
                ClearSelectedMapItem();
                mapMakerObjectPropertyListControl1.SetProperties(null);
                return;
            }

            if (e.MapMakerObject.IsManualPlacement)
            {
                ClearSelectedMapItem();
                mapMakerObjectPropertyListControl1.SetProperties(e.MapMakerObject);
            }
            else
            {
                GameObjectMapping mapping = GenerateMappingObjectFromMapMakerData(e.MapMakerObject);
                _cursorItem = mapping;
                this.Controls.Add(_cursorItem);
                _cursorUpdateTimer.Start();
            }
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
                    if (_cursorItem != null)
                    {
                        this.Controls.Remove(_cursorItem);
                        _cursorItem = null;
                    }
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
            if (string.IsNullOrWhiteSpace(txtMapName.Text))
            {
                MessageBox.Show("Please enter a valid map name");
                return;
            }

            Size mapSize = this.GetMapSize();

            if (MapUtility.SaveMap(txtMapName.Text, cmbGameMode.SelectedItem?.ToString(), mapSize, _mapMakerObjects))
            {
                MessageBox.Show($"Map '{txtMapName.Text}' was saved successfully!");
            }
            else
            {
                MessageBox.Show($"Map '{txtMapName.Text}' did not save successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            dialogMapLoader.InitialDirectory = MapUtility.GetSavedMapsPath(cmbGameMode.SelectedItem?.ToString());
            dialogMapLoader.Filter = "*.txt|";
            dialogMapLoader.ShowDialog();
        }

        private void DialogMapLoader_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                //load the map data
                string path = dialogMapLoader.FileName;
                var mapMakerData = MapUtility.LoadMapData(path);
                _mapMakerObjects = mapMakerData.MapData;

                //clear events for existing items
                var existingItems = pnlMapCanvas.Controls.OfType<GameObjectMapping>();
                if (existingItems.Any())
                {
                    foreach (var item in existingItems)
                    {
                        item.Click -= GameObjectMapping_Click;
                    }
                }
                //clear out the canvas
                pnlMapCanvas.Controls.Clear();

                //register new data on grid and load to canvas
                foreach (var mapObject in _mapMakerObjects)
                {
                    mapObject.Click += GameObjectMapping_Click;
                    pnlMapCanvas.Controls.Add(mapObject);
                }

                //set the map name text box value to the name of the newly loaded map
                txtMapName.Text = mapMakerData.MapName;

                //set the dimensions based on the saved info
                int wIndex =  cmbWidth.Items.IndexOf(mapMakerData.MapSize.Width);
                cmbWidth.SelectedIndex = wIndex;
                int hIndex = cmbHeight.Items.IndexOf(mapMakerData.MapSize.Height);
                cmbHeight.SelectedIndex = hIndex; 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show($"Map did not load successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtMapName_TextChanged(object sender, EventArgs e)
        {
            txtMapName.Text = InputValidation.SanitizeFileNameInput(txtMapName.Text);
        }

        private void _cursorUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_cursorItem != null)
            {
                _cursorItem.Location = new Point(Cursor.Position.X, Cursor.Position.Y);
            }
            else
            {
                _cursorUpdateTimer.Stop();
            }
        }

        private void PnlMapCanvas_Click(object sender, EventArgs e)
        {
            if (_cursorItem != null)
            {
                int xOffset = pnlMapCanvas.Location.X;
                int yOffset = pnlMapCanvas.Location.Y;

                _cursorItem.Location.Offset(xOffset, yOffset);

                pnlMapCanvas.Controls.Add(_cursorItem);
                this.Controls.Remove(_cursorItem);
                _cursorItem.Click += GameObjectMapping_Click;
                _selectedGameObjectMapping = _cursorItem;
                ClearSelectedMapItem();
                mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject);

                _cursorItem = null;
            }
        }

        #endregion
    }
}
