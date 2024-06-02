using KeenReloaded.Framework;
using KeenReloaded2.Constants;
using KeenReloaded2.Framework.Enums;
using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items
{
    public class Viva : Item, IDropCollector
    {
        private bool _perch;
        public Viva(Rectangle area, SpaceHashGrid grid, string imageName, int zIndex, bool perch)
            : base(area, imageName, grid, zIndex)
        {
            _perch = perch;
            Initialize();
        }
        public int DropVal
        {
            get { return 1; }
        }

        public bool Perched
        {
            get
            {
                return _perch;
            }
        }

        public override CollisionType CollisionType => CollisionType.ITEM;

        private void Initialize()
        {
            _canSteal = true;
            _moveUp = false;
            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.keen6_viva_acquired1,
                Properties.Resources.keen6_viva_acquired2,
                Properties.Resources.keen6_viva_acquired3,
                Properties.Resources.keen6_viva_acquired4
            };

            if (_perch)
            {
                this.SpriteList = new Image[]
                {
                    Properties.Resources.keen6_viva_perched1,
                    Properties.Resources.keen6_viva_perched2,
                    Properties.Resources.keen6_viva_perched3,
                    Properties.Resources.keen6_viva_perched4
                };
            }
            else
            {
                this.SpriteList = new Image[]
                {
                    Properties.Resources.keen6_viva_flying1,
                    Properties.Resources.keen6_viva_flying2,
                    Properties.Resources.keen6_viva_flying3
                };
            }
            this.Image = this.SpriteList[0];
        }

        public override string ToString()
        {
            string separator = MapMakerConstants.MAP_MAKER_PROPERTY_SEPARATOR;
            return base.ToString() + $"{separator}{this.ZIndex}{separator}{this.Perched}";
        }
    }
}
