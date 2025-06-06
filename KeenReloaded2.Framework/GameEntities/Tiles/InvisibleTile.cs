﻿using KeenReloaded.Framework;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles
{
    public class InvisibleTile : CollisionObject, IHangableTile
    {
        private readonly bool _hangable;

        public InvisibleTile(SpaceHashGrid grid, Rectangle hitbox, bool hangable = false) : base(grid, hitbox)
        {
            this.HitBox = hitbox;
            _hangable = hangable;
        }

        public override CollisionType CollisionType => CollisionType.BLOCK;

        public bool Hangable => _hangable;

        public void AddTileToGrid()
        {
            if ((_collidingNodes == null || !_collidingNodes.Any()) && _collisionGrid != null)
            {
                _collidingNodes = _collisionGrid.GetCurrentHashes(this);
            }
            foreach (var node in _collidingNodes)
            {
                node.Tiles.Add(this);
                node.NonEnemies.Add(this);
                node.Objects.Add(this);
            }
        }

        public void RemoveTileFromGrid()
        {
            if (_collidingNodes == null)
                return;

            foreach (var node in _collidingNodes)
            {
                node.Tiles.Remove(this);
                node.NonEnemies.Remove(this);
                node.Objects.Remove(this);
            }
        }
    }
}
