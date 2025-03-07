using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Animations;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Projectiles;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class EnemyTransporter : CollisionObject, IUpdatable, ISprite
    {
        private readonly int _zIndex;
        private Image _image;
        private EnemyTransporter _destinationNode;
        private Rectangle _portalEntryArea;
        private const int PORTAL_ENTRY_AREA_WIDTH = 42;
        private const int PORTAL_ENTRY_AREA_HEIGHT = 68;
        private bool _transportAnimationRunning;
        private readonly int _transportationAnimationDelay = 10;
        private int _transportationDelayTick = 0;
        private Image[] _transportElectricityImages = SpriteSheet.SpriteSheet.Keen6TransportElectricitySprites;
        private int _currentElectricityImageIndex = 0;

        public EnemyTransporter(Rectangle area, SpaceHashGrid grid, int zIndex, int nodeId, int? destinationNodeId, int transportationDelay)
            : base(grid, area)
        {
            this.Id = nodeId;
            _zIndex = zIndex;
            this.HitBox = area;
            this.DestinationNodeId = destinationNodeId;
            _transportationAnimationDelay = transportationDelay;
            Initialize();
        }

        private void Initialize()
        {
            _image = Properties.Resources.keen6_transporter;
            int x = this.HitBox.X + ((_image.Width - PORTAL_ENTRY_AREA_WIDTH) / 2);
            int y = this.HitBox.Y + (_image.Height - PORTAL_ENTRY_AREA_HEIGHT);
            _portalEntryArea = new Rectangle(x, y, PORTAL_ENTRY_AREA_WIDTH, PORTAL_ENTRY_AREA_HEIGHT);
        }

        public int Id { get; private set; }

        public int? DestinationNodeId { get; set; }

        public EnemyTransporter DestinationNode
        {
            get
            {
                return _destinationNode;
            }
            set
            {
                _destinationNode = value;
                this.DestinationNodeId = _destinationNode?.Id ?? 0;
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _image;

        public Point Location => this.HitBox.Location;

        public bool CanUpdate => true;

        private void StartDestinationNodeAnimation()
        {
            if (this.DestinationNode == null)
                return;

            this.DestinationNode._transportationDelayTick = 0;
            this.DestinationNode._transportAnimationRunning = true;
        }

        private bool IsEnemyMovable(CollisionObject collision)
        {
            var enemy = collision as DestructibleObject;
            return (enemy == null || (enemy != null && !enemy.IsDead()));
        }

        public void Update()
        {
            if (this.DestinationNode == null)
            {
                if (_transportAnimationRunning)
                    _transportAnimationRunning = false;
                return;
            }

            if (!_transportAnimationRunning)
            {
                var collisions = this.CheckCollision(_portalEntryArea);
                var enemyCollisions = collisions.Where(c => c.CollisionType == CollisionType.ENEMY && IsEnemyMovable(c)).ToList();
                if (enemyCollisions.Any() && !this.DestinationNode._transportAnimationRunning)
                {
                    _transportAnimationRunning = true;
                    this.StartDestinationNodeAnimation();
                    var transportArea = this._destinationNode._portalEntryArea;
                    foreach (var collision in enemyCollisions)
                    {
                        collision.MoveToPosition(transportArea.Location);
                    }
                }
            }
            else if (++_transportationDelayTick >= _transportationAnimationDelay)
            {
                _transportationDelayTick = 0;
                _transportAnimationRunning = false;
                _image = Properties.Resources.keen6_transporter;
            }
            else
            {
                if (++_currentElectricityImageIndex >= _transportElectricityImages.Length)
                    _currentElectricityImageIndex = 0;

                var canvasSize = new Size(_image.Width, _image.Height);
                var electricImage = _transportElectricityImages[_currentElectricityImageIndex];
                var x = (_image.Width / 2) - (electricImage.Width / 2);
                var y = _image.Height - (electricImage.Height * 2);
                _image = BitMapTool.DrawImagesOnCanvas(
                    canvasSize,
                    null,
                    new Image[] { Properties.Resources.keen6_transporter, electricImage },
                    new Point[] { new Point(0, 0), new Point(x, y) });
            }
        }

        public override string ToString()
        {
            var separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            var destinationNodeString = this.DestinationNode?.Id.ToString() ?? this.DestinationNodeId?.ToString() ?? string.Empty;
            var area = this.HitBox;
            var imageName = nameof(Properties.Resources.keen6_transporter);
            return $"{imageName}{separator}{area.X}{separator}{area.Y}{separator}{area.Width}{separator}{area.Height}{separator}{_zIndex}{separator}{this.Id.ToString()}{separator}{destinationNodeString}{separator}{_transportationAnimationDelay}";
        }
    }
}
