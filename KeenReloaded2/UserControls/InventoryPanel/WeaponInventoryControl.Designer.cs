namespace KeenReloaded2.UserControls.InventoryPanel
{
    partial class WeaponInventoryControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblSelectedWeapon = new System.Windows.Forms.Label();
            this.pnlWeapons = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Weapon:";
            // 
            // lblSelectedWeapon
            // 
            this.lblSelectedWeapon.AutoSize = true;
            this.lblSelectedWeapon.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedWeapon.ForeColor = System.Drawing.Color.Black;
            this.lblSelectedWeapon.Location = new System.Drawing.Point(3, 32);
            this.lblSelectedWeapon.Name = "lblSelectedWeapon";
            this.lblSelectedWeapon.Size = new System.Drawing.Size(185, 32);
            this.lblSelectedWeapon.TabIndex = 1;
            this.lblSelectedWeapon.Text = "Sample Text";
            // 
            // pnlWeapons
            // 
            this.pnlWeapons.Location = new System.Drawing.Point(3, 67);
            this.pnlWeapons.Name = "pnlWeapons";
            this.pnlWeapons.Size = new System.Drawing.Size(548, 765);
            this.pnlWeapons.TabIndex = 2;
            // 
            // WeaponInventoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.pnlWeapons);
            this.Controls.Add(this.lblSelectedWeapon);
            this.Controls.Add(this.label1);
            this.Name = "WeaponInventoryControl";
            this.Size = new System.Drawing.Size(554, 835);
            this.Load += new System.EventHandler(this.WeaponInventoryControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSelectedWeapon;
        private System.Windows.Forms.Panel pnlWeapons;
    }
}
