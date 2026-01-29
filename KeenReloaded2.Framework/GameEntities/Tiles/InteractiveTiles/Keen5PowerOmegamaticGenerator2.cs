using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
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
    public class Keen5PowerOmegamaticGenerator2 : CollisionObject, IUpdatable, ISprite, ICreateRemove, IActivator
    {
        protected readonly int _zIndex;
        protected readonly Rectangle _area;
        protected readonly ObjectiveEventType _eventType;
        protected readonly IActivateable[] _activateables;
        protected readonly Image[] _spritesList = SpriteSheet.SpriteSheet.Keen5PowerGenerator2Images;
        protected readonly int GLASS_X_OFFSET = 136;
        private readonly int GLASS_Y_OFFSET1 = 56;
        private readonly int GLASS_Y_OFFSET2 = 122;
        protected const int BLOCK_VERTICAL_OFFSET = 28;

        protected readonly int SPRITE_CHANGE_DELAY = 2;
        protected Image _sprite;
        protected bool _foreGroundAdded;
        protected InvisibleTile _leftSideCollisionArea;
        protected InvisibleTile _rightSideCollisionArea;
        protected int _currentSpriteIndex;

        private List<Keen5GeneratorGlass> _glassObjects = new List<Keen5GeneratorGlass>();
        private bool _stopUpdatingGlass;
        private int _currentSpriteChangeDelayTick;


        public Keen5PowerOmegamaticGenerator2(Rectangle area, SpaceHashGrid grid, int zIndex, ObjectiveEventType eventType, IActivateable[] activateables) : base(grid, area)
        {
            _zIndex = zIndex;
            _area = area;
            this.HitBox = area;
            _eventType = eventType;
            _activateables = activateables;

            Initialize();
        }

        protected int[] YOffsets
        {
            get
            {
                return new int[] { GLASS_Y_OFFSET1, GLASS_Y_OFFSET2 };
            }
        }

        protected int LEFT_BLOCK_HORIZONTAL_OFFSET
        {
            get
            {
                return 6;
            }
        }

        protected int RIGHT_BLOCK_HORIZONTAL_OFFSET
        {
            get
            {
                return 200;
            }
        }

        protected int LEFT_BLOCK_WIDTH
        {
            get
            {
                return 100;
            }
        }

        protected int RIGHT_BLOCK_WIDTH
        {
            get
            {
                return 70;
            }
        }

        protected void Initialize()
        {
            _sprite = _spritesList[_currentSpriteIndex];

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

        private void AddCrossbarForeGround()
        {
            if (_leftSideCollisionArea == null || this.Create == null)
                return;

            Rectangle leftBlockHitbox = _leftSideCollisionArea.HitBox;
            var crossbarImg = Properties.Resources.keen5_omegamatic_second_machine_crossbar;
            int crossbarHorizontalOffset = 2;
            int crossBarVerticalOffset = 166;
            Rectangle crossbarArea = new Rectangle(leftBlockHitbox.Right + crossbarHorizontalOffset
                , _area.Y + crossBarVerticalOffset, crossbarImg.Width, crossbarImg.Height);
            Background bgCrossbar = new Background(crossbarArea, crossbarImg, false, 201);
            this.Create?.Invoke(this, new ObjectEventArgs { ObjectSprite = bgCrossbar });
            _foreGroundAdded = true;
        }

        protected void DrawCombinedImage(bool initialDrawing = false)
        {
            Image newImg = initialDrawing
                ? SpriteSheet.SpriteSheet.Keen5GlassGeneratorSprites.FirstOrDefault()
                : _glassObjects?[0]?.Image;

            List<Point> points = new List<Point>() { new Point(0, 0) };
            List<Image> images = new List<Image>() { _sprite };
            foreach (int yOffset in this.YOffsets)
            {
                points.Add(new Point(GLASS_X_OFFSET, yOffset));
                images.Add(newImg);
            }

            _sprite = BitMapTool.DrawImagesOnCanvas(_area.Size,
                null, images.ToArray(), points.ToArray());
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public List<IActivateable> ToggleObjects => _activateables?.ToList() ?? new List<IActivateable>();

        public bool IsActive => _glassObjects.Any() && _glassObjects.Any(g => !g.IsDead());

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;
        public event EventHandler<ToggleEventArgs> Toggled;

        protected bool ValidGlassDeadState()
        {
            if (!_glassObjects.Any())
                return false;
            if (!_glassObjects.All(g => g.IsDead()))
                return false;

            return true;
        }

        public void Toggle()
        {
            if (_activateables == null || !_activateables.Any() || !ValidGlassDeadState())
                return;

            foreach (var component in _activateables)
            {
                component.Deactivate();
            }

            this.Toggled?.Invoke(this, new ToggleEventArgs() { IsActive = this.IsActive });
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
                    if (_currentSpriteIndex >= _spritesList.Length)
                    {
                        _currentSpriteIndex = 0;
                    }
                    _sprite = _spritesList[_currentSpriteIndex];
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

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen5_omegamatic_second_machine1);
            string arrayItemSeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;
            string data = $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_eventType}";
            if (_activateables != null)
            {
                data += $"{separator}{arrayStart}{string.Join(arrayItemSeparator, _activateables.Select(a => a.ActivationID))}{arrayEnd}";
            }
            return data;
        }
    }
}
