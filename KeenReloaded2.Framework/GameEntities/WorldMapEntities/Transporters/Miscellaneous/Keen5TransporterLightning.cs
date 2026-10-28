using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.Interfaces;
using System.Data;
using System.Drawing;
namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities.Transporters.Miscellaneous
{
    internal class Keen5TransporterLightning : CollisionObject, IUpdatable, ISprite
    {
        private int _zIndex;
        private readonly WorldMapPlayer _player;
        private Image _sprite;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _spriteChangeDelayTick;
        private int _currentSpriteIndex;
        private Image[] _sprites = SpriteSheet.SpriteSheet.Keen5TeleporterLightningImages;


        public Keen5TransporterLightning(Rectangle area, SpaceHashGrid grid, int zIndex, WorldMapPlayer player) : base(grid, area)
        {
            _zIndex = zIndex;
            _player = player;
            _player.Moved += _player_Moved;
        }

        private void _player_Moved(object sender, System.EventArgs e)
        {
            this.HitBox = new Rectangle(_player.Location, this.HitBox.Size);
        }

        public void Destroy()
        {
            _player.Moved -= _player_Moved;
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.NONE;
        public void Update()
        {
            UpdateSpriteByDelayBase(ref _spriteChangeDelayTick, ref _currentSpriteIndex,
                SPRITE_CHANGE_DELAY, () =>
                {
                    if (_currentSpriteIndex >= _sprites.Length)
                        _currentSpriteIndex = 0;

                    _sprite = _sprites[_currentSpriteIndex];
                });
        }
    }
}
