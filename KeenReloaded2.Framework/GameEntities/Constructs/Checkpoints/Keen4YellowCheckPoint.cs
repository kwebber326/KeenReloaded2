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
    public class Keen4YellowCheckPoint : Checkpoint, IUpdatable, ICreateRemove
    {
        protected readonly Image[] _flagBase = new Image[]
        {
            Properties.Resources.keen4_flag_base_yellow
        };
        public Keen4YellowCheckPoint(Rectangle area, SpaceHashGrid grid, int zIndex) : base(area, grid, zIndex)
        {
            _sprite = _flagBase[0];
            this.HitBox = area;
        }

        protected override Image[] InactiveSprites => _flagBase;

        protected override Image[] ActiveSprites => _flagBase;

        protected FlippingCheckpointFlag _flag;

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        protected void CreateFlag()
        {
            int width = Properties.Resources.keen_flipping_flag1.Width;
            int height = Properties.Resources.keen_flipping_flag1.Height;
            int x = this.HitBox.Right - this.HitBox.Width / 2 - 2;
            int y = this.HitBox.Top - height + 2;
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
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen4_flag_base_yellow);
            var area = this.HitBox;
            return $"{imageName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}";
        }
    }
}
