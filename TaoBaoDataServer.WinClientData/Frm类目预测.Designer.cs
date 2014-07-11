namespace TaoBaoDataServer.WinClientData
{
    partial class Frm类目预测
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnBatchGet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(25, 28);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(370, 385);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(485, 111);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "预测";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnBatchGet
            // 
            this.btnBatchGet.Location = new System.Drawing.Point(485, 41);
            this.btnBatchGet.Name = "btnBatchGet";
            this.btnBatchGet.Size = new System.Drawing.Size(75, 23);
            this.btnBatchGet.TabIndex = 2;
            this.btnBatchGet.Text = "防遗漏预测";
            this.btnBatchGet.UseVisualStyleBackColor = true;
            this.btnBatchGet.Click += new System.EventHandler(this.btnBatchGet_Click);
            // 
            // Frm类目预测
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 539);
            this.Controls.Add(this.btnBatchGet);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Frm类目预测";
            this.Text = "Frm类目预测";
            this.Load += new System.EventHandler(this.Frm类目预测_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnBatchGet;
    }
}