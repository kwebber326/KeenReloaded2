using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class Flag : Item, IFlag
    {
        #region constants
        private const int POINT_DEGRADATION_DELAY = 15;
        #endregion

        #region readonly initialization values
        private readonly int _maxPoints;
        private readonly int _minPoints;
        private readonly int _pointsDegradedPerSecond;
        #endregion

        #region private fields
        private int _currentPointValue;
        private int _currentPointDegradationDelayValue;

        private bool _isCaptured;

        private GemColor _color;

        private Point _originalLocation;
        #endregion

        #region events
        public event EventHandler<FlagCapturedEventArgs> FlagCaptured;
        public event EventHandler<FlagCapturedEventArgs> FlagPointsChanged;
        #endregion

        public Flag(Rectangle area, SpaceHashGrid grid, string imageName, int zIndex, GemColor color, int maxPoints, int minPoints, int pointsDegradedPerSecond) 
            : base(area, imageName, grid, zIndex)
        {
            if (maxPoints <= 0)
                throw new ArgumentException("Max points must be greater than zero");
            if (minPoints < 0)
                throw new ArgumentException("Min points must be greater than or equal to zero");
            if (maxPoints < minPoints)
                throw new ArgumentException("Max points must be greater than or equal to min points");

            _color = color;
            _maxPoints = maxPoints;
            _minPoints = minPoints;
            _pointsDegradedPerSecond = pointsDegradedPerSecond;
            _currentPointValue = maxPoints;

            InitializeSprite();
        }

        #region properties

        public int MaxPoints
        {
            get
            {
                return _maxPoints;
            }
        }

        public int MinPoints
        {
            get
            {
                return _minPoints;
            }
        }

        public int PointsDegradedPerSecond
        {
            get
            {
                return _pointsDegradedPerSecond;
            }
        }

        public int CurrentPointValue
        {
            get
            {
                return _currentPointValue;
            }
        }

        public bool IsCaptured
        {
            get
            {
                return _isCaptured;
            }
        }

        public GemColor Color
        {
            get
            {
                return _color;
            }
        }

        public Point LocationOfOrigin
        {
            get
            {
                return _originalLocation;
            }
        }

        public override CollisionType CollisionType => CollisionType.ITEM;
        #endregion


        #region public methods

        public override void Update()
        {
            base.Update();

            if (this.IsAcquired || _currentPointValue == _minPoints)
                return;

            if (_currentPointDegradationDelayValue++ >= POINT_DEGRADATION_DELAY)
            {
                _currentPointDegradationDelayValue = 0;
                if (_currentPointValue - _pointsDegradedPerSecond >= _minPoints)
                {
                    _currentPointValue -= _pointsDegradedPerSecond;
                }
                else
                {
                    _currentPointValue = _minPoints;
                }
                OnPointsChanged();
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + separator + _zIndex.ToString() + separator + _color.ToString() + separator + _maxPoints + separator + _minPoints + separator + _pointsDegradedPerSecond;
        }

        public void Capture()
        {
            _isCaptured = true;
            OnCaptured();
        }

        public Flag Copy()
        {
            return new Flag(new Rectangle(_originalLocation, this.HitBox.Size), _collisionGrid, _imageName, _zIndex, _color, _maxPoints, _minPoints, _pointsDegradedPerSecond);
        }
        #endregion

        #region protected methods
        protected void OnCaptured()
        {
            FlagCapturedEventArgs e = ConstructEventArgs();
            this.FlagCaptured?.Invoke(this, e);
        }

        protected void OnPointsChanged()
        {
            FlagCapturedEventArgs e = ConstructEventArgs();
            this.FlagPointsChanged?.Invoke(this, e);
        }
        #endregion

        #region private helper methods
        private FlagCapturedEventArgs ConstructEventArgs()
        {
            return new FlagCapturedEventArgs()
            {
                Flag = this
            };
        }
        private void InitializeSprite()
        {
            var colorImages = SpriteSheet.SpriteSheet.CTFColors;
            var image = colorImages[(int)_color];
            this.SpriteList = new Image[] { image };
            this.AcquiredSpriteList = new Image[] { Properties.Resources.Flag_Acquired };
            _sprite = image;
            _originalLocation = new Point(this.HitBox.X, this.HitBox.Y);
        }
        #endregion
    }
}
