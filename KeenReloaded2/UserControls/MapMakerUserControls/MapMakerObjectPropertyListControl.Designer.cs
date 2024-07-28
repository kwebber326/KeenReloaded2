namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class MapMakerObjectPropertyListControl
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.pbObjectImage = new System.Windows.Forms.PictureBox();
            this.btnPlace = new System.Windows.Forms.Button();
            this.pnlControls = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbObjectImage)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(109, 3);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(149, 29);
            this.lblHeader.TabIndex = 1;
            this.lblHeader.Text = "Sample Text";
            // 
            // pbObjectImage
            // 
            this.pbObjectImage.Location = new System.Drawing.Point(3, 3);
            this.pbObjectImage.Name = "pbObjectImage";
            this.pbObjectImage.Size = new System.Drawing.Size(100, 84);
            this.pbObjectImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbObjectImage.TabIndex = 0;
            this.pbObjectImage.TabStop = false;
            // 
            // btnPlace
            // 
            this.btnPlace.Location = new System.Drawing.Point(109, 35);
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.Size = new System.Drawing.Size(149, 33);
            this.btnPlace.TabIndex = 2;
            this.btnPlace.Text = "Update Object";
            this.btnPlace.UseVisualStyleBackColor = true;
            this.btnPlace.Visible = false;
            this.btnPlace.Click += new System.EventHandler(this.BtnPlace_Click);
            // 
            // pnlControls
            // 
            this.pnlControls.AutoScroll = true;
            this.pnlControls.Location = new System.Drawing.Point(3, 93);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(589, 390);
            this.pnlControls.TabIndex = 3;
            // 
            // MapMakerObjectPropertyListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.btnPlace);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pbObjectImage);
            this.Name = "MapMakerObjectPropertyListControl";
            this.Size = new System.Drawing.Size(595, 486);
            this.Load += new System.EventHandler(this.MapMakerObjectPropertyListControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbObjectImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbObjectImage;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Button btnPlace;
        private System.Windows.Forms.Panel pnlControls;
    }
}
