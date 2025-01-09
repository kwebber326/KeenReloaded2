using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Backgrounds
{
    public class MimicBlock : Background
    {
        public MimicBlock(Rectangle area, string imageName, bool stretchImage, int zIndex) 
            : base(area, imageName, stretchImage, zIndex)
        {
        }
    }
}
