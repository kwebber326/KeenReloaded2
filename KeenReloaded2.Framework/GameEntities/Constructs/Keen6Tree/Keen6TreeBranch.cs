using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles.Platforms;
using System.Drawing;

namespace KeenReloaded2.Framework.GameEntities.Constructs.Keen6Tree
{
    public class Keen6TreeBranch : CollisionObject, ISprite
    {
        private readonly int _zIndex;
        private Direction _branchDirection;
        private readonly bool _useFirstImage;
        private Image _sprite;
        private string _objectKey;

        private readonly Size _invisiblePlatformSize = new Size(16, 32);

        public Keen6TreeBranch(Rectangle area, SpaceHashGrid grid, int zIndex, Direction branchDirection, bool useFirstImage) 
            : base(grid, area)
        {
            _zIndex = zIndex;
            _branchDirection = branchDirection;
            _useFirstImage = useFirstImage;
            this.HitBox = area;

            Initialize();
        }

        private void Initialize()
        {
            SetObjectData();

            if (_collisionGrid != null && _collidingNodes != null)
            {
                //first left branch
                if (_useFirstImage && _branchDirection == Direction.LEFT)
                {
                    ConstructLeftDefaultBranch();
                }
                else if (!_useFirstImage && _branchDirection == Direction.LEFT)
                {
                    ConstructLeftNonDefaultBranch();
                }
                else if (_useFirstImage && _branchDirection == Direction.RIGHT)
                {
                    ConstructRightDefaultBranch();
                }
                else if (!_useFirstImage && _branchDirection == Direction.RIGHT)
                {
                    ConstructRightNonDefaultBranch();
                }
            }
        }

        private void ConstructLeftDefaultBranch()
        {
            int xOffset = 32, yOffset = 32;
            _objectKey = nameof(Properties.Resources.keen6_tree_left_branch1);
            InvisiblePlatformTile invisiblePlatformTile1 = new InvisiblePlatformTile(_collisionGrid,
                new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
                _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 16;
            InvisiblePlatformTile invisiblePlatformTile2 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 8;
            InvisiblePlatformTile invisiblePlatformTile3 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 8;
            InvisiblePlatformTile invisiblePlatformTile4 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 8;
            InvisiblePlatformTile invisiblePlatformTile5 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 8;
            InvisiblePlatformTile invisiblePlatformTile6 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));
        }

        private void ConstructLeftNonDefaultBranch()
        {
            int xOffset = 16, yOffset = 16;
            _objectKey = nameof(Properties.Resources.keen6_tree_left_branch2);
            InvisiblePlatformTile invisiblePlatformTile1 = new InvisiblePlatformTile(_collisionGrid,
                new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
                _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 24;
            InvisiblePlatformTile invisiblePlatformTile2 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 24;
            InvisiblePlatformTile invisiblePlatformTile3 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 24;
            InvisiblePlatformTile invisiblePlatformTile4 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 16;
            InvisiblePlatformTile invisiblePlatformTile5 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset += 8;
            InvisiblePlatformTile invisiblePlatformTile6 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));
        }

        private void ConstructRightDefaultBranch()
        {
            int xOffset = 32, yOffset = _sprite.Height - _invisiblePlatformSize.Height + 8;
            _objectKey = nameof(Properties.Resources.keen6_tree_right_branch1);
            InvisiblePlatformTile invisiblePlatformTile1 = new InvisiblePlatformTile(_collisionGrid,
                new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
                _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset -= 16;
            InvisiblePlatformTile invisiblePlatformTile2 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset -= 8;
            InvisiblePlatformTile invisiblePlatformTile3 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset -= 8;
            InvisiblePlatformTile invisiblePlatformTile4 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset -= 8;
            InvisiblePlatformTile invisiblePlatformTile5 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            yOffset -= 8;
            InvisiblePlatformTile invisiblePlatformTile6 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));
        }

        private void ConstructRightNonDefaultBranch()
        {
            int xOffset = 48, yOffset = _sprite.Height - _invisiblePlatformSize.Height - 8;
            _objectKey = nameof(Properties.Resources.keen6_tree_right_branch1);
            InvisiblePlatformTile invisiblePlatformTile1 = new InvisiblePlatformTile(_collisionGrid,
                new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
                _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            InvisiblePlatformTile invisiblePlatformTile2 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));

            xOffset += _invisiblePlatformSize.Width;
            InvisiblePlatformTile invisiblePlatformTile3 = new InvisiblePlatformTile(_collisionGrid,
               new Rectangle(this.HitBox.X + xOffset, this.HitBox.Y + yOffset,
               _invisiblePlatformSize.Width, _invisiblePlatformSize.Height));
        }

        private void SetObjectData()
        {
            if (_useFirstImage)
            {
                _objectKey = _branchDirection == Direction.LEFT ?
                    nameof(Properties.Resources.keen6_tree_left_branch1) :
                    nameof(Properties.Resources.keen6_tree_right_branch1);

                _sprite = _branchDirection == Direction.LEFT ?
                   Properties.Resources.keen6_tree_left_branch1 :
                   Properties.Resources.keen6_tree_right_branch1;
            }
            else
            {
                _objectKey = _branchDirection == Direction.LEFT ?
                   nameof(Properties.Resources.keen6_tree_left_branch2) :
                   nameof(Properties.Resources.keen6_tree_right_branch2);

                _sprite = _branchDirection == Direction.LEFT ?
                  Properties.Resources.keen6_tree_left_branch2 :
                  Properties.Resources.keen6_tree_right_branch2;
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var area = this.HitBox;
            return $"{_objectKey}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{_branchDirection}{separator}{_useFirstImage}";
        }

    }
}
