namespace KeenReloaded2.UserControls.InventoryPanel
{
    partial class WeaponDisplayControl
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
            this.pbWeaponNumber = new System.Windows.Forms.PictureBox();
            this.pbWeapon = new System.Windows.Forms.PictureBox();
            this.pbAmmoAmount = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbWeaponNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbWeapon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAmmoAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // pbWeaponNumber
            // 
            this.pbWeaponNumber.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pbWeaponNumber.Location = new System.Drawing.Point(3, 4);
            this.pbWeaponNumber.Name = "pbWeaponNumber";
            this.pbWeaponNumber.Size = new System.Drawing.Size(77, 63);
            this.pbWeaponNumber.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbWeaponNumber.TabIndex = 0;
            this.pbWeaponNumber.TabStop = false;
            this.pbWeaponNumber.Click += new System.EventHandler(this.PbWeaponNumber_Click);
            // 
            // pbWeapon
            // 
            this.pbWeapon.Location = new System.Drawing.Point(86, 4);
            this.pbWeapon.Name = "pbWeapon";
            this.pbWeapon.Size = new System.Drawing.Size(143, 50);
            this.pbWeapon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbWeapon.TabIndex = 1;
            this.pbWeapon.TabStop = false;
            // 
            // pbAmmoAmount
            // 
            this.pbAmmoAmount.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pbAmmoAmount.Location = new System.Drawing.Point(272, 4);
            this.pbAmmoAmount.Name = "pbAmmoAmount";
            this.pbAmmoAmount.Size = new System.Drawing.Size(110, 50);
            this.pbAmmoAmount.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbAmmoAmount.TabIndex = 3;
            this.pbAmmoAmount.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(235, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 29);
            this.label1.TabIndex = 2;
            this.label1.Text = "X";
            // 
            // WeaponDisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.pbAmmoAmount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbWeapon);
            this.Controls.Add(this.pbWeaponNumber);
            this.Name = "WeaponDisplayControl";
            this.Size = new System.Drawing.Size(399, 80);
            this.Load += new System.EventHandler(this.WeaponDisplayControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbWeaponNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbWeapon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAmmoAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbWeaponNumber;
        private System.Windows.Forms.PictureBox pbWeapon;
        private System.Windows.Forms.PictureBox pbAmmoAmount;
        private System.Windows.Forms.Label label1;
    }
}
