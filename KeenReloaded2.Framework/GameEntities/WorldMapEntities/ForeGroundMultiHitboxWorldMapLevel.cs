using KeenReloaded.Framework;
using KeenReloaded2.Framework.GameEntities.Backgrounds;
using KeenReloaded2.Framework.GameEventArgs;
using KeenReloaded2.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.WorldMapEntities
{
    public class ForeGroundMultiHitboxWorldMapLevel : MultiHitBoxWorldMapLevel, IUpdatable, ICreateRemove
    {
        private readonly List<Rectangle> _foregroundAreas;
        private bool _updated;

        public ForeGroundMultiHitboxWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, List<Rectangle> hitboxes, Image sprite, string levelName, string levelEntryText, List<Rectangle> foregroundAreas)
            : base(area, grid, zIndex, hitboxes, sprite, levelName, levelEntryText)
        {
            _foregroundAreas = foregroundAreas;
        }

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        protected void OnCreate(ObjectEventArgs e)
        {
            this.Create?.Invoke(this, e);
        }

        public void Update()
        {
            if (!_updated)
            {
                foreach (var foregroundArea in _foregroundAreas)
                {
                    Image image = _sprite;
                    Background background = new Background(foregroundArea, image, true, _zIndex + 200);
                    ObjectEventArgs e = new ObjectEventArgs()
                    {
                        ObjectSprite = background
                    };
                    OnCreate(e);
                }
                _updated = true;
            }
        }
    }
}
