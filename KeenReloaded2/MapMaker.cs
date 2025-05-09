﻿using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.ControlEventArgs.EventStoreData;
using KeenReloaded2.Entities;
using KeenReloaded2.Entities.DataStructures;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
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
        private Func<GameObjectMapping, GameObjectMapping, int> _comparatorFunction = (o1, o2) =>
        {
            int value1 = o1?.GameObject?.ZIndex ?? -10000;
            int value2 = o2?.GameObject?.ZIndex ?? -10000;

            if (value1 > value2) return 1;
            if (value1 < value2) return -1;
            return 0;
        };
        private OrderedList<GameObjectMapping> _mapMakerObjects;
        private GameObjectMapping _selectedGameObjectMapping;
        private GameObjectMapping _cursorItem;
        private Timer _cursorUpdateTimer = new Timer();
        private SmartPlacer _smartPlacer = new SmartPlacer();
        private bool _mouseInCanvas;
        private bool _useSmartPlacer = false;
        private bool _mapHasUnsavedChanges = false;
        private string _lastFilePath;
        private List<PointMarkerControl> _pathWayPoints = new List<PointMarkerControl>();

        public MapMaker()
        {
            InitializeComponent();
            _mapMakerObjects = new OrderedList<GameObjectMapping>(_comparatorFunction);
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
            mapMakerObjectPropertyListControl1.SetObjectBank(_mapMakerObjects);
            mapMakerObjectPropertyListControl1.PlaceObjectClicked += MapMakerObjectPropertyListControl1_UpdateObjectClicked;
            _cursorUpdateTimer.Interval = 10;
            _cursorUpdateTimer.Tick += _cursorUpdateTimer_Tick;

            SubscribeToEventStoreEvents();
        }

        private void SubscribeToEventStoreEvents()
        {
            EventStore<ActivatorSelectionChangedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_CHANGED, ActivatorSelection_Changed);
            EventStore<ActivatorSelectionCompletedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_COMPLETE, ActivatorSelection_Complete);
            EventStore<DoorSelectionChangedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_DOOR_SELECTION_CHANGED, DoorSelection_Changed);
            EventStore<DoorSelectionChangedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_DOOR_SELECTION_COMPLETE, DoorSelection_Complete);
            EventStore<DoorSelectionChangedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_NODE_SELECTION_CHANGED, NodeSelection_Changed);
            EventStore<DoorSelectionChangedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_NODE_SELECTION_COMPLETE, NodeSelection_Complete);

            EventStore<PointListChangedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_POINTS_LIST_CHANGED, PointList_Changed);
            EventStore<IndexedPoint>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_LOCATION_CHANGED, SinglePointLocation_Changed);
            EventStore<PointListChangedEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_POINTS_LIST_FINALIZED, PointList_Finalized);
            EventStore<int>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_SELECTED_INDEX_CHANGED, PathwayForm_SelectedIndex_Changed);
            //Advanced Tools Events
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, AdvancedTools_SelectionChanged);
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW, AdvancedTools_ActionPreview);
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT, AdvancedTools_ActionCommit);
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_UNDO, AdvancedTools_ActionCancel);
            EventStore<AdvancedToolsEventArgs>.Subscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_CANCEL, AdvancedTools_ActionCancel);
        }

        private void UnsubscribeToEventStoreEvents()
        {
            EventStore<ActivatorSelectionChangedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_CHANGED, ActivatorSelection_Changed);
            EventStore<ActivatorSelectionCompletedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ACTIVATOR_SELECTION_COMPLETE, ActivatorSelection_Complete);
            EventStore<DoorSelectionChangedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_DOOR_SELECTION_CHANGED, DoorSelection_Changed);
            EventStore<DoorSelectionChangedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_DOOR_SELECTION_COMPLETE, DoorSelection_Complete);
            EventStore<DoorSelectionChangedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_NODE_SELECTION_CHANGED, NodeSelection_Changed);
            EventStore<DoorSelectionChangedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_NODE_SELECTION_COMPLETE, NodeSelection_Complete);

            EventStore<PointListChangedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_POINTS_LIST_CHANGED, PointList_Changed);
            EventStore<IndexedPoint>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_LOCATION_CHANGED, SinglePointLocation_Changed);
            EventStore<PointListChangedEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_POINTS_LIST_FINALIZED, PointList_Finalized);
            EventStore<int>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_SELECTED_INDEX_CHANGED, PathwayForm_SelectedIndex_Changed);
            //Advanced Tools Events
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_SELECTION_CHANGED, AdvancedTools_SelectionChanged);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_PREVIEW, AdvancedTools_ActionPreview);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_COMMIT, AdvancedTools_ActionCommit);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_UNDO, AdvancedTools_ActionCancel);
            EventStore<AdvancedToolsEventArgs>.UnSubscribe(MapMakerConstants.EventStoreEventNames.EVENT_ADVANCED_TOOLS_ACTION_CANCEL, AdvancedTools_ActionCancel);
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
            for (int i = 500; i <= 20000; i += 100)
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
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_CTF_ITEMS);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_SHIELD);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_INTERACTIVE_TILES);
            cmbCategory.Items.Add(MapMakerConstants.Categories.OBJECT_CATEGORY_MISCELLANEOUS);

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
                Biomes.EpisodeToBiomeMapping.TryGetValue(cmbEpisode.SelectedItem?.ToString(), out List<string> biomes))
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
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_GEMS
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_CTF_ITEMS
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_SHIELD
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_MISCELLANEOUS)
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
            if (obj.ObjectType.GetInterface(nameof(IActivateable)) != null)
            {
                var property = obj.ConstructorParameters.FirstOrDefault(p => p.PropertyName == GeneralGameConstants.ACTIVATION_ID_PROPERTY_NAME);
                if (property != null)
                {
                    property.Value = Guid.NewGuid();
                }
            }
            else if (obj.ObjectType == typeof(Door) || obj.ObjectType == typeof(Keen4OracleDoor))
            {
                var property = obj.ConstructorParameters.FirstOrDefault(p => p.PropertyName == GeneralGameConstants.DOOR_ID_PROPERTY_NAME);
                if (property != null)
                {
                    var doors = _mapMakerObjects.Select(d => d.GameObject).OfType<Door>();
                    if (doors.Any())
                    {
                        var maxDoorId = doors.Select(d => d.Id).Max();
                        property.Value = maxDoorId + 1;
                    }
                }
            }
            else if (obj.ObjectType == typeof(EnemyTransporter))
            {
                var property = obj.ConstructorParameters.FirstOrDefault(p => p.PropertyName == GeneralGameConstants.NODE_ID_PROPERTY_NAME);
                if (property != null)
                {
                    var transporters = _mapMakerObjects.Select(d => d.GameObject).OfType<EnemyTransporter>();
                    if (transporters.Any())
                    {
                        var maxNodeId = transporters.Select(d => d.Id).Max();
                        property.Value = maxNodeId + 1;
                    }
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
            if (pnlMapCanvas.Controls.Contains(_smartPlacer))
                RemoveSmartPlacerFromCanvas();
        }

        private bool ValidateMapObjects()
        {
            MapMakerData data = new MapMakerData()
            {
                MapData = _mapMakerObjects,
                MapName = txtMapName.Text,
                GameMode = cmbGameMode.Text,
                MapPath = dialogMapLoader.FileName,
                MapSize = this.GetMapSize()
            };
            bool isValid = MapUtility.Validation.ValidateAllMapObjects(data, out List<string> errorMessages);
            if (!isValid)
            {
                string errorMessageText = string.Join("\n", errorMessages);
                MessageBox.Show(errorMessageText, "Invalid Map Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return isValid;
        }

        private void ValidateMap()
        {

            MapMakerData mapMakerData = new MapMakerData()
            {
                MapData = _mapMakerObjects,
                MapName = txtMapName.Text,
                MapPath = dialogMapLoader.FileName,
                GameMode = cmbGameMode.Text,
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

        private static void ClearDoorSelection(IEnumerable<GameObjectMapping> doors)
        {
            foreach (var door in doors)
            {
                door.BackColor = Color.Transparent;
                door.BorderStyle = BorderStyle.None;
            }
        }

        #endregion

        #region event handlers
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
                        pnlMapCanvas.Controls.Add(item);
                        item.BringToFront();
                    }
                    Rectangle offsetArea = new Rectangle(item.Location.X + pnlMapCanvas.AutoScrollPosition.X,
                      item.Location.Y + pnlMapCanvas.AutoScrollPosition.Y,
                      item.GameObject.Image.Width, item.GameObject.Image.Height);
                    item.Location = offsetArea.Location;
                }
            }
            else if (action == AdvancedToolsActions.DELETE)
            {
                foreach (var item in changedData)
                {
                    _mapMakerObjects.Remove(item);
                    pnlMapCanvas.Controls.Remove(item);
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
                    pnlMapCanvas.Controls.Remove(item);
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
                    pnlMapCanvas.Controls.Add(item);
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

        private void NodeSelection_Changed(object sender, ControlEventArgs<DoorSelectionChangedEventArgs> e)
        {
            var newNode = e?.Data?.NewDoor;
            var nodes = _mapMakerObjects.Where(m => m.GameObject is EnemyTransporter);
            ClearDoorSelection(nodes);
            if (newNode != null)
            {
                var matchingDoor = nodes.FirstOrDefault(d => d.GameObject == newNode);
                if (matchingDoor != null)
                {
                    matchingDoor.BorderStyle = BorderStyle.Fixed3D;
                    matchingDoor.BackColor = Color.Red;
                    pnlMapCanvas.ScrollControlIntoView(matchingDoor);
                }
            }
        }

        private void DoorSelection_Changed(object sender, ControlEventArgs.ControlEventArgs<DoorSelectionChangedEventArgs> e)
        {
            var newDoor = e?.Data?.NewDoor;
            var doors = _mapMakerObjects.Where(m => m.GameObject is Door);
            ClearDoorSelection(doors);
            if (newDoor != null)
            {
                var matchingDoor = doors.FirstOrDefault(d => d.GameObject == newDoor);
                if (matchingDoor != null)
                {
                    matchingDoor.BorderStyle = BorderStyle.Fixed3D;
                    matchingDoor.BackColor = Color.Red;
                    pnlMapCanvas.ScrollControlIntoView(matchingDoor);
                }
            }
        }

        private void PointList_Finalized(object sender, ControlEventArgs<PointListChangedEventArgs> e)
        {
            ClearPathwayPointControls();
        }

        private void SinglePointLocation_Changed(object sender, ControlEventArgs<IndexedPoint> e)
        {
            if (e?.Data == null)
                return;

            int index = e.Data.Index - 1;
            var ctrl = _pathWayPoints[index];
            ctrl.Location = GetOffsetPointForCanvas(e.Data.Location);
            pnlMapCanvas.ScrollControlIntoView(ctrl);
        }

        private void PathwayForm_SelectedIndex_Changed(object sender, ControlEventArgs<int> e)
        {
            if (e == null)
                return;

            int index = e.Data - 1;
            if (index >= _pathWayPoints.Count)
            {
                return;
            }
            pnlMapCanvas.ScrollControlIntoView(_pathWayPoints[index]);
        }

        private void PointList_Changed(object sender, ControlEventArgs<PointListChangedEventArgs> e)
        {
            ClearPathwayPointControls();
            List<Point> points = e?.Data?.NewPoints;
            if (points == null || !points.Any())
                return;

            for (int i = 0; i < points.Count; i++)
            {
                int index = i + 1;
                PointMarkerControl ctrl = new PointMarkerControl(index);
                ctrl.Location = GetOffsetPointForCanvas(points[i]);
                _pathWayPoints.Add(ctrl);
                pnlMapCanvas.Controls.Add(ctrl);
                ctrl.BringToFront();
            }
        }

        private Point GetOffsetPointForCanvas(Point point)
        {
            return new Point(point.X + pnlMapCanvas.AutoScrollPosition.X, point.Y + pnlMapCanvas.AutoScrollPosition.Y);
        }

        private Point GetReverseOffsetPointForCanvas(Point point)
        {
            return new Point(point.X - pnlMapCanvas.AutoScrollPosition.X, point.Y - pnlMapCanvas.AutoScrollPosition.Y);
        }

        private void ClearPathwayPointControls()
        {
            if (_pathWayPoints.Any())
            {
                foreach (var marker in _pathWayPoints)
                {
                    pnlMapCanvas.Controls.Remove(marker);
                }
            }
            _pathWayPoints.Clear();
        }

        private void NodeSelection_Complete(object sender, ControlEventArgs<DoorSelectionChangedEventArgs> e)
        {
            var nodes = _mapMakerObjects.Where(m => m.GameObject is EnemyTransporter);
            ClearDoorSelection(nodes);
        }

        private void DoorSelection_Complete(object sender, ControlEventArgs<DoorSelectionChangedEventArgs> e)
        {
            var doors = _mapMakerObjects.Where(m => m.GameObject is Door);
            ClearDoorSelection(doors);
        }

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
                if (!(placedItem is MapEdgeTile) && !MapUtility.Validation.ValidateObjectPlacement(objectArea, mapArea))
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
                gameObjectMapping.Location = new Point(placedItem.Location.X + pnlMapCanvas.AutoScrollPosition.X,
                    placedItem.Location.Y + pnlMapCanvas.AutoScrollPosition.Y);
                gameObjectMapping.SizeMode = PictureBoxSizeMode.AutoSize;
                gameObjectMapping.Image = placedItem.Image;
                // register events
                RegisterEventsForGameObjectMapping(gameObjectMapping);

                //add to collections
                _mapMakerObjects.InsertAscending(gameObjectMapping);
                pnlMapCanvas.Controls.Add(gameObjectMapping);

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
                PnlMapCanvas_Click(this, EventArgs.Empty);
            }
        }

        private void MapMaker_Load(object sender, EventArgs e)
        {
            InitializeMapMaker();
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
            catch (Exception ex)
            {
                GenerateObjectConstructionErrorMessage();
            }
        }

        private static void GenerateObjectConstructionErrorMessage()
        {
            string expectedDirectory = FileIOUtility.GetResourcePathForMainProject();
            MessageBox.Show($"Unable to construct container object. Ensure the associated image is present in the following directory:\n{expectedDirectory}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        _mapHasUnsavedChanges = true;
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
                    if (!txtMapName.Focused)
                        ClearFocus();
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
                                _selectedGameObjectMapping.Location = GetReverseOffsetPointForCanvas(new Point(areaRect.Location.X + 1, areaRect.Location.Y));
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
                MapPath = dialogMapLoader.FileName,
                GameMode = cmbGameMode.Text,
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
                string path = dialogMapLoader.FileName ??
                    Path.Combine(MapUtility.GetSavedMapsPath(cmbGameMode.Text), txtMapName.Text);
                _lastFilePath = path;
                var mapMakerData = MapUtility.LoadMapData(path);
                _mapMakerObjects = OrderedList<GameObjectMapping>.FromEnumerable(mapMakerData.MapData, _comparatorFunction, true);
                mapMakerObjectPropertyListControl1.SetObjectBank(_mapMakerObjects);

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

                this.ValidateMap();
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
            try
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
                    _mapMakerObjects.InsertAscending(_cursorItem);
                    this.Controls.Remove(_cursorItem);

                    Rectangle offsetArea = new Rectangle(area.X - pnlMapCanvas.AutoScrollPosition.X,
                        area.Y - pnlMapCanvas.AutoScrollPosition.Y,
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

        private void CmbWidth_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbHeight_SelectedIndexChanged(object sender, EventArgs e)
        {

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
            var mapSize = this.GetMapSize();
            MapMakerData data = new MapMakerData()
            {
                MapData = _mapMakerObjects,
                MapName = txtMapName.Text,
                MapPath = dialogMapLoader.FileName,
                GameMode = cmbGameMode.Text,
                MapSize = mapSize
            };

            if (!this.ValidateMapObjects())
            {
                return;
            }

            if (!MapUtility.SaveMap(txtMapName.Text, cmbGameMode.SelectedItem?.ToString(), mapSize, _mapMakerObjects))
            {
                MessageBox.Show($"Map '{txtMapName.Text}' did not save successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _mapHasUnsavedChanges = false;
            string directory = MapUtility.GetSavedMapsPath(cmbGameMode.Text);
            string mapFile = Path.Combine(directory, txtMapName.Text + ".txt");
            var mapData = MapUtility.LoadMapData(mapFile);
            using (Form1 gameForm = new Form1(cmbGameMode.Text, mapData, true))
            {
                gameForm.ShowDialog();
                dialogMapLoader.FileName = mapFile;
            }
            // DialogMapLoader_FileOk(this, null);
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

            //clear mapMaker Objects
            this.ClearMapMakerSelection();
            this.ClearSelectedMapItem();
            _mapMakerObjects.Clear();
        }

        private void MapMaker_FormClosing(object sender, FormClosingEventArgs e)
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

        #endregion

        private void BtnAdvancedTools_Click(object sender, EventArgs e)
        {
            this.ClearMapMakerSelection();
            AdvancedToolsForm advancedToolsForm = new AdvancedToolsForm(_mapMakerObjects);
            var dialogResult = advancedToolsForm.ShowDialog();
        }
    }
}