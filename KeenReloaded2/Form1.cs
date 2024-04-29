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
    public partial class Form1 : Form
    {
        private readonly string _gameMode;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string gameMode)
        {
            InitializeComponent();
            _gameMode = gameMode;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
