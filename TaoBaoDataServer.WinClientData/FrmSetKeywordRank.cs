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
using log4net;
using TaoBaoDataServer.WinClientData.BusinessLayer;

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmSetKeywordRank : Form
    {
        #region 属性
        private static log4net.ILog logger = LogManager.GetLogger("Logger");
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();
        BusinessUserHandler userHandler = new BusinessUserHandler();
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();

        /// <summary>
        /// 宝贝Id
        /// </summary>
        public long Current_ItemId
        {
            get
            {
                long itemId = 0;
                Int64.TryParse(txtItemId.Text, out itemId);
                return itemId;
            }
            set
            {
                txtItemId.Text = value.ToString();
            }
        }

        /// <summary>
        /// 关键词Id
        /// </summary>
        public long Current_KeywordId
        {
            get
            {
                long keywordId = 0;
                Int64.TryParse(txtKeywordId.Text, out keywordId);
                return keywordId;
            }
            set
            {
                txtKeywordId.Text = value.ToString();
            }
        }

        /// <summary>
        /// 关键词
        /// </summary>
        public string Current_Keyword
        {
            get
            {
                return txtKeyword.Text.Trim();
            }
            set
            {
                txtKeyword.Text = value;
            }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Current_NickName
        {
            get
            {
                return txtNickName.Text.Trim();
            }
            set
            {
                txtNickName.Text = value;
            }
        }

        /// <summary>
        /// 用户最高出价
        /// </summary>
        public long MaxPrice_UserSetup
        {
            get
            {
                long price = 0;
                Int64.TryParse(txtUserSetupMaxPrice.Text, out price);
                return price;
            }
            set
            {
                txtUserSetupMaxPrice.Text = value.ToString();
            }
        }

        /// <summary>
        /// 当前出价
        /// </summary>
        public long Current_Price
        {
            get
            {
                long price = 0;
                Int64.TryParse(txtCurrentPrice.Text, out price);
                return price;
            }
            set
            {
                txtCurrentPrice.Text = value.ToString();
            }
        }

        /// <summary>
        /// 关键词，当前匹配方式
        /// </summary>
        public string Current_MatchScope
        {
            get
            {
                return txtMatchScope.Text.Trim();
            }
            set
            {
                txtMatchScope.Text = value;
            }
        }

        #endregion


        public FrmSetKeywordRank()
        {
            InitializeComponent();
        }

        private void FrmSetKeywordRank_Load(object sender, EventArgs e)
        {

        }

        private void btnSearchRank_Click(object sender, EventArgs e)
        {
            string ip = "121.196.129.114";
            TopSession session = userHandler.GetUserSession(Current_NickName);
            //TopSession session = userHandler.GetUserSession("tp_世奇广告");
            var response = TaobaoApiHandler.TaobaoSimbaToolsItemsTopGet(session, Current_Keyword, ip);
            
            dgvRank.DataSource = response.Rankeditems;
            dgvRank.CurrentCell = null;
            if (response.Rankeditems != null)
            {
                for (int i = 0; i < response.Rankeditems.Count; i++)
                {
                    RankedItem item = dgvRank.Rows[i].DataBoundItem as RankedItem;
                    if (item.LinkUrl.Contains(Current_ItemId.ToString()))
                    {
                        dgvRank.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
            
        }

        private void btnSetRank_Click(object sender, EventArgs e)
        {
            TopSession session = userHandler.GetUserSession(Current_NickName);
            int rank = Convert.ToInt32(txtRank.Text);
            long compete_price = 10;    //从1毛开始竞价
            string strErrorMsg = SetKeywordRank(session, Current_KeywordId, ref compete_price, rank);

            if (strErrorMsg.Length > 0)
            {
                MessageBox.Show(strErrorMsg);
                return;
            }

            if (compete_price <= MaxPrice_UserSetup)
            {//设置成功 
                Current_Price = compete_price;
                Keyword k = new Keyword();
                k.KeywordId = Current_KeywordId;
                k.Word = Current_Keyword;
                k.MatchScope = Current_MatchScope;
                k.MaxPrice = compete_price;
                List<Keyword> lst = new List<Keyword>();
                lst.Add(k);

                var response = keywordHandler.ModifyKeywordOnline(session, lst);
                if (!response.IsError)
                {
                    MessageBox.Show(string.Format("竞价成功，出价{0}分", compete_price));
                }
                else
                {
                    MessageBox.Show("竞价失败");
                }
            }
            else
            {
                MessageBox.Show(string.Format("排名{0}需要出价{1}分，超过设定价格{2}分", rank, compete_price, MaxPrice_UserSetup));
            }

        }


        private string SetKeywordRank(TopSession session, long keywordId, ref long max_price, int rank)
        {
            string strErrorMsg = string.Empty;
            if (max_price > 9999)
            {
                max_price = 9999;
            }
            if (max_price < 10)
            {
                max_price = 10;
            }
            DateTime dtStart = new DateTime();
            //预估的出价排名区间，在max_price下1毛和上8毛之间
            //var response = TaobaoApiHandler.TaobaoSimbaKeywordKeywordforecastGet(session, keywordId, max_price);
            //if (response.IsError)
            //{
            //    if (CommonHandler.IsBanMsg(response))
            //    {//遇到频繁访问的错误，需要多次访问
            //        Boolean isBanError = true;
            //        while (isBanError)
            //        {
            //            System.Threading.Thread.Sleep(2000);
            //            response = TaobaoApiHandler.TaobaoSimbaKeywordKeywordforecastGet(session, keywordId, max_price);
            //            if (response.IsError && CommonHandler.IsBanMsg(response) && dtStart.AddMinutes(5) > DateTime.Now)
            //            {//超过5分钟放弃
            //                isBanError = true;
            //            }
            //            else
            //            {
            //                if (dtStart.AddMinutes(5) <= DateTime.Now)
            //                {
            //                    logger.Error("预估排名失败，已重试5分钟" + response.Body);
            //                    return "竞价失败，" + response.Body;
            //                }
            //                isBanError = false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        logger.Error("预估排名失败：" + response.Body);
            //        return "竞价失败，" + response.Body;
            //    }
            //}

            //string strPriceRank = response.KeywordForecast.PriceRank;
            //string[] arrPriceRank = strPriceRank.Split(',');
            //List<EntityPriceRank> lstPriceRank = new List<EntityPriceRank>();

            //foreach (var itemPriceRank in arrPriceRank)
            //{
            //    string[] arr = itemPriceRank.Split(':');

            //    EntityPriceRank pr = new EntityPriceRank();
            //    pr.max_price = Convert.ToInt64(arr[0]);
            //    pr.rank = Convert.ToInt32(arr[1]);

            //    lstPriceRank.Add(pr);
            //}

            //int minReturnRank = lstPriceRank.Min(o => o.rank);
            //int maxReturnRank = lstPriceRank.Max(o => o.rank);

            //if (rank < minReturnRank)
            //{//返回的排名靠后，需要加价继续预估
            //    if (max_price != 9999)
            //    {
            //        max_price = lstPriceRank.Max(o => o.max_price) + 10;            //加10分是根据淘宝返回的列表范围而定
            //        strErrorMsg = SetKeywordRank(session, keywordId, ref max_price, rank);
            //    }
            //    else
            //    {//超过9999的价格 
            //        return "所在排名价格超过99.99元，竞价失败";
            //    }
            //}
            //else if (rank <= maxReturnRank && rank >= minReturnRank)
            //{//找到符合的排名 
            //    double avg_price = 0;
            //    List<EntityPriceRank> lstRankEqual = lstPriceRank.Where(o => o.rank == rank).ToList();
            //    if (lstRankEqual.Count > 0)
            //    {//找到刚好匹配排名的，直接使用其平均出价
            //        avg_price = lstPriceRank.Where(o => o.rank == rank).Average(o => o.max_price);
            //    }
            //    else
            //    {//找出和需要的排名最接近的排名，使用它的最高出价 
            //        int nearestRank = GetNearestNum(lstPriceRank.Select(o => o.rank).ToList(), rank);
            //        lstRankEqual = lstPriceRank.Where(o => o.rank == nearestRank).ToList();
            //        avg_price = lstRankEqual.Max(o => o.max_price);
            //    }
            //    max_price = Convert.ToInt64(avg_price);
            //    return string.Empty ;
            //}
            //else
            //{//返回的排名靠前，需要降价继续预估
            //    if (max_price != 10)
            //    {
            //        max_price = lstPriceRank.Min(o => o.max_price) - 80;            //减80分是根据淘宝返回的列表范围而定
            //        strErrorMsg = SetKeywordRank(session, keywordId, ref max_price, rank);
            //    }
            //    else
            //    {//1毛钱以上才能预估 
            //        return "所在排名不足1毛，竞价失败";
            //    }
            //}

            return strErrorMsg;
        }

        /// <summary>
        /// 获取最接近的数
        /// </summary>
        private int GetNearestNum(List<int> lstNum, int targetNum)
        {
            List<int> buffer = new List<int>();    // 存放找到的数
            List<int> tmp = new List<int>();       // 临时缓存
            int min = 0;
            int dis = 0;

            min = Math.Abs(targetNum - lstNum[0]);
            foreach (int num in lstNum)
            {
                dis = Math.Abs(targetNum - num);
                if (dis < min)
                {
                    min = dis;
                    tmp.Clear();
                    tmp.Add((int)num);
                }
                else if (dis == min)
                {
                    tmp.Add((int)num);
                }
            }
            return tmp[0];
        }

        private void dgvRank_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dgv.RowHeadersWidth - 4,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgv.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dgv.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
    }
}
