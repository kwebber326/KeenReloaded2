namespace KeenReloaded2
{
    partial class AdvancedToolsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lstMapObjects = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Objects:";
            // 
            // lstMapObjects
            // 
            this.lstMapObjects.FormattingEnabled = true;
            this.lstMapObjects.ItemHeight = 20;
            this.lstMapObjects.Location = new System.Drawing.Point(17, 49);
            this.lstMapObjects.Name = "lstMapObjects";
            this.lstMapObjects.ScrollAlwaysVisible = true;
            this.lstMapObjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMapObjects.Size = new System.Drawing.Size(397, 424);
            this.lstMapObjects.TabIndex = 1;
            // 
            // AdvancedToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 793);
            this.Controls.Add(this.lstMapObjects);
            this.Controls.Add(this.label1);
            this.Name = "AdvancedToolsForm";
            this.Text = "AdvancedToolsForm";
            this.Load += new System.EventHandler(this.AdvancedToolsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstMapObjects;
    }
}