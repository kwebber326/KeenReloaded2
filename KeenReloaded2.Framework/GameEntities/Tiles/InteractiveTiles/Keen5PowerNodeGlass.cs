using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles
{
    public class Keen5PowerNodeGlass : Keen5GeneratorGlass
    {
        private readonly IActivator _parentActivator;

        public Keen5PowerNodeGlass(SpaceHashGrid grid, Rectangle hitbox, int zIndex, IActivator parentActivator)
            : base(grid, hitbox, zIndex)
        {
            _parentActivator = parentActivator;
        }

        public override ObjectiveEventType EventType => ObjectiveEventType.DEACTIVATE;

        public bool IsActive => !_isDead;

        

        public event EventHandler<ToggleEventArgs> Toggled;

        public void Toggle()
        {
            _isDead = true;
            _parentActivator.Toggle();
        }

        public override bool Equals(object obj)
        {
            return obj is Keen5PowerNodeGlass && base.Equals(obj);
        }

        public override void Update()
        {
            if (!_isDead && _unbroken)
            {
                UpdateSprite();
            }
            else
            {
                _keen = GetClosestAlivePlayer();
                if (_keen != null)
                {
                    _sprite = Properties.Resources.keen5_destructible_glass_tile_destroyed;
                    this.PublishSoundPlayEvent(
                        GeneralGameConstants.Sounds.GLASS_BREAK);
                    PerformActionForEvent();
                    _isDead = true;
                }
            }
        }

        protected override void PerformActionForEvent()
        {
           this.Toggle();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
