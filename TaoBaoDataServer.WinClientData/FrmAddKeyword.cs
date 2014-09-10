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

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmAddKeyword : Form
    {
        /// <summary>
        /// 当前操作的用户
        /// </summary>
        public TopSession CurrentSession { get; set; }

        public FrmAddKeyword()
        {
            InitializeComponent();
        }

        private void FrmAddKeyword_Load(object sender, EventArgs e)
        {
            this.gridView1.IndicatorWidth = 40;

            DataTable dt = new DataTable();
            //格式增加
            dt.Columns.Add("keyword", typeof(string));
            dt.Columns.Add("max_price", typeof(Int32));
            //dt.Columns.Add("时间", typeof(DateTime));
            
            //dt.Rows.Add(new object[] { "看书", 3200 });
            //dt.Rows.Add(new object[] { "上网,游戏", 4000});
            //dt.Rows.Add(new object[] { "上网,逛街", 7200 });
            //dt.Rows.Add(new object[] { "上网,逛街,看书,游戏", 2000 });
            //dt.Rows.Add(new object[] { "看书,逛街,游戏", 350 });

            gridControl1.DataSource = dt;
            gridControl1.Focus();
        }

        public void SetCurrentAdgrupId(long adgroupId)
        {
            txtAdGroupId.Text = adgroupId.ToString();
        }

        bool isCancel = true;
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            //不能编辑
            e.Cancel = isCancel;
        }

        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            //启用编辑
            isCancel = false;
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl+V粘贴，gridView1_ShowingEditor必不可少
            if (e.Modifiers.CompareTo(Keys.Control) == 0 && e.KeyCode == Keys.V)
            {
                object _objData = null;
                IDataObject dataObj = Clipboard.GetDataObject();

                if (dataObj.GetDataPresent(DataFormats.Text))
                    _objData = dataObj.GetData(DataFormats.Text);

                if (_objData != null)
                {
                    int RowNumber = 0;
                    string _tempStr = _objData.ToString();
                    string[] _split = { "\r\n" };
                    string[] _arrayStr = _tempStr.Split(_split, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < _arrayStr.Length; i++)
                    {
                        string[] _arrayStr2 = _arrayStr[i].Split('\t');
                        gridView1.AddNewRow();
                        gridView1.MoveLastVisible();
                        RowNumber = gridView1.FocusedRowHandle;
                        gridView1.SetRowCellValue(RowNumber, gridView1.Columns["keyword"], _arrayStr2[0]);
                        gridView1.SetRowCellValue(RowNumber, gridView1.Columns["max_price"], (int)Convert.ToDecimal(_arrayStr2[1]));
                    }
                    isCancel = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BusinessLayer.BusinessKeywordHandler keywordHandler = new BusinessLayer.BusinessKeywordHandler();
            
            List<Keyword> lstKeyword = new List<Keyword>();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                Keyword k = new Keyword();
                k.IsDefaultPrice = false;
                k.MatchScope = "4";

                k.Word = gridView1.GetRowCellValue(i, "keyword").ToString();
                k.MaxPrice = Convert.ToInt64(gridView1.GetRowCellValue(i, "max_price"));
                lstKeyword.Add(k);
            }

            var response = keywordHandler.AddKeywordOnline(CurrentSession, Convert.ToInt64(txtAdGroupId.Text), lstKeyword);
            if (response.IsError)
            {
                MessageBox.Show(response.ErrorMessage + "\r\n" + response.SubErrorMessage);
            }
            else
            {
                MessageBox.Show("关键词添加成功，添加成功的关键词数量：" + response.listResponseKeyword.Count);
                this.Close();
            }
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            gridView1.AddNewRow();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            this.gridView1.DeleteSelectedRows();
        }


        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            if (e.Info.IsRowIndicator)
            {
                if (e.RowHandle >= 0)
                {
                    e.Info.DisplayText = (e.RowHandle + 1).ToString();
                }
            }
        }


    }
}
