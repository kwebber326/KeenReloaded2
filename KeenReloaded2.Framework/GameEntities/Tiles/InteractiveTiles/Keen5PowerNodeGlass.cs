using KeenReloaded.Framework;
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
    public class Keen5PowerNodeGlass : Keen5GeneratorGlass, IActivator
    {
        private readonly List<IActivateable> _toggleObjects;
        public Keen5PowerNodeGlass(SpaceHashGrid grid, Rectangle hitbox, int zIndex, IActivateable[] activateables)
            : base(grid, hitbox, zIndex)
        {
            _toggleObjects = activateables?.ToList() ?? new List<IActivateable>();
        }

        public override ObjectiveEventType EventType => ObjectiveEventType.DEACTIVATE;

        public List<IActivateable> ToggleObjects => _toggleObjects;

        public bool IsActive => !_isDead;

        public event EventHandler<ToggleEventArgs> Toggled;

        public void Toggle()
        {
            _isDead = true;
            if (_toggleObjects == null || !_toggleObjects.Any())
                return;

            foreach (var component in _toggleObjects)
            {
                component.Deactivate();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Keen5PowerNodeGlass && base.Equals(obj);
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
