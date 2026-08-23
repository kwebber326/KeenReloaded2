namespace KeenReloaded2
{
    partial class WorldMapModeMainMenu
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
            this.pbScreen = new System.Windows.Forms.PictureBox();
            this.pbSelector = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelector)).BeginInit();
            this.SuspendLayout();
            // 
            // pbScreen
            // 
            this.pbScreen.Image = global::KeenReloaded2.Properties.Resources.keen_main_menu;
            this.pbScreen.Location = new System.Drawing.Point(0, 0);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(1024, 1024);
            this.pbScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbScreen.TabIndex = 0;
            this.pbScreen.TabStop = false;
            this.pbScreen.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pbSelector
            // 
            this.pbSelector.BackColor = System.Drawing.Color.Transparent;
            this.pbSelector.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbSelector.Image = global::KeenReloaded2.Properties.Resources.MainMenuSelector;
            this.pbSelector.Location = new System.Drawing.Point(343, 343);
            this.pbSelector.Name = "pbSelector";
            this.pbSelector.Size = new System.Drawing.Size(13, 13);
            this.pbSelector.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSelector.TabIndex = 1;
            this.pbSelector.TabStop = false;
            // 
            // WorldMapModeMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 881);
            this.Controls.Add(this.pbSelector);
            this.Controls.Add(this.pbScreen);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorldMapModeMainMenu";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorldMapModeMainMenu_FormClosing);
            this.Load += new System.EventHandler(this.WorldMapModeMainMenu_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.WorldMapModeMainMenu_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.WorldMapModeMainMenu_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelector)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbScreen;
        private System.Windows.Forms.PictureBox pbSelector;
    }
}