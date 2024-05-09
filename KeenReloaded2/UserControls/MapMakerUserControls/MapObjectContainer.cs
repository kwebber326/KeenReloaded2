using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Entities;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class MapObjectContainer : UserControl
    {
        public const int ROW_LENGTH = 300;
        private List<MapMakerObject> mapMakerObjects = new List<MapMakerObject>();
        public MapObjectContainer()
        {
            InitializeComponent();
        }

        private void MapObjectContainer_Load(object sender, EventArgs e)
        {

        }

        public void DisplayImageFiles(string[] files)
        {
            int x, y;
            for (int i = 0; i < files.Length; i++)
            {
                //TODO: show the files as images in the current container
            }
        }
    }
}
