using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities.Transporters
{
    public class Keen5DoorTransporter : CollisionObject, IUpdatable, ISprite, IActivateable, IPlayerTransporter, ICreateRemove
    {
        private readonly int _zIndex;
        private readonly Rectangle _area;
        private readonly Point _desination;
        private InvisibleTile _blockTile;
        private InvisibleTile _ceilingTile;
        private InvisibleTile _leftWallTile;
        private InvisibleTile _rightWallTile;
        private Image _sprite;
        private bool _isActive;
        private Guid _activationID;
        private Image[] _sprites = SpriteSheet.SpriteSheet.WeaponGeneratorImages;

        private const int RELEASE_SPEED = 6;
        private const int SPRITE_CHANGE_DELAY = 4;
        private int _currentSpriteIndex;
        private int _currentSpriteChangeDelayTick;

        bool _transporting;
        bool _shuttingDoor;
        bool _resetting;
        bool _delivered;
        private bool _releasing;
        private WorldMapPlayer _currentPlayer = null;


        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        public Keen5DoorTransporter(Rectangle area, SpaceHashGrid grid, int zIndex, Guid activationId, bool isActive, int desinationX, int destinationY)
            : base(grid, area)
        {
            _area = area;
            _zIndex = zIndex;
            _activationID = activationId;
            _isActive = isActive;
            _desination = new Point(desinationX, destinationY);
            UpdateSprite();
            if (_collisionGrid != null && _collidingNodes != null)
            {
                _blockTile = new InvisibleTile(grid, area);
                if (!_isActive)
                {
                    _blockTile.RemoveTileFromGrid();
                }
                _ceilingTile = new InvisibleTile(grid,
                        new Rectangle(area.X, area.Y, area.Width, 26));
                _leftWallTile = new InvisibleTile(grid,
                    new Rectangle(area.X, area.Y, 24, area.Height));
                _rightWallTile = new InvisibleTile(grid,
                    new Rectangle(area.X + 100, area.Y, 24, area.Height));
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public bool CanUpdate => true;

        public bool IsActive => _isActive;

        public Guid ActivationID => _activationID;

        public int TransportSpeed => 40;

        public Point Destination => _desination;

        public WorldMapPlayerMoveState DesiredPlayerMoveState => WorldMapPlayerMoveState.TRANSPORTING;

        public void Activate()
        {
            _isActive = true;
            if (!_collidingNodes.Any(c => c.Tiles.Contains(_blockTile)))
            {
                _blockTile.AddTileToGrid();
            }
            UpdateSprite();
        }

        public void Deactivate()
        {
            _isActive = false;
            _blockTile.RemoveTileFromGrid();
            UpdateSprite();
        }

        public void AttachPlayerForDoorReleaseProtocol(WorldMapPlayer player)
        {
            _releasing = true;
            _currentPlayer = player;
            this.PublishSoundPlayEvent(
    GeneralGameConstants.Sounds.KEEN5_TRANSPORTER_DOOR_OPEN);
        }

        public void Update()
        {
            //transporting case            
            if (_transporting && _currentPlayer != null && !_delivered)
            {
                ExecuteTransportProtocol();
            }
            //releasing case
            else if (_currentPlayer != null && _releasing)
            {
                ExecuteReleasePlayerProtocol();
            }
            else if (_resetting)
            {
                UpdateSprite();
            }
        }

        private void ExecuteReleasePlayerProtocol()
        {
            if (_currentPlayer != null && _currentPlayer.HitBox.Top <= this.HitBox.Bottom)
            {
                Point p = new Point(_currentPlayer.HitBox.X, _currentPlayer.HitBox.Y + RELEASE_SPEED);
                _currentPlayer.ForceNewPosition(p, this);
            }
            else
            {
                ReleaseTransportLockOnPlayer();
            }
            UpdateSprite();
        }

        private void ExecuteTransportProtocol()
        {
            if (_shuttingDoor)
            {
                ForcePlayerToCenter();
            }
            else if (!_currentPlayer.HitBox.Location.Equals(Destination))
            {
                SendPlayerToDestination();
            }
            else
            {
                _delivered = true;
                var transporterCollisions =
                    this.CheckCollision(_currentPlayer.HitBox)
                    .OfType<Keen5DoorTransporter>()?.ToList()
                    ?? new List<Keen5DoorTransporter>();
                var transporter = transporterCollisions.FirstOrDefault(t => t != this);
                if (transporter != null)
                {
                    transporter.Deactivate();
                    ForcePlayerToCenter(transporter);
                    transporter.AttachPlayerForDoorReleaseProtocol(_currentPlayer);
                }
                else
                {
                    ReleaseTransportLockOnPlayer();
                }
            }
            UpdateSprite();
        }

        private void ReleaseTransportLockOnPlayer()
        {
            _transporting = false;
            _releasing = false;
            _delivered = false;
            _currentPlayer.ReleaseTransportState();
            Create?.Invoke(this, new ObjectEventArgs() { ObjectSprite = _currentPlayer });
            _currentPlayer = null;
        }

        private void SendPlayerToDestination()
        {
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
                - (_currentPlayer.HitBox.Height / 2);

            Point center = new Point(xCenter, yCenter);
            _currentPlayer.ForceNewPosition(center, this);
        }

        private void ForcePlayerToCenter(Keen5DoorTransporter transporter)
        {
            int xCenter = (transporter.HitBox.X + (transporter.HitBox.Width / 2))
                - (_currentPlayer.HitBox.Width / 2);
            int yCenter = (transporter.HitBox.Y + (transporter.HitBox.Height / 2))
                - (_currentPlayer.HitBox.Height / 2);

            Point center = new Point(xCenter, yCenter);
            _currentPlayer.ForceNewPosition(center, transporter);
        }

        private void UpdateSprite()
        {

            if (_shuttingDoor)
            {
                UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSpriteIndex, SPRITE_CHANGE_DELAY,
                    () =>
                    {
                        if (_currentSpriteIndex > 0 && _currentSpriteIndex < _sprites.Length)
                        {
                            int index = _sprites.Length - _currentSpriteIndex;
                            _sprite = _sprites[index];
                        }
                        else
                        {
                            _currentSpriteIndex = 0;
                            _sprite = _sprites[_currentSpriteIndex];
                            _shuttingDoor = false;
                            _resetting = true;
                        }
                    });
                return;
            }
            if (!_isActive && (_resetting || _releasing))
            {
                UpdateSpriteByDelayBase(ref _currentSpriteChangeDelayTick, ref _currentSpriteIndex, SPRITE_CHANGE_DELAY,
                   () =>
                   {
                       if (_currentSpriteIndex < _sprites.Length - 1)
                       {
                           _sprite = _sprites[_currentSpriteIndex];
                       }
                       else
                       {
                           _sprite = _sprites[_currentSpriteIndex];
                           _shuttingDoor = false;
                           _resetting = false;
                           _currentSpriteIndex = 0;
                       }
                   });
                return;
            }
            _sprite = _isActive ? _sprites[0] : _sprites[_sprites.Length - 1];
        }

        public void TransportPlayer(WorldMapPlayer player)
        {
            if (_isActive || _resetting)
                return;

            Remove?.Invoke(this, new ObjectEventArgs() { ObjectSprite = player });
            _transporting = true;
            _shuttingDoor = true;
            _delivered = false;
            _currentPlayer = player;
            this.PublishSoundPlayEvent(
                GeneralGameConstants.Sounds.KEEN5_TRANSPORTER_DOOR_OPEN);
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imgName = nameof(Properties.Resources.random_item_generator_closed) + "_world_map";

            return $"{imgName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}" +
                   $"{separator}{_zIndex}{separator}{_activationID}{separator}{_isActive}{separator}{_desination.X}{separator}{_desination.Y}";
        }
    }
}
