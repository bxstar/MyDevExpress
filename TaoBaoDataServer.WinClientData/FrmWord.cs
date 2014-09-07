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
            dtStartDate.Value = DateTime.Now.AddDays(-7);
            dtEndDate.Value = DateTime.Now.AddDays(-1);
        }

        private void btnGetWord_Click(object sender, EventArgs e)
        {

        }


        private void btnGetKeywordIndex_Click(object sender, EventArgs e)
        {
            string strKeywords = txtKeywords.Text.Trim();
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

    }


}
