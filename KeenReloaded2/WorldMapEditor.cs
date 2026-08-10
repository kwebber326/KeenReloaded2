using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.DataStructures;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2
{
    public partial class WorldMapEditor : Form
    {
        private readonly Dictionary<string, string> _episodeFileFolderDict = new Dictionary<string, string>();
        private readonly string WORKING_DIRECTORY = Path.Combine(Environment.CurrentDirectory, FileIOUtility.WORLD_MAPS_DIRECTORY);
        private readonly Timer _cursorUpdateTimer = new Timer();
        private string _objectDirectory;
        private GameObjectMapping _selectedGameObjectMapping;
        private GameObjectMapping _cursorItem;
        private OrderedList<GameObjectMapping> _mapMakerObjects;
        private bool _useSmartPlacer;
        private bool _mapHasUnsavedChanges;
        private SmartPlacer _smartPlacer = new SmartPlacer();
        private bool _mouseInCanvas;
        private Func<GameObjectMapping, GameObjectMapping, int> _comparatorFunction = (o1, o2) =>
        {
            int value1 = o1?.GameObject?.ZIndex ?? -10000;
            int value2 = o2?.GameObject?.ZIndex ?? -10000;

            if (value1 > value2) return 1;
            if (value1 < value2) return -1;
            return 0;
        };
        private string _lastFilePath;
        private bool _showSaveMessage;
        private bool _advancedToolsOpen;
        private bool _levelObjectivesFormOpen;
        private LevelObjectivesForm _levelObjectivesForm;
        private AdvancedToolsForm _advancedToolsForm;
        private WorldMapObjectiveManager _worldMapData
            = new WorldMapObjectiveManager();

        public WorldMapEditor()
        {
            InitializeComponent();
            _mapMakerObjects = new OrderedList<GameObjectMapping>(_comparatorFunction);
        }

        public Point PlayerPosition
        {
            get
            {
                int x = int.TryParse(txtPlayerX.Text, out int xVal) ? xVal : 0;
                int y = int.TryParse(txtPlayerY.Text, out int yVal) ? yVal : 0;

                return new Point(x, y);
            }
            private set
            {
                txtPlayerX.Text = value.X.ToString();
                txtPlayerY.Text = value.Y.ToString();
            }
        }

        private void WorldMapEditor_Load(object sender, EventArgs e)
        {
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            InitializeEpisodeList();
            InitializeSongList();
            mapObjectContainer1.ObjectClicked += MapObjectContainer1_ObjectClicked;
            _cursorUpdateTimer.Interval = 10;
            _cursorUpdateTimer.Tick += _cursorUpdateTimer_Tick;
            mapMakerObjectPropertyListControl1.SetObjectBank(_mapMakerObjects);
            mapMakerObjectPropertyListControl1.PlaceObjectClicked += MapMakerObjectPropertyListControl1_PlaceObjectClicked;
            txtPlayerX.KeyPress += NumericTextBox_KeyPress;
            txtPlayerY.KeyPress += NumericTextBox_KeyPress;

            this.SubscribeToEventStoreEvents();
        }

        // Allow only digits + control keys (Backspace, etc.)
        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // block non-control and non-digit characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // cancel the key press
            }
            else
            {
                _mapHasUnsavedChanges = true;
            }
        }

        private void MapMakerObjectPropertyListControl1_PlaceObjectClicked(object sender, MapMakerObjectEventArgs e)
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

                //remove existing item if it is there
                if (_selectedGameObjectMapping != null)
                {
                    pnlCanvas.Controls.Remove(_selectedGameObjectMapping);
                    _mapMakerObjects.Remove(_selectedGameObjectMapping);
                }
                //add new item
                //construct mapping
                GameObjectMapping gameObjectMapping = new GameObjectMapping()
                {
                    GameObject = placedItem,
                    MapMakerObject = mapMakerObjectCopy
                };
                gameObjectMapping.Location = new Point(placedItem.Location.X + pnlCanvas.AutoScrollPosition.X,
                    placedItem.Location.Y + pnlCanvas.AutoScrollPosition.Y);
                gameObjectMapping.SizeMode = PictureBoxSizeMode.AutoSize;
                gameObjectMapping.Image = placedItem.Image;
                // register events
                RegisterEventsForGameObjectMapping(gameObjectMapping);

                //add to collections
                _mapMakerObjects.InsertAscending(gameObjectMapping);
                pnlCanvas.Controls.Add(gameObjectMapping);

                //reposition only the necessary items (items that collide and have higher zIndex)
                RefreshZIndexPositioningForCollidingObjects(gameObjectMapping);

                //replace existing selection if we have one
                if (_selectedGameObjectMapping != null)
                {
                    _selectedGameObjectMapping = gameObjectMapping;
                }

                _mapHasUnsavedChanges = true;

                if (e.MapMakerObject.ObjectType == typeof(Door))
                {
                    mapMakerObjectPropertyListControl1.SetProperties(e.MapMakerObject, true);
                }
            }
            catch
            {
                MessageBox.Show("Error placing object on the map. It is not in the standard sprite format.");
            }
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

        private void InitializeEpisodeList()
        {
            cmbEpisode.Items.Add(GeneralGameConstants.Episodes.EPISODE_4);
            cmbEpisode.Items.Add(GeneralGameConstants.Episodes.EPISODE_5);
            cmbEpisode.Items.Add(GeneralGameConstants.Episodes.EPISODE_6);
            _episodeFileFolderDict.Add(GeneralGameConstants.Episodes.EPISODE_4, "world_map_keen4");
            _episodeFileFolderDict.Add(GeneralGameConstants.Episodes.EPISODE_5, "world_map_keen5");
            _episodeFileFolderDict.Add(GeneralGameConstants.Episodes.EPISODE_6, "world_map_keen6");
            cmbEpisode.SelectedIndexChanged += CmbEpisode_SelectedIndexChanged;
            cmbEpisode.SelectedIndex = 0;
        }

        private void InitializeSongList()
        {
            var songs = FileIOUtility.LoadWavFormatSongs();
            foreach (var song in songs)
            {
                cmbMusic.Items.Add(song);
            }

            cmbMusic.SelectedIndex = 0;
        }

        private void RemoveCursorItem()
        {
            if (_cursorItem != null)
            {
                this.Controls.Remove(_cursorItem);
                _cursorItem = null;
            }
        }

        private void ClearFocus()
        {
            label1.Focus();
        }

        private void ClearMapMakerSelection()
        {
            mapObjectContainer1.ClearSelection();
            mapMakerObjectPropertyListControl1.SetProperties(null);
            ClearSelectedMapItem();
            RemoveCursorItem();
            if (pnlCanvas.Controls.Contains(_smartPlacer))
                RemoveSmartPlacerFromCanvas();
        }

        private void ClearSelectedMapItem()
        {
            if (_selectedGameObjectMapping != null)
            {
                _selectedGameObjectMapping.BorderStyle = BorderStyle.None;
                _selectedGameObjectMapping = null;
            }
        }

        private void MapObjectContainer1_ObjectClicked(object sender, ControlEventArgs.MapMakerObjectEventArgs e)
        {
            try
            {
                ClearFocus();
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
            catch
            {
                GenerateObjectConstructionErrorMessage();
            }
        }

        private GameObjectMapping GenerateMappingObjectFromMapMakerData(MapMakerObject mapMakerObject)
        {
            MapMakerObject obj = new MapMakerObject(
                  mapMakerObject.ObjectType,
                  mapMakerObject.ImageControl.ImageLocation,
                  mapMakerObject.IsManualPlacement,
                  mapMakerObject.CloneParameterList());
            if (obj.ObjectType.GetInterface(nameof(IActivateable)) != null)
            {
                var property = obj.ConstructorParameters.FirstOrDefault(p => p.PropertyName == GeneralGameConstants.ACTIVATION_ID_PROPERTY_NAME);
                if (property != null)
                {
                    property.Value = Guid.NewGuid();
                }
            }

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

        private static void GenerateObjectConstructionErrorMessage()
        {
            string expectedDirectory = FileIOUtility.GetResourcePathForMainProject();
            MessageBox.Show($"Unable to construct container object. Ensure the associated image is present in the following directory:\n{expectedDirectory}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CmbEpisode_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateObjectDirectory();
            SetSubCategories();
        }
        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string folder = cmbCategory.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(folder))
                return;

            string path = Path.Combine(_objectDirectory, folder);

            string[] files = Directory.GetFiles(path);
            mapObjectContainer1.DisplayImageFiles(files);
        }

        private void UpdateObjectDirectory()
        {
            try
            {
                string selectedEpisode = cmbEpisode.SelectedItem?.ToString();
                string objectFolder = _episodeFileFolderDict.TryGetValue(selectedEpisode, out string folder)
                    ? folder : throw new FileNotFoundException("Cannot find file for episode: " + selectedEpisode);

                if (string.IsNullOrEmpty(folder))
                {
                    throw new ArgumentException("Invalid directory name for episode: " + selectedEpisode);
                }

                _objectDirectory = Path.Combine(WORKING_DIRECTORY, folder);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading objects: " + ex.Message);
            }
        }

        private void SetSubCategories()
        {
            try
            {
                string[] directories = Directory.GetDirectories(_objectDirectory);

                cmbCategory.Items.Clear();

                foreach (string directory in directories)
                {
                    string category = directory.Substring(directory.LastIndexOf('\\') + 1);
                    cmbCategory.Items.Add(category);
                }

                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }

        private void GameObjectMapping_Click(object sender, EventArgs e)
        {
            ClearFocus();
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
                pnlCanvas_Click(this, EventArgs.Empty);
            }
        }

        private void pnlCanvas_Click(object sender, EventArgs e)
        {
            try
            {
                if (_cursorItem != null)
                {
                    int xOffset = pnlCanvas.Location.X;
                    int yOffset = pnlCanvas.Location.Y;

                    Rectangle area = _useSmartPlacer
                        ? new Rectangle(_smartPlacer.Location.X, _smartPlacer.Location.Y, _smartPlacer.Width, _smartPlacer.Height)
                        : new Rectangle(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset, _cursorItem.Width, _cursorItem.Height);

                    _useSmartPlacer = false;
                    SetNewAreaForMappingObject(area, _cursorItem);

                    pnlCanvas.Controls.Add(_cursorItem);
                    _mapMakerObjects.InsertAscending(_cursorItem);
                    this.Controls.Remove(_cursorItem);

                    Rectangle offsetArea = new Rectangle(area.X - pnlCanvas.AutoScrollPosition.X,
                        area.Y - pnlCanvas.AutoScrollPosition.Y,
                        area.Width, area.Height);
                    SetNewAreaForMappingObject(offsetArea, _cursorItem, true);
                    RegisterEventsForGameObjectMapping(_cursorItem);

                    ClearSelectedMapItem();
                    _selectedGameObjectMapping = _cursorItem;
                    _selectedGameObjectMapping.BorderStyle = BorderStyle.Fixed3D;
                    mapMakerObjectPropertyListControl1.SetProperties(_selectedGameObjectMapping.MapMakerObject, true);

                    var tmp = _cursorItem;
                    _cursorItem = null;
                    RefreshZIndexPositioningForCollidingObjects(tmp);

                    RemoveSmartPlacerFromCanvas();

                    _mapHasUnsavedChanges = true;
                }
            }
            catch (Exception ex)
            {
                GenerateObjectConstructionErrorMessage();
            }
        }

        private void RefreshZIndexPositioningForCollidingObjects(GameObjectMapping newlyPlacedObject)
        {
            var objectsToBringToFront = _mapMakerObjects.Where(o =>
                (o?.GameObject?.ZIndex ?? -10000) > newlyPlacedObject.GameObject.ZIndex);
            newlyPlacedObject.BringToFront();
            foreach (var item in objectsToBringToFront)
            {
                item.BringToFront();
            }
        }

        private void RegisterEventsForGameObjectMapping(GameObjectMapping gameObjectMapping)
        {
            gameObjectMapping.Click += GameObjectMapping_Click;
            gameObjectMapping.MouseEnter += pnlCanvas_MouseEnter;
            gameObjectMapping.MouseLeave += pnlCanvas_MouseLeave;
            gameObjectMapping.MouseMove += pnlCanvas_MouseMove;
        }

        private void UnRegisterEventsForGameObjectMapping(GameObjectMapping gameObjectMapping)
        {
            gameObjectMapping.Click -= GameObjectMapping_Click;
            gameObjectMapping.MouseEnter -= pnlCanvas_MouseEnter;
            gameObjectMapping.MouseLeave -= pnlCanvas_MouseLeave;
            gameObjectMapping.MouseMove -= pnlCanvas_MouseMove;
        }

        private void SetNewAreaForMappingObject(Rectangle area, GameObjectMapping mapping, bool ignoreMapCanvasPositioning = false)
        {
            var areaProperty = mapping.MapMakerObject.ConstructorParameters.FirstOrDefault(p => p.PropertyName == GeneralGameConstants.AREA_PROPERTY_NAME);
            if (areaProperty != null && mapping?.MapMakerObject != null)
            {
                areaProperty.Value = area;
                mapping.GameObject = (ISprite)mapping.MapMakerObject.Construct();
                if (!ignoreMapCanvasPositioning)
                {
                    mapping.Location = mapping.GameObject.Location;
                }
            }
        }


        private void RemoveSmartPlacerFromCanvas()
        {
            pnlCanvas.Controls.Remove(_smartPlacer);
            _smartPlacer.RemoveDrawing();
        }

        private void pnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseInCanvas && _cursorItem != null && UserWantsSmartPlacer())
            {
                if (_mapMakerObjects.Any())
                {
                    GameObjectMapping mapping = new GameObjectMapping()
                    {
                        Location = new Point(_cursorItem.Location.X - pnlCanvas.Location.X, _cursorItem.Location.Y - pnlCanvas.Location.Y),
                        GameObject = _cursorItem.GameObject,
                        MapMakerObject = _cursorItem.MapMakerObject,
                        Image = _cursorItem.Image,
                        Size = new Size(_cursorItem.Width, _cursorItem.Height)
                    };
                    var objectClosest = _smartPlacer.FindClosestBlockOfSameType(_mapMakerObjects, mapping, out Direction? direction);
                    if (objectClosest != null && direction != null)
                    {
                        _smartPlacer.DrawAdjacent(_cursorItem.Size, _mapMakerObjects, objectClosest, direction.Value);
                        if (!pnlCanvas.Controls.Contains(_smartPlacer))
                        {
                            pnlCanvas.Controls.Add(_smartPlacer);
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

        private bool UserWantsSmartPlacer()
        {
            return chkSmartPlacer.Checked;
        }

        private void pnlCanvas_MouseEnter(object sender, EventArgs e)
        {
            _mouseInCanvas = true;
        }

        private void pnlCanvas_MouseLeave(object sender, EventArgs e)
        {
            _mouseInCanvas = false;
        }

        private void WorldMapEditor_KeyUp(object sender, KeyEventArgs e)
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
                        pnlCanvas.Controls.Remove(_selectedGameObjectMapping);
                        mapObjectContainer1.ClearSelection();
                        mapMakerObjectPropertyListControl1.SetProperties(null);
                        ClearSelectedMapItem();
                        _mapHasUnsavedChanges = true;
                    }
                    break;
                case Keys.Space:
                    if (_cursorItem != null && UserWantsSmartPlacer())
                    {
                        _useSmartPlacer = true;
                        pnlCanvas_Click(this, EventArgs.Empty);
                    }
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _showSaveMessage = true;
            TrySaveMap();
        }

        private bool TrySaveMap()
        {
            if (string.IsNullOrWhiteSpace(txtMapName.Text))
            {
                MessageBox.Show("Enter a map name");
                return false;
            }

            Rectangle playerHitBox = new Rectangle(PlayerPosition.X, PlayerPosition.Y,
                16, 16);

            if (_mapMakerObjects.Count(m => m.GameObject is WorldMapPlayer) > 1)
            {
                MessageBox.Show("Cannot have duplicate players on the board. Delete extra world map players before saving,");
                return false;
            }
            else
            {
                WorldMapPlayer player = new WorldMapPlayer(playerHitBox, null, 20);

                if (!_mapMakerObjects.Select(g => g.GameObject).OfType<WorldMapPlayer>().Any())
                {
                    GameObjectMapping playerMapping = new GameObjectMapping();
                    playerMapping.GameObject = player;
                    _mapMakerObjects.Add(playerMapping);
                }
                else
                {
                    var playerMapping = _mapMakerObjects.First(g => g.GameObject is WorldMapPlayer);
                    playerMapping.GameObject = player;
                }
            }

            var rectangles = _mapMakerObjects.Select(s => new Rectangle(s.Location, s.Size)).ToList();
            var minX = rectangles.Select(l => l.Left).Min();
            var maxX = rectangles.Select(l => l.Right).Max();
            var minY = rectangles.Select(l => l.Top).Min();
            var maxY = rectangles.Select(l => l.Bottom).Max();

            if (minX > 0)
                minX = 0;
            if (minY > 0)
                minY = 0;

            var width = maxX - minX;
            var height = maxY - minY;

            var mapSize = new Size(width, height);
            bool successfulSave = MapUtility.SaveMap(txtMapName.Text, MainMenuConstants.OPTION_LABEL_WORLD_MODE, mapSize, _mapMakerObjects);
            if (successfulSave && _showSaveMessage)
            {
                MessageBox.Show($"Map '{txtMapName.Text}' was saved successfully!");
                _mapHasUnsavedChanges = false;
            }
            else if (!successfulSave)
            {
                MessageBox.Show($"Map '{txtMapName.Text}' did not save successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            string selectedSong = cmbMusic.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(selectedSong) &&
                !MapUtility.SaveWorldMapMusic(txtMapName.Text, selectedSong))
            {
                MessageBox.Show($"Music for map '{txtMapName.Text}' did not save successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return true;
        }

        private void SubscribeToEventStoreEvents()
        {
            //Advanced Tools Events
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, AdvancedTools_SelectionChanged);
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW, AdvancedTools_ActionPreview);
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT, AdvancedTools_ActionCommit);
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_UNDO, AdvancedTools_ActionCancel);
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_CANCEL, AdvancedTools_ActionCancel);

            EventStore<ActivatorSelectionChangedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_CHANGED, ActivatorSelection_Changed);
            EventStore<ActivatorSelectionCompletedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_COMPLETE, ActivatorSelection_Complete);
        }

        private void UnsubscribeToEventStoreEvents()
        {
            //Advanced Tools Events
            EventStore<ActivatorSelectionChangedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_CHANGED, ActivatorSelection_Changed);
            EventStore<ActivatorSelectionCompletedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_COMPLETE, ActivatorSelection_Complete);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, AdvancedTools_SelectionChanged);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW, AdvancedTools_ActionPreview);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT, AdvancedTools_ActionCommit);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_UNDO, AdvancedTools_ActionCancel);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_CANCEL, AdvancedTools_ActionCancel);
        }

        private void UpdatePlayerPositionOnCanvas()
        {
            int x = PlayerPosition.X; int y = PlayerPosition.Y;
            int canvasX = x + pnlCanvas.Location.X;
            int canvasY = y + pnlCanvas.Location.Y;
            var player = _mapMakerObjects.Select(g => g.GameObject).OfType<WorldMapPlayer>().FirstOrDefault();
            if (player != null)
            {
                var objectMapping = this.TryGetWorldMapPlayerObjectMapping(out GameObjectMapping mapping)
                   ? mapping : null;

                if (objectMapping != null)
                    objectMapping.Location = new Point(x, y);
            }
            else
            {
                var objectMapping = new GameObjectMapping()
                {
                    GameObject = new WorldMapPlayer(new Rectangle(x, y, 16, 16), null, 20),
                    Location = new Point(x, y)
                };
                _mapMakerObjects.InsertAscending(objectMapping);
                pnlCanvas.Controls.Add(objectMapping);
                RefreshZIndexPositioningForCollidingObjects(objectMapping);
            }
        }

        private void SetHighlightStateForSelection(List<GameObjectMapping> selection, bool highlighted)
        {
            if (selection == null || !selection.Any())
                return;

            foreach (var selectedItem in selection)
            {
                if (highlighted)
                {
                    selectedItem.BorderStyle = BorderStyle.Fixed3D;
                    selectedItem.BackColor = Color.Red;
                }
                else
                {
                    selectedItem.BorderStyle = BorderStyle.None;
                    selectedItem.BackColor = Color.Transparent;
                }
            }
        }

        private void ActivatorSelection_Changed(object sender, ControlEventArgs.ControlEventArgs<ActivatorSelectionChangedEventArgs> e)
        {
            HighlightActivateables(e.Data.CurrentActivateablesSelected, Color.Red, true);
            HighlightActivateables(e.Data.CurrentActiveablesUnSelected, Color.Transparent, true);
            HighlightActivateables(e.Data.OtherActivateablesSelected, Color.Blue);
            HighlightActivateables(e.Data.OtherActiveablesUnSelected, Color.Transparent);
        }

        private void ActivatorSelection_Complete(object sender, ControlEventArgs.ControlEventArgs<ActivatorSelectionCompletedEventArgs> e)
        {
            HighlightActivateables(e.Data.Activateables, Color.Transparent);
        }

        private void HighlightActivateables(List<IActivateable> activateables, Color color, bool addBorder = false)
        {
            foreach (var item in activateables)
            {
                var obj = _mapMakerObjects.FirstOrDefault(d => d.GameObject == item);
                if (obj != null)
                {
                    obj.BackColor = color;
                    if (addBorder)
                    {
                        obj.BorderStyle = BorderStyle.Fixed3D;
                    }
                    else
                    {
                        obj.BorderStyle = BorderStyle.None;
                    }
                }
            }
        }

        private void txtPlayerX_TextChanged(object sender, EventArgs e)
        {
            UpdatePlayerPositionOnCanvas();
        }

        private void txtPlayerY_TextChanged(object sender, EventArgs e)
        {
            UpdatePlayerPositionOnCanvas();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (_mapHasUnsavedChanges
              && MessageBox.Show("This map has unsaved changes, and this action will override those changes. Continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            dialogMapLoader.InitialDirectory = MapUtility.GetSavedMapsPath(MainMenuConstants.OPTION_LABEL_WORLD_MODE);
            dialogMapLoader.Filter = "*.txt|";
            dialogMapLoader.ShowDialog();
        }

        private void AdvancedTools_ActionPreview(object sender, ControlEventArgs<AdvancedToolsEventArgs> e)
        {
            List<GameObjectMapping> changedData = e?.Data?.ChangeData?.ChangedData as List<GameObjectMapping>;
            if (changedData == null || !changedData.Any())
                return;
            var action = e.Data.ChangeData.Action;

            if (action == AdvancedToolsActions.EXTEND ||
                action == AdvancedToolsActions.COPY ||
                action == AdvancedToolsActions.MOVE)
            {
                foreach (var item in changedData)
                {
                    item.BackColor = Color.Red;
                    item.BorderStyle = BorderStyle.Fixed3D;
                    if (action != AdvancedToolsActions.MOVE)
                    {
                        _mapMakerObjects.InsertAscending(item);
                        pnlCanvas.Controls.Add(item);
                        item.BringToFront();
                    }
                    Rectangle offsetArea = new Rectangle(item.Location.X + pnlCanvas.AutoScrollPosition.X,
                      item.Location.Y + pnlCanvas.AutoScrollPosition.Y,
                      item.GameObject.Image.Width, item.GameObject.Image.Height);
                    item.Location = offsetArea.Location;
                }
            }
            else if (action == AdvancedToolsActions.DELETE)
            {
                foreach (var item in changedData)
                {
                    _mapMakerObjects.Remove(item);
                    pnlCanvas.Controls.Remove(item);
                    UnRegisterEventsForGameObjectMapping(item);
                }
            }
        }

        private void AdvancedTools_ActionCommit(object sender, ControlEventArgs<AdvancedToolsEventArgs> e)
        {
            List<GameObjectMapping> changedData = e?.Data?.ChangeData?.ChangedData as List<GameObjectMapping>;
            if (changedData == null || !changedData.Any())
                return;
            var action = e.Data.ChangeData.Action;
            if (action == AdvancedToolsActions.EXTEND ||
                action == AdvancedToolsActions.COPY ||
                action == AdvancedToolsActions.MOVE)
            {
                foreach (var item in changedData)
                {
                    item.BackColor = Color.Transparent;
                    item.BorderStyle = BorderStyle.None;
                    if (action != AdvancedToolsActions.MOVE)
                    {
                        RegisterEventsForGameObjectMapping(item);
                    }
                }
                if (action != AdvancedToolsActions.MOVE)
                {
                    RefreshZIndexPositioning();
                }
            }
            _mapHasUnsavedChanges = true;
        }

        private void AdvancedTools_ActionCancel(object sender, ControlEventArgs<AdvancedToolsEventArgs> e)
        {
            List<GameObjectMapping> changedData = e?.Data?.ChangeData?.ChangedData as List<GameObjectMapping>;
            if (changedData == null || !changedData.Any())
                return;
            var action = e.Data.ChangeData.Action;

            if (action == AdvancedToolsActions.EXTEND ||
              action == AdvancedToolsActions.COPY)
            {
                foreach (var item in changedData)
                {
                    item.BackColor = Color.Transparent;
                    item.BorderStyle = BorderStyle.None;
                    _mapMakerObjects.Remove(item);
                    pnlCanvas.Controls.Remove(item);
                    UnRegisterEventsForGameObjectMapping(item);
                }
            }
            else if (action == AdvancedToolsActions.DELETE)
            {
                foreach (var item in changedData)
                {
                    item.BackColor = Color.Transparent;
                    item.BorderStyle = BorderStyle.None;
                    _mapMakerObjects.InsertAscending(item);
                    pnlCanvas.Controls.Add(item);
                    RefreshZIndexPositioningForCollidingObjects(item);
                    RegisterEventsForGameObjectMapping(item);
                }
            }
        }

        private void AdvancedTools_SelectionChanged(object sender, ControlEventArgs<AdvancedToolsEventArgs> e)
        {
            var changedData = e?.Data?.ChangeData?.ChangedData;
            var objMetaData = e?.Data?.ChangeData?.ChangeMetaData;
            if (changedData == null)
                return;

            if (changedData is List<GameObjectMapping> && bool.TryParse(objMetaData?.ToString(), out bool isSelected))
            {
                List<GameObjectMapping> changedObjects = (List<GameObjectMapping>)changedData;
                SetHighlightStateForSelection(changedObjects, isSelected);
            }
        }

        private void dialogMapLoader_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                //load the map data
                string path = dialogMapLoader.FileName ??
                    Path.Combine(MapUtility.GetSavedMapsPath(MainMenuConstants.OPTION_LABEL_WORLD_MODE), txtMapName.Text);
                _lastFilePath = path;
                var mapMakerData = MapUtility.LoadMapData(path);
                _mapMakerObjects = OrderedList<GameObjectMapping>.FromEnumerable(mapMakerData.MapData, _comparatorFunction, true);
                mapMakerObjectPropertyListControl1.SetObjectBank(_mapMakerObjects);

                var playerPosition = _mapMakerObjects.Select(g => g.GameObject).OfType<WorldMapPlayer>().FirstOrDefault()?.Location;

                if (playerPosition != null)
                {
                    this.PlayerPosition = playerPosition.Value;
                }

                //clear events for existing items
                var existingItems = pnlCanvas.Controls.OfType<GameObjectMapping>();
                if (existingItems.Any())
                {
                    foreach (var item in existingItems)
                    {
                        //item.Click -= GameObjectMapping_Click;
                        UnRegisterEventsForGameObjectMapping(item);
                    }
                }
                //clear out the canvas
                pnlCanvas.Controls.Clear();

                //register new data on grid and load to canvas
                foreach (var mapObject in _mapMakerObjects)
                {
                    RegisterEventsForGameObjectMapping(mapObject);
                    pnlCanvas.Controls.Add(mapObject);
                }

                //set the map name text box value to the name of the newly loaded map
                txtMapName.Text = mapMakerData.MapName;

                //refresh map state
                ClearSelectedMapItem();
                RefreshZIndexPositioning();

                pnlCanvas.Focus();

                if (MapUtility.LoadWorldMapMusic(mapMakerData.MapName, out string music))
                {
                    var selectedIndex = cmbMusic.Items.IndexOf(music);
                    cmbMusic.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
                }

                //reset dirty state
                _mapHasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show($"Map did not load successfully: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ClearMapMakerSelection();
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

        private bool TryGetWorldMapPlayerObjectMapping(out GameObjectMapping player)
        {
            player = null;
            foreach (var control in pnlCanvas.Controls)
            {
                if (control is GameObjectMapping)
                {
                    var mapping = (GameObjectMapping)control;
                    if (mapping.GameObject is WorldMapPlayer)
                    {
                        player = mapping;
                        return true;
                    }
                }
            }

            return false;
        }

        private void WorldMapEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_mapHasUnsavedChanges
          && MessageBox.Show("This map has unsaved changes. Closing the map maker will cause you to lose these changes. Continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                this.UnsubscribeToEventStoreEvents();
            }
        }

        private void btnAdvancedTools_Click(object sender, EventArgs e)
        {
            if (!_advancedToolsOpen)
            {
                this.ClearMapMakerSelection();
                _advancedToolsForm = new AdvancedToolsForm(_mapMakerObjects);
                _advancedToolsForm.FormClosing += _advancedToolsForm_FormClosing;
                _advancedToolsForm.Show();
                _advancedToolsOpen = true;
            }
        }

        private void _advancedToolsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _advancedToolsForm.FormClosing -= _advancedToolsForm_FormClosing;
            _advancedToolsOpen = false;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            _showSaveMessage = false;
            if (!TrySaveMap())
            {
                MessageBox.Show($"Map '{txtMapName.Text}' did not save successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LevelCompleteObjectives.ClearAll();
            _mapHasUnsavedChanges = false;
            string directory = MapUtility.GetSavedMapsPath(MainMenuConstants.OPTION_LABEL_WORLD_MODE);
            string mapFile = Path.Combine(directory, txtMapName.Text + ".txt");
            var mapData = MapUtility.LoadMapData(mapFile);
            _worldMapData = MapUtility.LoadWorldMapObjectives(txtMapName.Text);

            bool quitGame = false;

            do
            {
                using (WorldMapPlayerForm gameForm = new WorldMapPlayerForm(mapData, true, _worldMapData))
                {
                    gameForm.ShowDialog();
                    dialogMapLoader.FileName = mapFile;
                    if (gameForm.MenuDecision == null ||
                        gameForm.MenuDecision == WorldMapMenuOptionDecision.QUIT)
                        quitGame = true;
                }
            } while (!quitGame);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (_mapHasUnsavedChanges
           && MessageBox.Show("This map has unsaved changes, and this action will override those changes. Continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            //clear events for existing items
            var existingItems = pnlCanvas.Controls.OfType<GameObjectMapping>();
            if (existingItems.Any())
            {
                foreach (var item in existingItems)
                {
                    UnRegisterEventsForGameObjectMapping(item);
                }
            }
            //clear out the canvas
            pnlCanvas.Controls.Clear();

            //set map name default
            txtMapName.Text = "<New Map>";

            //clear mapMaker Objects
            this.ClearMapMakerSelection();
            this.ClearSelectedMapItem();
            _mapMakerObjects.Clear();
        }

        private void btnWorldMapObjectives_Click(object sender, EventArgs e)
        {
            if (!_levelObjectivesFormOpen)
            {
                var objects = _mapMakerObjects.Select(g => g.GameObject).ToList();
                var levels = objects.OfType<IWorldMapLevel>()?.ToList() ??
                     new List<IWorldMapLevel>();
                var activateables = objects.OfType<IActivateable>()?.ToList();
                _worldMapData = MapUtility.LoadWorldMapObjectives(txtMapName.Text);
                _levelObjectivesForm = new LevelObjectivesForm(txtMapName.Text, levels, activateables, _worldMapData);
                _levelObjectivesForm.FormClosing += LevelObjectivesForm_FormClosing;
                _levelObjectivesForm.Show();
                _levelObjectivesFormOpen = true;
            }
        }

        private void LevelObjectivesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _levelObjectivesForm.FormClosing -= LevelObjectivesForm_FormClosing;
            _levelObjectivesFormOpen = false;
        }
    }
}