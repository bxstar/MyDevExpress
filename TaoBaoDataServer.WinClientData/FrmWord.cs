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
            //发送宝贝找词消息
            string exchangeName = "ex_taobao_spider_samesimilar_item";
            BusinessMQ.SendMsgToExchange(null, exchangeName, string.Format("{0},{1},{2}", itemOnline.item_id, itemOnline.item_title, itemOnline.nick));


            string titleSplit = CommonHandler.SplitWordFromWs(itemOnline.item_title);
            frmOutPut.OutPutMsg(titleSplit);

            frmOutPut.OutPutMsgFormat("类目名次：{0}", itemOnline.categroy_name);

            //将宝贝标题的分词按长度排序，在类目名称中的关键词作为核心词
            List<string> lstTitleWord = titleSplit.Split(',').OrderByDescending(o => o.Length).ToList();
            //TODO标题分词在类目中出现的词，除按长度排序外，还要按照找词统计来排序
            string firstMainWord = lstTitleWord.Find(o => itemOnline.categroy_name.Contains(o));
            if (firstMainWord != null)
            {
                frmOutPut.OutPutMsgFormat("宝贝第一核心词：{0}", firstMainWord);
                isFindFirstMainWord = true;
            }

            //是否通过蜘蛛找到了同款和相似宝贝的关键词
            Boolean isFindKeywordBySpider = false;
            while ((!isFindFirstMainWord) && (!isFindKeywordBySpider) && (dtStartFind.AddSeconds(30) >= DateTime.Now))
            {//30秒内没找到放弃
                string strFindKeywordResult = CommonHandler.GetItemFindKeyword(itemOnline.item_id);
                if (string.IsNullOrEmpty(strFindKeywordResult))
                {//暂时没有找到
                    Thread.Sleep(2000);
                    continue;
                }
                isFindKeywordBySpider = true;

                List<string> lstFindWord = strFindKeywordResult.Split(',').ToList();
                firstMainWord = lstFindWord.Find(o => itemOnline.categroy_name.Contains(o));
                if (firstMainWord == null)
                {
                    firstMainWord = lstFindWord.First();
                }

                frmOutPut.OutPutMsgFormat("宝贝第一核心词：{0}", firstMainWord);
            }

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

    }


}
