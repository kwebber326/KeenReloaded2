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

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class LightningBolt : Hazard, IUpdatable
    {
        public LightningBolt(SpaceHashGrid grid, Rectangle hitbox, int zIndex)
            : base(grid, hitbox, HazardType.KEEN4_LIGHTNING_BOLT, zIndex)
        {
            OnCreated();
        }

        private const int SPRITE_SWITCHES = 10;
        private int _currentSpriteSwitchCount = 0;

        public event EventHandler<ObjectEventArgs> Removed;
        public event EventHandler<ObjectEventArgs> Created;

        private Image[] _images = new Image[]
        {
             Properties.Resources.keen4_lightning_bolt1,
             Properties.Resources.keen4_lightning_bolt2
        };

        protected void OnRemoved()
        {
            if (Removed != null)
            {
                if (_collidingNodes != null)
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

                Removed(this, args);
            }
        }

        protected void OnCreated()
        {
            if (Created != null)
            {
                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                Created(this, args);
            }
        }

        public void Update()
        {
            if (_currentSpriteSwitchCount < SPRITE_SWITCHES)
            {
                _sprite = _images[++_currentSpriteSwitchCount % 2];
                var collisionItems = this.CheckCollision(this.HitBox);
                var keens = collisionItems.OfType<CommanderKeen>();
                foreach (var keen in keens)
                {
                    keen.Die();
                }
            }
            else
            {
                OnRemoved();
            }
        }
    }
}
