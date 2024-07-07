using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo
{
    public class NeuralStunnerAmmo : Item
    {
        private int _ammoAmmount;
        public NeuralStunnerAmmo(SpaceHashGrid grid, Rectangle area, string imageName, int zIndex, int ammo)
            : base(area, imageName, grid, zIndex)
        {
            Initialize();
            _ammoAmmount = ammo;
        }

        protected virtual void Initialize()
        {
            this.SpriteList = new Image[]
            {
                Properties.Resources.neural_stunner1,
                Properties.Resources.neural_stunner2
            };

            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.neural_stunner_acquired
            };
            _sprite = Properties.Resources.neural_stunner1;
        }

        public int AmmoAmount
        {
            get
            {
                return _ammoAmmount;
            }
        }

        public override CollisionType CollisionType => CollisionType.ITEM;

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return $"{_imageName}{ separator }{this._area.Location.X}{ separator }{this._area.Location.Y}{ separator }{this._area.Width}{ separator }{this._area.Height}{ separator }{this.ZIndex}{separator}{this.AmmoAmount}";
        }
    }
}
