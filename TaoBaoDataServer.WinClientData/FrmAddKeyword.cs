﻿using System;
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

        public void SetCurrentAdgrupIds(string adgroupIds)
        {
            txtAdGroupId.Text = adgroupIds;
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

                        string strMaxPrice = _arrayStr2[1].ToString();
                        double maxPrice = 0;
                        Boolean isPrice = double.TryParse(strMaxPrice, out maxPrice);
                        if (isPrice)
                        {
                            gridView1.AddNewRow();
                            gridView1.MoveLastVisible();
                            RowNumber = gridView1.FocusedRowHandle;
                            gridView1.SetRowCellValue(RowNumber, gridView1.Columns["keyword"], _arrayStr2[0]);
                            gridView1.SetRowCellValue(RowNumber, gridView1.Columns["max_price"], maxPrice);
                        }
                        
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

                string strMaxPrice = gridView1.GetRowCellValue(i, "max_price").ToString();
                double maxPrice = 0;
                Boolean isPrice = double.TryParse(strMaxPrice, out maxPrice);
                if (isPrice)
                {
                    if (cbxUnitFen.Checked)
                    {//单位：分
                        k.MaxPrice = Convert.ToInt64(maxPrice);
                    }
                    else
                    {//单位：元
                        k.MaxPrice = Convert.ToInt64(maxPrice * 100);
                    }
                    lstKeyword.Add(k);
                }
                
            }

            ResponseKeyword response = null;
            if (txtAdGroupId.Text.Contains(","))
            {//多个推广组加词
                StringBuilder sbMsg = new StringBuilder();
                int resultSuccAddKeyword = 0; int resultFailAddKeyword = 0;
                List<long> lstAdgroupId = txtAdGroupId.Text.Split(',').Select(o => Convert.ToInt64(o)).ToList();
                foreach (var adgroupId in lstAdgroupId)
                {
                    response = keywordHandler.AddKeywordOnline(CurrentSession, adgroupId, lstKeyword);
                    if (response.IsError)
                    {
                        sbMsg.AppendFormat("errormsg:{0},suberrormsg:{1} \r\n", response.ErrorMessage, response.SubErrorMessage);
                        resultFailAddKeyword += lstKeyword.Count;
                    }
                    else
                    {
                        resultSuccAddKeyword += response.listResponseKeyword.Count;
                    }
                }
                if (sbMsg.Length == 0)
                {
                    MessageBox.Show("关键词添加成功，添加成功的关键词数量：" + resultSuccAddKeyword);
                    
                }
                else
                {
                    string strMsg = string.Format("关键词添加完成，部分失败，成功数量：{0}，失败数量：{1}\r\n失败信息：{2}", resultSuccAddKeyword, sbMsg.ToString());
                    MessageBox.Show(strMsg);
                }

            }
            else
            {//单个推广组加词 
                response = keywordHandler.AddKeywordOnline(CurrentSession, Convert.ToInt64(txtAdGroupId.Text), lstKeyword);
                if (response.IsError)
                {
                    MessageBox.Show(response.ErrorMessage + "\r\n" + response.SubErrorMessage);
                }
                else
                {
                    MessageBox.Show("关键词添加成功，添加成功的关键词数量：" + response.listResponseKeyword.Count);
                    
                }
            }

            this.Close();
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

        private void cbxUnitFen_CheckedChanged(object sender, EventArgs e)
        {
            cbxUnitYuan.Checked = !cbxUnitFen.Checked;
            if (cbxUnitFen.Checked)
            {
                this.gridColumn2.Caption = "出价（分）";
            }
            else
            {
                this.gridColumn2.Caption = "出价（元）";
            }
        }

        private void cbxUnitYuan_CheckedChanged(object sender, EventArgs e)
        {
            cbxUnitFen.Checked = !cbxUnitYuan.Checked;
            if (cbxUnitFen.Checked)
            {
                this.gridColumn2.Caption = "出价（分）";
            }
            else
            {
                this.gridColumn2.Caption = "出价（元）";
            }
        }


    }
}
