using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KeenReloaded2.Utilities.Extensions
{
    public static class RectangleExtensions
    {
        public static bool CompletelyContains(this Rectangle r1, Rectangle r2)
        {
            bool completelyContains = r1.Left <= r2.Left && r1.Right >= r2.Right
                && r1.Top <= r2.Top && r1.Bottom >= r2.Bottom;
            return completelyContains;
        }

        public static Rectangle ExpandToCompletelyContain(this Rectangle r1, Rectangle r2)
        {
            if (r1.CompletelyContains(r2))
                return r1;

            int leftAdjustment = r1.Left > r2.Left ? r1.Left - r2.Left : 0;
            int rightAdjustment = r1.Right < r2.Right ? r2.Right - r1.Right : 0;
            int topAdjustment = r1.Top > r1.Top ? r1.Top - r2.Top : 0;
            int bottomAdjustment = r1.Bottom < r2.Bottom ? r2.Bottom - r1.Bottom : 0;

            int x = r1.X - leftAdjustment;
            int y = r1.Y - topAdjustment;

            int width = r1.Width + rightAdjustment;
            int height = r1.Height + bottomAdjustment;

            return new Rectangle(x, y, width, height);
        }
    }
}
