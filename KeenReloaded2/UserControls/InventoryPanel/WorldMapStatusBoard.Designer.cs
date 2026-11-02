namespace KeenReloaded2.UserControls.InventoryPanel
{
    partial class WorldMapStatusBoard
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
            this.pbState = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbState)).BeginInit();
            this.SuspendLayout();
            // 
            // pbState
            // 
            this.pbState.Image = global::KeenReloaded2.Properties.Resources.Keen5BlankStatusBoard;
            this.pbState.Location = new System.Drawing.Point(0, 0);
            this.pbState.Name = "pbState";
            this.pbState.Size = new System.Drawing.Size(785, 530);
            this.pbState.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbState.TabIndex = 0;
            this.pbState.TabStop = false;
            // 
            // WorldMapStatusBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pbState);
            this.Name = "WorldMapStatusBoard";
            this.Size = new System.Drawing.Size(793, 524);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.WorldMapStatusBoard_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbState)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbState;
    }
}
