namespace KeenReloaded2.UserControls.InventoryPanel
{
    partial class ShieldInventoryControl
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
            this.lblShieldStatus = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbShieldCount = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbShieldCount)).BeginInit();
            this.SuspendLayout();
            // 
            // lblShieldStatus
            // 
            this.lblShieldStatus.AutoSize = true;
            this.lblShieldStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShieldStatus.ForeColor = System.Drawing.Color.Red;
            this.lblShieldStatus.Location = new System.Drawing.Point(4, 4);
            this.lblShieldStatus.Name = "lblShieldStatus";
            this.lblShieldStatus.Size = new System.Drawing.Size(159, 25);
            this.lblShieldStatus.TabIndex = 0;
            this.lblShieldStatus.Text = "Shield (Inactive):";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::KeenReloaded2.Properties.Resources.Shield_small;
            this.pictureBox1.Location = new System.Drawing.Point(9, 32);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 63);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(76, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "X";
            // 
            // pbShieldCount
            // 
            this.pbShieldCount.Location = new System.Drawing.Point(112, 32);
            this.pbShieldCount.Name = "pbShieldCount";
            this.pbShieldCount.Size = new System.Drawing.Size(148, 63);
            this.pbShieldCount.TabIndex = 3;
            this.pbShieldCount.TabStop = false;
            // 
            // ShieldInventoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.pbShieldCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblShieldStatus);
            this.Name = "ShieldInventoryControl";
            this.Size = new System.Drawing.Size(277, 146);
            this.Load += new System.EventHandler(this.ShieldInventoryControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbShieldCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblShieldStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbShieldCount;
    }
}
