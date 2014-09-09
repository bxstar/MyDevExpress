using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using log4net;
using System.Text.RegularExpressions;
using TaoBaoDataServer.WinClientData.Model;
using TaoBaoDataServer.WinClientData.BusinessLayer;
using WeifenLuo.WinFormsUI.Docking;
using iclickpro.AccessCommon;

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmWord : DockContent
    {
        IOutPut frmOutPut;
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();

        public FrmWord(IOutPut _frmOutPut)
        {
            frmOutPut = _frmOutPut;
            InitializeComponent();
        }

        private void FrmWord_Load(object sender, EventArgs e)
        {
            init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            dtStartDate.Value = DateTime.Now.AddDays(-1);
            dtEndDate.Value = DateTime.Now.AddDays(-1);

            gridViewKeywordBase.IndicatorWidth = gridViewWordsData.IndicatorWidth = gridViewWordsSubData.IndicatorWidth = 30;
            gridViewKeywordBase.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewWordsData.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewWordsSubData.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
        }

        private void btnGetWord_Click(object sender, EventArgs e)
        {
            string strItemId = txtNumID.Text.Trim();
            EntityItem itemOnline = CommonHandler.GetItemOnline(strItemId);
            
            Boolean isFindFirstMainWord = false;        //是否找到了第一核心词
            DateTime dtStartFind = DateTime.Now;

            string strFindKeywordResult = CommonHandler.GetItemFindKeyword(itemOnline.item_id);
            if (string.IsNullOrEmpty(strFindKeywordResult))
            {//发送宝贝找词消息
                string exchangeName = "ex_taobao_spider_samesimilar_item";
                BusinessMQ.SendMsgToExchange(null, exchangeName, string.Format("{0},{1},{2}", itemOnline.item_id, itemOnline.item_title, itemOnline.nick));
            }

            string titleSplit = CommonHandler.SplitWordFromWs(itemOnline.item_title);
            frmOutPut.OutPutMsg(titleSplit);

            frmOutPut.OutPutMsgFormat("类目名次：{0}", itemOnline.categroy_name);

            //将宝贝标题的分词按长度排序，在类目名称中的关键词作为核心词
            List<string> lstTitleWord = titleSplit.Split(',').OrderByDescending(o => o.Length).ToList();
            //标题分词在类目中出现的词，按重复字符数*长度排序，还要按照找词统计来排序
            List<string> lstMainWord = new List<string>();
            //核心词排序字典
            Dictionary<string, int> dicMainWord = new Dictionary<string, int>();
            foreach (var item in lstTitleWord)
            {
                if (item.Length > 1 && itemOnline.categroy_name.Contains(item) && !lstMainWord.Contains(item))
                {
                    lstMainWord.Add(item);
                    isFindFirstMainWord = true;
                }
            }

            //if (!isFindFirstMainWord)
            //{
                foreach (var item in lstTitleWord)
                { 
                    int sameCharCount=item.ToCharArray().Intersect(itemOnline.categroy_name.ToCharArray()).Count();
                    if (item.Length > 1 && sameCharCount > 0 && !dicMainWord.ContainsKey(item))
                        dicMainWord.Add(item, sameCharCount * item.Length);
                }

                if (dicMainWord.Count > 0)
                {//排序值最大，放最前
                    lstMainWord = lstMainWord.Union(dicMainWord.OrderByDescending(o => o.Value).Select(o => o.Key).ToList()).ToList();
                    isFindFirstMainWord = true;
                    dicMainWord = new Dictionary<string, int>();
                }
            //}

            //是否通过蜘蛛找到了同款和相似宝贝的关键词
            Boolean isFindKeywordBySpider = false;
            while ((lstMainWord.Count != 1) && (!isFindKeywordBySpider) && (dtStartFind.AddSeconds(30) >= DateTime.Now))
            {//类目找不到词或找到不只一个词，30秒内没找到放弃
                if (string.IsNullOrEmpty(strFindKeywordResult))
                {
                    strFindKeywordResult = CommonHandler.GetItemFindKeyword(itemOnline.item_id);
                }
                
                if (string.IsNullOrEmpty(strFindKeywordResult))
                {//暂时没有找到
                    Thread.Sleep(2000);
                    continue;
                }
                isFindKeywordBySpider = true;

                List<string> lstFindWord = strFindKeywordResult.Split(',').ToList();
                if (isFindFirstMainWord)
                {
                    //使用找词结果排序
                    foreach (var item in lstMainWord)
                    {
                        int intWordIndex = lstFindWord.FindIndex(o => o == item);
                        dicMainWord.Add(item, intWordIndex == -1 ? 9 : intWordIndex);   //不存在找词结果中的词，排最后
                    }
                    //排序值最小，放最前
                    lstMainWord = dicMainWord.OrderBy(o => o.Value).Select(o => o.Key).ToList();
                }
                else
                {
                    lstMainWord = lstFindWord.Take(2).ToList();
                }
            }

            frmOutPut.OutPutMsgFormat("宝贝第核心词：{0}", string.Join(",", lstMainWord));
        }


        private void btnGetKeywordIndex_Click(object sender, EventArgs e)
        {
            string strKeywords = txtKeywords.Text.Trim().Replace(" ",",");
            gridControlKeywordBase.DataSource = null;

            List<dynamic> lstKeywordBase = new List<dynamic>();
            var responseWordBase = CommonHandler.GetKeyWordBaseFromWs(strKeywords);
            for (int i = 0; i < responseWordBase.Count; i++)
            {
                if (responseWordBase[i].reord_base != null)
                {
                    List<TaoBaoDataServer.WinClientData.BusinessLayer.WService.EntityBaseInfo> lstEntityBaseInfo = responseWordBase[i].reord_base.ToList();
                    var tempLst = from o in lstEntityBaseInfo select new { o.impression, o.click, o.ctr, o.avg_price, o.competition, o.date, responseWordBase[i].word };
                    lstKeywordBase.AddRange(tempLst);
                }
            }
            gridControlKeywordBase.DataSource = lstKeywordBase;

            gridControlWordsData.DataSource = new SortableBindingList<Top.Api.Domain.InsightWordDataDTO>(CommonHandler.GetWordsDataFromWs(strKeywords, dtStartDate.Text, dtEndDate.Text));

            gridControlWordsSubData.DataSource = new SortableBindingList<Top.Api.Domain.InsightWordSubDataDTO>(CommonHandler.GetWordsSubDataFromWs(strKeywords, dtStartDate.Text, dtEndDate.Text));


        }


        private void gridViewCustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = Convert.ToString(e.RowHandle + 1);
            }
        }

        private void btnClearCache_Click(object sender, EventArgs e)
        {
            long itemId = Strings.GetItemId(txtNumID.Text);
            CommonHandler.RemoveItemFindKeywordCache(itemId);
            frmOutPut.OutPutMsgFormat("宝贝:{0},缓存清除完成", itemId);
        }

    }


}
