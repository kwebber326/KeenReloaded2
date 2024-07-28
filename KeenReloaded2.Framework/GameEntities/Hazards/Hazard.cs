using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Hazard : CollisionObject, ISprite
    {
        private HazardType _type;
        protected Image _sprite;
        protected int _zIndex;
        protected string _imageName;
        public Hazard(SpaceHashGrid grid, Rectangle area, HazardType hazardType, int zIndex)
            : base(grid, area)
        {
            _type = hazardType;
            _zIndex = zIndex;
            SetSpriteFromType(_type);
            if (grid == null)
            {
                this.HitBox = area;
            }
        }

        protected virtual void SetSpriteFromType(HazardType type)
        {
            switch (type)
            {
                case HazardType.KEEN4_SPIKE:
                    _sprite = Properties.Resources.keen_4_spikes;
                    _imageName = nameof(Properties.Resources.keen_4_spikes);
                    break;
                case HazardType.KEEN4_MINE:
                    _sprite = Properties.Resources.keen4_mine;
                    _imageName = nameof(Properties.Resources.keen4_mine);
                    break;
                case HazardType.KEEN4_LIGHTNING_BOLT:
                    _sprite = Properties.Resources.keen4_lightning_bolt1;
                    _imageName = nameof(Properties.Resources.keen4_lightning_bolt1);
                    break;
                case HazardType.KEEN4_ROCKET_PROPELLED_PLATFORM:
                    _sprite = Properties.Resources.keen4_rocket_propelled_platform1;
                    _imageName = nameof(Properties.Resources.keen4_rocket_propelled_platform1);
                    break;
                case HazardType.KEEN4_FIRE:
                    _sprite = Properties.Resources.keen4_fire_left1;
                    _imageName = nameof(Properties.Resources.keen4_fire_left1);
                    break;
                case HazardType.KEEN4_SLUG_POOP:
                    _sprite = Properties.Resources.keen4_slug_poop_active;
                    _imageName = nameof(Properties.Resources.keen4_slug_poop_active);
                    break;
                case HazardType.KEEN5_SPINNING_FIRE:
                    _sprite = Properties.Resources.keen5_spinning_fire_hazard1;
                    _imageName = nameof(Properties.Resources.keen5_spinning_fire_hazard1);
                    break;
                case HazardType.KEEN5_SPINNING_BURN_PLATFORM:
                    _sprite = Properties.Resources.keen5_spinning_burn_platform7;
                    _imageName = nameof(Properties.Resources.keen5_spinning_burn_platform7);
                    break;
                case HazardType.KEEN6_BURN_HAZARD:
                    _sprite = Properties.Resources.keen6_burn_hazard1;
                    _imageName = nameof(Properties.Resources.keen6_burn_hazard1);
                    break;
                case HazardType.KEEN6_SPIKE:
                    _sprite = Properties.Resources.keen6_dome_spikes;
                    _imageName = nameof(Properties.Resources.keen6_dome_spikes);
                    break;
                case HazardType.KEEN6_DRILL:
                    _sprite = Properties.Resources.keen6_drill1;
                    _imageName = nameof(Properties.Resources.keen6_drill1);
                    break;
                case HazardType.KEEN6_ELECTRIC_RODS:
                    _sprite = Properties.Resources.keen6_electric_rods1;
                    _imageName = nameof(Properties.Resources.keen6_electric_rods1);
                    break;
            }
        }

        public virtual bool IsDeadly
        {
            get
            {
                return true;
            }
        }

        public void Kill(DestructibleObject obj)
        {
            obj.Die();
        }

        protected void HandleCollision(CollisionObject obj)
        {
            DestructibleObject destructoObj = obj as DestructibleObject;
            if (destructoObj != null)
            {
                this.Kill(destructoObj);
            }
        }

        public override CollisionType CollisionType => CollisionType.HAZARD;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{_imageName}{separator}{this.HitBox.X}{separator}{this.HitBox.Y}{separator}{this.HitBox.Width}{separator}{this.HitBox.Height}";
        }
    }
}
