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
            this.label3 = new System.Windows.Forms.Label();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.dtStartDate = new System.Windows.Forms.DateTimePicker();
            this.btnGetKeywordIndex = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGetWord = new System.Windows.Forms.Button();
            this.txtKeywords = new System.Windows.Forms.TextBox();
            this.txtNumID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gridControlKeywordBase = new DevExpress.XtraGrid.GridControl();
            this.gridViewKeywordBase = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gridControlWordsData = new DevExpress.XtraGrid.GridControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.gridControlWordsSubData = new DevExpress.XtraGrid.GridControl();
            this.gridViewWordsSubData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridViewWordsData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelTop.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlKeywordBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewKeywordBase)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWordsData)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWordsSubData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWordsSubData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWordsData)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.label3);
            this.panelTop.Controls.Add(this.dtEndDate);
            this.panelTop.Controls.Add(this.dtStartDate);
            this.panelTop.Controls.Add(this.btnGetKeywordIndex);
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.btnGetWord);
            this.panelTop.Controls.Add(this.txtKeywords);
            this.panelTop.Controls.Add(this.txtNumID);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1010, 102);
            this.panelTop.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(864, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "至";
            // 
            // dtEndDate
            // 
            this.dtEndDate.CustomFormat = "yyyy-MM-dd";
            this.dtEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEndDate.Location = new System.Drawing.Point(887, 56);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.Size = new System.Drawing.Size(89, 21);
            this.dtEndDate.TabIndex = 8;
            // 
            // dtStartDate
            // 
            this.dtStartDate.CustomFormat = "yyyy-MM-dd";
            this.dtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStartDate.Location = new System.Drawing.Point(765, 56);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.Size = new System.Drawing.Size(93, 21);
            this.dtStartDate.TabIndex = 7;
            // 
            // btnGetKeywordIndex
            // 
            this.btnGetKeywordIndex.Location = new System.Drawing.Point(668, 56);
            this.btnGetKeywordIndex.Name = "btnGetKeywordIndex";
            this.btnGetKeywordIndex.Size = new System.Drawing.Size(75, 23);
            this.btnGetKeywordIndex.TabIndex = 6;
            this.btnGetKeywordIndex.Text = "获取词指数";
            this.btnGetKeywordIndex.UseVisualStyleBackColor = true;
            this.btnGetKeywordIndex.Click += new System.EventHandler(this.btnGetKeywordIndex_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "关键词：";
            // 
            // btnGetWord
            // 
            this.btnGetWord.Location = new System.Drawing.Point(668, 23);
            this.btnGetWord.Name = "btnGetWord";
            this.btnGetWord.Size = new System.Drawing.Size(75, 23);
            this.btnGetWord.TabIndex = 2;
            this.btnGetWord.Text = "取词";
            this.btnGetWord.UseVisualStyleBackColor = true;
            this.btnGetWord.Click += new System.EventHandler(this.btnGetWord_Click);
            // 
            // txtKeywords
            // 
            this.txtKeywords.Location = new System.Drawing.Point(84, 58);
            this.txtKeywords.Name = "txtKeywords";
            this.txtKeywords.Size = new System.Drawing.Size(565, 21);
            this.txtKeywords.TabIndex = 1;
            // 
            // txtNumID
            // 
            this.txtNumID.Location = new System.Drawing.Point(84, 25);
            this.txtNumID.Name = "txtNumID";
            this.txtNumID.Size = new System.Drawing.Size(565, 21);
            this.txtNumID.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "宝贝ID：";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 102);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1010, 481);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gridControlKeywordBase);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1002, 455);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "关键词基础指数";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gridControlKeywordBase
            // 
            this.gridControlKeywordBase.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlKeywordBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlKeywordBase.Location = new System.Drawing.Point(3, 3);
            this.gridControlKeywordBase.MainView = this.gridViewKeywordBase;
            this.gridControlKeywordBase.Name = "gridControlKeywordBase";
            this.gridControlKeywordBase.Size = new System.Drawing.Size(996, 449);
            this.gridControlKeywordBase.TabIndex = 0;
            this.gridControlKeywordBase.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewKeywordBase});
            // 
            // gridViewKeywordBase
            // 
            this.gridViewKeywordBase.GridControl = this.gridControlKeywordBase;
            this.gridViewKeywordBase.Name = "gridViewKeywordBase";
            this.gridViewKeywordBase.OptionsSelection.MultiSelect = true;
            this.gridViewKeywordBase.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gridViewKeywordBase.OptionsView.ShowGroupPanel = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gridControlWordsData);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1002, 455);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "关键词大盘指数";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gridControlWordsData
            // 
            this.gridControlWordsData.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlWordsData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlWordsData.Location = new System.Drawing.Point(3, 3);
            this.gridControlWordsData.MainView = this.gridViewWordsData;
            this.gridControlWordsData.Name = "gridControlWordsData";
            this.gridControlWordsData.Size = new System.Drawing.Size(996, 449);
            this.gridControlWordsData.TabIndex = 1;
            this.gridControlWordsData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewWordsData});
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.gridControlWordsSubData);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1002, 455);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "关键词流量细分指数";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // gridControlWordsSubData
            // 
            this.gridControlWordsSubData.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlWordsSubData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlWordsSubData.Location = new System.Drawing.Point(0, 0);
            this.gridControlWordsSubData.MainView = this.gridViewWordsSubData;
            this.gridControlWordsSubData.Name = "gridControlWordsSubData";
            this.gridControlWordsSubData.Size = new System.Drawing.Size(1002, 455);
            this.gridControlWordsSubData.TabIndex = 2;
            this.gridControlWordsSubData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewWordsSubData});
            // 
            // gridViewWordsSubData
            // 
            this.gridViewWordsSubData.GridControl = this.gridControlWordsSubData;
            this.gridViewWordsSubData.Name = "gridViewWordsSubData";
            this.gridViewWordsSubData.OptionsSelection.MultiSelect = true;
            this.gridViewWordsSubData.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gridViewWordsSubData.OptionsView.ShowGroupPanel = false;
            // 
            // gridViewWordsData
            // 
            this.gridViewWordsData.GridControl = this.gridControlWordsData;
            this.gridViewWordsData.Name = "gridViewWordsData";
            this.gridViewWordsData.OptionsSelection.MultiSelect = true;
            this.gridViewWordsData.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gridViewWordsData.OptionsView.ShowGroupPanel = false;
            // 
            // FrmWord
            // 
            this.AcceptButton = this.btnGetKeywordIndex;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 583);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "FrmWord";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "取词";
            this.Load += new System.EventHandler(this.FrmWord_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlKeywordBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewKeywordBase)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWordsData)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWordsSubData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWordsSubData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWordsData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnGetWord;
        private System.Windows.Forms.TextBox txtNumID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtKeywords;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtEndDate;
        private System.Windows.Forms.DateTimePicker dtStartDate;
        private System.Windows.Forms.Button btnGetKeywordIndex;
        private DevExpress.XtraGrid.GridControl gridControlKeywordBase;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewKeywordBase;
        private DevExpress.XtraGrid.GridControl gridControlWordsData;
        private DevExpress.XtraGrid.GridControl gridControlWordsSubData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewWordsSubData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewWordsData;
    }
}

