using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.GameEntities.Animations;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
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
    public class Keen4MirageWorldMapLevel : AnimatedForegroundMultiHitboxWorldMapLevel
    {
        List<Background> _foregrounds = new List<Background>();
        protected MirageState _state = MirageState.HOLD_ON;
        protected const int HOLD_TIME = 100;
        protected int _holdTimeTick = 0;
        protected int _animationDelayTick = 0;
        public Keen4MirageWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Guid activationId, Rectangle[] entryPoints, Rectangle[] hitboxes, Rectangle[] foregroundAreas, string imagesPath, int animationDelay, int animationStartIndex, string key)
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, activationId, entryPoints, hitboxes, foregroundAreas, imagesPath, animationDelay, animationStartIndex, key)
        {
            _animation.Stop();
            if (_collidingNodes != null && _collisionGrid != null)
            {
                InitializeForegroundObjects();
            }
        }

        public override void Update()
        {
            ChangeDisplayState();
        }

        private void ChangeDisplayState()
        {
            switch (_state)
            {
                case MirageState.HOLD_ON:
                case MirageState.HOLD_OFF:
                    if (++_holdTimeTick >= HOLD_TIME)
                    {
                        _holdTimeTick = 0;
                        ReconstructAnimation();
                        _state = _state == MirageState.HOLD_ON
                            ? MirageState.DISAPPEARING
                            : MirageState.REAPPEARING;

                        if (_state == MirageState.DISAPPEARING)
                        {
                            RemoveForegrounds();
                        }
                        _animation.Start();
                    }
                    break;
            }

        }

        private void ReconstructAnimation()
        {
            _animation.AnimationMoveNext -= _animation_AnimationMoveNext;
            _animation.AnimationEnd -= _animation_AnimationEnd;
            var list = _state == MirageState.HOLD_ON ? _imageList.ToList()
                : _imageList.Reverse().ToList();

            _animation = new Animation(list, _animationDelay, false);
            _animation.AnimationMoveNext += _animation_AnimationMoveNext;
            _animation.AnimationEnd += _animation_AnimationEnd;
        }

        private void _animation_AnimationEnd(object sender, EventArgs e)
        {
            if (_state == MirageState.DISAPPEARING)
            {
                _state = MirageState.HOLD_OFF;
                _sprite = null;
                RemoveHitBoxes();
                _animation.Stop();
                this.Deactivate();
            }
            else if (_state == MirageState.REAPPEARING)
            {
                _state = MirageState.HOLD_ON;
                CreateForegrounds();
                CreateHitboxes();
                this.Activate();
            }
        }

        private void RemoveForegrounds()
        {
            foreach (var foreground in _foregrounds)
            {
                ObjectEventArgs e = new ObjectEventArgs()
                {
                    ObjectSprite = foreground
                };
                OnRemove(e);
            }
        }

        private void RemoveHitBoxes()
        {
            foreach (var tile in _collisionTiles)
            {
                tile.RemoveTileFromGrid();
            }
        }

        private void CreateForegrounds()
        {
            foreach (var foreground in _foregrounds)
            {
                ObjectEventArgs e = new ObjectEventArgs()
                {
                    ObjectSprite = foreground
                };
                OnCreate(e);
            }
        }

        private void CreateHitboxes()
        {
            foreach (var tile in _collisionTiles)
            {
                tile.AddTileToGrid();
            }
        }

        private void InitializeForegroundObjects()
        {
            foreach (var foregroundArea in _foregroundAreas)
            {
                Image image = _sprite;
                var area = new Rectangle(this.HitBox.X + foregroundArea.X,
                    this.HitBox.Y + foregroundArea.Y, foregroundArea.Width, foregroundArea.Height);
                image = BitMapTool.CropImage(image, foregroundArea);
                Background background = new Background(area
                    , image, false, _zIndex + 200);

                _foregrounds.Add(background);
            }
        }

        protected override void _animation_AnimationMoveNext(object sender, EventArgs e)
        {
            if (_state != MirageState.HOLD_OFF)
                _sprite = _animation.CurrentImage;
        }
    }

    public enum MirageState
    {
        HOLD_ON,
        DISAPPEARING,
        HOLD_OFF,
        REAPPEARING
    }
}
