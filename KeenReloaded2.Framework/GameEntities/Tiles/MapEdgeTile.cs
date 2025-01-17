using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.HelperObjects;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class MapEdgeTile : MaskedTile, IUpdatable
    {
        private readonly MapEdgeBehavior _behavior;

        public MapEdgeTile(Rectangle area, SpaceHashGrid grid, int zIndex, MapEdgeBehavior behavior)
            : base(area, grid, area, null, zIndex)
        {
            DrawImage(area);
            _behavior = behavior;
        }

        private void DrawImage(Rectangle area)
        {
            var imgToDraw = Properties.Resources.edge_of_map_tile_debug;
            Size edgeOfMapTileDimensions = new Size(imgToDraw.Width, imgToDraw.Height);

            int rows = area.Height >= imgToDraw.Height ? area.Height / imgToDraw.Height : 1;
            if (area.Height % imgToDraw.Height != 0)
                rows++;

            int columns = area.Width >= imgToDraw.Width ? area.Width / imgToDraw.Width : 1;
            if (area.Width % imgToDraw.Width != 0)
                columns++;

            List<Image> images = new List<Image>();
            List<Point> imagePoints = new List<Point>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = j * imgToDraw.Width;
                    int y = i * imgToDraw.Height;
                    var locatedImg = new LocatedImage()
                    {
                        Image = imgToDraw,
                        Location = new Point(x, y)
                    };
                    images.Add(locatedImg.Image);
                    imagePoints.Add(locatedImg.Location);
                }
            }

            _image = BitMapTool.DrawImagesOnCanvas(area.Size, null, images.ToArray(), imagePoints.ToArray());
        }

        public MapEdgeBehavior Behavior
        {
            get
            {
                return _behavior;
            }
        }

        private CollisionType GetCollisionTypeFromBehavior()
        {
            switch (_behavior)
            {
                case MapEdgeBehavior.DEATH:
                    return CollisionType.HAZARD;
                case MapEdgeBehavior.EXIT:
                case MapEdgeBehavior.BARRIER:
                default:
                    return CollisionType.BLOCK;
            }
        }

        public override CollisionType CollisionType => GetCollisionTypeFromBehavior();

        public override string ToString()
        {
            return $"{nameof(Properties.Resources.edge_of_map_tile_debug)}{_separator}{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}{_separator}{_zIndex}{_separator}{_behavior}";
        }

        public void Update()
        {
            if (_image != null)
                _image = null;
        }
    }
}
