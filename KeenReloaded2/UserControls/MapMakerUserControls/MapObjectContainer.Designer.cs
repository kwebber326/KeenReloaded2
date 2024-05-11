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
            this.lblHelpText.Location = new System.Drawing.Point(3, 0);
            this.lblHelpText.Name = "lblHelpText";
            this.lblHelpText.Size = new System.Drawing.Size(319, 20);
            this.lblHelpText.TabIndex = 0;
            this.lblHelpText.Text = "Press the \"Escape\" button to clear selection";
            this.lblHelpText.Visible = false;
            // 
            // pnlImages
            // 
            this.pnlImages.Location = new System.Drawing.Point(4, 24);
            this.pnlImages.Name = "pnlImages";
            this.pnlImages.Size = new System.Drawing.Size(610, 616);
            this.pnlImages.TabIndex = 1;
            // 
            // MapObjectContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.pnlImages);
            this.Controls.Add(this.lblHelpText);
            this.Name = "MapObjectContainer";
            this.Size = new System.Drawing.Size(617, 643);
            this.Load += new System.EventHandler(this.MapObjectContainer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHelpText;
        private System.Windows.Forms.Panel pnlImages;
    }
}
