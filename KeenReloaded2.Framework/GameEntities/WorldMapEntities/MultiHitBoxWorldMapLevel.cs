using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class MultiHitBoxWorldMapLevel : WorldMapLevel
    {
        private readonly Rectangle[] _hitBoxes;

        public MultiHitBoxWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, Rectangle[] hitboxes) 
            : base(area, grid, zIndex, sprite, levelName, levelEntryText)
        {
            _hitBoxes = hitboxes;
            if (_collisionGrid != null && _collidingNodes != null)
            {
                foreach (var hitbox in _hitBoxes)
                {
                    InvisibleTile tile = new InvisibleTile(grid, hitbox);
                }
            }
        }

        public override CollisionType CollisionType => CollisionType.NONE;

        protected virtual string BuildRectangleArrayStringRepresentation(Rectangle[] rectangles)
        {
            StringBuilder builder = new StringBuilder();

            string arrayStart = MapMakerConstants.MAP_MAKER_ARRAY_START;
            string arrayEnd = MapMakerConstants.MAP_MAKER_ARRAY_END;
            string elementSeparator = MapMakerConstants.MAP_MAKER_ELEMENT_SEPARATOR;

            builder.Append(arrayStart);
            string[] elementStringReps = rectangles.Select(r => 
             $"{r.X}{elementSeparator}{r.Y}{elementSeparator}{r.Width}{elementSeparator}{r.Height}").ToArray();

            string arrStr = string.Join(elementSeparator, elementStringReps);
            builder.Append(arrStr);
            builder.Append(arrayEnd);

            return builder.ToString();
        }

        public override string ToString()
        {
            string hitBoxArr = BuildRectangleArrayStringRepresentation(_hitBoxes);
            return base.ToString() + $"{_separator}{hitBoxArr}";
        }
    }
}
