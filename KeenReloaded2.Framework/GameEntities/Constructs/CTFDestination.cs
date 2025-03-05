using KeenReloaded.Framework;
using KeenReloaded2.Constants;
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

        private Rectangle _area;

        public CTFDestination(Rectangle area, SpaceHashGrid grid, int zIndex, GemColor color)
            : base(grid, area)
        {
            _sprite= SpriteSheet.SpriteSheet.CTFDestinations[(int)color];
            _zIndex = zIndex;
            _color = color;
            _area = area;
        }
        public GemColor Color => _color;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.CTF_DESTINATION;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = GetImageNameFromColor();
            return imageName + separator + _area.X + separator + _area.Y + separator
                + _area.Width + separator + _area.Height + separator + _zIndex.ToString() 
                + separator + _color.ToString();
        }

        private string GetImageNameFromColor()
        {
            switch (_color)
            {
                case GemColor.BLUE:
                    return nameof(Properties.Resources.blue_flag_destination);
                case GemColor.GREEN:
                    return nameof(Properties.Resources.green_flag_destination);
                case GemColor.RED:
                    return nameof(Properties.Resources.red_flag_destination);
                case GemColor.YELLOW:
                    return nameof(Properties.Resources.yellow_flag_destination);
            }

            return null;
        }
    }
}
