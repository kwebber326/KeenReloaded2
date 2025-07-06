using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Checkpoints
{
    public abstract class Checkpoint : CollisionObject, ISprite
    {
        protected readonly int _zIndex;
        protected Image _sprite;
        protected bool _checkPointHit;

        public event EventHandler CheckPointHit;

        public Checkpoint(Rectangle area, SpaceHashGrid grid, int zIndex) : base(grid, area)
        {
            _zIndex = zIndex;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.CHECKPOINT;

        protected abstract Image[] InactiveSprites { get; }

        protected abstract Image[] ActiveSprites { get; }

        public virtual void MarkAsHit()
        {
            if (!_checkPointHit)
            {
                _checkPointHit = true;
                CheckPointHit?.Invoke(this, EventArgs.Empty);
                this.PublishSoundPlayEvent(GeneralGameConstants.Sounds.CHECKPOINT);
            }
        }
    }
}
