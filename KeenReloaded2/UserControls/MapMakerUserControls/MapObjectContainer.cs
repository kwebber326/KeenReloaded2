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
using KeenReloaded2.Entities.ReferenceData;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class MapObjectContainer : UserControl
    {
        private const int ROW_LENGTH = 300;
        private const int MAP_MAKER_OBJECT_MARGIN = 8;
        private PictureBox _selectedItem;
        private int _originalHeight;

        public event EventHandler<MapMakerObjectEventArgs> ObjectClicked;

        public MapObjectContainer()
        {
            InitializeComponent();
            _originalHeight = pnlImages.Height;
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
            pnlImages.Controls.Clear();
            pnlImages.Size = new Size(pnlImages.Width, _originalHeight);
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
                    pnlImages.Controls.Add(pb);

                    if (pb.Bottom > pnlImages.Height)
                    {
                        pnlImages.Size = new Size(pnlImages.Width, pb.Bottom);
                    }

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
                if (_selectedItem != null)
                {
                    _selectedItem.BorderStyle = BorderStyle.None;
                    _selectedItem.BackColor = Color.Transparent;
                }
                lblHelpText.Visible = true;
                pbControl.BorderStyle = BorderStyle.Fixed3D;
                pbControl.BackColor = Color.Red;
                _selectedItem = pbControl;
                var img = pbControl.ImageLocation;
                var imgName = FileIOUtility.ExtractFileNameFromPath(img);

                MapMakerObject mapMakerObject = ImageToObjectCreationFactory.GetMapMakerObjectFromImageName(imgName);
                MapMakerObjectEventArgs args = new MapMakerObjectEventArgs()
                {
                    MapMakerObject = mapMakerObject
                };
                ObjectClicked?.Invoke(this, args);
            }
        }

        public void ClearSelection()
        {
            lblHelpText.Visible = false;
            if (_selectedItem != null)
            {
                _selectedItem.BorderStyle = BorderStyle.None;
                _selectedItem.BackColor = Color.Transparent;
                _selectedItem = null;
            }
        }
    }
}
