using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
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
namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class BiomeProjectile : CollisionObject, IProjectile, IUpdatable, ISprite, ICreateRemove
    {
        protected int _verticalVelocity;
        protected int _horizontalVelocity;
        protected Image _sprite;

        protected const int AIR_RESISTANCE = 2;
        protected const int GRAVITY_ACCELERATION = 5;
        private const int MAX_FALL_VELOCITY = 150;

        protected bool _removed;

        public BiomeProjectile(SpaceHashGrid grid, Rectangle hitbox, int initialVerticalVelocity, int initialHorizontalVelocity, string biome)
            : base(grid, hitbox)
        {
            _biome = biome;
            _verticalVelocity = initialHorizontalVelocity;
            _horizontalVelocity = initialHorizontalVelocity;
            Initialize();
            this.Move();
        }

        public override Rectangle HitBox
        {
            get
            {
                return base.HitBox;
            }
            protected set
            {
                base.HitBox = value;
                if (value != null && _collidingNodes != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
                else if (_collidingNodes == null && _collisionGrid != null)
                {
                    _collidingNodes = _collisionGrid.GetCurrentHashes(this);
                }
            }
        }


        protected override CollisionObject GetLeftMostRightTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK || c is IBiomeTile).ToList();
            if (collisions.Any() && debugTiles.Any())
            {
                var rightTiles = debugTiles.Where(c => c.HitBox.Left > this.HitBox.Left && c.HitBox.Top < this.HitBox.Bottom && c.HitBox.Bottom > this.HitBox.Top).ToList();
                if (rightTiles.Any())
                {
                    int minX = rightTiles.Select(t => t.HitBox.Left).Min();
                    CollisionObject obj = rightTiles.FirstOrDefault(x => x.HitBox.Left == minX);
                    return obj;
                }
            }
            return null;
        }

        protected override CollisionObject GetRightMostLeftTile(List<CollisionObject> collisions)
        {
            var debugTiles = collisions.Where(c => c.CollisionType == CollisionType.BLOCK || c is IBiomeTile).ToList();
            if (collisions.Any() && debugTiles.Any())
            {
                var leftTiles = debugTiles.Where(c => c.HitBox.Left < this.HitBox.Left && c.HitBox.Top < this.HitBox.Bottom && c.HitBox.Bottom > this.HitBox.Top).ToList();
                if (leftTiles.Any())
                {
                    int maxX = leftTiles.Select(t => t.HitBox.Right).Max();
                    CollisionObject obj = leftTiles.FirstOrDefault(x => x.HitBox.Right == maxX);
                    return obj;
                }
            }
            return null;
        }

        private void Initialize()
        {
            SetSprite();

            var collisions = this.CheckCollision(this.HitBox);

            if (collisions.Any())
            {
                var horizontalCollision = _horizontalVelocity < 0 ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);
                var verticalCollision = _verticalVelocity < 0 ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);

                int verticalDist = int.MaxValue;
                int horizontalDist = int.MaxValue;

                if (horizontalCollision != null)
                {
                    horizontalDist = Math.Abs(this.HitBox.X - horizontalCollision.HitBox.X);
                }
                if (verticalCollision != null)
                {
                    verticalDist = Math.Abs(this.HitBox.Y - verticalCollision.HitBox.Y);
                }

                CollisionObject collision = verticalDist < horizontalDist ? verticalCollision : horizontalCollision;

                if (collision != null)
                {
                    this.HandleCollision(collision);
                }
            }
        }

        private void SetSprite()
        {
            switch (_biome)
            {
                case Biomes.BIOME_KEEN4_CAVE:
                    _sprite = Properties.Resources.keen4_cave_floor_middle;
                    break;
                case Biomes.BIOME_KEEN4_FOREST:
                    _sprite = Properties.Resources.keen4_forest_floor_middle;
                    break;
                case Biomes.BIOME_KEEN4_MIRAGE:
                    _sprite = Properties.Resources.keen4_mirage_floor_middle;
                    break;
                case Biomes.BIOME_KEEN4_PYRAMID:
                    _sprite = Properties.Resources.keen4_pyramid_floor_middle;
                    break;
                case Biomes.BIOME_KEEN5_BLACK:
                    _sprite = Properties.Resources.keen5_floor_black_middle;
                    break;
                case Biomes.BIOME_KEEN5_GREEN:
                    _sprite = Properties.Resources.keen5_floor_green_middle;
                    break;
                case Biomes.BIOME_KEEN5_RED:
                    _sprite = Properties.Resources.keen5_floor_red_middle;
                    break;
                case Biomes.BIOME_KEEN6_FOREST:
                    _sprite = Properties.Resources.keen6_forest_floor_middle;
                    break;
                case Biomes.BIOME_KEEN6_INDUSTRIAL:
                    _sprite = Properties.Resources.keen6_industrial_floor_middle;
                    break;
                case Biomes.BIOME_KEEN6_DOME:
                    _sprite = Properties.Resources.keen6_dome_floor_middle;
                    break;
            }
        }

        protected void HandleCollision(CollisionObject obj)
        {
            if (obj is IBiomeTile)
            {
                var biomeTile = (IBiomeTile)obj;
                biomeTile.ChangeBiome(_biome);
            }
            this.Stop();
        }

        public virtual int Damage
        {
            get { return -1; }
        }

        public virtual int Velocity
        {
            get { return Math.Abs(_horizontalVelocity) > Math.Abs(_verticalVelocity) ? _horizontalVelocity : _verticalVelocity; }
        }

        public virtual int Pierce
        {
            get { return -1; }
        }

        public virtual int Spread
        {
            get { return -1; }
        }

        public virtual int BlastRadius
        {
            get { return -1; }
        }

        public virtual int RefireDelay
        {
            get { return -1; }
        }

        public bool KillsKeen
        {
            get { return false; }
        }

        public void Move()
        {
            int speedX = Math.Abs(_horizontalVelocity);
            int speedY = Math.Abs(_verticalVelocity);
            int xPosCheck = _horizontalVelocity < 0 ? this.HitBox.X + _horizontalVelocity : this.HitBox.X;
            int yPosCheck = _verticalVelocity < 0 ? this.HitBox.Y + _verticalVelocity : this.HitBox.Y;

            Rectangle areaToCheck = new Rectangle(xPosCheck, yPosCheck, this.HitBox.Width + speedX, this.HitBox.Height + speedY);
            var collisions = this.CheckCollision(areaToCheck);

            var verticalTile = _verticalVelocity < 0 ? GetCeilingTile(collisions) : GetTopMostLandingTile(collisions);
            var horizontalTile = _horizontalVelocity < 0 ? GetRightMostLeftTile(collisions) : GetLeftMostRightTile(collisions);


            if (verticalTile != null && horizontalTile != null)
            {
                CollisionObject collisionTile = null;
                int xDistanceToCollisionTile = Math.Abs(horizontalTile.HitBox.X - this.HitBox.X), yDistanceToCollisionTile = Math.Abs(verticalTile.HitBox.Y - this.HitBox.Y);

                int xCollidePos = this.HitBox.X, yCollidePos = this.HitBox.Y;
                if (xDistanceToCollisionTile < yDistanceToCollisionTile)
                {
                    xCollidePos = _horizontalVelocity < 0 ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                    collisionTile = horizontalTile;
                }
                else
                {
                    yCollidePos = _verticalVelocity < 0 ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                    collisionTile = verticalTile;
                }
                this.HitBox = new Rectangle(xCollidePos, yCollidePos, this.HitBox.Width, this.HitBox.Height);
                this.HandleCollision(collisionTile);
            }
            else if (verticalTile != null)
            {
                int yCollidePos = _verticalVelocity < 0 ? verticalTile.HitBox.Bottom + 1 : verticalTile.HitBox.Top - this.HitBox.Height - 1;
                this.HitBox = new Rectangle(this.HitBox.X, yCollidePos, this.HitBox.Width, this.HitBox.Height);
                this.HandleCollision(verticalTile);
            }
            else if (horizontalTile != null)
            {
                int xCollidePos = _horizontalVelocity < 0 ? horizontalTile.HitBox.Right + 1 : horizontalTile.HitBox.Left - this.HitBox.Width - 1;
                this.HitBox = new Rectangle(xCollidePos, this.HitBox.Y, this.HitBox.Width, this.HitBox.Height);
                this.HandleCollision(horizontalTile);
            }
            else
            {
                this.HitBox = new Rectangle(this.HitBox.X + _horizontalVelocity, this.HitBox.Y + _verticalVelocity, this.HitBox.Width, this.HitBox.Height);
                DecelerateHorizontalVelocity();
                AccelerateGravity();
            }
        }

        protected override CollisionObject GetCeilingTile(List<CollisionObject> collisions)
        {
            var debugs = collisions.Where(c => c.CollisionType == CollisionType.BLOCK && c.HitBox.Bottom <= this.HitBox.Top).ToList();
            var biomes = collisions.Where(c => c is IBiomeTile && c.HitBox.Bottom <= this.HitBox.Top).ToList();
            if (!debugs.Any() && !biomes.Any())
                return null;

            int maxBottomDebugTile = debugs.Any() ? debugs.Select(c => c.HitBox.Bottom).Max() : 0;
            int maxBottomBiomeTile = biomes.Any() ? biomes.Select(c => c.HitBox.Bottom).Max() : 0;

            if (maxBottomBiomeTile >= maxBottomDebugTile)
            {
                CollisionObject obj = biomes.FirstOrDefault(c => c.HitBox.Bottom == maxBottomBiomeTile);
                return obj;
            }
            else
            {
                CollisionObject obj = debugs.FirstOrDefault(c => c.HitBox.Bottom == maxBottomDebugTile);
                return obj;
            }
        }

        protected override CollisionObject GetTopMostLandingTile(List<CollisionObject> collisions)
        {
            var debugs = collisions.Where(c => c.CollisionType == CollisionType.BLOCK && c.HitBox.Top >= this.HitBox.Top).ToList();
            var biomes = collisions.Where(c => c is IBiomeTile && c.HitBox.Top >= this.HitBox.Top).ToList();
            if (!debugs.Any() && !biomes.Any())
                return null;

            int minTopDebugTile = debugs.Any() ? debugs.Select(c => c.HitBox.Top).Min() : 0;
            int minTopBottomBiomeTile = biomes.Any() ? biomes.Select(c => c.HitBox.Top).Min() : 0;

            if (minTopBottomBiomeTile <= minTopDebugTile)
            {
                CollisionObject obj = biomes.FirstOrDefault(c => c.HitBox.Top == minTopBottomBiomeTile);
                return obj;
            }
            else
            {
                CollisionObject obj = debugs.FirstOrDefault(c => c.HitBox.Top == minTopDebugTile);
                return obj;
            }
        }

        private void AccelerateGravity()
        {
            if (_verticalVelocity < MAX_FALL_VELOCITY)
            {
                if (_verticalVelocity + GRAVITY_ACCELERATION < MAX_FALL_VELOCITY)
                {
                    _verticalVelocity += GRAVITY_ACCELERATION;
                }
                else
                {
                    _verticalVelocity = MAX_FALL_VELOCITY;
                }
            }
        }

        private void DecelerateHorizontalVelocity()
        {
            if (_horizontalVelocity != 0)
            {
                if (_horizontalVelocity < 0)
                {
                    if (_horizontalVelocity + AIR_RESISTANCE < 0)
                    {
                        _horizontalVelocity += AIR_RESISTANCE;
                    }
                    else
                    {
                        _horizontalVelocity = 0;
                    }
                }
                else if (_horizontalVelocity - AIR_RESISTANCE > 0)
                {
                    _horizontalVelocity -= AIR_RESISTANCE;
                }
                else
                {
                    _horizontalVelocity = 0;
                }
            }
        }

        public void Stop()
        {
            OnRemove(new ObjectEventArgs() { ObjectSprite = this });
            _stopped = true;
        }

        public Enums.MoveState MoveState
        {
            get;
            set;
        }

        public Enums.Direction Direction
        {
            get;
            set;
        }

        public void Update()
        {
            if (!_stopped)
                this.Move();
            else if (!_removed)
                OnRemove(new ObjectEventArgs() { ObjectSprite = this });
        }

        public override CollisionType CollisionType => CollisionType.FRIENDLY_PROJECTILE;

        public int ZIndex => 200;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        protected string _biome;
        private bool _stopped;

        protected void OnCreate(ObjectEventArgs e)
        {
            if (this.Create != null)
                this.Create(this, e);
        }

        protected void OnRemove(ObjectEventArgs e)
        {
            if (e.ObjectSprite == this)
            {
                foreach (var node in _collidingNodes)
                {
                    node.Objects.Remove(this);
                    node.NonEnemies.Remove(this);
                }
            }
            if (this.Remove != null)
            {
                this.Remove(this, e);
                _removed = true;
            }
        }
    }
}
