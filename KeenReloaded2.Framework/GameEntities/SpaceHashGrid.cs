using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KeenReloaded.Framework
{
    public class SpaceHashGrid
    {
        public SpaceHashGrid(int width, int height, int hashWidth, int hashHeight)
        {
            _width = width;
            _height = height;
            _hashWidth = hashWidth;
            _hashHeight = hashHeight;
            _size = new Size(_width, _height);
            //set x/y coordinates and sizes
            for (int x = 0; x < width; x += hashWidth)
            {
                for (int y = 0; y < height; y += hashHeight)
                {
                    SpaceHashGridNode node = new SpaceHashGridNode();
                    node.HashBox = new Rectangle(new Point(x, y), new Size(hashWidth, hashHeight));
                    _nodes.Add(node);
                    _nodeDict.Add(node.HashBox.Location.ToString(), node);
                }
            }
            //complete the grid data structure by mapping neighbor objects (up, down, left, and right directions)
            //SetNeighbors(_nodes.FirstOrDefault());
            SetNeighbors();
        }

        private void SetNeighbors()
        {
            foreach (var node in _nodes)
            {
                Point pLeft = new Point(node.HashBox.X - _hashWidth, node.HashBox.Location.Y);
                if (_nodeDict.ContainsKey(pLeft.ToString()))
                {
                    node.Left = _nodeDict[pLeft.ToString()];
                }

                Point pRight = new Point(node.HashBox.X + _hashWidth, node.HashBox.Location.Y);
                if (_nodeDict.ContainsKey(pRight.ToString()))
                {
                    node.Right = _nodeDict[pRight.ToString()];
                }

                Point pUp = new Point(node.HashBox.Location.X, node.HashBox.Y - _hashHeight);
                if (_nodeDict.ContainsKey(pUp.ToString()))
                {
                    node.Up = _nodeDict[pUp.ToString()];
                }

                Point pDown = new Point(node.HashBox.Location.X, node.HashBox.Y + _hashHeight);
                if (_nodeDict.ContainsKey(pDown.ToString()))
                {
                    node.Down = _nodeDict[pDown.ToString()];
                }
            }
            _nodeDict.Clear();
        }

        [Obsolete("Recursive method to set neighboring space hash grid nodes. It is hideously inefficient from a processing standpoint and murder to the callstack. A O(n) dictionary-based algorithm to load neighboring space hash grid nodes has replaced this method.  Use the parameterless SetNeighors() method instead")]
        private void SetNeighbors(SpaceHashGridNode node)
        {
            if (node == null)
                return;

            if (node.HashBox.X != (_width - _hashWidth))
            {
                node.Right = _nodes.FirstOrDefault(n => n.HashBox.Left == node.HashBox.Right && n.HashBox.Y == node.HashBox.Y);
                if (node.Right != null)
                {
                    node.Right.Left = node;
                    SetNeighbors(node.Right);
                }
            }

            if (node.HashBox.Y != (_height - _hashHeight))
            {
                node.Down = _nodes.FirstOrDefault(n => n.HashBox.Top == node.HashBox.Bottom && n.HashBox.X == node.HashBox.X);
                if (node.Down != null)
                {
                    node.Down.Up = node;
                    SetNeighbors(node.Down);
                }
            }
        }

        public HashSet<SpaceHashGridNode> GetCurrentHashes(CollisionObject obj)
        {
            var nodes = new HashSet<SpaceHashGridNode>(_nodes.Where(n => n.HashBox.IntersectsWith(obj.HitBox)));
            return nodes;
        }

        public List<SpaceHashGridNode> _nodes = new List<SpaceHashGridNode>();
        private Dictionary<string, SpaceHashGridNode> _nodeDict = new Dictionary<string, SpaceHashGridNode>();
        private int _height;
        private int _width;
        private int _hashWidth;
        private int _hashHeight;
        private readonly Size _size;

        public Size Size
        {
            get
            {
                return _size;
            }
        }
    }

    public class SpaceHashGridNode
    {
        public SpaceHashGridNode()
        {
            this.Objects = new HashSet<CollisionObject>();
            this.Tiles = new HashSet<CollisionObject>();
            this.NonEnemies = new HashSet<CollisionObject>();
        }

        public Rectangle HashBox { get; set; }

        public SpaceHashGridNode Up { get; set; }
        public SpaceHashGridNode Down { get; set; }
        public SpaceHashGridNode Left { get; set; }
        public SpaceHashGridNode Right { get; set; }

        public HashSet<CollisionObject> Objects { get; private set; }
        public HashSet<CollisionObject> Tiles { get; private set; }
        public HashSet<CollisionObject> NonEnemies { get; private set; }

        public Image Display
        {
            get;
            set;
        }
    }
}
