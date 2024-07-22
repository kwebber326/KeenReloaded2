using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
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
        private List<Image> _poleSprites;
        private readonly int _addedLengths;
        private readonly int _zIndex;
        private const int SWITCH_X_OFFSET = 10;
        private const int SWITCH_Y_OFFSET_OFF = 10;
        private const int SWITCH_Y_OFFSET_ON = 26;
        private const int SWITCH_WIDTH = 32;
        private const int SWITCH_HEIGHT = 16;

        private const int POLE_X_OFFSET = 30;
        private const int POLE_HEIGHT = 20;

        public Keen6Switch(SpaceHashGrid grid, Rectangle hitbox, List<IActivateable> toggleObjects, bool isActive, int addedLengths = 0) : base(grid, hitbox)
        {
            _toggleObjects = toggleObjects ?? new List<IActivateable>();
            _isActive = isActive;
            _addedLengths = addedLengths;
            Initialize();
        }

        private void Initialize()
        {
            this.UpdateSprite();

            UpdateHitbox();

            _poleSprites = new List<Image>();

            int currentY = this.HitBox.Bottom;

            for (int i = 0; i < _addedLengths; i++)
            {
                Image img = Properties.Resources.keen6_Switch_pole;
                _poleSprites.Add(img);

                currentY += POLE_HEIGHT;
            }
        }

        private void UpdateHitbox()
        {
            int xOffset = SWITCH_X_OFFSET;
            int yOffset = _isActive ? SWITCH_Y_OFFSET_ON : SWITCH_Y_OFFSET_OFF;

            this.HitBox = new Rectangle(this.Location.X + xOffset, this.Location.Y + yOffset, SWITCH_WIDTH, SWITCH_HEIGHT);
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

        public List<Image> PoleSprites
        {
            get
            {
                return _poleSprites;
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
                _isActive = value;
                UpdateSprite();
                UpdateHitbox();
            }
        }

        public override CollisionType CollisionType => CollisionType.KEEN6_SWITCH;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

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
            _sprite = _isActive ? Properties.Resources.keen6_Switch_On : Properties.Resources.keen6_Switch_Off;
        }

        protected void HandleCollision(CollisionObject obj)
        {
            if (obj.CollisionType == CollisionType.PLAYER)
            {
                if (obj.HitBox.Top > this.HitBox.Bottom)
                {
                    this.IsActive = false;
                }
                else if (obj.HitBox.Bottom < this.HitBox.Top)
                {
                    this.IsActive = true;
                }
            }
        }
    }
}
