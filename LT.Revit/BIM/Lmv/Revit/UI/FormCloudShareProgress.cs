using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using BIM.Lmv.Revit.Core.Cloud;
using BIM.Lmv.Revit.Properties;

namespace BIM.Lmv.Revit.UI
{
    internal class FormCloudShareProgress : Form
    {
        private readonly ManualResetEvent _CancelEvent;
        private readonly MemoryStream _OutputStream;
        private readonly InvokeResultUpload _UploadPermit;
        private Button btnCancel;
        private IContainer components;
        private GroupBox groupBox1;
        private Label lblPrompt;
        private ProgressBar pbProgress;

        public FormCloudShareProgress()
        {
            InitializeComponent();
        }

        public FormCloudShareProgress(MemoryStream outputStream, InvokeResultUpload uploadPermit) : this()
        {
            _OutputStream = outputStream;
            _UploadPermit = uploadPermit;
            _CancelEvent = new ManualResetEvent(false);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _CancelEvent.Set();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormCloudShareProgress_FormClosed(object sender, FormClosedEventArgs e)
        {
            _CancelEvent.Set();
        }

        private void FormCloudShareProgress_Load(object sender, EventArgs e)
        {
            Icon = Icon.FromHandle(Resources.share_32px.GetHicon());
            pbProgress.Value = 0;
        }

        private void FormCloudShareProgress_Shown(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(UploadThread);
        }

        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            pbProgress = new ProgressBar();
            lblPrompt = new Label();
            btnCancel = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            groupBox1.Controls.Add(pbProgress);
            groupBox1.Controls.Add(lblPrompt);
            groupBox1.Location = new Point(13, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(0x153, 0x59);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            pbProgress.Location = new Point(0x13, 0x31);
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new Size(300, 0x17);
            pbProgress.TabIndex = 1;
            pbProgress.Value = 40;
            lblPrompt.Location = new Point(0x10, 0x17);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(0x12f, 0x17);
            lblPrompt.TabIndex = 0;
            lblPrompt.Text = "请稍侯, 正在将轻量化数据发送到云端...";
            lblPrompt.TextAlign = ContentAlignment.MiddleLeft;
            btnCancel.Location = new Point(0x94, 0x73);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(0x4b, 0x20);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            AutoScaleDimensions = new SizeF(96f, 96f);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(0x16c, 0x9f);
            Controls.Add(btnCancel);
            Controls.Add(groupBox1);
            Font = new Font("Tahoma", 9f);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormCloudShareProgress";
            StartPosition = FormStartPosition.CenterParent;
            Text = "云端分享 - 比目鱼云";
            FormClosed += FormCloudShareProgress_FormClosed;
            Load += FormCloudShareProgress_Load;
            Shown += FormCloudShareProgress_Shown;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void UploadFinishCallback(bool success)
        {
            if (InvokeRequired)
            {
                Action<bool> method = UploadFinishCallback;
                Invoke(method, success);
            }
            else
            {
                DialogResult = success ? DialogResult.OK : DialogResult.Cancel;
                Close();
            }
        }

        private void UploadProgressCallback(long progressSize)
        {
            try
            {
                if (InvokeRequired)
                {
                    Action<long> method = UploadProgressCallback;
                    Invoke(method, progressSize);
                }
                else
                {
                    var num = progressSize*100L/_OutputStream.Length;
                    pbProgress.Value = (int) Math.Min(100L, num);
                    Application.DoEvents();
                }
            }
            catch
            {
            }
        }

        private void UploadThread(object state)
        {
            var fields = new Dictionary<string, string>();
            fields["policy"] = _UploadPermit.Policy;
            fields["OSSAccessKeyId"] = _UploadPermit.AccessKeyId;
            fields["signature"] = _UploadPermit.Signature;
            var uri = new Uri(_UploadPermit.OssServer);
            var objectKey = _UploadPermit.ObjectKey;
            var success = OssHelper.Post(uri, null, fields, null, objectKey, _OutputStream,
                (int) _OutputStream.Length, UploadProgressCallback, _CancelEvent);
            UploadFinishCallback(success);
        }
    }
}