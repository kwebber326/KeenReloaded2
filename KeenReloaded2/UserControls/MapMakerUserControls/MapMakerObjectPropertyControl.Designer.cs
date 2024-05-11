namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class MapMakerObjectPropertyControl
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
            this.lblPropertyName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPropertyName
            // 
            this.lblPropertyName.AutoSize = true;
            this.lblPropertyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPropertyName.Location = new System.Drawing.Point(3, 0);
            this.lblPropertyName.Name = "lblPropertyName";
            this.lblPropertyName.Size = new System.Drawing.Size(123, 25);
            this.lblPropertyName.TabIndex = 0;
            this.lblPropertyName.Text = "Sample Text";
            // 
            // MapMakerObjectPropertyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPropertyName);
            this.Name = "MapMakerObjectPropertyControl";
            this.Size = new System.Drawing.Size(419, 100);
            this.Load += new System.EventHandler(this.MapMakerObjectPropertyControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPropertyName;
    }
}
