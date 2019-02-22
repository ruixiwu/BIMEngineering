namespace BIM.Lmv.Revit.Helpers.Progress
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    internal class FormProgress : Form
    {
        private IContainer components;
        private Label label1;
        private Panel panel1;

        public FormProgress()
        {
            this.InitializeComponent();
        }

        public FormProgress(string title) : this()
        {
            if (title != null)
            {
                this.label1.Text = title;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.panel1 = new Panel();
            this.panel1.SuspendLayout();
            base.SuspendLayout();
            this.label1.BackColor = Color.Transparent;
            this.label1.Dock = DockStyle.Fill;
            this.label1.Font = new Font("Microsoft Sans Serif", 10.5f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.label1.Location = new Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x17d, 0x45);
            this.label1.TabIndex = 1;
            this.label1.Text = "正在加载...";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.panel1.BackColor = Color.White;
            this.panel1.BorderStyle = BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x17f, 0x47);
            this.panel1.TabIndex = 3;
            base.AutoScaleDimensions = new SizeF(96f, 96f);
            base.AutoScaleMode = AutoScaleMode.Dpi;
            base.ClientSize = new Size(0x17f, 0x47);
            base.Controls.Add(this.panel1);
            this.Font = new Font("Tahoma", 9f);
            base.FormBorderStyle = FormBorderStyle.None;
            base.Name = "FormProgress";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Progress";
            this.panel1.ResumeLayout(false);
            base.ResumeLayout(false);
        }
    }
}

