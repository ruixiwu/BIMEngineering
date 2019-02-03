using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BIM.Lmv.Revit.Properties;
using BIM.Lmv.Revit.Utility;

namespace BIM.Lmv.Revit.UI
{
    internal class FormCloudShareResultPublic : Form
    {
        private readonly string _Link;
        private Button btnCancel;
        private Button btnOK;
        private IContainer components;
        private GroupBox groupBox1;
        private Label label4;
        private TextBox txtLink;

        public FormCloudShareResultPublic()
        {
            InitializeComponent();
        }

        public FormCloudShareResultPublic(string link) : this()
        {
            _Link = link;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_Link);
            this.ShowMessageBox("复制成功!");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormCloudShareResultPublic_Load(object sender, EventArgs e)
        {
            Icon = Icon.FromHandle(Resources.share_32px.GetHicon());
            txtLink.Text = _Link;
        }

        private void FormCloudShareResultPublic_Shown(object sender, EventArgs e)
        {
            btnOK.Focus();
        }

        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            btnOK = new Button();
            txtLink = new TextBox();
            label4 = new Label();
            btnCancel = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            groupBox1.Controls.Add(btnOK);
            groupBox1.Controls.Add(txtLink);
            groupBox1.Controls.Add(label4);
            groupBox1.Location = new Point(13, 13);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(0x163, 0x62);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            btnOK.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnOK.Location = new Point(70, 0x33);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(0x56, 0x20);
            btnOK.TabIndex = 2;
            btnOK.Text = "复制链接";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            txtLink.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            txtLink.Location = new Point(70, 0x15);
            txtLink.MaxLength = 0x100;
            txtLink.Name = "txtLink";
            txtLink.ReadOnly = true;
            txtLink.Size = new Size(0xf5, 0x16);
            txtLink.TabIndex = 1;
            label4.AutoSize = true;
            label4.Location = new Point(0x1d, 0x18);
            label4.Name = "label4";
            label4.Size = new Size(0x23, 14);
            label4.TabIndex = 0;
            label4.Text = "链接:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(0x125, 0x7a);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(0x4b, 0x20);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "关闭";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            AutoScaleDimensions = new SizeF(96f, 96f);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(380, 0xa6);
            Controls.Add(btnCancel);
            Controls.Add(groupBox1);
            Font = new Font("Tahoma", 9f);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormCloudShareResultPublic";
            StartPosition = FormStartPosition.CenterParent;
            Text = "云端分享 - 比目鱼云";
            Load += FormCloudShareResultPublic_Load;
            Shown += FormCloudShareResultPublic_Shown;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }
    }
}