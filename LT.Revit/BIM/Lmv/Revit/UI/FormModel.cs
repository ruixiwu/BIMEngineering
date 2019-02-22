namespace BIM.Lmv.Revit.UI
{
    using BIM.Lmv.Revit.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Utils;

    public class FormModel : Form
    {
        private Button btn_submit;
        private Button btnCancel;
        private ComboBox cb_stage;
        private ComboBox chb_modelType;
        private IContainer components;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label label1;
        private Label label2;
        private Label label5;
        private TextBox tx_modelName;

        public FormModel()
        {
            this.InitializeComponent();
        }

        public FormModel(string userId) : this()
        {
            TableHelp.sUser = userId;
            TableHelp.g_modelTypeNo = "";
            Dictionary<string, string> dictParam = new Dictionary<string, string> {
                { 
                    "user_id",
                    userId
                }
            };
            WebServiceClient client = new WebServiceClient();
            string str = "";
            try
            {
                str = client.Post(client.getPrjInfo, dictParam);
            }
            catch (Exception)
            {
            }
            if (!string.IsNullOrWhiteSpace(str))
            {
                Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                int startIndex = 0;
                int index = 0;
                string key = "";
                string str3 = "";
                while (true)
                {
                    startIndex = str.IndexOf("no\":\"", startIndex);
                    if (startIndex < 0)
                    {
                        break;
                    }
                    index = str.IndexOf("\"", (int) (startIndex + 5));
                    if (index < 0)
                    {
                        break;
                    }
                    key = str.Substring(startIndex + 5, (index - startIndex) - 5);
                    startIndex = str.IndexOf("name\":\"", index);
                    if (startIndex < 0)
                    {
                        break;
                    }
                    index = str.IndexOf("\"", (int) (startIndex + 7));
                    if (index < 0)
                    {
                        break;
                    }
                    str3 = str.Substring(startIndex + 7, (index - startIndex) - 7);
                    dictionary2.Add(key, str3);
                    startIndex = index;
                }
                if (dictionary2.Count > 0)
                {
                    BindingSource source = new BindingSource {
                        DataSource = dictionary2
                    };
                    this.chb_modelType.DataSource = source;
                    this.chb_modelType.ValueMember = "Key";
                    this.chb_modelType.DisplayMember = "Value";
                }
            }
            dictParam = new Dictionary<string, string>();
            str = "";
            try
            {
                str = client.Post(client.getModeType, dictParam);
            }
            catch (Exception)
            {
            }
            if (!string.IsNullOrWhiteSpace(str))
            {
                Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
                int num3 = 0;
                int num4 = 0;
                string str4 = "";
                string str5 = "";
                while (true)
                {
                    num3 = str.IndexOf("no\":\"", num3);
                    if (num3 < 0)
                    {
                        break;
                    }
                    num4 = str.IndexOf("\"", (int) (num3 + 5));
                    if (num4 < 0)
                    {
                        break;
                    }
                    str4 = str.Substring(num3 + 5, (num4 - num3) - 5);
                    num3 = str.IndexOf("name\":\"", num4);
                    if (num3 < 0)
                    {
                        break;
                    }
                    num4 = str.IndexOf("\"", (int) (num3 + 7));
                    if (num4 < 0)
                    {
                        break;
                    }
                    str5 = str.Substring(num3 + 7, (num4 - num3) - 7);
                    dictionary3.Add(str4, str5);
                    num3 = num4;
                }
                if (dictionary3.Count > 0)
                {
                    BindingSource source2 = new BindingSource {
                        DataSource = dictionary3
                    };
                    this.cb_stage.DataSource = source2;
                    this.cb_stage.ValueMember = "Key";
                    this.cb_stage.DisplayMember = "Value";
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            this.btn_submit.Enabled = false;
            string str = this.tx_modelName.Text.ToString();
            if (string.IsNullOrEmpty(str))
            {
                MessageBox.Show("请输入模型名称！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                this.btn_submit.Enabled = true;
                base.Close();
            }
            else if ((this.chb_modelType.SelectedItem == null) || (this.cb_stage.SelectedItem == null))
            {
                base.Close();
            }
            else
            {
                KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>) this.chb_modelType.SelectedItem;
                TableHelp.g_modelTypeNo = selectedItem.Key.ToString();
                KeyValuePair<string, string> pair2 = (KeyValuePair<string, string>) this.cb_stage.SelectedItem;
                TableHelp.g_stage = pair2.Key.ToString();
                TableHelp.g_modelName = str;
                base.Close();
            }
        }

        private void chb_modelType_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormModel_Load(object sender, EventArgs e)
        {
        }

        private void InitializeComponent()
        {
            this.groupBox2 = new GroupBox();
            this.cb_stage = new ComboBox();
            this.label5 = new Label();
            this.groupBox3 = new GroupBox();
            this.label2 = new Label();
            this.label1 = new Label();
            this.tx_modelName = new TextBox();
            this.chb_modelType = new ComboBox();
            this.btnCancel = new Button();
            this.btn_submit = new Button();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            base.SuspendLayout();
            this.groupBox2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.groupBox2.Controls.Add(this.cb_stage);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new Point(11, 0x1c);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(0x1a2, 0x44);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "项目信息";
            this.cb_stage.FormattingEnabled = true;
            this.cb_stage.Location = new Point(0x59, 0x21);
            this.cb_stage.Margin = new Padding(2);
            this.cb_stage.Name = "cb_stage";
            this.cb_stage.Size = new Size(0x134, 20);
            this.cb_stage.TabIndex = 3;
            this.label5.AutoSize = true;
            this.label5.Location = new Point(0x16, 0x21);
            this.label5.Margin = new Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x41, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "项目阶段：";
            this.groupBox3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.tx_modelName);
            this.groupBox3.Controls.Add(this.chb_modelType);
            this.groupBox3.Location = new Point(13, 0x77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(0x1a1, 0x73);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "模型信息";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x15, 0x4a);
            this.label2.Margin = new Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x41, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "模型名称：";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x15, 30);
            this.label1.Margin = new Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "类型名称：";
            this.tx_modelName.Location = new Point(0x58, 70);
            this.tx_modelName.Margin = new Padding(2);
            this.tx_modelName.Name = "tx_modelName";
            this.tx_modelName.Size = new Size(0x134, 0x15);
            this.tx_modelName.TabIndex = 3;
            this.tx_modelName.TextChanged += new EventHandler(this.modelName_TextChanged);
            this.chb_modelType.FormattingEnabled = true;
            this.chb_modelType.Location = new Point(0x58, 0x1c);
            this.chb_modelType.Margin = new Padding(2);
            this.chb_modelType.Name = "chb_modelType";
            this.chb_modelType.Size = new Size(0x133, 20);
            this.chb_modelType.TabIndex = 2;
            this.chb_modelType.SelectedIndexChanged += new EventHandler(this.chb_modelType_SelectedIndexChanged);
            this.btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new Point(0xeb, 0x102);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(70, 0x20);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.btn_submit.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btn_submit.Location = new Point(0x14f, 0x102);
            this.btn_submit.Name = "btn_submit";
            this.btn_submit.Size = new Size(0x5d, 0x20);
            this.btn_submit.TabIndex = 7;
            this.btn_submit.Text = "确定(&S)";
            this.btn_submit.UseVisualStyleBackColor = true;
            this.btn_submit.Click += new EventHandler(this.btnSubmit_Click);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1b6, 0x137);
            base.Controls.Add(this.btn_submit);
            base.Controls.Add(this.btnCancel);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.groupBox2);
            base.Margin = new Padding(2);
            base.Name = "FormModel";
            this.Text = "模型信息";
            base.Load += new EventHandler(this.FormModel_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            base.ResumeLayout(false);
        }

        private void modelName_TextChanged(object sender, EventArgs e)
        {
        }

        public void showRadioButton(int listSize)
        {
            int y = 6;
            int num2 = 0;
            int index = 0;
            new GroupBox();
            RadioButton[] buttonArray = new RadioButton[listSize];
            for (index = 0; index < listSize; index++)
            {
                if (((index % 4) == 0) && (index != 0))
                {
                    y += 20;
                    num2 = 0;
                }
                buttonArray[index] = new RadioButton();
                buttonArray[index].AutoSize = true;
                buttonArray[index].Top = y;
                buttonArray[index].Location = new Point((num2 * 150) + 2, y);
                buttonArray[index].Text = index.ToString();
                buttonArray[index].Visible = true;
                buttonArray[index].Name = "radioButton" + index;
                this.groupBox2.Controls.Add(buttonArray[index]);
                num2++;
            }
        }
    }
}

