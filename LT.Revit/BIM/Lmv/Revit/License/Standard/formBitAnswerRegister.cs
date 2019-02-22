namespace BIM.Lmv.Revit.License.Standard
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Net;
    using System.Windows.Forms;

    internal class formBitAnswerRegister : Form
    {
        private string _xmlScope;
        public bool bCancel;
        public bool bSucess;
        private Button btnA;
        private Button btnCancel;
        private Button btnOK;
        private Button btnTest;
        private IContainer components;
        public uint featureId;
        private GroupBox groupBox1;
        private Label label1;
        private Label label2;
        private RadioButton radType1;
        private RadioButton radType2;
        public string strHost = "";
        private TextBox txtIP;
        private TextBox txtLicenseCode;

        public formBitAnswerRegister()
        {
            this.InitializeComponent();
        }

        private void btnA_Click(object sender, EventArgs e)
        {
            if (this.txtLicenseCode.Text.Trim() == "")
            {
                MessageBox.Show("请输入有效的授权码！");
            }
            else
            {
                BitAnswer answer = new BitAnswer();
                answer.UpdateOnline(null, this.txtLicenseCode.Text);
                if (answer.Status == 0)
                {
                    this.bSucess = true;
                    MessageBox.Show("在线注册成功！");
                }
                else
                {
                    MessageBox.Show("在线注册失败！");
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.bCancel = true;
            base.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.bCancel = false;
            this.bSucess = true;
            if (this.radType1.Checked)
            {
                this.strHost = "localhost";
            }
            else
            {
                this.strHost = this.txtIP.Text.Trim();
            }
            base.Hide();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            BitAnswer answer = new BitAnswer();
            if (this.txtIP.Text.Trim() != "")
            {
                answer.SetLocalServer(this.txtIP.Text.Trim(), 0x2051, 10);
                LoginMode remote = LoginMode.Remote;
                if (answer.LoginEx(null, null, this.featureId, this._xmlScope, remote))
                {
                    answer.Logout();
                    this.bSucess = true;
                    MessageBox.Show("连接成功！");
                }
                else
                {
                    MessageBox.Show("连接失败，请和系统管理员联系！");
                }
            }
            else
            {
                MessageBox.Show("请输入集团授权服务器的主机名或IP地址！");
            }
        }

        private void ChangeDisplay()
        {
            this.btnA.Enabled = this.radType1.Checked;
            this.txtLicenseCode.Enabled = this.radType1.Checked;
            this.btnTest.Enabled = !this.radType1.Checked;
            this.txtIP.Enabled = !this.radType1.Checked;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void formBitAnswerRegister_Load(object sender, EventArgs e)
        {
            if ((this.strHost != "") && (this.strHost.ToLower() != "localhost"))
            {
                IPAddress address;
                if (IPAddress.TryParse(this.strHost, out address))
                {
                    this.strHost = address.ToString();
                    this.txtIP.Text = this.strHost;
                    this.radType1.Checked = false;
                    this.radType2.Checked = true;
                    this.ChangeDisplay();
                }
                else
                {
                    this.strHost = "";
                }
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(formBitAnswerRegister));
            this.btnCancel = new Button();
            this.btnOK = new Button();
            this.groupBox1 = new GroupBox();
            this.btnA = new Button();
            this.btnTest = new Button();
            this.label2 = new Label();
            this.txtIP = new TextBox();
            this.radType2 = new RadioButton();
            this.radType1 = new RadioButton();
            this.txtLicenseCode = new TextBox();
            this.label1 = new Label();
            this.groupBox1.SuspendLayout();
            base.SuspendLayout();
            this.btnCancel.Location = new Point(0x11e, 0xc6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(0x4b, 0x17);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.btnOK.Location = new Point(0xcd, 0xc6);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(0x4b, 0x17);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            this.groupBox1.Controls.Add(this.btnA);
            this.groupBox1.Controls.Add(this.btnTest);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtIP);
            this.groupBox1.Controls.Add(this.radType2);
            this.groupBox1.Controls.Add(this.radType1);
            this.groupBox1.Controls.Add(this.txtLicenseCode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x15d, 180);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "授权方式";
            this.btnA.Location = new Point(0xf5, 0x27);
            this.btnA.Name = "btnA";
            this.btnA.Size = new Size(0x56, 0x17);
            this.btnA.TabIndex = 5;
            this.btnA.Text = "在线激活(&A)";
            this.btnA.UseVisualStyleBackColor = true;
            this.btnA.Click += new EventHandler(this.btnA_Click);
            this.btnTest.Enabled = false;
            this.btnTest.Location = new Point(0xf5, 0x73);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new Size(0x56, 0x17);
            this.btnTest.TabIndex = 5;
            this.btnTest.Text = "测试(&T)";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new EventHandler(this.btnTest_Click);
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x19, 0x7e);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0xdd, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "指定集团授权服务器的主机名或IP地址：";
            this.txtIP.Enabled = false;
            this.txtIP.Location = new Point(0x1b, 0x93);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new Size(0x130, 0x15);
            this.txtIP.TabIndex = 6;
            this.radType2.AutoSize = true;
            this.radType2.Location = new Point(7, 0x63);
            this.radType2.Name = "radType2";
            this.radType2.Size = new Size(0x47, 0x10);
            this.radType2.TabIndex = 5;
            this.radType2.Text = "集团授权";
            this.radType2.UseVisualStyleBackColor = true;
            this.radType2.CheckedChanged += new EventHandler(this.radType2_CheckedChanged);
            this.radType1.AutoSize = true;
            this.radType1.Checked = true;
            this.radType1.Location = new Point(7, 0x15);
            this.radType1.Name = "radType1";
            this.radType1.Size = new Size(0x6b, 0x10);
            this.radType1.TabIndex = 4;
            this.radType1.TabStop = true;
            this.radType1.Text = "单机或浮动授权";
            this.radType1.UseVisualStyleBackColor = true;
            this.radType1.CheckedChanged += new EventHandler(this.radType2_CheckedChanged);
            this.txtLicenseCode.Location = new Point(0x1b, 0x44);
            this.txtLicenseCode.Name = "txtLicenseCode";
            this.txtLicenseCode.Size = new Size(0x130, 0x15);
            this.txtLicenseCode.TabIndex = 3;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x19, 0x2f);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x35, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "授权码：";
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x173, 230);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.btnOK);
            base.Controls.Add(this.btnCancel);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "formBitAnswerRegister";
            base.StartPosition = FormStartPosition.CenterParent;
            this.Text = "软件授权";
            base.Load += new EventHandler(this.formBitAnswerRegister_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            base.ResumeLayout(false);
        }

        private void radType1_CheckedChanged(object sender, EventArgs e)
        {
            this.ChangeDisplay();
        }

        private void radType2_CheckedChanged(object sender, EventArgs e)
        {
            this.ChangeDisplay();
        }
    }
}

