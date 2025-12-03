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
    public class PointBasedHill : Hill
    {
        private readonly int _pointLimitPerLocation;
        private int _pointsScoredAtLocation;
        private int _basePointsScored;

        public PointBasedHill(Rectangle area, SpaceHashGrid grid, int zIndex, List<Point> points, int spawnDelaySeconds, int pointsPerSecond, int additionPointsPerMonster, int pointLimitPerLocation)
            : base(area, grid, zIndex, points, -1, spawnDelaySeconds, pointsPerSecond, additionPointsPerMonster)
        {
            _pointLimitPerLocation = pointLimitPerLocation;
        }

        protected override void Initialize()
        {
            this.HillState = HillState.STRENGTH1;
            _initialized = true;
            _currentPoint = 0;
        }


        public override void Update()
        {
            if (!_initialized)
                Initialize();

            if (this.IsActive && this.HillState != HillState.STRENGTH4 && !_fading)
            {
                if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
                {
                    _currentSpriteChangeDelayTick = 0;
                    UpdateStrength();
                }
                EvaluateScoreOnDelay();
            }
            else
            {
                if (IsActive)
                    EvaluateScoreOnDelay();

                if (!_fading)
                {
                    if (_currentHoldTimeTick++ == _holdTime)
                    {
                        _currentHoldTimeTick = 0;
                        _fading = true;
                    }
                }
                else if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
                {
                    _currentSpriteChangeDelayTick = 0;
                    int strength = (int)this.HillState;
                    if (strength > 0)
                    {
                        UpdateStrength();
                    }
                    else
                    {
                        ResetSequence();
                    }
                }
            }
        }

        protected override void EvaluatePointsScored(out int points)
        {
            base.EvaluatePointsScored(out points);
            _pointsScoredAtLocation += points;
            if (points > 0)
                _basePointsScored += _pointsPerSecond;

            if (_basePointsScored >= _pointLimitPerLocation)
            {
                _pointsScoredAtLocation = 0;
                SpawnAtNextLocation();
                _basePointsScored = 0;
            }
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
            string imageName = nameof(Properties.Resources.point_based_mirage_hill);

            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{pathArray}{separator}{_spawnDelay}{separator}{_pointsPerSecond}{separator}{_additionalPointsPerMonster}{separator}{_pointLimitPerLocation}";
        }
    }
}
