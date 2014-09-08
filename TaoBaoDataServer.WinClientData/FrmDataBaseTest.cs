using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaoBaoDataServer.WinClientData.Model;
using WeifenLuo.WinFormsUI.Docking;
using TaoBaoDataServer.WinClientData.BusinessLayer;
using Top.Api.Domain;
using log4net;

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmDataBaseTest : DockContent
    {
        private static log4net.ILog logger = LogManager.GetLogger("Logger");
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();
        BusinessTaobaoApiHandler taobaoHandler = new BusinessTaobaoApiHandler();
        BusinessUserHandler userHandler = new BusinessUserHandler();

        public FrmDataBaseTest()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            List<TopSession> lstUser = userHandler.GetUserInfo(null).Where(o => o.UserName == o.ProxyUserName).ToList();

            List<EntityKeywordEx> lstData = SqlDataProvider.GetDataFromDB<EntityKeywordEx>("ad_keyword_modify_batch");

            foreach (var k in lstData)
            {

                TopSession user = lstUser.Find(o => o.UserID == k.user_id);
                if (user != null)
                {

                    List<Keyword> lstKeywordModify = lstData.Where(o => o.keyword_id == k.keyword_id).Select(o => new Keyword() { CampaignId = o.campaign_id, AdgroupId = o.adgroup_id, KeywordId = o.keyword_id, Word = o.keyword, MaxPrice = o.max_price, MatchScope = "4" }).ToList();

                    var result = keywordHandler.ModifyKeywordOnline(user, lstKeywordModify);
                    if (result.IsError)
                    {
                        logger.ErrorFormat("keyword_id：{0}，改价价格出错，原因：{1}", k.keyword_id, result.ErrorMessage);
                    }
                }
            }

            MessageBox.Show("批量修改价格完成");
        }

        private void btnAdgroupRevert_Click(object sender, EventArgs e)
        {
            /*
             数据导入语句
             INSERT INTO batch_item_modify
(
	item_id,
	[user_id],
	campaign_id,
	item_title,
	item_url
)
SELECT aa.num_iid,aa.[user_id],aa.campaign_id,ac.title,ac.img_url
  FROM ad_adgroup aa LEFT JOIN 
(select * from ad_creative WHERE ad_creative.local_creative_id %2=0 AND ad_creative.[user_id]=2480) ac ON ac.adgroup_id = aa.adgroup_id 

WHERE aa.[user_id]=2480 AND aa.adgroup_id<>343774331
            */

            List<TopSession> lstUser = userHandler.GetUserInfo(null).Where(o => o.UserName == o.ProxyUserName).ToList();

            //List<EntityItem> lstData = SqlDataProvider.GetDataFromDB<EntityItem>("batch_item_modify");
            //foreach (var item in lstData)
            //{
            //    TopSession user = lstUser.Find(o => o.UserID == item.user_id);
            //    if (user != null)
            //    {
            //        var result = taobaoHandler.TaobaoSimbaAdgroupAdd(user, item.campaign_id, item.item_id, 20, item.item_title, item.item_url.Replace("_sum.jpg", ""));
            //        if (result.IsError)
            //        {
            //            logger.ErrorFormat("item_id：{0}，item_title：{1}，宝贝恢复出错，原因：{2}", item.item_id, item.item_title, result.SubErrMsg);
            //        }
            //    }


            //}

            MessageBox.Show("宝贝恢复完成");
        }
    }
}
