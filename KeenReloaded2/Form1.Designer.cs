using System.Windows.Forms;

namespace KeenReloaded2
{
    partial class Form1
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
            this.pnlGameWindow = new System.Windows.Forms.Panel();
            this.pbBackgroundImage = new System.Windows.Forms.PictureBox();
            this.pbGameImage = new System.Windows.Forms.PictureBox();
            this.lblStopwatch = new System.Windows.Forms.Label();
            this.soundPlayer1 = new KeenReloaded2.UserControls.MusicAndSound.SoundPlayer();
            this.inventoryPanel1 = new KeenReloaded2.UserControls.InventoryPanel.InventoryPanel();
            this.pnlGameWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackgroundImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGameImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlGameWindow
            // 
            this.pnlGameWindow.BackColor = System.Drawing.Color.White;
            this.pnlGameWindow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlGameWindow.Controls.Add(this.pbBackgroundImage);
            this.pnlGameWindow.Controls.Add(this.pbGameImage);
            this.pnlGameWindow.Location = new System.Drawing.Point(409, 1);
            this.pnlGameWindow.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlGameWindow.Name = "pnlGameWindow";
            this.pnlGameWindow.Size = new System.Drawing.Size(935, 781);
            this.pnlGameWindow.TabIndex = 2;
            // 
            // pbBackgroundImage
            // 
            this.pbBackgroundImage.BackColor = System.Drawing.Color.Transparent;
            this.pbBackgroundImage.Location = new System.Drawing.Point(3, 3);
            this.pbBackgroundImage.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pbBackgroundImage.Name = "pbBackgroundImage";
            this.pbBackgroundImage.Size = new System.Drawing.Size(100, 50);
            this.pbBackgroundImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbBackgroundImage.TabIndex = 1;
            this.pbBackgroundImage.TabStop = false;
            // 
            // pbGameImage
            // 
            this.pbGameImage.BackColor = System.Drawing.Color.Transparent;
            this.pbGameImage.Location = new System.Drawing.Point(3, 3);
            this.pbGameImage.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pbGameImage.Name = "pbGameImage";
            this.pbGameImage.Size = new System.Drawing.Size(100, 50);
            this.pbGameImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbGameImage.TabIndex = 0;
            this.pbGameImage.TabStop = false;
            // 
            // lblStopwatch
            // 
            this.lblStopwatch.AutoSize = true;
            this.lblStopwatch.BackColor = System.Drawing.Color.Transparent;
            this.lblStopwatch.Font = new System.Drawing.Font("Verdana", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStopwatch.ForeColor = System.Drawing.Color.YellowGreen;
            this.lblStopwatch.Location = new System.Drawing.Point(409, 790);
            this.lblStopwatch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStopwatch.MaximumSize = new System.Drawing.Size(933, 0);
            this.lblStopwatch.Name = "lblStopwatch";
            this.lblStopwatch.Size = new System.Drawing.Size(240, 32);
            this.lblStopwatch.TabIndex = 4;
            this.lblStopwatch.Text = "Keen Reloaded";
            // 
            // soundPlayer1
            // 
            this.soundPlayer1.Keen = null;
            this.soundPlayer1.Location = new System.Drawing.Point(244, 132);
            this.soundPlayer1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.soundPlayer1.Name = "soundPlayer1";
            this.soundPlayer1.Size = new System.Drawing.Size(100, 98);
            this.soundPlayer1.TabIndex = 3;
            this.soundPlayer1.Visible = false;
            // 
            // inventoryPanel1
            // 
            this.inventoryPanel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.inventoryPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.inventoryPanel1.Keen = null;
            this.inventoryPanel1.Location = new System.Drawing.Point(2, 1);
            this.inventoryPanel1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.inventoryPanel1.Name = "inventoryPanel1";
            this.inventoryPanel1.ShowFlagInventory = true;
            this.inventoryPanel1.Size = new System.Drawing.Size(405, 958);
            this.inventoryPanel1.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.ClientSize = new System.Drawing.Size(1069, 573);
            this.ControlBox = false;
            this.Controls.Add(this.lblStopwatch);
            this.Controls.Add(this.soundPlayer1);
            this.Controls.Add(this.pnlGameWindow);
            this.Controls.Add(this.inventoryPanel1);
            this.Cursor = System.Windows.Forms.Cursors.No;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Keen Reloaded";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.pnlGameWindow.ResumeLayout(false);
            this.pnlGameWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackgroundImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGameImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private UserControls.InventoryPanel.InventoryPanel inventoryPanel1;
        private Panel pnlGameWindow;
        private PictureBox pbGameImage;
        private UserControls.MusicAndSound.SoundPlayer soundPlayer1;
        private PictureBox pbBackgroundImage;
        private Label lblStopwatch;
    }
}

