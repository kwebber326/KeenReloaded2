using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Extensions;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class RandomHill : Hill
    {
        public RandomHill(Rectangle area, SpaceHashGrid grid, int zIndex, List<Point> points, int holdTimeSeconds, int spawnDelaySeconds, int pointsPerSecond, int additionPointsPerMonster)
            : base(area, grid, zIndex, points, holdTimeSeconds, spawnDelaySeconds, pointsPerSecond, additionPointsPerMonster)
        {
        }

        protected override void SpawnAtNextLocation()
        {
            this.HillState = HillState.STRENGTH1;
            _random = new Random();
            _currentPoint = _random.Next(0, _points.Count);
            var newPoint = _points[_currentPoint];
            this.HitBox = new Rectangle(newPoint, this.HitBox.Size);
        }

        public override string ToString()
        {
            string pathArray = MapMakerConstants.MAP_MAKER_ARRAY_START;
            foreach (var node in _points)
            {
                string item = node.X + MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR + node.Y + (node == _points.Last()
                    ? ""
                    : MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR);
                pathArray += item;
            }
            pathArray += MapMakerConstants.MAP_MAKER_ARRAY_END;

            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.random_mirage_hill);

            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{pathArray}{separator}{_holdTime}{separator}{_spawnDelay}{separator}{_pointsPerSecond}{separator}{_additionalPointsPerMonster}";
        }
    }
}
