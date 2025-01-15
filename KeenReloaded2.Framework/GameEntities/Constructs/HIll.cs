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
    public class Hill : CollisionObject, IUpdatable, ISprite
    {
        protected readonly List<Point> _points;
        protected readonly int _holdTime;
        protected readonly int _spawnDelay;
        protected readonly int _pointsPerSecond;
        protected readonly int _additionalPointsPerMonster;
        protected Image _sprite;

        protected int _currentHoldTimeTick;
        protected int _currentSpawnDelayTick;
        protected HillState _state;

        protected const int SPRITE_CHANGE_DELAY = 5;
        protected int _currentSpriteChangeDelayTick;

        protected int _currentPoint = -1;

        protected const int POINT_EVALUATION_DELAY = 20;
        protected int _currentPointEvaluationDelayTick;

        protected bool _fading;
        protected bool _firstAppearance = true;
        protected readonly int _zIndex;
        protected bool _initialized = false;

        public Hill(Rectangle area, SpaceHashGrid grid, int zIndex, List<Point> points, int holdTimeSeconds, int spawnDelaySeconds, int pointsPerSecond, int additionPointsPerMonster) : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            _points = points;
            _holdTime = holdTimeSeconds;
            _spawnDelay = spawnDelaySeconds;
            _pointsPerSecond = pointsPerSecond;
            _additionalPointsPerMonster = additionPointsPerMonster;
            _sprite = Properties.Resources.mirage_hill4;
        }

        private void Initialize()
        {
            this.HillState = HillState.INACTIVE;
            _initialized = true;
        }

        public override Rectangle HitBox
        {
            get => base.HitBox;
            protected set
            {
                base.HitBox = value;
                if (_collisionGrid != null && _collidingNodes != null && value != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public bool IsActive
        {
            get
            {
                return _state != HillState.INACTIVE;
            }
        }

        public HillState HillState
        {
            get
            {
                return _state;
            }
            protected set
            {
                _state = value;
                if (this.IsActive)
                {
                    _sprite = SpriteSheet.SpriteSheet.HillSprites[(int)_state];
                }
                else
                {
                    _sprite = null;
                }
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public void Update()
        {
            if (!_initialized)
                Initialize();

            if (!this.IsActive)
            {
                _currentPointEvaluationDelayTick = 0;
                if (_currentSpawnDelayTick++ == _spawnDelay)
                {
                    _currentSpawnDelayTick = 0;
                    if (_firstAppearance)
                    {
                        this.HillState = HillState.STRENGTH1;
                        _firstAppearance = false;
                    }
                    else
                    {
                        SpawnAtNextLocation();
                    }
                }
            }
            else if (this.HillState != HillState.STRENGTH4 && !_fading)
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

        private void ResetSequence()
        {
            _fading = false;
            _currentHoldTimeTick = 0;
            _currentSpawnDelayTick = 0;
            _currentSpriteChangeDelayTick = 0;
            this.HillState = HillState.INACTIVE;
        }

        private void EvaluateScoreOnDelay()
        {
            if (_currentPointEvaluationDelayTick++ == POINT_EVALUATION_DELAY)
            {
                _currentPointEvaluationDelayTick = 0;
                EvaluatePointsScored();
            }
        }

        private void EvaluatePointsScored()
        {
            int totalPointsScored = 0;
            var players = CurrentPlayerList.Players;
            foreach (var player in players)
            {
                if (!player.IsDead() && player.HitBox.IntersectsWith(this.HitBox))
                {
                    totalPointsScored += _pointsPerSecond;


                    var collisions = this.CheckCollision(this.HitBox);
                    var enemies = collisions.OfType<IEnemy>();
                    if (enemies.Any(e => e.IsActive))
                    {
                        int additionalPoints = enemies.Count(e => e.IsActive) * _additionalPointsPerMonster;
                        totalPointsScored += additionalPoints;
                    }
                }

                if (totalPointsScored > 0)
                {
                    player.GivePoints(totalPointsScored);
                }
            }
        }

        protected virtual void SpawnAtNextLocation()
        {

            this.HillState = HillState.STRENGTH1;

            if (!_points.Any())
                return;

            if (++_currentPoint >= _points.Count)
            {
                _currentPoint = 0;

            }
           
            this.HitBox = new Rectangle(_points[_currentPoint], this.HitBox.Size);
        }

        private void UpdateStrength()
        {
            int currentStrength = (int)this.HillState;
            if (!_fading)
                currentStrength++;
            else if (currentStrength > 0)
                currentStrength--;
            this.HillState = (HillState)Enum.Parse(typeof(HillState), "STRENGTH" + (currentStrength + 1));
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
            string imageName = nameof(Properties.Resources.mirage_hill4);

            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{pathArray}{separator}{_holdTime}{separator}{_spawnDelay}{separator}{_pointsPerSecond}{separator}{_additionalPointsPerMonster}";
        }
    }

    public enum HillState
    {
        STRENGTH1 = 0,
        STRENGTH2 = 1,
        STRENGTH3 = 2,
        STRENGTH4 = 3,
        INACTIVE = -1
    }
}
