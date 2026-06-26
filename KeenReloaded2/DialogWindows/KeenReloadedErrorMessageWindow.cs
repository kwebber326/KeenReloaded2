using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeenReloaded2.DialogWindows
{
    public class KeenReloadedErrorMessageWindow : KeenReloadedMessageWindow
    {
        private System.Windows.Forms.PictureBox pbError;

        public KeenReloadedErrorMessageWindow(string errorMsg) : base(errorMsg)
        {
            this.InitializeComponent();
        }

        protected override void KeenReloadedMessageWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void InitializeComponent()
        {
            this.pbError = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbError)).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.Size = new System.Drawing.Size(0, 18);
            this.lblText.Text = "";
            // 
            // pbError
            // 
            this.pbError.BackColor = System.Drawing.Color.Transparent;
            this.pbError.Image = global::KeenReloaded2.Properties.Resources.keen_face_scared;
            this.pbError.Location = new System.Drawing.Point(12, 37);
            this.pbError.Name = "pbError";
            this.pbError.Size = new System.Drawing.Size(102, 108);
            this.pbError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbError.TabIndex = 2;
            this.pbError.TabStop = false;
            // 
            // KeenReloadedErrorMessageWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(440, 157);
            this.Controls.Add(this.pbError);
            this.Name = "KeenReloadedErrorMessageWindow";
            this.Controls.SetChildIndex(this.lblText, 0);
            this.Controls.SetChildIndex(this.pbError, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pbError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
