namespace KeenReloaded2.UserControls.AdvancedTools.ActionControls
{
    partial class CopyActionControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblOldLocation = new System.Windows.Forms.Label();
            this.actionCommandControl1 = new KeenReloaded2.UserControls.AdvancedTools.ActionCommandControl();
            this.txtX = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "New Location (x, y): (";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(269, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = ",";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(288, 28);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(94, 26);
            this.txtY.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(389, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = ")";
            // 
            // lblOldLocation
            // 
            this.lblOldLocation.AutoSize = true;
            this.lblOldLocation.Location = new System.Drawing.Point(8, 4);
            this.lblOldLocation.Name = "lblOldLocation";
            this.lblOldLocation.Size = new System.Drawing.Size(0, 20);
            this.lblOldLocation.TabIndex = 7;
            // 
            // actionCommandControl1
            // 
            this.actionCommandControl1.CancelCommand = null;
            this.actionCommandControl1.CommitCommand = null;
            this.actionCommandControl1.Location = new System.Drawing.Point(8, 98);
            this.actionCommandControl1.Name = "actionCommandControl1";
            this.actionCommandControl1.PreviewCommand = null;
            this.actionCommandControl1.Size = new System.Drawing.Size(297, 71);
            this.actionCommandControl1.TabIndex = 4;
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(169, 28);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(94, 26);
            this.txtX.TabIndex = 2;
            // 
            // CopyActionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblOldLocation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.actionCommandControl1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.label1);
            this.Name = "CopyActionControl";
            this.Size = new System.Drawing.Size(474, 186);
            this.Load += new System.EventHandler(this.CopyActionControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ActionCommandControl actionCommandControl1;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblOldLocation;
        private System.Windows.Forms.TextBox txtX;
    }
}
