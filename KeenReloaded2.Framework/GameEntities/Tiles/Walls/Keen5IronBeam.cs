using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Walls
{
    public class Keen5IronBeam : MaskedTile
    {
        public Keen5IronBeam(Rectangle area, SpaceHashGrid grid, int zIndex) 
            : base(area, grid, area, null, zIndex)
        {
            _image = CommonGameFunctions.DrawImage(_area, Properties.Resources.keen5_iron_beam);
            _initialImageName = nameof(Properties.Resources.keen5_iron_beam);
            this.HitBox = new Rectangle(_area.Location, _area.Size);
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;
    }
}
