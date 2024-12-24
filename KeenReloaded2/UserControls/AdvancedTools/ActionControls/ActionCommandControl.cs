using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace KeenReloaded2.UserControls.AdvancedTools
{
    public class ActionCommandControl : UserControl
    {
        private Button btnPreview;
        private Button btnCommit;
        private Button btnCancel;

        public ActionCommandControl()
        {
            InitializeComponent();
        }

        public ICommand CancelCommand { get; set; }

        public ICommand PreviewCommand { get; set; }

        public ICommand CommitCommand { get; set; }

        private void InitializeComponent()
        {
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnCommit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(3, 3);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(82, 65);
            this.btnPreview.TabIndex = 0;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.BtnPreview_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Location = new System.Drawing.Point(91, 3);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(82, 65);
            this.btnCommit.TabIndex = 1;
            this.btnCommit.Text = "Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.BtnCommit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(179, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 65);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // ActionCommandControl
            // 
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnPreview);
            this.Name = "ActionCommandControl";
            this.Size = new System.Drawing.Size(297, 71);
            this.Load += new System.EventHandler(this.ActionCommandControl_Load);
            this.ResumeLayout(false);

        }

        private void ExecuteCommand(ICommand command)
        {
            if (command == null)
                return;

            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ExecuteCommand(CancelCommand);
        }

        private void BtnCommit_Click(object sender, EventArgs e)
        {
            ExecuteCommand(CommitCommand);
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            ExecuteCommand(PreviewCommand);
        }

        private void ActionCommandControl_Load(object sender, EventArgs e)
        {

        }
    }
}
