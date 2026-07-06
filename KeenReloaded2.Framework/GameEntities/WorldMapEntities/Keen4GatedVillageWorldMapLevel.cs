using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class Keen4GatedVillageWorldMapLevel : ForeGroundMultiHitboxWorldMapLevel
    {
        private Orientation _orientation;
        private Image _originalImage;

        public Keen4GatedVillageWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Guid activationId, Rectangle[] entryPoints, Rectangle[] hitboxes, Rectangle[] foregroundAreas, Orientation orientation)
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, activationId, entryPoints, hitboxes, foregroundAreas)
        {
            _orientation = orientation;
            _originalImage = sprite;
            RotateBaseOnOrientation(orientation);

        }

        private void RotateBaseOnOrientation(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.NORMAL:
                default:
                    break;
                case Orientation.ROTATE_RIGHT_90:
                    _sprite = new Bitmap(_originalImage);
                    _sprite.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    RotateHitboxesRight();
                    break;
                case Orientation.ROTATE_LEFT_90:
                    _sprite = new Bitmap(_originalImage);
                    _sprite.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    RotateHitboxesLeft();
                    break;
                case Orientation.ROTATE_180:
                    _sprite = new Bitmap(_originalImage);
                    _sprite.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    RotateHitboxes180();
                    break;
            }
        }

        #region Rotation methods
        private void RotateHitboxes180()
        {
            if (_collidingNodes != null && _collisionGrid != null)
            {
                RemoveHitBoxes();
                var tiles = _collisionTiles;
                Rotate180(tiles);
            }
        }

        private void Rotate180(List<InvisibleTile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                var collisionTileHitbox = tiles[i].HitBox;

                Rectangle newRect = Get180RotatedRectangle(collisionTileHitbox);

                tiles[i] = new InvisibleTile(_collisionGrid, newRect);
            }

            for (int i = 0; i < _trueEntryPoints.Length; i++)
            {
                Rectangle newRect = Get180RotatedRectangle(_trueEntryPoints[i]);
                _trueEntryPoints[i] = newRect;
            }
        }

        private Rectangle Get180RotatedRectangle(Rectangle collisionTileHitbox)
        {
            //retain width and height
            int width = collisionTileHitbox.Width;
            int height = collisionTileHitbox.Height;

            //on 180 degree rotation, the new x is the distance from hitbox right
            //to the right of the image space on the original image
            //the new y is the distance from the bottom of the hitbox to the bottom
            //of the image space
            int x = this.HitBox.X + this.HitBox.Right - collisionTileHitbox.Right;
            int y = this.HitBox.Y + this.HitBox.Bottom - collisionTileHitbox.Bottom;

            Rectangle newRect = new Rectangle(x, y, width, height);
            return newRect;
        }

        private void RotateHitboxesRight()
        {
            if (_collidingNodes != null && _collisionGrid != null)
            {
                RemoveHitBoxes();
                var tiles = _collisionTiles;
                Rotate90Right(tiles);
            }
        }

        private void Rotate90Right(List<InvisibleTile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                var collisionTileHitbox = tiles[i].HitBox;

                Rectangle newRect = GetRightRotatedRectangle(collisionTileHitbox);

                tiles[i] = new InvisibleTile(_collisionGrid, newRect);
            }

            for (int i = 0; i < _trueEntryPoints.Length; i++)
            {
                Rectangle newRect = GetRightRotatedRectangle(_trueEntryPoints[i]);
                _trueEntryPoints[i] = newRect;
            }
        }

        private Rectangle GetRightRotatedRectangle(Rectangle collisionTileHitbox)
        {
            //invert width and height
            int width = collisionTileHitbox.Height;
            int height = collisionTileHitbox.Width;

            //on 90 degree rotation, the new x becomes the distance between
            //the bottom of the image space and the bottom of the hitbox
            //and the y becomes the distance between the left of the hitbox
            //and the left of the image space
            int x = this.HitBox.X + this.HitBox.Bottom - collisionTileHitbox.Bottom;
            int y = this.HitBox.Y + collisionTileHitbox.Left - this.HitBox.Left;

            var newRect = new Rectangle(x, y, width, height);
            return newRect;
        }

        private void RotateHitboxesLeft()
        {
            if (_collidingNodes != null && _collisionGrid != null)
            {
                RemoveHitBoxes();
                var tiles = _collisionTiles;
                Rotate90Left(tiles);
            }
        }

        private void Rotate90Left(List<InvisibleTile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                var collisionTileHitbox = tiles[i].HitBox;

                Rectangle newRect = GetLeftRotatedRectangle(collisionTileHitbox);

                tiles[i] = new InvisibleTile(_collisionGrid, newRect);
            }

            for (int i = 0; i < _trueEntryPoints.Length; i++)
            {
                Rectangle newRect = GetLeftRotatedRectangle(_trueEntryPoints[i]);
                _trueEntryPoints[i] = newRect;
            }
        }

        private Rectangle GetLeftRotatedRectangle(Rectangle collisionTileHitbox)
        {
            //invert width and height
            int width = collisionTileHitbox.Height;
            int height = collisionTileHitbox.Width;

            //on 90 degree rotation, the new x becomes the distance between
            //the top of the image space and the top of the hitbox
            //and the y becomes the distance between the right of the hitbox
            //and the right of the image space
            int x = this.HitBox.X + collisionTileHitbox.Top - this.HitBox.Top;
            int y = this.HitBox.Y + this.HitBox.Right - collisionTileHitbox.Right;

            var newRect = new Rectangle(x, y, width, height);
            return newRect;
        }

        #endregion

        private void RemoveHitBoxes()
        {
            foreach (var tile in _collisionTiles)
            {
                tile.RemoveTileFromGrid();
            }
        }

        public override void Update()
        {
            if (!_updated)
            {
                _updated = true;
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            var gate = _collisionTiles.Last();
            gate.RemoveTileFromGrid();
            var x = gate.HitBox.X - this.HitBox.X;
            var y = gate.HitBox.Y - this.HitBox.Y;
            var eraseRect = new Rectangle(x, y, gate.HitBox.Width, gate.HitBox.Height);
            _sprite = BitMapTool.EraseImageSection(_sprite, eraseRect);
        }

        public override string ToString()
        {
            return base.ToString() + $"{_separator}{_orientation}";
        }
    }
}
