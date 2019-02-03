using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using BIM.Lmv.Revit.Config;
using BIM.Lmv.Revit.Core;
using BIM.Lmv.Revit.Helpers;
using BIM.Lmv.Revit.Properties;
using BIM.Lmv.Revit.Utility;
using BIM.Lmv.Types;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;

namespace BIM.Lmv.Revit.UI
{
    internal class FormLocalShare : Form
    {
        private readonly AppConfig _Config;
        private readonly Dictionary<int, bool> _ElementIds;
        private readonly View3D _View;
        private Button btnBrowse;
        private Button btnCancel;
        private Button btnLicense;
        private Button btnOK;
        private CheckBox cbIncludeProperty;
        private CheckBox cbIncludeTexture;
        private IContainer components;
        private FlowLayoutPanel flowLayoutPanel1;
        private GroupBox groupBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private LinkLabel llDownloadViewer;
        private SaveFileDialog saveFileDialog1;
        private TextBox txtTargetPath;

        public FormLocalShare()
        {
            InitializeComponent();
        }

        public FormLocalShare( View3D view, AppConfig config, Dictionary<int, bool> elementIds)
            : this()
        {
            _View = view;
            _Config = config;
            _ElementIds = elementIds;
        }

        public TimeSpan ExportDuration { get; private set; }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var text = txtTargetPath.Text;
            var dialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = ".vimodel",
                Title = "输出文件到",
                Filter = "微模文件|*.vimodel|所有文件|*.*"
            };
            if (!string.IsNullOrEmpty(text))
            {
                dialog.FileName = text;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtTargetPath.Text = dialog.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnLicense_Click(object sender, EventArgs e)
        {
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTargetPath.Text))
            {
                MessageBox.Show("请先选择输出路径!", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                var local = _Config.Local;
                local.IncludeTexture = cbIncludeTexture.Checked;
                local.IncludeProperty = cbIncludeProperty.Checked;
                local.LastTargetPath = txtTargetPath.Text;
                _Config.Save();
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    Enabled = false;
                    using (new ProgressHelper(this, "正在执行轻量化转换..."))
                    {
                        StartExport(_View, txtTargetPath.Text, ExportTarget.LocalPackage, null,
                            cbIncludeTexture.Checked, cbIncludeProperty.Checked);
                    }
                    stopwatch.Stop();
                    var elapsed = stopwatch.Elapsed;
                    ExportDuration = new TimeSpan(elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
                    this.ShowMessageBox("微模轻量化文件保存成功!(耗时:" + ExportDuration + ")");
                    DialogResult = DialogResult.OK;
                }
                catch (Exception exception)
                {
                    stopwatch.Stop();
                    this.ShowMessageBox(exception.ToString());
                    DialogResult = DialogResult.Cancel;
                }
                finally
                {
                    Enabled = true;
                }
                Close();
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

        private void FormExport_Load(object sender, EventArgs e)
        {
            Icon = Icon.FromHandle(Resources.share_32px_1201342.GetHicon());
            var local = _Config.Local;
            cbIncludeProperty.Checked = local.IncludeProperty;
            cbIncludeTexture.Checked = local.IncludeTexture;
            txtTargetPath.Text = local.LastTargetPath;
        }

        private void InitializeComponent()
        {
            btnOK = new Button();
            groupBox1 = new GroupBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            label1 = new Label();
            llDownloadViewer = new LinkLabel();
            cbIncludeTexture = new CheckBox();
            cbIncludeProperty = new CheckBox();
            label3 = new Label();
            btnBrowse = new Button();
            txtTargetPath = new TextBox();
            label2 = new Label();
            btnCancel = new Button();
            saveFileDialog1 = new SaveFileDialog();
            btnLicense = new Button();
            groupBox1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            btnOK.Location = new Point(0x115, 0xa3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(0x4b, 0x20);
            btnOK.TabIndex = 1;
            btnOK.Text = "确定(&O)";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            groupBox1.Controls.Add(flowLayoutPanel1);
            groupBox1.Controls.Add(cbIncludeTexture);
            groupBox1.Controls.Add(cbIncludeProperty);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(btnBrowse);
            groupBox1.Controls.Add(txtTargetPath);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(0x1c9, 0x91);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "选项";
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Controls.Add(llDownloadViewer);
            flowLayoutPanel1.Location = new Point(0x65, 0x5d);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(0, 5, 0, 0);
            flowLayoutPanel1.Size = new Size(350, 0x2e);
            flowLayoutPanel1.TabIndex = 3;
            flowLayoutPanel1.WrapContents = false;
            label1.AutoSize = true;
            label1.Location = new Point(3, 5);
            label1.Name = "label1";
            label1.Size = new Size(0xc7, 14);
            label1.TabIndex = 3;
            label1.Text = "注: 查看微模轻量化分享文件请安装 ";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            llDownloadViewer.AutoSize = true;
            llDownloadViewer.Location = new Point(0xd0, 5);
            llDownloadViewer.Name = "llDownloadViewer";
            llDownloadViewer.Size = new Size(0x43, 14);
            llDownloadViewer.TabIndex = 10;
            llDownloadViewer.TabStop = true;
            llDownloadViewer.Text = "微模浏览器";
            llDownloadViewer.TextAlign = ContentAlignment.MiddleLeft;
            llDownloadViewer.LinkClicked += llDownloadViewer_LinkClicked;
            cbIncludeTexture.Location = new Point(0xc7, 0x20);
            cbIncludeTexture.Name = "cbIncludeTexture";
            cbIncludeTexture.Size = new Size(0x53, 0x16);
            cbIncludeTexture.TabIndex = 6;
            cbIncludeTexture.Text = "包含纹理";
            cbIncludeTexture.UseVisualStyleBackColor = true;
            cbIncludeProperty.Location = new Point(0x65, 0x1f);
            cbIncludeProperty.Name = "cbIncludeProperty";
            cbIncludeProperty.Size = new Size(0x53, 0x16);
            cbIncludeProperty.TabIndex = 5;
            cbIncludeProperty.Text = "包含属性";
            cbIncludeProperty.UseVisualStyleBackColor = true;
            label3.AutoSize = true;
            label3.Location = new Point(0x24, 0x22);
            label3.Name = "label3";
            label3.Size = new Size(0x3b, 14);
            label3.TabIndex = 4;
            label3.Text = "输出内容:";
            label3.TextAlign = ContentAlignment.MiddleRight;
            btnBrowse.Location = new Point(0x180, 0x3f);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(0x25, 0x18);
            btnBrowse.TabIndex = 9;
            btnBrowse.Text = "...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            txtTargetPath.Location = new Point(0x65, 0x40);
            txtTargetPath.Name = "txtTargetPath";
            txtTargetPath.ReadOnly = true;
            txtTargetPath.Size = new Size(0x119, 0x16);
            txtTargetPath.TabIndex = 8;
            label2.AutoSize = true;
            label2.Location = new Point(0x24, 0x44);
            label2.Name = "label2";
            label2.Size = new Size(0x3b, 14);
            label2.TabIndex = 7;
            label2.Text = "输出路径:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(0x166, 0xa3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(0x4b, 0x20);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "取消(&C)";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            btnLicense.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            btnLicense.Location = new Point(0x18, 0xa3);
            btnLicense.Name = "btnLicense";
            btnLicense.Size = new Size(0x53, 0x20);
            btnLicense.TabIndex = 7;
            btnLicense.Text = "授权管理...";
            btnLicense.UseVisualStyleBackColor = true;
            btnLicense.Click += btnLicense_Click;
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96f, 96f);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(0x1e1, 0xcd);
            Controls.Add(btnLicense);
            Controls.Add(btnCancel);
            Controls.Add(groupBox1);
            Controls.Add(btnOK);
            Font = new Font("Tahoma", 9f);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLocalShare";
            StartPosition = FormStartPosition.CenterParent;
            Text = "本地分享 - 比目鱼云";
            Load += FormExport_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        private void llDownloadViewer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://vim-s1.oss-cn-shanghai.aliyuncs.com/products/vimodel-viewer/setup_v1.0.2.zip");
        }

        private void StartExport(View3D view, string targetPath, ExportTarget target, Stream outputStream,
            bool includeTexture, bool includeProperty)
        {
            var document = view.Document;
            var context = new ExportContext(view, document, targetPath, target, outputStream, includeTexture,
                includeProperty, null, _ElementIds);
            new CustomExporter(document, context)
            {
                IncludeGeometricObjects = false,
                ShouldStopOnError = false
            }.Export(view);
        }
    }
}