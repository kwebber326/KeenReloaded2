using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Framework.GameEntities.Players;
using KeenReloaded2.Framework.GameEntities.Items.WeaponsAmmo;

namespace KeenReloaded2.UserControls.InventoryPanel
{
    public partial class InventoryPanel : UserControl
    {
        private CommanderKeen _keen;

        public InventoryPanel()
        {
            InitializeComponent();
        }

        private void InventoryPanel_Load(object sender, EventArgs e)
        {

        }

        public CommanderKeen Keen
        {
            get
            {
                return _keen;
            }
            set
            {
                if (_keen != null)
                {
                    UnRegisterKeenEvents();
                }
                _keen = value;
                if (_keen != null)
                {
                    scoreBoard1.Keen = _keen;
                    RegisterKeenEvents();
                }
            }
        }

        private void RegisterKeenEvents()
        {
            if (_keen == null)
                return;

            _keen.KeenAcquiredItem += _keen_KeenAcquiredItem;
            _keen.KeenAcquiredWeapon += _keen_KeenAcquiredWeapon;
            _keen.KeenDied += _keen_KeenDied;
            _keen.ScoreChanged += _keen_ScoreChanged;
            _keen.ShieldAcquired += _keen_ShieldAcquired;
            _keen.WeaponChanged += _keen_WeaponChanged;
            _keen.LivesChanged += _keen_LivesChanged;
            _keen.LifeDropsChanged += _keen_LifeDropsChanged;
        }

        private void UnRegisterKeenEvents()
        {
            if (_keen == null)
                return;

            _keen.KeenAcquiredItem -= _keen_KeenAcquiredItem;
            _keen.KeenAcquiredWeapon -= _keen_KeenAcquiredWeapon;
            _keen.KeenDied -= _keen_KeenDied;
            _keen.ScoreChanged -= _keen_ScoreChanged;
            _keen.ShieldAcquired -= _keen_ShieldAcquired;
            _keen.WeaponChanged -= _keen_WeaponChanged;
            _keen.LivesChanged -= _keen_LivesChanged;
            _keen.LifeDropsChanged -= _keen_LifeDropsChanged;
        }

        private void _keen_LifeDropsChanged(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {

        }

        private void _keen_LivesChanged(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {
            scoreBoard1.UpdateScoreBoard();
        }

        private void _keen_ShieldAcquired(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {
            //TODO: implement 
        }

        private void _keen_WeaponChanged(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {
            scoreBoard1.UpdateScoreBoard();
        }

        private void _keen_ScoreChanged(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {
            scoreBoard1.UpdateScoreBoard();
        }

        private void _keen_KeenDied(object sender, Framework.GameEventArgs.ObjectEventArgs e)
        {
            scoreBoard1.UpdateScoreBoard();
        }

        private void _keen_KeenAcquiredWeapon(object sender, Framework.GameEventArgs.WeaponAcquiredEventArgs e)
        {
            scoreBoard1.UpdateScoreBoard();
        }

        private void _keen_KeenAcquiredItem(object sender, Framework.GameEventArgs.ItemAcquiredEventArgs e)
        {
            if (e.Item is NeuralStunnerAmmo)
            {
                scoreBoard1.UpdateScoreBoard();
            }
        }
    }
}
