using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles
{
    public class Keen5GeneratorGlass : DestructibleCollisionTile, IUpdatable, ISprite
    {
        private readonly int _zIndex;
        private Image _sprite;
        private bool _firstDeathEvaluation = true;
        private Image[] _glassSprites = SpriteSheet.SpriteSheet.Keen5GlassGeneratorSprites;
        private int _currentSpriteIndex = 0;

        public const int IMAGE_WIDTH = 32;
        public const int IMAGE_HEIGHT = 20;

        private CommanderKeen _keen;

        public Keen5GeneratorGlass(SpaceHashGrid grid, Rectangle hitbox, int zIndex) : base(grid, hitbox, true)
        {
            _zIndex = zIndex;
            _sprite = _glassSprites.FirstOrDefault();
            LevelCompleteObjectives.TryAddTileObjective(this);
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        public override TileDestroyedEventType EventType => TileDestroyedEventType.LEVEL_EXIT;

        public void Update()
        {
            if (!_isDead)
            {
                UpdateSprite();
            }
            else if (_firstDeathEvaluation)
            {
                _keen = GetClosestAlivePlayer();
                if (_keen != null)
                {
                    _firstDeathEvaluation = false;
                    _sprite = Properties.Resources.keen5_destructible_glass_tile_destroyed;
                    EventStore<string>.Publish(MapMakerConstants.EventStoreEventNames.EVENT_SOUND_PLAY,
                        GeneralGameConstants.Sounds.GLASS_BREAK);
                    if (LevelCompleteObjectives.AreAllTileObjectivesComplete())
                    {
                        _keen.PassLevel();
                    }
                }
            }
        }

        private void UpdateSprite()
        {
            if (_currentSpriteIndex == 0)
                _currentSpriteIndex = 1;
            else
                _currentSpriteIndex = 0;

            _sprite = _glassSprites[_currentSpriteIndex];
        }

        public override bool Equals(object obj)
        {
            return obj is Keen5GeneratorGlass && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
