using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class NospikeQuestionMark : CollisionObject, IUpdatable, ISprite, ICreateRemove
    {
        public const int WIDTH = 16;
        public const int HEIGHT = 32;
        private readonly int _zIndex;

        public NospikeQuestionMark(SpaceHashGrid grid, Rectangle hitbox, int zIndex)
            : base(grid, hitbox)
        {
            _zIndex = zIndex;
            Initialize();
        }

        private void Initialize()
        {
            _sprite = Properties.Resources.keen6_nospike_confused;
        }

        public void Update()
        {

        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private Image _sprite;

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                Create(this, args);
            }
        }

        protected void OnRemove(ObjectEventArgs args)
        {
            if (this.Remove != null)
            {
                if (args.ObjectSprite == this)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                    }
                }
                this.Remove(this, args);
            }
        }
    }
}
