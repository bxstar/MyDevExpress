﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TaoBaoDataServer.WinClientData.Model;
using Top.Api.Domain;
using System.Data;
using iclickpro.AccessCommon;
using Top.Api.Response;
using NetServ.Net.Json;
using System.IO;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class BusinessCampaignHandler
    {
        private static log4net.ILog logger = LogManager.GetLogger("loggerAX");
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();

        /// <summary>
        /// 数据库，获取用户绑定的推广计划
        /// </summary>
        public EntityCampaign GetCampaign(int userId)
        {
            EntityCampaign e = new EntityCampaign();

            var param = new Dictionary<string, object>();
            param.Add("user_id", userId);
            DataSet ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), "SELECT * FROM ad_campaign WHERE user_id = @user_id", SqlNameAndParamer.ConvertSqlParameter(param));
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                e.localcampaignid = Convert.ToInt32(dr["local_campaign_id"]);
                if (dr["campaign_id"] != null && dr["campaign_id"] != DBNull.Value)
                    e.campaignid = Convert.ToInt64(dr["campaign_id"]);
                if (dr["title"] != null && dr["title"] != DBNull.Value)
                    e.title = dr["title"].ToString();
                if (dr["online_status"] != null && dr["online_status"] != DBNull.Value)
                    e.onlinestatus = dr["online_status"].ToString();
                if (dr["settle_reason"] != null && dr["settle_reason"] != DBNull.Value)
                    e.onlinestatus = dr["settle_reason"].ToString();
                if (dr["settle_status"] != null && dr["settle_status"] != DBNull.Value)
                    e.onlinestatus = dr["settle_status"].ToString();
                if (dr["daily_limit"] != null && dr["daily_limit"] != DBNull.Value)
                    e.dailylimit = Convert.ToDecimal(dr["daily_limit"]);
                if (dr["click_cost"] != null && dr["click_cost"] != DBNull.Value)
                    e.dailylimit = Convert.ToDecimal(dr["click_cost"]);
                e.userid = Convert.ToInt32(dr["user_id"]);
                e.createdate = Convert.ToDateTime(dr["create_date"]);
                e.updatedate = Convert.ToDateTime(dr["update_date"]);
            }

            return e;
        }

        /// <summary>
        /// 线上，获取推广计划
        /// </summary>
        public List<Campaign> GetCampaignOnline(TopSession session)
        {
            // 定义返回值
            List<Campaign> listCampaign = new List<Campaign>();
            try
            {
                var response = TaobaoApiHandler.TaobaoSimbaCampaignsGet(session);
                if (response.IsError)
                {
                    logger.Error(string.Format("用户{0}，ID={1} 线上，淘宝API下载推广计划数据 失败：{2}", session.ProxyUserName, session.UserID, response.Body));
                }
                else
                {
                    listCampaign = response.Campaigns;
                }
            }
            catch (Exception ex)
            {
                logger.Error("线上，淘宝API下载推广计划数据 失败：", ex);
            }
            return listCampaign;
        }

        /// <summary>
        /// 推广计划报表
        /// </summary>
        public List<EntityCampaignReport> GetCampaignRpt(TopSession session, List<long> lstCampaignId, int day, ref string errorMsg)
        {

            List<EntityCampaignReport> lstRpt = new List<EntityCampaignReport>();

            if (lstCampaignId == null)
            {//取用户所有推广中的计划
                lstCampaignId = new List<long>();
                SimbaCampaignsGetResponse campaignsRes = TaobaoApiHandler.TaobaoSimbaCampaignsGet(session);
                if (campaignsRes.IsError)
                {
                    logger.Error(campaignsRes.Body);
                    errorMsg = campaignsRes.SubErrMsg;
                    return null;
                }
                lstCampaignId.AddRange(campaignsRes.Campaigns.Where(o => o.SettleStatus == "online").Select(o => o.CampaignId));
            }

            foreach (var itemCampaignId in lstCampaignId)
            {

                // 下载推广计划的基本报表
                var reportBase = TaobaoApiHandler.TaobaoSimbaRptCampaignbaseGet(session, itemCampaignId, DateTime.Now.AddDays(0 - day).Date.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd"));
                if (reportBase.IsError)
                {
                    logger.Error(reportBase.Body);
                    errorMsg = reportBase.SubErrMsg;
                    return null;
                }
                if (reportBase == null || String.IsNullOrEmpty(reportBase.RptCampaignBaseList) || reportBase.RptCampaignBaseList == "[]" || reportBase.RptCampaignBaseList == "{}")
                {
                    continue;
                }
                // 解析推广组json数据
                NetServ.Net.Json.JsonArray dataBaseRpt;
                // 解析推广组
                using (JsonParser parser = new JsonParser(new StringReader(reportBase.RptCampaignBaseList), true))
                {
                    dataBaseRpt = parser.ParseArray();
                }
                foreach (JsonObject service in dataBaseRpt)
                {
                    string date = TechNet.JsonObjectToString(service["date"]);
                    EntityCampaignReport date_rpt = lstRpt.Find(o => o.date == date);
                    if (date_rpt == null)
                    {
                        date_rpt = new EntityCampaignReport();
                        date_rpt.date = date;
                        lstRpt.Add(date_rpt);
                    }

                    date_rpt.impressions = date_rpt.impressions + TechNet.JsonObjectToInt(service["impressions"]);
                    date_rpt.click = date_rpt.click + TechNet.JsonObjectToInt(service["click"]);
                    date_rpt.cost = date_rpt.cost + TechNet.JsonObjectToInt(service["cost"]) / 100.0M;
                }

                var reportEffect = TaobaoApiHandler.TaobaoSimbaRptCampaigneffectGet(session, itemCampaignId, DateTime.Now.AddDays(0 - day).Date.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd"));

                if (reportEffect.IsError)
                {
                    logger.Error(reportEffect.Body);
                    errorMsg = reportEffect.SubErrMsg;
                    return null;
                }
                if (reportEffect == null || String.IsNullOrEmpty(reportEffect.RptCampaignEffectList) || reportEffect.RptCampaignEffectList == "[]" || reportEffect.RptCampaignEffectList == "{}")
                {
                    continue;
                }
                // 解析推广组json数据
                NetServ.Net.Json.JsonArray dataEffectRpt;
                // 解析推广组
                using (JsonParser parser = new JsonParser(new StringReader(reportEffect.RptCampaignEffectList), true))
                {
                    dataEffectRpt = parser.ParseArray();
                }
                foreach (JsonObject service in dataEffectRpt)
                {
                    string date = TechNet.JsonObjectToString(service["date"]);
                    EntityCampaignReport date_rpt = lstRpt.Find(o => o.date == date);
                    if (date_rpt == null)
                    {
                        date_rpt = new EntityCampaignReport();
                        date_rpt.date = date;
                        lstRpt.Add(date_rpt);
                    }

                    date_rpt.totalpay = date_rpt.totalpay + TechNet.JsonObjectToInt(service["directpay"]) / 100.0M + TechNet.JsonObjectToInt(service["indirectpay"]) / 100.0M;
                }
            }
            foreach (var item in lstRpt)
            {
                if (item.click != 0)
                    item.cpc = Math.Round(item.cost / item.click, 2);

                if (item.cost != 0)
                    item.roi = Math.Round(item.totalpay / item.cost, 2);
            }

            return lstRpt;
        }

        /// <summary>
        /// 数据库，获取所有推广计划
        /// </summary>
        public List<EntityCampaign> GetAllCampaign()
        {
            List<EntityCampaign> lstCampaign = new List<EntityCampaign>();
            DataSet ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), "SELECT * FROM ad_user au LEFT JOIN ad_campaign ac ON au.local_user_id=ac.[user_id] ");
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EntityCampaign e = new EntityCampaign();
                    System.Data.DataRow dr = ds.Tables[0].Rows[i];
                    if (dr["local_campaign_id"] != null && dr["local_campaign_id"] != DBNull.Value)
                        e.localcampaignid = Convert.ToInt32(dr["local_campaign_id"]);
                    if (dr["campaign_id"] != null && dr["campaign_id"] != DBNull.Value)
                        e.campaignid = Convert.ToInt64(dr["campaign_id"]);
                    if (dr["title"] != null && dr["title"] != DBNull.Value)
                        e.title = dr["title"].ToString();
                    if (dr["online_status"] != null && dr["online_status"] != DBNull.Value)
                        e.onlinestatus = dr["online_status"].ToString();
                    if (dr["settle_reason"] != null && dr["settle_reason"] != DBNull.Value)
                        e.onlinestatus = dr["settle_reason"].ToString();
                    if (dr["settle_status"] != null && dr["settle_status"] != DBNull.Value)
                        e.onlinestatus = dr["settle_status"].ToString();
                    if (dr["daily_limit"] != null && dr["daily_limit"] != DBNull.Value)
                        e.dailylimit = Convert.ToDecimal(dr["daily_limit"]);
                    if (dr["click_cost"] != null && dr["click_cost"] != DBNull.Value)
                        e.dailylimit = Convert.ToDecimal(dr["click_cost"]);
                    if (dr["user_id"] != null && dr["user_id"] != DBNull.Value)
                        e.userid = Convert.ToInt32(dr["user_id"]);
                    if (dr["proxy_user_name"] != null && dr["proxy_user_name"] != DBNull.Value)
                        e.nick = dr["proxy_user_name"].ToString();
                    if (dr["create_date"] != null && dr["create_date"] != DBNull.Value)
                        e.createdate = Convert.ToDateTime(dr["create_date"]);
                    if (dr["update_date"] != null && dr["update_date"] != DBNull.Value)
                        e.updatedate = Convert.ToDateTime(dr["update_date"]);

                    lstCampaign.Add(e);
                }

            }

            return lstCampaign;
        }

        /// <summary>
        /// 数据库，获取优化过的推广计划
        /// </summary>
        public List<EntityCampaign> GetMajorizationCampaign()
        {
            List<EntityCampaign> lstCampaign = new List<EntityCampaign>();
            DataSet ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), "SELECT DISTINCT [user_id],au.create_date,proxy_user_name,campaign_id  FROM ad_user au LEFT JOIN ad_majorization_adgroup_record ac ON au.local_user_id=ac.[user_id] WHERE ac.campaign_id IS NOT NULL");
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EntityCampaign e = new EntityCampaign();
                    System.Data.DataRow dr = ds.Tables[0].Rows[i];
                    if (dr["campaign_id"] != null && dr["campaign_id"] != DBNull.Value)
                        e.campaignid = Convert.ToInt64(dr["campaign_id"]);
                    if (dr["user_id"] != null && dr["user_id"] != DBNull.Value)
                        e.userid = Convert.ToInt32(dr["user_id"]);
                    if (dr["proxy_user_name"] != null && dr["proxy_user_name"] != DBNull.Value)
                        e.nick = dr["proxy_user_name"].ToString();
                    if (dr["create_date"] != null && dr["create_date"] != DBNull.Value)
                        e.createdate = Convert.ToDateTime(dr["create_date"]);

                    lstCampaign.Add(e);
                }

            }

            return lstCampaign;
        }

        /// <summary>
        /// 获取用户某个推广计划的推广组优化记录
        /// </summary>
        public List<EntityMajorizationAdgroupRecord> GetMajorizationAdgroup(long campaignId)
        {
            List<EntityMajorizationAdgroupRecord> lstRecord = new List<EntityMajorizationAdgroupRecord>();
            DataSet ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), "SELECT * FROM ad_majorization_adgroup_record WHERE campaign_id=" + campaignId);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EntityMajorizationAdgroupRecord e = new EntityMajorizationAdgroupRecord();
                    System.Data.DataRow dr = ds.Tables[0].Rows[i];
                    if (dr["user_id"] != null && dr["user_id"] != DBNull.Value)
                        e.user_id = Convert.ToInt32(dr["user_id"]);
                    if (dr["campaign_id"] != null && dr["campaign_id"] != DBNull.Value)
                        e.campaign_id = Convert.ToInt64(dr["campaign_id"]);
                    if (dr["adgroup_id"] != null && dr["adgroup_id"] != DBNull.Value)
                        e.adgroup_id = Convert.ToInt64(dr["adgroup_id"]);
                    if (dr["config_id"] != null && dr["config_id"] != DBNull.Value)
                        e.config_id = Convert.ToInt32(dr["config_id"]);
                    if (dr["create_date"] != null && dr["create_date"] != DBNull.Value)
                        e.create_date = Convert.ToDateTime(dr["create_date"]);

                    lstRecord.Add(e);
                }

            }

            return lstRecord;
        }

        /// <summary>
        /// 获取最近多少天的计划基础和效果报告
        /// </summary>
        public List<EntityCampaignReport> GetCampaignReport(long campaignId, int userId, int day)
        {
            List<EntityCampaignReport> lstCampaignReport = new List<EntityCampaignReport>();
            var param = new Dictionary<string, object>();
            param.Add("user_id", userId);
            param.Add("campaign_id", campaignId);
            param.Add("day", day);
            DataSet ds = SqlHelper.ExecuteDataSetStoredProcedure(SqlDataProvider.GetAPSqlConnection(), "pro_get_campaign_report", SqlNameAndParamer.ConvertSqlParameter(param));
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    EntityCampaignReport report = new EntityCampaignReport();
                    report.campaign_id = Convert.ToInt64(ds.Tables[0].Rows[i]["campaign_id"]);
                    report.date = ds.Tables[0].Rows[i]["date"].ToString();
                    report.impressions = Convert.ToInt32(ds.Tables[0].Rows[i]["pv"]);
                    report.click = Convert.ToInt32(ds.Tables[0].Rows[i]["click"]);
                    report.cost = Convert.ToDecimal(ds.Tables[0].Rows[i]["cost"]);
                    report.ctr = Convert.ToDecimal(ds.Tables[0].Rows[i]["ctr"]);
                    report.cpc = Convert.ToDecimal(ds.Tables[0].Rows[i]["cpc"]);
                    report.totalpay = Convert.ToDecimal(ds.Tables[0].Rows[i]["totalpay"]);
                    report.totalpaycount = Convert.ToInt32(ds.Tables[0].Rows[i]["totalpaycount"]);
                    report.favitemcount = Convert.ToInt32(ds.Tables[0].Rows[i]["favitemcount"]);
                    report.favshopcount = Convert.ToInt32(ds.Tables[0].Rows[i]["favshopcount"]);
                    report.rate = Convert.ToDecimal(ds.Tables[0].Rows[i]["rate"]);
                    report.roi = Convert.ToDecimal(ds.Tables[0].Rows[i]["roi"]);
                    lstCampaignReport.Add(report);
                }
            }
            return lstCampaignReport;
        }
    }
}
