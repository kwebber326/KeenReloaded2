﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Constants
{
    public static class MapMakerConstants
    {
        public const int DEFAULT_MAP_WIDTH = 1500;
        public const int DEFAULT_MAP_HEIGHT = 1500;

        public const string MAP_MAKER_FOLDER = "MapMakerObjects";
        public const string SAVED_MAPS_FOLDER = "SavedMaps";
        public const string NORMAL_MAPS_FOLDER = "NormalModeMaps";
        public const string ZOMBIE_MAPS_FOLDER = "ZombieModeMaps";
        public const string KING_OF_THE_HILL_FOLDER = "KingOfTheHillMaps";
        public const string CAPTURE_THE_FLAG_MAPS_FOLDER = "CaptureTheFlagMaps";
        public const string MAP_MAKER_PROPERTY_SEPARATOR = "|";
        public const string MAP_MAKER_ARRAY_START = "[";
        public const string MAP_MAKER_ARRAY_END = "]";
        public const string MAP_MAKER_ELEMENT_SEPARATOR = ",";
        public static class Categories
        {
            public const string OBJECT_CATEGORY_TILES = "Tiles";
            public const string OBJECT_CATEGORY_ENEMIES = "Enemies";
            public const string OBJECT_CATEGORY_POINTS = "Point Items";
            public const string OBJECT_CATEGORY_LIVES = "Extra Lives";
            public const string OBJECT_CATEGORY_WEAPONS = "Weapons";
            public const string OBJECT_CATEGORY_GEMS = "Gems";
            public const string OBJECT_CATEGORY_CONSTRUCTS = "Constructs";
            public const string OBJECT_CATEGORY_PLAYER = "Player";
            public const string OBJECT_CATEGORY_HAZARDS = "Hazards";
            public const string OBJECT_CATEGORY_BACKGROUNDS = "Backgrounds";
            public const string OBJECT_CATEGORY_FOREGROUNDS = "Foregrounds";
            public const string OBJECT_CATEGORY_CTF_ITEMS = "CTF Items";
            public const string OBJECT_CATEGORY_SHIELD = "Shield";
            public const string OBJECT_CATEGORY_INTERACTIVE_TILES = "Interactive Tiles";
            public const string OBJECT_CATEGORY_ANIMATED_BACKGROUNDS = "Animated Backgrounds";
            public const string OBJECT_CATEGORY_ANIMATED_FOREGROUNDS = "Animated Foregrounds";
            public const string OBJECT_CATEGORY_MISCELLANEOUS = "Miscellaneous";
        }

        public static class EventStoreEventNames
        {
            public const string EVENT_ACTIVATOR_SELECTION_CHANGED = "activatorSelectionChanged";
            public const string EVENT_ACTIVATOR_SELECTION_COMPLETE = "activatorSelectionComplete";
            public const string EVENT_DOOR_SELECTION_CHANGED = "doorSelectionChanged";
            public const string EVENT_DOOR_SELECTION_COMPLETE = "doorSelectionComplete";
            public const string EVENT_NODE_SELECTION_CHANGED = "nodeSelectionChanged";
            public const string EVENT_NODE_SELECTION_COMPLETE = "nodeSelectionComplete";
            public const string EVENT_POINTS_LIST_CHANGED = "pointsListChanged";
            public const string EVENT_POINTS_LIST_FINALIZED = "pointsListFinalized";
            public const string EVENT_SELECTED_INDEX_CHANGED = "selectedIndexChanged";
            public const string EVENT_LOCATION_CHANGED = "locationChanged";
            public const string EVENT_SELECTED_SONG_CHANGED = "selectedSongChanged";
            //advanceds tools event names
            public const string EVENT_ADVANCED_TOOLS_SELECTION_CHANGED = "[Advanced Tools]selectionChanged";
            public const string EVENT_ADVANCED_TOOLS_SELECTED_ACTION_CHANGED = "[Advanced Tools]selectedActionChanged";
            public const string EVENT_ADVANCED_TOOLS_ACTION_COMMIT = "[Advanced Tools]actionCommit";
            public const string EVENT_ADVANCED_TOOLS_ACTION_PREVIEW = "[Advanced Tools]actionPreview";
            public const string EVENT_ADVANCED_TOOLS_ACTION_UNDO = "[Advanced Tools]actionUndo";
            public const string EVENT_ADVANCED_TOOLS_ACTION_CANCEL = "[Advanced Tools]actionCancel";
            //music and sounds
            public const string EVENT_SOUND_PLAY = "playSound";
            //keen events
            public const string KEEN_DISAPPEAR_DEATH = "keenDisappearDeath";
            public const string KEEN_LEVEL_COMPLETE = "levelComplete";
        }
    }
}
