namespace TaoBaoDataServer.WinClientData
{
    partial class Frm类目分析
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
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.treeListCats = new DevExpress.XtraTreeList.TreeList();
            this.treeListCatId = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCatLevel = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCatName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListParentCatId = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListImpression = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListClick = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCtr = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCompetition = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCost = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListCpc = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListDirecttransaction = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListIndirecttransaction = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListTransactiontotal = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListFavtotal = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListRoi = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnGetAllCate = new DevExpress.XtraEditors.SimpleButton();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.gridControlCate = new DevExpress.XtraGrid.GridControl();
            this.gridViewCate = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.btnGetCate = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtCategoryIds = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListCats)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCategoryIds.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(961, 551);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.treeListCats);
            this.xtraTabPage1.Controls.Add(this.panelControl1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(955, 522);
            this.xtraTabPage1.Text = "全类目树形展示";
            // 
            // treeListCats
            // 
            this.treeListCats.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListCatId,
            this.treeListCatLevel,
            this.treeListCatName,
            this.treeListParentCatId,
            this.treeListImpression,
            this.treeListClick,
            this.treeListCtr,
            this.treeListCompetition,
            this.treeListCost,
            this.treeListCpc,
            this.treeListDirecttransaction,
            this.treeListIndirecttransaction,
            this.treeListTransactiontotal,
            this.treeListFavtotal,
            this.treeListRoi});
            this.treeListCats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListCats.Location = new System.Drawing.Point(0, 40);
            this.treeListCats.Name = "treeListCats";
            this.treeListCats.OptionsBehavior.EnableFiltering = true;
            this.treeListCats.OptionsFind.AllowFindPanel = true;
            this.treeListCats.OptionsView.ShowAutoFilterRow = true;
            this.treeListCats.Size = new System.Drawing.Size(955, 482);
            this.treeListCats.TabIndex = 3;
            // 
            // treeListCatId
            // 
            this.treeListCatId.Caption = "CatId";
            this.treeListCatId.FieldName = "CatId";
            this.treeListCatId.Name = "treeListCatId";
            this.treeListCatId.OptionsColumn.AllowEdit = false;
            this.treeListCatId.OptionsColumn.ReadOnly = true;
            this.treeListCatId.Visible = true;
            this.treeListCatId.VisibleIndex = 0;
            // 
            // treeListCatLevel
            // 
            this.treeListCatLevel.Caption = "CatLevel";
            this.treeListCatLevel.FieldName = "CatLevel";
            this.treeListCatLevel.Name = "treeListCatLevel";
            this.treeListCatLevel.OptionsColumn.AllowEdit = false;
            this.treeListCatLevel.OptionsColumn.ReadOnly = true;
            this.treeListCatLevel.Visible = true;
            this.treeListCatLevel.VisibleIndex = 1;
            // 
            // treeListCatName
            // 
            this.treeListCatName.Caption = "CatName";
            this.treeListCatName.FieldName = "CatName";
            this.treeListCatName.Name = "treeListCatName";
            this.treeListCatName.OptionsColumn.AllowEdit = false;
            this.treeListCatName.OptionsColumn.ReadOnly = true;
            this.treeListCatName.Visible = true;
            this.treeListCatName.VisibleIndex = 2;
            // 
            // treeListParentCatId
            // 
            this.treeListParentCatId.Caption = "ParentCatId";
            this.treeListParentCatId.FieldName = "ParentCatId";
            this.treeListParentCatId.Name = "treeListParentCatId";
            // 
            // treeListImpression
            // 
            this.treeListImpression.Caption = "Impression";
            this.treeListImpression.FieldName = "Impression";
            this.treeListImpression.Name = "treeListImpression";
            this.treeListImpression.OptionsColumn.AllowEdit = false;
            this.treeListImpression.Visible = true;
            this.treeListImpression.VisibleIndex = 3;
            // 
            // treeListClick
            // 
            this.treeListClick.Caption = "Click";
            this.treeListClick.FieldName = "Click";
            this.treeListClick.Name = "treeListClick";
            this.treeListClick.OptionsColumn.AllowEdit = false;
            this.treeListClick.Visible = true;
            this.treeListClick.VisibleIndex = 4;
            // 
            // treeListCtr
            // 
            this.treeListCtr.Caption = "Ctr";
            this.treeListCtr.FieldName = "Ctr";
            this.treeListCtr.Name = "treeListCtr";
            this.treeListCtr.OptionsColumn.AllowEdit = false;
            this.treeListCtr.Visible = true;
            this.treeListCtr.VisibleIndex = 7;
            // 
            // treeListCompetition
            // 
            this.treeListCompetition.Caption = "Competition";
            this.treeListCompetition.FieldName = "Competition";
            this.treeListCompetition.Name = "treeListCompetition";
            this.treeListCompetition.OptionsColumn.AllowEdit = false;
            this.treeListCompetition.Visible = true;
            this.treeListCompetition.VisibleIndex = 5;
            // 
            // treeListCost
            // 
            this.treeListCost.Caption = "Cost";
            this.treeListCost.FieldName = "Cost";
            this.treeListCost.Name = "treeListCost";
            this.treeListCost.OptionsColumn.AllowEdit = false;
            this.treeListCost.Visible = true;
            this.treeListCost.VisibleIndex = 6;
            // 
            // treeListCpc
            // 
            this.treeListCpc.Caption = "Cpc";
            this.treeListCpc.FieldName = "Cpc";
            this.treeListCpc.Name = "treeListCpc";
            this.treeListCpc.OptionsColumn.AllowEdit = false;
            this.treeListCpc.Visible = true;
            this.treeListCpc.VisibleIndex = 8;
            // 
            // treeListDirecttransaction
            // 
            this.treeListDirecttransaction.Caption = "直接转化";
            this.treeListDirecttransaction.FieldName = "Directtransaction";
            this.treeListDirecttransaction.Name = "treeListDirecttransaction";
            this.treeListDirecttransaction.OptionsColumn.AllowEdit = false;
            // 
            // treeListIndirecttransaction
            // 
            this.treeListIndirecttransaction.Caption = "间接转化";
            this.treeListIndirecttransaction.FieldName = "Indirecttransaction";
            this.treeListIndirecttransaction.Name = "treeListIndirecttransaction";
            this.treeListIndirecttransaction.OptionsColumn.AllowEdit = false;
            // 
            // treeListTransactiontotal
            // 
            this.treeListTransactiontotal.Caption = "转化";
            this.treeListTransactiontotal.FieldName = "Transactiontotal";
            this.treeListTransactiontotal.Name = "treeListTransactiontotal";
            this.treeListTransactiontotal.OptionsColumn.AllowEdit = false;
            this.treeListTransactiontotal.Visible = true;
            this.treeListTransactiontotal.VisibleIndex = 9;
            // 
            // treeListFavtotal
            // 
            this.treeListFavtotal.Caption = "Favtotal";
            this.treeListFavtotal.FieldName = "Favtotal";
            this.treeListFavtotal.Name = "treeListFavtotal";
            this.treeListFavtotal.OptionsColumn.AllowEdit = false;
            this.treeListFavtotal.Visible = true;
            this.treeListFavtotal.VisibleIndex = 10;
            // 
            // treeListRoi
            // 
            this.treeListRoi.Caption = "Roi";
            this.treeListRoi.FieldName = "Roi";
            this.treeListRoi.Name = "treeListRoi";
            this.treeListRoi.OptionsColumn.AllowEdit = false;
            this.treeListRoi.Visible = true;
            this.treeListRoi.VisibleIndex = 11;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnGetAllCate);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(955, 40);
            this.panelControl1.TabIndex = 0;
            // 
            // btnGetAllCate
            // 
            this.btnGetAllCate.Location = new System.Drawing.Point(94, 9);
            this.btnGetAllCate.Name = "btnGetAllCate";
            this.btnGetAllCate.Size = new System.Drawing.Size(75, 23);
            this.btnGetAllCate.TabIndex = 1;
            this.btnGetAllCate.Text = "获取";
            this.btnGetAllCate.Click += new System.EventHandler(this.btnGetAllCate_Click);
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Controls.Add(this.gridControlCate);
            this.xtraTabPage2.Controls.Add(this.panelControl2);
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(955, 522);
            this.xtraTabPage2.Text = "指定类目数据";
            // 
            // gridControlCate
            // 
            this.gridControlCate.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlCate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlCate.Location = new System.Drawing.Point(0, 40);
            this.gridControlCate.MainView = this.gridViewCate;
            this.gridControlCate.Name = "gridControlCate";
            this.gridControlCate.Size = new System.Drawing.Size(955, 482);
            this.gridControlCate.TabIndex = 3;
            this.gridControlCate.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCate});
            // 
            // gridViewCate
            // 
            this.gridViewCate.GridControl = this.gridControlCate;
            this.gridViewCate.Name = "gridViewCate";
            this.gridViewCate.OptionsSelection.MultiSelect = true;
            this.gridViewCate.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gridViewCate.OptionsView.ShowGroupPanel = false;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.btnGetCate);
            this.panelControl2.Controls.Add(this.labelControl1);
            this.panelControl2.Controls.Add(this.txtCategoryIds);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(955, 40);
            this.panelControl2.TabIndex = 0;
            // 
            // btnGetCate
            // 
            this.btnGetCate.Location = new System.Drawing.Point(354, 9);
            this.btnGetCate.Name = "btnGetCate";
            this.btnGetCate.Size = new System.Drawing.Size(75, 23);
            this.btnGetCate.TabIndex = 2;
            this.btnGetCate.Text = "获取";
            this.btnGetCate.Click += new System.EventHandler(this.btnGetCate_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(33, 13);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 14);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "类目ID：";
            // 
            // txtCategoryIds
            // 
            this.txtCategoryIds.Location = new System.Drawing.Point(87, 10);
            this.txtCategoryIds.Name = "txtCategoryIds";
            this.txtCategoryIds.Properties.NullText = "多个类目用,分隔";
            this.txtCategoryIds.Size = new System.Drawing.Size(233, 20);
            this.txtCategoryIds.TabIndex = 0;
            // 
            // Frm类目分析
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(961, 551);
            this.Controls.Add(this.xtraTabControl1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "Frm类目分析";
            this.Text = "类目分析";
            this.Load += new System.EventHandler(this.Frm类目分析_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListCats)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.xtraTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCategoryIds.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraTreeList.TreeList treeListCats;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCatId;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCatLevel;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCatName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListParentCatId;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListImpression;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListClick;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCtr;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCompetition;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCost;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListCpc;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListDirecttransaction;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListIndirecttransaction;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListTransactiontotal;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListFavtotal;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListRoi;
        private DevExpress.XtraEditors.SimpleButton btnGetAllCate;
        private DevExpress.XtraGrid.GridControl gridControlCate;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCate;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnGetCate;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtCategoryIds;

    }
}