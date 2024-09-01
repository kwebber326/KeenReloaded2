using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.HelperObjects
{
    public class LocatedImage
    {
        public Image Image { get; set; }

        public Point Location { get; set; }

        public int Top
        {
            get
            {
                return this.Location.Y;   
            }
        }

        public int Bottom
        {
            get
            {
                return this.Location.Y + this.Height;
            }
        }

        public int Left
        {
            get
            {
                return this.Location.X;
            }
        }

        public int Right
        {
            get
            {
                return this.Location.X + this.Width;
            }
        }

        public int Width
        {
            get
            {
                return this.Image?.Width ?? 0;
            }
        }

        public int Height
        {
            get
            {
                return this.Image?.Height ?? 0;
            }
        }
    }
}
