using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints
{
    public class Keen4RedWorldMapCheckPoint : Keen4RedCheckPoint, IActivator
    {
        private readonly List<IActivateable> _toggleObjects;
        private bool _isActive;

        public Keen4RedWorldMapCheckPoint(Rectangle area, SpaceHashGrid grid, int zIndex,
             IActivateable[] toggleObjects)
            : base(area, grid, zIndex)
        {
            _toggleObjects = toggleObjects.ToList();
            _isActive = true;
        }

        public List<IActivateable> ToggleObjects => _toggleObjects;

        public bool IsActive => _isActive;

        public event EventHandler<ToggleEventArgs> Toggled;

        public void Toggle()
        {
            _isActive = false;
            foreach (var obj in _toggleObjects)
            {
                obj.Deactivate();
            }
            this.MarkAsHit();
            Toggled?.Invoke(this, new ToggleEventArgs { IsActive = _isActive });
        }

        public override void MarkAsHit()
        {
            if (!_checkPointHit)
            {
                _checkPointHit = true;
                this.PublishSoundPlayEvent(GeneralGameConstants.Sounds.CHECKPOINT);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            Rectangle area = this.HitBox;
            string arraySeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;
            string activatorGuids = string.Join(arraySeparator, this.ToggleObjects.Select(t => t.ActivationID));
            string activatorStr = arrayStart + activatorGuids + arrayEnd;

            string imgName = nameof(Properties.Resources.keen4_flag_base_red1) + "_wm";

            return $"{imgName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{activatorStr}";
        }
    }
}
