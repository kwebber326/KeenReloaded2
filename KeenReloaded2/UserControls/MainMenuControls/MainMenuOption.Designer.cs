namespace KeenReloaded2.UserControls
{
    partial class MainMenuOption
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
            this.lblOptionText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblOptionText
            // 
            this.lblOptionText.AutoSize = true;
            this.lblOptionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOptionText.Location = new System.Drawing.Point(3, 0);
            this.lblOptionText.Name = "lblOptionText";
            this.lblOptionText.Size = new System.Drawing.Size(185, 32);
            this.lblOptionText.TabIndex = 0;
            this.lblOptionText.Text = "Sample Text";
            this.lblOptionText.Click += new System.EventHandler(this.LblOptionText_Click);
            this.lblOptionText.MouseEnter += new System.EventHandler(this.LblOptionText_MouseEnter);
            this.lblOptionText.MouseLeave += new System.EventHandler(this.LblOptionText_MouseLeave);
            // 
            // MainMenuOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblOptionText);
            this.ForeColor = System.Drawing.Color.YellowGreen;
            this.Name = "MainMenuOption";
            this.Size = new System.Drawing.Size(530, 64);
            this.Load += new System.EventHandler(this.MainMenuOption_Load);
            this.Click += new System.EventHandler(this.MainMenuOption_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOptionText;
    }
}
