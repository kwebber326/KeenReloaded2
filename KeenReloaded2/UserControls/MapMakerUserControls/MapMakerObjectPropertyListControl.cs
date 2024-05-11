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
using KeenReloaded2.Utilities;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class MapMakerObjectPropertyListControl : UserControl
    {
        private const int VERTICAL_MARGIN = 8;
        public MapMakerObjectPropertyListControl()
        {
            InitializeComponent();
        }

        private void MapMakerObjectPropertyListControl_Load(object sender, EventArgs e)
        {
            lblHeader.Text = string.Empty;
        }

        public void SetProperties(MapMakerObject mapMakerObject)
        {
            this.Controls.Clear();
            pbObjectImage.Image = null;

            if (mapMakerObject == null || mapMakerObject.ConstructorParameters == null || mapMakerObject.ObjectType == null)
                return;

            this.Controls.Add(pbObjectImage);
            this.Controls.Add(lblHeader);
            this.Controls.Add(btnPlace);
            btnPlace.Visible = mapMakerObject.IsManualPlacement;

            if (!string.IsNullOrEmpty(mapMakerObject.ImageControl?.ImageLocation))
                pbObjectImage.Image = Image.FromFile(mapMakerObject.ImageControl.ImageLocation);

            lblHeader.Text = $"{mapMakerObject.ObjectType} Properties:";

            int x = 0, y = pbObjectImage.Bottom + VERTICAL_MARGIN;
            for (int i = 0; i < mapMakerObject.ConstructorParameters.Length; i++)
            {
                var property = mapMakerObject.ConstructorParameters[i];
                MapMakerObjectPropertyControl control = new MapMakerObjectPropertyControl(property);
                control.Location = new Point(x, y);
                this.Controls.Add(control);

                y = control.Bottom + VERTICAL_MARGIN;
            }
        }

        private void BtnPlace_Click(object sender, EventArgs e)
        {

        }
    }
}
