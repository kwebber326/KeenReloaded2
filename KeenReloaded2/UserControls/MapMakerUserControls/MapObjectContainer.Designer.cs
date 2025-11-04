namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class MapObjectContainer
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
            this.lblHelpText = new System.Windows.Forms.Label();
            this.pnlImages = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblHelpText
            // 
            this.lblHelpText.AutoSize = true;
            this.lblHelpText.Location = new System.Drawing.Point(2, 0);
            this.lblHelpText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHelpText.Name = "lblHelpText";
            this.lblHelpText.Size = new System.Drawing.Size(216, 13);
            this.lblHelpText.TabIndex = 0;
            this.lblHelpText.Text = "Press the \"Escape\" button to clear selection";
            this.lblHelpText.Visible = false;
            // 
            // pnlImages
            // 
            this.pnlImages.AutoSize = true;
            this.pnlImages.Location = new System.Drawing.Point(3, 16);
            this.pnlImages.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlImages.Name = "pnlImages";
            this.pnlImages.Size = new System.Drawing.Size(407, 400);
            this.pnlImages.TabIndex = 1;
            // 
            // MapObjectContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.pnlImages);
            this.Controls.Add(this.lblHelpText);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MapObjectContainer";
            this.Size = new System.Drawing.Size(411, 418);
            this.Load += new System.EventHandler(this.MapObjectContainer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHelpText;
        private System.Windows.Forms.Panel pnlImages;
    }
}
