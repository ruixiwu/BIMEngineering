using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BIM.Lmv.Revit.Helpers.Progress
{
    internal class FormProgress : Form
    {
        private IContainer components;
        private Label label1;
        private Panel panel1;

        public FormProgress()
        {
            InitializeComponent();
        }

        public FormProgress(string title) : this()
        {
            if (title != null)
            {
                label1.Text = title;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            label1 = new Label();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            label1.BackColor = Color.Transparent;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft Sans Serif", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(0x17d, 0x45);
            label1.TabIndex = 1;
            label1.Text = "正在加载...";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            panel1.BackColor = Color.White;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(0x17f, 0x47);
            panel1.TabIndex = 3;
            AutoScaleDimensions = new SizeF(96f, 96f);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(0x17f, 0x47);
            Controls.Add(panel1);
            Font = new Font("Tahoma", 9f);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormProgress";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Progress";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}