using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public abstract class Item : CollisionObject, IAcquirable, IUpdatable, ISprite, ICreateRemove
    {
        private Image _sprite;
        public event EventHandler Acquired;
        protected bool _moveUp = true;
        protected bool _canSteal = false;
        protected bool _collectable = true;
        protected Rectangle _area;
        protected int _zIndex;
        protected string _imageName;

        public Item(Rectangle area, string imageName, SpaceHashGrid grid, int zIndex)
            : base(grid, area)
        {
            _area = area;
            _zIndex = zIndex;
            _imageName = imageName;
            string resourcesDirectory = FileIOUtility.GetResourcePathForMainProject();
            string imagePath = Path.Combine(resourcesDirectory, imageName + ".png");
            _sprite = Image.FromFile(imagePath);
            _itemRemoveSpriteChangeDelay = this.AcquiredSpriteList != null && this.AcquiredSpriteList.Length > 0 ? ITEM_REMOVE_DELAY / this.AcquiredSpriteList.Length : 0;
        }

        public virtual void Destroy()
        {
            foreach (var node in _collidingNodes)
            {
                node.Objects.Remove(this);
                node.NonEnemies.Remove(this);
            }
            OnRemoved();
        }

        protected virtual void OnAcquired()
        {
            if (Acquired != null)
            {
                Acquired(this, null);
            }
        }

        public virtual bool CanSteal
        {
            get
            {
                return _canSteal;
            }
        }

        public virtual bool CollectibleByPlayer
        {
            get
            {
                return _collectable;
            }
        }

        protected virtual void OnCreate()
        {
            this.Create?.Invoke(this, new ObjectEventArgs() { ObjectSprite = this });
        }

        protected virtual void OnRemoved()
        {
            if (Remove != null)
            {
                ObjectEventArgs args = new ObjectEventArgs()
                {
                    ObjectSprite = this
                };
                Remove(this, args);
            }
        }

        protected Image[] SpriteList;
        protected Image[] AcquiredSpriteList;
        protected int _currentSprite;
        private bool _isAcquired;
        protected const int ACQUIRED_STATE_ASCENSION_VELOCITY = 2;
        protected const int ITEM_REMOVE_DELAY = 8;
        protected int _currentItemRemoveDelay = 0;
        protected const int SPRITE_CHANGE_DELAY = 7;
        protected int _currentSpriteChangeTick = 0;
        private int _itemRemoveSpriteChangeDelay;
        private int _itemRemoveSpriteChangeDelayTick;

        public virtual void Update()
        {
            if (!this.IsAcquired && this.SpriteList != null)
            {
                if (_currentSpriteChangeTick >= SPRITE_CHANGE_DELAY)
                {
                    _currentSpriteChangeTick = 0;
                    if (_currentSprite >= this.SpriteList.Length)
                    {
                        _currentSprite = 0;
                    }
                    this.Image = this.SpriteList[_currentSprite++];
                }
                else
                {
                    _currentSpriteChangeTick++;
                }
            }
            else if (_currentItemRemoveDelay < ITEM_REMOVE_DELAY)
            {
                if (_itemRemoveSpriteChangeDelay == 0)
                    _itemRemoveSpriteChangeDelay = this.AcquiredSpriteList != null && this.AcquiredSpriteList.Length > 0 ? ITEM_REMOVE_DELAY / this.AcquiredSpriteList.Length : 0;
                if (this.AcquiredSpriteList != null)
                {
                    if (_currentSprite >= this.AcquiredSpriteList.Length)
                    {
                        _currentSprite = 0;
                    }
                    this.Image = this.AcquiredSpriteList[_currentSprite];
                    if (_moveUp)
                        this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - ACQUIRED_STATE_ASCENSION_VELOCITY, this.HitBox.Width, this.HitBox.Height);
                }
                if (_itemRemoveSpriteChangeDelayTick++ == _itemRemoveSpriteChangeDelay)
                {
                    _itemRemoveSpriteChangeDelayTick = 0;
                    _currentSprite++;
                }
                _currentItemRemoveDelay++;
            }
            else
            {
                if (_collidingNodes != null)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.NonEnemies.Remove(this);
                        node.Objects.Remove(this);
                    }
                }
                OnRemoved();
            }
        }

        public void SetAcquired()
        {
            this.IsAcquired = true;
        }


        public bool IsAcquired
        {
            get
            {
                return _isAcquired;
            }
            set
            {
                if (value != _isAcquired)
                {
                    _isAcquired = value;
                    if (_isAcquired)
                        OnAcquired();
                }
            }
        }

        public override Rectangle HitBox
        {
            get
            {
                return _area;
            }
            protected set
            {
                _area = value;
            }
        }

        public int ZIndex => _zIndex;

        public Image Image
        {
            get
            {
                return _sprite;
            }
            protected set
            {
                _sprite = value;
            }
        }

        public Point Location => _area.Location;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{_imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}";
        }
    }
}
