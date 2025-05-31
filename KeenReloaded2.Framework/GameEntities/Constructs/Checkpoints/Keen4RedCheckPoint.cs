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
    public class Keen4RedCheckPoint : Checkpoint, IUpdatable, ICreateRemove
    {
        protected readonly Image[] _flagBase = new Image[]
        {
            Properties.Resources.keen4_flag_base_red1,
            Properties.Resources.keen4_flag_base_red2,
            Properties.Resources.keen4_flag_base_red3,
            Properties.Resources.keen4_flag_base_red4
        };
        public Keen4RedCheckPoint(Rectangle area, SpaceHashGrid grid, int zIndex) : base(area, grid, zIndex)
        {
            _sprite = _flagBase[_currentSpriteIndex];
            this.HitBox = area;
        }

        protected override Image[] InactiveSprites => _flagBase;

        protected override Image[] ActiveSprites => _flagBase;

        protected FlippingCheckpointFlag _flag;

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        protected const int SPRITE_CHANGE_DELAY = 2;
        protected int _spriteChangeDelayTick = 0;
        protected int _currentSpriteIndex = 0;

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
                this.UpdateSpriteByDelayBase(ref _spriteChangeDelayTick, ref _currentSpriteIndex, SPRITE_CHANGE_DELAY,
                    () =>
                    {
                        if (_currentSpriteIndex >= ActiveSprites.Length)
                            _currentSpriteIndex = 0;

                        _sprite = ActiveSprites[_currentSpriteIndex];
                    });
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen4_flag_base_red1);
            var area = this.HitBox;
            return $"{imageName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}";
        }
    }
}
