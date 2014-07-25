using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaoBaoDataServer.WinClientData.Model;
using log4net;
using NetServ.Net.Json;
using System.IO;
using System.Data.SqlClient;
using iclickpro.AccessCommon;
using Top.Api.Response;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class BusinessBatchHandler
    {
        private static log4net.ILog logger = LogManager.GetLogger("loggerAX");
        BusinessTaobaoApiHandler taobaoApiHandler = new BusinessTaobaoApiHandler();

        /// <summary>
        /// 下载关键词的基本报表数据，并保存到数据库
        /// </summary>
        public Boolean DownLoadKeywordBaseReport(TopSession session, long campaignId, long adgroupId, int days, Boolean isNeedUpdate, ref Boolean isDifferent)
        {
            //每页返回
            long pageSize = 500;
            try
            {
                long i = 0;
                while (true)
                {
                    i = i + 1;
                    DateTime dtStart = DateTime.Now;
                    // 下载推广组基础报表
                    var response = CommonHandler.DoTaoBaoApi<SimbaRptAdgroupkeywordbaseGetResponse>(taobaoApiHandler.TaobaoSimbaRptAdgroupkeywordbaseGet,
                        session, campaignId, adgroupId, DateTime.Now.AddDays(0 - days).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "SUMMARY", pageSize, i, "SUMMARY");

                    if (response == null || String.IsNullOrEmpty(response.RptAdgroupkeywordBaseList) || response.RptAdgroupkeywordBaseList == "[]" || response.RptAdgroupkeywordBaseList == "{}")
                    {
                        break;
                    }
                    // 解析推广组json数据
                    JsonArray data;
                    // 解析推广组
                    using (JsonParser parser = new JsonParser(new StringReader(response.RptAdgroupkeywordBaseList), true))
                    {
                        data = parser.ParseArray();
                    }

                    foreach (JsonObject service in data)
                    {
                        EntityKeywordRpt rpt = GetKeywordBaseReport(service, Convert.ToInt32(session.UserID));
                        if (rpt.local_id == 0)
                        {
                            // 推广组基础报表数据入库
                            AddKeywordBaseReport(service, Convert.ToInt32(session.UserID));
                        }
                        else
                        {
                            if (rpt.impressions != CommonFunction.JsonObjectToInt(service["impressions"])
                                || rpt.click != CommonFunction.JsonObjectToInt(service["click"])
                                )
                            {//基础报表数据不一致
                                isDifferent = true;
                                if (isNeedUpdate)
                                {
                                    UpdateKeywordBaseReport(service, rpt.local_id);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    // 返回的行数小于500的时候，则返回
                    if (data.Count < pageSize)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/DownLoadKeywordBaseReport,淘宝API下载关键词基础报表数据 失败：", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 开始下载关键词的效果报表数据
        /// </summary>
        public Boolean DownLoadKeywordEffectReport(TopSession session, long campaignId, long adgroupId, int days, Boolean isNeedUpdate, ref Boolean isDifferent)
        {
            //每页返回
            long pageSize = 500;
            try
            {
                long i = 0;
                while (true)
                {
                    i = i + 1;
                    DateTime dtStart = DateTime.Now;
                    // 下载推广组效果报表
                    var response = CommonHandler.DoTaoBaoApi<SimbaRptAdgroupkeywordeffectGetResponse>(taobaoApiHandler.TaobaoSimbaRptAdgroupkeywordeffectGet,
                        session, campaignId, adgroupId, DateTime.Now.AddDays(0 - days).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "SUMMARY", pageSize, i, "SUMMARY");

                    if (response == null || String.IsNullOrEmpty(response.RptAdgroupkeywordEffectList) || response.RptAdgroupkeywordEffectList == "[]" || response.RptAdgroupkeywordEffectList == "{}")
                    {
                        break;
                    }
                    // 解析推广组json数据
                    JsonArray data;
                    // 解析推广组
                    using (JsonParser parser = new JsonParser(new StringReader(response.RptAdgroupkeywordEffectList), true))
                    {
                        data = parser.ParseArray();
                    }

                    foreach (JsonObject service in data)
                    {
                        EntityKeywordRpt rpt = GetKeywordEffectReport(service, Convert.ToInt32(session.UserID));
                        if (rpt.local_id == 0)
                        {
                            // 关键词报表数据入库
                            AddKeywordEffectReport(service, Convert.ToInt32(session.UserID));
                        }
                        else
                        {
                            if (rpt.directpay != CommonFunction.JsonObjectToDecimal(service["directpay"]) / 100
                                || rpt.indirectpay != CommonFunction.JsonObjectToDecimal(service["indirectpay"]) / 100
                                )
                            {//效果报表数据不一致
                                isDifferent = true;
                                if (isNeedUpdate)
                                {
                                    UpdateKeywordEffectReport(service, rpt.local_id);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    // 返回的行数小于500的时候，说明到了最后一页，返回
                    if (data.Count < pageSize)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/DownLoadKeywordEffectReport,淘宝API下载关键词的效果报表 失败：", ex);
                return false;
            }
            return true;

        }

        #region 关键词的基础数据操作

        /// <summary>
        /// 数据库，获取关键词的本地ID
        /// </summary>
        private EntityKeywordRpt GetKeywordBaseReport(JsonObject service, int userId)
        {
            EntityKeywordRpt rpt = new EntityKeywordRpt();
            try
            {
                var lstParameter = new List<SqlParameter>
                                       {
                                           new SqlParameter("@date", service["date"].ToString()),
                                           new SqlParameter("@campaign_id",Convert.ToInt64(service["campaignid"].ToString())),
                                           new SqlParameter("@adgroup_id",Convert.ToInt64(service["adgroupid"].ToString())),
                                           new SqlParameter("@keyword_id",Convert.ToInt64(service["keywordid"].ToString())),
                                           new SqlParameter("@user_id",userId),
                                           new SqlParameter("@source", CommonFunction.JsonObjectToString(service["source"]))
                                       };
                const string sqlText = @"SELECT * FROM ad_rpt_keyword_base where user_id =@user_id and [date] = @date  and campaign_id = @campaign_id  and adgroup_id = @adgroup_id and keyword_id = @keyword_id and source = @source";

                var dsLoaclKeyword = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), sqlText, lstParameter.ToArray());
                if (dsLoaclKeyword != null && dsLoaclKeyword.Tables.Count > 0 && dsLoaclKeyword.Tables[0].Rows.Count > 0)
                {
                    rpt.local_id = Convert.ToInt32(dsLoaclKeyword.Tables[0].Rows[0]["local_keyword_id"]);
                    if (dsLoaclKeyword.Tables[0].Rows[0]["impressions"] != null && dsLoaclKeyword.Tables[0].Rows[0]["impressions"] != DBNull.Value)
                        rpt.impressions = Convert.ToInt32(dsLoaclKeyword.Tables[0].Rows[0]["impressions"]);
                    if (dsLoaclKeyword.Tables[0].Rows[0]["click"] != null && dsLoaclKeyword.Tables[0].Rows[0]["click"] != DBNull.Value)
                        rpt.click = Convert.ToInt32(dsLoaclKeyword.Tables[0].Rows[0]["click"]);
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetKeywordBaseReportId,获取关键词基础报告失败：", ex);
            }
            return rpt;
        }

        /// <summary>
        /// 数据库，关键词基础报表数据入库
        /// </summary>
        private void AddKeywordBaseReport(JsonObject service, int userId)
        {
            try
            {
                const string sqlText = @"INSERT INTO ad_rpt_keyword_base
                                                   (date
                                                  ,campaign_id
                                                  ,adgroup_id
                                                  ,keyword_id
                                                  ,nick
                                                  ,impressions
                                                  ,click
                                                  ,cost
                                                  ,ctr
                                                  ,cpc
                                                  ,cpm
                                                  ,keywordstr
                                                  ,searchtype
                                                  ,source
                                                  ,avgpos
                                                  ,user_id
                                                  ,create_date)
                                            VALUES
                                                  (@date
                                                  ,@campaign_id
                                                  ,@adgroup_id
                                                  ,@keyword_id
                                                  ,@nick
                                                  ,@impressions
                                                  ,@click
                                                  ,@cost
                                                  ,@ctr
                                                  ,@cpc
                                                  ,@cpm
                                                  ,@keywordstr
                                                  ,@searchtype
                                                  ,@source
                                                  ,@avgpos
                                                  ,@user_id
                                                  ,getdate())";
                Decimal cpc = 0;
                if (service.ContainsKey("cpc") && service["cpc"] != JsonNull.Null)
                {
                    cpc = Convert.ToDecimal(service["cpc"].ToString());
                }
                var lstParameter = new List<SqlParameter>
                                       {
                                           new SqlParameter("@date", service["date"].ToString()),
                                           new SqlParameter("@campaign_id",Convert.ToInt64(service["campaignid"].ToString())),
                                           new SqlParameter("@adgroup_id",Convert.ToInt64(service["adgroupid"].ToString())),
                                           new SqlParameter("@keyword_id",Convert.ToInt64(service["keywordid"].ToString())),
                                           new SqlParameter("@nick", service["nick"].ToString()),
                                           new SqlParameter("@impressions",CommonFunction.JsonObjectToInt(service["impressions"])),
                                           new SqlParameter("@click", CommonFunction.JsonObjectToInt(service["click"])),
                                           new SqlParameter("@cost",CommonFunction.JsonObjectToDecimal(service["cost"])/100),
                                           new SqlParameter("@ctr", CommonFunction.JsonObjectToDecimal(service["ctr"])),
                                           new SqlParameter("@cpc", cpc/100),
                                           new SqlParameter("@cpm", CommonFunction.JsonObjectToDecimal(service["cpm"])/100),
                                           new SqlParameter("@keywordstr", CommonFunction.JsonObjectToString(service["keywordstr"])),
                                           new SqlParameter("@searchtype", CommonFunction.JsonObjectToString(service["searchtype"])),
                                           new SqlParameter("@source", CommonFunction.JsonObjectToString(service["source"])),
                                           new SqlParameter("@user_id", userId),
                                           new SqlParameter("@avgpos", CommonFunction.JsonObjectToInt(service["avgpos"]))
                                       };
                var nResult = SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), sqlText, lstParameter.ToArray());
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/AddKeywordBaseReport,关键词的基础报表数据入库 失败：", ex);
            }
        }


        /// <summary>
        /// 数据库，更新关键词基础报表数据
        /// </summary>
        /// <param name="service"></param>
        /// <param name="userId"></param>
        private void UpdateKeywordBaseReport(JsonObject service, int localKeywordId)
        {
            try
            {
                const string sqlText = @"UPDATE ad_rpt_keyword_base
                                            SET impressions = @impressions
                                               ,click = @click
                                               ,cost = @cost
                                               ,ctr = @ctr
                                               ,cpc = @cpc
                                               ,cpm = @cpm
                                               ,searchtype = @searchtype
                                               ,avgpos = @avgpos
                                        WHERE local_keyword_id = @local_keyword_id";
                Decimal cpc = 0;
                if (service.ContainsKey("cpc") && service["cpc"] != JsonNull.Null)
                {
                    cpc = Convert.ToDecimal(service["cpc"].ToString());
                }
                var lstParameter = new List<SqlParameter>
                                       {
                                           new SqlParameter("@impressions",CommonFunction.JsonObjectToInt(service["impressions"])),
                                           new SqlParameter("@click", CommonFunction.JsonObjectToInt(service["click"])),
                                           new SqlParameter("@cost",CommonFunction.JsonObjectToDecimal(service["cost"])/100),
                                           new SqlParameter("@ctr", CommonFunction.JsonObjectToDecimal(service["ctr"])),
                                           new SqlParameter("@cpc", cpc/100),
                                           new SqlParameter("@cpm", CommonFunction.JsonObjectToDecimal(service["cpm"])/100),
                                           new SqlParameter("@searchtype", CommonFunction.JsonObjectToString(service["searchtype"])),
                                           new SqlParameter("@local_keyword_id", localKeywordId),
                                           new SqlParameter("@avgpos", CommonFunction.JsonObjectToInt(service["avgpos"]))
                                       };
                var nResult = SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), sqlText, lstParameter.ToArray());
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/UpdateKeywordBaseReport,关键词的基础报表数据入库 失败：", ex);
            }
        }

        #endregion

        #region 关键词的效果数据操作
        /// <summary>
        /// 数据库，获取关键词的本地ID
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private EntityKeywordRpt GetKeywordEffectReport(JsonObject service, int userId)
        {
            EntityKeywordRpt rpt = new EntityKeywordRpt();
            try
            {
                var lstParameter = new List<SqlParameter>
                                       {
                                           new SqlParameter("@date", service["date"].ToString()),
                                           new SqlParameter("@campaign_id",Convert.ToInt64(service["campaignid"].ToString())),
                                           new SqlParameter("@adgroup_id",Convert.ToInt64(service["adgroupid"].ToString())),
                                           new SqlParameter("@keyword_id",Convert.ToInt64(service["keywordid"].ToString())),
                                           new SqlParameter("@user_id",userId),
                                           new SqlParameter("@source", CommonFunction.JsonObjectToString(service["source"]))
                                       };
                const string sqlText = @"SELECT * FROM ad_rpt_keyword_effect  where user_id =@user_id and [date] = @date  and campaign_id = @campaign_id  and adgroup_id = @adgroup_id and keyword_id = @keyword_id and source = @source";

                var dsLoaclKeyword = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), sqlText, lstParameter.ToArray());
                if (dsLoaclKeyword != null && dsLoaclKeyword.Tables.Count > 0 && dsLoaclKeyword.Tables[0].Rows.Count > 0)
                {
                    rpt.local_id = Convert.ToInt32(dsLoaclKeyword.Tables[0].Rows[0]["local_keyword_id"]);
                    if (dsLoaclKeyword.Tables[0].Rows[0]["directpay"] != null && dsLoaclKeyword.Tables[0].Rows[0]["directpay"] != DBNull.Value)
                    {
                        rpt.directpay = Convert.ToDecimal(dsLoaclKeyword.Tables[0].Rows[0]["directpay"]);
                    }
                    if (dsLoaclKeyword.Tables[0].Rows[0]["indirectpay"] != null && dsLoaclKeyword.Tables[0].Rows[0]["indirectpay"] != DBNull.Value)
                    {
                        rpt.indirectpay = Convert.ToDecimal(dsLoaclKeyword.Tables[0].Rows[0]["indirectpay"]);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetKeywordEffectReportId,获取关键词效果报告失败：", ex);
            }
            return rpt;
        }

        /// <summary>
        /// 数据库，关键词效果数据入库
        /// </summary>
        /// <param name="service"></param>
        /// <param name="userId"></param>
        private void AddKeywordEffectReport(JsonObject service, int userId)
        {
            const string sqlText = @"INSERT INTO ad_rpt_keyword_effect
                                                (date
                                                ,campaign_id
                                                ,adgroup_id
                                                ,keyword_id
                                                ,nick
                                                ,directpay
                                                ,indirectpay
                                                ,directpaycount
                                                ,indirectpaycount
                                                ,favitemcount
                                                ,favshopcount
                                                ,keywordstr
                                                ,searchtype
                                                ,source
                                                ,user_id
                                                ,create_date)
                                            VALUES
                                                (@date
                                                ,@campaign_id
                                                ,@adgroup_id
                                                ,@keyword_id
                                                ,@nick
                                                ,@directpay
                                                ,@indirectpay
                                                ,@directpaycount
                                                ,@indirectpaycount
                                                ,@favitemcount
                                                ,@favshopcount
                                                ,@keywordstr
                                                ,@searchtype
                                                ,@source
                                                ,@user_id
                                                ,getdate())";
            Decimal directpay = 0;
            if (service.ContainsKey("directpay") && service["directpay"] != JsonNull.Null)
            {
                directpay = Convert.ToDecimal(service["directpay"].ToString());
            }

            Decimal indirectpay = 0;
            if (service.ContainsKey("indirectpay") && service["indirectpay"] != JsonNull.Null)
            {
                indirectpay = Convert.ToDecimal(service["indirectpay"].ToString());
            }

            int directpaycount = 0;
            if (service.ContainsKey("directpaycount") && service["directpaycount"] != JsonNull.Null)
            {
                directpaycount = Convert.ToInt32(service["directpaycount"].ToString());
            }

            int indirectpaycount = 0;
            if (service.ContainsKey("indirectpaycount") && service["indirectpaycount"] != JsonNull.Null)
            {
                indirectpaycount = Convert.ToInt32(service["indirectpaycount"].ToString());
            }

            int favItemCount = 0;
            if (service.ContainsKey("favItemCount") && service["favItemCount"] != JsonNull.Null)
            {
                favItemCount = Convert.ToInt32(service["favItemCount"].ToString());
            }

            int favShopCount = 0;
            if (service.ContainsKey("favShopCount") && service["favShopCount"] != JsonNull.Null)
            {
                favShopCount = Convert.ToInt32(service["favShopCount"].ToString());
            }

            var lstParameter = new List<SqlParameter>
                                    {
                                        new SqlParameter("@date", service["date"].ToString()),
                                        new SqlParameter("@campaign_id",Convert.ToInt64(service["campaignid"].ToString())),
                                        new SqlParameter("@adgroup_id",Convert.ToInt64(service["adgroupid"].ToString())),
                                        new SqlParameter("@keyword_id",Convert.ToInt64(service["keywordid"].ToString())),
                                        new SqlParameter("@nick", service["nick"].ToString()),
                                        new SqlParameter("@directpay", directpay/100),
                                        new SqlParameter("@indirectpay", indirectpay/100),
                                        new SqlParameter("@directpaycount", directpaycount),
                                        new SqlParameter("@indirectpaycount", indirectpaycount),
                                        new SqlParameter("@favitemcount", favItemCount),
                                        new SqlParameter("@favshopcount", favShopCount),
                                        new SqlParameter("@user_id", userId),
                                        new SqlParameter("@keywordstr", CommonFunction.JsonObjectToString(service["keywordstr"])),
                                        new SqlParameter("@searchtype",service["searchtype"].ToString()),
                                        new SqlParameter("@source",service["source"].ToString())
                                    };
            var nResult = SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), sqlText, lstParameter.ToArray());
        }

        /// <summary>
        /// 数据库，更新关键词效果数据
        /// </summary>
        /// <param name="service"></param>
        /// <param name="userId"></param>
        private void UpdateKeywordEffectReport(JsonObject service, int localKeywordId)
        {
            try
            {
                const string sqlText = @"UPDATE ad_rpt_keyword_effect
                                            SET directpay = @directpay
                                               ,indirectpay = @indirectpay
                                               ,directpaycount = @directpaycount
                                               ,indirectpaycount = @indirectpaycount
                                               ,favitemcount = @favitemcount
                                               ,favshopcount = @favshopcount
                                          WHERE local_keyword_id = @local_keyword_id";
                Decimal directpay = 0;
                if (service.ContainsKey("directpay") && service["directpay"] != JsonNull.Null)
                {
                    directpay = Convert.ToDecimal(service["directpay"].ToString());
                }

                Decimal indirectpay = 0;
                if (service.ContainsKey("indirectpay") && service["indirectpay"] != JsonNull.Null)
                {
                    indirectpay = Convert.ToDecimal(service["indirectpay"].ToString());
                }

                int directpaycount = 0;
                if (service.ContainsKey("directpaycount") && service["directpaycount"] != JsonNull.Null)
                {
                    directpaycount = Convert.ToInt32(service["directpaycount"].ToString());
                }

                int indirectpaycount = 0;
                if (service.ContainsKey("indirectpaycount") && service["indirectpaycount"] != JsonNull.Null)
                {
                    indirectpaycount = Convert.ToInt32(service["indirectpaycount"].ToString());
                }

                int favItemCount = 0;
                if (service.ContainsKey("favItemCount") && service["favItemCount"] != JsonNull.Null)
                {
                    favItemCount = Convert.ToInt32(service["favItemCount"].ToString());
                }

                int favShopCount = 0;
                if (service.ContainsKey("favShopCount") && service["favShopCount"] != JsonNull.Null)
                {
                    favShopCount = Convert.ToInt32(service["favShopCount"].ToString());
                }

                var lstParameter = new List<SqlParameter>
                                       {
                                           new SqlParameter("@directpay", directpay/100),
                                           new SqlParameter("@indirectpay", indirectpay/100),
                                           new SqlParameter("@directpaycount", directpaycount),
                                           new SqlParameter("@indirectpaycount", indirectpaycount),
                                           new SqlParameter("@favitemcount", favItemCount),
                                           new SqlParameter("@favshopcount", favShopCount),
                                           new SqlParameter("@local_keyword_id", localKeywordId)
                                       };
                var nResult = SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), sqlText, lstParameter.ToArray());
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/UpdateKeywordEffectReport,更新关键词的效果报表数据 失败：", ex);
            }
        }


        #endregion


    }

}
