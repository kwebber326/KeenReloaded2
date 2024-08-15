using KeenReloaded2.Constants;
using KeenReloaded2.Entities;
using KeenReloaded2.Framework.GameEntities.Players;
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
        private Timer _gameUpdateTimer = new Timer();
        private CommanderKeenGame _game;
        private bool _paused;
        private CommanderKeen _keen;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string gameMode, MapMakerData data)
        {
            InitializeComponent();
            _gameMode = gameMode;
            _game = new CommanderKeenGame(data);
            var gameObjects = data.MapData.Select(d => d.GameObject);
            _keen = gameObjects.OfType<CommanderKeen>().FirstOrDefault();
            inventoryPanel1.Keen = _keen;
            inventoryPanel1.ShowFlagInventory = gameMode == MainMenuConstants.OPTION_LABEL_CTF_MODE;
        }

        private void InitializeGameState()
        {
            _gameUpdateTimer = new Timer();
            _gameUpdateTimer.Interval = 50;
            _gameUpdateTimer.Tick += _gameUpdateTimer_Tick;
            _gameUpdateTimer.Start();
        }

        private void _gameUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!_paused)
            {
                pbGameImage.Image = _game.UpdateGame();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return base.IsInputKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    KeyEventArgs args = new KeyEventArgs(keyData);
                    base.OnKeyDown(args);
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGameState();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameUpdateTimer.Stop();
            _gameUpdateTimer.Tick -= _gameUpdateTimer_Tick;
            _gameUpdateTimer.Dispose();
            _game.Dispose();
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _game.SetKeyPressed(e.KeyCode.ToString(), true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                OpenDialog(e.KeyCode);
            }
            else
            {
                _game.SetKeyPressed(e.KeyCode.ToString(), false);
            }
        }

        private void OpenDialog(Keys key)
        {

            switch (key)
            {
                case Keys.Escape:
                    //_paused = true;
                    //var messageWindow = new KeenReloadedYesNoDialogWindow("Are you sure you want to quit?", false);

                    //var dialogResult = messageWindow.ShowDialog();
                    //if (dialogResult == DialogResult.No)
                    //{
                    //    _paused = false;
                    //}
                    //else
                    //{
                    //    this.Close();
                    //}
                    break;
            }
        }
    }
}
