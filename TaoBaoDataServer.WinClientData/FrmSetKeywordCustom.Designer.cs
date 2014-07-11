namespace TaoBaoDataServer.WinClientData
{
    partial class FrmSetKeywordCustom
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
            this.btnBlackSet = new System.Windows.Forms.Button();
            this.dgvBlack = new System.Windows.Forms.DataGridView();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnWhiteAdd = new System.Windows.Forms.Button();
            this.btnWhiteDel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbWhiteMatchScope = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWhiteRemark = new System.Windows.Forms.TextBox();
            this.txtWhiteFindSource = new System.Windows.Forms.TextBox();
            this.txtWhiteMaxPrice = new System.Windows.Forms.TextBox();
            this.txtWhiteKeyword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnWhiteSet = new System.Windows.Forms.Button();
            this.dgvWhite = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBlackAdd = new System.Windows.Forms.Button();
            this.btnBlackDel = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBlackRemark = new System.Windows.Forms.TextBox();
            this.txtBlackFindSource = new System.Windows.Forms.TextBox();
            this.txtBlackKeyword = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbBlackLevel = new System.Windows.Forms.ComboBox();
            this.cmbBlackMatchScope = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBlack)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWhite)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBlackSet
            // 
            this.btnBlackSet.Location = new System.Drawing.Point(783, 291);
            this.btnBlackSet.Name = "btnBlackSet";
            this.btnBlackSet.Size = new System.Drawing.Size(75, 23);
            this.btnBlackSet.TabIndex = 0;
            this.btnBlackSet.Text = "黑名单保存";
            this.btnBlackSet.UseVisualStyleBackColor = true;
            this.btnBlackSet.Click += new System.EventHandler(this.btnBlackSet_Click);
            // 
            // dgvBlack
            // 
            this.dgvBlack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBlack.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBlack.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12,
            this.Column13,
            this.Column18,
            this.Column19,
            this.Column14,
            this.Column15});
            this.dgvBlack.Location = new System.Drawing.Point(6, 20);
            this.dgvBlack.Name = "dgvBlack";
            this.dgvBlack.RowTemplate.Height = 23;
            this.dgvBlack.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBlack.Size = new System.Drawing.Size(1262, 223);
            this.dgvBlack.TabIndex = 3;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "keyword";
            this.Column9.HeaderText = "关键词";
            this.Column9.Name = "Column9";
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "match_scope_display";
            this.Column10.HeaderText = "匹配方式";
            this.Column10.Name = "Column10";
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "is_succeed";
            this.Column11.HeaderText = "提交状态";
            this.Column11.Name = "Column11";
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "find_source";
            this.Column12.HeaderText = "来源";
            this.Column12.Name = "Column12";
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "remark";
            this.Column13.HeaderText = "备注";
            this.Column13.Name = "Column13";
            // 
            // Column18
            // 
            this.Column18.DataPropertyName = "user_id";
            this.Column18.HeaderText = "UserId";
            this.Column18.Name = "Column18";
            // 
            // Column19
            // 
            this.Column19.DataPropertyName = "adgroup_id";
            this.Column19.HeaderText = "AdgroupId";
            this.Column19.Name = "Column19";
            // 
            // Column14
            // 
            this.Column14.DataPropertyName = "create_date";
            this.Column14.HeaderText = "创建时间";
            this.Column14.Name = "Column14";
            // 
            // Column15
            // 
            this.Column15.DataPropertyName = "update_date";
            this.Column15.HeaderText = "修改时间";
            this.Column15.Name = "Column15";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnWhiteAdd);
            this.groupBox1.Controls.Add(this.btnWhiteDel);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbWhiteMatchScope);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtWhiteRemark);
            this.groupBox1.Controls.Add(this.txtWhiteFindSource);
            this.groupBox1.Controls.Add(this.txtWhiteMaxPrice);
            this.groupBox1.Controls.Add(this.txtWhiteKeyword);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnWhiteSet);
            this.groupBox1.Controls.Add(this.dgvWhite);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1280, 376);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "白名单设置：";
            // 
            // btnWhiteAdd
            // 
            this.btnWhiteAdd.Location = new System.Drawing.Point(309, 340);
            this.btnWhiteAdd.Name = "btnWhiteAdd";
            this.btnWhiteAdd.Size = new System.Drawing.Size(75, 23);
            this.btnWhiteAdd.TabIndex = 13;
            this.btnWhiteAdd.Text = "新增";
            this.btnWhiteAdd.UseVisualStyleBackColor = true;
            this.btnWhiteAdd.Click += new System.EventHandler(this.btnWhiteAdd_Click);
            // 
            // btnWhiteDel
            // 
            this.btnWhiteDel.Location = new System.Drawing.Point(551, 340);
            this.btnWhiteDel.Name = "btnWhiteDel";
            this.btnWhiteDel.Size = new System.Drawing.Size(75, 23);
            this.btnWhiteDel.TabIndex = 12;
            this.btnWhiteDel.Text = "删除";
            this.btnWhiteDel.UseVisualStyleBackColor = true;
            this.btnWhiteDel.Click += new System.EventHandler(this.btnWhiteDel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(945, 313);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "备注：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(713, 313);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "来源：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 313);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "出价：";
            // 
            // cmbWhiteMatchScope
            // 
            this.cmbWhiteMatchScope.FormattingEnabled = true;
            this.cmbWhiteMatchScope.Items.AddRange(new object[] {
            "广泛匹配",
            "中心词匹配",
            "精确匹配"});
            this.cmbWhiteMatchScope.Location = new System.Drawing.Point(552, 309);
            this.cmbWhiteMatchScope.Name = "cmbWhiteMatchScope";
            this.cmbWhiteMatchScope.Size = new System.Drawing.Size(122, 20);
            this.cmbWhiteMatchScope.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(480, 313);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "匹配方式：";
            // 
            // txtWhiteRemark
            // 
            this.txtWhiteRemark.Location = new System.Drawing.Point(1013, 309);
            this.txtWhiteRemark.Name = "txtWhiteRemark";
            this.txtWhiteRemark.Size = new System.Drawing.Size(122, 21);
            this.txtWhiteRemark.TabIndex = 6;
            // 
            // txtWhiteFindSource
            // 
            this.txtWhiteFindSource.Location = new System.Drawing.Point(783, 309);
            this.txtWhiteFindSource.Name = "txtWhiteFindSource";
            this.txtWhiteFindSource.Size = new System.Drawing.Size(122, 21);
            this.txtWhiteFindSource.TabIndex = 6;
            this.txtWhiteFindSource.Text = "常用词";
            // 
            // txtWhiteMaxPrice
            // 
            this.txtWhiteMaxPrice.Location = new System.Drawing.Point(310, 309);
            this.txtWhiteMaxPrice.Name = "txtWhiteMaxPrice";
            this.txtWhiteMaxPrice.Size = new System.Drawing.Size(122, 21);
            this.txtWhiteMaxPrice.TabIndex = 6;
            // 
            // txtWhiteKeyword
            // 
            this.txtWhiteKeyword.Location = new System.Drawing.Point(82, 309);
            this.txtWhiteKeyword.Name = "txtWhiteKeyword";
            this.txtWhiteKeyword.Size = new System.Drawing.Size(122, 21);
            this.txtWhiteKeyword.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 313);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "关键词：";
            // 
            // btnWhiteSet
            // 
            this.btnWhiteSet.Location = new System.Drawing.Point(782, 340);
            this.btnWhiteSet.Name = "btnWhiteSet";
            this.btnWhiteSet.Size = new System.Drawing.Size(75, 23);
            this.btnWhiteSet.TabIndex = 4;
            this.btnWhiteSet.Text = "白名单保存";
            this.btnWhiteSet.UseVisualStyleBackColor = true;
            this.btnWhiteSet.Click += new System.EventHandler(this.btnWhiteSet_Click);
            // 
            // dgvWhite
            // 
            this.dgvWhite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvWhite.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWhite.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column16,
            this.Column17,
            this.Column7,
            this.Column8});
            this.dgvWhite.Location = new System.Drawing.Point(12, 30);
            this.dgvWhite.Name = "dgvWhite";
            this.dgvWhite.RowTemplate.Height = 23;
            this.dgvWhite.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWhite.Size = new System.Drawing.Size(1256, 247);
            this.dgvWhite.TabIndex = 3;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "keyword";
            this.Column1.HeaderText = "关键词";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "max_price";
            this.Column2.HeaderText = "出价";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "match_scope_display";
            this.Column3.HeaderText = "匹配方式";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "is_succeed";
            this.Column4.HeaderText = "提交状态";
            this.Column4.Name = "Column4";
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "find_source";
            this.Column5.HeaderText = "来源";
            this.Column5.Name = "Column5";
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "remark";
            this.Column6.HeaderText = "备注";
            this.Column6.Name = "Column6";
            // 
            // Column16
            // 
            this.Column16.DataPropertyName = "user_id";
            this.Column16.HeaderText = "UserId";
            this.Column16.Name = "Column16";
            // 
            // Column17
            // 
            this.Column17.DataPropertyName = "adgroup_id";
            this.Column17.HeaderText = "AdgroupId";
            this.Column17.Name = "Column17";
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "create_date";
            this.Column7.HeaderText = "创建时间";
            this.Column7.Name = "Column7";
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "update_date";
            this.Column8.HeaderText = "修改时间";
            this.Column8.Name = "Column8";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnBlackAdd);
            this.groupBox2.Controls.Add(this.btnBlackDel);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtBlackRemark);
            this.groupBox2.Controls.Add(this.txtBlackFindSource);
            this.groupBox2.Controls.Add(this.txtBlackKeyword);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.dgvBlack);
            this.groupBox2.Controls.Add(this.btnBlackSet);
            this.groupBox2.Controls.Add(this.cmbBlackLevel);
            this.groupBox2.Controls.Add(this.cmbBlackMatchScope);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 376);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1280, 326);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "黑名单设置";
            // 
            // btnBlackAdd
            // 
            this.btnBlackAdd.Location = new System.Drawing.Point(310, 291);
            this.btnBlackAdd.Name = "btnBlackAdd";
            this.btnBlackAdd.Size = new System.Drawing.Size(75, 23);
            this.btnBlackAdd.TabIndex = 13;
            this.btnBlackAdd.Text = "新增";
            this.btnBlackAdd.UseVisualStyleBackColor = true;
            this.btnBlackAdd.Click += new System.EventHandler(this.btnBlackAdd_Click);
            // 
            // btnBlackDel
            // 
            this.btnBlackDel.Location = new System.Drawing.Point(552, 291);
            this.btnBlackDel.Name = "btnBlackDel";
            this.btnBlackDel.Size = new System.Drawing.Size(75, 23);
            this.btnBlackDel.TabIndex = 12;
            this.btnBlackDel.Text = "删除";
            this.btnBlackDel.UseVisualStyleBackColor = true;
            this.btnBlackDel.Click += new System.EventHandler(this.btnBlackDel_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(945, 269);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "备注：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(713, 268);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 14;
            this.label9.Text = "来源：";
            // 
            // txtBlackRemark
            // 
            this.txtBlackRemark.Location = new System.Drawing.Point(1013, 265);
            this.txtBlackRemark.Name = "txtBlackRemark";
            this.txtBlackRemark.Size = new System.Drawing.Size(122, 21);
            this.txtBlackRemark.TabIndex = 12;
            // 
            // txtBlackFindSource
            // 
            this.txtBlackFindSource.Location = new System.Drawing.Point(783, 265);
            this.txtBlackFindSource.Name = "txtBlackFindSource";
            this.txtBlackFindSource.Size = new System.Drawing.Size(122, 21);
            this.txtBlackFindSource.TabIndex = 13;
            this.txtBlackFindSource.Text = "常用词";
            // 
            // txtBlackKeyword
            // 
            this.txtBlackKeyword.Location = new System.Drawing.Point(84, 265);
            this.txtBlackKeyword.Name = "txtBlackKeyword";
            this.txtBlackKeyword.Size = new System.Drawing.Size(122, 21);
            this.txtBlackKeyword.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(238, 269);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 7;
            this.label10.Text = "级别：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 269);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "关键词：";
            // 
            // cmbBlackLevel
            // 
            this.cmbBlackLevel.FormattingEnabled = true;
            this.cmbBlackLevel.Items.AddRange(new object[] {
            "全局黑名单",
            "用户黑名单",
            "推广组黑名单"});
            this.cmbBlackLevel.Location = new System.Drawing.Point(310, 266);
            this.cmbBlackLevel.Name = "cmbBlackLevel";
            this.cmbBlackLevel.Size = new System.Drawing.Size(122, 20);
            this.cmbBlackLevel.TabIndex = 8;
            // 
            // cmbBlackMatchScope
            // 
            this.cmbBlackMatchScope.FormattingEnabled = true;
            this.cmbBlackMatchScope.Items.AddRange(new object[] {
            "模糊匹配",
            "精确匹配"});
            this.cmbBlackMatchScope.Location = new System.Drawing.Point(552, 265);
            this.cmbBlackMatchScope.Name = "cmbBlackMatchScope";
            this.cmbBlackMatchScope.Size = new System.Drawing.Size(122, 20);
            this.cmbBlackMatchScope.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(480, 269);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 7;
            this.label7.Text = "匹配方式：";
            // 
            // FrmSetKeywordCustom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 702);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmSetKeywordCustom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置黑白名单";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmSetKeywordCustom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBlack)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWhite)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBlackSet;
        private System.Windows.Forms.DataGridView dgvBlack;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvWhite;
        private System.Windows.Forms.Button btnWhiteSet;
        private System.Windows.Forms.ComboBox cmbWhiteMatchScope;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWhiteKeyword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWhiteRemark;
        private System.Windows.Forms.TextBox txtWhiteFindSource;
        private System.Windows.Forms.TextBox txtWhiteMaxPrice;
        private System.Windows.Forms.Button btnWhiteDel;
        private System.Windows.Forms.Button btnWhiteAdd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtBlackRemark;
        private System.Windows.Forms.TextBox txtBlackFindSource;
        private System.Windows.Forms.TextBox txtBlackKeyword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbBlackMatchScope;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnBlackAdd;
        private System.Windows.Forms.Button btnBlackDel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbBlackLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column18;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column19;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
    }
}