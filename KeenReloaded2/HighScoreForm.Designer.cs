namespace KeenReloaded2
{
    partial class HighScoreForm
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
            this.pbHighScoreImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbHighScoreImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pbHighScoreImage
            // 
            this.pbHighScoreImage.Image = global::KeenReloaded2.Properties.Resources.high_score_panel;
            this.pbHighScoreImage.Location = new System.Drawing.Point(-3, -2);
            this.pbHighScoreImage.Name = "pbHighScoreImage";
            this.pbHighScoreImage.Size = new System.Drawing.Size(955, 583);
            this.pbHighScoreImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbHighScoreImage.TabIndex = 0;
            this.pbHighScoreImage.TabStop = false;
            // 
            // HighScoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 577);
            this.ControlBox = false;
            this.Controls.Add(this.pbHighScoreImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HighScoreForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.HighScoreForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HighScoreForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbHighScoreImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbHighScoreImage;
    }
}