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
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.Framework.GameEntities.Interfaces;

namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    public partial class MapMakerObjectPropertyListControl : UserControl
    {
        private const int VERTICAL_MARGIN = 8;
        private MapMakerObject _mapMakerObject;
        private List<GameObjectMapping> _gameObjectMappings;

        public event EventHandler<MapMakerObjectEventArgs> PlaceObjectClicked;
        public MapMakerObjectPropertyListControl()
        {
            InitializeComponent();
        }

        private void MapMakerObjectPropertyListControl_Load(object sender, EventArgs e)
        {
            lblHeader.Text = string.Empty;
        }

        public void SetObjectBank(List<GameObjectMapping> gameObjectMappings)
        {
            _gameObjectMappings = gameObjectMappings;
        }

        public void SetProperties(MapMakerObject mapMakerObject, bool overrideToAutoPlace = false, bool autoPlace = false)
        {
            this.Controls.Clear();
            this.Controls.Add(pnlControls);
            pnlControls.Controls.Clear();
            pbObjectImage.Image = null;

            if (mapMakerObject == null || mapMakerObject.ConstructorParameters == null || mapMakerObject.ObjectType == null)
                return;

            this.Controls.Add(pbObjectImage);
            this.Controls.Add(lblHeader);
            this.Controls.Add(btnPlace);
            btnPlace.Visible = mapMakerObject.IsManualPlacement || overrideToAutoPlace;
            _mapMakerObject = mapMakerObject;

            if (!string.IsNullOrEmpty(mapMakerObject.ImageControl?.ImageLocation))
                pbObjectImage.Image = Image.FromFile(mapMakerObject.ImageControl.ImageLocation);

            lblHeader.Text = $"{mapMakerObject.ObjectType.Name} Properties:";

            int x = 0, y = 0;
            for (int i = 0; i < mapMakerObject.ConstructorParameters.Length; i++)
            {
                var property = mapMakerObject.ConstructorParameters[i];
                if (!property.Hidden)
                {
                    var activateables = _gameObjectMappings?.Select(g => g.GameObject).OfType<IActivateable>()?.ToList() ?? new List<IActivateable>();
                    MapMakerObjectPropertyControl control = new MapMakerObjectPropertyControl(property, activateables);
                    control.Location = new Point(x, y);
                    pnlControls.Controls.Add(control);
                    control.BringToFront();

                    y = control.Bottom + VERTICAL_MARGIN;
                }
            }

            if (autoPlace)
            {
                BtnPlace_Click(this, EventArgs.Empty);
            }
        }

        private void BtnPlace_Click(object sender, EventArgs e)
        {
            MapMakerObjectEventArgs args = new MapMakerObjectEventArgs()
            {
                MapMakerObject = _mapMakerObject
            };

            this.PlaceObjectClicked?.Invoke(this, args);
        }
    }
}
