namespace KeenReloaded2
{
    partial class Form1
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
            this.pnlGameWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGameImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlGameWindow
            // 
            this.pnlGameWindow.AutoScroll = true;
            this.pnlGameWindow.Controls.Add(this.pbGameImage);
            this.pnlGameWindow.Location = new System.Drawing.Point(614, 12);
            this.pnlGameWindow.Name = "pnlGameWindow";
            this.pnlGameWindow.Size = new System.Drawing.Size(1552, 1450);
            this.pnlGameWindow.TabIndex = 0;
            // 
            // pbGameImage
            // 
            this.pbGameImage.Location = new System.Drawing.Point(3, 3);
            this.pbGameImage.Name = "pbGameImage";
            this.pbGameImage.Size = new System.Drawing.Size(100, 50);
            this.pbGameImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbGameImage.TabIndex = 0;
            this.pbGameImage.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2178, 1474);
            this.Controls.Add(this.pnlGameWindow);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Keen Reloaded";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.pnlGameWindow.ResumeLayout(false);
            this.pnlGameWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGameImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlGameWindow;
        private System.Windows.Forms.PictureBox pbGameImage;
    }
}

