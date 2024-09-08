using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.Framework.GameEntities.Hazards
{
    public class InvisibleHazard : Hazard
    {
        public InvisibleHazard(SpaceHashGrid grid, Rectangle area) : base(grid, area, HazardType.INVISIBLE, 0)
        {
        }
    }
}
