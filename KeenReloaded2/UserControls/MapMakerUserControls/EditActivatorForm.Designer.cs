namespace KeenReloaded2.UserControls.MapMakerUserControls
{
    partial class EditActivatorForm
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
            this.lstCurrent = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstRemaining = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddSelection = new System.Windows.Forms.Button();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstCurrent
            // 
            this.lstCurrent.FormattingEnabled = true;
            this.lstCurrent.ItemHeight = 20;
            this.lstCurrent.Location = new System.Drawing.Point(12, 65);
            this.lstCurrent.Name = "lstCurrent";
            this.lstCurrent.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstCurrent.Size = new System.Drawing.Size(254, 364);
            this.lstCurrent.TabIndex = 0;
            this.lstCurrent.SelectedIndexChanged += new System.EventHandler(this.LstCurrent_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current Items:";
            // 
            // lstRemaining
            // 
            this.lstRemaining.FormattingEnabled = true;
            this.lstRemaining.ItemHeight = 20;
            this.lstRemaining.Location = new System.Drawing.Point(388, 65);
            this.lstRemaining.Name = "lstRemaining";
            this.lstRemaining.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstRemaining.Size = new System.Drawing.Size(254, 364);
            this.lstRemaining.TabIndex = 2;
            this.lstRemaining.SelectedIndexChanged += new System.EventHandler(this.LstRemaining_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(383, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Remaining Items:";
            // 
            // btnAddSelection
            // 
            this.btnAddSelection.Location = new System.Drawing.Point(289, 110);
            this.btnAddSelection.Name = "btnAddSelection";
            this.btnAddSelection.Size = new System.Drawing.Size(75, 40);
            this.btnAddSelection.TabIndex = 4;
            this.btnAddSelection.Text = "<<";
            this.btnAddSelection.UseVisualStyleBackColor = true;
            this.btnAddSelection.Click += new System.EventHandler(this.BtnAddSelection_Click);
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Location = new System.Drawing.Point(289, 277);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(75, 40);
            this.btnRemoveSelected.TabIndex = 5;
            this.btnRemoveSelected.Text = ">>";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.BtnRemoveSelected_Click);
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(567, 435);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 35);
            this.btnDone.TabIndex = 6;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.BtnDone_Click);
            // 
            // EditActivatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 509);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnRemoveSelected);
            this.Controls.Add(this.btnAddSelection);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstRemaining);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstCurrent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditActivatorForm";
            this.Text = "Select Activation Items";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditActivatorForm_FormClosing);
            this.Load += new System.EventHandler(this.EditActivatorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstCurrent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstRemaining;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddSelection;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnDone;
    }
}