using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.ControlEventArgs
{
    public class CharacterSelectControlEventArgs : EventArgs
    {
        public string CharacterName { get; set; }
        
        public Bitmap Image { get; set; }
    }
}
