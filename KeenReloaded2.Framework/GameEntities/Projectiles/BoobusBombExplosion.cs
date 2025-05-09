﻿using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class BoobusBombExplosion : CollisionObject, IUpdatable, IExplodable, ISprite, ICreateRemove
    {
        private int _currentSprite;
        private Image[] _explosionImages = new Image[]
        {
            Properties.Resources.keen_dreams_boobus_bomb_explode1,
            Properties.Resources.keen_dreams_boobus_bomb_explode2
        };

        private const int EXPLOSION_LENGTH = 4;
        private int _explosionTick;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _spriteChangeTick;

        public BoobusBombExplosion(SpaceHashGrid grid, Rectangle hitbox, int blastRadius, int damage)
            : base(grid, hitbox)
        {
            _damage = damage;
            Initialize();
        }

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
            }
        }


        private void Initialize()
        {
            _sprite = _explosionImages[_currentSprite];
            _explosionState = Enums.ExplosionState.EXPLODING;
        }

        public void Update()
        {
            this.Explode();
        }

        public void Explode()
        {
            var collisions = this.CheckCollision(this.HitBox);
            foreach (var collision in collisions)
            {
                this.HandleCollision(collision);
            }
            if (_explosionTick < EXPLOSION_LENGTH)
            {
                if (_spriteChangeTick++ == SPRITE_CHANGE_DELAY)
                {
                    _spriteChangeTick = 0;
                    _explosionTick++;
                    UpdateSprite();
                }
            }
            else
            {
                _explosionState = Enums.ExplosionState.DONE;
                OnRemove();
            }
        }

        private void UpdateSprite()
        {
            if (_currentSprite < _explosionImages.Length - 1)
            {
                _currentSprite++;
            }
            else
            {
                _currentSprite = 0;
            }
            _sprite = _explosionImages[_currentSprite];
        }

        public Enums.ExplosionState ExplosionState
        {
            get { return _explosionState; }
        }

        public Image Image
        {
            get { return _sprite; }
        }

        public override CollisionType CollisionType => CollisionType.EXPLOSION;

        public int ZIndex => 500;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public bool ExplodesFromProjectileCollision => false;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private Image _sprite;
        private Enums.ExplosionState _explosionState;
        private int _damage;

        protected void OnRemove()
        {
            if (this.Remove != null)
            {
                _explosionState = Enums.ExplosionState.DONE;
                foreach (var node in _collidingNodes)
                {
                    node.Objects.Remove(this);
                    node.NonEnemies.Remove(this);
                }
                ObjectEventArgs args = new ObjectEventArgs() { ObjectSprite = this };
                this.Remove(this, args);
            }
        }

        protected void OnCreate()
        {
            if (this.Create != null)
            {
                ObjectEventArgs args = new ObjectEventArgs() { ObjectSprite = this };
                this.Create(this, args);
            }
        }

        protected void HandleCollision(CollisionObject obj)
        {
            if (!(obj is CommanderKeen))
            {
                if (obj is IStunnable)
                {
                    ((IStunnable)obj).Stun();
                }
                else if (obj is DestructibleObject)
                {
                    ((DestructibleObject)obj).TakeDamage(this._damage);
                }
            }
        }
    }
}
