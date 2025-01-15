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


namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class FartBubble : CollisionObject, IUpdatable, ICreateRemove, ISprite
    {
        private const int FLOAT_VELOCITY = 15;
        private const int HORIZONTAL_VELOCITY = 5;
        private const int HORIZONTAL_MOVE_TIME = 6;
        private readonly int _zIndex;
        private int _horizontalMoveTimeTick;

        public FartBubble(SpaceHashGrid grid, Rectangle hitbox, int zIndex)
            : base(grid, hitbox)
        {
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            _sprite = Properties.Resources.keen4_dopefish_fart_bubble;
            _direction = Direction.RIGHT;
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
                if (_collisionGrid != null && _collidingNodes != null && this.HitBox != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.UP);
                    this.UpdateCollisionNodes(_direction);
                }
            }
        }

        public void Update()
        {
            int xOffset = _direction == Direction.RIGHT ? HORIZONTAL_VELOCITY : HORIZONTAL_VELOCITY * -1;
            this.HitBox = new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y - FLOAT_VELOCITY, this.HitBox.Width, this.HitBox.Height);
            if (++_horizontalMoveTimeTick == HORIZONTAL_MOVE_TIME)
            {
                _horizontalMoveTimeTick = 0;
                _direction = ChangeHorizontalDirection(_direction);
            }

            if (!_collidingNodes.Any())
            {
                ObjectEventArgs e = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                OnRemove(e);
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        protected void OnCreate(ObjectEventArgs e)
        {
            if (this.Create != null)
            {
                this.Create(this, e);
            }
        }

        protected void OnRemove(ObjectEventArgs e)
        {
            if (this.Remove != null)
            {
                this.Remove(this, e);
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.NONE;

        private Image _sprite;
        private Direction _direction;
    }
}
