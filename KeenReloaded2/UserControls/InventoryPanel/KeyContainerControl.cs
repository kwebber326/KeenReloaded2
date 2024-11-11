using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded.Framework.Utilities;
using KeenReloaded2.Framework.Enums;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class KeyContainerControl : UserControl
    {
        const int GEM_WIDTH = 14;
        const int GEM_HEIGHT = 10;
        const int HORIZONTAL_OFFSET = 83;
        const int VERTICAL_OFFSET = 10;
        const int MARGIN = 2;
        private Image _originalImage;

        private Dictionary<GemColor, Image> _gemInventoryImages = new Dictionary<GemColor, Image>()
        {
            { GemColor.RED, Properties.Resources.red_gem_inventory },
            { GemColor.BLUE, Properties.Resources.inventory_blue_gem },
            { GemColor.YELLOW, Properties.Resources.inventory_yellow_gem },
            { GemColor.GREEN, Properties.Resources.inventory_green_gem }
        };

        private HashSet<GemColor> _gems = new HashSet<GemColor>();

        public KeyContainerControl()
        {
            InitializeComponent();
            _originalImage = pbDisplay.Image;
        }

        private void KeyContainerControl_Load(object sender, EventArgs e)
        {

        }

        public void AddGem(GemColor gemGemColor)
        {
            int oldCount = _gems.Count;
            _gems.Add(gemGemColor);
            int newCount = _gems.Count;
            if (oldCount != newCount)
            {
                UpdateDisplay();
            }
        }

        public void RemoveGem(GemColor gemGemColor)
        {
            if (_gems.Contains(gemGemColor))
            {
                _gems.Remove(gemGemColor);
                UpdateDisplay();
            }
        }

        public void ResetInventory()
        {
            this.RemoveGem(GemColor.BLUE);
            this.RemoveGem(GemColor.GREEN);
            this.RemoveGem(GemColor.RED);
            this.RemoveGem(GemColor.YELLOW);
        }

        private void UpdateDisplay()
        {
            List<Image> images = new List<Image>();
            List<Point> locations = new List<Point>();

            int x = HORIZONTAL_OFFSET, y = VERTICAL_OFFSET;
            foreach (var gem in _gems)
            {
                if (_gemInventoryImages.TryGetValue(gem, out Image img))
                {
                    images.Add(img);
                    locations.Add(new Point(x, y));
                    x += GEM_WIDTH + MARGIN;
                }
            }
            
            pbDisplay.Image = BitMapTool.DrawImagesOnCanvas(pbDisplay.Size, _originalImage, images.ToArray(), locations.ToArray());
        }
    }
}
