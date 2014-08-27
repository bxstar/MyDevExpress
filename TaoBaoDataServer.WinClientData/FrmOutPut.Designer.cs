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
            this.components = new System.ComponentModel.Container();
            this.rtxOutPut = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.清除内容ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtxOutPut
            // 
            this.rtxOutPut.ContextMenuStrip = this.contextMenuStrip1;
            this.rtxOutPut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxOutPut.Location = new System.Drawing.Point(0, 0);
            this.rtxOutPut.Name = "rtxOutPut";
            this.rtxOutPut.Size = new System.Drawing.Size(430, 659);
            this.rtxOutPut.TabIndex = 0;
            this.rtxOutPut.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.清除内容ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(119, 26);
            // 
            // 清除内容ToolStripMenuItem
            // 
            this.清除内容ToolStripMenuItem.Name = "清除内容ToolStripMenuItem";
            this.清除内容ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.清除内容ToolStripMenuItem.Text = "清除内容";
            this.清除内容ToolStripMenuItem.Click += new System.EventHandler(this.清除内容ToolStripMenuItem_Click);
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
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxOutPut;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 清除内容ToolStripMenuItem;
    }
}