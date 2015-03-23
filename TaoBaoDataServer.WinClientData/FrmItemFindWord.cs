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
    public partial class FrmItemFindWord : MyDockContent
    {
        IOutPut frmOutPut;
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();

        public FrmItemFindWord(IOutPut _frmOutPut)
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

            //显示行号
            gridViewKeywordBase.IndicatorWidth = gridViewWordsData.IndicatorWidth = gridViewWordsSubData.IndicatorWidth = 40;
            gridViewKeywordBase.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewWordsData.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewWordsSubData.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            //排序完显示第一行
            gridViewKeywordBase.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewWordsData.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewWordsSubData.EndSorting += new EventHandler(gridViewEndSorting);
        }

        private void btnGetWord_Click(object sender, EventArgs e)
        {
            gridControlKeywordBase.DataSource = null;

            string strItemId = txtNumID.Text.Trim();
            EntityItem itemOnline = CommonHandler.GetItemOnline(strItemId);
            if (itemOnline == null || itemOnline.item_id == 0)
            {//获取宝贝失败，再次调用
                System.Threading.Thread.Sleep(100);
                itemOnline = CommonHandler.GetItemOnline(strItemId);
            }

            //标题分词在类目中出现的词，按重复字符数*长度排序，还要按照找词统计来排序
            List<string> lstMainWord = new List<string>();
            //蜘蛛抓取的关键词
            List<string> lstSpiderFindWord = new List<string>();

            Boolean isFindFirstMainWord = false;        //是否找到了第一核心词
            DateTime dtStartFind = DateTime.Now;

            string strFindKeywordResult = CommonHandler.GetItemFindKeyword(itemOnline.item_id);
            if (string.IsNullOrEmpty(strFindKeywordResult))
            {//发送宝贝找词消息
                string exchangeName = "ex_taobao_spider_samesimilar_item";
                BusinessMQ.SendMsgToExchange(null, exchangeName, string.Format("{0},{1},{2}", itemOnline.item_id, itemOnline.item_title, itemOnline.nick));
            }
            else
            {//缓存中获取抓取词
                lstSpiderFindWord = strFindKeywordResult.Split(',').ToList();
            }

            //获取类目热词
            List<string> lstCategoryWord = CommonHandler.GetCatTopKeyword(itemOnline.cid);

            string titleSplit = CommonHandler.SplitWordFromWs(itemOnline.item_title);
            frmOutPut.OutPutMsg(titleSplit);

            frmOutPut.OutPutMsgFormat("类目ID:{0},名称:{1}", itemOnline.cid, itemOnline.category_name);

            //将宝贝标题的分词按长度排序，在类目名称中的关键词作为核心词
            List<string> lstTitleWord = titleSplit.Split(',').OrderByDescending(o => o.Length).ToList();

            //核心词排序字典
            Dictionary<string, int> dicMainWord = new Dictionary<string, int>();
            foreach (var item in lstTitleWord)
            {//标题分词中，被类目名称包含的为核心词
                if (item.Length > 1 && itemOnline.category_name.Contains(item) && !lstMainWord.Contains(item))
                {
                    lstMainWord.Add(item);
                    isFindFirstMainWord = true;
                }
            }

            foreach (var item in lstTitleWord)
            {//标题分词中，和类目名称有交集的，为核心词 
                int sameCharCount=item.ToCharArray().Intersect(itemOnline.category_name.ToCharArray()).Count();
                if (item.Length > 1 && sameCharCount > 0 && !dicMainWord.ContainsKey(item))
                    dicMainWord.Add(item, sameCharCount * item.Length);
            }

            if (dicMainWord.Count > 0)
            {//核心词汇总，交集中重复字符越多，排序值最大，放最前
                lstMainWord = lstMainWord.Union(dicMainWord.OrderByDescending(o => o.Value).Select(o => o.Key).ToList()).ToList();
                isFindFirstMainWord = true;
                dicMainWord = new Dictionary<string, int>();
            }


            //是否通过蜘蛛找到了同款和相似宝贝的关键词
            Boolean isFindKeywordBySpider = false;
            while ((!isFindKeywordBySpider) && (dtStartFind.AddSeconds(30) >= DateTime.Now))
            {//类目找不到词或找到不只一个词，30秒内没找到放弃
                if (string.IsNullOrEmpty(strFindKeywordResult))
                {
                    strFindKeywordResult = CommonHandler.GetItemFindKeyword(itemOnline.item_id);
                }
                else
                {
                    frmOutPut.OutPutMsgFormat("宝贝抓取词：{0}", strFindKeywordResult);
                }
                
                if (string.IsNullOrEmpty(strFindKeywordResult))
                {//暂时没有找到
                    Thread.Sleep(2000);
                    continue;
                }
                isFindKeywordBySpider = true;

                lstSpiderFindWord = strFindKeywordResult.Split(',').ToList();
                if (isFindFirstMainWord)
                {
                    //使用找词结果排序
                    foreach (var item in lstMainWord)
                    {
                        int intWordIndex = lstSpiderFindWord.FindIndex(o => o == item);
                        dicMainWord.Add(item, intWordIndex == -1 ? 9 : intWordIndex);   //不存在找词结果中的词，排最后
                    }
                    //排序值最小，放最前
                    lstMainWord = dicMainWord.OrderBy(o => o.Value).Select(o => o.Key).ToList();
                }
                else
                {
                    lstMainWord = lstSpiderFindWord.Take(2).ToList();
                }
            }

            frmOutPut.OutPutMsgFormat("宝贝第核心词：{0}", string.Join(",", lstMainWord));

            txtKeywords.Text = strFindKeywordResult;

            //CombineWord(lstMainWord, lstFindWord.Union(lstTitleWord).Except(lstMainWord).ToList());
            CombineWord(itemOnline.item_title, lstMainWord, lstSpiderFindWord.Union(lstTitleWord).Union(lstCategoryWord).ToList());
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

        /// <summary>
        /// 组词，返回词指数
        /// </summary>
        /// <param name="itemTitle">宝贝标题</param>
        /// <param name="lstMainWord">核心词</param>
        /// <param name="lstOtherWord">属性词或类目热词</param>
        /// <returns></returns>
        private void CombineWord(string itemTitle, List<string> lstMainWord, List<string> lstOtherWord)
        {
            //关键词字典：键关键词，值权重
            Dictionary<string, int> dicResult = new Dictionary<string, int>();

            //属性词+核心词
            foreach (var itemMainWord in lstMainWord)
            {
                foreach (var itemOtherWord in lstOtherWord)
                {
                    if (itemOtherWord != itemMainWord)
                    {
                        if (!dicResult.ContainsKey(itemOtherWord + itemMainWord))
                        {
                            dicResult.Add(itemOtherWord + itemMainWord, 10);
                        }
                    }
                }

                //核心词
                if (!dicResult.ContainsKey(itemMainWord))
                {
                    dicResult.Add(itemMainWord, 8);
                }
            }

            //核心词，属性词+核心词，淘宝拓展
            string strKeywordTopExtend = string.Join(",", dicResult.Select(o => o.Key));
            string strExtendWords = CommonHandler.GetRelatedwordsByKeyword(strKeywordTopExtend);
            if (!string.IsNullOrEmpty(strExtendWords))
            {
                string[] arrRelWord = strExtendWords.Split(',');
                foreach (var itemRelWord in arrRelWord)
                {
                    if (!dicResult.ContainsKey(itemRelWord))
                    {
                        dicResult.Add(itemRelWord, 7);
                    }
                }
            }

            //TODO淘宝拓词后，没有核心词的词可以和核心词组词
            //TODO标题词的分词，可以用属性词+属性词+核心词组

            int wordCount = dicResult.Count;
            frmOutPut.OutPutMsgFormat("组词总数量：{0}", wordCount);
            string strMainWord = string.Join("", lstMainWord);
            string strKeywords = string.Join(",", dicResult.Select(o => o.Key).Distinct());

            List<dynamic> lstKeywordBase = new List<dynamic>();
            var responseWordBase = CommonHandler.GetKeyWordBaseFromWs(strKeywords);
            for (int i = 0; i < responseWordBase.Count; i++)
            {
                if (responseWordBase[i].reord_base != null)
                {
                    List<TaoBaoDataServer.WinClientData.BusinessLayer.WService.EntityBaseInfo> lstEntityBaseInfo = responseWordBase[i].reord_base.ToList();
                    //关键词相似度
                    char[] arrItemWord = responseWordBase[i].word.ToCharArray();
                    decimal matchDegree = strMainWord.ToCharArray().Intersect(arrItemWord).Count() / strMainWord.Length * 1.00M
                                            + 0.1M * (itemTitle.ToCharArray().Intersect(arrItemWord).Count());

                    var tempLst = new
                    {
                        word = responseWordBase[i].word,
                        impression = (long)lstEntityBaseInfo.Average(o => o.impression),
                        click = (long)lstEntityBaseInfo.Average(o => o.click),
                        ctr = Math.Round(lstEntityBaseInfo.Average(o => o.impression) == 0 ? 0 : lstEntityBaseInfo.Average(o => o.click) / lstEntityBaseInfo.Average(o => o.impression), 2),
                        avg_price = (long)lstEntityBaseInfo.Average(o => o.avg_price),
                        competition = (long)lstEntityBaseInfo.Average(o => o.competition),
                        order = Math.Round(matchDegree, 2)
                    };

                    lstKeywordBase.Add(tempLst);
                }
            }

            //按照相似度，展现排序
            gridControlKeywordBase.DataSource = lstKeywordBase.OrderByDescending(o => o.impression).OrderByDescending(o => o.order).ToList();

        }

        private void btnClearCache_Click(object sender, EventArgs e)
        {
            long itemId = Strings.GetItemId(txtNumID.Text);
            CommonHandler.RemoveItemFindKeywordCache(itemId);
            frmOutPut.OutPutMsgFormat("宝贝:{0},缓存清除完成", itemId);
        }

    }


}
