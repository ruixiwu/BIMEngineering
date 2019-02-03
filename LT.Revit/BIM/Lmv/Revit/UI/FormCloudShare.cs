using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using BIM.Lmv.Revit.Config;
using BIM.Lmv.Revit.Core;
using BIM.Lmv.Revit.Core.Cloud;
using BIM.Lmv.Revit.Helpers;
using BIM.Lmv.Revit.Properties;
using BIM.Lmv.Revit.Utility;
using BIM.Lmv.Types;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;

namespace BIM.Lmv.Revit.UI
{
    internal class FormCloudShare : Form
    {
        private readonly CloudClient _Client;
        private readonly AppConfig _Config;
        private readonly string _ModelName;
        private readonly Stream _OutputStream;
        private readonly View3D _View;
        private Button btnCancel;
        private Button btnLicense;
        private Button btnLoginConfig;
        private Button btnOK;
        private CheckBox cbIncludeProperty;
        private CheckBox cbIncludeTexture;
        private IContainer components;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label label3;
        private Label label4;
        private RadioButton rbShareExpired7Days;
        private RadioButton rbShareExpiredLongTerm;
        private RadioButton rbShareModePrivate;
        private RadioButton rbShareModePublic;
        private SaveFileDialog saveFileDialog1;
        private TextBox txtModelName;

        public FormCloudShare()
        {
            InitializeComponent();
        }

        public FormCloudShare( View3D view, CloudClient client, AppConfig config,
            string modelName, Stream outputStream) : this()
        {
            _View = view;
            _Client = client;
            _Config = config;
            _ModelName = modelName;
            _OutputStream = outputStream;
        }

        public TimeSpan ExportDuration { get; private set; }

        public string ModelName { get; private set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnLicense_Click(object sender, EventArgs e)
        {
        }

        private void btnLoginConfig_Click(object sender, EventArgs e)
        {
            new FormCloudLogin(_Client, _Config).ShowDialog(this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!_Client.IsLogined())
            {
                var login = new FormCloudLogin(_Client, _Config);
                if (login.ShowDialog(this) == DialogResult.Cancel)
                {
                    return;
                }
            }
            var cloud = _Config.Cloud;
            cloud.IncludeTexture = cbIncludeTexture.Checked;
            cloud.IncludeProperty = cbIncludeProperty.Checked;
            cloud.ShareMode = rbShareModePublic.Checked ? 1 : 2;
            cloud.ShareExpireDays = rbShareExpiredLongTerm.Checked ? 0 : 7;
            _Config.Save();
            ModelName = string.IsNullOrEmpty(txtModelName.Text) ? _ModelName : txtModelName.Text;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                Enabled = false;
                using (new ProgressHelper(this, "正在执行轻量化转换..."))
                {
                    StartExport(_View, null, ExportTarget.CloudPackage, _OutputStream,
                        cbIncludeTexture.Checked, cbIncludeProperty.Checked);
                }
                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                ExportDuration = new TimeSpan(elapsed.Days, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
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
            Icon = Icon.FromHandle(Resources.share_32px.GetHicon());
            var cloud = _Config.Cloud;
            txtModelName.Text = _ModelName;
            cbIncludeProperty.Checked = cloud.IncludeProperty;
            cbIncludeTexture.Checked = cloud.IncludeTexture;
            if (cloud.ShareMode == 1)
            {
                rbShareModePublic.Checked = true;
            }
            else
            {
                rbShareModePrivate.Checked = true;
            }
            if (cloud.ShareExpireDays == 0)
            {
                rbShareExpiredLongTerm.Checked = true;
            }
            else
            {
                rbShareExpired7Days.Checked = true;
            }
        }

        private void InitializeComponent()
        {
            btnOK = new Button();
            groupBox1 = new GroupBox();
            txtModelName = new TextBox();
            label4 = new Label();
            cbIncludeTexture = new CheckBox();
            cbIncludeProperty = new CheckBox();
            label3 = new Label();
            btnCancel = new Button();
            saveFileDialog1 = new SaveFileDialog();
            groupBox2 = new GroupBox();
            rbShareModePrivate = new RadioButton();
            rbShareModePublic = new RadioButton();
            groupBox3 = new GroupBox();
            rbShareExpired7Days = new RadioButton();
            rbShareExpiredLongTerm = new RadioButton();
            btnLoginConfig = new Button();
            btnLicense = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            btnOK.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnOK.Location = new Point(0x108, 0x15b);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(0x63, 0x20);
            btnOK.TabIndex = 4;
            btnOK.Text = "开始分享(&S)";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            groupBox1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            groupBox1.Controls.Add(txtModelName);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(cbIncludeTexture);
            groupBox1.Controls.Add(cbIncludeProperty);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(0x1cd, 100);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "模型信息";
            txtModelName.Location = new Point(0x5f, 0x22);
            txtModelName.MaxLength = 0x1000;
            txtModelName.Name = "txtModelName";
            txtModelName.Size = new Size(0x151, 0x16);
            txtModelName.TabIndex = 1;
            label4.AutoSize = true;
            label4.Location = new Point(0x1d, 0x25);
            label4.Name = "label4";
            label4.Size = new Size(0x3b, 14);
            label4.TabIndex = 0;
            label4.Text = "模型名称:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            cbIncludeTexture.Location = new Point(0xc1, 0x3e);
            cbIncludeTexture.Name = "cbIncludeTexture";
            cbIncludeTexture.Size = new Size(0x53, 0x16);
            cbIncludeTexture.TabIndex = 4;
            cbIncludeTexture.Text = "包含纹理";
            cbIncludeTexture.UseVisualStyleBackColor = true;
            cbIncludeProperty.Location = new Point(0x5f, 0x3e);
            cbIncludeProperty.Name = "cbIncludeProperty";
            cbIncludeProperty.Size = new Size(0x53, 0x16);
            cbIncludeProperty.TabIndex = 3;
            cbIncludeProperty.Text = "包含属性";
            cbIncludeProperty.UseVisualStyleBackColor = true;
            label3.AutoSize = true;
            label3.Location = new Point(0x1d, 0x3e);
            label3.Name = "label3";
            label3.Size = new Size(0x3b, 14);
            label3.TabIndex = 2;
            label3.Text = "模型选项:";
            label3.TextAlign = ContentAlignment.MiddleRight;
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(0x171, 0x15b);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(0x4b, 0x20);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "取消(&C)";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            groupBox2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            groupBox2.Controls.Add(rbShareModePrivate);
            groupBox2.Controls.Add(rbShareModePublic);
            groupBox2.Location = new Point(12, 0x81);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(0x1cd, 90);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "分享模式";
            rbShareModePrivate.AutoSize = true;
            rbShareModePrivate.Location = new Point(0x23, 0x37);
            rbShareModePrivate.Name = "rbShareModePrivate";
            rbShareModePrivate.Size = new Size(0x169, 0x12);
            rbShareModePrivate.TabIndex = 1;
            rbShareModePrivate.Text = "私密分享 - 必须输入云端自动生成的查看密码才能打开分享链接";
            rbShareModePrivate.UseVisualStyleBackColor = true;
            rbShareModePublic.AutoSize = true;
            rbShareModePublic.Checked = true;
            rbShareModePublic.Location = new Point(0x23, 0x1f);
            rbShareModePublic.Name = "rbShareModePublic";
            rbShareModePublic.Size = new Size(0x121, 0x12);
            rbShareModePublic.TabIndex = 0;
            rbShareModePublic.TabStop = true;
            rbShareModePublic.Text = "公开分享 - 任何人都可以打开分享的链接查看模型";
            rbShareModePublic.UseVisualStyleBackColor = true;
            groupBox3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            groupBox3.Controls.Add(rbShareExpired7Days);
            groupBox3.Controls.Add(rbShareExpiredLongTerm);
            groupBox3.Location = new Point(12, 0xf1);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(0x1cd, 90);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "分享期限";
            rbShareExpired7Days.AutoSize = true;
            rbShareExpired7Days.Location = new Point(0x23, 0x37);
            rbShareExpired7Days.Name = "rbShareExpired7Days";
            rbShareExpired7Days.Size = new Size(0x100, 0x12);
            rbShareExpired7Days.TabIndex = 1;
            rbShareExpired7Days.Text = "临时分享 - 云端分享的模型 7 天后自动删除";
            rbShareExpired7Days.UseVisualStyleBackColor = true;
            rbShareExpiredLongTerm.AutoSize = true;
            rbShareExpiredLongTerm.Checked = true;
            rbShareExpiredLongTerm.Location = new Point(0x23, 0x1f);
            rbShareExpiredLongTerm.Name = "rbShareExpiredLongTerm";
            rbShareExpiredLongTerm.Size = new Size(0x49, 0x12);
            rbShareExpiredLongTerm.TabIndex = 0;
            rbShareExpiredLongTerm.TabStop = true;
            rbShareExpiredLongTerm.Text = "长期有效";
            rbShareExpiredLongTerm.UseVisualStyleBackColor = true;
            btnLoginConfig.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            btnLoginConfig.Location = new Point(0x6a, 0x15b);
            btnLoginConfig.Name = "btnLoginConfig";
            btnLoginConfig.Size = new Size(0x53, 0x20);
            btnLoginConfig.TabIndex = 3;
            btnLoginConfig.Text = "登录设置...";
            btnLoginConfig.UseVisualStyleBackColor = true;
            btnLoginConfig.Click += btnLoginConfig_Click;
            btnLicense.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            btnLicense.Location = new Point(0x11, 0x15b);
            btnLicense.Name = "btnLicense";
            btnLicense.Size = new Size(0x53, 0x20);
            btnLicense.TabIndex = 6;
            btnLicense.Text = "授权管理...";
            btnLicense.UseVisualStyleBackColor = true;
            btnLicense.Click += btnLicense_Click;
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96f, 96f);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(0x1e6, 400);
            Controls.Add(btnLicense);
            Controls.Add(btnLoginConfig);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(btnCancel);
            Controls.Add(groupBox1);
            Controls.Add(btnOK);
            Font = new Font("Tahoma", 9f);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(0x1f6, 0x1b7);
            Name = "FormCloudShare";
            StartPosition = FormStartPosition.CenterParent;
            Text = "云端分享 - 比目鱼云";
            Load += FormExport_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        private void StartExport(View3D view, string targetPath, ExportTarget target, Stream outputStream,
            bool includeTexture, bool includeProperty)
        {
            var document = view.Document;
            var context = new ExportContext(view, document, targetPath, target, outputStream, includeTexture,
                includeProperty, null, null);
            new CustomExporter(document, context)
            {
                IncludeGeometricObjects = false,
                ShouldStopOnError = false
            }.Export(view);
        }
    }
}