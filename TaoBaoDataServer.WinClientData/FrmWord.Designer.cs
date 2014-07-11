namespace TaoBaoDataServer.WinClientData
{
    partial class FrmWord
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGetWord = new System.Windows.Forms.Button();
            this.txtCreative = new System.Windows.Forms.TextBox();
            this.txtNumID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.btnGetWord);
            this.panelTop.Controls.Add(this.txtCreative);
            this.panelTop.Controls.Add(this.txtNumID);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(930, 102);
            this.panelTop.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "创意：";
            // 
            // btnGetWord
            // 
            this.btnGetWord.Location = new System.Drawing.Point(759, 26);
            this.btnGetWord.Name = "btnGetWord";
            this.btnGetWord.Size = new System.Drawing.Size(75, 23);
            this.btnGetWord.TabIndex = 2;
            this.btnGetWord.Text = "取词";
            this.btnGetWord.UseVisualStyleBackColor = true;
            this.btnGetWord.Click += new System.EventHandler(this.btnGetWord_Click);
            // 
            // txtCreative
            // 
            this.txtCreative.Location = new System.Drawing.Point(108, 59);
            this.txtCreative.Name = "txtCreative";
            this.txtCreative.Size = new System.Drawing.Size(565, 21);
            this.txtCreative.TabIndex = 1;
            // 
            // txtNumID
            // 
            this.txtNumID.Location = new System.Drawing.Point(108, 26);
            this.txtNumID.Name = "txtNumID";
            this.txtNumID.Size = new System.Drawing.Size(565, 21);
            this.txtNumID.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "宝贝ID：";
            // 
            // rtbResult
            // 
            this.rtbResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbResult.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtbResult.Location = new System.Drawing.Point(0, 102);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.Size = new System.Drawing.Size(930, 481);
            this.rtbResult.TabIndex = 1;
            this.rtbResult.Text = "";
            // 
            // FrmWord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 583);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.panelTop);
            this.Name = "FrmWord";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "取词";
            this.Load += new System.EventHandler(this.FrmWord_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnGetWord;
        private System.Windows.Forms.TextBox txtNumID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCreative;
    }
}

