using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
using KeenReloaded.Framework.Utilities;

namespace KeenReloaded2.Utilities
{
    public static class CommonGameFunctions
    {
        public static double GetEuclideanDistance(Point p1, Point p2)
        {
            double xDiff = p1.X - p2.X;
            double yDiff = p1.Y - p2.Y;

            double xDist = Math.Pow(xDiff, 2.0);
            double yDist = Math.Pow(yDiff, 2.0);

            double euclideanDistance = Math.Sqrt(xDist + yDist);
            return euclideanDistance;
        }

        public static List<FieldInfo> GetConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }

        public static Image DrawImage(Rectangle area, Image img)
        {
            var imgToDraw = img;
            Size edgeOfMapTileDimensions = new Size(imgToDraw.Width, imgToDraw.Height);

            int rows = area.Height >= imgToDraw.Height ? area.Height / imgToDraw.Height : 1;
            if (area.Height % imgToDraw.Height != 0)
                rows++;

            int columns = area.Width >= imgToDraw.Width ? area.Width / imgToDraw.Width : 1;
            if (area.Width % imgToDraw.Width != 0)
                columns++;

            //if there are no new rows or columns to draw, just return the image back
            if (rows == 1 && columns == 1)
                return img;

            List<Image> images = new List<Image>();
            List<Point> imagePoints = new List<Point>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = j * imgToDraw.Width;
                    int y = i * imgToDraw.Height;

                    var location = new Point(x, y);

                    images.Add(imgToDraw);
                    imagePoints.Add(location);
                }
            }
            
            return BitMapTool.DrawImagesOnCanvas(area.Size, null, images.ToArray(), imagePoints.ToArray());
        }
    }
}
