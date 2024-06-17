using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.UserControls.MapMakerUserControls;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private SmartPlacer _smartPlacer = new SmartPlacer();
        private bool _mouseInCanvas;
        private bool _useSmartPlacer = false;
        private bool _mapHasUnsavedChanges = false;

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
            mapMakerObjectPropertyListControl1.PlaceObjectClicked += MapMakerObjectPropertyListControl1_UpdateObjectClicked;
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
            this.cmbWidth.SelectedIndexChanged += new EventHandler(this.CmbWidth_SelectedIndexChanged);
            this.cmbHeight.SelectedIndexChanged += new EventHandler(this.CmbHeight_SelectedIndexChanged);
        }

        private void SetDimensionsToDefaultValues()
        {
            cmbWidth.SelectedItem = MapMakerConstants.DEFAULT_MAP_WIDTH;
            cmbHeight.SelectedItem = MapMakerConstants.DEFAULT_MAP_HEIGHT;
        }

        private void InitializeCategories()
        {
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_BACKGROUNDS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_ANIMATED_BACKGROUNDS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_FOREGROUNDS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_ANIMATED_FOREGROUNDS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_TILES);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_CONSTRUCTS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_PLAYER);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_POINTS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_LIVES);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_GEMS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_WEAPONS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_HAZARDS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_ENEMIES);

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

        private bool UserWantsSmartPlacer()
        {
            return chkUseSmartPlacer.Checked;
        }

        private void ClearSelectedMapItem()
        {
            if (_selectedGameObjectMapping != null)
            {
                _selectedGameObjectMapping.BorderStyle = BorderStyle.None;
                _selectedGameObjectMapping = null;
            }
        }
        private void RemoveCursorItem()
        {
            if (_cursorItem != null)
            {
                this.Controls.Remove(_cursorItem);
                _cursorItem = null;
            }
        }
        private void RefreshZIndexPositioning()
        {
            var orderedByZindexObjects = _mapMakerObjects.OrderBy(o => o.GameObject.ZIndex);
            foreach (var obj in orderedByZindexObjects)
            {
                obj.BringToFront();
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
            int width = (int)(cmbWidth?.SelectedItem ?? MapMakerConstants.DEFAULT_MAP_WIDTH);
            int height = (int)(cmbHeight?.SelectedItem ?? MapMakerConstants.DEFAULT_MAP_HEIGHT);

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

        private void ClearMapMakerSelection()
        {
            mapObjectContainer1.ClearSelection();
            mapMakerObjectPropertyListControl1.SetProperties(null);
            ClearSelectedMapItem();
            RemoveCursorItem();
        }

        private void ValidateMapObjects()
        {
            MapMakerData data = new MapMakerData()
            {
                MapData = _mapMakerObjects,
                MapName = txtMapName.Text,
                MapSize = this.GetMapSize()
            };
            if (!MapUtility.Validation.ValidateAllMapObjects(data, out List<string> errorMessages))
            {
                string errorMessageText = string.Join("\n", errorMessages);
                MessageBox.Show(errorMessageText, "Invalid Map Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ValidateMap()
        {

            MapMakerData mapMakerData = new MapMakerData()
            {
                MapData = _mapMakerObjects,
                MapName = txtMapName.Text,
                MapSize = this.GetMapSize()
            };

            if (!MapUtility.Validation.ValidateMap(mapMakerData, out List<string> errorMessages))
            {
                string headerText = $"Map '{txtMapName.Text}' did not save successfully. Errors: ";
                errorMessages.Insert(0, headerText);
                string validationMessage = string.Join("\n", errorMessages);
                MessageBox.Show(validationMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void RemoveSmartPlacerFromCanvas()
        {
            pnlMapCanvas.Controls.Remove(_smartPlacer);
            _smartPlacer.RemoveDrawing();
        }

        private void RegisterEventsForGameObjectMapping(GameObjectMapping gameObjectMapping)
        {
            gameObjectMapping.Click += GameObjectMapping_Click;
            gameObjectMapping.MouseEnter += PnlMapCanvas_MouseEnter;
            gameObjectMapping.MouseLeave += PnlMapCanvas_MouseLeave;
            gameObjectMapping.MouseMove += PnlMapCanvas_MouseMove;
        }

        private void UnRegisterEventsForGameObjectMapping(GameObjectMapping gameObjectMapping)
        {
            gameObjectMapping.Click -= GameObjectMapping_Click;
            gameObjectMapping.MouseEnter -= PnlMapCanvas_MouseEnter;
            gameObjectMapping.MouseLeave -= PnlMapCanvas_MouseLeave;
            gameObjectMapping.MouseMove -= PnlMapCanvas_MouseMove;
        }

        #endregion

        #region event handlers

        private void MapMakerObjectPropertyListControl1_UpdateObjectClicked(object sender, ControlEventArgs.MapMakerObjectEventArgs e)
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
                // gameObjectMapping.Click += GameObjectMapping_Click;
                RegisterEventsForGameObjectMapping(gameObjectMapping);

                //add to collections
                _mapMakerObjects.Add(gameObjectMapping);
                pnlMapCanvas.Controls.Add(gameObjectMapping);

                //redraw grid
                RefreshZIndexPositioning();

                //replace existing selection if we have one
                if (_selectedGameObjectMapping != null)
                {
                    _selectedGameObjectMapping = gameObjectMapping;
                }

                _mapHasUnsavedChanges = true;
            }
            catch
            {
                MessageBox.Show("Error placing object on the map. It is not in the standard sprite format.");
            }
        }

        private void GameObjectMapping_Click(object sender, EventArgs e)
        {
            if (_cursorItem == null)
            {
                ClearSelectedMapItem();
                _selectedGameObjectMapping = sender as GameObjectMapping;
                if (_selectedGameObjectMapping != null)
                {
                    mapObjectContainer1.ClearSelection();
                    _selectedGameObjectMapping.BorderStyle = BorderStyle.Fixed3D;
                    mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject, true);
                }
            }
            else
            {
                PnlMapCanvas_Click(this, EventArgs.Empty);
            }
        }

        private void MapMaker_Load(object sender, EventArgs e)
        {
            InitializeMapMaker();
        }
        private void MapObjectContainer1_ObjectClicked(object sender, ControlEventArgs.MapMakerObjectEventArgs e)
        {
            ClearSelectedMapItem();
            if (e.MapMakerObject == null)
            {
                mapMakerObjectPropertyListControl1.SetProperties(null);
                return;
            }

            if (e.MapMakerObject.IsManualPlacement)
            {
                mapMakerObjectPropertyListControl1.SetProperties(e.MapMakerObject);
            }
            else
            {
                RemoveCursorItem();
                GameObjectMapping mapping = GenerateMappingObjectFromMapMakerData(e.MapMakerObject);
                _cursorItem = mapping;
                mapMakerObjectPropertyListControl1.SetProperties(null);
                this.Controls.Add(_cursorItem);
                _cursorItem.BringToFront();
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
                    ClearMapMakerSelection();
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
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    if (_selectedGameObjectMapping != null)
                    {
                        _selectedGameObjectMapping.BorderStyle = BorderStyle.Fixed3D;
                        mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject, true, true);
                    }
                    break;
                case Keys.Space:
                    if (_cursorItem != null && UserWantsSmartPlacer())
                    {
                        _useSmartPlacer = true;
                        PnlMapCanvas_Click(this, EventArgs.Empty);
                    }
                    break;
            }
        }

        private void MapMaker_KeyDown(object sender, KeyEventArgs e)
        {
            if (_selectedGameObjectMapping != null)
            {
                var areaProperty = _selectedGameObjectMapping.MapMakerObject?.ConstructorParameters?
                     .FirstOrDefault(p => p.PropertyName == GeneralGameConstants.AREA_PROPERTY_NAME);
                if (areaProperty != null)
                {
                    var areaRect = (Rectangle)areaProperty.Value;
                    var mapSize = GetMapSize();
                    switch (e.KeyData)
                    {
                        case Keys.Up:
                            if (areaRect.Y > 0)
                            {
                                areaProperty.Value = new Rectangle(areaRect.X, areaRect.Y - 1, areaRect.Width, areaRect.Height);
                                _selectedGameObjectMapping.Location = new Point(areaRect.Location.X, areaRect.Location.Y - 1);
                                mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject, true);
                            }
                            break;
                        case Keys.Down:
                            if (areaRect.Y < mapSize.Height - areaRect.Height)
                            {
                                areaProperty.Value = new Rectangle(areaRect.X, areaRect.Y + 1, areaRect.Width, areaRect.Height);
                                _selectedGameObjectMapping.Location = new Point(areaRect.Location.X, areaRect.Location.Y + 1);
                                mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject, true);
                            }
                            break;
                        case Keys.Left:
                            if (areaRect.X > 0)
                            {
                                areaProperty.Value = new Rectangle(areaRect.X - 1, areaRect.Y, areaRect.Width, areaRect.Height);
                                _selectedGameObjectMapping.Location = new Point(areaRect.Location.X - 1, areaRect.Location.Y);
                                mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject, true);
                            }
                            break;
                        case Keys.Right:
                            if (areaRect.X < mapSize.Width - areaRect.Width)
                            {
                                areaProperty.Value = new Rectangle(areaRect.X + 1, areaRect.Y, areaRect.Width, areaRect.Height);
                                _selectedGameObjectMapping.Location = new Point(areaRect.Location.X + 1, areaRect.Location.Y);
                                mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject, true);
                            }
                            break;
                    }
                }
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

            MapMakerData mapMakerData = new MapMakerData()
            {
                MapData = _mapMakerObjects,
                MapName = txtMapName.Text,
                MapSize = mapSize
            };

            if (!MapUtility.Validation.ValidateMap(mapMakerData, out List<string> errorMessages))
            {
                string headerText = $"Map '{txtMapName.Text}' did not save successfully. Errors: ";
                errorMessages.Insert(0, headerText);
                string validationMessage = string.Join("\n", errorMessages);
                MessageBox.Show(validationMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MapUtility.SaveMap(txtMapName.Text, cmbGameMode.SelectedItem?.ToString(), mapSize, _mapMakerObjects))
            {
                MessageBox.Show($"Map '{txtMapName.Text}' was saved successfully!");
                _mapHasUnsavedChanges = false;
            }
            else
            {
                MessageBox.Show($"Map '{txtMapName.Text}' did not save successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (_mapHasUnsavedChanges 
                && MessageBox.Show("This map has unsaved changes, and this action will override those changes. Continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

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
                        //item.Click -= GameObjectMapping_Click;
                        UnRegisterEventsForGameObjectMapping(item);
                    }
                }
                //clear out the canvas
                pnlMapCanvas.Controls.Clear();

                //register new data on grid and load to canvas
                foreach (var mapObject in _mapMakerObjects)
                {
                    RegisterEventsForGameObjectMapping(mapObject);
                    pnlMapCanvas.Controls.Add(mapObject);
                }

                //set the map name text box value to the name of the newly loaded map
                txtMapName.Text = mapMakerData.MapName;

                //set the dimensions based on the saved info
                int wIndex = cmbWidth.Items.IndexOf(mapMakerData.MapSize.Width);
                cmbWidth.SelectedIndex = wIndex;
                int hIndex = cmbHeight.Items.IndexOf(mapMakerData.MapSize.Height);
                cmbHeight.SelectedIndex = hIndex;
                ClearSelectedMapItem();
                RefreshZIndexPositioning();

                pnlMapCanvas.Focus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show($"Map did not load successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ClearMapMakerSelection();
            }
        }

        private void TxtMapName_TextChanged(object sender, EventArgs e)
        {
            txtMapName.Text = InputValidation.SanitizeFileNameInput(txtMapName.Text);
            dialogMapLoader.FileName = string.Empty;
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

                Rectangle area = _useSmartPlacer
                    ? new Rectangle(_smartPlacer.Location.X, _smartPlacer.Location.Y, _smartPlacer.Width, _smartPlacer.Height)
                    : new Rectangle(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset, _cursorItem.Width, _cursorItem.Height);

                _useSmartPlacer = false;
                SetNewAreaForMappingObject(area, _cursorItem);

                pnlMapCanvas.Controls.Add(_cursorItem);
                _mapMakerObjects.Add(_cursorItem);
                this.Controls.Remove(_cursorItem);
                // _cursorItem.Click += GameObjectMapping_Click;
                RegisterEventsForGameObjectMapping(_cursorItem);

                ClearSelectedMapItem();
                _selectedGameObjectMapping = _cursorItem;
                _selectedGameObjectMapping.BorderStyle = BorderStyle.Fixed3D;
                mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject, true);

                _cursorItem = null;
                RefreshZIndexPositioning();

                RemoveSmartPlacerFromCanvas();

                _mapHasUnsavedChanges = true;
            }
        }

        private void SetNewAreaForMappingObject(Rectangle area, GameObjectMapping mapping)
        {
            var areaProperty = mapping.MapMakerObject.ConstructorParameters.FirstOrDefault(p => p.PropertyName == GeneralGameConstants.AREA_PROPERTY_NAME);
            if (areaProperty != null && mapping?.MapMakerObject != null)
            {
                areaProperty.Value = area;
                mapping.GameObject = (ISprite)mapping.MapMakerObject.Construct();
                mapping.Location = mapping.GameObject.Location;
            }
        }

        private void CmbWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ValidateMap();
        }

        private void CmbHeight_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ValidateMap();
        }

        private void PnlMapCanvas_MouseEnter(object sender, EventArgs e)
        {
            _mouseInCanvas = true;
        }

        private void PnlMapCanvas_MouseLeave(object sender, EventArgs e)
        {
            _mouseInCanvas = false;
        }

        private void PnlMapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseInCanvas && _cursorItem != null && UserWantsSmartPlacer())
            {
                if (_mapMakerObjects.Any())
                {
                    GameObjectMapping mapping = new GameObjectMapping()
                    {
                        Location = new Point(_cursorItem.Location.X - pnlMapCanvas.Location.X, _cursorItem.Location.Y - pnlMapCanvas.Location.Y),
                        GameObject = _cursorItem.GameObject,
                        MapMakerObject = _cursorItem.MapMakerObject,
                        Image = _cursorItem.Image,
                        Size = new Size(_cursorItem.Width, _cursorItem.Height)
                    };
                    var objectClosest = _smartPlacer.FindClosestBlockOfSameType(_mapMakerObjects, mapping, out Direction? direction);
                    if (objectClosest != null && direction != null)
                    {
                        _smartPlacer.DrawAdjacent(_cursorItem.Size, _mapMakerObjects, objectClosest, direction.Value);
                        if (!pnlMapCanvas.Controls.Contains(_smartPlacer))
                        {
                            pnlMapCanvas.Controls.Add(_smartPlacer);
                            _smartPlacer.BringToFront();
                        }
                    }
                    else
                    {
                        RemoveSmartPlacerFromCanvas();
                    }
                }
            }
        }

        private void ChkUseSmartPlacer_CheckedChanged(object sender, EventArgs e)
        {
            if (pnlMapCanvas.Controls.Contains(_smartPlacer))
                RemoveSmartPlacerFromCanvas();
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Under Construction.");
        }

        private void BtnNewMap_Click(object sender, EventArgs e)
        {
            if (_mapHasUnsavedChanges
             && MessageBox.Show("This map has unsaved changes, and this action will override those changes. Continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            //clear events for existing items
            var existingItems = pnlMapCanvas.Controls.OfType<GameObjectMapping>();
            if (existingItems.Any())
            {
                foreach (var item in existingItems)
                {
                    UnRegisterEventsForGameObjectMapping(item);
                }
            }
            //clear out the canvas
            pnlMapCanvas.Controls.Clear();

            //set map name default
            txtMapName.Text = "<New Map>";
        }

        private void MapMaker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_mapHasUnsavedChanges
           && MessageBox.Show("This map has unsaved changes. Closing the map maker will cause you to lose these changes. Continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        #endregion
    }
}