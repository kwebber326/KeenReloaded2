using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeenReloaded2.Constants;
using KeenReloaded2.ControlEventArgs;
using KeenReloaded2.Utilities;

namespace KeenReloaded2.UserControls.MainMenuControls
{
    public partial class CharacterSelectControl : UserControl
    {
        public event EventHandler<CharacterSelectControlEventArgs> SelectedCharacterChanged;
        Dictionary<string, Bitmap> _characters = new Dictionary<string, Bitmap>()
        {
            { MainMenuConstants.Characters.COMMANDER_KEEN, Properties.Resources.keen_shoot_right_standing },
            { MainMenuConstants.Characters.BILLY_BLAZE, Properties.Resources.keen_dreams_shoot_right_standing },
            { MainMenuConstants.Characters.MORTIMER_MCMIRE, Properties.Resources.mm_shoot_right_standing },
            { MainMenuConstants.Characters.PRINCESS_LINDSEY, Properties.Resources.princess_indi_shoot_right_standing },
            { MainMenuConstants.Characters.COUNCIL_PAGE, Properties.Resources.council_page_shoot_right_standing },
            { MainMenuConstants.Characters.ORACLE_ELDER, Properties.Resources.oracle_elder_shoot_right_standing },
            { MainMenuConstants.Characters.LT_BARKER, Properties.Resources.baby_louie_shoot_right_standing },
            { MainMenuConstants.Characters.YORP, Properties.Resources.yorp_shoot_right_standing }
        };
        public CharacterSelectControl()
        {
            InitializeComponent();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.OnSelectedCharacterChanged();
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void CharacterSelectControl_Load(object sender, EventArgs e)
        {
            foreach (var key in _characters.Keys)
            {
                cmbCharacters.Items.Add(key);
            }
            string savedCharacterName = FileIOUtility.LoadSavedCharacterSelection();
            if (!string.IsNullOrEmpty(savedCharacterName))
            {
                int index = _characters.Keys.ToList().IndexOf(savedCharacterName);
                if (index == -1)
                    index = 0;
                cmbCharacters.SelectedIndex = index;
            }
            else
            {
                cmbCharacters.SelectedIndex = 0;
            }
            
        }

        public string SelectedCharacterName
        {
            get
            {
                return cmbCharacters.SelectedItem?.ToString();
            }
        }

        public Bitmap SelectedImage
        {
            get
            {
                string selectedCharacterName = cmbCharacters.SelectedItem?.ToString();
                if (!string.IsNullOrWhiteSpace(selectedCharacterName) &&
                    _characters.TryGetValue(selectedCharacterName, out Bitmap image))
                {
                    return image;
                }
                return null;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!msg.HWnd.Equals(this.Handle) &&
                (keyData == Keys.Left || keyData == Keys.Right ||
                keyData == Keys.Up || keyData == Keys.Down))
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected void OnSelectedCharacterChanged()
        {
            CharacterSelectControlEventArgs e = new CharacterSelectControlEventArgs()
            {
                CharacterName = this.SelectedCharacterName,
                Image = this.SelectedImage
            };
            this.SelectedCharacterChanged?.Invoke(this, e);
        }

    }
}
