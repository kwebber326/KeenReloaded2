using KeenReloaded2.Utilities;
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
    public partial class MapLoader : Form
    {
        private readonly string _gameMode;
        private string _path;

        public MapLoader()
        {
            InitializeComponent();
        }

        public MapLoader(string gameMode)
        {
            InitializeComponent();
            _gameMode = gameMode;
            _path = MapUtility.GetSavedMapsPath(_gameMode);
        }


        private void MapLoader_Load(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = _path;
            openFileDialog1.ShowDialog();
            this.Close();
        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var mapData = MapUtility.LoadMapData(openFileDialog1.FileName);
                Form1 game = new Form1(_gameMode, mapData, false);
                game.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Loading Map {ex}");
            }
        }
    }
}
