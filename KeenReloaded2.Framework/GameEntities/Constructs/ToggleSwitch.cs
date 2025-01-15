using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using KeenReloaded2.Framework.Enums;
using KeenReloaded.Framework;
using KeenReloaded2.Framework.GameEventArgs;
using System.Timers;
using KeenReloaded2.Constants;
using KeenReloaded2.Utilities;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class ToggleSwitch : CollisionObject, IActivator, ISprite
    {
        private SwitchType _type;
        private bool _canToggle = true;
        Timer _toggleDelayTimer;

        private const int TOGGLE_DELAY_MILLISECONDS = 500;
        private readonly int _zIndex;

        public ToggleSwitch(Rectangle area, SpaceHashGrid grid, int zIndex, SwitchType type, IActivateable[] toggleObjects, bool isActive)
            : base(grid, area)
        {
            _isActive = isActive;
            _toggleObjects = toggleObjects.ToList();
            _type = type;
            this.HitBox = area;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            if (_toggleObjects == null)
                _toggleObjects = new List<IActivateable>();
            UpdateSprite();
            if (_sprite != null && this.HitBox != null)
            {
                this.HitBox = new Rectangle(this.HitBox.Location, _sprite.Size);
            }
            UpdateToggleObjects();
            _toggleDelayTimer = new Timer();
            _toggleDelayTimer.Interval = TOGGLE_DELAY_MILLISECONDS;
            _toggleDelayTimer.Elapsed += _toggleDelayTimer_Tick;
        }

        private void _toggleDelayTimer_Tick(object sender, EventArgs e)
        {
            _canToggle = true;
            _toggleDelayTimer.Stop();
        }

        public List<IActivateable> ToggleObjects
        {
            get { return _toggleObjects; }
        }

        public bool IsActive
        {
            get { return _isActive; }
        }

        public void Toggle()
        {
            if (_canToggle)
            {
                _isActive = !_isActive;
                UpdateSprite();
                UpdateToggleObjects();
                DelayToggleAbility();
                EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                    GeneralGameConstants.Sounds.KEEN_TOGGLE_SWITCH);
            }
        }

        private void DelayToggleAbility()
        {
            _canToggle = false;
            _toggleDelayTimer.Start();
        }

        private void UpdateSprite()
        {
            switch (_type)
            {
                case SwitchType.KEEN4_1:
                    _sprite = _isActive ? Properties.Resources.keen4_switch1_on : Properties.Resources.keen4_switch1_off;
                    _sprite.Tag = _isActive ? nameof(Properties.Resources.keen4_switch1_on) : nameof(Properties.Resources.keen4_switch1_off);
                    break;
                case SwitchType.KEEN4_2:
                    _sprite = _isActive ? Properties.Resources.keen4_switch2_on : Properties.Resources.keen4_switch2_off;
                    _sprite.Tag = _isActive ? nameof(Properties.Resources.keen4_switch2_on) : nameof(Properties.Resources.keen4_switch2_off);
                    break;
                case SwitchType.KEEN5_1:
                    _sprite = _isActive ? Properties.Resources.keen5_switch1_on : Properties.Resources.keen5_switch1_off;
                    _sprite.Tag = _isActive ? nameof(Properties.Resources.keen5_switch1_on) : nameof(Properties.Resources.keen5_switch1_off);
                    break;
                case SwitchType.KEEN5_2:
                    _sprite = _isActive ? Properties.Resources.keen5_switch2_on : Properties.Resources.keen5_switch2_off;
                    _sprite.Tag = _isActive ? nameof(Properties.Resources.keen5_switch2_on) : nameof(Properties.Resources.keen5_switch2_off);
                    break;
            }
        }

        private void UpdateToggleObjects()
        {
            if (_isActive)
            {
                foreach (var to in _toggleObjects)
                {
                    to.Activate();
                }
            }
            else
            {
                foreach (var to in _toggleObjects)
                {
                    to.Deactivate();
                }
            }
        }

        public event EventHandler<ToggleEventArgs> Toggled;
        private List<IActivateable> _toggleObjects;
        private bool _isActive;
        private Image _sprite;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.TOGGLE_SWITCH;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            Rectangle area = this.HitBox;
            string arraySeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;
            string activatorGuids = string.Join(arraySeparator, this.ToggleObjects.Select(t => t.ActivationID));
            string activatorStr = arrayStart + activatorGuids + arrayEnd;
            return $"{_sprite.Tag}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_type}{separator}{activatorStr}{separator}{_isActive}";
        }
    }
}
