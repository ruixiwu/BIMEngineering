using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BIM.Lmv.Revit.Config;
using BIM.Lmv.Revit.Core.Cloud;
using BIM.Lmv.Revit.Helpers;
using BIM.Lmv.Revit.Properties;
using BIM.Lmv.Revit.Utility;
using Newtonsoft.Json;
using WebSocketSharp;
using Timer = System.Timers.Timer;

namespace BIM.Lmv.Revit.UI
{
    internal class FormCloudLogin : Form
    {
        private const string URL_REG = "http://www.v.yunzu360.com/Account/register";
        private readonly CloudClient _Client;
        private readonly AppConfig _Config;
        private Button btn_retry;
        private Button btnCancel;
        private Button btnOK;
        private CheckBox cbRemember;
        private IContainer components;
        private Label label1;
        private Label label4;
        private TabControl lbcontrolLogin;
        private Label lbQRCodeInfo;
        private Label lbSuccessInfo;
        private LinkLabel llRegister;
        private readonly SynchronizationContext m_SyncContext;
        private PictureBox picQRCode;
        private TabPage tabPageNormal;
        private TabPage tabpageWX;
        private Timer timer;
        private TextBox txtPassword;
        private TextBox txtUserName;
        private WebSocket ws;

        public FormCloudLogin()
        {
            InitializeComponent();
            m_SyncContext = SynchronizationContext.Current;
        }

        public FormCloudLogin(CloudClient client, AppConfig config) : this()
        {
            _Client = client;
            _Config = config;
        }

        private void btn_retry_Click(object sender, EventArgs e)
        {
            LoadQRCode();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Enabled = false;
                using (new ProgressHelper(this, "正在验证云端账号..."))
                {
                    if (_Client.Login(txtUserName.Text, txtPassword.Text).Success)
                    {
                        if (cbRemember.Checked)
                        {
                            _Config.Cloud.UserName = txtUserName.Text;
                            _Config.Cloud.UserPassword = txtPassword.Text;
                            _Config.Cloud.UserRemember = true;
                        }
                        else
                        {
                            _Config.Cloud.UserName = string.Empty;
                            _Config.Cloud.UserPassword = string.Empty;
                            _Config.Cloud.UserRemember = true;
                        }
                        _Config.Save();
                        ProgressHelper.Close();
                        this.ShowMessageBox("云端账号验证通过!");
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        this.ShowMessageBox("云端账号验证失败...请检查您输入的账号和密码!");
                    }
                }
            }
            finally
            {
                Enabled = true;
            }
        }

        private bool ConnectToWebSocketServer(string wsUrl, string sceneId)
        {
            EventHandler<MessageEventArgs> handler = null;
            try
            {
                if (ws != null)
                {
                    ws.Close();
                }
                ws = new WebSocket(wsUrl);
                if (handler == null)
                {
                    handler = delegate(object s, MessageEventArgs e)
                    {
                        if (!string.IsNullOrEmpty(e.Data) && (e.Data != "failed"))
                        {
                            _Config.Cloud.WXToken = e.Data;
                            _Config.Save();
                            ws.Close();
                            _Client.Login(e.Data);
                            this.ShowMessageBox("微信验证通过!");
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                    };
                }
                ws.OnMessage += handler;
                ws.Connect();
                ws.Send(sceneId);
                return true;
            }
            catch
            {
                return false;
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

        private void FormCloudConfig_Load(object sender, EventArgs e)
        {
            Icon = Icon.FromHandle(Resources.share_32px.GetHicon());
            txtUserName.Text = _Config.Cloud.UserName;
            txtPassword.Text = _Config.Cloud.UserPassword;
            cbRemember.Checked = _Config.Cloud.UserRemember;
        }

        private void FormCloudLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Close();
                timer.Dispose();
            }
        }

        private void FormCloudLogin_Shown(object sender, EventArgs e)
        {
            if (lbcontrolLogin.TabPages.ContainsKey("tabpageWX"))
            {
                LoadQRCode();
            }
        }

        private void InitializeComponent()
        {
            btnCancel = new Button();
            btnOK = new Button();
            lbcontrolLogin = new TabControl();
            tabpageWX = new TabPage();
            lbSuccessInfo = new Label();
            btn_retry = new Button();
            lbQRCodeInfo = new Label();
            picQRCode = new PictureBox();
            tabPageNormal = new TabPage();
            llRegister = new LinkLabel();
            cbRemember = new CheckBox();
            txtPassword = new TextBox();
            label1 = new Label();
            txtUserName = new TextBox();
            label4 = new Label();
            lbcontrolLogin.SuspendLayout();
            tabpageWX.SuspendLayout();
            ((ISupportInitialize) picQRCode).BeginInit();
            tabPageNormal.SuspendLayout();
            SuspendLayout();
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(0xfe, 0x86);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(0x4b, 0x20);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "取消(&C)";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            btnOK.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnOK.Location = new Point(0xad, 0x86);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(0x4b, 0x20);
            btnOK.TabIndex = 3;
            btnOK.Text = "确定(&O)";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            lbcontrolLogin.Controls.Add(tabpageWX);
            lbcontrolLogin.Controls.Add(tabPageNormal);
            lbcontrolLogin.Location = new Point(12, 5);
            lbcontrolLogin.Name = "lbcontrolLogin";
            lbcontrolLogin.SelectedIndex = 0;
            lbcontrolLogin.Size = new Size(0x165, 0xde);
            lbcontrolLogin.TabIndex = 5;
            tabpageWX.BackColor = Color.Transparent;
            tabpageWX.Controls.Add(lbSuccessInfo);
            tabpageWX.Controls.Add(btn_retry);
            tabpageWX.Controls.Add(lbQRCodeInfo);
            tabpageWX.Controls.Add(picQRCode);
            tabpageWX.Cursor = Cursors.Default;
            tabpageWX.Location = new Point(4, 0x17);
            tabpageWX.Name = "tabpageWX";
            tabpageWX.Padding = new Padding(3);
            tabpageWX.Size = new Size(0x15d, 0xc3);
            tabpageWX.TabIndex = 1;
            tabpageWX.Text = "微信扫码登陆";
            tabpageWX.UseVisualStyleBackColor = true;
            lbSuccessInfo.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            lbSuccessInfo.AutoSize = true;
            lbSuccessInfo.Location = new Point(0x26, 0xa4);
            lbSuccessInfo.Name = "lbSuccessInfo";
            lbSuccessInfo.Size = new Size(0x103, 14);
            lbSuccessInfo.TabIndex = 3;
            lbSuccessInfo.Text = "使用微信扫描上方二维码关注公众号后自动登陆";
            lbSuccessInfo.Visible = false;
            btn_retry.Location = new Point(0x89, 0x7d);
            btn_retry.Name = "btn_retry";
            btn_retry.Size = new Size(0x4b, 0x17);
            btn_retry.TabIndex = 2;
            btn_retry.Text = "重 试";
            btn_retry.UseVisualStyleBackColor = true;
            btn_retry.Visible = false;
            btn_retry.Click += btn_retry_Click;
            lbQRCodeInfo.Dock = DockStyle.Fill;
            lbQRCodeInfo.Location = new Point(3, 3);
            lbQRCodeInfo.Name = "lbQRCodeInfo";
            lbQRCodeInfo.Size = new Size(0x157, 0xbd);
            lbQRCodeInfo.TabIndex = 1;
            lbQRCodeInfo.Text = "正在生成二维码...";
            lbQRCodeInfo.TextAlign = ContentAlignment.MiddleCenter;
            picQRCode.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            picQRCode.Location = new Point(0x60, 6);
            picQRCode.Name = "picQRCode";
            picQRCode.Size = new Size(160, 0x97);
            picQRCode.SizeMode = PictureBoxSizeMode.StretchImage;
            picQRCode.TabIndex = 0;
            picQRCode.TabStop = false;
            tabPageNormal.BackColor = Color.Transparent;
            tabPageNormal.Controls.Add(llRegister);
            tabPageNormal.Controls.Add(cbRemember);
            tabPageNormal.Controls.Add(btnCancel);
            tabPageNormal.Controls.Add(txtPassword);
            tabPageNormal.Controls.Add(btnOK);
            tabPageNormal.Controls.Add(label1);
            tabPageNormal.Controls.Add(txtUserName);
            tabPageNormal.Controls.Add(label4);
            tabPageNormal.Location = new Point(4, 0x17);
            tabPageNormal.Name = "tabPageNormal";
            tabPageNormal.Padding = new Padding(3);
            tabPageNormal.Size = new Size(0x15d, 0xc3);
            tabPageNormal.TabIndex = 0;
            tabPageNormal.Text = "账号密码登陆";
            tabPageNormal.UseVisualStyleBackColor = true;
            llRegister.AutoSize = true;
            llRegister.Location = new Point(0x112, 0x56);
            llRegister.Name = "llRegister";
            llRegister.Size = new Size(0x37, 14);
            llRegister.TabIndex = 0x15;
            llRegister.TabStop = true;
            llRegister.Text = "注册账号";
            llRegister.TextAlign = ContentAlignment.MiddleRight;
            llRegister.LinkClicked += llRegister_LinkClicked;
            cbRemember.AutoSize = true;
            cbRemember.Location = new Point(70, 0x52);
            cbRemember.Name = "cbRemember";
            cbRemember.Size = new Size(0x62, 0x12);
            cbRemember.TabIndex = 20;
            cbRemember.Text = "记住登录信息";
            cbRemember.UseVisualStyleBackColor = true;
            txtPassword.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            txtPassword.Location = new Point(70, 0x36);
            txtPassword.MaxLength = 0x100;
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(0x103, 0x16);
            txtPassword.TabIndex = 0x13;
            label1.AutoSize = true;
            label1.Location = new Point(0x1d, 0x39);
            label1.Name = "label1";
            label1.Size = new Size(0x23, 14);
            label1.TabIndex = 0x12;
            label1.Text = "密码:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            txtUserName.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            txtUserName.Location = new Point(70, 0x1a);
            txtUserName.MaxLength = 0x100;
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(0x103, 0x16);
            txtUserName.TabIndex = 0x11;
            label4.AutoSize = true;
            label4.Location = new Point(0x1d, 0x1d);
            label4.Name = "label4";
            label4.Size = new Size(0x23, 14);
            label4.TabIndex = 0x10;
            label4.Text = "账号:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96f, 96f);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = btnCancel;
            ClientSize = new Size(0x17a, 230);
            Controls.Add(lbcontrolLogin);
            Font = new Font("Tahoma", 9f);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(0x16c, 0xe1);
            Name = "FormCloudLogin";
            StartPosition = FormStartPosition.CenterParent;
            Text = "登录设置 - 比目鱼云";
            FormClosed += FormCloudLogin_FormClosed;
            Load += FormCloudConfig_Load;
            Shown += FormCloudLogin_Shown;
            lbcontrolLogin.ResumeLayout(false);
            tabpageWX.ResumeLayout(false);
            tabpageWX.PerformLayout();
            ((ISupportInitialize) picQRCode).EndInit();
            tabPageNormal.ResumeLayout(false);
            tabPageNormal.PerformLayout();
            ResumeLayout(false);
        }

        private void llRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.v.yunzu360.com/Account/register");
        }

        private void LoadQRCode()
        {
            ThreadStart start2 = null;
            try
            {
                if (start2 == null)
                {
                    start2 = delegate
                    {
                        SendOrPostCallback d = null;
                        SendOrPostCallback callback2 = null;
                        SendOrPostCallback callback3 = null;
                        SendOrPostCallback callback4 = null;
                        SendOrPostCallback callback5 = null;
                        SendOrPostCallback callback6 = null;
                        SendOrPostCallback callback7 = null;
                        SendOrPostCallback callback8 = null;
                        SendOrPostCallback callback9 = null;
                        m_SyncContext.Post(text => lbQRCodeInfo.Text = text.ToString(), "正在生成二维码...");
                        m_SyncContext.Post(visible => btn_retry.Visible = (bool) visible, false);
                        var code =
                            JsonConvert.DeserializeObject<InvokeResultQRCode>(_Client.GetQRCode());
                        if ((code == null) || string.IsNullOrEmpty(code.QRCodeUrl) || string.IsNullOrEmpty(code.SceneId) ||
                            string.IsNullOrEmpty(code.WsUrl) || !ConnectToWebSocketServer(code.WsUrl, code.SceneId))
                        {
                            MessageBox.Show("获取二维码失败!");
                            if (d == null)
                            {
                                d = visible => btn_retry.Visible = (bool) visible;
                            }
                            m_SyncContext.Post(d, true);
                            if (callback2 == null)
                            {
                                callback2 = visible => lbQRCodeInfo.Visible = (bool) visible;
                            }
                            m_SyncContext.Post(callback2, true);
                            if (callback3 == null)
                            {
                                callback3 = visible => lbSuccessInfo.Visible = (bool) visible;
                            }
                            m_SyncContext.Post(callback3, false);
                            if (callback4 == null)
                            {
                                callback4 = text => lbQRCodeInfo.Text = text.ToString();
                            }
                            m_SyncContext.Post(callback4, "点击重试按钮重新获取二维码");
                        }
                        else
                        {
                            if (callback5 == null)
                            {
                                callback5 = visible => btn_retry.Visible = (bool) visible;
                            }
                            m_SyncContext.Post(callback5, false);
                            var state = Image.FromStream(HttpHelper.HttpGetStream(code.QRCodeUrl, null));
                            if (state != null)
                            {
                                var size = state.Size;
                            }
                            else
                            {
                                MessageBox.Show("获取二维码失败!");
                                if (callback6 == null)
                                {
                                    callback6 = visible => btn_retry.Visible = (bool) visible;
                                }
                                m_SyncContext.Post(callback6, true);
                                if (callback7 == null)
                                {
                                    callback7 = visible => lbQRCodeInfo.Visible = (bool) visible;
                                }
                                m_SyncContext.Post(callback7, true);
                                if (callback8 == null)
                                {
                                    callback8 = visible => lbSuccessInfo.Visible = (bool) visible;
                                }
                                m_SyncContext.Post(callback8, false);
                                if (callback9 == null)
                                {
                                    callback9 = text => lbQRCodeInfo.Text = text.ToString();
                                }
                                m_SyncContext.Post(callback9, "点击重试按钮重新获取二维码");
                                return;
                            }
                            timer = new Timer(600000.0);
                            timer.Elapsed += delegate
                            {
                                m_SyncContext.Post(visible => btn_retry.Visible = (bool) visible, true);
                                m_SyncContext.Post(visible => lbQRCodeInfo.Visible = (bool) visible, true);
                                m_SyncContext.Post(visible => lbSuccessInfo.Visible = (bool) visible, false);
                                m_SyncContext.Post(image => picQRCode.Image = null, null);
                                m_SyncContext.Post(text => lbQRCodeInfo.Text = text.ToString(),
                                    "点击重试按钮重新获取二维码");
                                timer.Stop();
                                timer.Close();
                                timer.Dispose();
                            };
                            timer.AutoReset = false;
                            timer.Enabled = true;
                            m_SyncContext.Post(delegate(object image)
                            {
                                picQRCode.Image = (Image) image;
                                lbSuccessInfo.Visible = true;
                                lbQRCodeInfo.Visible = false;
                            }, state);
                        }
                    };
                }
                ThreadStart start = start2.Invoke;
                new Thread(start).Start();
            }
            catch
            {
                MessageBox.Show("获取二维码失败!");
                btn_retry.Visible = true;
            }
        }
    }
}