using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Top.Api.Domain;
using TaoBaoDataServer.WinClientData.Model;
using log4net;
using iclickpro.AccessCommon;
using Top.Api.Response;
using System.Data.SqlClient;
using System.Data;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class BusinessAdgroupHandler
    {
        private static log4net.ILog logger = LogManager.GetLogger("Logger");
        BusinessTaobaoApiHandler taobaoApiHandler = new BusinessTaobaoApiHandler();

        /// <summary>
        /// 线上，获取推广组
        /// </summary>
        public List<ADGroup> GetAdgroupOnline(TopSession session, long campaignId)
        {
            // 定义返回值
            List<ADGroup> lstAdgroup = new List<ADGroup>();

            // 按照一页取200条数据，获取计划下的推广组信息
            long pageIndex = 1;
            long pageSize = 200;
            // 取得第一页的推广组信息
            DateTime dtStart = DateTime.Now;
            var response = CommonHandler.DoTaoBaoApi<SimbaAdgroupsbycampaignidGetResponse>(taobaoApiHandler.TaobaoSimbaAdgroupsGetByCampaignId, session, campaignId, pageSize, pageIndex);
            if (response == null || response.IsError)
            {
                logger.Error("线上获取计划的推广组失败：" + response != null ? response.Body : string.Empty);
                return lstAdgroup;
            }

            // 循环取得推广组信息
            if (response != null && response.Adgroups != null && response.Adgroups.AdgroupList != null && response.Adgroups.AdgroupList.Count > 0)
            {
                lstAdgroup = response.Adgroups.AdgroupList;
                // 计算页数，按照计算的页数，分页获取推广组信息
                int rowCount = CommonFunction.getPageCount(Convert.ToInt32(response.Adgroups.TotalItem), Convert.ToInt32(pageSize));
                if (rowCount > 1)
                {
                    // 循环获取第二页后面的推广组数据
                    for (pageIndex = 2; pageIndex < rowCount + 1; pageIndex++)
                    {
                        var nextAdgroupList = CommonHandler.DoTaoBaoApi<SimbaAdgroupsbycampaignidGetResponse>(taobaoApiHandler.TaobaoSimbaAdgroupsGetByCampaignId, session, campaignId, pageSize, pageIndex);
                        if (nextAdgroupList != null && nextAdgroupList.Adgroups != null && nextAdgroupList.Adgroups.AdgroupList != null && nextAdgroupList.Adgroups.AdgroupList.Count > 0)
                        {
                            lstAdgroup.AddRange(nextAdgroupList.Adgroups.AdgroupList);
                        }
                    }
                }
            }

            return lstAdgroup;
        }

        /// <summary>
        /// 线上，获取推广组信息（根据推广组ID）
        /// </summary>
        public ADGroup GetAdgroupOnlineByAdgroupId(TopSession session, long adgroupId)
        {
            List<long> adgroupIds = new List<long>();
            adgroupIds.Add(adgroupId);
            var response = CommonHandler.DoTaoBaoApi<SimbaAdgroupsbyadgroupidsGetResponse>(taobaoApiHandler.TaobaoSimbaAdgroupsByAdgroupIds, session, adgroupIds);
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

        /// <summary>
        /// 数据库，获取宝贝信息
        /// </summary>
        public EntityItem GetItem(long itemId)
        {
            EntityItem item = new EntityItem();
            string strSql = "select * from ad_item where item_id=" + itemId;
            DataSet ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), strSql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                item.item_id = itemId;
                item.item_title = dr["item_title"].ToString();
                item.local_id = Convert.ToInt32(dr["local_id"]);
                if (dr["cid"] != null && dr["cid"] != DBNull.Value)
                {
                    long cid = 0;
                    Int64.TryParse(dr["cid"].ToString(), out cid);
                    item.cid = cid;
                }
                item.category_name = dr["category_name"].ToString();
            }

            return item;
        }

        /// <summary>
        /// 下载推广组报表，最近多少天
        /// </summary>
        public List<EntityAdgroupReport> DownLoadAdgroupReport(TopSession session, long campaignId, long adgroupId, int days)
        {
            string strStartDay = DateTime.Now.AddDays(0 - days).Date.ToString("yyyy-MM-dd");
            string strEndDay = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
            return DownLoadAdgroupReport(session, campaignId, adgroupId, strStartDay, strEndDay);
        }

        /// <summary>
        /// 下载推广组报表，时间段
        /// </summary>
        public List<EntityAdgroupReport> DownLoadAdgroupReport(TopSession session, long campaignId, long adgroupId, string strStartDay, string strEndDay)
        {
            List<EntityAdgroupReport> lstAll = new List<EntityAdgroupReport>();
            //获取推广组基础数据
            long pageSize = 500;
            long i = 0;
            while (true)
            {
                i = i + 1;
                int returnPageSize = 0;
                string jsonBaseRpt = DownLoadAdgroupBaseReport(session, campaignId, adgroupId, strStartDay, strEndDay).ToLower();
                if (!string.IsNullOrEmpty(jsonBaseRpt) && jsonBaseRpt.Length > 2)
                {
                    var data = new DynamicJsonParser().FromJson(jsonBaseRpt);
                    foreach (var item in data)
                    {
                        EntityAdgroupReport rpt = new EntityAdgroupReport();
                        rpt.date = Convert.ToString(item.date);
                        rpt.campaign_id = Convert.ToInt64(item.campaignid);
                        rpt.adgroup_id = Convert.ToInt64(item.adgroupid);
                        rpt.impressions = item.impressions == null ? 0 : Convert.ToInt32(item.impressions);
                        rpt.click = item.click == null ? 0 : Convert.ToInt32(item.click);
                        rpt.ctr = item.ctr == null ? 0M : Convert.ToDecimal(item.ctr);
                        rpt.cost = item.cost == null ? 0M : Convert.ToDecimal(item.cost);
                        rpt.cpc = item.cpc == null ? 0M : Convert.ToDecimal(item.cpc);
                        rpt.avgpos = item.avgpos == null ? 0M : Convert.ToDecimal(item.avgpos);
                        rpt.source = Convert.ToString(item.source);
                        lstAll.Add(rpt);
                        returnPageSize++;
                    }
                }

                // 返回的行数小于500的时候，说明到了最后一页，返回
                if (returnPageSize < pageSize)
                {
                    break;
                }
            }

            //获取推广组效果数据
            i = 0;
            while (true)
            {
                i = i + 1;
                int returnPageSize = 0;
                string jsonEffectRpt = DownLoadAdgroupEffectReport(session, campaignId, adgroupId, strStartDay, strEndDay).ToLower();
                if (!string.IsNullOrEmpty(jsonEffectRpt) && jsonEffectRpt.Length > 2)
                {
                    var data = new DynamicJsonParser().FromJson(jsonEffectRpt);
                    foreach (var item in data)
                    {
                        EntityAdgroupReport rpt = lstAll.Find(o => o.adgroup_id == item.adgroupid && o.date == item.date);
                        if (rpt == null)
                        {
                            logger.ErrorFormat("获取推广组报表有误，{0}", jsonEffectRpt);
                            continue;
                        }
                        rpt.directpay = item.directpay == null ? 0M : Convert.ToDecimal(item.directpay);
                        rpt.indirectpay = item.indirectpay == null ? 0M : Convert.ToDecimal(item.indirectpay);
                        rpt.directpaycount = item.directpaycount == null ? 0 : Convert.ToInt32(item.directpaycount);
                        rpt.indirectpaycount = item.indirectpaycount == null ? 0 : Convert.ToInt32(item.indirectpaycount);
                        rpt.favitemcount = item.favitemcount == null ? 0 : Convert.ToInt32(item.favitemcount);
                        rpt.favshopcount = item.favshopcount == null ? 0 : Convert.ToInt32(item.favshopcount);
                        rpt.roi = rpt.cost == 0M ? 0M : Math.Round((rpt.directpay + rpt.indirectpay) / rpt.cost, 2);
                        returnPageSize++;
                    }
                }

                // 返回的行数小于500的时候，说明到了最后一页，返回
                if (returnPageSize < pageSize)
                {
                    break;
                }
            }

            return lstAll;
        }

        /// <summary>
        /// 下载推广组的基础报表
        /// </summary>
        private string DownLoadAdgroupBaseReport(TopSession session, long campaignId, long adgroupId, string strStartDay, string strEndDay)
        {
            var response = CommonHandler.DoTaoBaoApi<SimbaRptAdgroupbaseGetResponse>(taobaoApiHandler.TaobaoSimbaRptAdgroupbaseGet, session, campaignId, adgroupId, strStartDay, strEndDay);
            return response == null ? string.Empty : response.RptAdgroupBaseList ?? string.Empty;
        }

        /// <summary>
        /// 下载推广组的效果报表
        /// </summary>
        private string DownLoadAdgroupEffectReport(TopSession session, long campaignId, long adgroupId, string strStartDay, string strEndDay)
        {
            var response = CommonHandler.DoTaoBaoApi<SimbaRptAdgroupeffectGetResponse>(taobaoApiHandler.TaobaoSimbaRptAdgroupeffectGet, session, campaignId, adgroupId, strStartDay, strEndDay);
            return response == null ? string.Empty : response.RptAdgroupEffectList ?? string.Empty;
        }

        /// <summary>
        /// 下载推广计划下所有推广组报表，最近多少天
        /// </summary>
        public List<EntityAdgroupReport> DownLoadAdgroupReportByCampaign(TopSession session, long campaignId, int days)
        {
            string strStartDay = DateTime.Now.AddDays(0 - days).Date.ToString("yyyy-MM-dd");
            string strEndDay = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
            return DownLoadAdgroupReportByCampaign(session, campaignId, strStartDay, strEndDay);
        }

        /// <summary>
        /// 下载推广计划下所有推广组报表，时间段
        /// </summary>
        public List<EntityAdgroupReport> DownLoadAdgroupReportByCampaign(TopSession session, long campaignId, string strStartDay, string strEndDay)
        {
            List<EntityAdgroupReport> lstAll = new List<EntityAdgroupReport>();
            //获取推广组基础数据
            long pageSize = 500;
            long i = 0;
            while (true)
            {
                i = i + 1;
                int returnPageSize = 0;
                string jsonBaseRpt = DownLoadAdgroupBaseReportByCampaign(session, campaignId, strStartDay, strEndDay).ToLower();
                if (!string.IsNullOrEmpty(jsonBaseRpt) && jsonBaseRpt.Length > 2)
                {
                    var data = new DynamicJsonParser().FromJson(jsonBaseRpt);
                    foreach (var item in data)
                    {
                        EntityAdgroupReport rpt = new EntityAdgroupReport();
                        rpt.date = Convert.ToString(item.date);
                        rpt.campaign_id = Convert.ToInt64(item.campaignid);
                        rpt.adgroup_id = Convert.ToInt64(item.adgroupid);
                        rpt.impressions = item.impressions == null ? 0 : Convert.ToInt32(item.impressions);
                        rpt.click = item.click == null ? 0 : Convert.ToInt32(item.click);
                        rpt.ctr = item.ctr == null ? 0M : Convert.ToDecimal(item.ctr);
                        rpt.cost = item.cost == null ? 0M : Convert.ToDecimal(item.cost);
                        rpt.cpc = item.cpc == null ? 0M : Convert.ToDecimal(item.cpc);
                        rpt.avgpos = item.avgpos == null ? 0M : Convert.ToDecimal(item.avgpos);
                        rpt.source = Convert.ToString(item.source);
                        lstAll.Add(rpt);
                        returnPageSize++;
                    }
                }

                // 返回的行数小于500的时候，说明到了最后一页，返回
                if (returnPageSize < pageSize)
                {
                    break;
                }
            }

            //获取推广组效果数据
            i = 0;
            while (true)
            {
                i = i + 1;
                int returnPageSize = 0;
                string jsonEffectRpt = DownLoadAdgroupEffectReportByCampaign(session, campaignId, strStartDay, strEndDay).ToLower();
                if (!string.IsNullOrEmpty(jsonEffectRpt) && jsonEffectRpt.Length > 2)
                {
                    var data = new DynamicJsonParser().FromJson(jsonEffectRpt);
                    foreach (var item in data)
                    {
                        EntityAdgroupReport rpt = lstAll.Find(o => o.adgroup_id == item.adgroupid && o.date == item.date);
                        if (rpt == null)
                        {
                            logger.ErrorFormat("获取推广组报表有误，{0}", jsonEffectRpt);
                            continue;
                        }
                        rpt.directpay = item.directpay == null ? 0M : Convert.ToDecimal(item.directpay);
                        rpt.indirectpay = item.indirectpay == null ? 0M : Convert.ToDecimal(item.indirectpay);
                        rpt.directpaycount = item.directpaycount == null ? 0 : Convert.ToInt32(item.directpaycount);
                        rpt.indirectpaycount = item.indirectpaycount == null ? 0 : Convert.ToInt32(item.indirectpaycount);
                        rpt.favitemcount = item.favitemcount == null ? 0 : Convert.ToInt32(item.favitemcount);
                        rpt.favshopcount = item.favshopcount == null ? 0 : Convert.ToInt32(item.favshopcount);
                        rpt.roi = rpt.cost == 0M ? 0M : Math.Round((rpt.directpay + rpt.indirectpay) / rpt.cost, 2);
                        returnPageSize++;
                    }
                }

                // 返回的行数小于500的时候，说明到了最后一页，返回
                if (returnPageSize < pageSize)
                {
                    break;
                }
            }

            return lstAll;
        }


        /// <summary>
        /// 下载推广计划下，所有推广组的基础报表
        /// </summary>
        private string DownLoadAdgroupBaseReportByCampaign(TopSession session, long campaignId, string strStartDay, string strEndDay)
        {
            var response = CommonHandler.DoTaoBaoApi<SimbaRptCampadgroupbaseGetResponse>(taobaoApiHandler.TaobaoSimbaRptCampadgroupbaseGet, session, campaignId, strStartDay, strEndDay);
            return response == null ? string.Empty : response.RptCampadgroupBaseList ?? string.Empty;
        }

        /// <summary>
        /// 下载推广计划下，所有推广组的效果报表
        /// </summary>
        private string DownLoadAdgroupEffectReportByCampaign(TopSession session, long campaignId, string strStartDay, string strEndDay)
        {
            var response = CommonHandler.DoTaoBaoApi<SimbaRptCampadgroupeffectGetResponse>(taobaoApiHandler.TaobaoSimbaRptCampadgroupeffectGet, session, campaignId, strStartDay, strEndDay);
            return response == null ? string.Empty : response.RptCampadgroupEffectList ?? string.Empty;
        }

        /// <summary>
        /// 下载推广组的创意报表，最近多少天
        /// </summary>
        public List<EntityCreativeReport> DownLoadCreativeReport(TopSession session, long campaignId, long adgroupId, int days)
        {
            string strStartDay = DateTime.Now.AddDays(0 - days).Date.ToString("yyyy-MM-dd");
            string strEndDay = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
            return DownLoadCreativeReport(session, campaignId, adgroupId, strStartDay, strEndDay);
        }

        /// <summary>
        /// 下载推广组的创意报表，时间段
        /// </summary>
        public List<EntityCreativeReport> DownLoadCreativeReport(TopSession session, long campaignId, long adgroupId, string strStartDay, string strEndDay)
        {
            List<EntityCreativeReport> lstAll = new List<EntityCreativeReport>();
            //获取推广组的创意基础数据
            long pageSize = 500;
            long i = 0;
            while (true)
            {
                i = i + 1;
                int returnPageSize = 0;
                string jsonBaseRpt = DownLoadCreativeBaseReport(session, campaignId, adgroupId, strStartDay, strEndDay).ToLower();
                if (!string.IsNullOrEmpty(jsonBaseRpt) && jsonBaseRpt.Length > 2)
                {
                    var data = new DynamicJsonParser().FromJson(jsonBaseRpt);
                    foreach (var item in data)
                    {
                        EntityCreativeReport rpt = new EntityCreativeReport();
                        rpt.date = Convert.ToString(item.date);
                        rpt.campaign_id = Convert.ToInt64(item.campaignid);
                        rpt.adgroup_id = Convert.ToInt64(item.adgroupid);
                        rpt.creative_id = Convert.ToInt64(item.creativeid);
                        rpt.impressions = item.impressions == null ? 0 : Convert.ToInt32(item.impressions);
                        rpt.click = item.click == null ? 0 : Convert.ToInt32(item.click);
                        rpt.ctr = item.ctr == null ? 0M : Convert.ToDecimal(item.ctr);
                        rpt.cost = item.cost == null ? 0M : Convert.ToDecimal(item.cost);
                        rpt.cpc = item.cpc == null ? 0M : Convert.ToDecimal(item.cpc);
                        rpt.avgpos = item.avgpos == null ? 0M : Convert.ToDecimal(item.avgpos);
                        rpt.source = Convert.ToString(item.source);
                        lstAll.Add(rpt);
                        returnPageSize++;
                    }
                }

                // 返回的行数小于500的时候，说明到了最后一页，返回
                if (returnPageSize < pageSize)
                {
                    break;
                }
            }

            //获取推广组效果数据
            i = 0;
            while (true)
            {
                i = i + 1;
                int returnPageSize = 0;
                string jsonEffectRpt = DownLoadCreativeEffectReport(session, campaignId, adgroupId, strStartDay, strEndDay).ToLower();
                if (!string.IsNullOrEmpty(jsonEffectRpt) && jsonEffectRpt.Length > 2)
                {
                    var data = new DynamicJsonParser().FromJson(jsonEffectRpt);
                    foreach (var item in data)
                    {
                        EntityCreativeReport rpt = lstAll.Find(o => o.adgroup_id == item.adgroupid && o.creative_id == item.creativeid && o.date == item.date);
                        if (rpt == null)
                        {
                            logger.ErrorFormat("获取推广组创意报表有误，{0}", jsonEffectRpt);
                            continue;
                        }
                        rpt.directpay = item.directpay == null ? 0M : Convert.ToDecimal(item.directpay);
                        rpt.indirectpay = item.indirectpay == null ? 0M : Convert.ToDecimal(item.indirectpay);
                        rpt.directpaycount = item.directpaycount == null ? 0 : Convert.ToInt32(item.directpaycount);
                        rpt.indirectpaycount = item.indirectpaycount == null ? 0 : Convert.ToInt32(item.indirectpaycount);
                        rpt.favitemcount = item.favitemcount == null ? 0 : Convert.ToInt32(item.favitemcount);
                        rpt.favshopcount = item.favshopcount == null ? 0 : Convert.ToInt32(item.favshopcount);
                        rpt.roi = rpt.cost == 0M ? 0M : Math.Round((rpt.directpay + rpt.indirectpay) / rpt.cost, 2);
                        returnPageSize++;
                    }
                }

                // 返回的行数小于500的时候，说明到了最后一页，返回
                if (returnPageSize < pageSize)
                {
                    break;
                }
            }

            return lstAll;
        }

        /// <summary>
        /// 下载推广组的创意基础报表
        /// </summary>
        private string DownLoadCreativeBaseReport(TopSession session, long campaignId, long adgroupId, string strStartDay, string strEndDay)
        {
            var response = CommonHandler.DoTaoBaoApi<SimbaRptAdgroupcreativebaseGetResponse>(taobaoApiHandler.TaobaoSimbaRptAdgroupcreativebaseGet, session, campaignId, adgroupId, strStartDay, strEndDay);

            return response == null ? string.Empty : response.RptAdgroupcreativeBaseList ?? string.Empty;
        }

        /// <summary>
        /// 下载推广组的创意效果报表
        /// </summary>
        private string DownLoadCreativeEffectReport(TopSession session, long campaignId, long adgroupId, string strStartDay, string strEndDay)
        {
            var response = CommonHandler.DoTaoBaoApi<SimbaRptAdgroupcreativeeffectGetResponse>(taobaoApiHandler.TaobaoSimbaRptAdgroupcreativeeffectGet, session, campaignId, adgroupId, strStartDay, strEndDay);
            if (response == null) return string.Empty;

            if (!string.IsNullOrEmpty(response.RptAdgroupcreativeEffectList))
            {
                return response.RptAdgroupcreativeEffectList;
            }
            else
            {//RptAdgroupcreativeEffectList属性为空，但是body可能有值，原因是Response的注释有误，不应该注释为rpt_adgroupcreative_list
                if (!string.IsNullOrEmpty(response.Body))
                {
                    string json = response.Body;
                    try
                    {
                        var r = new DynamicJsonParser().FromJson(json);
                        return DynamicJsonParser.FromObject(r.simba_rpt_adgroupcreativeeffect_get_response.rpt_adgroupcreative_list);
                    }
                    catch (Exception se)
                    {
                        logger.Error(string.Format("用户：{0}，推广组：{1}，创意效果报表数据解析出错", session.UserName, adgroupId), se);
                    }
                    return json;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}