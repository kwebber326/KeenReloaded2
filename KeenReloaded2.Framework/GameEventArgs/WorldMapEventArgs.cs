using KeenReloaded2.Framework.GameEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Framework.GameEventArgs
{
    public class WorldMapEventArgs : EventArgs
    {
        public Size MapSize { get; set; }

        public string MapName { get; set; }

        public string GameMode { get; set; }

        public string MapPath { get; set; }

        public List<ISprite> MapData { get; set; }
    }
}
