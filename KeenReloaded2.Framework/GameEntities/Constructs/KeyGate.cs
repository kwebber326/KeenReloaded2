using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class KeyGate : MaskedTile, IActivateable, IUpdatable
    {
        private int _currentDeactivateSprite = 1;
        private bool _IsOpened;
        private Image[] _images = new Image[]
        {
            Properties.Resources.key_gate1,
            Properties.Resources.key_gate2,
            Properties.Resources.key_gate3,
            Properties.Resources.key_gate4
        };
        private bool _isActive;
        private readonly Guid _activationId;

        public KeyGate(Rectangle area, SpaceHashGrid grid, string imageFile, int zIndex, Guid activationId) 
            : base(area, grid, area, imageFile, zIndex)
        {
            _activationId = activationId;
            Initialize();
        }

        private void Initialize()
        {
            _isActive = true;
            InitializeSpriteMap(_image.Size);
        }

        private void InitializeSpriteMap(Size imageDimensions)
        {
            //image width and height
            int width = imageDimensions.Width;
            int height = imageDimensions.Height;
            //location to write image (0,0 is the top left corner of the picture box, not the form)
            int currentX = 0;
            int currentY = 0;
            //declare a bitmap for the image
            var bitmap = new Bitmap(this.HitBox.Width, this.HitBox.Height);

            //for each subsequent length and height, draw the image onto the bitmap
            //to fill out the background rectangle
            for (int i = 0; i < this.HitBox.Width; i += width)
            {
                for (int j = 0; j < this.HitBox.Height; j += height)
                {
                    Graphics.FromImage(bitmap).DrawImage(_image, new Rectangle(currentX, currentY, _image.Width, _image.Height));
                    currentY += height;
                }
                currentY = 0;
                currentX += width;
            }
            //assign the resulting bitmap to the picture box's image property so it loads as 
            //one image onto the form
            _image = bitmap;
        }

        public void Activate()
        {
            _isActive = true;
        }

        public void Deactivate()
        {
            _isActive = false;
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
        }

        public Guid ActivationID => _activationId;

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public void Update()
        {
            if (!this.IsActive)
            {
                if (_currentDeactivateSprite == _images.Length && !_IsOpened)
                {
                    _IsOpened = true;
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                        node.Tiles.Remove(this);
                        node.NonEnemies.Remove(this);
                    }
                }
                else if (!_IsOpened)
                {
                    _image = _images[_currentDeactivateSprite++];
                    InitializeSpriteMap(_image.Size);
                }
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + separator + _activationId.ToString();
        }
    }
}
