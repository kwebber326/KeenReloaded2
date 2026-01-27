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
using System.Xml.Linq;

namespace KeenReloaded2.Framework.GameEntities.Tiles.InteractiveTiles
{
    public class Keen5PowerOmegamaticGenerator1 : CollisionObject, IUpdatable, ISprite, ICreateRemove
    {
        private Image _sprite;
        private readonly int _zIndex;
        private readonly Rectangle _area;
        private readonly Image[] _spritesList = SpriteSheet.SpriteSheet.Keen5PowerGenerator1Images;
        private const int SPRITE_CHANGE_DELAY = 4;
        private int _currentSpriteChangeDelayTick;
        private int _currentSpriteIndex;
        private bool _stopUpdatingGlass;
        private bool _foreGroundAdded;

        private InvisibleTile _metalPipeCollisionArea;
        private InvisibleTile _leftSideCollisionArea;
        private InvisibleTile _rightSideCollisionArea;
        private Keen5GeneratorGlass _glass;
        private const int GLASS_X_OFFSET = 172;
        private const int GLASS_Y_OFFSET = 56;
        private ObjectiveEventType _eventType;
        private IActivateable[] _activateables;

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        public Keen5PowerOmegamaticGenerator1(Rectangle area, SpaceHashGrid grid, int zIndex, ObjectiveEventType eventType, IActivateable[] activateables) : base(grid, area)
        {
            _zIndex = zIndex;
            _area = area;
            _eventType = eventType;
            this.HitBox = area;
            _activateables = activateables;
            Initialize();
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public override CollisionType CollisionType => CollisionType.NONE;

        public void Update()
        {
            if (!(_glass?.IsDead() ?? true))
            {
                _glass.Update();
                DrawCombinedImage();
            }
            else if (_glass != null && !_stopUpdatingGlass)
            {
                _glass.Update();
                DrawCombinedImage();
                _stopUpdatingGlass = true;
            }

            if (!_foreGroundAdded)
            {
                this.AddCrossbarForeGround();
            }

            this.UpdateSprite();
        }

        private void Initialize()
        {
            _sprite = _spritesList[_currentSpriteIndex];

            if (_collidingNodes != null && _collisionGrid != null)
            {
                //metal pipe hitbox
                Rectangle metalPipeHitbox = new Rectangle(_area.X, _area.Y + 74, 26, 46);
                _metalPipeCollisionArea = new InvisibleTile(_collisionGrid, metalPipeHitbox, true);

                //left block hitbox
                int blockVerticalOffset = 28;
                int leftBlockHorizontalOffset = 6;
                Rectangle leftBlockHitbox = new Rectangle(_area.X + metalPipeHitbox.Width + leftBlockHorizontalOffset,
                    _area.Y + blockVerticalOffset,
                    100 - leftBlockHorizontalOffset,
                    _area.Height - blockVerticalOffset);
                _leftSideCollisionArea = new InvisibleTile(_collisionGrid, leftBlockHitbox, true);

                //right block hitbox
                int rightBlockHorizontalOffset = 228;
                int rightBlockWidth = 70;
                Rectangle rightBlockHitbox = new Rectangle(_area.X + rightBlockHorizontalOffset,
                    _area.Y + blockVerticalOffset,
                    rightBlockWidth, _area.Height - blockVerticalOffset);
                _rightSideCollisionArea = new InvisibleTile(_collisionGrid, rightBlockHitbox, true);


                //power node glass object
                Rectangle powerNodeHitbox = new Rectangle(_area.X + GLASS_X_OFFSET,
                    _area.Y + GLASS_Y_OFFSET,
                    Keen5PowerNodeGlass.IMAGE_WIDTH, Keen5PowerNodeGlass.IMAGE_HEIGHT);

                if (_eventType == ObjectiveEventType.DEACTIVATE)
                {
                    Keen5PowerNodeGlass keen5PowerNodeGlass = new Keen5PowerNodeGlass(_collisionGrid, powerNodeHitbox, _zIndex, _activateables);
                    _glass = keen5PowerNodeGlass;
                }
                else if (_eventType == ObjectiveEventType.LEVEL_EXIT)
                {
                    Keen5GeneratorGlass keen5GeneratorGlass = new Keen5GeneratorGlass(_collisionGrid, powerNodeHitbox, _zIndex);
                    _glass = keen5GeneratorGlass;
                }
            }
            DrawCombinedImage(true);
        }

        private void AddCrossbarForeGround()
        {
            if (_leftSideCollisionArea == null || this.Create == null)
                return;

            Rectangle leftBlockHitbox = _leftSideCollisionArea.HitBox;
            var crossbarImg = Properties.Resources.keen5_omegamatic_first_machine_crossbar;
            int crossbarHorizontalOffset = 4;
            int crossBarVerticalOffset = 164;
            Rectangle crossbarArea = new Rectangle(leftBlockHitbox.Right + crossbarHorizontalOffset
                , _area.Y + crossBarVerticalOffset, crossbarImg.Width, crossbarImg.Height);
            Background bgCrossbar = new Background(crossbarArea, crossbarImg, false, 201);
            this.Create?.Invoke(this, new ObjectEventArgs { ObjectSprite = bgCrossbar });
            _foreGroundAdded = true;
        }

        private void DrawCombinedImage(bool initialDrawing = false)
        {
            Image newImg = initialDrawing
                ? SpriteSheet.SpriteSheet.Keen5GlassGeneratorSprites.FirstOrDefault()
                : _glass?.Image;

            _sprite = BitMapTool.DrawImagesOnCanvas(_area.Size,
                null, new Image[] { _sprite, newImg },
                new Point[] { new Point(0, 0), new Point(GLASS_X_OFFSET, GLASS_Y_OFFSET) });
        }

        private void UpdateSprite()
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

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.keen5_omegamatic_first_machine1);
            string arrayItemSeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
            string data = $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_eventType}";
            if (_activateables != null)
            {
                data += $"{separator}{string.Join(arrayItemSeparator, _activateables.Select(a => a.ActivationID))}";
            }
            return data;
        }
    }
}
