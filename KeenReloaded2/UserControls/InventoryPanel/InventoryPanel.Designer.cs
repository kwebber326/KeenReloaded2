namespace KeenReloaded2.UserControls.InventoryPanel
{
    partial class InventoryPanel
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
            this.scoreBoard1 = new KeenReloaded2.UserControls.InventoryPanel.ScoreBoard();
            this.weaponInventoryControl1 = new KeenReloaded2.UserControls.InventoryPanel.WeaponInventoryControl();
            this.SuspendLayout();
            // 
            // scoreBoard1
            // 
            this.scoreBoard1.Keen = null;
            this.scoreBoard1.Location = new System.Drawing.Point(4, 4);
            this.scoreBoard1.Name = "scoreBoard1";
            this.scoreBoard1.Size = new System.Drawing.Size(357, 149);
            this.scoreBoard1.TabIndex = 0;
            // 
            // weaponInventoryControl1
            // 
            this.weaponInventoryControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.weaponInventoryControl1.Location = new System.Drawing.Point(4, 192);
            this.weaponInventoryControl1.Name = "weaponInventoryControl1";
            this.weaponInventoryControl1.Size = new System.Drawing.Size(554, 835);
            this.weaponInventoryControl1.TabIndex = 1;
            // 
            // InventoryPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.weaponInventoryControl1);
            this.Controls.Add(this.scoreBoard1);
            this.Name = "InventoryPanel";
            this.Size = new System.Drawing.Size(605, 1301);
            this.Load += new System.EventHandler(this.InventoryPanel_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ScoreBoard scoreBoard1;
        private WeaponInventoryControl weaponInventoryControl1;
    }
}
