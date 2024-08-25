using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.GameEntities.Hazards;
using KeenReloaded2.Framework.GameEntities.Items;
using KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEntities.Weapons;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Interfaces;

namespace KeenReloaded2.Entities.ReferenceData
{
    public static class ImageToObjectCreationFactory
    {
        #region tile type dictionary
        private static Dictionary<Type, List<string>> _tileTypeDictionary = new Dictionary<Type, List<string>>()
            {
                {
                    typeof(CeilingTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_cave_wall_bottom1),
                        nameof(Properties.Resources.keen4_cave_wall_bottom2),
                        nameof(Properties.Resources.keen4_cave_wall_bottom3),
                        nameof(Properties.Resources.keen4_cave_wall_bottom4),
                         nameof(Properties.Resources.keen4_cave_wall_bottom5),
                        nameof(Properties.Resources.keen4_cave_wall_bottom6),
                        nameof(Properties.Resources.keen4_cave_wall_bottom7),
                        nameof(Properties.Resources.keen4_forest_wall_bottom1),
                        nameof(Properties.Resources.keen4_forest_wall_bottom2),
                        nameof(Properties.Resources.keen4_pyramid_wall_bottom1),
                        nameof(Properties.Resources.keen4_pyramid_wall_bottom2),
                        nameof(Properties.Resources.keen4_mirage_wall_bottom1),
                        nameof(Properties.Resources.keen4_mirage_wall_bottom2),
                        nameof(Properties.Resources.keen4_mirage_wall_bottom3),
                        nameof(Properties.Resources.keen4_mirage_wall_bottom4),
                        nameof(Properties.Resources.keen5_wall_black_bottom),
                        nameof(Properties.Resources.keen5_ceiling_green),
                        nameof(Properties.Resources.keen5_ceiling_red),
                        nameof(Properties.Resources.keen6_dome_ceiling1),
                        nameof(Properties.Resources.keen6_dome_ceiling2),
                        nameof(Properties.Resources.keen6_dome_ceiling3),
                        nameof(Properties.Resources.keen6_forest_ceiling1),
                        nameof(Properties.Resources.keen6_forest_ceiling2),
                        nameof(Properties.Resources.keen6_industrial_ceiling),
                    }
                },
                {
                    typeof(MiddleWallTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_wall_middle),
                        nameof(Properties.Resources.keen4_cave_wall_middle),
                        nameof(Properties.Resources.keen4_mirage_wall_middle),
                        nameof(Properties.Resources.keen4_pyramid_wall_middle),
                        nameof(Properties.Resources.keen5_wall_black_middle),
                        nameof(Properties.Resources.keen5_wall_red_middle),
                        nameof(Properties.Resources.keen5_wall_green_middle),
                        nameof(Properties.Resources.keen6_industrial_wall_middle),
                        nameof(Properties.Resources.keen6_forest_wall_middle1),
                        nameof(Properties.Resources.keen6_forest_wall_middle2),
                        nameof(Properties.Resources.keen6_dome_wall_middle1),
                        nameof(Properties.Resources.keen6_dome_wall_middle2),
                    }
                },
                {
                    typeof(LeftEdgeWallTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_wall_left_edge),
                        nameof(Properties.Resources.keen4_cave_wall_edge_left),
                        nameof(Properties.Resources.keen4_mirage_wall_left_edge),
                        nameof(Properties.Resources.keen4_pyramid_wall_edge_left),
                        nameof(Properties.Resources.keen5_wall_black_edge_left),
                        nameof(Properties.Resources.keen5_wall_red_edge_left),
                        nameof(Properties.Resources.keen5_wall_green_edge_left),
                        nameof(Properties.Resources.keen6_industrial_wall_edge_left),
                        nameof(Properties.Resources.keen6_forest_wall_edge_left),
                        nameof(Properties.Resources.keen6_dome_wall_edge_left),
                    }
                },
                {
                    typeof(RightEdgeWallTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_wall_right_edge),
                        nameof(Properties.Resources.keen4_cave_wall_edge_right),
                        nameof(Properties.Resources.keen4_mirage_wall_right_edge),
                        nameof(Properties.Resources.keen4_pyramid_wall_edge_right),
                        nameof(Properties.Resources.keen5_wall_black_edge_right),
                        nameof(Properties.Resources.keen5_wall_red_edge_right),
                        nameof(Properties.Resources.keen5_wall_green_edge_right),
                        nameof(Properties.Resources.keen6_industrial_wall_edge_right),
                        nameof(Properties.Resources.keen6_forest_wall_edge_right),
                        nameof(Properties.Resources.keen6_dome_wall_edge_right),
                    }
                },
                {
                    typeof(MiddleFloorTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_floor_middle),
                        nameof(Properties.Resources.keen4_cave_floor_middle),
                        nameof(Properties.Resources.keen4_mirage_floor_middle),
                        nameof(Properties.Resources.keen4_pyramid_floor_middle),
                        nameof(Properties.Resources.keen5_floor_black_middle),
                        nameof(Properties.Resources.keen5_floor_red_middle),
                        nameof(Properties.Resources.keen5_floor_green_middle),
                        nameof(Properties.Resources.keen6_industrial_floor_middle),
                        nameof(Properties.Resources.keen6_forest_floor_middle),
                        nameof(Properties.Resources.keen6_dome_floor_middle),
                    }
                },
                {
                    typeof(LeftEdgeFloorTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_floor_edge_left),
                        nameof(Properties.Resources.keen4_cave_air_floor_edge_left),
                        nameof(Properties.Resources.keen4_mirage_floor_edge_left),
                        nameof(Properties.Resources.keen4_pyramid_floor_edge_left),
                        nameof(Properties.Resources.keen5_floor_black_floor_edge_left),
                        nameof(Properties.Resources.keen5_floor_red_edge_left),
                        nameof(Properties.Resources.keen5_floor_green_edge_left),
                        nameof(Properties.Resources.keen6_industrial_floor_edge_left),
                        nameof(Properties.Resources.keen6_forest_floor_edge_left),
                        nameof(Properties.Resources.keen6_dome_floor_edge_left),
                    }
                },
                {
                    typeof(RightEdgeFloorTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_floor_edge_right),
                        nameof(Properties.Resources.keen4_cave_air_floor_edge_right),
                        nameof(Properties.Resources.keen4_mirage_floor_edge_right),
                        nameof(Properties.Resources.keen4_pyramid_floor_edge_right),
                        nameof(Properties.Resources.keen5_floor_black_floor_edge_right),
                        nameof(Properties.Resources.keen5_floor_red_edge_right),
                        nameof(Properties.Resources.keen5_floor_green_edge_right),
                        nameof(Properties.Resources.keen6_industrial_floor_edge_right),
                        nameof(Properties.Resources.keen6_forest_floor_edge_right),
                        nameof(Properties.Resources.keen6_dome_floor_edge_right),
                    }
                },
                {
                    typeof(MiddlePlatformTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_platform_middle),
                        nameof(Properties.Resources.keen4_cave_platform_middle),
                        nameof(Properties.Resources.keen4_mirage_platform_middle),
                        nameof(Properties.Resources.keen5_platform_blue_middle),
                        nameof(Properties.Resources.keen5_platform_red_middle),
                        nameof(Properties.Resources.keen5_platform_green_middle),
                        nameof(Properties.Resources.keen6_industrial_platform_middle),
                        nameof(Properties.Resources.keen6_dome_platform_middle),
                    }
                },
                {
                    typeof(LeftEdgePlatformTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_platform_left_edge),
                        nameof(Properties.Resources.keen4_cave_platform_left_edge),
                        nameof(Properties.Resources.keen4_mirage_platform_left_edge),
                        nameof(Properties.Resources.keen5_platform_blue_edge_left),
                        nameof(Properties.Resources.keen5_platform_red_edge_left),
                        nameof(Properties.Resources.keen5_platform_green_edge_left),
                        nameof(Properties.Resources.keen6_industrial_platform_left),
                        nameof(Properties.Resources.keen6_dome_platform_edge_left),
                    }
                },
                {
                    typeof(RightEdgePlatformTile),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_platform_right_edge),
                        nameof(Properties.Resources.keen4_cave_platform_right_edge),
                        nameof(Properties.Resources.keen4_mirage_platform_right_edge),
                        nameof(Properties.Resources.keen5_platform_blue_edge_right),
                        nameof(Properties.Resources.keen5_platform_red_edge_right),
                        nameof(Properties.Resources.keen5_platform_green_edge_right),
                        nameof(Properties.Resources.keen6_industrial_platform_right),
                        nameof(Properties.Resources.keen6_dome_platform_edge_right),
                    }
                },
                {
                    typeof(SinglePlatform),
                    new List<string>()
                    {
                        nameof(Properties.Resources.keen4_forest_platform_single),
                        nameof(Properties.Resources.keen4_mirage_platform_single),
                        nameof(Properties.Resources.keen5_single_platform_blue),
                        nameof(Properties.Resources.keen5_single_platform_red),
                        nameof(Properties.Resources.keen6_dome_platform_single),
                        nameof(Properties.Resources.keen6_industrial_single_masked_platform),
                    }
                },
            };
        #endregion

        #region point type dictionary
        private static Dictionary<string, PointItemType> _pointImageTypeDict = new Dictionary<string, PointItemType>()
        {
            { nameof(Properties.Resources.keen4_shikadi_soda1), PointItemType.KEEN4_SHIKADI_SODA },
            { nameof(Properties.Resources.keen4_three_tooth_gum1), PointItemType.KEEN4_THREE_TOOTH_GUM },
            { nameof(Properties.Resources.keen4_candy_bar1), PointItemType.KEEN4_SHIKKERS_CANDY_BAR },
            { nameof(Properties.Resources.keen4_jawbreaker1), PointItemType.KEEN4_JAWBREAKER },
            { nameof(Properties.Resources.keen4_doughnut1), PointItemType.KEEN4_DOUGHNUT },
            { nameof(Properties.Resources.keen4_icecream_cone1), PointItemType.KEEN4_ICECREAM_CONE },
            { nameof(Properties.Resources.keen5_shikadi_gum1), PointItemType.KEEN5_SHIKADI_GUM },
            { nameof(Properties.Resources.keen5_marshmallow1), PointItemType.KEEN5_MARSHMALLOW },
            { nameof(Properties.Resources.keen5_chocolate_milk1), PointItemType.KEEN5_CHOCOLATE_MILK },
            { nameof(Properties.Resources.keen5_tart_stix1), PointItemType.KEEN5_TART_STIX },
            { nameof(Properties.Resources.keen5_sugar_stoopies_cereal1), PointItemType.KEEN5_SUGAR_STOOPIES_CEREAL },
            { nameof(Properties.Resources.keen5_bag_o_sugar1), PointItemType.KEEN5_BAG_O_SUGAR },
            { nameof(Properties.Resources.keen6_bloog_soda1), PointItemType.KEEN6_BLOOG_SODA },
            { nameof(Properties.Resources.keen6_ice_cream_bar1), PointItemType.KEEN6_ICE_CREAM_BAR },
            { nameof(Properties.Resources.keen6_pudding1), PointItemType.KEEN6_PUDDING },
            { nameof(Properties.Resources.keen6_root_beer_float1), PointItemType.KEEN6_ROOT_BEER_FLOAT },
            { nameof(Properties.Resources.keen6_banana_split1), PointItemType.KEEN6_BANANA_SPLIT },
            { nameof(Properties.Resources.keen6_pizza_slice1), PointItemType.KEEN6_PIZZA_SLICE },
        };
        #endregion

        private static Dictionary<string, MapMakerObject> GetBackgroundObjectData()
        {
            Dictionary<string, MapMakerObject> backgroundReferenceData = new Dictionary<string, MapMakerObject>();

            for (int i = 4; i <= 6; i++)
            {
                #region backgrounds
                string path = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_BACKGROUNDS, $"keen{i}", Biomes.BIOME_KEEN5_BLACK);

                string[] imageFiles = Directory.GetFiles(path);
                foreach (var file in imageFiles)
                {
                    try
                    {
                        Image img = Image.FromFile(file);
                        string imageName = FileIOUtility.ExtractFileNameFromPath(file);
                        Type type = typeof(Background);
                        string imagePath = file;

                        var parameters = new List<MapMakerObjectProperty>()
                    {
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                             DisplayName = "Area: ",
                             DataType = typeof(Rectangle),
                             Value = new Rectangle(0, 0, img.Width, img.Height)
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "imagePath",
                             DisplayName = "Image: ",
                             DataType = typeof(string),
                             Value = imageName + ".png",
                             IsSpriteProperty = true,
                             Readonly = true,
                             Hidden = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "stretchImage",
                             DisplayName = "Stretch Image: ",
                             DataType = typeof(bool),
                             Value = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "zIndex",
                             DisplayName = "Z Index: ",
                             DataType = typeof(int),
                             Value = 0
                         }
                    }.ToArray();

                        MapMakerObject obj = new MapMakerObject(type, imagePath, false, parameters);
                        backgroundReferenceData.Add(imageName, obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return new Dictionary<string, MapMakerObject>();
                    }
                }

                #endregion

                #region foregrounds
                path = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_FOREGROUNDS, $"keen{i}", Biomes.BIOME_KEEN5_BLACK);

                imageFiles = Directory.GetFiles(path);
                foreach (var file in imageFiles)
                {
                    try
                    {
                        Image img = Image.FromFile(file);
                        string imageName = FileIOUtility.ExtractFileNameFromPath(file);
                        Type type = typeof(Background);
                        string imagePath = file;

                        var parameters = new List<MapMakerObjectProperty>()
                    {
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                             DisplayName = "Area: ",
                             DataType = typeof(Rectangle),
                             Value = new Rectangle(0, 0, img.Width, img.Height)
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "imagePath",
                             DisplayName = "Image: ",
                             DataType = typeof(string),
                             Value = imageName + ".png",
                             IsSpriteProperty = true,
                             Readonly = true,
                             Hidden = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "stretchImage",
                             DisplayName = "Stretch Image: ",
                             DataType = typeof(bool),
                             Value = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "zIndex",
                             DisplayName = "Z Index: ",
                             DataType = typeof(int),
                             Value = 100
                         }
                    }.ToArray();

                        MapMakerObject obj = new MapMakerObject(type, imagePath, false, parameters);
                        backgroundReferenceData.Add(imageName, obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return new Dictionary<string, MapMakerObject>();
                    }
                }

                #endregion

                #region animated backgrounds
                path = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_ANIMATED_BACKGROUNDS, $"keen{i}", Biomes.BIOME_KEEN5_BLACK);

                imageFiles = Directory.GetFiles(path);
                foreach (var file in imageFiles)
                {
                    try
                    {
                        Image img = Image.FromFile(file);
                        string imageName = FileIOUtility.ExtractFileNameFromPath(file);
                        Type type = typeof(AnimatedBackground);
                        string imagePath = file;
                        string imageSetPath = Path.Combine(path, imageName);
                        string[] imageSet = Directory.GetFiles(imageSetPath)
                           .Select(f => FileIOUtility.ExtractFileNameFromPath(f) + ".png").ToArray();

                        var parameters = new List<MapMakerObjectProperty>()
                    {
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                             DisplayName = "Area: ",
                             DataType = typeof(Rectangle),
                             Value = new Rectangle(0, 0, img.Width, img.Height)
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "imagePath",
                             DisplayName = "Image: ",
                             DataType = typeof(string),
                             Value = imageName + ".png",
                             IsSpriteProperty = true,
                             Readonly = true,
                             Hidden = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "stretchImage",
                             DisplayName = "Stretch Image: ",
                             DataType = typeof(bool),
                             Value = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "zIndex",
                             DisplayName = "Z Index: ",
                             DataType = typeof(int),
                             Value = 0
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "images",
                             DisplayName = "Images: ",
                             Hidden = true,
                             DataType = typeof(string[]),
                             Value = imageSet
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "imageRotationDelayMilliseconds",
                             DisplayName = "Amimation update delay (ms): ",
                             DataType = typeof(int),
                             Value = 200
                         }
                    }.ToArray();

                        MapMakerObject obj = new MapMakerObject(type, imagePath, false, parameters);
                        backgroundReferenceData.Add(imageName, obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return new Dictionary<string, MapMakerObject>();
                    }
                }

                #endregion

                #region animated foregrounds
                path = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_ANIMATED_FOREGROUNDS, $"keen{i}", Biomes.BIOME_KEEN5_BLACK);

                imageFiles = Directory.GetFiles(path);
                foreach (var file in imageFiles)
                {
                    try
                    {
                        Image img = Image.FromFile(file);
                        string imageName = FileIOUtility.ExtractFileNameFromPath(file);
                        Type type = typeof(AnimatedBackground);
                        string imagePath = file;
                        string imageSetPath = Path.Combine(path, imageName);
                        string[] imageSet = Directory.GetFiles(imageSetPath)
                            .Select(f => FileIOUtility.ExtractFileNameFromPath(f) + ".png").ToArray();

                        var parameters = new List<MapMakerObjectProperty>()
                    {
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                             DisplayName = "Area: ",
                             DataType = typeof(Rectangle),
                             Value = new Rectangle(0, 0, img.Width, img.Height)
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "imagePath",
                             DisplayName = "Image: ",
                             DataType = typeof(string),
                             Value = imageName + ".png",
                             IsSpriteProperty = true,
                             Readonly = true,
                             Hidden = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "stretchImage",
                             DisplayName = "Stretch Image: ",
                             DataType = typeof(bool),
                             Value = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "zIndex",
                             DisplayName = "Z Index: ",
                             DataType = typeof(int),
                             Value = 100
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "images",
                             DisplayName = "Images: ",
                             Hidden = true,
                             DataType = typeof(string[]),
                             Value = imageSet
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "imageRotationDelayMilliseconds",
                             DisplayName = "Amimation update delay (ms): ",
                             DataType = typeof(int),
                             Value = 200
                         }
                    }.ToArray();

                        MapMakerObject obj = new MapMakerObject(type, imagePath, false, parameters);
                        backgroundReferenceData.Add(imageName, obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        return new Dictionary<string, MapMakerObject>();
                    }
                }

                #endregion
            }

            #region tiles
            foreach (var item in _tileTypeDictionary)
            {
                var type = item.Key;
                foreach (var imageName in item.Value)
                {
                    string biome = InferBiomeFromImage(imageName);
                    string episodeFolder = biome.Substring(0, biome.LastIndexOf('_')).ToLower();
                    string path = Path.Combine(FileIOUtility.GetResourcePathForMainProject(), imageName + ".png");
                    Image img = Image.FromFile(path);
                    MapMakerObjectProperty[] properties = new MapMakerObjectProperty[]
                    {
                        new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, img.Width, img.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.HITBOX_PROPERTY_NAME,
                            Hidden = true,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, img.Width, img.Height),
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "imageFile",
                            DataType = typeof(string),
                            Hidden = true,
                            Value = path,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                         {
                             PropertyName = "zIndex",
                             DisplayName = "Z Index: ",
                             DataType = typeof(int),
                             Value = 10
                         },
                         new MapMakerObjectProperty()
                         {
                            PropertyName = "biome",
                            DataType = typeof(string),
                            Hidden = true,
                            Value = biome
                         }
                    };
                    MapMakerObject obj = new MapMakerObject(type, path, false, properties);
                    backgroundReferenceData.Add(imageName, obj);
                }
            }
            #endregion

            #region gems

            string gemPath = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_GEMS, $"keen4", Biomes.BIOME_KEEN5_BLACK);
            var gemFiles = Directory.GetFiles(gemPath);

            foreach (var filePath in gemFiles)
            {
                var img = Image.FromFile(filePath);
                string imgName = FileIOUtility.ExtractFileNameFromPath(filePath);

                if (filePath.Contains("placeholder"))
                {
                    MapMakerObjectProperty[] constructorParameters = new MapMakerObjectProperty[]
                   {
                    new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, img.Width, img.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "zIndex",
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "color",
                            DataType = typeof(GemColor),
                            Hidden = true,
                            Value = InferGemColorFromImageName(imgName),
                            PossibleValues = Enum.GetNames(typeof(GemColor)),
                            IsSpriteProperty = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "objectsToActivate",
                            DataType = typeof(IActivateable[]),
                            Value = new IActivateable[] { },
                            DisplayName = "Activation Objects: "
                        },
                   };
                    MapMakerObject obj = new MapMakerObject(typeof(GemPlaceHolder), filePath, false, constructorParameters);

                    backgroundReferenceData.Add(imgName, obj);
                }
                else if (filePath.Contains("key_gate"))
                {
                    MapMakerObjectProperty[] constructorParameters = new MapMakerObjectProperty[]
                    {
                        new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, img.Width, img.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                         new MapMakerObjectProperty()
                        {
                            PropertyName = "imageName",
                            DataType = typeof(string),
                            Hidden = true,
                            Value = filePath,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "zIndex",
                            DataType = typeof(int),
                            Value = 10,
                            DisplayName ="Z Index: "
                        },
                         new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.ACTIVATION_ID_PROPERTY_NAME,
                            DataType = typeof(Guid),
                            Value = new Guid(),
                            DisplayName ="Id: ",
                            Readonly = true
                        },
                    };
                    MapMakerObject obj = new MapMakerObject(typeof(KeyGate), filePath, false, constructorParameters);

                    backgroundReferenceData.Add(imgName, obj);
                }
                else
                {
                    MapMakerObjectProperty[] constructorParameters = new MapMakerObjectProperty[]
                    {
                    new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, img.Width, img.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "imageName",
                            DataType = typeof(string),
                            Hidden = true,
                            Value = imgName,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "color",
                            DataType = typeof(GemColor),
                            Hidden = true,
                            Value = InferGemColorFromImageName(imgName),
                            PossibleValues = Enum.GetNames(typeof(GemColor)),
                            IsSpriteProperty = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "zIndex",
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "hasGravity",
                            DataType = typeof(bool),
                            Value = false,
                            Hidden = true
                        },
                    };
                    MapMakerObject obj = new MapMakerObject(typeof(Gem), filePath, false, constructorParameters);

                    backgroundReferenceData.Add(imgName, obj);
                }
            }

            #endregion

            #region point items 
            string[] pointItemsPaths = new string[]
            {
                GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_POINTS, "Keen4", Biomes.BIOME_KEEN4_CAVE),
                GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_POINTS, "Keen5", Biomes.BIOME_KEEN4_CAVE),
                GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_POINTS, "Keen6", Biomes.BIOME_KEEN4_CAVE)
            };
            foreach (string pointItemPath in pointItemsPaths)
            {
                var pointImageFiles = Directory.GetFiles(pointItemPath);
                foreach (var file in pointImageFiles)
                {
                    string pointImageName = FileIOUtility.ExtractFileNameFromPath(file);

                    if (!_pointImageTypeDict.TryGetValue(pointImageName, out PointItemType pointItemType))
                        continue;

                    Image img = Image.FromFile(file);
                    MapMakerObjectProperty[] objectProperties = new MapMakerObjectProperty[]
                    {
                        new MapMakerObjectProperty()
                        {
                                DisplayName = "Area: ",
                                PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                                DataType = typeof(Rectangle),
                                Value = new Rectangle(0, 0, img.Width, img.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                                PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                                DataType = typeof(SpaceHashGrid),
                                Value = null,
                                Hidden = true,
                                IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                                DisplayName = "Image Name: ",
                                PropertyName = "imageName",
                                DataType = typeof(string),
                                Value = pointImageName,
                                IsSpriteProperty = true,
                                Hidden = true,
                                IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                                PropertyName = "zIndex",
                                DataType = typeof(int),
                                Value = 50,
                                DisplayName ="Z Index: "
                        },
                        new MapMakerObjectProperty()
                        {
                                PropertyName = "type",
                                DataType = typeof(PointItemType),
                                Value = pointItemType,
                                PossibleValues = Enum.GetNames(typeof(PointItemType)),
                                Readonly = true
                        },
                    };
                    MapMakerObject obj = new MapMakerObject(typeof(PointItem), file, false, objectProperties);
                    backgroundReferenceData.Add(pointImageName, obj);
                }
            }



            #endregion

            #region drops and extra lives

            pointItemsPaths = new string[]
            {
                GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_LIVES, "Keen4", Biomes.BIOME_KEEN4_CAVE),
                GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_LIVES, "Keen5", Biomes.BIOME_KEEN4_CAVE),
                GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_LIVES, "Keen6", Biomes.BIOME_KEEN4_CAVE)
            };

            var keen4Files = Directory.GetFiles(pointItemsPaths[0]);
            var keen5Files = Directory.GetFiles(pointItemsPaths[1]);
            var keen6Files = Directory.GetFiles(pointItemsPaths[2]);

            #region keen 4 
            string rainDropPath = keen4Files.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen4_drop1)));
            string rainDropKey = nameof(Properties.Resources.keen4_drop1);
            Image rainDropImage = Properties.Resources.keen4_drop1;
            MapMakerObjectProperty[] rainDropParameters = new MapMakerObjectProperty[]
            {
                 new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, rainDropImage.Width, rainDropImage.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.IMAGE_NAME_PROPERTY_NAME,
                            DataType = typeof(string),
                            Hidden = true,
                            Value = rainDropKey,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.Z_INDEX_PROPERTY_NAME,
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
            };
            MapMakerObject keen4RainDrop = new MapMakerObject(typeof(RainDrop), rainDropPath, false, rainDropParameters);
            backgroundReferenceData.Add(rainDropKey, keen4RainDrop);


            string lifewaterPath = keen4Files.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen4_lifewater_flask1)));
            string lifewaterKey = nameof(Properties.Resources.keen4_lifewater_flask1);
            Image lifewaterImage = Properties.Resources.keen4_lifewater_flask1;
            MapMakerObjectProperty[] lifewaterParameters = new MapMakerObjectProperty[]
            {
                 new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, lifewaterImage.Width, lifewaterImage.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.IMAGE_NAME_PROPERTY_NAME,
                            DataType = typeof(string),
                            Hidden = true,
                            Value = lifewaterKey,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.Z_INDEX_PROPERTY_NAME,
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "type",
                            DataType = typeof(ExtraLifeType),
                            PossibleValues = Enum.GetNames(typeof(ExtraLifeType)),
                            Value = ExtraLifeType.KEEN4_LIFEWATER_FLASK,
                            Readonly = true
                        }
            };
            MapMakerObject keen4LifewaterFlask = new MapMakerObject(typeof(ExtraLife), lifewaterPath, false, lifewaterParameters);
            backgroundReferenceData.Add(lifewaterKey, keen4LifewaterFlask);
            #endregion

            #region keen 5
            string vitalinPath = keen5Files.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen5_vitalin1)));
            string vitalinKey = nameof(Properties.Resources.keen5_vitalin1);
            Image vitalinImage = Properties.Resources.keen5_vitalin1;
            MapMakerObjectProperty[] vitalinParameters = new MapMakerObjectProperty[]
            {
                 new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, vitalinImage.Width, vitalinImage.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.IMAGE_NAME_PROPERTY_NAME,
                            DataType = typeof(string),
                            Hidden = true,
                            Value = vitalinKey,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.Z_INDEX_PROPERTY_NAME,
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
            };
            MapMakerObject keen5Vitalin = new MapMakerObject(typeof(Vitalin), vitalinPath, false, vitalinParameters);
            backgroundReferenceData.Add(vitalinKey, keen5Vitalin);


            string kegPath = keen5Files.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen5_keg_o_vitalin1)));
            string kegKey = nameof(Properties.Resources.keen5_keg_o_vitalin1);
            Image kegImage = Properties.Resources.keen5_keg_o_vitalin1;
            MapMakerObjectProperty[] kegParameters = new MapMakerObjectProperty[]
            {
                 new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, kegImage.Width, kegImage.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.IMAGE_NAME_PROPERTY_NAME,
                            DataType = typeof(string),
                            Hidden = true,
                            Value = kegKey,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.Z_INDEX_PROPERTY_NAME,
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "type",
                            DataType = typeof(ExtraLifeType),
                            PossibleValues = Enum.GetNames(typeof(ExtraLifeType)),
                            Value = ExtraLifeType.KEEN5_KEG_O_VITALIN,
                            Readonly = true
                        }
            };
            MapMakerObject keen5KegOVitalin = new MapMakerObject(typeof(ExtraLife), kegPath, false, kegParameters);
            backgroundReferenceData.Add(kegKey, keen5KegOVitalin);
            #endregion

            #region keen 6
            string vivaFlyingPath = keen6Files.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen6_viva_flying1)));
            string vivaFlyingKey = nameof(Properties.Resources.keen6_viva_flying1);
            Image vivaFlyingImage = Properties.Resources.keen6_viva_flying1;
            MapMakerObjectProperty[] vivaFlyingParameters = new MapMakerObjectProperty[]
            {
                 new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, vitalinImage.Width, vitalinImage.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.IMAGE_NAME_PROPERTY_NAME,
                            DataType = typeof(string),
                            Hidden = true,
                            Value = vivaFlyingKey,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.Z_INDEX_PROPERTY_NAME,
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
                         new MapMakerObjectProperty()
                        {
                            PropertyName = "perch",
                            DataType = typeof(bool),
                            Value = false,
                            Hidden = true
                        },
            };
            MapMakerObject keen6VivaFlying = new MapMakerObject(typeof(Viva), vivaFlyingPath, false, vivaFlyingParameters);
            backgroundReferenceData.Add(vivaFlyingKey, keen6VivaFlying);

            string vivaPerchedPath = keen6Files.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen6_viva_perched1)));
            string vivaPerchedKey = nameof(Properties.Resources.keen6_viva_perched1);
            Image vivaPerchedImage = Properties.Resources.keen6_viva_perched1;
            MapMakerObjectProperty[] vivaPerchedParameters = new MapMakerObjectProperty[]
            {
                 new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, vitalinImage.Width, vitalinImage.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.IMAGE_NAME_PROPERTY_NAME,
                            DataType = typeof(string),
                            Hidden = true,
                            Value = vivaPerchedKey,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.Z_INDEX_PROPERTY_NAME,
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
                         new MapMakerObjectProperty()
                        {
                            PropertyName = "perch",
                            DataType = typeof(bool),
                            Value = true,
                            Hidden = true
                        },
            };
            MapMakerObject keen6VivaPerched = new MapMakerObject(typeof(Viva), vivaPerchedPath, false, vivaPerchedParameters);
            backgroundReferenceData.Add(vivaPerchedKey, keen6VivaPerched);


            string vivaQueenPath = keen6Files.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen6_viva_queen2)));
            string vivaQueenKey = nameof(Properties.Resources.keen6_viva_queen2);
            Image vivaQueenImage = Properties.Resources.keen6_viva_queen2;
            MapMakerObjectProperty[] vivaQueenParameters = new MapMakerObjectProperty[]
            {
                 new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, vivaQueenImage.Width, vivaQueenImage.Height),
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.IMAGE_NAME_PROPERTY_NAME,
                            DataType = typeof(string),
                            Hidden = true,
                            Value = vivaQueenKey,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.Z_INDEX_PROPERTY_NAME,
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "type",
                            DataType = typeof(ExtraLifeType),
                            PossibleValues = Enum.GetNames(typeof(ExtraLifeType)),
                            Value = ExtraLifeType.KEEN6_VIVA_QUEEN,
                            Readonly = true
                        }
            };
            MapMakerObject keen6VivaQueen = new MapMakerObject(typeof(ExtraLife), vivaQueenPath, false, vivaQueenParameters);
            backgroundReferenceData.Add(vivaQueenKey, keen6VivaQueen);
            #endregion

            #endregion

            #region Weapons

            string weaponsPath = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_WEAPONS, "keen4", Biomes.BIOME_KEEN5_BLACK);

            #region neural stunner

            MapMakerObject neuralStunner = GetWeaponObjectFromWeaponsData<NeuralStunnerAmmo>(weaponsPath, nameof(Properties.Resources.neural_stunner1), out string neuralStunnerKey);

            backgroundReferenceData.Add(neuralStunnerKey, neuralStunner);
            #endregion

            #region railgun

            MapMakerObject railgun = GetWeaponObjectFromWeaponsData<RailgunNeuralStunnerAmmo>(weaponsPath, nameof(Properties.Resources.neural_stunner_railgun1), out string railKey);

            backgroundReferenceData.Add(railKey, railgun);
            #endregion

            #region RPG
            MapMakerObject rpg = GetWeaponObjectFromWeaponsData<RPGNeuralStunnerAmmo>(weaponsPath, nameof(Properties.Resources.neural_stunner_rocket_launcher1), out string rpgKey);

            backgroundReferenceData.Add(rpgKey, rpg);
            #endregion

            #region smg

            MapMakerObject smg = GetWeaponObjectFromWeaponsData<SMGNeuralStunnerAmmo>(weaponsPath, nameof(Properties.Resources.neural_stunner_smg_1), out string smgKey);

            backgroundReferenceData.Add(smgKey, smg);

            #endregion

            #region BFG

            MapMakerObject bfg = GetWeaponObjectFromWeaponsData<BFGAmmo>(weaponsPath, nameof(Properties.Resources.BFG1), out string bfgKey);

            backgroundReferenceData.Add(bfgKey, bfg);

            #endregion

            #region boobus bomb

            MapMakerObject bbLauncher = GetWeaponObjectFromWeaponsData<BoobusBombLauncherAmmo>(weaponsPath, nameof(Properties.Resources.keen_dreams_boobus_bomb2), out string bbKey);

            backgroundReferenceData.Add(bbKey, bbLauncher);

            #endregion

            #region Shotgun

            MapMakerObject shotgun = GetWeaponObjectFromWeaponsData<ShotgunNeuralStunnerAmmo>(weaponsPath, nameof(Properties.Resources.neural_stunner_shotgun), out string shotgunKey);

            backgroundReferenceData.Add(shotgunKey, shotgun);

            #endregion

            #region snake gun

            MapMakerObject snakeGun = GetWeaponObjectFromWeaponsData<SnakeGunAmmo>(weaponsPath, nameof(Properties.Resources.snake_gun1), out string snakeGunKey);

            backgroundReferenceData.Add(snakeGunKey, snakeGun);

            #endregion

            #endregion

            #region Player

            string playerPath = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_PLAYER, "keen4", Biomes.BIOME_KEEN5_BLACK);
            string[] playerFiles = Directory.GetFiles(playerPath);

            string imgPlayerLeftPath = playerFiles.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen_stand_left)));
            string imgPlayerRightPath = playerFiles.FirstOrDefault(f => f.Contains(nameof(Properties.Resources.keen_stand_right)));
            Image imgPlayerLeft = Properties.Resources.keen_stand_left;
            Image imgPlayerRight = Properties.Resources.keen_stand_right;

            MapMakerObjectProperty[] playerLeftProperties = new MapMakerObjectProperty[]
            {
                   new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, imgPlayerLeft.Width, imgPlayerLeft.Height),
                        },
                  new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                  new MapMakerObjectProperty()
                        {
                            PropertyName = "direction",
                            DataType = typeof(Direction),
                            Hidden = true,
                            Value = Direction.LEFT,
                            PossibleValues = Enum.GetNames(typeof(Direction)),
                            IsSpriteProperty = true
                        },
                  new MapMakerObjectProperty()
                        {
                            PropertyName = "lives",
                            DataType = typeof(int),
                            Value = 3,
                            DisplayName ="lives: ",
                            Hidden = true
                        },
                    new MapMakerObjectProperty()
                        {
                            PropertyName = "points",
                            DataType = typeof(int),
                            Value = 0,
                            DisplayName ="points: ",
                            Hidden = true
                        },
            };

            MapMakerObjectProperty[] playerRightProperties = new MapMakerObjectProperty[]
            {
                   new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, imgPlayerRight.Width, imgPlayerRight.Height),
                        },
                  new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                  new MapMakerObjectProperty()
                        {
                            PropertyName = "direction",
                            DataType = typeof(Direction),
                            Hidden = true,
                            Value = Direction.RIGHT,
                            PossibleValues = Enum.GetNames(typeof(Direction)),
                            IsSpriteProperty = true
                        },
                  new MapMakerObjectProperty()
                        {
                            PropertyName = "lives",
                            DataType = typeof(int),
                            Value = 3,
                            DisplayName ="lives: ",
                            Hidden = true
                        },
                    new MapMakerObjectProperty()
                        {
                            PropertyName = "points",
                            DataType = typeof(int),
                            Value = 0,
                            DisplayName ="points: ",
                            Hidden = true
                        },
         };

            MapMakerObject keenLeftObj = new MapMakerObject(typeof(CommanderKeen), imgPlayerLeftPath, false, playerLeftProperties);
            MapMakerObject keenRightObj = new MapMakerObject(typeof(CommanderKeen), imgPlayerRightPath, false, playerRightProperties);

            backgroundReferenceData.Add(nameof(Properties.Resources.keen_stand_left), keenLeftObj);
            backgroundReferenceData.Add(nameof(Properties.Resources.keen_stand_right), keenRightObj);


            #endregion

            #region Hazards

            string keen4HazardPath = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_HAZARDS, "keen4", Biomes.BIOME_KEEN4_CAVE);
            string[] keen4HazardFiles = Directory.GetFiles(keen4HazardPath);

            #region mine
            string keen4MineImagePath = keen4HazardFiles.FirstOrDefault(m => m.Contains(nameof(Properties.Resources.keen4_mine)));
            string mineKeyName = FileIOUtility.ExtractFileNameFromPath(keen4MineImagePath);
            Image keen4MineImage = Image.FromFile(keen4MineImagePath);

            MapMakerObjectProperty[] mineProperties = new MapMakerObjectProperty[]
            {
                  new MapMakerObjectProperty()
                  {
                      PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                      DisplayName = "Area: ",
                      DataType = typeof(Rectangle),
                      Value = new Rectangle(0, 0, keen4MineImage.Width, keen4MineImage.Height),
                  },
                  new MapMakerObjectProperty()
                  {
                       PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                       DataType = typeof(SpaceHashGrid),
                       Value = null,
                       Hidden = true,
                       IsIgnoredInMapData = true
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "initialDirection",
                      DisplayName = "Start Direction: ",
                      DataType = typeof(Direction),
                      Value = Direction.LEFT,
                      PossibleValues = new string[]
                      {
                          Direction.LEFT.ToString(),
                          Direction.RIGHT.ToString(),
                          Direction.UP.ToString(),
                          Direction.DOWN.ToString()
                      },
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "boundsX",
                      DisplayName = "Bounds X: ",
                      DataType = typeof(int),
                      Value = -1,
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "boundsY",
                      DisplayName = "Bounds Y: ",
                      DataType = typeof(int),
                      Value = -1,
                  },
                    new MapMakerObjectProperty()
                  {
                      PropertyName = "boundsWidth",
                      DisplayName = "Bounds Width: ",
                      DataType = typeof(int),
                      Value = keen4MineImage.Width,
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "boundsHeight",
                      DisplayName = "Bounds Height: ",
                      DataType = typeof(int),
                      Value = keen4MineImage.Height,
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "zIndex",
                      DataType = typeof(int),
                      Value = 20,
                      DisplayName ="Z Index: "
                  },
            };

            MapMakerObject mineObj = new MapMakerObject(typeof(Mine), keen4MineImagePath, false, mineProperties);
            backgroundReferenceData.Add(mineKeyName, mineObj);
            #endregion

            #region Spikes
            string keen4spikeImagePath = keen4HazardFiles.FirstOrDefault(m => m.Contains(nameof(Properties.Resources.keen_4_spikes)));
            string spikeKeyName = FileIOUtility.ExtractFileNameFromPath(keen4spikeImagePath);
            Image keen4SpikeImage = Image.FromFile(keen4MineImagePath);

            MapMakerObjectProperty[] keen4SpikeProperties = new MapMakerObjectProperty[]
            {
                  new MapMakerObjectProperty()
                  {
                       PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                       DataType = typeof(SpaceHashGrid),
                       Value = null,
                       Hidden = true,
                       IsIgnoredInMapData = true
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                      DisplayName = "Area: ",
                      DataType = typeof(Rectangle),
                      Value = new Rectangle(0, 0, keen4MineImage.Width, keen4MineImage.Height),
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "hazardType",
                      DataType = typeof(HazardType),
                      Value = HazardType.KEEN4_SPIKE,
                      Hidden = true,
                      IsSpriteProperty = true
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "zIndex",
                      DataType = typeof(int),
                      Value = 20,
                      DisplayName ="Z Index: "
                  },
            };

            MapMakerObject keen4SpikeObj = new MapMakerObject(typeof(Hazard), keen4spikeImagePath, false, keen4SpikeProperties);

            backgroundReferenceData.Add(spikeKeyName, keen4SpikeObj);
            #endregion
            #region Keen 4



            #endregion

            #endregion

            #region keen 5 key card

            string keyCardDirectory = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_CONSTRUCTS, "keen5", Biomes.BIOME_KEEN5_BLACK);
            string keyCardPath = Directory.GetFiles(keyCardDirectory).FirstOrDefault(s => s.Contains("key_card"));

            string keyCardKeyName = FileIOUtility.ExtractFileNameFromPath(keyCardPath);
            Image keyCardImg = Image.FromFile(keyCardPath);


            MapMakerObjectProperty[] keyCardProperties = new MapMakerObjectProperty[]
            {
                  new MapMakerObjectProperty()
                  {
                      PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                      DisplayName = "Area: ",
                      DataType = typeof(Rectangle),
                      Value = new Rectangle(0, 0, keyCardImg.Width, keyCardImg.Height),
                  },
                  new MapMakerObjectProperty()
                  {
                       PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                       DataType = typeof(SpaceHashGrid),
                       Value = null,
                       Hidden = true,
                       IsIgnoredInMapData = true
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "imageName",
                      Readonly = true,
                      DataType = typeof(string),
                      Value = keyCardKeyName,
                      IsIgnoredInMapData = true
                  },
                  new MapMakerObjectProperty()
                  {
                      PropertyName = "zIndex",
                      DataType = typeof(int),
                      Value = 20,
                      DisplayName ="Z Index: "
                  },
            };

            MapMakerObject keyCardObj = new MapMakerObject(typeof(KeyCard), keyCardPath, false, keyCardProperties);
            backgroundReferenceData.Add(keyCardKeyName, keyCardObj);

            #endregion

            #region CTF

            string ctfDirectory = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_CTF_ITEMS, "keen5", Biomes.BIOME_KEEN5_BLACK);
            string[] ctfFiles = Directory.GetFiles(ctfDirectory);

            foreach (var file in ctfFiles)
            {
                string key = GetFlagKeyNameFromFile(file);

                string imageName = FileIOUtility.ExtractFileNameFromPath(file);
                Image img = Image.FromFile(file);

                bool isFlag = !key.Contains("destination");

                if (isFlag)
                {
                    MapMakerObjectProperty[] ctfProperties = new MapMakerObjectProperty[]
                    {
                      new MapMakerObjectProperty()
                      {
                          PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                          DisplayName = "Area: ",
                          DataType = typeof(Rectangle),
                          Value = new Rectangle(0, 0, img.Width, img.Height),
                      },
                      new MapMakerObjectProperty()
                      {
                           PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                           DataType = typeof(SpaceHashGrid),
                           Value = null,
                           Hidden = true,
                           IsIgnoredInMapData = true
                      },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = "imageName",
                          Readonly = true,
                          DataType = typeof(string),
                          Value = imageName,
                          IsIgnoredInMapData = true
                      },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = "zIndex",
                          DataType = typeof(int),
                          Value = 20,
                          DisplayName ="Z Index: "
                      },
                       new MapMakerObjectProperty()
                       {
                            PropertyName = "color",
                            DataType = typeof(GemColor),
                            Readonly = true,
                            Value = InferFlagColorFromKey(key),
                            PossibleValues = Enum.GetNames(typeof(GemColor)),
                            IsSpriteProperty = true
                        },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = "maxPoints",
                          DataType = typeof(int),
                          Value = 10000,
                          DisplayName ="Max Points: "
                      },
                       new MapMakerObjectProperty()
                       {
                          PropertyName = "minPoints",
                          DataType = typeof(int),
                          Value = 5000,
                          DisplayName ="Min Points: "
                        },
                        new MapMakerObjectProperty()
                        {
                          PropertyName = "pointsDegradedPerSecond",
                          DataType = typeof(int),
                          Value = 10,
                          DisplayName ="Min Points: "
                        },
                    };

                    MapMakerObject flagObj = new MapMakerObject(typeof(Flag), file, false, ctfProperties);
                    backgroundReferenceData.Add(key, flagObj);
                }
                else
                {
                    MapMakerObjectProperty[] ctfProperties = new MapMakerObjectProperty[]
                  {
                      new MapMakerObjectProperty()
                      {
                          PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                          DisplayName = "Area: ",
                          DataType = typeof(Rectangle),
                          Value = new Rectangle(0, 0, img.Width, img.Height),
                      },
                      new MapMakerObjectProperty()
                      {
                           PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                           DataType = typeof(SpaceHashGrid),
                           Value = null,
                           Hidden = true,
                           IsIgnoredInMapData = true
                      },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = "zIndex",
                          DataType = typeof(int),
                          Value = 20,
                          DisplayName ="Z Index: "
                      },
                       new MapMakerObjectProperty()
                       {
                            PropertyName = "color",
                            DataType = typeof(GemColor),
                            Readonly = true,
                            Value = InferFlagColorFromKey(key),
                            PossibleValues = Enum.GetNames(typeof(GemColor)),
                            IsSpriteProperty = true
                        }
                  };

                    MapMakerObject flagObj = new MapMakerObject(typeof(CTFDestination), file, false, ctfProperties);
                    backgroundReferenceData.Add(key, flagObj);
                }
            }

            #endregion

            #region Shield
            string shieldDirectory = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_SHIELD, "keen5", Biomes.BIOME_KEEN5_BLACK);
            string shieldFile = Directory.GetFiles(shieldDirectory).FirstOrDefault(f => f.Contains("Shield"));

            string shieldImageName = FileIOUtility.ExtractFileNameFromPath(shieldFile);
            Image shieldImg = Image.FromFile(shieldFile);

            MapMakerObjectProperty[] shieldProperties = new MapMakerObjectProperty[]
            {
                      new MapMakerObjectProperty()
                      {
                          PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                          DisplayName = "Area: ",
                          DataType = typeof(Rectangle),
                          Value = new Rectangle(0, 0, shieldImg.Width, shieldImg.Height),
                      },
                      new MapMakerObjectProperty()
                      {
                           PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                           DataType = typeof(SpaceHashGrid),
                           Value = null,
                           Hidden = true,
                           IsIgnoredInMapData = true
                      },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = "imageName",
                          Readonly = true,
                          DataType = typeof(string),
                          Value = shieldImageName,
                          IsIgnoredInMapData = true
                      },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = "zIndex",
                          DataType = typeof(int),
                          Value = 20,
                          DisplayName ="Z Index: "
                      },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = "duration",
                          DataType = typeof(int),
                          Value = 30,
                          DisplayName ="Duration (seconds): "
                      }
            };

            MapMakerObject shieldObj = new MapMakerObject(typeof(Shield), shieldFile, false, shieldProperties);
            backgroundReferenceData.Add(shieldImageName, shieldObj);


            #endregion

            #region Keen Constructs
            string keen4ConstructDirectory = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_CONSTRUCTS, "keen4", Biomes.BIOME_KEEN4_CAVE);
            string[] keen4ConstructFiles = Directory.GetFiles(keen4ConstructDirectory);

            string keen5ConstructDirectory = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_CONSTRUCTS, "keen5", Biomes.BIOME_KEEN5_BLACK);
            string[] keen5ConstructFiles = Directory.GetFiles(keen5ConstructDirectory);

            string keen6ConstructDirectory = GetImageDirectory(MapMakerConstants.Categories.OBJECT_CATEGORY_CONSTRUCTS, "keen6", Biomes.BIOME_KEEN6_DOME);
            string[] keen6ConstructFiles = Directory.GetFiles(keen6ConstructDirectory);

            List<string> allFiles = new List<string>(keen4ConstructFiles);
            allFiles.AddRange(keen5ConstructFiles);
            allFiles.AddRange(keen6ConstructFiles);
            var doorFiles = allFiles.Where(f => f.Contains("door") || f.Contains("chute"));
            foreach (var file in doorFiles)
            {
                bool isExitDoor = file.Contains("exit");
                bool isChute = file.Contains("chute");
                string doorImageName = FileIOUtility.ExtractFileNameFromPath(file);
                Image doorImg = Image.FromFile(file);

                MapMakerObjectProperty[] doorProperties = new MapMakerObjectProperty[]
                {
                      new MapMakerObjectProperty()
                      {
                          PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                          DisplayName = "Area: ",
                          DataType = typeof(Rectangle),
                          Value = new Rectangle(0, 0, doorImg.Width, doorImg.Height),
                      },
                      new MapMakerObjectProperty()
                      {
                           PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                           DataType = typeof(SpaceHashGrid),
                           Value = null,
                           Hidden = true,
                           IsIgnoredInMapData = true
                      },
                      new MapMakerObjectProperty()
                      {
                            PropertyName = "doorType",
                            DataType = typeof(DoorType),
                            Value = InferDoorTypeFromImage(doorImageName),
                            PossibleValues = Enum.GetNames(typeof(DoorType)),
                            IsSpriteProperty = true,
                            Hidden = true
                      },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = GeneralGameConstants.DOOR_ID_PROPERTY_NAME,
                          DataType = typeof(int),
                          Value = 1,
                          DisplayName ="Id: "
                      },
                      new MapMakerObjectProperty()
                      {
                          PropertyName = GeneralGameConstants.DESTINATION_DOOR_ID_PROPERTY_NAME,
                          DisplayName = "Select Door: ",
                          DataType = typeof(int?),
                          Value = null,
                          IsDoorSelectionProperty = true
                      },
                };
                if (isExitDoor)
                {
                    var altDoorProperties = new MapMakerObjectProperty[]
                    {
                        new MapMakerObjectProperty()
                      {
                          PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                          DisplayName = "Area: ",
                          DataType = typeof(Rectangle),
                          Value = new Rectangle(0, 0, doorImg.Width, doorImg.Height),
                      },
                      new MapMakerObjectProperty()
                      {
                           PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                           DataType = typeof(SpaceHashGrid),
                           Value = null,
                           Hidden = true,
                           IsIgnoredInMapData = true
                      },
                    };
                    MapMakerObject doorObj = new MapMakerObject(typeof(ExitDoor), file, false, altDoorProperties);
                    backgroundReferenceData.Add(doorImageName, doorObj);
                }
                else
                {
                    if (isChute)
                        doorProperties.LastOrDefault().Hidden = true;

                    MapMakerObject doorObj = new MapMakerObject(typeof(Door), file, false, doorProperties);
                    if (!backgroundReferenceData.ContainsKey(doorImageName))
                        backgroundReferenceData.Add(doorImageName, doorObj);
                }
            }

            #endregion

            return backgroundReferenceData;
        }

        #region reference data
        private static Dictionary<string, MapMakerObject> _imageObjectMapping = GetBackgroundObjectData();

        #endregion

        public static string GetImageDirectory(string categoryFolder, string episodeFolder, string biomeFolder)
        {
            string path = string.Empty;
            string mapMakerFolder = MapMakerConstants.MAP_MAKER_FOLDER;
            if (string.IsNullOrEmpty(categoryFolder)
             || string.IsNullOrEmpty(episodeFolder)
             || string.IsNullOrEmpty(biomeFolder))
                return path;



            if (categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_WEAPONS
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_PLAYER
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_GEMS
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_CTF_ITEMS
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_SHIELD)
            {
                path = Path.Combine(System.Environment.CurrentDirectory, mapMakerFolder, categoryFolder);
            }
            else if (categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_TILES)
            {
                path = Path.Combine(System.Environment.CurrentDirectory, mapMakerFolder, categoryFolder, episodeFolder, biomeFolder);
            }
            else
            {
                path = Path.Combine(System.Environment.CurrentDirectory, mapMakerFolder, categoryFolder, episodeFolder);
            }
            return path;
        }

        public static MapMakerObject GetMapMakerObjectFromImageName(string imageName)
        {
            if (_imageObjectMapping.TryGetValue(imageName, out MapMakerObject item) && item != null)
            {
                MapMakerObjectProperty[] clonedProperties = item.CloneParameterList();
                MapMakerObject copy = new MapMakerObject(item.ObjectType, item.ImageControl.ImageLocation, item.IsManualPlacement, clonedProperties);
                return copy;
            }
            return null;
        }

        #region helper methods

        private static string GetFlagKeyNameFromFile(string filePath)
        {
            if (filePath.Contains("Black"))
                return "Black Flag";

            if (filePath.Contains("Blue"))
                return "Blue Flag";
            if (filePath.Contains("blue_"))
                return "blue_flag_destination";

            if (filePath.Contains("Green"))
                return "Green Flag";
            if (filePath.Contains("green_"))
                return "green_flag_destination";

            if (filePath.Contains("Red"))
                return "Red Flag";
            if (filePath.Contains("red_"))
                return "red_flag_destination";

            if (filePath.Contains("Yellow"))
                return "Yellow Flag";
            if (filePath.Contains("yellow_"))
                return "yellow_flag_destination";

            return string.Empty;
        }

        private static GemColor InferFlagColorFromKey(string key)
        {
            if (key == null)
                return GemColor.RED;

            if (key.Contains("Blue") || key.Contains("blue"))
                return GemColor.BLUE;

            if (key.Contains("Green") || key.Contains("green"))
                return GemColor.GREEN;

            if (key.Contains("Yellow") || key.Contains("yellow"))
                return GemColor.YELLOW;

            return GemColor.RED;
        }

        private static MapMakerObject GetWeaponObjectFromWeaponsData<TWeapon>(string weaponsPath, string resourceName, out string mapMakerObjectKey)
        {

            string imagePath = weaponsPath + @"\" + resourceName + ".png";
            string imageName = FileIOUtility.ExtractFileNameFromPath(imagePath);
            mapMakerObjectKey = imageName;
            Image image = Image.FromFile(imagePath);

            MapMakerObjectProperty[] weaponProperties = new MapMakerObjectProperty[]
          {

                        new MapMakerObjectProperty()
                        {
                            PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                            DataType = typeof(SpaceHashGrid),
                            Value = null,
                            Hidden = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            DisplayName = "Area: ",
                            PropertyName = GeneralGameConstants.AREA_PROPERTY_NAME,
                            DataType = typeof(Rectangle),
                            Value = new Rectangle(0, 0, image.Width, image.Height),
                        },

                        new MapMakerObjectProperty()
                        {
                            PropertyName = "imageName",
                            DataType = typeof(string),
                            Hidden = true,
                            Value = imageName,
                            IsSpriteProperty = true,
                            IsIgnoredInMapData = true
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "zIndex",
                            DataType = typeof(int),
                            Value = 50,
                            DisplayName ="Z Index: "
                        },
                        new MapMakerObjectProperty()
                        {
                            PropertyName = "ammo",
                            DataType = typeof(int),
                            Value = 5,
                            DisplayName = "Ammo: "
                        }
          };
            MapMakerObject rpg = new MapMakerObject(typeof(TWeapon), imagePath, false, weaponProperties);
            return rpg;
        }

        private static string InferBiomeFromImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return imageName;

            if (imageName.Contains("keen4_cave"))
            {
                return Biomes.BIOME_KEEN4_CAVE;
            }

            if (imageName.Contains("keen4_forest"))
            {
                return Biomes.BIOME_KEEN4_FOREST;
            }

            if (imageName.Contains("keen4_mirage"))
            {
                return Biomes.BIOME_KEEN4_MIRAGE;
            }

            if (imageName.Contains("keen4_pyramid"))
            {
                return Biomes.BIOME_KEEN4_PYRAMID;
            }


            if (imageName.Contains("keen5_black") ||
                (imageName.Contains("keen5") &&
                    (imageName.Contains("black") || imageName.Contains("blue"))))
            {
                return Biomes.BIOME_KEEN5_BLACK;
            }

            if (imageName.Contains("keen5_red") || (imageName.Contains("keen5") &&
                    imageName.Contains("red")))
            {
                return Biomes.BIOME_KEEN5_RED;
            }

            if (imageName.Contains("keen5_green") || (imageName.Contains("keen5") &&
                    imageName.Contains("green")))
            {
                return Biomes.BIOME_KEEN5_GREEN;
            }

            if (imageName.Contains("keen6_dome"))
            {
                return Biomes.BIOME_KEEN6_DOME;
            }

            if (imageName.Contains("keen6_forest"))
            {
                return Biomes.BIOME_KEEN6_FOREST;
            }

            if (imageName.Contains("keen6_industrial"))
            {
                return Biomes.BIOME_KEEN6_INDUSTRIAL;
            }

            if (imageName.Contains("keen6_final"))
            {
                return Biomes.BIOME_KEEN6_FINAL;
            }

            return string.Empty;
        }

        private static DoorType InferDoorTypeFromImage(string imgName)
        {
            switch (imgName)
            {
                case nameof(Properties.Resources.keen4_blue_door):
                    return DoorType.KEEN4_BLUE;
                case nameof(Properties.Resources.keen4_gray_door):
                    return DoorType.KEEN4_GRAY;
                case nameof(Properties.Resources.keen4_oracle_door2):
                    return DoorType.KEEN4_ORACLE;
                case nameof(Properties.Resources.keen5_door):
                    return DoorType.KEEN5_REGULAR;
                case nameof(Properties.Resources.keen5_exit_door_closed):
                    return DoorType.KEEN5_EXIT;
                case nameof(Properties.Resources.keen6_door):
                    return DoorType.KEEN6;
                case nameof(Properties.Resources.chute):
                    return DoorType.CHUTE;
            }

            return DoorType.KEEN4_BLUE;
        }

        private static GemColor InferGemColorFromImageName(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                throw new ArgumentNullException(nameof(imageName));

            string lowerImg = imageName.ToLower();

            if (lowerImg.Contains("red"))
                return GemColor.RED;
            if (lowerImg.Contains("blue"))
                return GemColor.BLUE;
            if (lowerImg.Contains("green"))
                return GemColor.GREEN;
            if (lowerImg.Contains("yellow"))
                return GemColor.YELLOW;

            throw new ArgumentException("Could not infer gem color from image name");
        }

        #endregion
    }
}
