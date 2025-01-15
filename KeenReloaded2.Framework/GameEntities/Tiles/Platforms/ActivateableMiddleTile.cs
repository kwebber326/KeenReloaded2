using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Platforms
{
    public class ActivateableMiddleTile : MiddleFloorTile, IActivateable
    {
        private bool _isActive;
        private readonly Guid _activationId;

        public ActivateableMiddleTile(Rectangle area, SpaceHashGrid grid, Rectangle hitbox, string imageFile, int zIndex, string biome, bool isActive, Guid activationId)
            : base(area, grid, hitbox, imageFile, zIndex, biome)
        {

            _activationId = activationId;
            this.IsActive = isActive;
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (!value)
                {
                    this.RemoveSelfFromCollisionGrid();
                }
                else if (!_isActive && value)
                {
                    this.AddSelfToCollisionGrid();
                }
                _isActive = value;
                this.SetImageFromBiome();
            }
        }

        public Guid ActivationID => _activationId;

        public void Activate()
        {
            if (!_isActive)
            {
                this.AddSelfToCollisionGrid();
            }
            this.IsActive = true;
        }

        public void Deactivate()
        {

            this.IsActive = false;
        }

        protected override void SetImageFromBiome()
        {
            _image = _isActive 
                ? Properties.Resources.keen4_removable_platform 
                : Properties.Resources.keen4_removable_platform_inactive;
        }

        protected virtual void RemoveSelfFromCollisionGrid()
        {
            if (_collisionGrid != null && _collidingNodes != null && _collidingNodes.Any())
            {
                foreach (var node in _collidingNodes)
                {
                    node.Objects.Remove(this);
                    node.Tiles.Remove(this);
                    node.NonEnemies.Remove(this);
                }
            }
        }

        protected virtual void AddSelfToCollisionGrid()
        {
            if (_collisionGrid != null && _collidingNodes != null && _collidingNodes.Any())
            {
                this.UpdateCollisionNodes(Enums.Direction.DOWN_LEFT);
                this.UpdateCollisionNodes(Enums.Direction.UP_RIGHT);
            }
        }

        public override bool CanUpdate => true;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;

            return $"{_initialImageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_biome}{separator}{IsActive}{separator}{_activationId}";
        }
    }
}
