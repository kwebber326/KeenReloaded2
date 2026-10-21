using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace KeenReloaded2.Utilities
{
    public static class WorldMapLevelFactory
    {
        private static string[] _worldMapSongs;

        public static string[] WorldMapSongs
        {
            get
            {
                if (_worldMapSongs == null)
                {
                    _worldMapSongs = FileIOUtility.LoadWavFormatSongs().ToArray();
                }
                return _worldMapSongs;
            }
        }
        public static MapMakerObject GetWorldMapLevelObject(string file, string currentDirectory)
        {
            Image img = Image.FromFile(file);
            Rectangle area = new Rectangle(0, 0, img.Width, img.Height);
            string imgName = FileIOUtility.ExtractFileNameFromPath(file);
            img.Tag = file;

            List<MapMakerObjectProperty> properties = new List<MapMakerObjectProperty>()
                    {
                          new MapMakerObjectProperty()
                         {
                             DisplayName = "Display Area:",
                             PropertyName  = GeneralGameConstants.AREA_PROPERTY_NAME,
                             DataType = typeof(Rectangle),
                             Value = area
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.SPACE_HASH_GRID_PROPERTY_NAME,
                             DataType = typeof(SpaceHashGrid),
                             Value = null,
                             IsIgnoredInMapData = true,
                             Hidden = true
                         },
                         new MapMakerObjectProperty()
                         {
                             DisplayName = "ZIndex:",
                             DataType = typeof(int),
                             Value = 10,
                             PropertyName = "zIndex"
                         },
                           new MapMakerObjectProperty()
                         {
                             PropertyName = "sprite",
                             DataType = typeof(Image),
                             Value = img,
                             Hidden = true,
                             IsIgnoredInMapData = true
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "levelName",
                             DisplayName = "Level:",
                             DataType = typeof(string),
                             Value = string.Empty
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "levelEntryText",
                             DisplayName = "Entry Text:",
                             DataType = typeof(string),
                             Value = string.Empty
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "episode",
                             DataType = typeof(string),
                             Hidden = true,
                             Value = string.Empty
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.MUSIC_PROPERTY_NAME,
                             DataType = typeof(string),
                             DisplayName = "Music:",
                             PossibleValues = WorldMapSongs,
                             Value = string.Empty
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = GeneralGameConstants.ACTIVATION_ID_PROPERTY_NAME,
                             DataType = typeof(Guid),
                             DisplayName = "Id:",
                             Readonly = true,
                             Value = new Guid()
                         },
                         new MapMakerObjectProperty()
                         {
                             PropertyName = "entryPoints",
                             DataType = typeof(Rectangle[]),
                             Value = new Rectangle[0],
                             Hidden = true,
                         }
                    };

            string folder = imgName.Substring(0, imgName.LastIndexOf("_"));
            string animationImagesPath = Path.Combine(currentDirectory, folder);
            animationImagesPath = animationImagesPath.Replace(Environment.CurrentDirectory + "\\", "");
            List<MapMakerObjectProperty> animatedWorldMapProperties = new List<MapMakerObjectProperty>() {
                new MapMakerObjectProperty()
                {
                    PropertyName = "imagesPath",
                    DataType = typeof(string),
                    Value = animationImagesPath,
                    IsIgnoredInMapData = true,
                    Hidden = true
                },

                new MapMakerObjectProperty()
                {
                    PropertyName = "animationDelay",
                    DisplayName = "Animation Delay:",
                    DataType = typeof(int),
                    Value = 200,
                },
                new MapMakerObjectProperty()
                {
                    PropertyName = "animationStartIndex",
                    DisplayName = "Animation Start Index:",
                    DataType = typeof(int),
                    Value = 0,
                },
                new MapMakerObjectProperty()
                {
                    PropertyName = "key",
                    DataType = typeof(string),
                    Value = folder,
                    Hidden = true,
                    IsIgnoredInMapData = true
                }
            };

            MapMakerObject obj = null;
            if (imgName.Contains("keen4_cave_mountains_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 3, img.Height),
                        new Rectangle(0, 0, img.Width, img.Height / 2),
                        new Rectangle(img.Width/ 3, img.Height - img.Height / 5, 16, 22),
                        new Rectangle(95, 52, 30, 32)
                    },
                    Hidden = true
                });
                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(0, 60, 60, 32)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 2, img.Height),
                        new Rectangle(0, 0, img.Width, img.Height / 2)
                    },
                    Hidden = true
                });
                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_crystal_cave_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 3, img.Height),
                        new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2),
                        new Rectangle(img.Width - img.Width / 3, img.Height / 2, img.Width / 3, img.Height / 2),
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(0, 30, 60, 62)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 2, img.Height),
                        new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2),
                        new Rectangle(img.Width - img.Width / 3, img.Height / 2, img.Width / 3, img.Height / 2),
                        new Rectangle(46, 35, 30, 11),
                        new Rectangle(49,47, 9, 6),
                        new Rectangle(49, 53, 8, 3),
                        new Rectangle(50, 57, 3, 2),
                        new Rectangle(53, 56, 3, 4),
                        new Rectangle(44, 43, 15, 16)
                    },
                    Hidden = true
                });
                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_level_fire_c2"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(2, 2, 77, 26),
                        new Rectangle(2, 26, 48, 38),
                        new Rectangle(50, 54, 17, 9)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(48, 30, 21, 24),
                        new Rectangle(74, 37, 13, 24)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[0],
                    Hidden = true
                });
                properties.AddRange(animatedWorldMapProperties);

                obj = new MapMakerObject(typeof(AnimatedForegroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_level_village2_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 3, img.Height),
                        new Rectangle(img.Width - img.Width / 3, 0, img.Width / 3, img.Height)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(30, 0, 30, 64)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                obj = new MapMakerObject(typeof(MultiHitBoxWorldMapLevel), file, false,
                    properties.ToArray());
            }
            else if (imgName.Contains("keen4_mirage_c2"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                   {
                        new Rectangle(0, 0, img.Width / 3, img.Height),
                        new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2),
                        new Rectangle(img.Width - img.Width / 3, img.Height / 2, img.Width / 3, img.Height / 2)
                   },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(0, 30, 60, 62)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(29, 45, 21, 1)
                    },
                    Hidden = true
                });
                properties.AddRange(animatedWorldMapProperties);

                obj = new MapMakerObject(typeof(Keen4MirageWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_oasis_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 3, img.Height),
                        new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2),
                        new Rectangle(img.Width - img.Width / 3, img.Height / 2, img.Width / 3, img.Height / 2)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 96, 80)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(41, 45, 11, 7),
                        new Rectangle(25,41, 15, 13),
                        new Rectangle(26, 51, 13, 6),
                        new Rectangle(50, 50, 4, 3),
                        new Rectangle(54, 50, 4, 4),
                        new Rectangle(58, 53, 3, 4),
                        new Rectangle(60, 56, 4, 3),
                        new Rectangle(60, 58, 6, 3),
                        new Rectangle(52, 43, 4, 4),
                        new Rectangle(49, 45, 5, 3)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_pyramid_eye_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 3, img.Height),
                        new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2),
                        new Rectangle(img.Width - img.Width / 3, img.Height / 2, img.Width / 3, img.Height / 2)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(27, 46, 39, 19)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 96, 63),
                        new Rectangle(0, 0, 32, 14),
                        new Rectangle(63, 62, 32, 14)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_pyramid_forbidden_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 3, img.Height),
                        new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2),
                        new Rectangle(img.Width - img.Width / 3, img.Height / 2, img.Width / 3, img.Height / 2)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(27, 46, 39, 19)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 96, 63),
                        new Rectangle(0, 0, 32, 14),
                        new Rectangle(63, 62, 32, 14)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_pyramid_moon_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                       new Rectangle(0,0, 32, 68),
                       new Rectangle(31, 0, 32, 36),
                       new Rectangle(63, 0, 31, 68)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(30, 38, 34, 15)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(24, 0, 48, 53)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_sand_yego_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(8, 6, 75, 18),
                        new Rectangle(3, 30, 13, 12),
                        new Rectangle(74, 32, 17, 11),
                        new Rectangle(0, 45, 31, 20),
                        new Rectangle(64, 44, 31, 20)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(15, 27, 96, 64)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(24, 31, 2, 14),
                        new Rectangle(27, 28, 7, 19),
                        new Rectangle(32, 32, 5, 14),
                        new Rectangle(36, 28, 9, 19),
                        new Rectangle(35, 33, 5, 14),
                        new Rectangle(50, 28, 8, 19),
                        new Rectangle(58, 33, 2, 14),
                        new Rectangle(60, 28, 7, 19),
                        new Rectangle(67, 33, 3, 13),
                        new Rectangle(24, 34, 46, 12)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_small_village_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 10, 48),
                        new Rectangle(8, 11, 16, 15),
                        new Rectangle(8, 26, 41, 22),
                        new Rectangle(32, 34, 11, 2)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(23, 0, 48, 25),
                        new Rectangle(48, 24, 23, 24)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[0],
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_small_village_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 3, img.Height),
                        new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2),
                        new Rectangle(img.Width - img.Width / 3, img.Height / 2, img.Width / 3, img.Height / 2)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(0, 60, 60, 32)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, img.Width / 2, img.Height),
                        new Rectangle(img.Width / 2, 0, img.Width / 2, img.Height / 2),
                        new Rectangle(img.Width - img.Width / 3, img.Height / 2, img.Width / 3, img.Height / 2)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_tar_isle_c2"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                            new Rectangle(2, 40, 94, 87),
                            new Rectangle(10, 22, 13, 15),
                            new Rectangle(86, 32, 6, 7)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(25, 21, 53, 22)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[0],
                    Hidden = true
                });
                properties.AddRange(animatedWorldMapProperties);

                obj = new MapMakerObject(typeof(AnimatedForegroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_underground_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 159, 25),
                        new Rectangle(0, 24, 98, 17),
                        new Rectangle(0, 41, 58, 20),
                        new Rectangle(68, 92, 90, 22),
                        new Rectangle(85, 83, 73, 11),
                        new Rectangle(108, 76, 53, 11),
                        new Rectangle(151, 0, 7, 115)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(80, 16, 77, 68)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(126, 0, 32, 20),
                        new Rectangle(135, 22, 23, 7),
                        new Rectangle(139, 28, 19, 6),
                        new Rectangle(139, 48, 19, 67),
                        new Rectangle(84, 86, 75, 29),
                        new Rectangle(98, 74, 61, 13),
                        new Rectangle(112, 68, 10, 7),
                        new Rectangle(133, 57, 16, 16)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_village_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 33, 80),
                        new Rectangle(64, 0, 48, 80),
                        new Rectangle(32, 66, 31, 12)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(34, 16, 32, 48)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[0],
                    Hidden = true
                });

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "orientation",
                    DisplayName = "Orientation:",
                    DataType = typeof(Orientation),
                    Value = Orientation.NORMAL,
                    PossibleValues = Enum.GetNames(typeof(Orientation)),
                });

                obj = new MapMakerObject(typeof(Keen4GatedVillageWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen4_well_of_wishes_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(15, 10, 63, 31),
                        new Rectangle(50, 48, 25, 9),
                        new Rectangle(48, 56, 5, 10)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(35, 32, 32, 15),
                        new Rectangle(20, 38, 30, 21)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_4;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[0],
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_jet_fuel_exhaust_c2"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                            new Rectangle(24, 0, 124, 100),
                            new Rectangle(146, 72, 194, 26),
                            new Rectangle(216, 0, 124, 16),
                            new Rectangle(312, 0, 28, 90),
                            new Rectangle(153, 76, 117, 17),
                            new Rectangle(271,79,14,17),
                            new Rectangle(285, 82, 13, 17),
                            new Rectangle(298, 85, 12, 16)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(150,16, 158, 40),
                        new Rectangle(150, 0, 64, 16)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[0],
                    Hidden = true
                });
                properties.AddRange(animatedWorldMapProperties);

                obj = new MapMakerObject(typeof(AnimatedForegroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_hallway_vertical") && imgName.Contains("c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                      {
                                new Rectangle(0, 0, 32, 86),
                                new Rectangle(0, 67, 19, 111),
                                new Rectangle(0, 181, 32, 40),
                                new Rectangle(95, 0, 32, 86),
                                new Rectangle(108, 67, 20, 86),
                                new Rectangle(96,151, 32,70),
                      },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(21, 1, 80, 220)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(45, 15, 37, 146),
                        new Rectangle(31, 21, 10, 142),
                        new Rectangle(37, 19, 10, 144),
                        new Rectangle(82, 21, 10, 142),
                        new Rectangle(29, 26, 4, 52),
                        new Rectangle(31, 117, 10, 48),
                        new Rectangle(85, 117, 10, 48),
                        new Rectangle(27, 119, 10, 52),
                        new Rectangle(89, 117, 6, 54),
                        new Rectangle(22, 122, 9, 60),
                        new Rectangle(16, 126, 16, 56),
                        new Rectangle(92, 117, 10, 58),
                        new Rectangle(100, 121, 6, 52),
                        new Rectangle(19, 91, 8, 6),
                        new Rectangle(27, 96, 2, 4),
                        new Rectangle(31, 100, 2, 2),
                        new Rectangle(22, 99, 2, 2),
                        new Rectangle(24, 101, 2, 2),
                        new Rectangle(19, 115, 2, 2),
                        new Rectangle(22, 118, 2, 2),
                        new Rectangle(98, 94, 2, 2),
                        new Rectangle(102, 96, 2, 2),
                        new Rectangle(99,97, 4, 2),
                        new Rectangle(98, 100, 4, 2),
                        new Rectangle(95, 102, 2, 2),
                        new Rectangle(93, 103, 2, 2),
                        new Rectangle(105, 117, 2, 2),
                        new Rectangle(103, 119, 2, 2),
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_world_map_sphere_right_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                   {
                        new Rectangle(155, 71, 11, 36),
                        new Rectangle(116, 55, 114, 16),
                        new Rectangle(134, 105, 162, 33),
                        new Rectangle(235, 73, 61, 5)
                   },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(157, 71, 74, 64)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(144, 71, 73, 52),
                        new Rectangle(215, 87, 5, 15),
                        new Rectangle(221, 91, 4, 12),
                        new Rectangle(225, 96, 5, 8),
                        new Rectangle(232, 101, 4, 4),
                        new Rectangle(211, 93, 14, 13)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_world_map_sphere_left_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                   {
                        new Rectangle(0, 98, 117, 5),
                        new Rectangle(116, 98, 16, 64),
                        new Rectangle(0, 131, 136, 30),
                   },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(15, 103, 118, 27)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(48, 97, 74, 34),
                        new Rectangle(42, 112, 6, 19),
                        new Rectangle(39, 115, 4, 16),
                        new Rectangle(34, 119, 4, 14),
                        new Rectangle(232, 101, 4, 4),
                        new Rectangle(211, 93, 14, 13),
                        new Rectangle(30, 123, 4, 8)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_world_map_sphere_up_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                   {
                        new Rectangle(58, 0, 32, 67),
                        new Rectangle(58, 56, 130, 3),
                        new Rectangle(154, 0, 32, 67),
                   },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(89, 8, 64, 48)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(90, 5, 14, 2),
                        new Rectangle(141, 5, 14, 2),
                        new Rectangle(88, 7, 71, 52)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_world_map_sphere_down_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                   {
                        new Rectangle(62, 160, 32, 121),
                        new Rectangle(65, 150, 126, 8),
                        new Rectangle(158, 137, 32, 144),
                   },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(62, 107, 129, 92)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(62, 99, 129, 98),
                        new Rectangle(62, 200, 47, 4),
                        new Rectangle(77, 203, 24, 4),
                        new Rectangle(76, 208, 2, 6),
                        new Rectangle(136, 196, 58, 4),
                        new Rectangle(144, 199, 48, 4),
                        new Rectangle(150, 204, 41, 4),
                        new Rectangle(157, 208, 35, 8),
                        new Rectangle(71, 196, 37, 5)
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_world_map_barrier_level_horizontal_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(10, 0, 12, 62),
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 10, 62),
                        new Rectangle(20, 0, 10, 62)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[0],
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(BarrierLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_world_map_barrier_level_vertical_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 8, 60, 10),
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 60, 8),
                        new Rectangle(0, 18, 60, 8)
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[0],
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(BarrierLevel),
                    file, false, properties.ToArray());
            }
            else if (imgName.Contains("keen5_world_map_corridor_c1"))
            {
                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "hitboxes",
                    DisplayName = "Hitboxes:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(0, 0, 159, 36),
                        new Rectangle(0, 115, 160, 12)
                    },
                    Hidden = true
                });

                var entryPointProp = properties.FirstOrDefault(p => p.PropertyName == "entryPoints");
                if (entryPointProp != null)
                    entryPointProp.Value = new Rectangle[]
                    {
                        new Rectangle(0, 30, 160, 86),
                    };

                var episodeProp = properties.FirstOrDefault(p => p.PropertyName == "episode");
                if (episodeProp != null) episodeProp.Value = GeneralGameConstants.Episodes.EPISODE_5;

                properties.Add(new MapMakerObjectProperty()
                {
                    PropertyName = "foregroundAreas",
                    DisplayName = "Foreground Areas:",
                    DataType = typeof(Rectangle[]),
                    Value = new Rectangle[]
                    {
                        new Rectangle(2, 15, 140, 14),
                        new Rectangle(0, 29, 140, 10),
                        new Rectangle(0, 39, 128, 15),
                        new Rectangle(0, 53, 36, 74),
                        new Rectangle(95, 53, 42, 74),
                        new Rectangle(0, 104, 138, 24),
                        new Rectangle(37, 55, 6, 46),
                        new Rectangle(38, 96, 56, 6),
                        new Rectangle(38, 56, 6, 28),
                          new Rectangle(38, 56, 4, 46),
                          new Rectangle(46, 56, 2, 2),
                          new Rectangle(52, 56, 6, 2),
                          new Rectangle(52, 56, 4, 4),
                          new Rectangle(38, 58, 8, 2),
                          new Rectangle(50, 58, 4, 4),
                          new Rectangle(48, 60, 2, 6),
                          new Rectangle(46, 62, 6, 2),
                          new Rectangle(38, 64, 10, 4),
                          new Rectangle(38, 64, 8, 6),
                          new Rectangle(68, 64, 2, 2),
                          new Rectangle(66, 66, 2, 2),
                          new Rectangle(64, 68, 2, 2),
                          new Rectangle(62, 70, 2, 2),
                          new Rectangle(58, 72, 4, 2),
                          new Rectangle(58, 72, 2, 4),
                          new Rectangle(50, 74, 2, 2),
                          new Rectangle(54, 74, 4, 4),
                          new Rectangle(48, 76, 2, 2),
                          new Rectangle(52, 76, 4, 6),
                          new Rectangle(46, 78, 2, 2),
                          new Rectangle(50, 78, 2, 8),
                          new Rectangle(38, 80, 8, 2),
                          new Rectangle(48, 80, 6, 4),
                          new Rectangle(60, 80, 2, 2),
                          new Rectangle(92, 80, 2, 6),
                          new Rectangle(46, 82, 4, 6),
                          new Rectangle(58, 82, 2, 2),
                          new Rectangle(90, 82, 4, 4),
                          new Rectangle(44, 84, 4, 6),
                          new Rectangle(56, 84, 2, 2),
                          new Rectangle(88, 84, 4, 4),
                          new Rectangle(38, 86, 6, 16),
                          new Rectangle(38, 86, 8, 6),
                          new Rectangle(54, 86, 2, 2),
                          new Rectangle(86, 86, 4, 4),
                          new Rectangle(52, 88, 2, 2),
                          new Rectangle(80, 88, 2, 2),
                          new Rectangle(84, 88, 2, 6),
                          new Rectangle(50, 90, 2, 2),
                          new Rectangle(78, 90, 2, 2),
                          new Rectangle(82, 90, 6, 2),
                          new Rectangle(92, 90, 2, 2),
                          new Rectangle(48, 92, 2, 2),
                          new Rectangle(76, 92, 2, 2),
                          new Rectangle(80, 92, 4, 10),
                          new Rectangle(90, 92, 2, 2),
                          new Rectangle(46, 94, 2, 8),
                          new Rectangle(74, 94, 2, 8),
                          new Rectangle(78, 94, 6, 8),
                          new Rectangle(88, 94, 2, 8),
                          new Rectangle(38, 96, 56, 6),
                    },
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }

            return obj;
        }
    }
}
