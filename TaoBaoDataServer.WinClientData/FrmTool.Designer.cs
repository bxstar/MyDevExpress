namespace TaoBaoDataServer.WinClientData
{
    partial class FrmTool
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ultraButton1 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMd5Encrypt = new System.Windows.Forms.TextBox();
            this.btnMd5Encrypt = new System.Windows.Forms.Button();
            this.txtDecrypt = new System.Windows.Forms.TextBox();
            this.txtEncrypt = new System.Windows.Forms.TextBox();
            this.txtOrign = new System.Windows.Forms.TextBox();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDepress = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnJsonToList = new System.Windows.Forms.Button();
            this.btnCompress = new System.Windows.Forms.Button();
            this.tabPageJsonList = new System.Windows.Forms.TabPage();
            this.gridJsonList = new DevExpress.XtraGrid.GridControl();
            this.gridViewJsonList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPageJsonList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridJsonList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewJsonList)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraButton1
            // 
            this.ultraButton1.Location = new System.Drawing.Point(62, 292);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(75, 23);
            this.ultraButton1.TabIndex = 0;
            this.ultraButton1.Text = "日志输出";
            this.ultraButton1.UseVisualStyleBackColor = true;
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1011, 482);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(96, 32);
            this.txtPwd.Multiline = true;
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Size = new System.Drawing.Size(760, 144);
            this.txtPwd.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "明文密码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 231);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "密码MD5值：";
            // 
            // txtMd5Encrypt
            // 
            this.txtMd5Encrypt.Location = new System.Drawing.Point(96, 222);
            this.txtMd5Encrypt.Multiline = true;
            this.txtMd5Encrypt.Name = "txtMd5Encrypt";
            this.txtMd5Encrypt.Size = new System.Drawing.Size(760, 133);
            this.txtMd5Encrypt.TabIndex = 2;
            // 
            // btnMd5Encrypt
            // 
            this.btnMd5Encrypt.Location = new System.Drawing.Point(96, 414);
            this.btnMd5Encrypt.Name = "btnMd5Encrypt";
            this.btnMd5Encrypt.Size = new System.Drawing.Size(101, 23);
            this.btnMd5Encrypt.TabIndex = 5;
            this.btnMd5Encrypt.Text = "生成MD5密码";
            this.btnMd5Encrypt.UseVisualStyleBackColor = true;
            this.btnMd5Encrypt.Click += new System.EventHandler(this.btnMd5Encrypt_Click);
            // 
            // txtDecrypt
            // 
            this.txtDecrypt.Location = new System.Drawing.Point(84, 386);
            this.txtDecrypt.Multiline = true;
            this.txtDecrypt.Name = "txtDecrypt";
            this.txtDecrypt.Size = new System.Drawing.Size(809, 138);
            this.txtDecrypt.TabIndex = 13;
            // 
            // txtEncrypt
            // 
            this.txtEncrypt.Location = new System.Drawing.Point(84, 200);
            this.txtEncrypt.Multiline = true;
            this.txtEncrypt.Name = "txtEncrypt";
            this.txtEncrypt.Size = new System.Drawing.Size(809, 151);
            this.txtEncrypt.TabIndex = 14;
            // 
            // txtOrign
            // 
            this.txtOrign.Location = new System.Drawing.Point(84, 39);
            this.txtOrign.Multiline = true;
            this.txtOrign.Name = "txtOrign";
            this.txtOrign.Size = new System.Drawing.Size(809, 126);
            this.txtOrign.TabIndex = 15;
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(84, 357);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(75, 23);
            this.btnDecrypt.TabIndex = 12;
            this.btnDecrypt.Text = "解密";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.Location = new System.Drawing.Point(84, 171);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(75, 23);
            this.btnEncrypt.TabIndex = 11;
            this.btnEncrypt.Text = "加密";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 389);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "解密文：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 203);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "密文：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "明文：";
            // 
            // btnDepress
            // 
            this.btnDepress.Location = new System.Drawing.Point(5, 14);
            this.btnDepress.Name = "btnDepress";
            this.btnDepress.Size = new System.Drawing.Size(75, 23);
            this.btnDepress.TabIndex = 16;
            this.btnDepress.Text = "解压";
            this.btnDepress.UseVisualStyleBackColor = true;
            this.btnDepress.Click += new System.EventHandler(this.btnDepress_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPageJsonList);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1025, 567);
            this.tabControl1.TabIndex = 17;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1017, 541);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "解压或压缩字符串";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnJsonToList);
            this.panel1.Controls.Add(this.btnCompress);
            this.panel1.Controls.Add(this.btnDepress);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 485);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1011, 53);
            this.panel1.TabIndex = 17;
            // 
            // btnJsonToList
            // 
            this.btnJsonToList.Location = new System.Drawing.Point(230, 14);
            this.btnJsonToList.Name = "btnJsonToList";
            this.btnJsonToList.Size = new System.Drawing.Size(150, 23);
            this.btnJsonToList.TabIndex = 18;
            this.btnJsonToList.Text = "Json数组转为列表显示";
            this.btnJsonToList.UseVisualStyleBackColor = true;
            this.btnJsonToList.Click += new System.EventHandler(this.btnJsonToList_Click);
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(114, 14);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(75, 23);
            this.btnCompress.TabIndex = 17;
            this.btnCompress.Text = "压缩";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // tabPageJsonList
            // 
            this.tabPageJsonList.Controls.Add(this.gridJsonList);
            this.tabPageJsonList.Location = new System.Drawing.Point(4, 22);
            this.tabPageJsonList.Name = "tabPageJsonList";
            this.tabPageJsonList.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageJsonList.Size = new System.Drawing.Size(1017, 541);
            this.tabPageJsonList.TabIndex = 1;
            this.tabPageJsonList.Text = "Json列表";
            this.tabPageJsonList.UseVisualStyleBackColor = true;
            // 
            // gridJsonList
            // 
            this.gridJsonList.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridJsonList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridJsonList.Location = new System.Drawing.Point(3, 3);
            this.gridJsonList.MainView = this.gridViewJsonList;
            this.gridJsonList.Name = "gridJsonList";
            this.gridJsonList.Size = new System.Drawing.Size(1011, 535);
            this.gridJsonList.TabIndex = 1;
            this.gridJsonList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewJsonList});
            // 
            // gridViewJsonList
            // 
            this.gridViewJsonList.GridControl = this.gridJsonList;
            this.gridViewJsonList.Name = "gridViewJsonList";
            this.gridViewJsonList.OptionsSelection.MultiSelect = true;
            this.gridViewJsonList.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gridViewJsonList.OptionsView.ShowGroupPanel = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.btnMd5Encrypt);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.txtMd5Encrypt);
            this.tabPage3.Controls.Add(this.txtPwd);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1017, 541);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "MD5";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.txtDecrypt);
            this.tabPage4.Controls.Add(this.btnDecrypt);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Controls.Add(this.txtEncrypt);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.txtOrign);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.btnEncrypt);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1017, 541);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Token加密解密";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ultraButton1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(1017, 541);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "日志输出";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // FrmTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 567);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FrmTool";
            this.Text = "工具";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabPageJsonList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridJsonList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewJsonList)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ultraButton1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMd5Encrypt;
        private System.Windows.Forms.Button btnMd5Encrypt;
        private System.Windows.Forms.TextBox txtDecrypt;
        private System.Windows.Forms.TextBox txtEncrypt;
        private System.Windows.Forms.TextBox txtOrign;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDepress;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPageJsonList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Button btnJsonToList;
        private DevExpress.XtraGrid.GridControl gridJsonList;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewJsonList;
    }
}

