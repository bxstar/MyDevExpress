using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaoBaoDataServer.WinClientData.Model;
using log4net;
using NetServ.Net.Json;
using iclickpro.AccessCommon;
using System.IO;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class BusinessReportHandler
    {
        private static log4net.ILog logger = LogManager.GetLogger("loggerAX");
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();

        /// <summary>
        /// 所有推广计划报表
        /// </summary>
        public List<EntityCampaignReport> GetAllCampaignRpt(TopSession session, int day, ref string errorMsg)
        {
            List<EntityCampaignReport> lstRpt = new List<EntityCampaignReport>();
            DateTime dtStart = DateTime.Now;
            // 下载推广计划的基本报表
            var reportBase = TaobaoApiHandler.TaobaoSimbaRptCustbaseGet(session, DateTime.Now.AddDays(0 - day), DateTime.Now.AddDays(-1), 500, 1);
            if (reportBase.IsError)
            {
                if (CommonHandler.IsBanMsg(reportBase))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        System.Threading.Thread.Sleep(2000);
                        reportBase = TaobaoApiHandler.TaobaoSimbaRptCustbaseGet(session, DateTime.Now.AddDays(0 - day), DateTime.Now.AddDays(-1), 500, 1);
                        if (reportBase.IsError && CommonHandler.IsBanMsg(reportBase) && dtStart.AddMinutes(5) > DateTime.Now)
                        {//超过5分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddMinutes(5) <= DateTime.Now)
                            {
                                errorMsg = "线上下载推广计划的基本报表出错，已重试5分钟";
                                logger.Error(errorMsg + reportBase.Body);
                                return null;
                            }
                            isBanError = false;
                        }
                    }
                }
                else
                {
                    logger.Error(reportBase.Body);
                    errorMsg = reportBase.SubErrMsg;
                    return null;
                }

            }
            if (reportBase == null || String.IsNullOrEmpty(reportBase.RptCustBaseList) || reportBase.RptCustBaseList == "[]" || reportBase.RptCustBaseList == "{}")
            {
                return null;
            }
            // 解析推广组json数据
            NetServ.Net.Json.JsonArray dataBaseRpt;
            // 解析推广组
            using (JsonParser parser = new JsonParser(new StringReader(reportBase.RptCustBaseList), true))
            {
                dataBaseRpt = parser.ParseArray();
            }
            foreach (JsonObject service in dataBaseRpt)
            {
                string date = CommonFunction.JsonObjectToString(service["date"]);
                EntityCampaignReport date_rpt = lstRpt.Find(o => o.date == date);
                if (date_rpt == null)
                {
                    date_rpt = new EntityCampaignReport();
                    date_rpt.date = date;
                    lstRpt.Add(date_rpt);
                }

                date_rpt.impressions = date_rpt.impressions + CommonFunction.JsonObjectToInt(service["impressions"]);
                date_rpt.click = date_rpt.click + CommonFunction.JsonObjectToInt(service["click"]);
                date_rpt.cost = date_rpt.cost + CommonFunction.JsonObjectToInt(service["cost"]) / 100.0M;
            }

            dtStart = DateTime.Now;
            var reportEffect = TaobaoApiHandler.TaobaoSimbaRptCusteffectGet(session, DateTime.Now.AddDays(0 - day), DateTime.Now.AddDays(-1), 500, 1);

            if (reportEffect.IsError)
            {
                if (CommonHandler.IsBanMsg(reportEffect))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        System.Threading.Thread.Sleep(2000);
                        reportEffect = TaobaoApiHandler.TaobaoSimbaRptCusteffectGet(session, DateTime.Now.AddDays(0 - day), DateTime.Now.AddDays(-1), 500, 1);
                        if (reportEffect.IsError && CommonHandler.IsBanMsg(reportEffect) && dtStart.AddMinutes(5) > DateTime.Now)
                        {//超过5分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddMinutes(5) <= DateTime.Now)
                            {
                                errorMsg = "线上下载推广计划的基本报表出错，已重试5分钟";
                                logger.Error(errorMsg + reportEffect.Body);
                                return null;
                            }
                            isBanError = false;
                        }
                    }
                }
                else
                {
                    logger.Error(reportEffect.Body);
                    errorMsg = reportEffect.SubErrMsg;
                    return null;
                }
            }
            if (reportEffect != null && !String.IsNullOrEmpty(reportEffect.RptCustEffectList) && reportEffect.RptCustEffectList != "[]" && reportEffect.RptCustEffectList != "{}")
            {
                // 解析推广组json数据
                NetServ.Net.Json.JsonArray dataEffectRpt;
                // 解析推广组
                using (JsonParser parser = new JsonParser(new StringReader(reportEffect.RptCustEffectList), true))
                {
                    dataEffectRpt = parser.ParseArray();
                }
                foreach (JsonObject service in dataEffectRpt)
                {
                    string date = CommonFunction.JsonObjectToString(service["date"]);
                    EntityCampaignReport date_rpt = lstRpt.Find(o => o.date == date);
                    if (date_rpt == null)
                    {
                        date_rpt = new EntityCampaignReport();
                        date_rpt.date = date;
                        lstRpt.Add(date_rpt);
                    }

                    date_rpt.totalpay = date_rpt.totalpay + CommonFunction.JsonObjectToInt(service["directpay"]) / 100.0M + CommonFunction.JsonObjectToInt(service["indirectpay"]) / 100.0M;
                }
            }

            foreach (var item in lstRpt)
            {
                if (item.impressions != 0)
                    item.ctr = Math.Round(item.click * 100.0M / item.impressions, 2);

                if (item.click != 0)
                    item.cpc = Math.Round(item.cost / item.click, 2);

                if (item.cost != 0)
                    item.roi = Math.Round(item.totalpay / item.cost, 2);
            }

            return lstRpt;
        }

    }
}