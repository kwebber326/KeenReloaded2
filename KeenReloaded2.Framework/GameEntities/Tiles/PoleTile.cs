using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class PoleTile : CollisionObject, ISprite
    {
        private readonly int _zIndex;
        private Image _sprite;
        private string _type;

        public PoleTile(SpaceHashGrid grid, Rectangle hitbox, Image sprite, int zIndex)
            : base(grid, hitbox)
        {
            _zIndex = zIndex;
            _sprite = sprite;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public Size ImageSize => _sprite.Size;

        public override CollisionType CollisionType => CollisionType.POLE_TILE;
    }
}
