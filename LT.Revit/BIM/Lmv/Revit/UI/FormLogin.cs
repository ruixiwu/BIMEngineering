namespace BIM.Lmv.Revit.UI
{
    using BIM.Lmv.Revit.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Utils;

    public class FormLogin : Form
    {
        private Button btn_submit;
        private Button btnCancel;
        private IContainer components;
        private GroupBox groupBox1;
        private Label label3;
        private Label label4;
        private TextBox tx_password;
        private TextBox tx_userName;

        public FormLogin()
        {
            this.InitializeComponent();
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            TableHelp.sUser = "";
            string str = this.tx_userName.Text.ToString();
            string str2 = this.tx_password.Text.ToString();
            this.btn_submit.Enabled = false;
            if ((str == "") || (str2 == ""))
            {
                MessageBox.Show("请输入用户名或密码！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                this.btn_submit.Enabled = true;
            }
            else
            {
                Dictionary<string, string> dictParam = new Dictionary<string, string> {
                    { 
                        "phone",
                        str
                    }
                };
                string str3 = "";
                WebServiceClient client = new WebServiceClient();
                try
                {
                    str3 = client.Post(client.GETUSERIDBYPHONE, dictParam);
                }
                catch (Exception)
                {
                    MessageBox.Show("服务无法连接！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    this.btn_submit.Enabled = true;
                    return;
                }
                if (!string.IsNullOrWhiteSpace(str3))
                {
                    string str4 = new MD5Convertor().MD5Encrypt32(str3 + str2);
                    dictParam = new Dictionary<string, string> {
                        { 
                            "userId",
                            str3
                        },
                        { 
                            "md5Str",
                            str4
                        }
                    };
                    string str5 = "";
                    try
                    {
                        str5 = client.Post(client.CHECKUSERLOGIN, dictParam);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("服务无法连接！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                        this.btn_submit.Enabled = true;
                        return;
                    }
                    if (str5 != "true")
                    {
                        MessageBox.Show("您输入的用户名密码错误！", "消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.tx_userName.Text = "";
                        this.tx_password.Text = "";
                        this.tx_userName.Focus();
                        this.btn_submit.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("登录成功", "消息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        this.btn_submit.Enabled = false;
                        base.Close();
                        TableHelp.sUser = str3;
                    }
                }
                else
                {
                    MessageBox.Show("用户名不存在！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    this.btn_submit.Enabled = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
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
            this.groupBox1 = new GroupBox();
            this.tx_password = new TextBox();
            this.tx_userName = new TextBox();
            this.label4 = new Label();
            this.label3 = new Label();
            this.btn_submit = new Button();
            this.btnCancel = new Button();
            this.groupBox1.SuspendLayout();
            base.SuspendLayout();
            this.groupBox1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.groupBox1.Controls.Add(this.tx_password);
            this.groupBox1.Controls.Add(this.tx_userName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new Point(0x1f, 0x1f);
            this.groupBox1.Margin = new Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new Padding(4);
            this.groupBox1.Size = new Size(0x16e, 0x81);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "用户信息";
            this.tx_password.Location = new Point(0x68, 0x53);
            this.tx_password.Name = "tx_password";
            this.tx_password.PasswordChar = '*';
            this.tx_password.Size = new Size(0xd4, 0x19);
            this.tx_password.TabIndex = 5;
            this.tx_userName.Location = new Point(0x68, 0x2b);
            this.tx_userName.Margin = new Padding(4);
            this.tx_userName.MaxLength = 0x1000;
            this.tx_userName.Name = "tx_userName";
            this.tx_userName.Size = new Size(0xd4, 0x19);
            this.tx_userName.TabIndex = 1;
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0x24, 0x2e);
            this.label4.Margin = new Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(60, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "用户名:";
            this.label4.TextAlign = ContentAlignment.MiddleRight;
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0x24, 0x58);
            this.label3.Margin = new Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x3d, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "密  码:";
            this.label3.TextAlign = ContentAlignment.MiddleRight;
            this.btn_submit.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btn_submit.DialogResult = DialogResult.OK;
            this.btn_submit.Location = new Point(0x111, 0xc6);
            this.btn_submit.Margin = new Padding(4);
            this.btn_submit.Name = "btn_submit";
            this.btn_submit.Size = new Size(0x7c, 40);
            this.btn_submit.TabIndex = 9;
            this.btn_submit.Text = "登录";
            this.btn_submit.UseVisualStyleBackColor = true;
            this.btn_submit.Click += new EventHandler(this.btn_submit_Click);
            this.btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(0x94, 0xc6);
            this.btnCancel.Margin = new Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(0x5e, 40);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            base.AutoScaleDimensions = new SizeF(8f, 15f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1b1, 0x10a);
            base.Controls.Add(this.btn_submit);
            base.Controls.Add(this.btnCancel);
            base.Controls.Add(this.groupBox1);
            base.Name = "FormLogin";
            this.Text = "用户登录";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            base.ResumeLayout(false);
        }
    }
}

