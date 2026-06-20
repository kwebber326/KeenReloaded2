using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using KeenReloaded2.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class WorldMapLevel : CollisionObject, ISprite, IWorldMapLevel
    {
        protected readonly int _zIndex;
        protected Image _sprite;
        protected readonly Rectangle _area;
        protected readonly Rectangle _levelActionArea;
        protected string _separator;
        protected string _objectKey;
        protected readonly string _levelName;
        protected readonly string _levelEntryText;
        protected readonly Rectangle[] _levelEntryPoints;
        protected readonly Rectangle[] _trueEntryPoints;
        protected readonly string _episode;
        private readonly string _music;

        public event EventHandler WorldMapEntered;

        public WorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Rectangle[] entryPoints) 
            : base(grid, area)
        {
            _zIndex = zIndex;
            _sprite = sprite;
            _area = area;
            _separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            _objectKey = _sprite?.Tag?.ToString() ?? "unknown_level";
            _objectKey = FileIOUtility.ExtractFileNameFromPath(_objectKey);
            _levelName = levelName;
            _levelEntryText = levelEntryText;
            _episode = episode;
            _music = music;
            _levelEntryPoints = entryPoints;
            _trueEntryPoints = entryPoints.Select(e => new Rectangle(
                this.HitBox.X + e.X,
                this.HitBox.Y + e.Y,
                e.Width, e.Height)).ToArray();
        }

        public int ZIndex => _zIndex;

        public Image Image => _sprite;

        public Point Location => _area.Location;

        public virtual bool CanUpdate => false;

        public override CollisionType CollisionType => CollisionType.NONE;

        public string LevelName => _levelName;

        public string LevelEntryText => _levelEntryText;

        public string Episode => _episode;

        public  string Music => _music;

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

        public void TryEnterLevel(WorldMapPlayer player)
        {
            if (player == null ||
                !_trueEntryPoints.Any(e => e.IntersectsWith(player.HitBox)))
                return;

            WorldMapEntered?.Invoke(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            string entryPointsArr = BuildRectangleArrayStringRepresentation(_levelEntryPoints);
            return $"{_objectKey}{_separator}"+ 
                $"{_area.X}{_separator}{_area.Y}{_separator}{_area.Width}{_separator}{_area.Height}"+
                $"{_separator}{_zIndex}" + 
                $"{_separator}{_levelName}{_separator}{_levelEntryText}{_separator}{_episode}{_separator}{_music}{_separator}{entryPointsArr}";
        }
    }
}
