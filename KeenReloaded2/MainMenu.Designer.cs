namespace KeenReloaded2
{
    partial class MainMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlMenuOptions = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pbCharacter = new System.Windows.Forms.PictureBox();
            this.chkSounds = new System.Windows.Forms.CheckBox();
            this.chkMusic = new System.Windows.Forms.CheckBox();
            this.chkPartialUpdate = new System.Windows.Forms.CheckBox();
            this.toolTipPartialAlgo = new System.Windows.Forms.ToolTip(this.components);
            this.musicSelectControl1 = new KeenReloaded2.UserControls.MainMenuControls.MusicSelectControl();
            this.characterSelectControl1 = new KeenReloaded2.UserControls.MainMenuControls.CharacterSelectControl();
            ((System.ComponentModel.ISupportInitialize)(this.pbCharacter)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMenuOptions
            // 
            this.pnlMenuOptions.BackColor = System.Drawing.Color.Transparent;
            this.pnlMenuOptions.Location = new System.Drawing.Point(387, 74);
            this.pnlMenuOptions.Name = "pnlMenuOptions";
            this.pnlMenuOptions.Size = new System.Drawing.Size(691, 666);
            this.pnlMenuOptions.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.YellowGreen;
            this.label1.Location = new System.Drawing.Point(365, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(357, 48);
            this.label1.TabIndex = 1;
            this.label1.Text = "Keen Reloaded";
            // 
            // pbCharacter
            // 
            this.pbCharacter.BackColor = System.Drawing.Color.Transparent;
            this.pbCharacter.Location = new System.Drawing.Point(264, 74);
            this.pbCharacter.Name = "pbCharacter";
            this.pbCharacter.Size = new System.Drawing.Size(100, 50);
            this.pbCharacter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbCharacter.TabIndex = 2;
            this.pbCharacter.TabStop = false;
            // 
            // chkSounds
            // 
            this.chkSounds.AutoSize = true;
            this.chkSounds.BackColor = System.Drawing.Color.Transparent;
            this.chkSounds.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSounds.ForeColor = System.Drawing.Color.YellowGreen;
            this.chkSounds.Location = new System.Drawing.Point(12, 283);
            this.chkSounds.Name = "chkSounds";
            this.chkSounds.Size = new System.Drawing.Size(120, 29);
            this.chkSounds.TabIndex = 4;
            this.chkSounds.Text = "Sounds";
            this.chkSounds.UseVisualStyleBackColor = false;
            this.chkSounds.CheckedChanged += new System.EventHandler(this.ChkSounds_CheckedChanged);
            // 
            // chkMusic
            // 
            this.chkMusic.AutoSize = true;
            this.chkMusic.BackColor = System.Drawing.Color.Transparent;
            this.chkMusic.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMusic.ForeColor = System.Drawing.Color.YellowGreen;
            this.chkMusic.Location = new System.Drawing.Point(12, 318);
            this.chkMusic.Name = "chkMusic";
            this.chkMusic.Size = new System.Drawing.Size(102, 29);
            this.chkMusic.TabIndex = 5;
            this.chkMusic.Text = "Music";
            this.chkMusic.UseVisualStyleBackColor = false;
            this.chkMusic.CheckedChanged += new System.EventHandler(this.ChkMusic_CheckedChanged);
            // 
            // chkPartialUpdate
            // 
            this.chkPartialUpdate.AutoSize = true;
            this.chkPartialUpdate.BackColor = System.Drawing.Color.Transparent;
            this.chkPartialUpdate.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPartialUpdate.ForeColor = System.Drawing.Color.YellowGreen;
            this.chkPartialUpdate.Location = new System.Drawing.Point(12, 353);
            this.chkPartialUpdate.Name = "chkPartialUpdate";
            this.chkPartialUpdate.Size = new System.Drawing.Size(304, 29);
            this.chkPartialUpdate.TabIndex = 5;
            this.chkPartialUpdate.Text = "Use Partial Update Algo";
            this.chkPartialUpdate.UseVisualStyleBackColor = false;
            this.chkPartialUpdate.CheckedChanged += new System.EventHandler(this.ChkPartialUpdate_CheckedChanged);
            this.chkPartialUpdate.MouseLeave += new System.EventHandler(this.ChkPartialUpdate_MouseLeave);
            this.chkPartialUpdate.MouseHover += new System.EventHandler(this.ChkPartialUpdate_MouseHover);
            // 
            // toolTipPartialAlgo
            // 
            this.toolTipPartialAlgo.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipPartialAlgo.Popup += new System.Windows.Forms.PopupEventHandler(this.ToolTipPartialAlgo_Popup);
            // 
            // musicSelectControl1
            // 
            this.musicSelectControl1.BackColor = System.Drawing.Color.Transparent;
            this.musicSelectControl1.Location = new System.Drawing.Point(12, 389);
            this.musicSelectControl1.Margin = new System.Windows.Forms.Padding(4);
            this.musicSelectControl1.Name = "musicSelectControl1";
            this.musicSelectControl1.Size = new System.Drawing.Size(254, 223);
            this.musicSelectControl1.TabIndex = 6;
            // 
            // characterSelectControl1
            // 
            this.characterSelectControl1.BackColor = System.Drawing.Color.Transparent;
            this.characterSelectControl1.Location = new System.Drawing.Point(-2, 74);
            this.characterSelectControl1.Margin = new System.Windows.Forms.Padding(4);
            this.characterSelectControl1.Name = "characterSelectControl1";
            this.characterSelectControl1.Size = new System.Drawing.Size(224, 233);
            this.characterSelectControl1.TabIndex = 3;
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::KeenReloaded2.Properties.Resources.keen4_living_tree_background2;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1100, 772);
            this.Controls.Add(this.chkPartialUpdate);
            this.Controls.Add(this.musicSelectControl1);
            this.Controls.Add(this.chkMusic);
            this.Controls.Add(this.chkSounds);
            this.Controls.Add(this.characterSelectControl1);
            this.Controls.Add(this.pbCharacter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlMenuOptions);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "MainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainMenu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainMenu_FormClosing);
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainMenu_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbCharacter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMenuOptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbCharacter;
        private UserControls.MainMenuControls.CharacterSelectControl characterSelectControl1;
        private System.Windows.Forms.CheckBox chkSounds;
        private System.Windows.Forms.CheckBox chkMusic;
        private UserControls.MainMenuControls.MusicSelectControl musicSelectControl1;
        private System.Windows.Forms.CheckBox chkPartialUpdate;
        private System.Windows.Forms.ToolTip toolTipPartialAlgo;
    }
}