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
            this.flagInventoryBoard1 = new KeenReloaded2.UserControls.InventoryPanel.FlagInventoryBoard();
            this.shieldInventoryControl1 = new KeenReloaded2.UserControls.InventoryPanel.ShieldInventoryControl();
            this.lifeDropInventoryControl1 = new KeenReloaded2.UserControls.InventoryPanel.LifeDropInventoryControl();
            this.keyCardInventoryControl1 = new KeenReloaded2.UserControls.InventoryPanel.KeyCardInventoryControl();
            this.keyContainerControl1 = new KeenReloaded2.UserControls.InventoryPanel.KeyContainerControl();
            this.weaponInventoryControl1 = new KeenReloaded2.UserControls.InventoryPanel.WeaponInventoryControl();
            this.scoreBoard1 = new KeenReloaded2.UserControls.InventoryPanel.ScoreBoard();
            this.SuspendLayout();
            // 
            // flagInventoryBoard1
            // 
            this.flagInventoryBoard1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.flagInventoryBoard1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flagInventoryBoard1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.flagInventoryBoard1.Location = new System.Drawing.Point(297, 1042);
            this.flagInventoryBoard1.Name = "flagInventoryBoard1";
            this.flagInventoryBoard1.Size = new System.Drawing.Size(305, 309);
            this.flagInventoryBoard1.TabIndex = 6;
            // 
            // shieldInventoryControl1
            // 
            this.shieldInventoryControl1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.shieldInventoryControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.shieldInventoryControl1.Location = new System.Drawing.Point(325, -2);
            this.shieldInventoryControl1.Name = "shieldInventoryControl1";
            this.shieldInventoryControl1.Shield = null;
            this.shieldInventoryControl1.Size = new System.Drawing.Size(277, 146);
            this.shieldInventoryControl1.TabIndex = 5;
            // 
            // lifeDropInventoryControl1
            // 
            this.lifeDropInventoryControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lifeDropInventoryControl1.Location = new System.Drawing.Point(-4, 1118);
            this.lifeDropInventoryControl1.Name = "lifeDropInventoryControl1";
            this.lifeDropInventoryControl1.Size = new System.Drawing.Size(362, 107);
            this.lifeDropInventoryControl1.TabIndex = 4;
            // 
            // keyCardInventoryControl1
            // 
            this.keyCardInventoryControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.keyCardInventoryControl1.Location = new System.Drawing.Point(2, 1087);
            this.keyCardInventoryControl1.Name = "keyCardInventoryControl1";
            this.keyCardInventoryControl1.Size = new System.Drawing.Size(434, 78);
            this.keyCardInventoryControl1.TabIndex = 3;
            // 
            // keyContainerControl1
            // 
            this.keyContainerControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.keyContainerControl1.Location = new System.Drawing.Point(0, 1042);
            this.keyContainerControl1.Name = "keyContainerControl1";
            this.keyContainerControl1.Size = new System.Drawing.Size(233, 46);
            this.keyContainerControl1.TabIndex = 2;
            // 
            // weaponInventoryControl1
            // 
            this.weaponInventoryControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.weaponInventoryControl1.Location = new System.Drawing.Point(4, 192);
            this.weaponInventoryControl1.Name = "weaponInventoryControl1";
            this.weaponInventoryControl1.Size = new System.Drawing.Size(554, 835);
            this.weaponInventoryControl1.TabIndex = 1;
            // 
            // scoreBoard1
            // 
            this.scoreBoard1.Keen = null;
            this.scoreBoard1.Location = new System.Drawing.Point(4, 4);
            this.scoreBoard1.Name = "scoreBoard1";
            this.scoreBoard1.Size = new System.Drawing.Size(357, 149);
            this.scoreBoard1.TabIndex = 0;
            // 
            // InventoryPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.flagInventoryBoard1);
            this.Controls.Add(this.shieldInventoryControl1);
            this.Controls.Add(this.lifeDropInventoryControl1);
            this.Controls.Add(this.keyCardInventoryControl1);
            this.Controls.Add(this.keyContainerControl1);
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
        private KeyContainerControl keyContainerControl1;
        private KeyCardInventoryControl keyCardInventoryControl1;
        private LifeDropInventoryControl lifeDropInventoryControl1;
        private ShieldInventoryControl shieldInventoryControl1;
        private FlagInventoryBoard flagInventoryBoard1;
    }
}
