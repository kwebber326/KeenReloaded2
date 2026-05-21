namespace KeenReloaded2
{
    partial class WorldMapEditor
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
            this.cmbEpisode = new System.Windows.Forms.ComboBox();
            this.txtMapName = new System.Windows.Forms.TextBox();
            this.pnlSelectionData = new System.Windows.Forms.Panel();
            this.txtPlayerY = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPlayerX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.pnlCanvas = new System.Windows.Forms.Panel();
            this.chkSmartPlacer = new System.Windows.Forms.CheckBox();
            this.mapMakerObjectPropertyListControl1 = new KeenReloaded2.UserControls.MapMakerUserControls.MapMakerObjectPropertyListControl();
            this.mapObjectContainer1 = new KeenReloaded2.UserControls.MapMakerUserControls.MapObjectContainer();
            this.pnlSelectionData.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbEpisode
            // 
            this.cmbEpisode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEpisode.FormattingEnabled = true;
            this.cmbEpisode.Location = new System.Drawing.Point(76, 29);
            this.cmbEpisode.Name = "cmbEpisode";
            this.cmbEpisode.Size = new System.Drawing.Size(187, 21);
            this.cmbEpisode.TabIndex = 0;
            // 
            // txtMapName
            // 
            this.txtMapName.Location = new System.Drawing.Point(76, 3);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.Size = new System.Drawing.Size(187, 20);
            this.txtMapName.TabIndex = 1;
            // 
            // pnlSelectionData
            // 
            this.pnlSelectionData.Controls.Add(this.txtPlayerY);
            this.pnlSelectionData.Controls.Add(this.label6);
            this.pnlSelectionData.Controls.Add(this.label5);
            this.pnlSelectionData.Controls.Add(this.txtPlayerX);
            this.pnlSelectionData.Controls.Add(this.label4);
            this.pnlSelectionData.Controls.Add(this.label3);
            this.pnlSelectionData.Controls.Add(this.cmbCategory);
            this.pnlSelectionData.Controls.Add(this.label2);
            this.pnlSelectionData.Controls.Add(this.label1);
            this.pnlSelectionData.Controls.Add(this.txtMapName);
            this.pnlSelectionData.Controls.Add(this.cmbEpisode);
            this.pnlSelectionData.Location = new System.Drawing.Point(12, 13);
            this.pnlSelectionData.Name = "pnlSelectionData";
            this.pnlSelectionData.Size = new System.Drawing.Size(266, 118);
            this.pnlSelectionData.TabIndex = 2;
            // 
            // txtPlayerY
            // 
            this.txtPlayerY.Location = new System.Drawing.Point(186, 81);
            this.txtPlayerY.Name = "txtPlayerY";
            this.txtPlayerY.Size = new System.Drawing.Size(48, 20);
            this.txtPlayerY.TabIndex = 9;
            this.txtPlayerY.Text = "0";
            this.txtPlayerY.TextChanged += new System.EventHandler(this.txtPlayerY_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(163, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Y:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(85, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "X:";
            // 
            // txtPlayerX
            // 
            this.txtPlayerX.Location = new System.Drawing.Point(108, 81);
            this.txtPlayerX.Name = "txtPlayerX";
            this.txtPlayerX.Size = new System.Drawing.Size(48, 20);
            this.txtPlayerX.TabIndex = 6;
            this.txtPlayerX.Text = "0";
            this.txtPlayerX.TextChanged += new System.EventHandler(this.txtPlayerX_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Player Position:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Category:";
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(76, 56);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(187, 21);
            this.cmbCategory.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Theme:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "World Name:";
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.btnTest);
            this.pnlActions.Controls.Add(this.btnSave);
            this.pnlActions.Controls.Add(this.btnLoad);
            this.pnlActions.Controls.Add(this.btnNew);
            this.pnlActions.Location = new System.Drawing.Point(12, 134);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(266, 118);
            this.pnlActions.TabIndex = 3;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(6, 90);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(96, 23);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "Test World Map";
            this.btnTest.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 61);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(96, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save World Map";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(6, 32);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(96, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load World Map";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(6, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(96, 23);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New World Map";
            this.btnNew.UseVisualStyleBackColor = true;
            // 
            // pnlCanvas
            // 
            this.pnlCanvas.AutoScroll = true;
            this.pnlCanvas.BackColor = System.Drawing.Color.White;
            this.pnlCanvas.Location = new System.Drawing.Point(427, 13);
            this.pnlCanvas.Name = "pnlCanvas";
            this.pnlCanvas.Size = new System.Drawing.Size(845, 724);
            this.pnlCanvas.TabIndex = 4;
            this.pnlCanvas.Click += new System.EventHandler(this.pnlCanvas_Click);
            this.pnlCanvas.MouseEnter += new System.EventHandler(this.pnlCanvas_MouseEnter);
            this.pnlCanvas.MouseLeave += new System.EventHandler(this.pnlCanvas_MouseLeave);
            this.pnlCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlCanvas_MouseMove);
            // 
            // chkSmartPlacer
            // 
            this.chkSmartPlacer.AutoSize = true;
            this.chkSmartPlacer.Location = new System.Drawing.Point(281, 16);
            this.chkSmartPlacer.Name = "chkSmartPlacer";
            this.chkSmartPlacer.Size = new System.Drawing.Size(105, 17);
            this.chkSmartPlacer.TabIndex = 7;
            this.chkSmartPlacer.Text = "Use SmartPlacer";
            this.chkSmartPlacer.UseVisualStyleBackColor = true;
            // 
            // mapMakerObjectPropertyListControl1
            // 
            this.mapMakerObjectPropertyListControl1.AutoScroll = true;
            this.mapMakerObjectPropertyListControl1.Location = new System.Drawing.Point(12, 483);
            this.mapMakerObjectPropertyListControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.mapMakerObjectPropertyListControl1.Name = "mapMakerObjectPropertyListControl1";
            this.mapMakerObjectPropertyListControl1.Size = new System.Drawing.Size(397, 316);
            this.mapMakerObjectPropertyListControl1.TabIndex = 6;
            // 
            // mapObjectContainer1
            // 
            this.mapObjectContainer1.AutoScroll = true;
            this.mapObjectContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mapObjectContainer1.Location = new System.Drawing.Point(11, 257);
            this.mapObjectContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.mapObjectContainer1.Name = "mapObjectContainer1";
            this.mapObjectContainer1.Size = new System.Drawing.Size(411, 222);
            this.mapObjectContainer1.TabIndex = 5;
            // 
            // WorldMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 805);
            this.Controls.Add(this.chkSmartPlacer);
            this.Controls.Add(this.mapMakerObjectPropertyListControl1);
            this.Controls.Add(this.mapObjectContainer1);
            this.Controls.Add(this.pnlCanvas);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlSelectionData);
            this.KeyPreview = true;
            this.Name = "WorldMapEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WorldMapEditor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.WorldMapEditor_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.WorldMapEditor_KeyUp);
            this.pnlSelectionData.ResumeLayout(false);
            this.pnlSelectionData.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbEpisode;
        private System.Windows.Forms.TextBox txtMapName;
        private System.Windows.Forms.Panel pnlSelectionData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Panel pnlCanvas;
        private UserControls.MapMakerUserControls.MapObjectContainer mapObjectContainer1;
        private System.Windows.Forms.TextBox txtPlayerY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPlayerX;
        private System.Windows.Forms.Label label4;
        private UserControls.MapMakerUserControls.MapMakerObjectPropertyListControl mapMakerObjectPropertyListControl1;
        private System.Windows.Forms.CheckBox chkSmartPlacer;
    }
}