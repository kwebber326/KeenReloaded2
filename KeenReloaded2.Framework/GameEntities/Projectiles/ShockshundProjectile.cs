using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Constructs;
using KeenReloaded2.Framework.GameEntities.Enemies;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

namespace KeenReloaded2.Framework.GameEntities.Projectiles
{
    public class ShockshundProjectile : StraightShotProjectile, ICancellableProjectile
    {
        public ShockshundProjectile(SpaceHashGrid grid, Rectangle hitbox, Direction direction)
            : base(grid, hitbox, direction, EnemyProjectileType.KEEN5_SHOCKSHUND_SHOT)
        {

        }

        public override void Update()
        {
            if (!_shotComplete)
            {
                GetSpreadOffset();
                var areaToCheck = GetAreaToCheckForCollision();
                var collisionObjects = this.CheckCollision(areaToCheck);
                var debugTiles = collisionObjects.Where(c => c.CollisionType == CollisionType.BLOCK);
                var keens = collisionObjects.OfType<CommanderKeen>().ToList();
                var itemsToCheck = new List<CollisionObject>();
                var explodables = collisionObjects.OfType<Shelley>().ToList();
                var keenStunShots = collisionObjects.OfType<KeenStunShot>().ToList();
                itemsToCheck.AddRange(debugTiles);
                foreach (var keen in keens)
                {
                    if (keen != null)
                    {
                        itemsToCheck.Add(keen);
                    }
                }
                itemsToCheck.AddRange(explodables);
                itemsToCheck.AddRange(keenStunShots);
                if (itemsToCheck.Any())
                {
                    HandleCollisionByDirection(itemsToCheck);
                }
                else
                {
                    this.Move();
                }
            }
            else if (_shotCompleteSprites != null && _shotCompleteSprites.Any())
            {
                UpdateSprite();
            }
            else
            {
                EndShot();
            }
        }

        private void EndShot()
        {
            this.UpdateCollisionNodes(this.Direction);
            CleanUpCollisionNodes();
            OnObjectComplete();
        }

        public void Cancel()
        {
            _shotComplete = true;
            UpdateSprite();
        }
    }
}
