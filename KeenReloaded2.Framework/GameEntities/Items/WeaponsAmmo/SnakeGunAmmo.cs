using KeenReloaded.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo
{
    public class SnakeGunAmmo : NeuralStunnerAmmo
    {
        public SnakeGunAmmo(SpaceHashGrid grid, Rectangle area, string imageName, int zIndex, int ammo)
            : base(grid, area, imageName, zIndex, ammo)
        {
        }

        protected override void Initialize()
        {
            this.SpriteList = new Image[]
            {
                Properties.Resources.snake_gun1,
                Properties.Resources.snake_gun2
            };

            this.AcquiredSpriteList = new Image[]
            {
                Properties.Resources.snake_gun_acquired
            };
            _sprite = this.SpriteList.FirstOrDefault();
        }
    }
}
