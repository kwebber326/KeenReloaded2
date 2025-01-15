using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Weapons;
using KeenReloaded2.Framework.GameEventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Items;
using System.Drawing;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.AltCharacters;
using KeenReloaded2.Entities.ReferenceData;
using KeenReloaded2.Framework.GameEntities;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.Factories;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Entities.DataStructures;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded.Framework.Utilities;
using KeenReloaded.Framework;
using KeenReloaded2.Framework.GameEntities.Tiles;

namespace KeenReloaded2.Entities
{
    public class CommanderKeenGame : IDisposable
    {
        private Dictionary<string, bool> _keysPressed;
        private CommanderKeen _keen;
        private OrderedList<ISprite> _gameObjects;
        private OrderedList<ISprite> _backgroundsAndTiles;
        private List<IUpdatable> _updatableGameObjects = new List<IUpdatable>();
        private List<AnimatedBackground> _animatedBackgrounds = new List<AnimatedBackground>();
        private Bitmap _backgroundImage;
        private bool _disposed;

        private Func<ISprite, ISprite, int> _compareFunction = (x1, x2) =>
        {
            if (x1.ZIndex == x2.ZIndex)
                return 0;
            if (x1.ZIndex > x2.ZIndex)
                return 1;

            return -1;
        };
        public CommanderKeenGame(MapMakerData map)
        {
            _keysPressed = new Dictionary<string, bool>();
            _keysPressed.Add("Left", false);
            _keysPressed.Add("Right", false);
            _keysPressed.Add("Up", false);
            _keysPressed.Add("Down", false);
            _keysPressed.Add("ControlKey", false);
            _keysPressed.Add("Space", false);
            _keysPressed.Add("D1", false);
            _keysPressed.Add("D2", false);
            _keysPressed.Add("D3", false);
            _keysPressed.Add("D4", false);
            _keysPressed.Add("D5", false);
            _keysPressed.Add("D6", false);
            _keysPressed.Add("Return", false);
            _keysPressed.Add("ShiftKey", false);
            _keysPressed.Add("Menu", false);
            if (map != null && map.MapData != null)
            {
                _keen = map.MapData.Select(d => d.GameObject).OfType<CommanderKeen>().FirstOrDefault();
                //populate non backgrounds
                Func<GameObjectMapping, bool> backgroundTypeFunc = (c) => (!c.GameObject?.CanUpdate ?? false) && !(c.GameObject is MapEdgeTile);
                var nonBackGrounds = map.MapData.Where(c => !backgroundTypeFunc(c) && !(c.GameObject is MapEdgeTile)).Select(d => d.GameObject).ToList();
                _gameObjects = OrderedList<ISprite>.FromEnumerable(nonBackGrounds, _compareFunction, true);
                //combine all backgrounds into one image
                FillBackGround(map, backgroundTypeFunc);

                //this will get all tiles in front of an animated background to be redrawn
                SetStaticTilesThatWillNeedToBeRedrawn();

                //get updatables
                var updatables = _gameObjects.OfType<IUpdatable>();
                if (updatables.Any())
                    _updatableGameObjects = updatables.ToList();

                foreach (var obj in _gameObjects)
                {
                    this.RegisterItemEventsForObject(obj);
                }
                //initialize any enemy spawners placed on the map
                var enemySpawners = _gameObjects.OfType<EnemySpawner>();
                if (enemySpawners.Any())
                {
                    var biomeTiles = _backgroundsAndTiles.OfType<IBiomeTile>()?.ToList()
                        ?? new List<IBiomeTile>();
                    foreach (var spawner in enemySpawners)
                    {
                        spawner.Initialize(biomeTiles);
                    }
                }
            }
            this.Map = map;
        }

        private void SetStaticTilesThatWillNeedToBeRedrawn()
        {
            _animatedBackgrounds = _gameObjects.OfType<AnimatedBackground>()?.ToList() ??
           new List<AnimatedBackground>();

            if (_animatedBackgrounds.Any())
            {
                List<ISprite> tilesToBeRedrawn = new List<ISprite>();
                var tiles = _backgroundsAndTiles.Where(c => !(c is Background)).ToList();
                foreach (var bg in _animatedBackgrounds)
                {
                    Rectangle collisionRectBG = new Rectangle(bg.Location, bg.Image.Size);
                    var collidingTiles = tiles.Where(t =>
                    {
                        Rectangle tileCollisionRect = new Rectangle(t.Location, t.Image.Size);
                        bool collides = tileCollisionRect.IntersectsWith(collisionRectBG);
                        return collides;
                    });
                    foreach (var tile in collidingTiles)
                    {
                        if (!_gameObjects.Contains(tile))
                        {
                            _gameObjects.InsertAscending(tile);
                        }
                    }
                }
            }
        }

        public Bitmap BackGroundImage
        {
            get
            {
                return _backgroundImage;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return _disposed;
            }
        }

        private void FillBackGround(MapMakerData map, Func<GameObjectMapping, bool> backgroundTypeFunc)
        {
            var backgrounds = map.MapData.Where(c => backgroundTypeFunc(c))
                .Select(c => c.GameObject)
                .OrderBy(c => c.ZIndex);
            //set relevant background collections
            _backgroundsAndTiles = OrderedList<ISprite>.FromEnumerable(backgrounds, _compareFunction, true);

            var backgroundImages = backgrounds.Select(i => i.Image).ToArray();
            var backgroundLocations = backgrounds.Select(i => i.Location).ToArray();
            _backgroundImage = BitMapTool.DrawImagesOnCanvas(map.MapSize, _backgroundImage, backgroundImages, backgroundLocations);
        }

        public MapMakerData Map
        {
            get;
            private set;
        }

        public void SetKeyPressed(string key, bool isPressed)
        {
            if (_keen != null)
                _keen.SetKeyPressed(key, isPressed);
        }

        public bool IsKeyPressed(string key)
        {
            if (_keen != null)
                return _keen.IsKeyPressed(key);

            return false;
        }

        public void ChangeKeenSkin(string characterName, out CommanderKeen keen)
        {
            //detach keen from events, updatable objects and collision detection
            _updatableGameObjects.Remove(_keen);
            _gameObjects.Remove(_keen);
            DetachEventsForObject(_keen);
            var nodes = _keen.CollisionGrid.GetCurrentHashes(_keen);
            foreach (var node in nodes)
            {
                node.Objects.Remove(_keen);
                node.NonEnemies.Remove(_keen);
            }
            //build new character and attach it to collision detection
            switch (characterName)
            {
                case MainMenuConstants.Characters.ORACLE_ELDER:
                    _keen = new OracleElder(_keen.CollisionGrid, _keen.HitBox, _keen.Direction, _keen.Lives, _keen.Points);
                    break;
                case MainMenuConstants.Characters.COMMANDER_KEEN:
                    _keen = new CommanderKeen(_keen.HitBox, _keen.CollisionGrid, _keen.Direction, _keen.Lives, _keen.Points);
                    break;
                case MainMenuConstants.Characters.LT_BARKER:
                    _keen = new BabyLouie(_keen.CollisionGrid, _keen.HitBox, _keen.Direction, _keen.Lives, _keen.Points);
                    break;
                case MainMenuConstants.Characters.COUNCIL_PAGE:
                    _keen = new CouncilPage(_keen.CollisionGrid, _keen.HitBox, _keen.Direction, _keen.Lives, _keen.Points);
                    break;
                case MainMenuConstants.Characters.BILLY_BLAZE:
                    _keen = new KChirps(_keen.CollisionGrid, _keen.HitBox, _keen.Direction, _keen.Lives, _keen.Points);
                    break;
                case MainMenuConstants.Characters.YORP:
                    _keen = new Locoyorp(_keen.CollisionGrid, _keen.HitBox, _keen.Direction, _keen.Lives, _keen.Points);
                    break;
                case MainMenuConstants.Characters.MORTIMER_MCMIRE:
                    _keen = new MortimerMcMire(_keen.CollisionGrid, _keen.HitBox, _keen.Direction, _keen.Lives, _keen.Points);
                    break;
                case MainMenuConstants.Characters.PRINCESS_LINDSEY:
                    _keen = new PrincessIndi(_keen.CollisionGrid, _keen.HitBox, _keen.Direction, _keen.Lives, _keen.Points);
                    break;
            }
            //assign output variable
            keen = _keen;
            //attach new keen object to events and updatable objects
            _gameObjects.InsertAscending(_keen);
            _updatableGameObjects.Add(_keen);
            RegisterItemEventsForObject(_keen);
        }

        public Bitmap UpdateGame()
        {
            if (this.Map != null && _updatableGameObjects != null)
            {
                int length = _updatableGameObjects.Count;
                for (int i = 0; i < length; i++)
                {
                    try
                    {
                        if (i < _updatableGameObjects.Count)
                        {
                            var obj = _updatableGameObjects[i];
                            obj.Update();
                        }
                    }

                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                return DrawMapImage(_gameObjects);
            }
            return null;
        }

        public void RegisterItemEventsForObject(object obj)
        {
            if (obj is Item)
            {
                var item = obj as Item;
                item.Acquired += Item_Acquired;
            }
            if (obj is NeuralStunner)
            {
                var item = obj as NeuralStunner;
                item.CreatedObject += new EventHandler<ObjectEventArgs>(item_CreatedObject);
                item.RemovedObject += new EventHandler<ObjectEventArgs>(item_RemovedObject);
            }
            else if (obj is ICreateRemove)
            {
                var item = obj as ICreateRemove;
                item.Create += new EventHandler<ObjectEventArgs>(item_CreatedObject);
                item.Remove += new EventHandler<ObjectEventArgs>(item_RemovedObject);
                if (obj is IFlag)
                {
                    var flag = (IFlag)obj;
                    flag.FlagCaptured += Flag_FlagCaptured;
                }
            }

            RegisterZombieEnemy(obj);
        }

        private void Item_Acquired(object sender, EventArgs e)
        {
            Item item = sender as Item;

            if (item == null)
                return;

            int itemIndex = _gameObjects.IndexOf(item);
            if (itemIndex == -1)
                return;

            _gameObjects.Remove(item);
            _gameObjects.InsertAscending(item);
        }

        private void ZombieEnemy_Killed(object sender, ObjectEventArgs e)
        {
            var removedObject = e.ObjectSprite;
            if (this.Map?.GameMode == MainMenuConstants.OPTION_LABEL_ZOMBIE_MODE)
            {
                var zombieBountyEnemy = (IZombieBountyEnemy)removedObject;
                PointItem item = PointItemFactory.GeneratePointItemFromType(zombieBountyEnemy);
                if (item != null)
                {
                    RegisterItemEventsForObject(item);
                    item_CreatedObject(this, new ObjectEventArgs() { ObjectSprite = item });
                }
            }
        }

        public void DetachEventsForObject(object obj)
        {
            if (obj is NeuralStunner)
            {
                var item = obj as NeuralStunner;
                item.CreatedObject -= item_CreatedObject;
                item.RemovedObject -= item_RemovedObject;
            }
            else if (obj is ICreateRemove)
            {
                var item = obj as ICreateRemove;
                item.Create -= item_CreatedObject;
                item.Remove -= item_RemovedObject;
                if (obj is IFlag)
                {
                    var flag = (IFlag)obj;
                    flag.FlagCaptured -= Flag_FlagCaptured;
                }
            }
            if (obj is Item)
            {
                var item = obj as Item;
                item.Acquired -= Item_Acquired;
            }
            UnRegisterZombieEnemy(obj);
        }

        private void UnRegisterZombieEnemy(object obj)
        {
            if (obj is IZombieBountyEnemy)
            {
                var zombieEnemy = (IZombieBountyEnemy)obj;
                zombieEnemy.Killed -= ZombieEnemy_Killed;
            }
        }

        private void RegisterZombieEnemy(object obj)
        {
            if (obj is IZombieBountyEnemy)
            {
                var zombieEnemy = (IZombieBountyEnemy)obj;
                zombieEnemy.Killed += ZombieEnemy_Killed;
            }
        }

        public Bitmap DrawMapImage<T>(IEnumerable<T> collection)  where T : ISprite
        {
            if (collection == null)
                return null;

            int mapWidth = this.Map.MapSize.Width;
            int mapHeight = this.Map.MapSize.Height;

            Bitmap mapCanvas = new Bitmap(mapWidth, mapHeight);
            try
            {
                using (Graphics g = Graphics.FromImage(mapCanvas))
                {
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                    foreach (var item in collection)
                    {
                        if (item?.Image == null)
                            continue;

                        var bitmap = ((Bitmap)item.Image);
                        g.DrawImage(bitmap, new Rectangle(item.Location, bitmap.Size));
                    }
                }
                return mapCanvas;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}\n{ex}");
                mapCanvas.Dispose();
            }
            finally
            {
                GC.Collect();
            }
            return null;
        }

        private void Flag_FlagCaptured(object sender, FlagCapturedEventArgs e)
        {
            var flag = e.Flag;
            if (flag != null)
            {
                var newFlag = flag.Copy();

                this.RegisterItemEventsForObject(newFlag);

                item_CreatedObject(this, new ObjectEventArgs() { ObjectSprite = newFlag });
            }
        }

        void item_RemovedObject(object sender, ObjectEventArgs e)
        {
            ISprite removedObject = e.ObjectSprite as ISprite;
            if (removedObject != null)
            {
                _gameObjects.Remove(removedObject);

                IUpdatable updatable = e.ObjectSprite as IUpdatable;
                if (updatable != null)
                {
                    _updatableGameObjects.Remove(updatable);
                }
            }
        }

        void item_CreatedObject(object sender, ObjectEventArgs e)
        {
            ISprite addedObject = e.ObjectSprite as ISprite;
            if (addedObject != null)
            {
                _gameObjects.InsertAscending(addedObject);
                IUpdatable obj = e.ObjectSprite as IUpdatable;
                if (obj != null)
                {
                    _updatableGameObjects.Add(obj);
                    RegisterZombieEnemy(obj);
                }
            }


            //if (obj != null)
            //{
            //    bool isHill = obj is Hill;
            //    if (!isHill || !this.Map.Objects.Contains(obj))
            //        this.Map.Objects.Add(obj);
            //}
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var obj in _gameObjects)
                {
                    this.DetachEventsForObject(obj);
                }
                if (_backgroundImage != null)
                {
                    _backgroundImage = null;
                }
            }
            _disposed = true;
        }
    }
}
