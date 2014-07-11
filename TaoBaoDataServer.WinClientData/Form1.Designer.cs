namespace TaoBaoDataServer.WinClientData
{
    partial class Form1
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
            this.SuspendLayout();
            // 
            // ultraButton1
            // 
            this.ultraButton1.Location = new System.Drawing.Point(743, 61);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(75, 23);
            this.ultraButton1.TabIndex = 0;
            this.ultraButton1.Text = "日志输出";
            this.ultraButton1.UseVisualStyleBackColor = true;
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(55, 61);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(592, 353);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(126, 466);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Size = new System.Drawing.Size(166, 21);
            this.txtPwd.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 469);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "明文密码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(356, 469);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "密码MD5值：";
            // 
            // txtMd5Encrypt
            // 
            this.txtMd5Encrypt.Location = new System.Drawing.Point(433, 466);
            this.txtMd5Encrypt.Name = "txtMd5Encrypt";
            this.txtMd5Encrypt.Size = new System.Drawing.Size(214, 21);
            this.txtMd5Encrypt.TabIndex = 2;
            // 
            // btnMd5Encrypt
            // 
            this.btnMd5Encrypt.Location = new System.Drawing.Point(743, 464);
            this.btnMd5Encrypt.Name = "btnMd5Encrypt";
            this.btnMd5Encrypt.Size = new System.Drawing.Size(101, 23);
            this.btnMd5Encrypt.TabIndex = 5;
            this.btnMd5Encrypt.Text = "生成MD5密码";
            this.btnMd5Encrypt.UseVisualStyleBackColor = true;
            this.btnMd5Encrypt.Click += new System.EventHandler(this.btnMd5Encrypt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 612);
            this.Controls.Add(this.btnMd5Encrypt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMd5Encrypt);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.ultraButton1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ultraButton1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMd5Encrypt;
        private System.Windows.Forms.Button btnMd5Encrypt;
    }
}

