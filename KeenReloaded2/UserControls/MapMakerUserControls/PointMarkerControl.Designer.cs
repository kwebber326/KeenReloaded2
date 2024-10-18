namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class PointMarkerControl
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
            this.lblOrdinalPosition = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblOrdinalPosition
            // 
            this.lblOrdinalPosition.AutoSize = true;
            this.lblOrdinalPosition.Location = new System.Drawing.Point(3, 0);
            this.lblOrdinalPosition.Name = "lblOrdinalPosition";
            this.lblOrdinalPosition.Size = new System.Drawing.Size(24, 25);
            this.lblOrdinalPosition.TabIndex = 0;
            this.lblOrdinalPosition.Text = "0";
            // 
            // PointMarkerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblOrdinalPosition);
            this.Name = "PointMarkerControl";
            this.Size = new System.Drawing.Size(62, 58);
            this.Load += new System.EventHandler(this.PointMarkerControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblOrdinalPosition;
    }
}
