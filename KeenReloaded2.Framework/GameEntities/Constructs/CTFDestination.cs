using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class CTFDestination : CollisionObject, ISprite
    {
        private readonly GemColor _color;
        private Image _sprite;
        private readonly int _zIndex;

        public CTFDestination(SpaceHashGrid grid, Rectangle hitbox, int zIndex, GemColor color)
            : base(grid, hitbox)
        {
            _sprite= SpriteSheet.SpriteSheet.CTFDestinations[(int)color];
            _zIndex = zIndex;
            _color = color;
        }
        public GemColor Color => _color;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public override CollisionType CollisionType => CollisionType.CTF_DESTINATION;

        public override string ToString()
        {
            return base.ToString() + "|" + _color.ToString();
        }
    }
}
