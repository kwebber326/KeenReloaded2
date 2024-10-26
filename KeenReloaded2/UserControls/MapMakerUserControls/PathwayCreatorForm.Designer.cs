namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class PathwayCreatorForm
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
            this.txtXPosition = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtYPosition = new System.Windows.Forms.TextBox();
            this.btnAddPoint = new System.Windows.Forms.Button();
            this.lstPoints = new System.Windows.Forms.ListBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "X:";
            // 
            // txtXPosition
            // 
            this.txtXPosition.Location = new System.Drawing.Point(52, 13);
            this.txtXPosition.Name = "txtXPosition";
            this.txtXPosition.Size = new System.Drawing.Size(100, 31);
            this.txtXPosition.TabIndex = 1;
            this.txtXPosition.TextChanged += new System.EventHandler(this.TxtXPosition_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y:";
            // 
            // txtYPosition
            // 
            this.txtYPosition.Location = new System.Drawing.Point(197, 13);
            this.txtYPosition.Name = "txtYPosition";
            this.txtYPosition.Size = new System.Drawing.Size(100, 31);
            this.txtYPosition.TabIndex = 3;
            this.txtYPosition.TextChanged += new System.EventHandler(this.TxtYPosition_TextChanged);
            // 
            // btnAddPoint
            // 
            this.btnAddPoint.Location = new System.Drawing.Point(303, 13);
            this.btnAddPoint.Name = "btnAddPoint";
            this.btnAddPoint.Size = new System.Drawing.Size(127, 65);
            this.btnAddPoint.TabIndex = 4;
            this.btnAddPoint.Text = "Add New";
            this.btnAddPoint.UseVisualStyleBackColor = true;
            this.btnAddPoint.Click += new System.EventHandler(this.BtnAddPoint_Click);
            // 
            // lstPoints
            // 
            this.lstPoints.FormattingEnabled = true;
            this.lstPoints.ItemHeight = 25;
            this.lstPoints.Location = new System.Drawing.Point(18, 75);
            this.lstPoints.Name = "lstPoints";
            this.lstPoints.Size = new System.Drawing.Size(279, 404);
            this.lstPoints.TabIndex = 5;
            this.lstPoints.SelectedIndexChanged += new System.EventHandler(this.LstPoints_SelectedIndexChanged);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(308, 296);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(126, 46);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(491, 419);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(108, 60);
            this.btnDone.TabIndex = 7;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.BtnDone_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(309, 348);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(126, 65);
            this.btnDown.TabIndex = 8;
            this.btnDown.Text = "Move Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.BtnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(308, 419);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(127, 60);
            this.btnUp.TabIndex = 9;
            this.btnUp.Text = "Move Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.BtnUp_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(436, 13);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(163, 65);
            this.btnUpdate.TabIndex = 10;
            this.btnUpdate.Text = "Update Selected";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // PathwayCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 493);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lstPoints);
            this.Controls.Add(this.btnAddPoint);
            this.Controls.Add(this.txtYPosition);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtXPosition);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "PathwayCreatorForm";
            this.Text = "PathwayCreatorForm";
            this.Load += new System.EventHandler(this.PathwayCreatorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtXPosition;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtYPosition;
        private System.Windows.Forms.Button btnAddPoint;
        private System.Windows.Forms.ListBox lstPoints;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnUpdate;
    }
}