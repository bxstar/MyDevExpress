namespace TaoBaoDataServer.WinClientData
{
    partial class FrmFindWord
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
            this.btnWant = new System.Windows.Forms.Button();
            this.txtWord = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearchList = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCreativeOne = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCreativeTwo = new System.Windows.Forms.TextBox();
            this.btnGetCreative = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnWant
            // 
            this.btnWant.Location = new System.Drawing.Point(661, 47);
            this.btnWant.Name = "btnWant";
            this.btnWant.Size = new System.Drawing.Size(174, 23);
            this.btnWant.TabIndex = 0;
            this.btnWant.Text = "你是不是想找的词";
            this.btnWant.UseVisualStyleBackColor = true;
            this.btnWant.Click += new System.EventHandler(this.btnFindWord_Click);
            // 
            // txtWord
            // 
            this.txtWord.Location = new System.Drawing.Point(61, 12);
            this.txtWord.Name = "txtWord";
            this.txtWord.Size = new System.Drawing.Size(560, 21);
            this.txtWord.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "拓展词：";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(61, 49);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(560, 293);
            this.txtResult.TabIndex = 3;
            this.txtResult.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "结果：";
            // 
            // btnSearchList
            // 
            this.btnSearchList.Location = new System.Drawing.Point(661, 100);
            this.btnSearchList.Name = "btnSearchList";
            this.btnSearchList.Size = new System.Drawing.Size(75, 23);
            this.btnSearchList.TabIndex = 5;
            this.btnSearchList.Text = "下拉框";
            this.btnSearchList.UseVisualStyleBackColor = true;
            this.btnSearchList.Click += new System.EventHandler(this.btnSearchList_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 408);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "创意一：";
            // 
            // txtCreativeOne
            // 
            this.txtCreativeOne.Location = new System.Drawing.Point(61, 405);
            this.txtCreativeOne.Name = "txtCreativeOne";
            this.txtCreativeOne.Size = new System.Drawing.Size(560, 21);
            this.txtCreativeOne.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 438);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "创意二：";
            // 
            // txtCreativeTwo
            // 
            this.txtCreativeTwo.Location = new System.Drawing.Point(61, 435);
            this.txtCreativeTwo.Name = "txtCreativeTwo";
            this.txtCreativeTwo.Size = new System.Drawing.Size(560, 21);
            this.txtCreativeTwo.TabIndex = 1;
            // 
            // btnGetCreative
            // 
            this.btnGetCreative.Location = new System.Drawing.Point(661, 408);
            this.btnGetCreative.Name = "btnGetCreative";
            this.btnGetCreative.Size = new System.Drawing.Size(75, 23);
            this.btnGetCreative.TabIndex = 8;
            this.btnGetCreative.Text = "生成创意";
            this.btnGetCreative.UseVisualStyleBackColor = true;
            this.btnGetCreative.Click += new System.EventHandler(this.btnGetCreative_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(61, 371);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(560, 21);
            this.textBox1.TabIndex = 9;
            // 
            // FrmFindWord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 481);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnGetCreative);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSearchList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCreativeTwo);
            this.Controls.Add(this.txtCreativeOne);
            this.Controls.Add(this.txtWord);
            this.Controls.Add(this.btnWant);
            this.Name = "FrmFindWord";
            this.Text = "取词测试";
            this.Load += new System.EventHandler(this.FrmFindWord_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWant;
        private System.Windows.Forms.TextBox txtWord;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearchList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCreativeOne;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCreativeTwo;
        private System.Windows.Forms.Button btnGetCreative;
        private System.Windows.Forms.TextBox textBox1;
    }
}