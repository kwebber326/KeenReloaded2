using KeenReloaded2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2
{
    public partial class AdvancedToolsForm : Form
    {
        private List<GameObjectMapping> _gameObjects;

        public AdvancedToolsForm()
        {
            InitializeComponent();
        }

        public AdvancedToolsForm(List<GameObjectMapping> gameObjects)
        {
            InitializeComponent();
            _gameObjects = gameObjects ?? new List<GameObjectMapping>();
        }

        private void AdvancedToolsForm_Load(object sender, EventArgs e)
        {
            PopulateListBoxWithCurrentItems();
        }

        private void PopulateListBoxWithCurrentItems()
        {
            lstMapObjects.Items.Clear();
            if (_gameObjects == null)
                return;
            foreach (var obj in _gameObjects)
            {
                lstMapObjects.Items.Add(obj.ToString());
            }
        }
    }
}
