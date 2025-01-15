using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Framework.GameEntities.Items;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class GnosticeneItemDestroyer : CollisionObject, IUpdatable, ICreateRemove, ISprite
    {
        private Item _itemToDestroy;
        private const int SPRITE_CHANGE_DELAY = 1;
        private readonly int _zIndex;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;

        private Image[] _images = new Image[]
        {
            Properties.Resources.keen4_gnosticene_ancient_dust1,
            Properties.Resources.keen4_gnosticene_ancient_dust2,
            Properties.Resources.keen4_gnosticene_ancient_dust3,
            Properties.Resources.keen4_gnosticene_ancient_dust4
        };

        public GnosticeneItemDestroyer(SpaceHashGrid grid, Rectangle hitbox, int zIndex, Item itemToDestroy)
            : base(grid, hitbox)
        {
            if (itemToDestroy == null)
                throw new ArgumentNullException("there needs to be an item for the destroyer to destroy");

            _itemToDestroy = itemToDestroy;
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            _itemToDestroy.Destroy();
            _sprite = _images[0];
        }

        public void Update()
        {
            if (_currentSprite >= _images.Length)
            {
                OnRemove();
            }
            else if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                _sprite = _images[_currentSprite++];
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private Image _sprite;

        protected void OnRemove()
        {
            if (Remove != null)
            {
                foreach (var node in _collidingNodes)
                {
                    node.Objects.Remove(this);
                    node.NonEnemies.Remove(this);
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

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;
    }
}
