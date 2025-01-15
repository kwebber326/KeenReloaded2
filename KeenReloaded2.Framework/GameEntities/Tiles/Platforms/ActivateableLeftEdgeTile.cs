using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class ActivateableLeftEdgeTile : LeftEdgeFloorTile, IActivateable
    {
        private bool _isActive;
        private readonly Guid _activationId;

        public ActivateableLeftEdgeTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, string biome, bool isActive, Guid activationId) 
            : base(area, grid, hitbox, imageFile, zIndex, biome)
        {
            _isActive = isActive;
            _activationId = activationId;
            this.SetImageFromBiome();
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            protected set
            {
                _isActive = value;
                this.SetImageFromBiome();
            }
        }


        public Guid ActivationID => _activationId;

        public void Activate()
        {
            this.IsActive = true;
        }

        public void Deactivate()
        {
            this.IsActive = false;
        }

        protected override void SetImageFromBiome()
        {
            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_CAVE:
                case Biomes.BIOME_KEEN4_FOREST:
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _image = _isActive 
                        ? Properties.Resources.keen4_removable_platform_left_edge_filled
                        : Properties.Resources.keen4_removable_platform_left_edge;
                    break;
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _image = _isActive
                        ? Properties.Resources.keen4_removable_platform_left_edge_filled_pyramid
                        : Properties.Resources.keen4_removable_platform_left_edge_pyramid;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _image = _isActive
                         ? Properties.Resources.keen5_removable_platform_left_edge_filled_black
                         : Properties.Resources.keen5_removable_platform_left_edge_black;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _image = _isActive
                         ? Properties.Resources.keen5_removable_platform_left_edge_filled_green
                         : Properties.Resources.keen5_removable_platform_left_edge_green;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _image = _isActive
                         ? Properties.Resources.keen5_removable_platform_left_edge_filled_red
                         : Properties.Resources.keen5_removable_platform_left_edge_red;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _image = _isActive
                        ? Properties.Resources.keen6_removable_platform_left_edge_filled_dome
                        : Properties.Resources.keen6_removable_platform_left_edge_dome;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _image = _isActive
                        ? Properties.Resources.keen6_removable_platform_forest_left_edge_filled
                        : Properties.Resources.keen6_removable_platform_forest_left_edge;
                    break;
            }
        }

        public override bool CanUpdate => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

            return $"{_initialImageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_biome}{separator}{IsActive}{separator}{_activationId}";
        }
    }
}
