﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.GameEntities.Hazards;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Framework.ReferenceDataClasses;
using KeenReloaded2.Utilities;

namespace KeenReloaded2.Framework.GameEntities.Tiles.Floors
{
    public class Keen6SLimeHazardMiddleFloor : MiddleFloorTile, IUpdatable
    {
        private const int SPRITE_CHANGE_DELAY = 1;
        private const int HAZARD_COLLISION_HEIGHT = 4;
        private const int CEILING_COLLISION_HEIGHT = 16;

        private int _currnetSpriteChangeDelayTick;
        private int _currentSpriteIndex;
        private Image[] _sprites;
        private InvisibleHazard _deathCollisionRange;

        public Keen6SLimeHazardMiddleFloor(Rectangle area, SpaceHashGrid grid, int zIndex, int currentSpriteIndex)
            : base(area, grid, area, null, zIndex, Biomes.BIOME_KEEN6_INDUSTRIAL)
        {
            _currentSpriteIndex = currentSpriteIndex;
            this.Initialize();
        }

        private void Initialize()
        {
            _sprites = SpriteSheet.SpriteSheet.Keen6SlimeHazardMiddleImages;
            this.UpdateSprite();
            if (_collisionGrid != null && _collidingNodes != null)
            {
                _deathCollisionRange = new InvisibleHazard(_collisionGrid, new Rectangle(
                    _area.X,
                    _area.Y + _downwardCollisionOffset - 2,
                    _area.Width, HAZARD_COLLISION_HEIGHT));
            }
        }

        public override bool CanUpdate => true;

        public void Update()
        {
            this.UpdateSpriteByDelayBase(
                ref _currnetSpriteChangeDelayTick,
                ref _currentSpriteIndex,
                SPRITE_CHANGE_DELAY, UpdateSprite);
        }

        private void UpdateSprite()
        {
            if (_currentSpriteIndex >= _sprites.Length)
                _currentSpriteIndex = 0;
            _image = _sprites[_currentSpriteIndex];
            if (_image != null)
                _image = CommonGameFunctions.DrawImage(_area, _image);
        }
        private string GetImageNameFromCurrentSpriteIndex()
        {
            string imageName;
            switch (_currentSpriteIndex)
            {
                case 1:
                    imageName = nameof(Properties.Resources.keen6_slime_hazard_middle1);
                    break;
                case 2:
                    imageName = nameof(Properties.Resources.keen6_slime_hazard_middle2);
                    break;
                case 3:
                    imageName = nameof(Properties.Resources.keen6_slime_hazard_middle3);
                    break;
                default:
                    imageName = nameof(Properties.Resources.keen6_slime_hazard_middle1);
                    break;
            }

            return imageName;
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            string imageName = GetImageNameFromCurrentSpriteIndex();

            return $"{imageName}{separator}{_area.X}{separator}{_area.Y}{separator}{_area.Width}{separator}{_area.Height}{separator}{_zIndex}{separator}{_currentSpriteIndex}";
        }
    }
}
