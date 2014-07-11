namespace TaoBaoDataServer.WinClientData
{
    partial class FrmDataBaseTest
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAdgroupRevert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(118, 268);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "关键词改价";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // btnAdgroupRevert
            // 
            this.btnAdgroupRevert.Location = new System.Drawing.Point(296, 267);
            this.btnAdgroupRevert.Name = "btnAdgroupRevert";
            this.btnAdgroupRevert.Size = new System.Drawing.Size(75, 23);
            this.btnAdgroupRevert.TabIndex = 2;
            this.btnAdgroupRevert.Text = "推广组恢复";
            this.btnAdgroupRevert.UseVisualStyleBackColor = true;
            this.btnAdgroupRevert.Click += new System.EventHandler(this.btnAdgroupRevert_Click);
            // 
            // FrmDataBaseTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 481);
            this.Controls.Add(this.btnAdgroupRevert);
            this.Controls.Add(this.btnSave);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FrmDataBaseTest";
            this.Text = "数据维护";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAdgroupRevert;
    }
}