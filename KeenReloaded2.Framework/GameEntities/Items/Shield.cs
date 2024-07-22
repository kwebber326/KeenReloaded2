using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class Shield : Item, IUpdatable
    {
        private CommanderKeen _keen;
        private int _duration;
        private bool _acquiredEventRegistered;
        public bool _isActive;

        private const int SHIELD_DEPLETION_DELAY = 10;
        private int _currentShieldDepletionDelayTick;
        public event EventHandler<ObjectEventArgs> ShieldDurationChanged;

        private const int SHIELD_WARNING_DELAY = 5;
        private const int SHIELD_WARNING_THRESHOLD = 10;

        private int _currentShieldWarningDelayTick;
        private bool _isRed;

        public event EventHandler Depleted;

        public Shield(SpaceHashGrid grid, Rectangle hitbox, string imageName, int zIndex, int duration, CommanderKeen keen) : base(hitbox, imageName, grid, zIndex)
        {

            this.SpriteList = new Image[] { Properties.Resources.Shield };
            _keen = keen;
            _duration = duration;
            _keen.KeenDied += _keen_KeenDied;
           _sprite = this.SpriteList[0];
        }

        private void _keen_KeenDied(object sender, ObjectEventArgs e)
        {
            _keen.KeenMoved -= _keen_KeenMoved;
            _keen.KeenDied -= _keen_KeenDied;
        }

        private void _keen_KeenMoved(object sender, EventArgs e)
        {
            if (_keen.HasShield)
                this.UpdateSpriteToSurroundKeen();
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
                if (value != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }
        public int Duration
        {
            get
            {
                return _duration;
            }
        }

        public bool TryActivate()
        {
            if (_duration > 0)
            {
                if (!_isActive)
                {
                    OnCreate();
                    _zIndex = 99999;
                }
                _isActive = true;
                UpdateSpriteToSurroundKeen();
            }
            return _isActive;
        }

        public void Deactivate()
        {
            _isActive = false;
            OnRemoved();
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
        }

        public override CollisionType CollisionType => CollisionType.ITEM;

        public override void Update()
        {
            if (this.IsAcquired && _isActive)
            {
                _sprite = Properties.Resources.Shield;
                if (!_acquiredEventRegistered)
                {
                    _keen.KeenMoved += _keen_KeenMoved;
                    _acquiredEventRegistered = true;
                }
                if (++_currentShieldDepletionDelayTick == SHIELD_DEPLETION_DELAY)
                {
                    _currentShieldDepletionDelayTick = 0;
                    if (_duration > 0)
                    {
                        _duration--;
                        if (_duration == 0)
                        {
                            this.Deactivate();
                            OnDepleted();
                        }

                        OnShieldDurationChanged();
                    }
                }
                if (_duration > 0 && _duration <= SHIELD_WARNING_THRESHOLD)
                {
                    this.ToggleWarning();
                }
                else
                {
                    _isRed = false;
                }
            }
        }

        protected void OnShieldDurationChanged()
        {
            this.ShieldDurationChanged?.Invoke(this, new ObjectEventArgs() { ObjectSprite = this });
        }

        protected void OnDepleted()
        {
            this.Depleted?.Invoke(this, EventArgs.Empty);
        }

        public void AddShieldToCurrent(Shield shield)
        {
            _duration += shield?.Duration ?? 0;
            OnShieldDurationChanged();
        }

        private void UpdateSpriteToSurroundKeen()
        {
            int widthDifference = Math.Abs(this.HitBox.Width - _keen.HitBox.Width);
            int heightDifference = Math.Abs(this.HitBox.Height - _keen.HitBox.Height);

            int centerX = _keen.HitBox.X - (widthDifference / 2);
            int centerY = _keen.HitBox.Y - (heightDifference / 2);

            Point newLocation = new Point(centerX, centerY);
            this.HitBox = new Rectangle(newLocation, this.HitBox.Size);
        }

        private void ToggleWarning()
        {
            if (++_currentShieldWarningDelayTick == SHIELD_WARNING_DELAY)
            {
                _currentShieldWarningDelayTick = 0;
                _isRed = !_isRed;
            }
        }

        public void SetKeen(CommanderKeen keen)
        {
            _keen = keen;
            if (_keen != null)
            {
                _keen.KeenDied += _keen_KeenDied;
                _keen.KeenMoved += _keen_KeenMoved;
            }
        }
    }
}
