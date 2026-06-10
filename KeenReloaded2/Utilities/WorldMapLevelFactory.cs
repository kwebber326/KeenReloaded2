using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
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
            else if (imgName.Contains("keen4_level_fire_c2"))
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
                        new Rectangle(0, 60, 60, 32)
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
                properties.AddRange(animatedWorldMapProperties);

                obj = new MapMakerObject(typeof(AnimatedForegroundMultiHitboxWorldMapLevel),
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
            else if (imgName.Contains("keen4_pyramid_moon_c1"))
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
            else if (imgName.Contains("keen4_sand_yego_c1"))
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
            else if (imgName.Contains("keen4_village_c1"))
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
            else if (imgName.Contains("keen4_well_of_wishes_c1"))
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
                    Value = new Rectangle[0],
                    Hidden = true
                });

                obj = new MapMakerObject(typeof(ForeGroundMultiHitboxWorldMapLevel),
                    file, false, properties.ToArray());
            }

            return obj;
        }
    }
}
