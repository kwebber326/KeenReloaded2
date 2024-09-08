using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.SpriteSheet
{
    public static class SpriteSheet
    {
        #region ceilings

        #region keen4
        private static Image[] _keen4CaveCeilings;

        public static Image[] Keen4CaveCeilings
        {
            get
            {
                if (_keen4CaveCeilings == null)
                {
                    _keen4CaveCeilings = new Image[]
                    {
                        Properties.Resources.keen4_cave_wall_bottom1,
                        Properties.Resources.keen4_cave_wall_bottom2,
                        Properties.Resources.keen4_cave_wall_bottom3,
                        Properties.Resources.keen4_cave_wall_bottom4,
                        Properties.Resources.keen4_cave_wall_bottom5,
                        Properties.Resources.keen4_cave_wall_bottom6,
                        Properties.Resources.keen4_cave_wall_bottom7
                    };
                }
                return _keen4CaveCeilings;
            }
        }


        private static Image[] _keen4ForestCeilings;

        public static Image[] Keen4ForestCeilings
        {
            get
            {
                if (_keen4ForestCeilings == null)
                {
                    _keen4ForestCeilings = new Image[]
                    {
                        Properties.Resources.keen4_forest_wall_bottom1,
                        Properties.Resources.keen4_forest_wall_bottom2
                    };
                }
                return _keen4ForestCeilings;
            }
        }

        private static Image[] _keen4PyramidCeilings;

        public static Image[] Keen4PyramidCeilings
        {
            get
            {
                if (_keen4PyramidCeilings == null)
                {
                    _keen4PyramidCeilings = new Image[]
                    {
                        Properties.Resources.keen4_pyramid_wall_bottom1,
                        Properties.Resources.keen4_pyramid_wall_bottom2
                    };
                }
                return _keen4PyramidCeilings;
            }
        }

        private static Image[] _keen4MirageCeilings;

        public static Image[] Keen4MirageCeilings
        {
            get
            {
                if (_keen4MirageCeilings == null)
                {
                    _keen4MirageCeilings = new Image[]
                    {
                        Properties.Resources.keen4_mirage_wall_bottom1,
                        Properties.Resources.keen4_mirage_wall_bottom2,
                        Properties.Resources.keen4_mirage_wall_bottom3,
                    };
                }
                return _keen4MirageCeilings;
            }
        }
        #endregion

        #region keen5

        private static Image[] _keen5BlackCeilings;

        public static Image[] Keen5BlackCeilings
        {
            get
            {
                if (_keen5BlackCeilings == null)
                {
                    _keen5BlackCeilings = new Image[]
                    {
                        Properties.Resources.keen5_wall_black_bottom
                    };
                }
                return _keen5BlackCeilings;
            }
        }

        private static Image[] _keen5RedCeilings;

        public static Image[] Keen5RedCeilings
        {
            get
            {
                if (_keen5RedCeilings == null)
                {
                    _keen5RedCeilings = new Image[]
                    {
                        Properties.Resources.keen5_ceiling_red
                    };
                }
                return _keen5RedCeilings;
            }
        }

        private static Image[] _keen5GreenCeilings;

        public static Image[] Keen5GreenCeilings
        {
            get
            {
                if (_keen5GreenCeilings == null)
                {
                    _keen5GreenCeilings = new Image[]
                    {
                        Properties.Resources.keen5_ceiling_green
                    };
                }
                return _keen5GreenCeilings;
            }
        }


        #endregion

        #region keen6

        private static Image[] _keen6DomeCeilings;

        public static Image[] Keen6DomeCeilings
        {
            get
            {
                if (_keen6DomeCeilings == null)
                {
                    _keen6DomeCeilings = new Image[]
                    {
                        Properties.Resources.keen6_dome_ceiling1,
                        Properties.Resources.keen6_dome_ceiling2,
                        Properties.Resources.keen6_dome_ceiling3,
                    };
                }
                return _keen6DomeCeilings;
            }
        }

        private static Image[] _keen6ForestCeilings;

        public static Image[] Keen6ForestCeilings
        {
            get
            {
                if (_keen6ForestCeilings == null)
                {
                    _keen6ForestCeilings = new Image[]
                    {
                        Properties.Resources.keen6_forest_ceiling1,
                        Properties.Resources.keen6_forest_ceiling2,
                    };
                }
                return _keen6ForestCeilings;
            }
        }

        private static Image[] _keen6IndustrialCeilings;

        public static Image[] Keen6IndustrialCeilings
        {
            get
            {
                if (_keen6IndustrialCeilings == null)
                {
                    _keen6IndustrialCeilings = new Image[]
                    {
                        Properties.Resources.keen6_industrial_ceiling
                    };
                }
                return _keen6IndustrialCeilings;
            }
        }

        #endregion

        #endregion

        #region gems

        private static Image[] _redGemImages;

        public static Image[] RedGemImages
        {
            get
            {
                if (_redGemImages == null)
                {
                    _redGemImages = new Image[]
                    {
                        Properties.Resources.gem_red1,
                        Properties.Resources.gem_red2
                    };
                }
                return _redGemImages;
            }
        }

        private static Image[] _blueGemImages;

        public static Image[] BlueGemImages
        {
            get
            {
                if (_blueGemImages == null)
                {
                    _blueGemImages = new Image[]
                    {
                        Properties.Resources.gem_blue1,
                        Properties.Resources.gem_blue2
                    };
                }
                return _blueGemImages;
            }
        }

        private static Image[] _greenGemImages;

        public static Image[] GreenGemImages
        {
            get
            {
                if (_greenGemImages == null)
                {
                    _greenGemImages = new Image[]
                    {
                        Properties.Resources.gem_green1,
                        Properties.Resources.gem_green2
                    };
                }
                return _greenGemImages;
            }
        }

        private static Image[] _yellowGemImages;

        public static Image[] YellowGemImages
        {
            get
            {
                if (_yellowGemImages == null)
                {
                    _yellowGemImages = new Image[]
                    {
                        Properties.Resources.gem_yellow1,
                        Properties.Resources.gem_yellow2
                    };
                }
                return _yellowGemImages;
            }
        }

        private static Image[] _gemAcquiredImages;

        public static Image[] GemAcquiredImages
        {
            get
            {
                if (_gemAcquiredImages == null)
                {
                    _gemAcquiredImages = new Image[]
                    {
                        Properties.Resources.gem_acquired
                    };
                }
                return _gemAcquiredImages;
            }
        }

        #endregion

        #region Items

        #region Points

        #region Keen4

        private static Image[] _keen4ShikadiSodaImages;

        public static Image[] Keen4ShikadiSodaImages
        {
            get
            {
                if (_keen4ShikadiSodaImages == null)
                {
                    _keen4ShikadiSodaImages = new Image[]
                    {
                        Properties.Resources.keen4_shikadi_soda1,
                        Properties.Resources.keen4_shikadi_soda2
                    };
                }
                return _keen4ShikadiSodaImages;
            }
        }


        private static Image[] _keen43ToothGum;

        public static Image[] Keen43ToothGum
        {
            get
            {
                if (_keen43ToothGum == null)
                {
                    _keen43ToothGum = new Image[]
                    {
                        Properties.Resources.keen4_three_tooth_gum1,
                        Properties.Resources.keen4_three_tooth_gum2
                    };
                }
                return _keen43ToothGum;
            }
        }

        private static Image[] _keen4ShikkersCandyBar;

        public static Image[] Keen4ShikkersCandyBar
        {
            get
            {
                if (_keen4ShikkersCandyBar == null)
                {
                    _keen4ShikkersCandyBar = new Image[]
                    {
                        Properties.Resources.keen4_candy_bar1,
                        Properties.Resources.keen4_candy_bar2
                    };
                }
                return _keen4ShikkersCandyBar;
            }
        }

        private static Image[] _keen4JawBreaker;

        public static Image[] Keen4JawBreaker
        {
            get
            {
                if (_keen4JawBreaker == null)
                {
                    _keen4JawBreaker = new Image[]
                    {
                        Properties.Resources.keen4_jawbreaker1,
                        Properties.Resources.keen4_jawbreaker2
                    };
                }
                return _keen4JawBreaker;
            }
        }


        private static Image[] _keen4Doughnut;
        public static Image[] Keen4Doughnut
        {
            get
            {
                if (_keen4Doughnut == null)
                {
                    _keen4Doughnut = new Image[]
                    {
                        Properties.Resources.keen4_doughnut1,
                        Properties.Resources.keen4_doughnut1
                    };
                }
                return _keen4Doughnut;
            }
        }


        private static Image[] _keen4IceCreamCone;
        public static Image[] Keen4IceCreamCone
        {
            get
            {
                if (_keen4IceCreamCone == null)
                {
                    _keen4IceCreamCone = new Image[]
                    {
                        Properties.Resources.keen4_icecream_cone1,
                        Properties.Resources.keen4_icecream_cone2
                    };
                }
                return _keen4IceCreamCone;
            }
        }

        #endregion

        #region Keen5
        private static Image[] _keen5ShikadiGum;
        public static Image[] Keen5ShikadiGum
        {
            get
            {
                if (_keen5ShikadiGum == null)
                {
                    _keen5ShikadiGum = new Image[]
                    {
                        Properties.Resources.keen5_shikadi_gum1,
                        Properties.Resources.keen5_shikadi_gum2
                    };
                }
                return _keen5ShikadiGum;
            }
        }

        private static Image[] _keen5Marshmallow;
        public static Image[] Keen5Marshmallow
        {
            get
            {
                if (_keen5Marshmallow == null)
                {
                    _keen5Marshmallow = new Image[]
                    {
                                   Properties.Resources.keen5_marshmallow1,
                        Properties.Resources.keen5_marshmallow2
                    };
                }
                return _keen5Marshmallow;
            }
        }

        private static Image[] _keen5ChocolateMilk;
        public static Image[] Keen5ChocolateMilk
        {
            get
            {
                if (_keen5ChocolateMilk == null)
                {
                    _keen5ChocolateMilk = new Image[]
                    {
                        Properties.Resources.keen5_chocolate_milk1,
                        Properties.Resources.keen5_chocolate_milk2
                    };
                }
                return _keen5ChocolateMilk;
            }
        }

        private static Image[] _keen5TartStix;
        public static Image[] Keen5TartStix
        {
            get
            {
                if (_keen5TartStix == null)
                {
                    _keen5TartStix = new Image[]
                    {
                         Properties.Resources.keen5_tart_stix1,
                        Properties.Resources.keen5_tart_stix1
                    };
                }
                return _keen5TartStix;
            }
        }


        private static Image[] _keen5SugarStoopiesCereal;
        public static Image[] Keen5SugarStoopiesCereal
        {
            get
            {
                if (_keen5SugarStoopiesCereal == null)
                {
                    _keen5SugarStoopiesCereal = new Image[]
                    {
                        Properties.Resources.keen5_sugar_stoopies_cereal1,
                        Properties.Resources.keen5_sugar_stoopies_cereal2
                    };
                }
                return _keen5SugarStoopiesCereal;
            }
        }

        private static Image[] _keen5BagOSugar;
        public static Image[] Keen5BagOSugar
        {
            get
            {
                if (_keen5BagOSugar == null)
                {
                    _keen5BagOSugar = new Image[]
                    {
                        Properties.Resources.keen5_bag_o_sugar1,
                        Properties.Resources.keen5_bag_o_sugar2
                    };
                }
                return _keen5BagOSugar;
            }
        }

        #endregion

        #region Keen6
        private static Image[] _keen6BloogSoda;
        public static Image[] Keen6BloogSoda
        {
            get
            {
                if (_keen6BloogSoda == null)
                {
                    _keen6BloogSoda = new Image[]
                    {
                        Properties.Resources.keen6_bloog_soda1,
                        Properties.Resources.keen6_bloog_soda2
                    };
                }
                return _keen6BloogSoda;
            }
        }

        private static Image[] _keen6IceCreamBar;
        public static Image[] Keen6IceCreamBar
        {
            get
            {
                if (_keen6IceCreamBar == null)
                {
                    _keen6IceCreamBar = new Image[]
                    {
                        Properties.Resources.keen6_ice_cream_bar1,
                        Properties.Resources.keen6_ice_cream_bar2
                    };
                }
                return _keen6IceCreamBar;
            }
        }

        private static Image[] _keen6Pudding;
        public static Image[] Keen6Pudding
        {
            get
            {
                if (_keen6Pudding == null)
                {
                    _keen6Pudding = new Image[]
                    {
                        Properties.Resources.keen6_pudding1,
                        Properties.Resources.keen6_pudding2
                    };
                }
                return _keen6Pudding;
            }
        }

        private static Image[] _keen6RootBeerFloat;
        public static Image[] Keen6RootBeerFloat
        {
            get
            {
                if (_keen6RootBeerFloat == null)
                {
                    _keen6RootBeerFloat = new Image[]
                    {
                        Properties.Resources.keen6_root_beer_float1,
                        Properties.Resources.keen6_root_beer_float2
                    };
                }
                return _keen6RootBeerFloat;
            }
        }

        private static Image[] _keen6BananaSplit;
        public static Image[] Keen6BananaSplit
        {
            get
            {
                if (_keen6BananaSplit == null)
                {
                    _keen6BananaSplit = new Image[]
                    {
                        Properties.Resources.keen6_banana_split1,
                        Properties.Resources.keen6_banana_split2
                    };
                }
                return _keen6BananaSplit;
            }
        }

        private static Image[] _keen6PizzaSlice;
        public static Image[] Keen6PizzaSlice
        {
            get
            {
                if (_keen6PizzaSlice == null)
                {
                    _keen6PizzaSlice = new Image[]
                    {
                        Properties.Resources.keen6_pizza_slice1,
                        Properties.Resources.keen6_pizza_slice2
                    };
                }
                return _keen6PizzaSlice;
            }
        }
        #endregion

        #region Point Number Labels

        private static Image[] _keenPoints100;

        public static Image[] KeenPoints100
        {
            get
            {
                if (_keenPoints100 == null)
                {
                    _keenPoints100 = new Image[]
                    {
                        Properties.Resources.keen_points_100
                    };
                }
                return _keenPoints100;
            }
        }

        private static Image[] _keenPoints200;

        public static Image[] KeenPoints200
        {
            get
            {
                if (_keenPoints200 == null)
                {
                    _keenPoints200 = new Image[]
                    {
                        Properties.Resources.keen_points_200
                    };
                }
                return _keenPoints200;
            }
        }

        private static Image[] _keenPoints500;

        public static Image[] KeenPoints500
        {
            get
            {
                if (_keenPoints500 == null)
                {
                    _keenPoints500 = new Image[]
                    {
                        Properties.Resources.keen_points_500
                    };
                }
                return _keenPoints500;
            }
        }

        private static Image[] _keenPoints1000;

        public static Image[] KeenPoints1000
        {
            get
            {
                if (_keenPoints1000 == null)
                {
                    _keenPoints1000 = new Image[]
                    {
                        Properties.Resources.keen_points_1000
                    };
                }
                return _keenPoints1000;
            }
        }

        private static Image[] _keenPoints2000;

        public static Image[] KeenPoints2000
        {
            get
            {
                if (_keenPoints2000 == null)
                {
                    _keenPoints2000 = new Image[]
                    {
                        Properties.Resources.keen_points_2000
                    };
                }
                return _keenPoints2000;
            }
        }

        private static Image[] _keenPoints5000;

        public static Image[] KeenPoints5000
        {
            get
            {
                if (_keenPoints5000 == null)
                {
                    _keenPoints5000 = new Image[]
                    {
                        Properties.Resources.keen_points_5000
                    };
                }
                return _keenPoints5000;
            }
        }

        #endregion

        #endregion

        #region life drops

        #region keen 4 rain drops

        private static Image[] _keen4RainDropImages;

        public static Image[] Keen4RainDropImages
        {
            get
            {
                if (_keen4RainDropImages == null)
                {
                    _keen4RainDropImages = new Image[]
                    {
                        Properties.Resources.keen4_drop1,
                        Properties.Resources.keen4_drop2,
                        Properties.Resources.keen4_drop3
                    };
                }
                return _keen4RainDropImages;
            }
        }


        private static Image[] _keen4RainDropAcquiredImages;

        public static Image[] Keen4RainDropAcquiredImages
        {
            get
            {
                if (_keen4RainDropAcquiredImages == null)
                {
                    _keen4RainDropAcquiredImages = new Image[]
                    {
                        Properties.Resources.keen4_drop_acquired1,
                        Properties.Resources.keen4_drop_acquired2,
                        Properties.Resources.keen4_drop_acquired3
                    };
                }
                return _keen4RainDropAcquiredImages;
            }
        }

        #endregion

        #region keen 5 vitalin
        private static Image[] _keen5VitalinImages;

        public static Image[] Keen5VitalinImages
        {
            get
            {
                if (_keen5VitalinImages == null)
                {
                    _keen5VitalinImages = new Image[]
                    {
                        Properties.Resources.keen5_vitalin1,
                        Properties.Resources.keen5_vitalin2,
                        Properties.Resources.keen5_vitalin3,
                        Properties.Resources.keen5_vitalin4
                    };
                }

                return _keen5VitalinImages;
            }
        }

        private static Image[] _keen5VitalinAcquiredImages;

        public static Image[] Keen5VitalinAcquiredImages
        {
            get
            {
                if (_keen5VitalinAcquiredImages == null)
                {
                    _keen5VitalinAcquiredImages = new Image[]
                    {
                        Properties.Resources.keen5_vitalin_acquired1,
                        Properties.Resources.keen5_vitalin_acquired2,
                        Properties.Resources.keen5_vitalin_acquired3,
                        Properties.Resources.keen5_vitalin_acquired4
                    };
                }

                return _keen5VitalinAcquiredImages;
            }
        }
        #endregion

        #region keen 6 viva

        private static Image[] _vivaFlying;

        public static Image[] VivaFlying
        {
            get
            {
                if (_vivaFlying == null)
                {
                    _vivaFlying = new Image[]
                    {
                        Properties.Resources.keen6_viva_flying1,
                        Properties.Resources.keen6_viva_flying2,
                        Properties.Resources.keen6_viva_flying3
                    };
                }
                return _vivaFlying;
            }
        }

        private static Image[] _vivaPerched;

        public static Image[] VivaPerched
        {
            get
            {
                if (_vivaPerched == null)
                {
                    _vivaPerched = new Image[]
                    {
                        Properties.Resources.keen6_viva_perched1,
                        Properties.Resources.keen6_viva_perched2,
                        Properties.Resources.keen6_viva_perched3,
                        Properties.Resources.keen6_viva_perched4
                    };
                }
                return _vivaPerched;
            }
        }

        private static Image[] _vivaAcquired;

        public static Image[] VivaAcquired
        {
            get
            {
                if (_vivaAcquired == null)
                {
                    _vivaAcquired = new Image[]
                    {
                        Properties.Resources.keen6_viva_acquired1,
                        Properties.Resources.keen6_viva_acquired2,
                        Properties.Resources.keen6_viva_acquired3,
                        Properties.Resources.keen6_viva_acquired4
                    };
                }
                return _vivaAcquired;
            }
        }

        #endregion

        #endregion

        #region extra lives
        private static Image[] _keen4LifeWaterFlaskImages;

        public static Image[] Keen4LifeWaterFlaskImages
        {
            get
            {
                if (_keen4LifeWaterFlaskImages == null)
                {
                    _keen4LifeWaterFlaskImages = new Image[]
                    {
                        Properties.Resources.keen4_lifewater_flask1,
                        Properties.Resources.keen4_lifewater_flask2
                    };
                }
                return _keen4LifeWaterFlaskImages;
            }
        }


        private static Image[] _keen5KegOVitalin;

        public static Image[] Keen5KegOVitalin
        {
            get
            {
                if (_keen5KegOVitalin == null)
                {
                    _keen5KegOVitalin = new Image[]
                    {
                        Properties.Resources.keen5_keg_o_vitalin1,
                        Properties.Resources.keen5_keg_o_vitalin2
                    };
                }
                return _keen5KegOVitalin;
            }
        }

        private static Image[] _keen6VivaQueenImages;

        public static Image[] Keen6VivaQueenImages
        {
            get
            {
                if (_keen6VivaQueenImages == null)
                {
                    _keen6VivaQueenImages = new Image[]
                    {
                        Properties.Resources.keen6_viva_queen2,
                        Properties.Resources.keen6_viva_queen1
                    };
                }
                return _keen6VivaQueenImages;
            }
        }
        #endregion

        #region Keen 5 KeyCard
        private static Image[] _keen5KeyCardImages;
        public static Image[] Keen5KeyCardImages
        {
            get
            {
                if (_keen5KeyCardImages == null)
                {
                    _keen5KeyCardImages = new Image[]
                    {
                        Properties.Resources.keen5_key_card1,
                        Properties.Resources.keen5_key_card2
                    };
                }
                return _keen5KeyCardImages;
            }
        }

        private static Image[] _keen5KeyCardAcquiredImages;
        public static Image[] Keen5KeyCardAcquiredImages
        {
            get
            {
                if (_keen5KeyCardAcquiredImages == null)
                {
                    _keen5KeyCardAcquiredImages = new Image[]
                    {
                        Properties.Resources.keen5_key_card_acquired
                    };
                }
                return _keen5KeyCardAcquiredImages;
            }
        }

        #endregion

        #region CTF Game Mode

        private static Image[] _ctfColors;

        public static Image[] CTFColors
        {
            get
            {
                if (_ctfColors == null)
                {
                    _ctfColors = new Image[]
                    {
                        Properties.Resources.Red_Flag,
                        Properties.Resources.Blue_Flag,
                        Properties.Resources.Green_Flag,
                        Properties.Resources.Yellow_Flag
                    };
                }
                return _ctfColors;
            }
        }

        private static Image[] _ctfDestinations;

        public static Image[] CTFDestinations
        {
            get
            {
                if (_ctfDestinations == null)
                {
                    _ctfDestinations = new Image[]
                    {
                        Properties.Resources.red_flag_destination,
                        Properties.Resources.blue_flag_destination,
                        Properties.Resources.green_flag_destination,
                        Properties.Resources.yellow_flag_destination
                    };
                }
                return _ctfDestinations;
            }
        }

        #endregion

        #region Keen 5 Exit Door

        private static Image[] _keen5ExitDoorOpenImages;

        public static Image[] Keen5ExitDoorOpenImages
        {
            get
            {
                if (_keen5ExitDoorOpenImages == null)
                {
                    _keen5ExitDoorOpenImages = new Image[]
                    {
                        Properties.Resources.keen5_exit_door_open1,
                        Properties.Resources.keen5_exit_door_open2,
                        Properties.Resources.keen5_exit_door_open3
                    };
                }
                return _keen5ExitDoorOpenImages;
            }
        }

        #endregion

        #endregion

        #region Hazards
        #region Keen 4 Poison Pool

        private static Image[] _keen4PoisonPoolLeftImages;

        public static Image[] Keen4PoisonPoolLeftImages
        {
            get
            {
                if (_keen4PoisonPoolLeftImages == null)
                {
                    _keen4PoisonPoolLeftImages = new Image[]
                    {
                        Properties.Resources.keen4_poison_pool1_edge_left,
                        Properties.Resources.keen4_poison_pool2_edge_left
                    };
                }
                return _keen4PoisonPoolLeftImages;
            }
        }

        private static Image[] _keen4PoisonPoolRightImages;

        public static Image[] Keen4PoisonPoolRightImages
        {
            get
            {
                if (_keen4PoisonPoolRightImages == null)
                {
                    _keen4PoisonPoolRightImages = new Image[]
                    {
                        Properties.Resources.keen4_poison_pool1_edge_right,
                        Properties.Resources.keen4_poison_pool2_edge_right
                    };
                }
                return _keen4PoisonPoolRightImages;
            }
        }

        private static Image[] _keen4PoisonPoolMiddleImages;

        public static Image[] Keen4PoisonPoolMiddleImages
        {
            get
            {
                if (_keen4PoisonPoolMiddleImages == null)
                {
                    _keen4PoisonPoolMiddleImages = new Image[]
                    {
                        Properties.Resources.keen4_poison_pool1_middle,
                        Properties.Resources.keen4_poison_pool2_middle
                    };
                }
                return _keen4PoisonPoolMiddleImages;
            }
        }
        #endregion

        #region Keen 4 TarPit
        private static Image[] _keen4TarPitImages;

        public static Image[] Keen4TarPitImages
        {
            get
            {
                if (_keen4TarPitImages == null)
                {
                    _keen4TarPitImages = new Image[]
                    {
                         Properties.Resources.keen4_tar1,
                         Properties.Resources.keen4_tar2,
                         Properties.Resources.keen4_tar3,
                         Properties.Resources.keen4_tar4
                    };
                }
                return _keen4TarPitImages;
            }
        }
        #endregion

        #region Keen 4 Rocket-Propelled Platform
        private static Image[] _rocketPropelledPlatformImages;

        public static Image[] RocketPropelledPlatformImages
        {
            get
            {
                if (_rocketPropelledPlatformImages == null)
                {
                    _rocketPropelledPlatformImages = new Image[]{
                        Properties.Resources.keen4_rocket_propelled_platform1,
                        Properties.Resources.keen4_rocket_propelled_platform2,
                        Properties.Resources.keen4_rocket_propelled_platform3,
                        Properties.Resources.keen4_rocket_propelled_platform4
                    };
                }
                return _rocketPropelledPlatformImages;
            }
        }
        #endregion

        #region Keen 5 Spinning Fire
        private static Image[] _keen5SpinningFireImages;
        public static Image[] Keen5SpinningFireImages
        {
            get
            {
                if (_keen5SpinningFireImages == null)
                {
                    _keen5SpinningFireImages = new Image[]
                    {
                        Properties.Resources.keen5_spinning_fire_hazard1,
                        Properties.Resources.keen5_spinning_fire_hazard2,
                        Properties.Resources.keen5_spinning_fire_hazard3,
                        Properties.Resources.keen5_spinning_fire_hazard4,
                        Properties.Resources.keen5_spinning_fire_hazard5,
                        Properties.Resources.keen5_spinning_fire_hazard6,
                        Properties.Resources.keen5_spinning_fire_hazard7
                    };
                }
                return _keen5SpinningFireImages;
            }
        }
        #endregion


        #region Keen5 Spinning Burn Platform

        private static Image[] _spinningBurnPlatformImages;

        public static Image[] SpinningBurnPlatformImages
        {
            get
            {
                if (_spinningBurnPlatformImages == null)
                {
                    _spinningBurnPlatformImages = new Image[]
                    {
                        Properties.Resources.keen5_spinning_burn_platform1,
                        Properties.Resources.keen5_spinning_burn_platform2,
                        Properties.Resources.keen5_spinning_burn_platform3,
                        Properties.Resources.keen5_spinning_burn_platform4,
                        Properties.Resources.keen5_spinning_burn_platform5,
                        Properties.Resources.keen5_spinning_burn_platform6,
                        Properties.Resources.keen5_spinning_burn_platform7,
                        Properties.Resources.keen5_spinning_burn_platform8
                    };
                }
                return _spinningBurnPlatformImages;
            }
        }

        #endregion

        #region Keen 6 Burn Hazard

        private static Image[] _keen6BurnHazardImages;

        public static Image[] Keen6BurnHazardImages
        {
            get
            {
                if (_keen6BurnHazardImages == null)
                {
                    _keen6BurnHazardImages = new Image[]
                    {
                        Properties.Resources.keen6_burn_hazard1,
                        Properties.Resources.keen6_burn_hazard2,
                        Properties.Resources.keen6_burn_hazard3,
                        Properties.Resources.keen6_burn_hazard4
                    };
                }
                return _keen6BurnHazardImages;
            }
        }

        #endregion

        #region Keen 6 Drill
        private static Image[] _keen6DrillSprites;

        public static Image[] Keen6DrillSprites
        {
            get
            {
                if (_keen6DrillSprites == null)
                {
                    _keen6DrillSprites = new Image[]
                    {
                         Properties.Resources.keen6_drill1,
                         Properties.Resources.keen6_drill2,
                         Properties.Resources.keen6_drill3,
                         Properties.Resources.keen6_drill4
                    };
                }
                return _keen6DrillSprites;
            }
        }
        #endregion

        #region Keen 6 Conveyer Belt

        private static Image[] _keen6ConveyerBeltLeftSprites;

        public static Image[] Keen6ConveyerBeltLeftSprites
        {
            get
            {
                if (_keen6ConveyerBeltLeftSprites == null)
                {
                    _keen6ConveyerBeltLeftSprites = new Image[]
                    {
                        Properties.Resources.keen6_conveyer_belt_left1,
                        Properties.Resources.keen6_conveyer_belt_left2,
                        Properties.Resources.keen6_conveyer_belt_left3,
                        Properties.Resources.keen6_conveyer_belt_left4,
                    };
                }
                return _keen6ConveyerBeltLeftSprites;
            }
        }

        private static Image[] _keen6ConveyerBeltRightSprites;

        public static Image[] Keen6ConveyerBeltRightSprites
        {
            get
            {
                if (_keen6ConveyerBeltRightSprites == null)
                {
                    _keen6ConveyerBeltRightSprites = new Image[]
                    {
                        Properties.Resources.keen6_conveyer_belt_right1,
                        Properties.Resources.keen6_conveyer_belt_right2,
                        Properties.Resources.keen6_conveyer_belt_right3,
                        Properties.Resources.keen6_conveyer_belt_right4,
                    };
                }
                return _keen6ConveyerBeltRightSprites;
            }
        }

        private static Image[] _keen6ConveyerBeltMiddleSprites;

        public static Image[] Keen6ConveyerBeltMiddleSprites
        {
            get
            {
                if (_keen6ConveyerBeltMiddleSprites == null)
                {
                    _keen6ConveyerBeltMiddleSprites = new Image[]
                    {
                        Properties.Resources.keen6_conveyer_belt_middle1,
                        Properties.Resources.keen6_conveyer_belt_middle2,
                        Properties.Resources.keen6_conveyer_belt_middle3,
                        Properties.Resources.keen6_conveyer_belt_middle4,
                    };
                }
                return _keen6ConveyerBeltMiddleSprites;
            }
        }

        #endregion

        #region Keen 6 Flame Thrower
        private static Image[] _flameThrowerBurnImages;

        public static Image[] Keen6FlameThrowerBurnImages
        {
            get
            {
                if (_flameThrowerBurnImages == null)
                {
                    _flameThrowerBurnImages = new Image[]
                    {
                        Properties.Resources.keen6_flame_thrower_off,
                        Properties.Resources.keen6_flame_thrower_on1,
                        Properties.Resources.keen6_flame_thrower_on2,
                        Properties.Resources.keen6_flame_thrower_on3,
                        Properties.Resources.keen6_flame_thrower_on2,
                        Properties.Resources.keen6_flame_thrower_on1
                    };
                }
                return _flameThrowerBurnImages;
            }
        }
        #endregion

        #region Keen 6 LaserField
        private static Image[] _keen6LaserFieldImages;

        public static Image[] Keen6LaserFieldImages
        {
            get
            {
                if (_keen6LaserFieldImages == null)
                {
                    _keen6LaserFieldImages = new Image[]{
                        Properties.Resources.keen6_laser_field_laser1,
                        Properties.Resources.keen6_laser_field_laser2,
                        Properties.Resources.keen6_laser_field_laser3
                    };
                }
                return _keen6LaserFieldImages;
            }
        }
        #endregion

        #region Keen 6 Slime Hazard
        private static Image[] _keen6SlimeHazardLeftImages;

        public static Image[] Keen6SlimeHazardLeftImages
        {
            get
            {
                if (_keen6SlimeHazardLeftImages == null)
                {
                    _keen6SlimeHazardLeftImages = new Image[]
                    {
                        Properties.Resources.keen6_slime_hazard_left1,
                        Properties.Resources.keen6_slime_hazard_left2,
                        Properties.Resources.keen6_slime_hazard_left3
                    };
                }
                return _keen6SlimeHazardLeftImages;
            }
        }

        private static Image[] _keen6SlimeHazardRightImages;

        public static Image[] Keen6SlimeHazardRightImages
        {
            get
            {
                if (_keen6SlimeHazardRightImages == null)
                {
                    _keen6SlimeHazardRightImages = new Image[]
                    {
                        Properties.Resources.keen6_slime_hazard_right1,
                        Properties.Resources.keen6_slime_hazard_right2,
                        Properties.Resources.keen6_slime_hazard_right3
                    };
                }
                return _keen6SlimeHazardRightImages;
            }
        }

        private static Image[] _keen6SlimeHazardMiddleImages;

        public static Image[] Keen6SlimeHazardMiddleImages
        {
            get
            {
                if (_keen6SlimeHazardMiddleImages == null)
                {
                    _keen6SlimeHazardMiddleImages = new Image[]
                    {
                        Properties.Resources.keen6_slime_hazard_middle1,
                        Properties.Resources.keen6_slime_hazard_middle2,
                        Properties.Resources.keen6_slime_hazard_middle3
                    };
                }
                return _keen6SlimeHazardMiddleImages;
            }
        }
        #endregion

        #region Keen 6 Electric Rods
        private static Image[] _keen6ElectricRodSprites;

        public static Image[] Keen6ElectricRodSprites
        {
            get
            {
                if (_keen6ElectricRodSprites == null)
                {
                    _keen6ElectricRodSprites = new Image[]
                    {
                        Properties.Resources.keen6_electric_rods1,
                        Properties.Resources.keen6_electric_rods2
                    };
                }
                return _keen6ElectricRodSprites;
            }
        }
        #endregion
        #endregion

        #region projectiles and explosions

        private static Image[] _rpgExplosionImages;

        public static Image[] RPGExplosionSprites
        {
            get
            {
                if (_rpgExplosionImages == null)
                {
                    _rpgExplosionImages = new Image[]
                    {
                        Properties.Resources.keen_stun_shot_hit1,
                        Properties.Resources.keen_stun_shot_hit1,
                        Properties.Resources.keen_stun_shot_hit1,
                        Properties.Resources.keen_stun_shot_hit2,
                    };
                }
                return _rpgExplosionImages;
            }
        }

        #endregion
    }
}
