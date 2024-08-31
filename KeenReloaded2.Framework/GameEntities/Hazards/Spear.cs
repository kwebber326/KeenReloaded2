using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Spear : Hazard, IUpdatable, ISprite
    {
        private Direction _direction;
        private Image[] _images;

        private const int STAB_UPDATE_DELAY = 1;
        private const int STAB_DELAY = 35;
        private int _currentStabDelayTick = 0;
        private int _currentStabUpdateDelayTick = 0;
        private int _currentStabState = 0;
        private bool _retracting;
        private int _currentYOffset;
        private int _currentXOffset;
        private int _currentWidth;
        private int _currentHeight;
        private Point _originalLocation;

        public Spear(Rectangle area, SpaceHashGrid grid, int zIndex, Direction direction)
            : base(grid, area, HazardType.KEEN4_SPEAR, zIndex)
        {
            _direction = direction;
            this.HitBox = area;
            Initialize(_direction);
        }

        private void Initialize(Direction direction)
        {
            switch (_direction)
            {
                case Direction.LEFT:
                    _sprite = Properties.Resources.keen4_spear_wait_left;
                    _sprite.Tag = nameof(Properties.Resources.keen4_spear_wait_left);
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_spear_wait_left,
                        Properties.Resources.keen4_spear_stab_left1,
                        Properties.Resources.keen4_spear_stab_left2
                    };
                    break;
                case Direction.RIGHT:
                    _sprite = Properties.Resources.keen4_spear_wait_right;
                    _sprite.Tag = nameof(Properties.Resources.keen4_spear_wait_right);
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_spear_wait_right,
                        Properties.Resources.keen4_spear_stab_right1,
                        Properties.Resources.keen4_spear_stab_right2
                    };
                    break;
                case Direction.UP:
                    _sprite = Properties.Resources.keen4_spear_wait_up;
                    _sprite.Tag = nameof(Properties.Resources.keen4_spear_wait_up);
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_spear_wait_up,
                        Properties.Resources.keen4_spear_stab_up1,
                        Properties.Resources.keen4_spear_stab_up2
                    };
                    break;
                case Direction.DOWN:
                    _sprite = Properties.Resources.keen4_spear_wait_down;
                    _sprite.Tag = nameof(Properties.Resources.keen4_spear_wait_down);
                    _images = new Image[]
                    {
                        Properties.Resources.keen4_spear_wait_down,
                        Properties.Resources.keen4_spear_stab_down1,
                        Properties.Resources.keen4_spear_stab_down2
                    };
                    break;
            }
            _originalLocation = this.HitBox.Location;
        }

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (_sprite != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(_direction);
                }
            }
        }

        public override bool IsDeadly
        {
            get
            {
                return _currentStabState > 0;
            }
        }

        public void Update()
        {
            switch (_currentStabState)
            {
                case 0:
                    if (_currentStabDelayTick++ == STAB_DELAY)
                    {
                        _currentStabDelayTick = 0;
                        _sprite = _images[++_currentStabState];
                        SetHitboxBasedOnDirectionAndState();
                    }
                    break;
                case 1:
                    if (_currentStabUpdateDelayTick++ == STAB_UPDATE_DELAY)
                    {
                        _currentStabUpdateDelayTick = 0;
                        if (!_retracting)
                        {
                            _sprite = _images[++_currentStabState];
                            SetHitboxBasedOnDirectionAndState();
                        }
                        else
                        {
                            _sprite = _images[--_currentStabState];
                            _retracting = false;
                            SetHitboxBasedOnDirectionAndState();
                        }
                    }
                    break;
                case 2:
                    if (_currentStabUpdateDelayTick++ == STAB_UPDATE_DELAY)
                    {
                        _currentStabUpdateDelayTick = 0;

                        if (!_retracting)
                        {
                            _retracting = true;
                        }
                        else
                        {
                            _sprite = _images[--_currentStabState];
                            SetHitboxBasedOnDirectionAndState();
                        }
                    }
                    break;
            }
            if (this.IsDeadly)
                CheckKeenCollision();
        }

        private void CheckKeenCollision()
        {
            var collisionItems = this.CheckCollision(this.HitBox);
            var collisionKeens = collisionItems.OfType<CommanderKeen>();
            if (collisionKeens.Any())
            {
                foreach (var keen in collisionKeens)
                {
                    keen.Die();
                }
            }

        }

        private void SetHitboxBasedOnDirectionAndState()
        {
            switch (this._direction)
            {
                case Direction.DOWN:
                    _currentYOffset = 0;
                    break;
                case Direction.UP:
                    if (_currentStabState == 1)
                    {
                        _currentYOffset = _retracting ? 26 : -38;
                    }
                    else if (_currentStabState == 2)
                    {
                        _currentYOffset = _retracting ? 26 : -26;
                    }
                    break;
                case Direction.LEFT:
                    if (_currentStabState == 1)
                    {
                        _currentXOffset = _retracting ? 42 : -46;
                    }
                    else if (_currentStabState == 2)
                    {
                        _currentXOffset = _retracting ? 42 : -42;
                    }
                    break;
                case Direction.RIGHT:
                    _currentXOffset = 0;
                    break;
            }
            if (_currentStabState == 0)
            {
                this.HitBox = new Rectangle(_originalLocation, _sprite.Size);
            }
            else
            {
                this.HitBox = new Rectangle(new Point(this.HitBox.X + _currentXOffset, this.HitBox.Y + _currentYOffset), _sprite.Size);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var area = this.HitBox;
            return $"{_sprite.Tag}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_direction}";
        }
    }
}
