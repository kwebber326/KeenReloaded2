namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class AreaControl
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
            this.txtXLocation = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtYLocation = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "X:";
            // 
            // txtXLocation
            // 
            this.txtXLocation.Location = new System.Drawing.Point(64, 0);
            this.txtXLocation.Name = "txtXLocation";
            this.txtXLocation.Size = new System.Drawing.Size(64, 26);
            this.txtXLocation.TabIndex = 1;
            this.txtXLocation.TextChanged += new System.EventHandler(this.TxtXLocation_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Width:";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(64, 42);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(64, 26);
            this.txtWidth.TabIndex = 5;
            this.txtWidth.TextChanged += new System.EventHandler(this.TxtWidth_TextChanged);
            // 
            // txtYLocation
            // 
            this.txtYLocation.Location = new System.Drawing.Point(200, 0);
            this.txtYLocation.Name = "txtYLocation";
            this.txtYLocation.Size = new System.Drawing.Size(64, 26);
            this.txtYLocation.TabIndex = 6;
            this.txtYLocation.TextChanged += new System.EventHandler(this.TxtYLocation_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Height:";
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(200, 42);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(64, 26);
            this.txtHeight.TabIndex = 8;
            this.txtHeight.TextChanged += new System.EventHandler(this.TxtHeight_TextChanged);
            // 
            // AreaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtYLocation);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtXLocation);
            this.Controls.Add(this.label1);
            this.Name = "AreaControl";
            this.Size = new System.Drawing.Size(278, 77);
            this.Load += new System.EventHandler(this.AreaControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtXLocation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtYLocation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtHeight;
    }
}
