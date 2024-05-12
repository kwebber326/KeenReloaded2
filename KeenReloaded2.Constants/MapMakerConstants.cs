using System;
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
        }
    }
}
