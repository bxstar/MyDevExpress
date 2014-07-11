using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using TaoBaoDataServer.WinClientData.Model;
using TaoBaoDataServer.WinClientData.BusinessLayer;
using DevExpress.XtraGrid;

namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// 报表数据呈现窗口
    /// </summary>
    public partial class FrmReport : DockContent
    {
        /// <summary>
        /// 窗口标题
        /// </summary>
        public string FormTitle { get; set; }

        /// <summary>
        /// 报表数据，List对象
        /// </summary>
        public object ReportData { get; set; }

        BusinessUserHandler userHandler = new BusinessUserHandler();

        public FrmReport(string _formTitle)
        {
            FormTitle = _formTitle;
            InitializeComponent();
        }

        public FrmReport()
        {
            InitializeComponent();
        }

        private void FrmReport_Load(object sender, EventArgs e)
        {
            gridView1.IndicatorWidth = 50;
            gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            //排序完成后显示第一行
            gridView1.EndSorting += new EventHandler(gridViewEndSorting);

            if (!string.IsNullOrEmpty(FormTitle))
            {
                panel1.Visible = false;
                contextMenuStrip1.Enabled = false;
                this.Text = FormTitle;
                gridControl1.DataSource = ReportData;
            }
        }

        private void btnGetReport_Click(object sender, EventArgs e)
        {
            List<TopSession> lstUser = userHandler.GetUserInfo(null).Where(o => o.UserName == o.ProxyUserName).ToList();
            gridControl1.DataSource = lstUser;
        }

        private void btnGetReportDetail_Click(object sender, EventArgs e)
        {


        }

        private void 查看计划名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopSession user = gridView1.GetFocusedRow() as TopSession;

            MessageBox.Show(user.ProxyUserName);
        }

        private void gridViewEndSorting(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            gv.FocusedRowHandle = 0;
        }

        private void gridViewCustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = Convert.ToString(e.RowHandle + 1);
            }
        }
    }
}
