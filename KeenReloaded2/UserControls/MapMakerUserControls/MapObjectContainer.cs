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
using KeenReloaded2.ControlEventArgs;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class MapObjectContainer : UserControl
    {
        public const int ROW_LENGTH = 300;
        public const int MAP_MAKER_OBJECT_MARGIN = 8;

        public event EventHandler<MapMakerObjectEventArgs> ObjectClicked;

        public MapObjectContainer()
        {
            InitializeComponent();
        }

        private void MapObjectContainer_Load(object sender, EventArgs e)
        {

        }

        public void DisplayImageFiles(string[] files)
        {
            int x = 0, y = 0, maxHeight = 0; 
            this.Controls.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                string fileExtension = files[i].Substring(files[i].LastIndexOf('.') + 1);
                if (fileExtension == "png" || fileExtension == "jpg" || fileExtension == "bmp")
                {
                    var image = Image.FromFile(files[i]);
                    PictureBox pb = new PictureBox();
                    pb.Location = new Point(x, y);
                    pb.SizeMode = PictureBoxSizeMode.AutoSize;
                    pb.Image = image;
                    this.Controls.Add(pb);

                    if (pb.Height > maxHeight)
                        maxHeight = pb.Height;

                    x = pb.Right + MAP_MAKER_OBJECT_MARGIN;
                    if (x > ROW_LENGTH)
                    {
                        x = 0;
                        y += maxHeight + MAP_MAKER_OBJECT_MARGIN;
                        maxHeight = 0;
                    }
                }
            }
        }
    }
}
