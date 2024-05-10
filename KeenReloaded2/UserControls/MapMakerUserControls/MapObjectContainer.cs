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
using KeenReloaded2.Utilities;
using KeenReloaded2.Framework.Enums;

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
            var pbs = this.Controls.OfType<PictureBox>();
            if (pbs.Any())
            {
                foreach (var pb in pbs)
                {
                    pb.Click -= Pb_Click;
                }
            }
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
                    pb.ImageLocation = files[i];
                    pb.Click += Pb_Click;
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

        private void Pb_Click(object sender, EventArgs e)
        {
            PictureBox pbControl = sender as PictureBox;
            if (pbControl != null)
            {
                var pbs = this.Controls.OfType<PictureBox>();
                if (pbs.Any())
                {
                    foreach (var pb in pbs)
                    {
                        pb.BorderStyle = BorderStyle.None;
                        pb.BackColor = Color.Transparent;
                    }
                }
                pbControl.BorderStyle = BorderStyle.Fixed3D;
                pbControl.BackColor = Color.Red;
                var img = pbControl.ImageLocation;
                var imgName = FileIOUtility.ExtractFileNameFromPath(img);

                //TODO: use dictionary
                MapMakerObject mapMakerObject = new MapMakerObject("test Type", img, new MapMakerObjectProperty[] {
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "test1",
                        DisplayName = "test 1",
                        Value = "abcTest",
                        DataType = typeof(string)
                    },
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "test2",
                        DisplayName = "test 2",
                        Value = 12,
                        DataType = typeof(int)
                    },
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "test3",
                        DisplayName = "test 3",
                        Value = true,
                        DataType = typeof(bool)
                    },
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "test4",
                        DisplayName = "test 4",
                        Value = true,
                        DataType = typeof(Enum),
                        PossibleValues = Enum.GetNames(typeof(Direction))
                    },
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "test5",
                        DisplayName = "test 5",
                        Value = true,
                        DataType = typeof(Enum),
                        PossibleValues = Enum.GetNames(typeof(Direction)),
                        Readonly = true
                    },
                    new MapMakerObjectProperty()
                    {
                        PropertyName = "test6",
                        DisplayName = "test 6",
                        Value = "cannot edit",
                        DataType = typeof(string),
                        Readonly = true
                    },
                      new MapMakerObjectProperty()
                    {
                        PropertyName = "test7",
                        DisplayName = "test 7",
                        Value = false,
                        DataType = typeof(bool),
                        Readonly = true
                    },
                }); ;
                MapMakerObjectEventArgs args = new MapMakerObjectEventArgs()
                {
                    MapMakerObject = mapMakerObject
                };
                ObjectClicked?.Invoke(this, args);
            }
        }
    }
}
