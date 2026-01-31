using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles
{
    public abstract class Keen5PowerGenerator : CollisionObject, IUpdatable, ISprite, ICreateRemove, IActivator
    {
        protected readonly int _zIndex;
        protected readonly Rectangle _area;
        protected readonly ObjectiveEventType _eventType;
        protected readonly IActivateable[] _activateables;
        protected abstract int GLASS_X_OFFSET { get; }

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;
        public event EventHandler<ToggleEventArgs> Toggled;

        protected int _currentSpriteIndex;

        private int _currentSpriteChangeDelayTick;

        protected Image _sprite;
        protected bool _foreGroundAdded;
        protected InvisibleTile _leftSideCollisionArea;
        protected InvisibleTile _rightSideCollisionArea;

        protected List<Keen5GeneratorGlass> _glassObjects = new List<Keen5GeneratorGlass>();

        private bool _stopUpdatingGlass;
        private bool _toggled;

        public Keen5PowerGenerator(Rectangle area, SpaceHashGrid grid, int zIndex, ObjectiveEventType eventType, IActivateable[] activateables) : base(grid, area)
        {
            _zIndex = zIndex;
            _area = area;
            this.HitBox = area;
            _eventType = eventType;
            _activateables = activateables;

            Initialize();
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public List<IActivateable> ToggleObjects => _activateables?.ToList() ?? new List<IActivateable>();

        public bool IsActive => _glassObjects.Any() && _glassObjects.Any(g => !g.IsDead());

        protected virtual void Initialize()
        {
            _sprite = SpriteList[_currentSpriteIndex];

            if (_collidingNodes != null && _collisionGrid != null)
            {
                //left block hitbox
                int blockVerticalOffset = BLOCK_VERTICAL_OFFSET;
                int leftBlockHorizontalOffset = LEFT_BLOCK_HORIZONTAL_OFFSET;
                Rectangle leftBlockHitbox = new Rectangle(_area.X + leftBlockHorizontalOffset,
                    _area.Y + blockVerticalOffset,
                    LEFT_BLOCK_WIDTH - leftBlockHorizontalOffset,
                    _area.Height - blockVerticalOffset);
                _leftSideCollisionArea = new InvisibleTile(_collisionGrid, leftBlockHitbox, true);

                //right block hitbox
                int rightBlockHorizontalOffset = RIGHT_BLOCK_HORIZONTAL_OFFSET;
                int rightBlockWidth = RIGHT_BLOCK_WIDTH;
                Rectangle rightBlockHitbox = new Rectangle(_area.X + rightBlockHorizontalOffset,
                    _area.Y + blockVerticalOffset,
                    rightBlockWidth, _area.Height - blockVerticalOffset);
                _rightSideCollisionArea = new InvisibleTile(_collisionGrid, rightBlockHitbox, true);


                //power node glass objects
                foreach (int yOffset in this.YOffsets)
                {
                    Rectangle powerNodeHitbox = new Rectangle(_area.X + GLASS_X_OFFSET,
                     _area.Y + yOffset,
                     Keen5PowerNodeGlass.IMAGE_WIDTH, Keen5PowerNodeGlass.IMAGE_HEIGHT);

                    Keen5GeneratorGlass glass = _eventType == ObjectiveEventType.DEACTIVATE
                        ? new Keen5PowerNodeGlass(_collisionGrid, powerNodeHitbox, _zIndex, this)
                        : new Keen5GeneratorGlass(_collisionGrid, powerNodeHitbox, _zIndex);

                    _glassObjects.Add(glass);
                }
            }
            DrawCombinedImage(true);
        }

        protected void DrawCombinedImage(bool initialDrawing = false)
        {
            List<Point> points = new List<Point>() { new Point(0, 0) };
            List<Image> images = new List<Image>() { _sprite };
            for(int i = 0; i < this.YOffsets.Length; i++)
            {
                int yOffset = this.YOffsets[i];
                Image newImg = initialDrawing
                ? SpriteSheet.SpriteSheet.Keen5GlassGeneratorSprites.FirstOrDefault()
                : _glassObjects?[i]?.Image;

                points.Add(new Point(GLASS_X_OFFSET, yOffset));
                images.Add(newImg);
            }

            _sprite = BitMapTool.DrawImagesOnCanvas(_area.Size,
                null, images.ToArray(), points.ToArray());
        }

        protected void AddCrossbarForeGround()
        {
            if (_leftSideCollisionArea == null || this.Create == null || CrossBars == null || !CrossBars.Any())
                return;


            foreach (var crossbar in CrossBars)
            {
                if (crossbar?.Image == null)
                    continue;

                Rectangle leftBlockHitbox = _leftSideCollisionArea.HitBox;
                var crossbarImg = crossbar.Image;
                int crossbarHorizontalOffset = crossbar.HorizontalOffset;
                int crossBarVerticalOffset = crossbar.VerticalOffset;
                Rectangle crossbarArea = new Rectangle(leftBlockHitbox.Right + crossbarHorizontalOffset
                    , _area.Y + crossBarVerticalOffset, crossbarImg.Width, crossbarImg.Height);
                Background bgCrossbar = new Background(crossbarArea, crossbarImg, false, crossbar.ZIndex);
                this.Create?.Invoke(this, new ObjectEventArgs { ObjectSprite = bgCrossbar });
            }
           
            _foreGroundAdded = true;
        }

        protected bool ValidGlassDeadState()
        {
            if (!_glassObjects.Any())
                return false;
            if (!_glassObjects.All(g => g.IsDead()))
                return false;

            return true;
        }

        public void Update()
        {
            if (!ValidGlassDeadState())
            {
                UpdateAllGlassNodes();
                DrawCombinedImage();
            }
            else if (ValidGlassDeadState() && !_stopUpdatingGlass)
            {
                UpdateAllGlassNodes();
                DrawCombinedImage();
                _stopUpdatingGlass = true;
            }

            if (!_foreGroundAdded)
            {
                this.AddCrossbarForeGround();
            }

            this.UpdateSprite();
        }

        protected void UpdateSprite()
        {
            this.UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSpriteIndex, SPRITE_CHANGE_DELAY,
                () =>
                {
                    if (_currentSpriteIndex >= SpriteList.Length)
                    {
                        _currentSpriteIndex = 0;
                    }
                    _sprite = SpriteList[_currentSpriteIndex];
                    DrawCombinedImage();
                });
        }

        protected void UpdateAllGlassNodes()
        {
            foreach (var glass in _glassObjects)
            {
                glass.Update();
            }
        }

        public void Toggle()
        {
            if (_toggled || _activateables == null || !_activateables.Any() || !ValidGlassDeadState())
                return;

            foreach (var component in _activateables)
            {
                component.Deactivate();
            }

            this.Toggled?.Invoke(this, new ToggleEventArgs() { IsActive = this.IsActive });
            _toggled = true;
        }

        protected abstract List<ICrossBar> CrossBars { get; }

        protected abstract Image[] SpriteList { get; }

        protected abstract int SPRITE_CHANGE_DELAY { get; }

        protected abstract int BLOCK_VERTICAL_OFFSET { get; }

        protected abstract int[] YOffsets
        {
            get;
        }

        protected abstract int LEFT_BLOCK_HORIZONTAL_OFFSET
        {
            get;
        }

        protected abstract int RIGHT_BLOCK_HORIZONTAL_OFFSET
        {
            get;
        }

        protected abstract int LEFT_BLOCK_WIDTH
        {
            get;
        }

        protected abstract int RIGHT_BLOCK_WIDTH
        {
            get;
        }
    }

    public interface ICrossBar : ISprite
    {
        int HorizontalOffset { get; }

        int VerticalOffset { get; }
    }
}
