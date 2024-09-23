using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class PoisonSlugPoop : Hazard, IUpdatable, ISprite, ICreateRemove
    {
        private SlugPoopState _state;
        private const int ACTIVE_STATE_REMOVE_DELAY = 80;
        private int _currentActiveStateDelayTick = 0;

        private const int REMOVE_DELAY = 40;
        private const int FALL_VELOCITY = 40;
        private int _currentRemoveDelay;

        public static int POOP_HEIGHT
        {
            get
            {
                return 14;
            }
        }
        public static int POOP_WIDTH
        {
            get
            {
                return 30;
            }
        }

        public PoisonSlugPoop(SpaceHashGrid grid, Rectangle area, int zIndex)
            : base(grid, area, Enums.HazardType.KEEN4_SLUG_POOP, zIndex)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.State = SlugPoopState.ACTIVE;
        }

        public void Update()
        {
            this.BasicFall(FALL_VELOCITY);
            if (this.State == SlugPoopState.ACTIVE)
            {
                if (_currentActiveStateDelayTick++ == ACTIVE_STATE_REMOVE_DELAY)
                {
                    _currentActiveStateDelayTick = 0;
                    this.State = SlugPoopState.FADING;
                }
            }
            else
            {
                if (_currentRemoveDelay++ == REMOVE_DELAY)
                {
                    _currentRemoveDelay = 0;
                    _sprite = null;
                    OnRemove();
                }
            }
        }

        private void UpdateSprite()
        {
            switch (this.State)
            {
                case SlugPoopState.ACTIVE:
                    _sprite = Properties.Resources.keen4_slug_poop_active;
                    break;
                case SlugPoopState.FADING:
                    _sprite = Properties.Resources.keen4_slug_poop_fading;
                    break;
            }
        }

        internal SlugPoopState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                UpdateSprite();
            }
        }

        public override bool IsDeadly
        {
            get
            {
                return this.State == SlugPoopState.ACTIVE;
            }
        }

        public override Rectangle HitBox
        {
            get => base.HitBox;
            protected set
            {
                base.HitBox = value;
                if (_collisionGrid != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        protected void OnRemove()
        {
            if (Remove != null)
            {
                if (_collidingNodes != null && _collidingNodes.Any())
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                    }
                }

                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                Remove(this, args);
            }
        }

        protected void OnCreate()
        {
            if (Create != null)
            {
                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                Create(this, args);
            }
        }
    }

    enum SlugPoopState
    {
        ACTIVE,
        FADING
    }
}
