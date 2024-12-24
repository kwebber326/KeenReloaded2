namespace KeenReloaded2.UserControls.AdvancedTools
{
    partial class ExtendActionControl
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
            this.cmbDirection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLengths = new System.Windows.Forms.TextBox();
            this.actionCommandControl1 = new KeenReloaded2.UserControls.AdvancedTools.ActionCommandControl();
            this.SuspendLayout();
            // 
            // cmbDirection
            // 
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Location = new System.Drawing.Point(3, 27);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(137, 28);
            this.cmbDirection.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Direction:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(178, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Lengths:";
            // 
            // txtLengths
            // 
            this.txtLengths.Location = new System.Drawing.Point(182, 29);
            this.txtLengths.Name = "txtLengths";
            this.txtLengths.Size = new System.Drawing.Size(100, 26);
            this.txtLengths.TabIndex = 3;
            // 
            // actionCommandControl1
            // 
            this.actionCommandControl1.CancelCommand = null;
            this.actionCommandControl1.CommitCommand = null;
            this.actionCommandControl1.Location = new System.Drawing.Point(8, 73);
            this.actionCommandControl1.Name = "actionCommandControl1";
            this.actionCommandControl1.PreviewCommand = null;
            this.actionCommandControl1.Size = new System.Drawing.Size(244, 71);
            this.actionCommandControl1.TabIndex = 4;
            // 
            // ExtendActionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.actionCommandControl1);
            this.Controls.Add(this.txtLengths);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbDirection);
            this.Name = "ExtendActionControl";
            this.Size = new System.Drawing.Size(295, 147);
            this.Load += new System.EventHandler(this.ExtendActionControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbDirection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLengths;
        private ActionCommandControl actionCommandControl1;
    }
}
