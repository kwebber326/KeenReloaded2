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
using System.Windows.Forms;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class GemPlaceHolder : CollisionObject, ISprite, IActivator
    {
        private bool _isActive;
        private GemColor _gemColor;
        private Rectangle _area;
        private Image _sprite;
        private readonly int _zIndex;

        public GemPlaceHolder(Rectangle area, SpaceHashGrid grid, int zIndex, GemColor color, IActivateable[] objectsToActivate)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.ToggleObjects = objectsToActivate?.ToList();
            _gemColor = color;
            _area = area;
            Initialize();
        }

        private void Initialize()
        {
            switch (_gemColor)
            {
                case GemColor.BLUE:
                    _sprite = Properties.Resources.gem_placeholder_blue_empty;
                    _sprite.Tag = nameof(Properties.Resources.gem_placeholder_blue_empty);
                    break;
                case GemColor.GREEN:
                    _sprite = Properties.Resources.gem_placeholder_green_empty;
                    _sprite.Tag = nameof(Properties.Resources.gem_placeholder_green_empty);
                    break;
                case GemColor.RED:
                    _sprite = Properties.Resources.gem_placeholder_red_empty;
                    _sprite.Tag = nameof(Properties.Resources.gem_placeholder_red_empty);
                    break;
                case GemColor.YELLOW:
                    _sprite = Properties.Resources.gem_placeholder_yellow_empty;
                    _sprite.Tag = nameof(Properties.Resources.gem_placeholder_yellow_empty);
                    break;
            }

            this.Toggled += new EventHandler<ToggleEventArgs>(GemPlaceHolder_Toggled);
        }

        void GemPlaceHolder_Toggled(object sender, ToggleEventArgs e)
        {
            if (this.ToggleObjects != null && this.ToggleObjects.Any())
            {
                foreach (var obj in this.ToggleObjects)
                {
                    if (_isActive)
                    {
                        obj.Deactivate();
                    }
                }
            }
        }

        protected void HandleCollision(CollisionObject obj)
        {

        }

        public List<IActivateable> ToggleObjects
        {
            get;
            private set;
        }

        public bool IsActive
        {
            get { return _isActive; }
        }

        public GemColor Color
        {
            get
            {
                return _gemColor;
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.GEM_PLACEHOLDER;

        public void Toggle()
        {
            _isActive = !_isActive;

            ToggleEventArgs e = new ToggleEventArgs()
            {
                IsActive = this.IsActive
            };
            if (this.IsActive)
            {
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - 10, this.HitBox.Width, this.HitBox.Height + 10);
                _area = this.HitBox;
                SetActivatedSprite();
            }
            OnToggled(e);
        }

        private void SetActivatedSprite()
        {
            switch (_gemColor)
            {
                case GemColor.BLUE:
                    _sprite = Properties.Resources.gem_placeholder_blue_filled;
                    break;
                case GemColor.GREEN:
                    _sprite = Properties.Resources.gem_placeholder_green_filled;
                    break;
                case GemColor.RED:
                    _sprite = Properties.Resources.gem_placeholder_red_filled;
                    break;
                case GemColor.YELLOW:
                    _sprite = Properties.Resources.gem_placeholder_yellow_filled;
                    break;
            }
        }



        protected void OnToggled(ToggleEventArgs e)
        {
            if (Toggled != null)
            {
                Toggled(this, e);
            }
            this.PublishSoundPlayEvent(
               GeneralGameConstants.Sounds.KEY_GATE_OPEN);
        }

        public event EventHandler<ToggleEventArgs> Toggled;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string arraySeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;
            string activatorGuids = string.Join(arraySeparator, this.ToggleObjects.Select(t => t.ActivationID));
            string activatorStr = arrayStart + activatorGuids + arrayEnd;
            return $"{_sprite.Tag?.ToString()}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{ZIndex}{separator}{_gemColor.ToString()}{separator}{activatorStr}";
        }
    }
}
