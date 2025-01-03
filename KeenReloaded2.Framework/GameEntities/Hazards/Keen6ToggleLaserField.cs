using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen6ToggleLaserField : Keen6LaserField, IActivateable
    {
        private readonly Guid _activationId;
        private bool _isActive;

        public Keen6ToggleLaserField(Rectangle area, SpaceHashGrid grid, int zIndex, bool initiallyActive, Guid activationId)
            : base(area, grid, zIndex, initiallyActive ? LaserFieldState.PHASE1 : LaserFieldState.OFF)
        {
            _isActive = initiallyActive;
            _activationId = activationId;
        }

        public bool IsActive => _isActive;

        public Guid ActivationID => _activationId;

        public override void Update()
        {
            if (_state == LaserFieldState.PHASE1)
                this.UpdateLaserPhase();
        }

        protected override void UpdateLaserPhase()
        {
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSprite, SPRITE_CHANGE_DELAY, UpdateSprite);
            TryKillKeen();
        }

        public void Activate()
        {
            _isActive = true;
            this.State = LaserFieldState.PHASE1;
        }

        public void Deactivate()
        {
            _isActive = false;
            this.State = LaserFieldState.OFF;
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

            return $"{nameof(Properties.Resources.keen6_laser_field_toggle)}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_isActive}{separator}{_activationId}";
        }
    }
}
