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
using KeenReloaded2.Framework.GameEntities.Items;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;

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
              || categoryFolder == MapMakerConstants.Categories.OBJECT_CATEGORY_GEMS)
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
