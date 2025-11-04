namespace KeenReloaded2
{
    partial class MapMaker
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
            this.txtMapName = new System.Windows.Forms.TextBox();
            this.pnlMapCanvas = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbGameMode = new System.Windows.Forms.ComboBox();
            this.cmbEpisode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbWidth = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbHeight = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.btnDefaultDimensions = new System.Windows.Forms.Button();
            this.cmbBiome = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.dialogMapLoader = new System.Windows.Forms.OpenFileDialog();
            this.btnTest = new System.Windows.Forms.Button();
            this.chkUseSmartPlacer = new System.Windows.Forms.CheckBox();
            this.btnNewMap = new System.Windows.Forms.Button();
            this.mapMakerObjectPropertyListControl1 = new KeenReloaded2.UserControls.MapMakerUserControls.MapMakerObjectPropertyListControl();
            this.mapObjectContainer1 = new KeenReloaded2.UserControls.MapMakerUserControls.MapObjectContainer();
            this.btnAdvancedTools = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Map Name:";
            // 
            // txtMapName
            // 
            this.txtMapName.Location = new System.Drawing.Point(109, 7);
            this.txtMapName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.Size = new System.Drawing.Size(160, 20);
            this.txtMapName.TabIndex = 1;
            this.txtMapName.TextChanged += new System.EventHandler(this.TxtMapName_TextChanged);
            // 
            // pnlMapCanvas
            // 
            this.pnlMapCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMapCanvas.AutoScroll = true;
            this.pnlMapCanvas.BackColor = System.Drawing.Color.White;
            this.pnlMapCanvas.Location = new System.Drawing.Point(417, 8);
            this.pnlMapCanvas.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlMapCanvas.Name = "pnlMapCanvas";
            this.pnlMapCanvas.Size = new System.Drawing.Size(641, 800);
            this.pnlMapCanvas.TabIndex = 2;
            this.pnlMapCanvas.Click += new System.EventHandler(this.PnlMapCanvas_Click);
            this.pnlMapCanvas.MouseEnter += new System.EventHandler(this.PnlMapCanvas_MouseEnter);
            this.pnlMapCanvas.MouseLeave += new System.EventHandler(this.PnlMapCanvas_MouseLeave);
            this.pnlMapCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PnlMapCanvas_MouseMove);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 27);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Game Mode:";
            // 
            // cmbGameMode
            // 
            this.cmbGameMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGameMode.FormattingEnabled = true;
            this.cmbGameMode.Location = new System.Drawing.Point(109, 28);
            this.cmbGameMode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbGameMode.Name = "cmbGameMode";
            this.cmbGameMode.Size = new System.Drawing.Size(160, 21);
            this.cmbGameMode.TabIndex = 5;
            // 
            // cmbEpisode
            // 
            this.cmbEpisode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEpisode.FormattingEnabled = true;
            this.cmbEpisode.Location = new System.Drawing.Point(109, 50);
            this.cmbEpisode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbEpisode.Name = "cmbEpisode";
            this.cmbEpisode.Size = new System.Drawing.Size(160, 21);
            this.cmbEpisode.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 49);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Episode:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(302, 5);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Dimensions:";
            // 
            // cmbWidth
            // 
            this.cmbWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWidth.FormattingEnabled = true;
            this.cmbWidth.Location = new System.Drawing.Point(280, 25);
            this.cmbWidth.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbWidth.Name = "cmbWidth";
            this.cmbWidth.Size = new System.Drawing.Size(52, 21);
            this.cmbWidth.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(335, 27);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "x";
            // 
            // cmbHeight
            // 
            this.cmbHeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHeight.FormattingEnabled = true;
            this.cmbHeight.Location = new System.Drawing.Point(349, 25);
            this.cmbHeight.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbHeight.Name = "cmbHeight";
            this.cmbHeight.Size = new System.Drawing.Size(52, 21);
            this.cmbHeight.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 94);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 17);
            this.label6.TabIndex = 7;
            this.label6.Text = "Category:";
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(109, 94);
            this.cmbCategory.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(160, 21);
            this.cmbCategory.TabIndex = 12;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.CmbCategory_SelectedIndexChanged);
            // 
            // btnDefaultDimensions
            // 
            this.btnDefaultDimensions.Location = new System.Drawing.Point(305, 47);
            this.btnDefaultDimensions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDefaultDimensions.Name = "btnDefaultDimensions";
            this.btnDefaultDimensions.Size = new System.Drawing.Size(94, 20);
            this.btnDefaultDimensions.TabIndex = 13;
            this.btnDefaultDimensions.Text = "Reset to Default";
            this.btnDefaultDimensions.UseVisualStyleBackColor = true;
            this.btnDefaultDimensions.Click += new System.EventHandler(this.BtnDefaultDimensions_Click);
            // 
            // cmbBiome
            // 
            this.cmbBiome.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBiome.FormattingEnabled = true;
            this.cmbBiome.Location = new System.Drawing.Point(109, 72);
            this.cmbBiome.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbBiome.Name = "cmbBiome";
            this.cmbBiome.Size = new System.Drawing.Size(160, 21);
            this.cmbBiome.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(8, 72);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "Biome:";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(305, 72);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(94, 23);
            this.btnLoad.TabIndex = 18;
            this.btnLoad.Text = "Load Map";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(305, 94);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 21);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Save Map";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // dialogMapLoader
            // 
            this.dialogMapLoader.FileName = "openFileDialog1";
            this.dialogMapLoader.FileOk += new System.ComponentModel.CancelEventHandler(this.DialogMapLoader_FileOk);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(305, 125);
            this.btnTest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(95, 21);
            this.btnTest.TabIndex = 20;
            this.btnTest.Text = "Test Map";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // chkUseSmartPlacer
            // 
            this.chkUseSmartPlacer.AutoSize = true;
            this.chkUseSmartPlacer.Checked = true;
            this.chkUseSmartPlacer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseSmartPlacer.Location = new System.Drawing.Point(11, 128);
            this.chkUseSmartPlacer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkUseSmartPlacer.Name = "chkUseSmartPlacer";
            this.chkUseSmartPlacer.Size = new System.Drawing.Size(108, 17);
            this.chkUseSmartPlacer.TabIndex = 21;
            this.chkUseSmartPlacer.Text = "Use Smart Placer";
            this.chkUseSmartPlacer.UseVisualStyleBackColor = true;
            this.chkUseSmartPlacer.CheckedChanged += new System.EventHandler(this.ChkUseSmartPlacer_CheckedChanged);
            // 
            // btnNewMap
            // 
            this.btnNewMap.Location = new System.Drawing.Point(305, 150);
            this.btnNewMap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnNewMap.Name = "btnNewMap";
            this.btnNewMap.Size = new System.Drawing.Size(95, 21);
            this.btnNewMap.TabIndex = 21;
            this.btnNewMap.Text = "New Map";
            this.btnNewMap.UseVisualStyleBackColor = true;
            this.btnNewMap.Click += new System.EventHandler(this.BtnNewMap_Click);
            // 
            // mapMakerObjectPropertyListControl1
            // 
            this.mapMakerObjectPropertyListControl1.AutoScroll = true;
            this.mapMakerObjectPropertyListControl1.Location = new System.Drawing.Point(3, 617);
            this.mapMakerObjectPropertyListControl1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.mapMakerObjectPropertyListControl1.Name = "mapMakerObjectPropertyListControl1";
            this.mapMakerObjectPropertyListControl1.Size = new System.Drawing.Size(397, 316);
            this.mapMakerObjectPropertyListControl1.TabIndex = 17;
            // 
            // mapObjectContainer1
            // 
            this.mapObjectContainer1.AutoScroll = true;
            this.mapObjectContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mapObjectContainer1.Location = new System.Drawing.Point(1, 195);
            this.mapObjectContainer1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.mapObjectContainer1.Name = "mapObjectContainer1";
            this.mapObjectContainer1.Size = new System.Drawing.Size(413, 419);
            this.mapObjectContainer1.TabIndex = 16;
            // 
            // btnAdvancedTools
            // 
            this.btnAdvancedTools.Location = new System.Drawing.Point(142, 150);
            this.btnAdvancedTools.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAdvancedTools.Name = "btnAdvancedTools";
            this.btnAdvancedTools.Size = new System.Drawing.Size(125, 21);
            this.btnAdvancedTools.TabIndex = 22;
            this.btnAdvancedTools.Text = "Advanced Tools...";
            this.btnAdvancedTools.UseVisualStyleBackColor = true;
            this.btnAdvancedTools.Click += new System.EventHandler(this.BtnAdvancedTools_Click);
            // 
            // MapMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 573);
            this.Controls.Add(this.btnAdvancedTools);
            this.Controls.Add(this.btnNewMap);
            this.Controls.Add(this.chkUseSmartPlacer);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.mapMakerObjectPropertyListControl1);
            this.Controls.Add(this.mapObjectContainer1);
            this.Controls.Add(this.cmbBiome);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnDefaultDimensions);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbHeight);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbWidth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbEpisode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbGameMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pnlMapCanvas);
            this.Controls.Add(this.txtMapName);
            this.Controls.Add(this.label1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MapMaker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Map Maker";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MapMaker_FormClosing);
            this.Load += new System.EventHandler(this.MapMaker_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapMaker_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MapMaker_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMapName;
        private System.Windows.Forms.Panel pnlMapCanvas;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbGameMode;
        private System.Windows.Forms.ComboBox cmbEpisode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbWidth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbHeight;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Button btnDefaultDimensions;
        private System.Windows.Forms.ComboBox cmbBiome;
        private System.Windows.Forms.Label label7;
        private UserControls.MapMakerUserControls.MapObjectContainer mapObjectContainer1;
        private UserControls.MapMakerUserControls.MapMakerObjectPropertyListControl mapMakerObjectPropertyListControl1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.OpenFileDialog dialogMapLoader;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.CheckBox chkUseSmartPlacer;
        private System.Windows.Forms.Button btnNewMap;
        private System.Windows.Forms.Button btnAdvancedTools;
    }
}