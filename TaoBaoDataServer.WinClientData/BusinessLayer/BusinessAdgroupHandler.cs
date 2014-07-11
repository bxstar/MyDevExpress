using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Top.Api.Domain;
using TaoBaoDataServer.WinClientData.Model;
using log4net;
using iclickpro.AccessCommon;
using System.Data.SqlClient;
using System.Data;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class BusinessAdgroupHandler
    {
        private static log4net.ILog logger = LogManager.GetLogger("loggerAX");
        BusinessTaobaoApiHandler taobaoApiHandler = new BusinessTaobaoApiHandler();

        /// <summary>
        /// 线上，获取推广组
        /// </summary>
        public List<ADGroup> GetAdgroupOnline(TopSession session, long campaignId)
        {
            // 定义返回值
            List<ADGroup> listADGroup = new List<ADGroup>();

            // 按照一页取200条数据，获取计划下的推广组信息
            int pageSize = 200;
            // 取得第一页的推广组信息
            DateTime dtStart = DateTime.Now;
            var response = taobaoApiHandler.TaobaoSimbaAdgroupsGetByCampaignId(session, campaignId, pageSize, 1);
            if (response.IsError)
            {
                if (CommonHandler.IsBanMsg(response))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        System.Threading.Thread.Sleep(2000);
                        response = taobaoApiHandler.TaobaoSimbaAdgroupsGetByCampaignId(session, campaignId, pageSize, 1);
                        if (response.IsError && CommonHandler.IsBanMsg(response) && dtStart.AddMinutes(5) > DateTime.Now)
                        {//超过5分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddMinutes(5) <= DateTime.Now)
                            {
                                logger.Error("线上创建一批关键词出错，已重试5分钟" + response.Body);
                            }
                            isBanError = false;
                        }
                    }
                }
                else
                {
                    logger.Error("BusinessBatchHandler/GetAdgroup,淘宝API下载推广组数据 失败：" + response.Body);
                }
            }

            // 循环取得推广组信息
            if (response != null && response.Adgroups != null && response.Adgroups.AdgroupList.Count > 0)
            {
                listADGroup.AddRange(response.Adgroups.AdgroupList);
                int nCount = (Int32)response.Adgroups.TotalItem;
                // 计算页数，按照计算的页数，分页获取推广组信息
                var rowCount = CommonFunction.getPageCount(nCount, pageSize);
                if (rowCount > 1)
                {
                    // 循环获取第二页后面的推广组数据
                    for (int i = 2; i < rowCount + 1; i++)
                    {
                        var nextAdgroupList = taobaoApiHandler.TaobaoSimbaAdgroupsGetByCampaignId(session, campaignId, pageSize, i);
                        if (nextAdgroupList != null && nextAdgroupList.Adgroups != null && nextAdgroupList.Adgroups.AdgroupList.Count > 0)
                        {
                            listADGroup.AddRange(nextAdgroupList.Adgroups.AdgroupList);
                        }
                    }
                }
            }

            return listADGroup;
        }

        /// <summary>
        /// 线上，获取推广组信息（根据推广组ID）
        /// </summary>
        public ADGroup GetAdgroupOnlineByAdgroupId(TopSession session, long adgroupId)
        {
            List<long> adgroupIds = new List<long>();
            adgroupIds.Add(adgroupId);
            var response = taobaoApiHandler.TaobaoSimbaAdgroupsByAdgroupIds(session, adgroupIds);
            if (response != null && !response.IsError && response.Adgroups != null && response.Adgroups.AdgroupList != null && response.Adgroups.AdgroupList.Count > 0)
                return response.Adgroups.AdgroupList[0];
            else
                return null;
        }

        #region 取得用户选择的推广宝贝

        /// <summary>
        /// 数据库，获取用户选择的推广组
        /// </summary>
        public List<EntityAdgroup> GetAdAdgroup(int userId, long campaignId)
        {
            List<EntityAdgroup> listAdgroup = new List<EntityAdgroup>();
            try
            {
                // 更新用户的状态
                var param = new Dictionary<string, object>();
                param.Add("user_id", userId);
                param.Add("campaign_id", campaignId);
                // 更新数据库影响的行数
                var dsAdgroup = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), "SELECT * FROM ad_adgroup where [user_id]=@user_id and campaign_id = @campaign_id", SqlNameAndParamer.ConvertSqlParameter(param));
                if (dsAdgroup != null && dsAdgroup.Tables.Count > 0 && dsAdgroup.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsAdgroup.Tables[0].Rows.Count; i++)
                    {
                        EntityAdgroup adgroup = new EntityAdgroup();
                        adgroup.localadgroupid = Convert.ToInt32(dsAdgroup.Tables[0].Rows[i]["local_adgroup_id"]);
                        adgroup.adgroupid = Convert.ToInt64(dsAdgroup.Tables[0].Rows[i]["adgroup_id"]);
                        adgroup.campaignid = Convert.ToInt64(dsAdgroup.Tables[0].Rows[i]["campaign_id"]);
                        adgroup.categoryids = dsAdgroup.Tables[0].Rows[i]["category_ids"].ToString();
                        adgroup.defaultprice = Convert.ToDecimal(dsAdgroup.Tables[0].Rows[i]["default_price"]);
                        adgroup.numiid = Convert.ToInt64(dsAdgroup.Tables[0].Rows[i]["num_iid"]);
                        adgroup.offlinetype = dsAdgroup.Tables[0].Rows[i]["offline_type"].ToString();
                        adgroup.onlinestatus = dsAdgroup.Tables[0].Rows[i]["online_status"].ToString();
                        adgroup.userid = dsAdgroup.Tables[0].Rows[i]["user_id"].ToString();
                        adgroup.IsEnableMajorization = Convert.ToBoolean(dsAdgroup.Tables[0].Rows[i]["IsEnableMajorization"]);
                        listAdgroup.Add(adgroup);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetAdAdgroup,从数据库中取得用户选择的推广组信息：", ex);
            }
            return listAdgroup;
        }
        #endregion

        /// <summary>
        /// 删除指定推广组
        /// </summary>
        public void DeleteAdgroup(long adgroupId)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            var param = new Dictionary<string, object>();
            param.Add("adgroup_id", adgroupId);
            SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), "DELETE FROM ad_adgroup WHERE adgroup_id = @adgroup_id;", SqlNameAndParamer.ConvertSqlParameter(param));
        }

        /// <summary>
        /// 删除推广组信息
        /// </summary>
        public void DeleteAdgroup(int userId)
        {
            // 参数设置
            var param = new Dictionary<string, object>();
            param.Add("user_id", userId);
            const string sqlText = @"delete from ad_adgroup where user_id=@user_id ";
            // 执行删除操作
            SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), sqlText, SqlNameAndParamer.ConvertSqlParameter(param));
        }

        /// <summary>
        /// 更新指定推广组的状态
        /// </summary>
        public void UpdateAdgroupStatus(long adgroupId, string onlineStatus, string offlineType)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            var param = new Dictionary<string, object>();
            param.Add("adgroup_id", adgroupId);
            param.Add("online_status", onlineStatus);
            param.Add("offline_type", offlineType);
            SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), "UPDATE ad_adgroup SET online_status = @online_status,offline_type=@offline_type,update_date=getdate() WHERE adgroup_id = @adgroup_id;", SqlNameAndParamer.ConvertSqlParameter(param));
        }

        /// <summary>
        /// 数据库，推广组信息入库
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="adgroup">推广组信息</param>
        public void AddAdgroup(int userId, ADGroup adgroup)
        {
            const string sqlText = @"
IF not exists(SELECT 1 FROM ad_adgroup WHERE  adgroup_id = @adgroup_id)
insert into ad_adgroup
                                                (adgroup_id
                                                ,campaign_id
                                                ,num_iid
                                                ,category_ids
                                                ,default_price
                                                ,offline_type
                                                ,online_status
                                                ,[user_id]
                                                ,create_date
                                                ,update_date)
                                         values
                                               (@adgroup_id
                                               ,@campaign_id
                                               ,@num_iid
                                               ,@category_ids
                                               ,@default_price
                                               ,@offline_type
                                               ,@online_status
                                               ,@user_id
                                               ,getdate()
                                               ,getdate())";


            var lstParameter = new List<SqlParameter>
                                       {
                                           new SqlParameter("@adgroup_id", Convert.ToInt64(adgroup.AdgroupId)),
                                           new SqlParameter("@campaign_id",Convert.ToInt64(adgroup.CampaignId)),
                                           new SqlParameter("@num_iid",Convert.ToInt64(adgroup.NumIid)),
                                           new SqlParameter("@category_ids", adgroup.CategoryIds),
                                           new SqlParameter("@default_price",Convert.ToDecimal(adgroup.DefaultPrice)/100),
                                           new SqlParameter("@offline_type", adgroup.OfflineType),
                                           new SqlParameter("@online_status",adgroup.OnlineStatus),
                                           new SqlParameter("@user_id", userId)
                                       };
            SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), sqlText, lstParameter.ToArray());
        }

        /// <summary>
        /// 数据库，根据推广组ID获取推广组信息
        /// </summary>
        public Top.Api.Domain.ADGroup GetAdgroupByAdgroupId(long adgroupId)
        {
            DataSet ds = new DataSet();

            Top.Api.Domain.ADGroup result = new Top.Api.Domain.ADGroup();
            string strSql = string.Format(@"SELECT * FROM ad_adgroup where adgroup_id={0}", adgroupId);
            
            ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), strSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result.AdgroupId = Convert.ToInt64(ds.Tables[0].Rows[0]["adgroup_id"].ToString());
                result.CampaignId = Convert.ToInt64(ds.Tables[0].Rows[0]["campaign_id"].ToString());
                result.NumIid = Convert.ToInt64(ds.Tables[0].Rows[0]["num_iid"].ToString());
                result.CategoryIds = ds.Tables[0].Rows[0]["category_ids"].ToString();
            }

            return result;
        }
    }
}
