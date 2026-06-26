using KeenReloaded.Framework;
using KeenReloaded.Framework.Utilities;
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
        protected readonly Rectangle[] _foregroundAreas;
        protected bool _updated;

        public ForeGroundMultiHitboxWorldMapLevel(Rectangle area, SpaceHashGrid grid, int zIndex, Image sprite, string levelName, string levelEntryText, string episode, string music, Guid activationId, Rectangle[] entryPoints, Rectangle[] hitboxes, Rectangle[] foregroundAreas)
            : base(area, grid, zIndex, sprite, levelName, levelEntryText, episode, music, activationId, entryPoints, hitboxes)
        {
            _foregroundAreas = foregroundAreas;
        }

        public event EventHandler<ObjectEventArgs> Create;
        public event EventHandler<ObjectEventArgs> Remove;

        protected void OnCreate(ObjectEventArgs e)
        {
            this.Create?.Invoke(this, e);
        }

        protected void OnRemove(ObjectEventArgs e)
        {
            this.Remove?.Invoke(this, e);
        }

        public virtual void Update()
        {
            if (!_updated)
            {
                foreach (var foregroundArea in _foregroundAreas)
                {
                    Image image = _sprite;
                    var area = new Rectangle(this.HitBox.X + foregroundArea.X,
                        this.HitBox.Y + foregroundArea.Y, foregroundArea.Width, foregroundArea.Height);
                    image = BitMapTool.CropImage(image, foregroundArea);
                    Background background = new Background(area
                        , image, false, _zIndex + 200);
                    ObjectEventArgs e = new ObjectEventArgs()
                    {
                        ObjectSprite = background
                    };
                    OnCreate(e);
                }
                _updated = true;
            }
        }

        public override bool CanUpdate => true;

        public override string ToString()
        {
            string foregroundArr = BuildRectangleArrayStringRepresentation(_foregroundAreas);
            return base.ToString() + $"{_separator}{foregroundArr}";
        }
    }
}
