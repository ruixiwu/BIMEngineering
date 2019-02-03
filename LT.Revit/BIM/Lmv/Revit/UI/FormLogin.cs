using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BIM.Lmv.Revit.Helpers;
using Utils;

namespace BIM.Lmv.Revit.UI
{
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
            InitializeComponent();
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            TableHelp.sUser = "";
            var str = tx_userName.Text;
            var str2 = tx_password.Text;
            btn_submit.Enabled = false;
            if ((str == "") || (str2 == ""))
            {
                MessageBox.Show("请输入用户名或密码！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                btn_submit.Enabled = true;
            }
            else
            {
                var dictParam = new Dictionary<string, string>
                {
                    {
                        "phone",
                        str
                    }
                };
                var str3 = "";
                var client = new WebServiceClient();
                try
                {
                    str3 = client.Post(client.GETUSERIDBYPHONE, dictParam);
                }
                catch (Exception)
                {
                    MessageBox.Show("服务无法连接！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    btn_submit.Enabled = true;
                    return;
                }
                if (!string.IsNullOrWhiteSpace(str3))
                {
                    var str4 = new MD5Convertor().MD5Encrypt32(str3 + str2);
                    dictParam = new Dictionary<string, string>
                    {
                        {
                            "userId",
                            str3
                        },
                        {
                            "md5Str",
                            str4
                        }
                    };
                    var str5 = "";
                    try
                    {
                        str5 = client.Post(client.CHECKUSERLOGIN, dictParam);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("服务无法连接！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                        btn_submit.Enabled = true;
                        return;
                    }

                    MessageBox.Show("登录成功", "消息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btn_submit.Enabled = false;
                    Close();
                   // TableHelp.sUser = str3;
                    TableHelp.sUser = "testuser";//作为测试用户
                    //if (str5 != "true")
                    //{
                    //    MessageBox.Show("您输入的用户名密码错误！", "消息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //    tx_userName.Text = "";
                    //    tx_password.Text = "";
                    //    tx_userName.Focus();
                    //    btn_submit.Enabled = true;
                    //}
                    //else
                    //{
                    //    MessageBox.Show("登录成功", "消息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    //    btn_submit.Enabled = false;
                    //    Close();
                    //    TableHelp.sUser = str3;
                    //}
                }
                else
                {
                    MessageBox.Show("用户名不存在！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    btn_submit.Enabled = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
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

        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            tx_password = new TextBox();
            tx_userName = new TextBox();
            label4 = new Label();
            label3 = new Label();
            btn_submit = new Button();
            btnCancel = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            groupBox1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            groupBox1.Controls.Add(tx_password);
            groupBox1.Controls.Add(tx_userName);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(0x1f, 0x1f);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(0x16e, 0x81);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "用户信息";
            tx_password.Location = new Point(0x68, 0x53);
            tx_password.Name = "tx_password";
            tx_password.PasswordChar = '*';
            tx_password.Size = new Size(0xd4, 0x19);
            tx_password.TabIndex = 5;
            tx_userName.Location = new Point(0x68, 0x2b);
            tx_userName.Margin = new Padding(4);
            tx_userName.MaxLength = 0x1000;
            tx_userName.Name = "tx_userName";
            tx_userName.Size = new Size(0xd4, 0x19);
            tx_userName.TabIndex = 1;
            label4.AutoSize = true;
            label4.Location = new Point(0x24, 0x2e);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(60, 15);
            label4.TabIndex = 0;
            label4.Text = "用户名:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            label3.AutoSize = true;
            label3.Location = new Point(0x24, 0x58);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(0x3d, 15);
            label3.TabIndex = 2;
            label3.Text = "密  码:";
            label3.TextAlign = ContentAlignment.MiddleRight;
            btn_submit.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btn_submit.DialogResult = DialogResult.OK;
            btn_submit.Location = new Point(0x111, 0xc6);
            btn_submit.Margin = new Padding(4);
            btn_submit.Name = "btn_submit";
            btn_submit.Size = new Size(0x7c, 40);
            btn_submit.TabIndex = 9;
            btn_submit.Text = "登录";
            btn_submit.UseVisualStyleBackColor = true;
            btn_submit.Click += btn_submit_Click;
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(0x94, 0xc6);
            btnCancel.Margin = new Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(0x5e, 40);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            AutoScaleDimensions = new SizeF(8f, 15f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(0x1b1, 0x10a);
            Controls.Add(btn_submit);
            Controls.Add(btnCancel);
            Controls.Add(groupBox1);
            Name = "FormLogin";
            Text = "用户登录";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }
    }
}