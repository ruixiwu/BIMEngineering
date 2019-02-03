using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BIM.Lmv.Revit.Helpers;
using Utils;

namespace BIM.Lmv.Revit.UI
{
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
            InitializeComponent();
        }

        public FormModel(string userId) : this()
        {
            TableHelp.sUser = userId;
            TableHelp.g_modelTypeNo = "";
            var dictParam = new Dictionary<string, string>
            {
                {
                    "user_id",
                    userId
                }
            };
            var client = new WebServiceClient();
            var str = "";
            try
            {
                str = client.Post(client.getPrjInfo, dictParam);//从朗坤服务器端,根据用户名称user_id获得项目信息
            }
            catch (Exception)
            {
            }
            if (!string.IsNullOrWhiteSpace(str))//若str不为空，则解析出项目名称列表
            {
                var dictionary2 = new Dictionary<string, string>();
                var startIndex = 0;
                var index = 0;
                var key = "";
                var str3 = "";
                while (true)
                {
                    startIndex = str.IndexOf("no\":\"", startIndex);
                    if (startIndex < 0)
                    {
                        break;
                    }
                    index = str.IndexOf("\"", startIndex + 5);
                    if (index < 0)
                    {
                        break;
                    }
                    key = str.Substring(startIndex + 5, index - startIndex - 5);
                    startIndex = str.IndexOf("name\":\"", index);
                    if (startIndex < 0)
                    {
                        break;
                    }
                    index = str.IndexOf("\"", startIndex + 7);
                    if (index < 0)
                    {
                        break;
                    }
                    str3 = str.Substring(startIndex + 7, index - startIndex - 7);
                    dictionary2.Add(key, str3);
                    startIndex = index;
                }
                if (dictionary2.Count > 0)
                {
                    var source = new BindingSource
                    {
                        DataSource = dictionary2
                    };
                    chb_modelType.DataSource = source;
                    chb_modelType.ValueMember = "Key";
                    chb_modelType.DisplayMember = "Value";
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
            if (!string.IsNullOrWhiteSpace(str))//str根据项目名称，解析出项目的阶段？？
            {
                var dictionary3 = new Dictionary<string, string>();
                var num3 = 0;
                var num4 = 0;
                var str4 = "";
                var str5 = "";
                while (true)
                {
                    num3 = str.IndexOf("no\":\"", num3);
                    if (num3 < 0)
                    {
                        break;
                    }
                    num4 = str.IndexOf("\"", num3 + 5);
                    if (num4 < 0)
                    {
                        break;
                    }
                    str4 = str.Substring(num3 + 5, num4 - num3 - 5);
                    num3 = str.IndexOf("name\":\"", num4);
                    if (num3 < 0)
                    {
                        break;
                    }
                    num4 = str.IndexOf("\"", num3 + 7);
                    if (num4 < 0)
                    {
                        break;
                    }
                    str5 = str.Substring(num3 + 7, num4 - num3 - 7);
                    dictionary3.Add(str4, str5);
                    num3 = num4;
                }
                if (dictionary3.Count > 0)
                {
                    var source2 = new BindingSource
                    {
                        DataSource = dictionary3
                    };
                    cb_stage.DataSource = source2;
                    cb_stage.ValueMember = "Key";
                    cb_stage.DisplayMember = "Value";
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {//单击确定开始转换模型了
            btn_submit.Enabled = false;
            var str = tx_modelName.Text;//模型名称，tx_modelName是模型信息对话框中的值
                                        //if (str == "")
                                        //{//转换之前， 对模型的信息输入的条件检查
                                        //    MessageBox.Show("请输入模型名称！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                                        //    btn_submit.Enabled = true;
                                        //}
                                        //else
                                        //{
                                        //    var selectedItem =(KeyValuePair<string, string>) chb_modelType.SelectedItem;//chb_modelType是项目名称控件，需要从下拉列表中选择一项
                                        //    TableHelp.g_modelTypeNo = selectedItem.Key;
                                        //    var pair2 = (KeyValuePair<string, string>) cb_stage.SelectedItem; //cb_stage是模型设计阶段
                                        //    TableHelp.g_stage = pair2.Key;
                                        //    TableHelp.g_modelName = str;
                                        //    Close();
                                        //}

            //以下为测试目的制作的数据
            TableHelp.g_modelTypeNo = "testProject";
            TableHelp.g_stage = "设计阶段";
            TableHelp.g_modelName = "testModel";
            Close();
        }

        private void chb_modelType_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FormModel_Load(object sender, EventArgs e)
        {
        }

        private void InitializeComponent()
        {
            groupBox2 = new GroupBox();
            cb_stage = new ComboBox();
            label5 = new Label();
            groupBox3 = new GroupBox();
            label2 = new Label();
            label1 = new Label();
            tx_modelName = new TextBox();
            chb_modelType = new ComboBox();
            btnCancel = new Button();
            btn_submit = new Button();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            groupBox2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            groupBox2.Controls.Add(cb_stage);
            groupBox2.Controls.Add(label5);
            groupBox2.Location = new Point(11, 0x1c);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(0x1a2, 0x44);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "项目信息";
            cb_stage.FormattingEnabled = true;
            cb_stage.Location = new Point(0x59, 0x21);
            cb_stage.Margin = new Padding(2);
            cb_stage.Name = "cb_stage";
            cb_stage.Size = new Size(0x134, 20);
            cb_stage.TabIndex = 3;
            label5.AutoSize = true;
            label5.Location = new Point(0x16, 0x21);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(0x41, 12);
            label5.TabIndex = 2;
            label5.Text = "项目阶段：";
            groupBox3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(tx_modelName);
            groupBox3.Controls.Add(chb_modelType);
            groupBox3.Location = new Point(13, 0x77);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(0x1a1, 0x73);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "模型信息";
            label2.AutoSize = true;
            label2.Location = new Point(0x15, 0x4a);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(0x41, 12);
            label2.TabIndex = 5;
            label2.Text = "模型名称：";
            label1.AutoSize = true;
            label1.Location = new Point(0x15, 30);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(0x41, 12);
            label1.TabIndex = 4;
            label1.Text = "类型名称：";
            tx_modelName.Location = new Point(0x58, 70);
            tx_modelName.Margin = new Padding(2);
            tx_modelName.Name = "tx_modelName";
            tx_modelName.Size = new Size(0x134, 0x15);
            tx_modelName.TabIndex = 3;
            tx_modelName.TextChanged += modelName_TextChanged;
            chb_modelType.FormattingEnabled = true;
            chb_modelType.Location = new Point(0x58, 0x1c);
            chb_modelType.Margin = new Padding(2);
            chb_modelType.Name = "chb_modelType";
            chb_modelType.Size = new Size(0x133, 20);
            chb_modelType.TabIndex = 2;
            chb_modelType.SelectedIndexChanged += chb_modelType_SelectedIndexChanged;
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(0xeb, 0x102);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(70, 0x20);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "取消(&C)";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            btn_submit.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btn_submit.Location = new Point(0x14f, 0x102);
            btn_submit.Name = "btn_submit";
            btn_submit.Size = new Size(0x5d, 0x20);
            btn_submit.TabIndex = 7;
            btn_submit.Text = "确定(&S)";
            btn_submit.UseVisualStyleBackColor = true;
            btn_submit.Click += btnSubmit_Click;
            AutoScaleDimensions = new SizeF(6f, 12f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(0x1b6, 0x137);
            Controls.Add(btn_submit);
            Controls.Add(btnCancel);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Margin = new Padding(2);
            Name = "FormModel";
            Text = "模型信息";
            Load += FormModel_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        private void modelName_TextChanged(object sender, EventArgs e)
        {
        }

        public void showRadioButton(int listSize)
        {
            var y = 6;
            var num2 = 0;
            var index = 0;
            new GroupBox();
            var buttonArray = new RadioButton[listSize];
            for (index = 0; index < listSize; index++)
            {
                if ((index%4 == 0) && (index != 0))
                {
                    y += 20;
                    num2 = 0;
                }
                buttonArray[index] = new RadioButton();
                buttonArray[index].AutoSize = true;
                buttonArray[index].Top = y;
                buttonArray[index].Location = new Point(num2*150 + 2, y);
                buttonArray[index].Text = index.ToString();
                buttonArray[index].Visible = true;
                buttonArray[index].Name = "radioButton" + index;
                groupBox2.Controls.Add(buttonArray[index]);
                num2++;
            }
        }
    }
}