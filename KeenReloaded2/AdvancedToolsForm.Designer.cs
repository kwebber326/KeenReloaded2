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
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lstMapObjects = new KeenReloaded2.UserControls.AdvancedTools.ScrollableListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(143, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(240, 26);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::KeenReloaded2.Properties.Resources.search_icon;
            this.pictureBox1.Location = new System.Drawing.Point(389, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(31, 26);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // lstMapObjects
            // 
            this.lstMapObjects.FormattingEnabled = true;
            this.lstMapObjects.HorizontalScrollbar = true;
            this.lstMapObjects.ItemHeight = 20;
            this.lstMapObjects.Location = new System.Drawing.Point(17, 54);
            this.lstMapObjects.Name = "lstMapObjects";
            this.lstMapObjects.ScrollAlwaysVisible = true;
            this.lstMapObjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMapObjects.Size = new System.Drawing.Size(714, 404);
            this.lstMapObjects.TabIndex = 4;
            // 
            // AdvancedToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 793);
            this.Controls.Add(this.lstMapObjects);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.label1);
            this.Name = "AdvancedToolsForm";
            this.Text = "AdvancedToolsForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdvancedToolsForm_FormClosing);
            this.Load += new System.EventHandler(this.AdvancedToolsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UserControls.AdvancedTools.ScrollableListBox lstMapObjects;
    }
}