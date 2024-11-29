using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Tiles;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Constructs
{
    public class EnemySpawner : DestructibleObject, ICreateRemove, ISprite, IEnemy, IUpdatable
    {
        private readonly int _zIndex;
        private List<string> _enemyTypeList;
        private Image _sprite;
        private SpawnerState _state;
        private int _electricState = 1;
        private Image[] _sprites;
        private const int SPRITE_CHANGE_DELAY = 1;
        private int _currentSpriteChangeDelayTick;

        private const int ANIMATION_TIME = 2;
        private int _animationTimeTick;

        private readonly int _maxConcurrentSpawn;
        private List<IEnemy> _enemies = new List<IEnemy>();

        private readonly int _spawnDelay;
        private int _spawnDelayTick;

        private const int SPAWN_TIME = 8;
        private int _currentSpawnTimeTick;

        private const int MAX_NEAREST_TILES = 10;
        private List<IBiomeTile> _nearestTiles;

        private Queue<IEnemy> _deadEnemies = new Queue<IEnemy>();

        public EnemySpawner(Rectangle area, SpaceHashGrid grid, int zIndex, string[] enemyTypeList, int maxConcurrentSpawn, int spawnDelayTicks, int health)
            : base(grid, area)
        {
            _zIndex = zIndex;
            this.HitBox = area;
            _enemyTypeList = enemyTypeList.ToList();
            _maxConcurrentSpawn = maxConcurrentSpawn;
            _spawnDelay = spawnDelayTicks;
            _enemyMapping = ReferenceDataClasses.Enemies.EnemyToEpisodeMapping;
            _biomeMapping = Biomes.BiomeToEpisodeMapping;
            _sprite = Properties.Resources.enemy_spawner1;

            this.Health = health;
        }


        public void Initialize(List<IBiomeTile> tiles)
        {
            _sprites = SpriteSheet.SpriteSheet.EnemySpawnerImages;
            this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
            this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
            _nearestTiles = GetNearestTiles(tiles);
            this.State = SpawnerState.WAITING;
        }

        private List<IBiomeTile> GetNearestTiles(List<IBiomeTile> tiles)
        {
            HashSet<IBiomeTile> totalTiles = new HashSet<IBiomeTile>();

            if (tiles != null && tiles.Any())
            {
                var tilesAsCollisionObjects = tiles.OfType<CollisionObject>()?.ToList() 
                    ?? new List<CollisionObject>();
                var orderedTiles = tilesAsCollisionObjects.OrderBy(o => Math.Abs(o.HitBox.X - this.HitBox.X) < Math.Abs(o.HitBox.Y - this.HitBox.Y)
                    ? Math.Abs(o.HitBox.X - this.HitBox.X)
                    : Math.Abs(o.HitBox.Y - this.HitBox.Y)).OfType<IBiomeTile>();


                return orderedTiles.Any()
                    ? orderedTiles.Take(MAX_NEAREST_TILES).ToList()
                    : new List<IBiomeTile>();
            }
            return new List<IBiomeTile>();
        }

        public void HandleHit(IProjectile projectile)
        {
            this.TakeDamage(projectile);
            if (this.Health > 0)
            {
                _hitAnimation = true;
                UpdateSprite();
            }
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (!this.IsDead())
            {
                _hitAnimation = true;
                UpdateSprite();
            }
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
                if (_collisionGrid != null && _collidingNodes != null && value != null)
                {
                    this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                    this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
                }
            }
        }

        SpawnerState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {

            switch (_state)
            {
                case SpawnerState.WAITING:
                    _sprite = Properties.Resources.enemy_spawner1;
                    break;
                case SpawnerState.SPAWNING:
                case SpawnerState.DISABLED:
                    _sprite = _sprites[_electricState];
                    break;
            }

            if (_hitAnimation)
                _sprite = this.GetCurrentSpriteWithWhiteBackground(_sprite);
        }

        public event EventHandler<ObjectEventArgs> Create;

        public event EventHandler<ObjectEventArgs> Remove;
        private bool _hitAnimation;
        private Dictionary<string, string> _enemyMapping;
        private Dictionary<string, string> _biomeMapping;

        protected void OnCreate(ObjectEventArgs args)
        {
            if (Create != null)
            {
                Create(this, args);
            }
        }

        protected void OnRemove(ObjectEventArgs args)
        {
            if (this.Remove != null)
            {
                this.Remove(this, args);
                if (args.ObjectSprite is IEnemy)
                {
                    var enemy = args.ObjectSprite as IEnemy;
                    _enemies.Remove(enemy);
                    RemoveBounderHeadTiles(enemy);
                }
            }
        }

        public override void Die()
        {
            this.State = SpawnerState.DISABLED;
            ClearMapOfDeadEnemies();
        }

        public bool DeadlyTouch
        {
            get { return false; }
        }

        public bool IsActive
        {
            get { return _state != SpawnerState.DISABLED; }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => this.HitBox.Location;

        public void Update()
        {
            if (_hitAnimation && _animationTimeTick++ == ANIMATION_TIME)
            {
                _animationTimeTick = 0;
                _hitAnimation = false;
            }
            switch (_state)
            {
                case SpawnerState.WAITING:
                    this.Wait();
                    break;
                case SpawnerState.SPAWNING:
                    this.SpawnEnemy();
                    break;
                case SpawnerState.DISABLED:
                    this.UpdateDisabledState();
                    break;
            }
        }

        private void UpdateDisabledState()
        {
            if (this.State != SpawnerState.DISABLED)
            {
                this.State = SpawnerState.DISABLED;
                _currentSpriteChangeDelayTick = 0;
            }

            UpdateElectricState();
        }

        private void UpdateElectricState()
        {
            if (_currentSpriteChangeDelayTick++ == SPRITE_CHANGE_DELAY)
            {
                _currentSpriteChangeDelayTick = 0;
                if (_electricState == 1)
                {
                    this.UpdateSprite();
                    _electricState++;
                }
                else if (_electricState == 2)
                {
                    this.UpdateSprite();
                    _electricState = 1;
                }
                else
                {
                    _electricState = 1;
                }
            }
        }

        private void SpawnEnemy()
        {
            if (this.State != SpawnerState.SPAWNING)
            {
                this.State = SpawnerState.SPAWNING;
                _currentSpriteChangeDelayTick = 0;
                _currentSpawnTimeTick = 0;
            }

            if (_deadEnemies.Any() && _enemies.Count > _maxConcurrentSpawn)
            {
                RemoveEarliestDeadEnemy();
            }

            if (_enemies.Count < _maxConcurrentSpawn || (_deadEnemies.Any() && _enemies.Count == _maxConcurrentSpawn))
            {
                UpdateElectricState();

                if (++_currentSpawnTimeTick == SPAWN_TIME)
                {
                    this.Wait();
                }
                else if (_currentSpawnTimeTick == SPAWN_TIME / 2)
                {
                    if ((_deadEnemies.Any() && _enemies.Count >= _maxConcurrentSpawn))
                    {
                        RemoveEarliestDeadEnemy();
                    }
                    if (_enemies.Count < _maxConcurrentSpawn)
                        this.CreateEnemySpawn();
                }
            }
            else
            {
                this.Wait();
            }
        }

        private void RemoveEarliestDeadEnemy()
        {
            var enemyToRemove = _deadEnemies.Dequeue();

            if (enemyToRemove != null)
            {
                _enemies.Remove(enemyToRemove);
                var spriteObj = enemyToRemove as ISprite;
                if (spriteObj != null)
                {
                    if (enemyToRemove is CollisionObject)
                    {
                        var obj = (CollisionObject)enemyToRemove;
                        obj.DetachFromCollisionGrid();
                    }
                    OnRemove(new ObjectEventArgs() { ObjectSprite = spriteObj });
                }
            }
        }

        private void ClearMapOfDeadEnemies()
        {
            while (_deadEnemies.Any())
            {
                RemoveEarliestDeadEnemy();
            }
        }

        private void CreateEnemySpawn()
        {
            _random = new Random();
            int randomBiomePick = _random.Next(0, _nearestTiles.Count);

            IBiomeTile pickedTile = _nearestTiles[randomBiomePick];

            if (pickedTile != null)
            {
                string episodeMapping = _biomeMapping[pickedTile.Biome];
                var episodeEnemies = _enemyTypeList.Where(e => _enemyMapping[e] == episodeMapping).ToList();
                if (!episodeEnemies.Any())
                {
                    episodeEnemies = _enemyTypeList;
                }
                _random = new Random();
                int randomEnemyPick = _random.Next(0, episodeEnemies.Count);
                string pickedEnemy = episodeEnemies[randomEnemyPick];
                if (pickedEnemy != null)
                {
                    ISprite spriteObj = GenerateEnemySpawn(pickedEnemy);
                    var enemy = spriteObj as IEnemy;
                    if (enemy != null)
                    {
                        _enemies.Add(enemy);
                    }
                    OnCreate(new ObjectEventArgs() { ObjectSprite = spriteObj });
                }
            }
        }

        private ISprite GenerateEnemySpawn(string enemyType)
        {
            if (enemyType == typeof(Lick).Name)
            {
                Lick lick = new Lick(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 18), this.HitBox.Y + (this.HitBox.Height / 2), 36, 32), _collisionGrid, _zIndex + 1);
                lick.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return lick;
            }
            else if (enemyType == typeof(Shockshund).Name)
            {
                Shockshund shockshund = new Shockshund(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 23), this.HitBox.Y + (this.HitBox.Height / 2), 46, 32), _collisionGrid, _zIndex);
                shockshund.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                shockshund.Create += new EventHandler<ObjectEventArgs>(object_Create);
                shockshund.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                return shockshund;
            }
            else if (enemyType == typeof(PoisonSlug).Name)
            {
                PoisonSlug slug = new PoisonSlug(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 23), this.HitBox.Y + (this.HitBox.Height / 2), 46, 44), _collisionGrid, _zIndex);
                slug.Create += new EventHandler<ObjectEventArgs>(object_Create);
                slug.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                slug.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return slug;
            }
            else if (enemyType == typeof(Wormmouth).Name)
            {
                Wormmouth wormmouth = new Wormmouth(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 23), this.HitBox.Y + (this.HitBox.Height / 2), 46, 44), _collisionGrid, _zIndex);
                wormmouth.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return wormmouth;
            }
            else if (enemyType == typeof(Sparky).Name)
            {
                Sparky sparky = new Sparky(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 29), this.HitBox.Bottom - 60, 58, 60), _collisionGrid, _zIndex);
                sparky.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return sparky;
            }
            else if (enemyType == typeof(DiagonalSlicestar).Name)
            {
                DiagonalSlicestar slicestar = new DiagonalSlicestar(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 29), this.HitBox.Y + (this.HitBox.Height / 2) - 20, 58, 60), _collisionGrid, _zIndex);
                slicestar.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                slicestar.Create += new EventHandler<ObjectEventArgs>(object_Create);
                slicestar.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                return slicestar;
            }
            else if (enemyType == typeof(SkyPest).Name)
            {
                SkyPest pest = new SkyPest(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 23), this.HitBox.Y + (this.HitBox.Height / 2), 34, 14), _collisionGrid, _zIndex);
                pest.Squashed += new EventHandler<ObjectEventArgs>(enemy_Squashed);
                pest.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return pest;
            }
            else if (enemyType == typeof(Mimrock).Name)
            {
                Mimrock mimrock = new Mimrock(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 23), this.HitBox.Y + (this.HitBox.Height / 2), 46, 40), _collisionGrid, _zIndex);
                mimrock.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return mimrock;
            }
            else if (enemyType == typeof(Arachnut).Name)
            {
                Arachnut arachnut = new Arachnut(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 38), this.HitBox.Bottom - 80, 76, 80), _collisionGrid, _zIndex);
                return arachnut;
            }
            else if (enemyType == typeof(LittleAmpton).Name)
            {
                LittleAmpton littleAmpton = new LittleAmpton(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 21), this.HitBox.Bottom - 48, 42, 48), _collisionGrid, _zIndex);
                littleAmpton.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return littleAmpton;
            }
            else if (enemyType == typeof(Bounder).Name)
            {
                Bounder bounder = new Bounder(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 23), this.HitBox.Bottom - 46, 46, 46), _collisionGrid, _zIndex);
                bounder.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return bounder;
            }
            else if (enemyType == typeof(ShikadiMine).Name)
            {
                ShikadiMine shikadiMine = new ShikadiMine(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 26), this.HitBox.Bottom - 52, 68, 52), _collisionGrid, _zIndex);
                shikadiMine.Create += new EventHandler<ObjectEventArgs>(object_Create);
                shikadiMine.Remove += new EventHandler<ObjectEventArgs>(object_Remove);

                return shikadiMine;
            }
            else if (enemyType == typeof(Shelley).Name)
            {
                Shelley shelley = new Shelley(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 11), this.HitBox.Bottom - 30, 24, 30), _collisionGrid, _zIndex);
                shelley.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                shelley.Create += new EventHandler<ObjectEventArgs>(object_Create);
                shelley.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                return shelley;
            }
            else if (enemyType == typeof(NoSpike).Name)
            {
                NoSpike nospike = new NoSpike(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 36), this.HitBox.Bottom - 62, 72, 62), _collisionGrid, _zIndex);
                nospike.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                nospike.Create += new EventHandler<ObjectEventArgs>(object_Create);
                nospike.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                return nospike;
            }
            else if (enemyType == typeof(Shikadi).Name)
            {
                Shikadi shikadi = new Shikadi(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 24), this.HitBox.Bottom - 62, 48, 62), _collisionGrid, _zIndex);
                shikadi.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                shikadi.Create += new EventHandler<ObjectEventArgs>(object_Create);
                shikadi.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                return shikadi;
            }
            else if (enemyType == typeof(Fleex).Name)
            {
                Fleex fleex = new Fleex(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 47), this.HitBox.Bottom - 106, 94, 106), _collisionGrid, _zIndex);
                fleex.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return fleex;
            }
            else if (enemyType == typeof(Flect).Name)
            {
                Flect flect = new Flect(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 21), this.HitBox.Bottom - 62, 42, 62), _collisionGrid, _zIndex);
                flect.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return flect;
            }
            else if (enemyType == typeof(Blooglet).Name)
            {
                Color color = GetRandomBloogletColor();
                Blooglet blooglet = new Blooglet(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 21), this.HitBox.Bottom - 62, 42, 44), _collisionGrid,  _zIndex, color, false);
                blooglet.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                blooglet.Create += new EventHandler<ObjectEventArgs>(object_Create);
                blooglet.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                return blooglet;
            }
            else if (enemyType == typeof(BipShip).Name)
            {
                BipShip bipship = new BipShip(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 31), this.HitBox.Bottom - 62, 62, 32), _collisionGrid, _zIndex);
                bipship.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                bipship.Create += new EventHandler<ObjectEventArgs>(object_Create);
                bipship.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                return bipship;
            }
            else if (enemyType == typeof(Bip).Name)
            {
                Bip bip = new Bip(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 6), this.HitBox.Bottom - 16, 12, 16), _collisionGrid, _zIndex);
                bip.Create += new EventHandler<ObjectEventArgs>(object_Create);
                bip.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                bip.Squashed += new EventHandler<ObjectEventArgs>(enemy_Squashed);
                return bip;
            }
            else if (enemyType == typeof(GnosticeneAncient).Name)
            {
                GnosticeneAncient ancient = new GnosticeneAncient(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 45), this.HitBox.Bottom - 72, 90, 72), _collisionGrid, _zIndex);
                ancient.Create += new EventHandler<ObjectEventArgs>(object_Create);
                ancient.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                ancient.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return ancient;
            }
            else if (enemyType == typeof(Bloog).Name)
            {
                Bloog bloog = new Bloog(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 40), this.HitBox.Bottom - 84, 80, 84), _collisionGrid, _zIndex);
                bloog.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return bloog;
            }
            else if (enemyType == typeof(Blooguard).Name)
            {
                Blooguard blooguard = new Blooguard(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 46), this.HitBox.Bottom - 100, 92, 100), _collisionGrid, _zIndex);
                blooguard.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return blooguard;
            }
            else if (enemyType == typeof(Babobba).Name)
            {
                Babobba babobba = new Babobba(new Rectangle(this.HitBox.X + (this.HitBox.Width / 2 - 18), this.HitBox.Bottom - 44, 36, 44), _collisionGrid, _zIndex);
                babobba.Create += new EventHandler<ObjectEventArgs>(object_Create);
                babobba.Remove += new EventHandler<ObjectEventArgs>(object_Remove);
                babobba.Killed += new EventHandler<ObjectEventArgs>(enemy_Killed);
                return babobba;
            }
            return null;
        }


        private Color GetRandomBloogletColor()
        {
            int randColor = _random.Next(0, 4);
            Color color = Color.Red;
            switch (randColor)
            {
                case 0:
                    color = Color.Red;
                    break;
                case 1:
                    color = Color.Blue;
                    break;
                case 2:
                    color = Color.Green;
                    break;
                case 3:
                    color = Color.Yellow;
                    break;

            }
            return color;
        }

        void enemy_Squashed(object sender, ObjectEventArgs e)
        {
            AddObjectToDeadQueue(e);
        }

        void object_Create(object sender, ObjectEventArgs e)
        {
            OnCreate(e);
        }

        void object_Remove(object sender, ObjectEventArgs e)
        {
            OnRemove(e);
        }

        void enemy_Killed(object sender, ObjectEventArgs e)
        {
            if (!(e.ObjectSprite is IExplodable || e.ObjectSprite is Slicestar || e.ObjectSprite is SkyPest))
                AddObjectToDeadQueue(e);
            if (_enemies.Count(e1 => e1.IsActive) == _maxConcurrentSpawn - 1)
            {
                _spawnDelayTick = 0;
            }
        }

        private void AddObjectToDeadQueue(ObjectEventArgs e)
        {
            var enemy = e.ObjectSprite as IEnemy;
            if (enemy != null)
            {
                if (_deadEnemies.Count < _maxConcurrentSpawn)
                {
                    _deadEnemies.Enqueue(enemy);
                }
                else
                {
                    _enemies.Remove(enemy);
                    OnRemove(e);
                    RemoveBounderHeadTiles(enemy);
                }
            }
        }

        private void RemoveBounderHeadTiles(IEnemy enemy)
        {
            if (enemy is Bounder)
            {
                var headTile = ((Bounder)enemy).HeadTile;
                WipeObjectFromMap(headTile);
            }
        }

        private void WipeObjectFromMap(Tiles.Platforms.MovingPlatformTile headTile)
        {
            var collidingNodes = _collisionGrid.GetCurrentHashes(headTile);
            foreach (var node in collidingNodes)
            {
                node.Objects.Remove(headTile);
                node.Tiles.Remove(headTile);
                node.NonEnemies.Remove(headTile);
                OnRemove(new ObjectEventArgs() { ObjectSprite = headTile });
            }
        }

        private void Wait()
        {
            if (this.State != SpawnerState.WAITING)
            {
                this.State = SpawnerState.WAITING;
                _spawnDelayTick = 0;
            }
            if (_spawnDelayTick++ >= _spawnDelay)
            {
                this.SpawnEnemy();
            }
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = nameof(Properties.Resources.enemy_spawner1);
            string pathArray = MapMakerConstants.MAP_MAKER_ARRAY_START;
            for (int i = 0; i < _enemyTypeList.Count; i++)
            {
                var enemyType = _enemyTypeList[i];
                string item = i == _enemyTypeList.Count - 1 
                    ? enemyType 
                    : enemyType + MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;
                pathArray += item;
            }
            pathArray += MapMakerConstants.MAP_MAKER_ARRAY_END;

            return $"{imageName}{separator}{this.HitBox.X}{separator}{this.HitBox.Y}{separator}{this.HitBox.Width}{separator}{this.HitBox.Height}{separator}{_zIndex}{separator}{pathArray}{separator}{_maxConcurrentSpawn}{separator}{_spawnDelay}{separator}{this.Health}";
        }
    }

    enum SpawnerState
    {
        WAITING,
        SPAWNING,
        DISABLED
    }
}
