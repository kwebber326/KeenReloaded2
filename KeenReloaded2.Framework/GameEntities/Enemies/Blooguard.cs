using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Drawing;


namespace KeenReloaded2.Framework.GameEntities.Enemies
{
    public class Blooguard : Bloog
    {
        protected Image[] _smashLeftSprites, _smashRightSprites;
        protected const int SMASH_CHANCE = 30;
        protected const int SMASH_DUD_CHANCE = 50;
        protected const int SMASH_RANGE = 400;
        private const int SMASH_SPRITE_CHANGE_DELAY = 2;
        private const int FIRST_SMASH_SPRITE_CHANGE_DELAY = 10;
        private int _currentSmashSpriteChangeDelayTick;
        private int _currentSmashSprite;
        private bool _smashDud;
        private readonly int _originalWidth, _originalHeight;

        private bool _hitAnimation;
        private const int HIT_ANIMATION_TIME = 1;
        private int _hitAnimationTimeTick;

        public Blooguard(Rectangle area, SpaceHashGrid grid, int zIndex)
            : base(area, grid, zIndex)
        {
            _originalHeight = area.Height;
            _originalWidth = area.Width;
        }

        public override void HandleHit(IProjectile projectile)
        {
            base.HandleHit(projectile);

            if (this.Health > 0)
            {
                _hitAnimation = true;
                UpdateSprite();
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (!this.IsDead())
            {
                _hitAnimation = true;
                UpdateSprite();
            }
        }

        protected override void Initialize()
        {
            this.Health = 3;
            _walkLeftSprites = SpriteSheet.SpriteSheet.BlooguardWalkLeftImages;
            _walkRightSprites = SpriteSheet.SpriteSheet.BlooguardWalkRightImages;
            _stunnedSprites = SpriteSheet.SpriteSheet.BlooguardStunnedImages;
            _smashLeftSprites = SpriteSheet.SpriteSheet.BlooguardSmashLeftImages;
            _smashRightSprites = SpriteSheet.SpriteSheet.BlooguardSmashRightImages;

            int directionVal = _random.Next(0, 2);
            this.Direction = directionVal == 0 ? Enums.Direction.LEFT : Enums.Direction.RIGHT;

            this.State = BloogState.FALLING;
        }

        public override void Update()
        {
            if (_hitAnimation && _hitAnimationTimeTick++ == HIT_ANIMATION_TIME)
            {
                _hitAnimationTimeTick = 0;
                _hitAnimation = false;
            }

            if (_state != BloogState.STUNNED)
            {
                _keen = this.GetClosestPlayer();
            }

            switch (_state)
            {
                case BloogState.MOVING:
                    this.Move();
                    break;
                case BloogState.STUNNED:
                    this.UpdateStunnedState();
                    break;
                case BloogState.FALLING:
                    this.Fall();
                    break;
                case BloogState.SMASHING:
                    this.Smash();
                    break;
            }
        }

        protected bool IsKeenAlignedOnVerticalPlainAndOnTheGround()
        {
            return _keen.HitBox.Bottom == this.HitBox.Bottom
                  && (_keen.MoveState == Enums.MoveState.STANDING || _keen.MoveState == Enums.MoveState.RUNNING || _keen.MoveState == Enums.MoveState.STUNNED);
        }

        protected override void Move()
        {
            base.Move();

            if (IsKeenAlignedOnVerticalPlainAndOnTheGround())
            {
                var direction = this.SetDirectionFromObjectHorizontal(_keen, true);
                int xReferencePoint = direction == Enums.Direction.LEFT ? this.HitBox.Left : this.HitBox.Right;
                int xKeenReferencePoint = direction == Enums.Direction.LEFT ? _keen.HitBox.Right : _keen.HitBox.Left;
                int xDistanceFromKeen = Math.Abs(xReferencePoint - xKeenReferencePoint);
                if (xDistanceFromKeen <= SMASH_RANGE && this.Direction == direction)
                {
                    int smashVal = _random.Next(1, SMASH_CHANCE + 1);

                    if (smashVal == SMASH_CHANCE)
                    {
                        this.Smash();
                    }
                }
            }
        }

        private void Smash()
        {
            if (this.State != BloogState.SMASHING)
            {
                _currentSmashSprite = 0;
                this.State = BloogState.SMASHING;
                var smashDudVal = _random.Next(1, SMASH_DUD_CHANCE + 1);
                _smashDud = smashDudVal == SMASH_DUD_CHANCE;
            }
            var spriteSet = this.Direction == Enums.Direction.LEFT ? _smashLeftSprites : _smashRightSprites;
            if (_currentSmashSprite < spriteSet.Length - 1)
            {
                var spriteChangeDelay = _currentSmashSprite == 0 ? FIRST_SMASH_SPRITE_CHANGE_DELAY : SMASH_SPRITE_CHANGE_DELAY;
                this.UpdateHitboxBasedOnStunnedImage(
                   spriteSet
                   , ref _currentSmashSprite
                   , ref _currentSmashSpriteChangeDelayTick
                   , spriteChangeDelay
                   , UpdateSprite);
                foreach (var keen in CurrentPlayerList.Players)
                {
                    if (keen.IsDead())
                        continue;

                    _keen = keen;
                    if (_currentSmashSprite == spriteSet.Length - 1 && !_smashDud && IsKeenAlignedOnVerticalPlainAndOnTheGround())
                    {
                        _keen.Stun();
                    }
                }
                _keen = this.GetClosestPlayer();
            }
            else
            {
                var nextImage = Direction == Enums.Direction.LEFT ? _walkLeftSprites[0] : _walkRightSprites[0];
                int oldBottom = this.HitBox.Bottom;
                this.HitBox = new Rectangle(this.HitBox.X, this.HitBox.Y - (nextImage.Height - this.HitBox.Height), _originalWidth, _originalHeight);
                int newBottom = this.HitBox.Bottom;
                //_sprite.Location = new Point(_sprite.Location.X, _sprite.Location.Y - (newBottom - oldBottom));
                this.Fall();
            }
        }

        public override PointItemType PointItem => PointItemType.KEEN6_ROOT_BEER_FLOAT;

        protected override void UpdateSprite()
        {
            base.UpdateSprite();
            if (this.State == BloogState.SMASHING)
            {
                var spriteSet = this.Direction == Enums.Direction.LEFT ? _smashLeftSprites : _smashRightSprites;
                if (_currentSmashSprite < spriteSet.Length)
                {
                    _sprite = spriteSet[_currentSmashSprite];
                }

            }

            if (_hitAnimation)
            {
                _sprite = this.GetCurrentSpriteWithWhiteBackground(_sprite);
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var _area = this.HitBox;
            string imageName = nameof(Properties.Resources.keen6_blooguard_right4);
            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}";
        }
    }
}
