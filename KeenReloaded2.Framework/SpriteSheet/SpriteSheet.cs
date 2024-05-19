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
    }
}
