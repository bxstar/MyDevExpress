using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaoBaoDataServer.WinClientData.Model;
using Top.Api.Domain;
using TaoBaoDataServer.WinClientData.BusinessLayer;

namespace TaoBaoDataServer.WinClientData
{

    public partial class Frm类目分析 : MyDockContent
    {

        IOutPut frmOutPut;

        public Frm类目分析(IOutPut _frmOutPut)
        {
            InitializeComponent();
            frmOutPut = _frmOutPut;
        }

        private void Frm类目分析_Load(object sender, EventArgs e)
        {
            gridViewCate.IndicatorWidth = 30;
            gridViewCate.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewCate.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewCate.OptionsView.ShowFooter = true;

            //TreeList绑定
            treeListCats.VirtualTreeGetCellValue += treeListCats_VirtualTreeGetCellValue;
            treeListCats.VirtualTreeGetChildNodes += treeListCats_VirtualTreeGetChildNodes;
        }

        private void btnGetAllCate_Click(object sender, EventArgs e)
        {
            treeListCats.DataSource = new EntityCategoryEx() { CatLevel = 0 };
        }

        void treeListCats_VirtualTreeGetCellValue(object sender, DevExpress.XtraTreeList.VirtualTreeGetCellValueInfo e)
        {
            EntityCategoryEx catEx = e.Node as EntityCategoryEx;

            if (e.Column == treeListCatId)
            {
                e.CellData = catEx.CatId;
            }
            else if (e.Column == treeListCatLevel)
            {
                e.CellData = catEx.CatLevel;
            }
            else if (e.Column == treeListCatName)
            {
                e.CellData = catEx.CatName;
            }
            else if (e.Column == treeListImpression)
            {
                e.CellData = catEx.Impression;
            }
            else if (e.Column == treeListClick)
            {
                e.CellData = catEx.Click;
            }
            else if (e.Column == treeListCompetition)
            {
                e.CellData = catEx.Competition;
            }
            else if (e.Column == treeListCost)
            {
                e.CellData = catEx.Cost;
            }
            else if (e.Column == treeListCtr)
            {
                e.CellData = catEx.Ctr;
            }
            else if (e.Column == treeListTransactiontotal)
            {
                e.CellData = catEx.Transactiontotal;
            }
            else if (e.Column == treeListCpc)
            {
                e.CellData = catEx.Cpc;
            }
            else if (e.Column == treeListRoi)
            {
                e.CellData = catEx.Roi;
            }
            else if (e.Column == treeListFavtotal)
            {
                e.CellData = catEx.Favtotal;
            }
        }

        void treeListCats_VirtualTreeGetChildNodes(object sender, DevExpress.XtraTreeList.VirtualTreeGetChildNodesInfo e)
        {
            EntityCategoryEx data = (EntityCategoryEx)e.Node;

            List<InsightCategoryInfoDTO> lstCatInfo = null;
            List<InsightCategoryDataDTO> lstCatData = null;

            if (data.Children == null)
            {
                data.Children = new BindingList<EntityCategoryEx>();
                if (data.CatLevel == 0)
                {//加载顶级类目 
                    lstCatInfo = CommonHandler.GetCatsFullInfo("0", null);
                }
                else
                {//加载子类目 
                    lstCatInfo = CommonHandler.GetCatsFullInfo("2", data.CatId.ToString());
                }
                if (lstCatInfo != null && lstCatInfo.Count > 0)
                {//存在子类目
                    string strCatIds = string.Join(",", lstCatInfo.Select(o => o.CatId));
                    frmOutPut.OutPutMsgFormat("api catIds:{0}", strCatIds);
                    lstCatData = CommonHandler.GetCatsData(strCatIds);
                }
                else
                {
                    return;
                }

                foreach (var itemCatInfo in lstCatInfo)
                {
                    frmOutPut.OutPutMsgFormat("catId:{0},catName:{1},catLevel:{2}", itemCatInfo.CatId, itemCatInfo.CatName, itemCatInfo.CatLevel);
                    EntityCategoryEx catEx = new EntityCategoryEx() { CatId = itemCatInfo.CatId, CatLevel = itemCatInfo.CatLevel, CatName = itemCatInfo.CatName, CatPathId = itemCatInfo.CatPathId, CatPathName = itemCatInfo.CatPathName, ParentCatId = itemCatInfo.ParentCatId };
                    var itemCatData = lstCatData.Find(o => o.CatId == itemCatInfo.CatId);
                    if (itemCatData != null)
                    {
                        catEx.Impression = itemCatData.Impression;
                        catEx.Click = itemCatData.Click;
                        catEx.Cost = itemCatData.Cost;
                        catEx.Ctr = itemCatData.Ctr;
                        catEx.Cpc = itemCatData.Cpc;
                        catEx.Roi = itemCatData.Roi;
                        catEx.Competition = itemCatData.Competition;
                        catEx.Coverage = itemCatData.Coverage;
                        catEx.Directtransaction = itemCatData.Directtransaction;
                        catEx.Directtransactionshipping = itemCatData.Directtransactionshipping;
                        catEx.Indirecttransaction = itemCatData.Indirecttransaction;
                        catEx.Indirecttransactionshipping = itemCatData.Indirecttransactionshipping;
                        catEx.Favitemtotal = itemCatData.Favitemtotal;
                        catEx.Favshoptotal = itemCatData.Favshoptotal;
                        catEx.Favtotal = itemCatData.Favtotal;
                        catEx.Transactionshippingtotal = itemCatData.Transactionshippingtotal;
                        catEx.Transactiontotal = itemCatData.Transactiontotal;
                    }
                    data.Children.Add(catEx);
                }
            }

            if (data.Children.Count != 0)
            {
                e.Children = data.Children;
            }
            else
            {//没有子节点 
                e.Children = null;
            }
        }

        private void btnGetCate_Click(object sender, EventArgs e)
        {
            gridControlCate.DataSource = null;

            string strCatIds = txtCategoryIds.Text.Trim();
            List<InsightCategoryInfoDTO> lstCatInfo = null;
            List<InsightCategoryDataDTO> lstCatData = null;
            if (strCatIds.Length == 0)
            {
                lstCatInfo = CommonHandler.GetCatsFullInfo("0", null);
                if (lstCatInfo == null) return;
                lstCatData = CommonHandler.GetCatsData(string.Join(",", lstCatInfo.Select(o => o.CatId)));
            }
            else
            {
                lstCatInfo = CommonHandler.GetCatsFullInfo("1", strCatIds);
                lstCatData = CommonHandler.GetCatsData(strCatIds);
            }

            //类目信息，大盘数据合并
            List<EntityCategoryEx> lstCatEx = new List<EntityCategoryEx>();
            foreach (var itemCatInfo in lstCatInfo)
            {
                EntityCategoryEx catEx = new EntityCategoryEx() { CatId = itemCatInfo.CatId, CatLevel = itemCatInfo.CatLevel, CatName = itemCatInfo.CatName, CatPathId = itemCatInfo.CatPathId, CatPathName = itemCatInfo.CatPathName, ParentCatId = itemCatInfo.ParentCatId };
                var itemCatData = lstCatData.Find(o => o.CatId == itemCatInfo.CatId);
                if (itemCatData != null)
                {
                    catEx.Impression = itemCatData.Impression;
                    catEx.Click = itemCatData.Click;
                    catEx.Cost = itemCatData.Cost;
                    catEx.Ctr = itemCatData.Ctr;
                    catEx.Cpc = itemCatData.Cpc;
                    catEx.Roi = itemCatData.Roi;
                    catEx.Competition = itemCatData.Competition;
                    catEx.Coverage = itemCatData.Coverage;
                    catEx.Directtransaction = itemCatData.Directtransaction;
                    catEx.Directtransactionshipping = itemCatData.Directtransactionshipping;
                    catEx.Indirecttransaction = itemCatData.Indirecttransaction;
                    catEx.Indirecttransactionshipping = itemCatData.Indirecttransactionshipping;
                    catEx.Favitemtotal = itemCatData.Favitemtotal;
                    catEx.Favshoptotal = itemCatData.Favshoptotal;
                    catEx.Favtotal = itemCatData.Favtotal;
                    catEx.Transactionshippingtotal = itemCatData.Transactionshippingtotal;
                    catEx.Transactiontotal = itemCatData.Transactiontotal;
                }
                lstCatEx.Add(catEx);
            }

            gridControlCate.DataSource = lstCatEx;
            gridViewCate.Columns["Cpc"].SummaryItem.DisplayFormat = "(Avg:{0:N})";
            gridViewCate.Columns["Cpc"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Average;

        }
    }
}
