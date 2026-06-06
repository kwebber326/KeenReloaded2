namespace KeenReloaded2
{
    partial class WorldMapPlayerForm
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
            this.pnlGameWindow = new System.Windows.Forms.Panel();
            this.pbGameImage = new System.Windows.Forms.PictureBox();
            this.pbBackgroundImage = new System.Windows.Forms.PictureBox();
            this.pnlGameWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGameImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackgroundImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlGameWindow
            // 
            this.pnlGameWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGameWindow.Controls.Add(this.pbGameImage);
            this.pnlGameWindow.Controls.Add(this.pbBackgroundImage);
            this.pnlGameWindow.Location = new System.Drawing.Point(12, 12);
            this.pnlGameWindow.Name = "pnlGameWindow";
            this.pnlGameWindow.Size = new System.Drawing.Size(904, 837);
            this.pnlGameWindow.TabIndex = 0;
            // 
            // pbGameImage
            // 
            this.pbGameImage.BackColor = System.Drawing.Color.Transparent;
            this.pbGameImage.Location = new System.Drawing.Point(3, 3);
            this.pbGameImage.Name = "pbGameImage";
            this.pbGameImage.Size = new System.Drawing.Size(100, 50);
            this.pbGameImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbGameImage.TabIndex = 1;
            this.pbGameImage.TabStop = false;
            // 
            // pbBackgroundImage
            // 
            this.pbBackgroundImage.Location = new System.Drawing.Point(3, 3);
            this.pbBackgroundImage.Name = "pbBackgroundImage";
            this.pbBackgroundImage.Size = new System.Drawing.Size(100, 50);
            this.pbBackgroundImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbBackgroundImage.TabIndex = 0;
            this.pbBackgroundImage.TabStop = false;
            // 
            // WorldMapPlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 861);
            this.Controls.Add(this.pnlGameWindow);
            this.Name = "WorldMapPlayerForm";
            this.Text = "WorldMapPlayer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorldMapPlayerForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WorldMapPlayerForm_FormClosed);
            this.Load += new System.EventHandler(this.WorldMapPlayerForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WorldMapPlayerForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.WorldMapPlayerForm_KeyUp);
            this.pnlGameWindow.ResumeLayout(false);
            this.pnlGameWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGameImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackgroundImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlGameWindow;
        private System.Windows.Forms.PictureBox pbBackgroundImage;
        private System.Windows.Forms.PictureBox pbGameImage;
    }
}