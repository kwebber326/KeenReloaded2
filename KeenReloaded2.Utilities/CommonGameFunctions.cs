using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KeenReloaded2.Utilities
{
    public static class CommonGameFunctions
    {
        public static double GetEuclideanDistance(Point p1, Point p2)
        {
            double xDiff = p1.X - p2.X;
            double yDiff = p2.Y - p2.Y;

            double xDist = Math.Pow(xDiff, 2.0);
            double yDist = Math.Pow(yDiff, 2.0);

            double euclideanDistance = Math.Sqrt(xDist + yDist);
            return euclideanDistance;
        }
    }
}
