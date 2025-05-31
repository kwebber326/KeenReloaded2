using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints
{
    public class Keen6CheckPoint : Checkpoint, IUpdatable, ICreateRemove
    {
        protected readonly Image[] _handOpen = new Image[]
        {
            Properties.Resources.keen6_flag_hand_opened
        };

        protected readonly Image[] _handClosed = new Image[]
       {
            Properties.Resources.keen6_flag_hand_closed
       };

        public Keen6CheckPoint(Rectangle area, SpaceHashGrid grid, int zIndex) : base(area, grid, zIndex)
        {
            _sprite = InactiveSprites[0];
            this.HitBox = area;
        }

        protected override Image[] InactiveSprites => _handOpen;

        protected override Image[] ActiveSprites => _handClosed;

        protected FlippingCheckpointFlag _flag;

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        protected bool _firstLand = true;

        protected void CreateFlag()
        {
            int width = Properties.Resources.keen_flipping_flag1.Width;
            int height = Properties.Resources.keen_flipping_flag1.Height;
            int x = this.HitBox.Right - this.HitBox.Width / 2 - 2;
            int y = this.HitBox.Top - height + 16;
            _flag = new FlippingCheckpointFlag(_collisionGrid, new Rectangle(x, y, width, height), _zIndex);
            Create?.Invoke(this, new ObjectEventArgs() { ObjectSprite = _flag });
        }

        public void Update()
        {
            if (_checkPointHit && _flag == null)
            {
                this.CreateFlag();
            }
            else if (_checkPointHit)
            {
                _flag.Update();
                if ((_flag.State == CheckPointFlagState.LANDING || _flag.State == CheckPointFlagState.WAVING)
                    && _firstLand)
                {
                    _firstLand = false;
                    _sprite = ActiveSprites[0];
                }
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen6_flag_hand_opened);
            var area = this.HitBox;
            return $"{imageName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}";
        }
    }
}
