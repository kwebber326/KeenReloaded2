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

        public MimicBlock(Rectangle area, Image image, bool stretchImage, int zIndex, bool drawImageOnInitialization)
            : base(area, image, stretchImage, zIndex, drawImageOnInitialization)
        {
        }

        public override bool CanUpdate => true;
    }
}
