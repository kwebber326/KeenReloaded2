namespace KeenReloaded2.UserControls.InventoryPanel
{
    partial class ScoreBoard
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
            this.pbScoreBoard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbScoreBoard)).BeginInit();
            this.SuspendLayout();
            // 
            // pbScoreBoard
            // 
            this.pbScoreBoard.Image = global::KeenReloaded2.Properties.Resources.scoreboard_blank1;
            this.pbScoreBoard.Location = new System.Drawing.Point(3, 3);
            this.pbScoreBoard.Name = "pbScoreBoard";
            this.pbScoreBoard.Size = new System.Drawing.Size(164, 64);
            this.pbScoreBoard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbScoreBoard.TabIndex = 0;
            this.pbScoreBoard.TabStop = false;
            // 
            // ScoreBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbScoreBoard);
            this.Name = "ScoreBoard";
            this.Size = new System.Drawing.Size(357, 149);
            this.Load += new System.EventHandler(this.ScoreBoard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbScoreBoard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbScoreBoard;
    }
}
