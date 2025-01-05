namespace KeenReloaded2.UserControls.MainMenuControls
{
    partial class MusicSelectControl
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
            this.cmbSongs = new System.Windows.Forms.ComboBox();
            this.btnRandomSong = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.YellowGreen;
            this.label1.Location = new System.Drawing.Point(-5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "Music Select:";
            // 
            // cmbSongs
            // 
            this.cmbSongs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSongs.FormattingEnabled = true;
            this.cmbSongs.Location = new System.Drawing.Point(0, 32);
            this.cmbSongs.Name = "cmbSongs";
            this.cmbSongs.Size = new System.Drawing.Size(209, 28);
            this.cmbSongs.TabIndex = 2;
            this.cmbSongs.SelectedIndexChanged += new System.EventHandler(this.CmbSongs_SelectedIndexChanged);
            // 
            // btnRandomSong
            // 
            this.btnRandomSong.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnRandomSong.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRandomSong.ForeColor = System.Drawing.Color.Lime;
            this.btnRandomSong.Location = new System.Drawing.Point(0, 75);
            this.btnRandomSong.Name = "btnRandomSong";
            this.btnRandomSong.Size = new System.Drawing.Size(209, 113);
            this.btnRandomSong.TabIndex = 7;
            this.btnRandomSong.Text = "Choose Random Song";
            this.btnRandomSong.UseVisualStyleBackColor = false;
            this.btnRandomSong.Click += new System.EventHandler(this.BtnRandomSong_Click);
            // 
            // MusicSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnRandomSong);
            this.Controls.Add(this.cmbSongs);
            this.Controls.Add(this.label1);
            this.Name = "MusicSelectControl";
            this.Size = new System.Drawing.Size(222, 223);
            this.Load += new System.EventHandler(this.MusicSelectControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSongs;
        private System.Windows.Forms.Button btnRandomSong;
    }
}
