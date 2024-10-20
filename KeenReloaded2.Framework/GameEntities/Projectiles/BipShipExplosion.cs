using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class BipShipExplosion : IUpdatable, ISprite, ICreateRemove
    {

        public const int HEIGHT = 84, WIDTH = 64;
        private const int SPRITE_CHANGE_DELAY = 2;
        private readonly int _zIndex;
        private int _currentSpriteChangeDelayTick;
        private int _currentSprite;
        private Image _sprite;
        private Point _location;

        Image[] _sprites = new Image[]
        {
            Properties.Resources.keen6_bip_ship_explosion1,
            Properties.Resources.keen6_bip_ship_explosion2
        };

        public BipShipExplosion(Point location)
        {
            _location = location;
            _sprite = _sprites[_currentSprite];
        }

        public void Update()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                _currentSprite++;
                if (_currentSprite < _sprites.Length)
                {
                    _sprite = _sprites[_currentSprite];
                }
                else
                {
                    OnRemove(new ObjectEventArgs() { ObjectSprite = this });
                }
            }
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _location;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;

        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                Create(this, args);
            }
        }

        protected void OnRemove(ObjectEventArgs args)
        {
            if (this.Remove != null)
            {
                this.Remove(this, args);
            }
        }
    }
}
