namespace KeenReloaded2.UserControls.InventoryPanel
{
    partial class FlagScoreKeeperControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbFlag = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPoints = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbFlag)).BeginInit();
            this.SuspendLayout();
            // 
            // pbFlag
            // 
            this.pbFlag.Location = new System.Drawing.Point(4, 4);
            this.pbFlag.Name = "pbFlag";
            this.pbFlag.Size = new System.Drawing.Size(60, 57);
            this.pbFlag.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFlag.TabIndex = 0;
            this.pbFlag.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "X";
            // 
            // lblPoints
            // 
            this.lblPoints.AutoSize = true;
            this.lblPoints.Location = new System.Drawing.Point(97, 20);
            this.lblPoints.Name = "lblPoints";
            this.lblPoints.Size = new System.Drawing.Size(18, 20);
            this.lblPoints.TabIndex = 2;
            this.lblPoints.Text = "0";
            // 
            // FlagScoreKeeperControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.lblPoints);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbFlag);
            this.ForeColor = System.Drawing.Color.Yellow;
            this.Name = "FlagScoreKeeperControl";
            this.Size = new System.Drawing.Size(207, 64);
            this.Load += new System.EventHandler(this.FlagScoreKeeperControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbFlag)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbFlag;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPoints;
    }
}
