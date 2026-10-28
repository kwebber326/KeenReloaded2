using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEntities.WorldMapEntities.Transporters.Miscellaneous;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities.Transporters
{
    public class Keen5SpaceshipTeleporter : CollisionObject, IUpdatable, ISprite, IPlayerTransporter, ICreateRemove
    {
        private readonly Rectangle _area;
        private readonly Point _destination;
        private readonly InvisibleTile _ceilingTile;
        private readonly InvisibleTile _leftWallTile;
        private readonly InvisibleTile _rightWallTile;
        private int _zIndex;
        private Image _sprite;
        private bool _isActive;
        private bool _inDelaySequence;
        private bool _delivered;
        private Image[] _sprites = SpriteSheet.SpriteSheet.EnemySpawnerImages;

        private const int TRANSPORT_DELAY = 20;
        private int _transportDelayTick;
        private const int SPRITE_CHANGE_DELAY = 2;
        private int _currentSpriteChangeDelayTick;
        private int _currentActiveSpriteIndex;
        private WorldMapPlayer _currentPlayer;
        private Keen5TransporterLightning _lightning;

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        public Keen5SpaceshipTeleporter(Rectangle area, SpaceHashGrid grid, int zIndex, int desinationX, int destinationY) : base(grid, area)
        {
            _area = area;
            _zIndex = zIndex;
            _destination = new Point(desinationX, destinationY);
            if (_collisionGrid != null && _collidingNodes != null)
            {
                _ceilingTile = new InvisibleTile(grid,
                     new Rectangle(area.X, area.Y, area.Width, 50));
                _leftWallTile = new InvisibleTile(grid,
                    new Rectangle(area.X, area.Y + 50, 30, 44));
                _rightWallTile = new InvisibleTile(grid,
                    new Rectangle(area.X + 61, area.Y + 50, 30, 44));
            }
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (!_isActive)
            {
                _sprite = _sprites[0];
            }
            else if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                int index = _currentActiveSpriteIndex > 1 ? 1 : 2;
                _sprite = _sprites[_currentActiveSpriteIndex];
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public int TransportSpeed => 1000;

        public Point Destination => _destination;

        public WorldMapPlayerMoveState DesiredPlayerMoveState => WorldMapPlayerMoveState.TRANSPORTING;

        public bool IsActive => _isActive;

        public void TransportPlayer(WorldMapPlayer player)
        {
            _isActive = true;
            _inDelaySequence = true;
            _delivered = false;
            _currentPlayer = player;
            this.PublishSoundPlayEvent(
                GeneralGameConstants.Sounds.TELEPORT);
            CreateLightning();
        }

        private void CreateLightning()
        {
            if (_currentPlayer == null)
                return;

            _lightning = new Keen5TransporterLightning(_currentPlayer.HitBox,
                _collisionGrid, _currentPlayer.ZIndex + 1, _currentPlayer);
            Create?.Invoke(this, new ObjectEventArgs() { ObjectSprite = _lightning });
        }

        private void DestroyLightning()
        {
            _lightning.Destroy();
            Remove?.Invoke(this, new ObjectEventArgs() { ObjectSprite = _lightning });
        }

        private void ReleaseTransportLockOnPlayer()
        {
            _isActive = false;
            _inDelaySequence = false;
            _delivered = false;
            _currentPlayer.ReleaseTransportState();
            _currentPlayer = null;
            DestroyLightning();
        }

        public void Update()
        {
            if (!_isActive || _currentPlayer == null)
                return;

            if (_inDelaySequence)
            {
                ExecuteDelayProtocol(!_delivered);
            }
            else if (!_currentPlayer.HitBox.Location.Equals(Destination))
            {
                SendPlayerToDestination();
            }
            else
            {
                _delivered = true;
                _inDelaySequence = true;
                this.PublishSoundPlayEvent(GeneralGameConstants.Sounds.TELEPORT);
            }
            UpdateSprite();
        }

        private void ExecuteDelayProtocol(bool forceCenter)
        {
            if (_transportDelayTick++ == TRANSPORT_DELAY)
            {
                _transportDelayTick = 0;
                SendPlayerToDestination();
                _inDelaySequence = false;
                if (!forceCenter)
                {
                    ReleaseTransportLockOnPlayer();
                }
            }
            else if (forceCenter)
            {
                ForcePlayerToCenter();
            }

        }

        private void SendPlayerToDestination()
        {
            if (_currentPlayer == null)
                return;

            int xDistanceToTarget = Destination.X - _currentPlayer.HitBox.Location.X;
            int yDistanceToTarget = Destination.Y - _currentPlayer.HitBox.Location.Y;

            int xCoefficient = xDistanceToTarget < 0 ? -1 : 1;
            int yCoefficient = yDistanceToTarget < 0 ? -1 : 1;

            int xDist = Math.Abs(xDistanceToTarget) >= TransportSpeed
                ? TransportSpeed * xCoefficient : xDistanceToTarget;

            int yDist = Math.Abs(yDistanceToTarget) >= TransportSpeed
                ? TransportSpeed * yCoefficient : yDistanceToTarget;

            Point newPos = new Point(_currentPlayer.HitBox.X + xDist, _currentPlayer.HitBox.Y + yDist);
            _currentPlayer.ForceNewPosition(newPos, this);
        }

        private void ForcePlayerToCenter()
        {
            int xCenter = (this.HitBox.X + (this.HitBox.Width / 2))
                - (_currentPlayer.HitBox.Width / 2);
            int yCenter = (this.HitBox.Y + (this.HitBox.Height / 2))
                - (_currentPlayer.HitBox.Height / 2) + 16;

            Point center = new Point(xCenter, yCenter);
            _currentPlayer.ForceNewPosition(center, this);
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imgName = "world_map_teleporter";

            return $"{imgName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}" +
                   $"{separator}{_zIndex}{separator}{_destination.X}{separator}{_destination.Y}";
        }
    }
}
