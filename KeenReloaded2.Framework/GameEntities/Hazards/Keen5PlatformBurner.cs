using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class Keen5PlatformBurner : Hazard
    {
        public Keen5PlatformBurner(SpaceHashGrid grid, Rectangle hitbox, int zIndex)
            : base(grid, hitbox, Enums.HazardType.KEEN5_BURNER, zIndex)
        {

        }

        public int SpinSequence
        {
            get;
            set;
        }

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (_collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        public void UpdateLocation(Rectangle newHitbox)
        {
            this.HitBox = newHitbox;
        }

        public override bool IsDeadly
        {
            get
            {
                return SpinSequence != 6 && SpinSequence != 7;
            }
        }
    }
}
