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
        private Button btn_Submit;
        private Button btn_Cancel;
        private ComboBox chb_workspace;//项目名称
        private IContainer components;
        private GroupBox groupBox3;
        private Label label1;
        private Label label2;
        private TextBox tx_modelName;//模型名称

        public FormModel()
        {
            this.InitializeComponent();
        }

        public FormModel(string userId) : this()
        {
            TableHelp.sUser = userId;
            TableHelp.g_workspaceId = "";
            Dictionary<string, string> dictParam = new Dictionary<string, string> {
                { 
                    "userId",
                    userId
                }
            };
            WebServiceClient client = new WebServiceClient();
            string str = "";
            try
            {//获得工作空间信息
                str = client.Post(client.getWrkInfo, dictParam);
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
                    this.chb_workspace.DataSource = source;
                    this.chb_workspace.ValueMember = "Key";
                    this.chb_workspace.DisplayMember = "Value";
                }
            }


            dictParam = new Dictionary<string, string>();
            str = "";
            try
            {////返回str解析出项目的阶段？？
                str = client.Post(client.getModeType, dictParam);
            }
            catch (Exception)
            {
            }
            //if (!string.IsNullOrWhiteSpace(str))
            //{
            //    Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
            //    int num3 = 0;
            //    int num4 = 0;
            //    string str4 = "";
            //    string str5 = "";
            //    while (true)
            //    {
            //        num3 = str.IndexOf("no\":\"", num3);
            //        if (num3 < 0)
            //        {
            //            break;
            //        }
            //        num4 = str.IndexOf("\"", (int) (num3 + 5));
            //        if (num4 < 0)
            //        {
            //            break;
            //        }
            //        str4 = str.Substring(num3 + 5, (num4 - num3) - 5);
            //        num3 = str.IndexOf("name\":\"", num4);
            //        if (num3 < 0)
            //        {
            //            break;
            //        }
            //        num4 = str.IndexOf("\"", (int) (num3 + 7));
            //        if (num4 < 0)
            //        {
            //            break;
            //        }
            //        str5 = str.Substring(num3 + 7, (num4 - num3) - 7);
            //        dictionary3.Add(str4, str5);
            //        num3 = num4;
            //    }
            //    if (dictionary3.Count > 0)
            //    {
            //        BindingSource source2 = new BindingSource {
            //            DataSource = dictionary3
            //        };
            //        this.cb_stage.DataSource = source2;
            //        this.cb_stage.ValueMember = "Key";
            //        this.cb_stage.DisplayMember = "Value";
            //    }
            //}
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            this.btn_Submit.Enabled = false;
            string modelName = this.tx_modelName.Text.ToString();
            if (string.IsNullOrEmpty(modelName))
            {
                MessageBox.Show("请输入模型名称！", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                this.btn_Submit.Enabled = true;
                base.Close();
            }
            else if (this.chb_workspace.SelectedItem == null)
            {
                base.Close();
            }
            else
            {
                KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>) this.chb_workspace.SelectedItem;
                TableHelp.g_workspaceId = selectedItem.Key.ToString();
               // KeyValuePair<string, string> pair2 = (KeyValuePair<string, string>) this.cb_stage.SelectedItem;
               // TableHelp.g_stage = pair2.Key.ToString();
                TableHelp.g_modelName = modelName;
                base.Close();
            }
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tx_modelName = new System.Windows.Forms.TextBox();
            this.chb_workspace = new System.Windows.Forms.ComboBox();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Submit = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.tx_modelName);
            this.groupBox3.Controls.Add(this.chb_workspace);
            this.groupBox3.Location = new System.Drawing.Point(17, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(539, 138);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "模型信息";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "模型名称：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "项目名称：";
            // 
            // tx_modelName
            // 
            this.tx_modelName.Location = new System.Drawing.Point(117, 88);
            this.tx_modelName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tx_modelName.Name = "tx_modelName";
            this.tx_modelName.Size = new System.Drawing.Size(409, 25);
            this.tx_modelName.TabIndex = 3;
            // 
            // chb_workspace
            // 
            this.chb_workspace.FormattingEnabled = true;
            this.chb_workspace.Location = new System.Drawing.Point(117, 35);
            this.chb_workspace.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chb_workspace.Name = "chb_workspace";
            this.chb_workspace.Size = new System.Drawing.Size(408, 23);
            this.chb_workspace.TabIndex = 2;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(262, 156);
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(93, 40);
            this.btn_Cancel.TabIndex = 6;
            this.btn_Cancel.Text = "取消(&C)";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btn_Submit
            // 
            this.btn_Submit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Submit.Location = new System.Drawing.Point(396, 156);
            this.btn_Submit.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Submit.Name = "btn_Submit";
            this.btn_Submit.Size = new System.Drawing.Size(124, 40);
            this.btn_Submit.TabIndex = 7;
            this.btn_Submit.Text = "确定(&S)";
            this.btn_Submit.UseVisualStyleBackColor = true;
            this.btn_Submit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // FormModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 209);
            this.Controls.Add(this.btn_Submit);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.groupBox3);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormModel";
            this.Text = "模型信息";
            this.Load += new System.EventHandler(this.FormModel_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        //public void showRadioButton(int listSize)
        //{
        //    int y = 6;
        //    int num2 = 0;
        //    int index = 0;
        //    new GroupBox();
        //    RadioButton[] buttonArray = new RadioButton[listSize];
        //    for (index = 0; index < listSize; index++)
        //    {
        //        if (((index % 4) == 0) && (index != 0))
        //        {
        //            y += 20;
        //            num2 = 0;
        //        }
        //        buttonArray[index] = new RadioButton();
        //        buttonArray[index].AutoSize = true;
        //        buttonArray[index].Top = y;
        //        buttonArray[index].Location = new Point((num2 * 150) + 2, y);
        //        buttonArray[index].Text = index.ToString();
        //        buttonArray[index].Visible = true;
        //        buttonArray[index].Name = "radioButton" + index;
        //        //this.groupBox2.Controls.Add(buttonArray[index]);
        //        num2++;
        //    }
        //}
    }
}

