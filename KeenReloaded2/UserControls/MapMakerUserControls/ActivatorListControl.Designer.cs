namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class ActivatorListControl
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
            this.btnEditItems = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEditItems
            // 
            this.btnEditItems.Location = new System.Drawing.Point(3, 3);
            this.btnEditItems.Name = "btnEditItems";
            this.btnEditItems.Size = new System.Drawing.Size(115, 55);
            this.btnEditItems.TabIndex = 0;
            this.btnEditItems.Text = "Edit Items...";
            this.btnEditItems.UseVisualStyleBackColor = true;
            this.btnEditItems.Click += new System.EventHandler(this.BtnEditItems_Click);
            // 
            // ActivatorListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEditItems);
            this.Name = "ActivatorListControl";
            this.Size = new System.Drawing.Size(121, 61);
            this.Load += new System.EventHandler(this.ActivatorListControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEditItems;
    }
}
