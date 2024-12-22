using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.Entities
{
    public class GameObjectMapping : PictureBox
    {
        public MapMakerObject MapMakerObject { get; set; }

        public ISprite GameObject { get; set; }

        public override string ToString()
        {
            return $"Type: {this.MapMakerObject?.ObjectType?.Name}, Location: ({ this.GameObject?.Location.X }, {this.GameObject?.Location.X})";
        }
    }
}
