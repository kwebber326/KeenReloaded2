using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;
using IActivator = KeenReloaded2.Framework.GameEntities.Interfaces.IActivator;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class Keen6Switch : CollisionObject, IActivator, ISprite
    {
        private List<IActivateable> _toggleObjects;
        private Image _sprite;
        private bool _isActive;
        private readonly int _zIndex;
        private const int SWITCH_X_OFFSET = 10;
        private const int SWITCH_Y_OFFSET_OFF = 26;
        private const int SWITCH_Y_OFFSET_ON = 10;
        private const int SWITCH_WIDTH = 32;
        private const int SWITCH_HEIGHT = 16;

        private const int POLE_X_OFFSET = 30;
        private const int POLE_HEIGHT = 20;

        private Rectangle _switchArea;

        public Keen6Switch(Rectangle area, SpaceHashGrid grid, int zIndex, IActivateable[] toggleObjects, bool isActive) : base(grid, area)
        {
            _toggleObjects = toggleObjects.ToList() ?? new List<IActivateable>();
            _isActive = isActive;
            _zIndex = zIndex;
            _switchArea = area;
            Initialize();
        }

        private void Initialize()
        {
            this.UpdateSprite();

            UpdateHitbox();
        }

        private void UpdateHitbox()
        {
            int xOffset = SWITCH_X_OFFSET;
            int yOffset = _isActive ? SWITCH_Y_OFFSET_ON : SWITCH_Y_OFFSET_OFF;

            this.HitBox = new Rectangle(_switchArea.X + xOffset, _switchArea.Y + yOffset, SWITCH_WIDTH, SWITCH_HEIGHT);
        }

        public override Rectangle HitBox
        {
            get => base.HitBox;

            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null && _collidingNodes.Any())
                {
                    this.UpdateCollisionNodes(Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Direction.UP_RIGHT);
                }
            }
        }

        public List<IActivateable> ToggleObjects => _toggleObjects;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    UpdateSprite();
                    UpdateHitbox();
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.KEEN_TOGGLE_SWITCH);
                }
            }
        }

        public override CollisionType CollisionType => CollisionType.KEEN6_SWITCH;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _switchArea.Location;

        public bool CanUpdate => true;

        public event EventHandler<ToggleEventArgs> Toggled;

        public void Toggle()
        {
            foreach (var obj in _toggleObjects)
            {
                if (!this.IsActive)
                {
                    obj.Deactivate();
                }
                else
                {
                    obj.Activate();
                }
            }

            ToggleEventArgs e = new ToggleEventArgs()
            {
                IsActive = _isActive
            };
        }

        protected void OnToggled(ToggleEventArgs e)
        {
            this.Toggled?.Invoke(this, e);
        }

        public void OnCollide(CollisionObject obj)
        {
            this.HandleCollision(obj);
            this.Toggle();
        }

        private void UpdateSprite()
        {
            Image switchImg = _isActive ? Properties.Resources.keen6_Switch_On : Properties.Resources.keen6_Switch_Off;
            _sprite = switchImg;
        }

        protected void HandleCollision(CollisionObject obj)
        {
            if (obj.CollisionType == CollisionType.PLAYER)
            {
                if (obj.HitBox.Top > this.HitBox.Bottom)
                {
                    this.IsActive = true;
                }
                else if (obj.HitBox.Bottom < this.HitBox.Top)
                {
                    this.IsActive = false;
                }
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            Rectangle area = _switchArea;
            string arraySeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;
            string activatorGuids = string.Join(arraySeparator, this.ToggleObjects.Select(t => t.ActivationID));
            string activatorStr = arrayStart + activatorGuids + arrayEnd;
            string imageName = IsActive ? nameof(Properties.Resources.keen6_Switch_On) : nameof(Properties.Resources.keen6_Switch_Off);
            return $"{imageName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{activatorStr}{separator}{_isActive}";
        }
    }
}
