namespace KeenReloaded2
{
    partial class LevelObjectivesForm
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
            this.cmbLevelNames = new System.Windows.Forms.ComboBox();
            this.pnlLevelControls = new System.Windows.Forms.Panel();
            this.pnlManageObjective = new System.Windows.Forms.Panel();
            this.btnAddToItems = new System.Windows.Forms.Button();
            this.btnActivation = new System.Windows.Forms.Button();
            this.btnAddObjective = new System.Windows.Forms.Button();
            this.pbObjectiveImage = new System.Windows.Forms.PictureBox();
            this.cmbObjectives = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlLevelControls.SuspendLayout();
            this.pnlManageObjective.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbObjectiveImage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose Level:";
            // 
            // cmbLevelNames
            // 
            this.cmbLevelNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLevelNames.FormattingEnabled = true;
            this.cmbLevelNames.Location = new System.Drawing.Point(17, 36);
            this.cmbLevelNames.Name = "cmbLevelNames";
            this.cmbLevelNames.Size = new System.Drawing.Size(184, 21);
            this.cmbLevelNames.TabIndex = 1;
            this.cmbLevelNames.SelectedIndexChanged += new System.EventHandler(this.cmbLevelNames_SelectedIndexChanged);
            // 
            // pnlLevelControls
            // 
            this.pnlLevelControls.Controls.Add(this.pnlManageObjective);
            this.pnlLevelControls.Controls.Add(this.pbObjectiveImage);
            this.pnlLevelControls.Controls.Add(this.cmbObjectives);
            this.pnlLevelControls.Controls.Add(this.label3);
            this.pnlLevelControls.Location = new System.Drawing.Point(17, 63);
            this.pnlLevelControls.Name = "pnlLevelControls";
            this.pnlLevelControls.Size = new System.Drawing.Size(771, 375);
            this.pnlLevelControls.TabIndex = 2;
            // 
            // pnlManageObjective
            // 
            this.pnlManageObjective.Controls.Add(this.btnAddToItems);
            this.pnlManageObjective.Controls.Add(this.btnActivation);
            this.pnlManageObjective.Controls.Add(this.btnAddObjective);
            this.pnlManageObjective.Location = new System.Drawing.Point(3, 39);
            this.pnlManageObjective.Name = "pnlManageObjective";
            this.pnlManageObjective.Size = new System.Drawing.Size(200, 157);
            this.pnlManageObjective.TabIndex = 6;
            this.pnlManageObjective.Visible = false;
            // 
            // btnAddToItems
            // 
            this.btnAddToItems.Location = new System.Drawing.Point(4, 101);
            this.btnAddToItems.Name = "btnAddToItems";
            this.btnAddToItems.Size = new System.Drawing.Size(141, 43);
            this.btnAddToItems.TabIndex = 6;
            this.btnAddToItems.Text = "Add Objective to Game Items";
            this.btnAddToItems.UseVisualStyleBackColor = true;
            this.btnAddToItems.Click += new System.EventHandler(this.btnAddToItems_Click);
            // 
            // btnActivation
            // 
            this.btnActivation.Location = new System.Drawing.Point(3, 3);
            this.btnActivation.Name = "btnActivation";
            this.btnActivation.Size = new System.Drawing.Size(141, 43);
            this.btnActivation.TabIndex = 4;
            this.btnActivation.Text = "Manage Activation Components";
            this.btnActivation.UseVisualStyleBackColor = true;
            this.btnActivation.Click += new System.EventHandler(this.btnActivation_Click);
            // 
            // btnAddObjective
            // 
            this.btnAddObjective.Location = new System.Drawing.Point(4, 52);
            this.btnAddObjective.Name = "btnAddObjective";
            this.btnAddObjective.Size = new System.Drawing.Size(141, 43);
            this.btnAddObjective.TabIndex = 5;
            this.btnAddObjective.Text = "Add Objective to Game Completion Ruleset";
            this.btnAddObjective.UseVisualStyleBackColor = true;
            this.btnAddObjective.Click += new System.EventHandler(this.btnAddObjective_Click);
            // 
            // pbObjectiveImage
            // 
            this.pbObjectiveImage.Location = new System.Drawing.Point(618, 12);
            this.pbObjectiveImage.Name = "pbObjectiveImage";
            this.pbObjectiveImage.Size = new System.Drawing.Size(132, 113);
            this.pbObjectiveImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbObjectiveImage.TabIndex = 3;
            this.pbObjectiveImage.TabStop = false;
            // 
            // cmbObjectives
            // 
            this.cmbObjectives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbObjectives.FormattingEnabled = true;
            this.cmbObjectives.Location = new System.Drawing.Point(201, 12);
            this.cmbObjectives.Name = "cmbObjectives";
            this.cmbObjectives.Size = new System.Drawing.Size(411, 21);
            this.cmbObjectives.TabIndex = 2;
            this.cmbObjectives.SelectedIndexChanged += new System.EventHandler(this.cmbObjectives_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(201, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Choose Level Objective:";
            // 
            // LevelObjectivesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnlLevelControls);
            this.Controls.Add(this.cmbLevelNames);
            this.Controls.Add(this.label1);
            this.Name = "LevelObjectivesForm";
            this.Text = "Level Objectives";
            this.Load += new System.EventHandler(this.LevelObjectivesForm_Load);
            this.pnlLevelControls.ResumeLayout(false);
            this.pnlLevelControls.PerformLayout();
            this.pnlManageObjective.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbObjectiveImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbLevelNames;
        private System.Windows.Forms.Panel pnlLevelControls;
        private System.Windows.Forms.PictureBox pbObjectiveImage;
        private System.Windows.Forms.ComboBox cmbObjectives;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlManageObjective;
        private System.Windows.Forms.Button btnActivation;
        private System.Windows.Forms.Button btnAddObjective;
        private System.Windows.Forms.Button btnAddToItems;
    }
}