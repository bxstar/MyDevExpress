using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using iclickpro.AccessCommon;
using WeifenLuo.WinFormsUI.Docking;
using DevExpress.Data;

namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// 继承DockContent，并增加了GridControl的共用方法
    /// </summary>
    public class MyDockContent : DockContent
    {
        /// <summary>
        /// 浏览器路径
        /// </summary>
        protected static readonly string Const_BrowserPath = CommonFunction.GetAppSetting("BrowserPath");

        double totalImp = 0; double totalClick = 0; double totalPay = 0; double totalCost = 0; double totalClickForCpc = 0; double totalCostForCpc = 0;
        protected void gridViewCustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            if (((DevExpress.XtraGrid.GridSummaryItem)e.Item).FieldName.CompareTo("ctr") == 0)
            {
                if (e.SummaryProcess == CustomSummaryProcess.Start)
                {//初始化起始值，防止多次统计后起始值发生变化
                    totalImp = 0;
                    totalClick = 0;
                }
                if (e.SummaryProcess == CustomSummaryProcess.Calculate)
                {//每行数据的统计
                    dynamic entity = e.Row as dynamic;
                    totalImp += entity.impressions;
                    totalClick += entity.click;
                }
                if (e.SummaryProcess == CustomSummaryProcess.Finalize)
                {//最终结果计算
                    if (totalImp != 0)
                        e.TotalValue = string.Format("AVG={0}", Math.Round(totalClick / totalImp * 100, 2));
                    else
                        e.TotalValue = "AVG=0.00";
                }
            }

            if (((DevExpress.XtraGrid.GridSummaryItem)e.Item).FieldName.CompareTo("cpc") == 0)
            {
                if (e.SummaryProcess == CustomSummaryProcess.Start)
                {//初始化起始值，防止多次统计后起始值发生变化
                    totalClickForCpc = 0;
                    totalCostForCpc = 0;
                }
                if (e.SummaryProcess == CustomSummaryProcess.Calculate)
                {//每行数据的统计
                    dynamic entity = e.Row as dynamic;
                    totalClickForCpc += entity.click;
                    totalCostForCpc += Convert.ToDouble(entity.cost);
                }

                if (e.SummaryProcess == CustomSummaryProcess.Finalize)
                {//最终结果计算
                    if (totalClickForCpc != 0)
                        e.TotalValue = string.Format("AVG={0}", Math.Round(totalCostForCpc / totalClickForCpc, 2));
                    else
                        e.TotalValue = "AVG=0.00";
                }
            }

            if (((DevExpress.XtraGrid.GridSummaryItem)e.Item).FieldName.CompareTo("roi") == 0)
            {
                if (e.SummaryProcess == CustomSummaryProcess.Start)
                {//初始化起始值，防止多次统计后起始值发生变化
                    totalPay = 0;
                    totalCost = 0;
                }
                if (e.SummaryProcess == CustomSummaryProcess.Calculate)
                {//每行数据的统计
                    dynamic entity = e.Row as dynamic;
                    totalCost += Convert.ToDouble(entity.cost);
                    totalPay += Convert.ToDouble(entity.directpay + entity.indirectpay);
                }
                if (e.SummaryProcess == CustomSummaryProcess.Finalize)
                {//最终结果计算
                    if (totalCost != 0)
                        e.TotalValue = string.Format("AVG={0}", Math.Round(totalPay / totalCost, 2));
                    else
                        e.TotalValue = "AVG=0.00";
                }
            }


        }

        public void gridViewCustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = Convert.ToString(e.RowHandle + 1);
            }
        }

        public void gridViewEndSorting(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            gv.FocusedRowHandle = 0;
        }
    }

    /// <summary>
    /// 日志滚动输出，多线程实现，不用Application.DoEvents
    /// </summary>
    public partial class FrmTool : MyDockContent
    {
        Thread Log;

        public FrmTool()
        {
            InitializeComponent();

            //行号列宽
            gridViewJsonList.IndicatorWidth  = 40;
            //排序完显示第一行
            gridViewJsonList.EndSorting += new EventHandler(gridViewEndSorting);
            //行号事件
            gridViewJsonList.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
        }

        private void btnDepress_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = CommonFunction.Decompress(richTextBox1.Text);
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = CommonFunction.Compress(richTextBox1.Text);
        }

        private void btnJsonToList_Click(object sender, EventArgs e)
        {
            gridJsonList.DataSource = null;
            gridViewJsonList.Columns.Clear();

            var data = new DynamicJsonParser().FromJson(richTextBox1.Text);
            if (data is Array)
            {
                DataTable dt = new DataTable();

                foreach (var itemDic in data)
                {
                    Dictionary<string, object> dic = itemDic.GetDictionary();
                    if (dt.Columns.Count == 0)
                    {
                        foreach (var entry in dic)
                        {
                            string columnName = entry.Key.ToLower();
                            if (entry.Value != null)
                            {
                                if (columnName.Contains("date") || columnName.Contains("time"))
                                {
                                    DateTime dtType;
                                    if (DateTime.TryParse(entry.Value.ToString(), out dtType))
                                    {
                                        dt.Columns.Add(columnName, typeof(System.DateTime));
                                        continue;
                                    }
                                }
                                double dType;
                                if (double.TryParse(entry.Value.ToString(), out dType))
                                {
                                    dt.Columns.Add(columnName, typeof(System.Double));
                                    continue;
                                }
                            }

                            dt.Columns.Add(columnName, typeof(System.String));
                        }
                    }
                    DataRow dr = dt.NewRow();
                    foreach (var entry in dic)
                    {
                        dr[entry.Key] = entry.Value;
                    }
                    dt.Rows.Add(dr);
                }

                gridJsonList.DataSource = dt;
                
                tabControl1.SelectedTab = tabPageJsonList;
            }
            else
            {
                MessageBox.Show("字符串不是数组类型，不能转化为列表显示");
            }
        }

        private void btnMd5Encrypt_Click(object sender, EventArgs e)
        {
            txtMd5Encrypt.Text = CommonFunction.Md5Encrypt(txtPwd.Text.Trim());
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string strOrign = txtOrign.Text.Trim();
            txtEncrypt.Text = CryptHelper.Encrypt(strOrign);
            txtDecrypt.Clear();
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            txtDecrypt.Text = CryptHelper.Decrypt(txtEncrypt.Text);
        }




        #region 日志使用测试

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            int pageIndex = 3;

            List<int> lstTest = new List<int>();
            for (int i = 0; i < 133; i++)
            {
                lstTest.Add(i);
            }
            var lst = lstTest.GetRange(0, 100);


            int pageSize = 50;
            int pageCount = 0;
            if (lstTest.Count >= ((pageIndex - 1) * pageSize + pageSize))
            {
                pageCount = pageSize;
            }
            else
            {
                pageCount = lstTest.Count - (pageIndex - 1) * pageSize;
            }

            lst = lstTest.GetRange((pageIndex - 1) * pageSize, pageCount);


            if (Log != null && Log.IsAlive)
            {
                Log.Abort();
                Log.Join();
                Log = null;
            }
            Log = new Thread(new ParameterizedThreadStart(OutPutLog.GetNumber));
            Log.Start(this);
            Log.IsBackground = true;

        }

        public void MsgAppend(string strMsg, Color c)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                int selectStart = richTextBox1.TextLength;
                richTextBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + strMsg + Environment.NewLine);
                richTextBox1.Select(selectStart, richTextBox1.TextLength - 1);
                richTextBox1.SelectionColor = c;
                richTextBox1.ScrollToCaret();

            }));
        }

        public class OutPutLog
        {
            /// <summary>
            /// 循环
            /// </summary>
            public static void GetNumber(object para)
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("运行了" + i + "次");
                    //消息写入richTextbox控件里
                    FrmTool f = para as FrmTool;
                    f.MsgAppend(i.ToString(), Color.Red);
                    Thread.Sleep(200);
                    //Application.DoEvents();
                }
            }
        }

        #endregion
    }
}
