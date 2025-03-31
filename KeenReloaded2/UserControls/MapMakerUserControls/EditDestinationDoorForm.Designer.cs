namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class EditDestinationDoorForm
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
            this.lstDoors = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstDoors
            // 
            this.lstDoors.FormattingEnabled = true;
            this.lstDoors.ItemHeight = 20;
            this.lstDoors.Location = new System.Drawing.Point(12, 51);
            this.lstDoors.Name = "lstDoors";
            this.lstDoors.Size = new System.Drawing.Size(250, 264);
            this.lstDoors.TabIndex = 0;
            this.lstDoors.SelectedIndexChanged += new System.EventHandler(this.LstDoors_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Available Doors:";
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(142, 321);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(120, 48);
            this.btnDone.TabIndex = 2;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.BtnDone_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(12, 321);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(124, 46);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear Selection";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // EditDestinationDoorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 379);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstDoors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "EditDestinationDoorForm";
            this.Text = "Select Door Destination";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditDestinationDoorForm_FormClosing);
            this.Load += new System.EventHandler(this.EditDestinationDoorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstDoors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnClear;
    }
}