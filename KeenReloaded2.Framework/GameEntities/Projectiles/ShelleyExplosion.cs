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
    public class ShelleyExplosion : CollisionObject, IUpdatable, ISprite, ICreateRemove, IExplodable
    {
        private int _currentSprite;
        private const int SPRITE_CHANGE_DELAY = 2;
        private int _currentSpriteChangeDelayTick;
        private Image[] _explosionSprites = new Image[]
        {
            Properties.Resources.keen5_shelley_explosion1,
            Properties.Resources.keen5_shelley_explosion2,
            Properties.Resources.keen5_shelley_explosion3,
            Properties.Resources.keen5_shelley_explosion4
        };
        public ShelleyExplosion(SpaceHashGrid grid, Rectangle hitbox)
            : base(grid, hitbox)
        {
            Initialize();
        }

        private void Initialize()
        {
            _sprite = _explosionSprites[_currentSprite];
            _state = Enums.ExplosionState.EXPLODING;
        }

        public void Update()
        {
            if (_state == Enums.ExplosionState.EXPLODING)
            {
                this.Explode();
            }
            else if (_state == Enums.ExplosionState.DONE)
            {
                OnRemove(new ObjectEventArgs() { ObjectSprite = this });
            }
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private Image _sprite;
        private Enums.ExplosionState _state;

        public void Explode()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                int nextSprite = _currentSprite + 1;
                if (nextSprite >= _explosionSprites.Length)
                {
                    _state = Enums.ExplosionState.DONE;
                    return;
                }
                else
                {
                    int verticalOffset = 0, horizontalOffset = 0;
                    var currentImg = _sprite;
                    var nextImg = _explosionSprites[nextSprite];
                    //center explosion
                    if (currentImg.Height < nextImg.Height)
                    {
                        verticalOffset = currentImg.Height - nextImg.Height;
                    }
                    if (currentImg.Width < nextImg.Width)
                    {
                        horizontalOffset = currentImg.Width - nextImg.Width;
                    }
                    //set next image;
                    _sprite = nextImg;
                    _currentSprite = nextSprite;
                    //align hitbox and recheck collision
                    this.HitBox = new Rectangle(this.HitBox.X + horizontalOffset, this.HitBox.Y + verticalOffset, _sprite.Width, _sprite.Height);
                }
            }
            //check for collision with keen
            var collisions = this.CheckCollision(this.HitBox);
            var players = collisions.Where(c => c.CollisionType == CollisionType.PLAYER);
            if (players.Any())
            {
                foreach (var player in players)
                {
                    ((CommanderKeen)player).Die();
                }
            }
        }

        public Enums.ExplosionState ExplosionState
        {
            get { return _state; }
        }

        public override CollisionType CollisionType => CollisionType.EXPLOSION;

        public int ZIndex => 200;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public bool ExplodesFromProjectileCollision => false;

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
                if (args.ObjectSprite == this)
                {
                    foreach (var node in _collidingNodes)
                    {
                        node.Objects.Remove(this);
                    }
                }
                this.Remove(this, args);
            }
        }
    }
}
