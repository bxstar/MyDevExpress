namespace TaoBaoDataServer.WinClientData
{
    partial class FrmOutPut
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
            this.rtxOutPut = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtxOutPut
            // 
            this.rtxOutPut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxOutPut.Location = new System.Drawing.Point(0, 0);
            this.rtxOutPut.Name = "rtxOutPut";
            this.rtxOutPut.Size = new System.Drawing.Size(430, 659);
            this.rtxOutPut.TabIndex = 0;
            this.rtxOutPut.Text = "";
            // 
            // FrmOutPut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 659);
            this.Controls.Add(this.rtxOutPut);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FrmOutPut";
            this.Text = "输出窗口";
            this.Load += new System.EventHandler(this.FrmOutPut_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxOutPut;
    }
}