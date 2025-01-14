using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Enemies;
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

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class Keen6SetPathPlatform : SetPathPlatform, ICreateRemove
    {
        private BipPlatformOperator _bipPlatformOperator;

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        public Keen6SetPathPlatform(Rectangle area, SpaceHashGrid grid, int zIndex, List<Point> locations, Guid activationId, bool initiallyActive = false)
            : base(area, grid, zIndex, PlatformType.KEEN6, locations, activationId, initiallyActive)
        {

        }

        public void InitializeBipOperator()
        {
            Point location = this.GetBipOperatorLocation();
            _bipPlatformOperator = new BipPlatformOperator(location, _zIndex);

            ObjectEventArgs e = new ObjectEventArgs()
            {
                ObjectSprite = _bipPlatformOperator
            };

            this.Create?.Invoke(this, e);
        }

        private Point GetBipOperatorLocation()
        {
            int thisX = this.HitBox.X, thisY = this.HitBox.Y;
            int thisWidth = this.HitBox.Width, thisHeight = this.HitBox.Height;
            int bipOperatorWidth = Properties.Resources.keen6_bip_platform_down.Width;
            int x = thisX + (thisWidth / 2) - bipOperatorWidth / 2;//center the operator
            int y = this.HitBox.Bottom - (thisHeight / 2) + 1; //underneath the bottom of platform
            Point location = new Point(x, y);
            return location;
        }

        public override void Update()
        {
            base.Update();
            if (_bipPlatformOperator == null)
            {
                InitializeBipOperator();
            }
            else
            {
                _bipPlatformOperator.Direction = this.Direction;
                Point location = this.GetBipOperatorLocation();
                _bipPlatformOperator.SetLocation(location);
            }
        }

        public override Direction Direction
        {
            get
            {
                return _direction;
            }
            protected set
            {
                _direction = value;
                if (_bipPlatformOperator != null)
                    _bipPlatformOperator.Direction = value;
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_bip_platform);
            string pathArray = MapMakerConstants.MAP_MAKER_ARRAY_START;
            for (int i = 0; i < _pathwayPoints.Count; i++)
            {
                var node = _pathwayPoints[i];
                string item = node.X + MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR + node.Y + ((i == _pathwayPoints.Count - 1)
                    ? ""
                    : MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR);
                pathArray += item;
            }
            pathArray += MapMakerConstants.MAP_MAKER_ARRAY_END;
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{pathArray}{separator}{_activationId}{separator}{_isActive}";
        }
    }
}
