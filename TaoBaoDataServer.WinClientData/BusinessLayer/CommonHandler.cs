using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaoBaoDataServer.WinClientData.Model;
using iclickpro.AccessCommon;
using log4net;
using Top.Api.Domain;
using Top.Api.Response;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class CommonHandler
    {
        /// <summary>
        /// 写log信息
        /// </summary>
        private static ILog logger = log4net.LogManager.GetLogger("Logger");

        /// <summary>
        /// 缓存获取淘宝词指数和类目预测代理类
        /// </summary>
        private static WService.WebServiceForKeywordForecast wsKeywordForecastProxy = new WService.WebServiceForKeywordForecast();

        /// <summary>
        /// 分词服务代理类
        /// </summary>
        private static WService.ServiceSplitWord wsSplitWordProxy = new WService.ServiceSplitWord();

        /// <summary>
        /// 优化策略，删除所有的关键词，换一批新词
        /// </summary>
        public const string Const_MajorizationConfig临时删词 = "临时删词规则";

        /// <summary>
        /// 优化策略，花费不足，对关键词加价并找均价更高的词，由系统检测花费不足或用户增加投入所触发
        /// </summary>
        public const string Const_MajorizationConfig浮动加价 = "花费不足浮动加价策略";

        /// <summary>
        /// 优化策略，花费过高，对关键词降价并找均价更低的词，由用户降低投入所触发
        /// </summary>
        public const string Const_MajorizationConfig浮动降价 = "花费过高浮动降价策略";

        /// <summary>
        /// 线上，获取宝贝信息
        /// </summary>
        public static EntityItem GetItemOnline(string itemIdOrUrl)
        {
            EntityItem result = null;
            try
            {
                string strItem = wsKeywordForecastProxy.GetItemInfoCache("test?" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), itemIdOrUrl);
                result = DynamicJsonParser.ToObject<EntityItem>(strItem);
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取宝贝信息错误", se);
                return null;
            }
            return result;
        }

        /// <summary>
        /// 获取宝贝的找词结果
        /// </summary>
        public static string GetItemFindKeyword(long itemId)
        {
            string result = null;
            string cacheKey = string.Format("top_findkeyword_itemid_{0}", itemId);
            try
            {
                result = wsKeywordForecastProxy.GetValue(cacheKey);
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取宝贝的找词结果错误", se);
            }
            return result;
        }

        /// <summary>
        /// 线上，类目TOP100关键词
        /// </summary>
        public static List<string> GetCatTop100Keyword(long catId)
        {
            List<string> result = null;
            try
            {
                string strItem = wsKeywordForecastProxy.GetCatTop100Keyword("test?" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), catId.ToString());
                result = strItem.Split(',').ToList();
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取类目TOP100关键词错误", se);
                return null;
            }
            return result;
        }

        /// <summary>
        /// 线上，获取类目名称
        /// </summary>
        public static string GetItemCatsOnline(string catIds)
        {
            string result = null;
            try
            {
                result = wsKeywordForecastProxy.GetItemCatsOnline("test?" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), catIds);
            }
            catch (Exception se)
            {
                logger.Error("缓存，获取获取类目名称错误", se);
                return null;
            }
            return result;
        }

        /// <summary>
        /// 获取关键词基础数据
        /// </summary>
        public static List<TaoBaoDataServer.WinClientData.BusinessLayer.WService.EntityWordBase> GetKeyWordBaseFromWs(string strKeywords)
        {
            return wsKeywordForecastProxy.KeywordBaseCache(strKeywords).ToList();
        }


        /// <summary>
        /// 获取关键词大盘数据
        /// </summary>
        public static List<InsightWordDataDTO> GetWordsDataFromWs(string keywords, string startDate, string endDate)
        {
            List<InsightWordDataDTO> result = new List<InsightWordDataDTO>();

            List<string> lstKeywords = CommonFunction.SplitterGroupList(keywords.Split(',').ToList(), ',', 10);
            foreach (var itemKeywords in lstKeywords)
            {
                string strResponse = wsKeywordForecastProxy.GetWordsDataCache("test?" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), itemKeywords, startDate, endDate);
                SimbaInsightWordsdataGetResponse response = Top.Api.Util.TopUtils.ParseResponse<SimbaInsightWordsdataGetResponse>(strResponse);
                if (!response.IsError)
                {
                    result.AddRange(response.WordDataList);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取流量细分数据
        /// </summary>
        public static List<InsightWordSubDataDTO> GetWordsSubDataFromWs(string keywords, string startDate, string endDate)
        {
            List<InsightWordSubDataDTO> result = new List<InsightWordSubDataDTO>();

            List<string> lstKeywords = CommonFunction.SplitterGroupList(keywords.Split(',').ToList(), ',', 10);
            foreach (var itemKeywords in lstKeywords)
            {
                string strResponse = wsKeywordForecastProxy.GetWordsSubDataCache("test?" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), itemKeywords, startDate, endDate);
                SimbaInsightWordssubdataGetResponse response = Top.Api.Util.TopUtils.ParseResponse<SimbaInsightWordssubdataGetResponse>(strResponse);
                if (!response.IsError)
                {
                    result.AddRange(response.WordSubdataList);
                }
            }

            return result;
        }

        /// <summary>
        /// 分词
        /// </summary>
        public static string SplitWordFromWs(string keywords)
        {
            string result = null;
            try
            {
                result = wsSplitWordProxy.SplitWordPanGu(keywords);
            }
            catch (Exception se)
            {
                logger.Error("分词调用错误", se);
            }
            return result;
        }

        /// <summary>
        /// 获取策略
        /// </summary>
        public static EntityMajorConfig GetMajorConfig(string configIdOrTitle)
        {
            int configId = 0;
            var param = new Dictionary<string, object>();
            if (Int32.TryParse(configIdOrTitle, out configId))
            {
                param.Add("local_id", configIdOrTitle);
            }
            else
            {
                param.Add("config_title", configIdOrTitle);
            }

            System.Data.DataSet ds = new System.Data.DataSet();
            if (configId != 0)
            {
                ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), "SELECT * FROM ad_majorization_config WHERE local_id=@local_id", SqlNameAndParamer.ConvertSqlParameter(param));
            }
            else
            {
                ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), "SELECT * FROM ad_majorization_config WHERE config_title=@config_title", SqlNameAndParamer.ConvertSqlParameter(param));
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                System.Data.DataRow dr = ds.Tables[0].Rows[0];
                EntityMajorConfig config = new EntityMajorConfig();
                config.LocalID = Convert.ToInt32(dr["local_id"]);
                if (dr["config_title"] != null && dr["config_title"] != DBNull.Value)
                {
                    config.ConfigTitle = dr["config_title"].ToString();
                }
                if (dr["user_id"] != null && dr["user_id"] != DBNull.Value)
                {
                    config.UserID = Convert.ToInt32(dr["user_id"]);
                }
                if (dr["adgroup_id"] != null && dr["adgroup_id"] != DBNull.Value)
                {
                    config.AdgroupID = Convert.ToInt32(dr["adgroup_id"]);
                }
                if (dr.Table.Columns.Contains("major_codes") && dr["major_codes"] != null && dr["major_codes"] != DBNull.Value)
                {
                    config.MajorCodes = dr["major_codes"].ToString();
                }
                config.TaskDelay = Convert.ToInt32(dr["task_delay"]);
                config.IsEnableAddKeyword = Convert.ToBoolean(dr["is_enable_add_keyword"]);
                if (dr.Table.Columns.Contains("add_keyword_type") && dr["add_keyword_type"] != null && dr["add_keyword_type"] != DBNull.Value)
                {
                    config.AddKeywordType = dr["add_keyword_type"].ToString();
                }
                config.IsMustLessClickCost = Convert.ToBoolean(dr["is_must_less_click_cost"]);
                config.MinPrice = Convert.ToInt32(dr["min_price"]);
                config.MaxWordCount = Convert.ToInt32(dr["max_word_count"]);
                config.MatchScope = dr["match_scope"].ToString();
                config.OptProcess = dr["opt_process"].ToString();
                if (dr.Table.Columns.Contains("run_frequency") && dr["run_frequency"] != null && dr["run_frequency"] != DBNull.Value)
                {
                    config.RunFrequency = Convert.ToInt32(dr["run_frequency"]);
                }
                config.CreateDate = Convert.ToDateTime(dr["create_date"]);
                return config;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 计划报表汇总数
        /// </summary>
        public static EntityCampaignReport CalculateSumReport(List<EntityCampaignReport> lstUserReport)
        {
            EntityCampaignReport rpt = new EntityCampaignReport();
            foreach (EntityCampaignReport report in lstUserReport)
            {
                rpt.impressions += report.impressions;
                rpt.click += report.click;
                rpt.cost += report.cost;
                rpt.totalpay += report.totalpay;
                rpt.totalpaycount += report.totalpaycount;
            }
            if (rpt.impressions != 0)
            {
                rpt.ctr = Math.Round(Convert.ToDecimal(rpt.click * 100) / rpt.impressions, 2);
                if (rpt.click != 0)
                {
                    rpt.rate = Math.Round(Convert.ToDecimal(rpt.totalpaycount * 100) / rpt.click, 2);
                    rpt.cpc = Math.Round(rpt.cost / rpt.click, 2);
                }
                if (rpt.totalpay > 0 && rpt.cost > 0)
                {
                    rpt.roi = Math.Round(rpt.totalpay / rpt.cost, 2);
                }
            }
            rpt.campaign_id = lstUserReport[0].campaign_id;
            return rpt;
        }

        /// <summary>
        /// 调用淘宝API出错后，返回结果是否是频繁访问
        /// </summary>
        public static Boolean IsBanMsg(Top.Api.TopResponse response)
        {
            if (response.ErrMsg == null || response.SubErrMsg == null)
            {
                return false;
            }
            if (response.ErrMsg.Contains("Limited") || response.SubErrMsg.Contains("ban"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<List<T>> SplitLst<T>(List<T> lst, int count)
        {
            List<List<T>> groupLstWord = new List<List<T>>();
            List<T> lstWord = new List<T>();
            for (int i = 0; i < lst.Count; i++)
            {
                if (i % count == 0)
                {
                    if (lstWord.Count != 0)
                    {
                        groupLstWord.Add(lstWord);
                    }
                    lstWord = new List<T>();
                    lstWord.Add(lst[i]);
                }
                else
                {
                    lstWord.Add(lst[i]);
                }
            }
            if (lst.Count != 0)
            {
                groupLstWord.Add(lstWord);
            }
            return groupLstWord;
        }

        /// <summary>
        /// 调用淘宝API封装，参数类型TopSession
        /// </summary>
        /// <typeparam name="T">TopAPI返回类型</typeparam>
        /// <param name="apiMethod">TopAPI方法名</param>
        /// <param name="user">用户token</param>
        /// <param name="reDoTimes">遇到一般性错误的重做次数（不包括ban），默认为0</param>
        /// <param name="executionTimeout">遇到ban时，重做的时间长度</param>
        /// <returns></returns>
        public static T DoTaoBaoApi<T>(Func<TopSession, T> apiMethod, TopSession user, int reDoTimes = 0, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            var response = apiMethod(user);
            DateTime dtStart = DateTime.Now;
            int banCount = 0;
            if (response.IsError)
            {
                if (CommonHandler.IsBanMsg(response))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        banCount++;
                        System.Threading.Thread.Sleep(2000 * banCount);
                        response = apiMethod(user);
                        if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                        {//默认，超过1分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                            {
                                return response;
                            }
                            isBanError = false;
                        }
                    }
                }
                else if (reDoTimes > 0)
                {//遇到一般性错误重试
                    int times = 1;
                    while (response.IsError && times <= reDoTimes)
                    {
                        times++;
                        System.Threading.Thread.Sleep(300);
                        response = apiMethod(user);
                    }
                }
                else
                {
                    return response;
                }
            }

            return response;
        }

        /// <summary>
        /// 调用淘宝API封装，参数类型TopSession,String
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<TopSession, string, T> apiMethod, TopSession user, string strPara, int reDoTimes = 0, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            var response = apiMethod(user, strPara);
            DateTime dtStart = DateTime.Now;
            int banCount = 0;
            if (response.IsError)
            {
                if (CommonHandler.IsBanMsg(response))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        banCount++;
                        System.Threading.Thread.Sleep(2000 * banCount);
                        response = apiMethod(user, strPara);
                        if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                        {//默认，超过1分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                            {
                                return response;
                            }
                            isBanError = false;
                        }
                    }
                }
                else if (reDoTimes > 0)
                {//遇到一般性错误重试
                    int times = 1;
                    while (response.IsError && times <= reDoTimes)
                    {
                        times++;
                        System.Threading.Thread.Sleep(300);
                        response = apiMethod(user, strPara);
                    }
                }
                else
                {
                    return response;
                }
            }

            return response;
        }

        /// <summary>
        /// 调用淘宝API封装，参数类型TopSession,String,String
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<TopSession, string, string, T> apiMethod, TopSession user, string strPara1, string strPara2, int reDoTimes = 0, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            var response = apiMethod(user, strPara1, strPara2);
            DateTime dtStart = DateTime.Now;
            int banCount = 0;
            if (response.IsError)
            {
                if (CommonHandler.IsBanMsg(response))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        banCount++;
                        System.Threading.Thread.Sleep(2000 * banCount);
                        response = apiMethod(user, strPara1, strPara2);
                        if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                        {//默认，超过1分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                            {
                                return response;
                            }
                            isBanError = false;
                        }
                    }
                }
                else if (reDoTimes > 0)
                {//遇到一般性错误重试
                    int times = 1;
                    while (response.IsError && times <= reDoTimes)
                    {
                        times++;
                        System.Threading.Thread.Sleep(300);
                        response = apiMethod(user, strPara1, strPara2);
                    }
                }
                else
                {
                    return response;
                }
            }

            return response;
        }

        /// <summary>
        /// 调用淘宝API封装，参数类型TopSession,long,long,String,String
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<TopSession, long, long, string, string, T> apiMethod, TopSession user, long longPara1, long longPara2, string strPara3, string strPara4, int reDoTimes = 0, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            var response = apiMethod(user, longPara1, longPara2, strPara3, strPara4);
            DateTime dtStart = DateTime.Now;
            int banCount = 0;
            if (response.IsError)
            {
                if (CommonHandler.IsBanMsg(response))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        banCount++;
                        System.Threading.Thread.Sleep(2000 * banCount);
                        response = apiMethod(user, longPara1, longPara2, strPara3, strPara4);
                        if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                        {//默认，超过1分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                            {
                                return response;
                            }
                            isBanError = false;
                        }
                    }
                }
                else if (reDoTimes > 0)
                {//遇到一般性错误重试
                    int times = 1;
                    while (response.IsError && times <= reDoTimes)
                    {
                        times++;
                        System.Threading.Thread.Sleep(300);
                        response = apiMethod(user, longPara1, longPara2, strPara3, strPara4);
                    }
                }
                else
                {
                    return response;
                }
            }

            return response;
        }

        /// <summary>
        /// 调用淘宝API封装，参数类型TopSession,long,String,String
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<TopSession, long, string, string, T> apiMethod, TopSession user, long longPara1, string strPara2, string strPara3, int reDoTimes = 0, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            var response = apiMethod(user, longPara1, strPara2, strPara3);
            DateTime dtStart = DateTime.Now;
            int banCount = 0;
            if (response.IsError)
            {
                if (CommonHandler.IsBanMsg(response))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        banCount++;
                        System.Threading.Thread.Sleep(2000 * banCount);
                        response = apiMethod(user, longPara1, strPara2, strPara3);
                        if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                        {//默认，超过1分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                            {
                                return response;
                            }
                            isBanError = false;
                        }
                    }
                }
                else if (reDoTimes > 0)
                {//遇到一般性错误重试
                    int times = 1;
                    while (response.IsError && times <= reDoTimes)
                    {
                        times++;
                        System.Threading.Thread.Sleep(300);
                        response = apiMethod(user, longPara1, strPara2, strPara3);
                    }
                }
                else
                {
                    return response;
                }
            }

            return response;
        }

        /// <summary>
        /// 调用淘宝API封装，参数类型TopSession,long,long,String,String,String,long,long,String
        /// </summary>
        public static T DoTaoBaoApi<T>(Func<TopSession, long, long, string, string, string, long, long, string, T> apiMethod, TopSession user, long longPara1, long longPara2, string strPara3, string strPara4, string strPara5, long longPara6, long longPara7, string strPara8, int reDoTimes = 0, int executionTimeout = 60) where T : Top.Api.TopResponse
        {
            var response = apiMethod(user, longPara1, longPara2, strPara3, strPara4, strPara5, longPara6, longPara7, strPara8);
            DateTime dtStart = DateTime.Now;
            int banCount = 0;
            if (response.IsError)
            {
                if (CommonHandler.IsBanMsg(response))
                {//遇到频繁访问的错误，需要多次访问
                    Boolean isBanError = true;
                    while (isBanError)
                    {
                        banCount++;
                        System.Threading.Thread.Sleep(2000 * banCount);
                        response = apiMethod(user, longPara1, longPara2, strPara3, strPara4, strPara5, longPara6, longPara7, strPara8);
                        if (response.IsError && IsBanMsg(response) && dtStart.AddSeconds(executionTimeout) > DateTime.Now)
                        {//默认，超过1分钟放弃
                            isBanError = true;
                        }
                        else
                        {
                            if (dtStart.AddSeconds(executionTimeout) <= DateTime.Now)
                            {
                                return response;
                            }
                            isBanError = false;
                        }
                    }
                }
                else if (reDoTimes > 0)
                {//遇到一般性错误重试
                    int times = 1;
                    while (response.IsError && times <= reDoTimes)
                    {
                        times++;
                        System.Threading.Thread.Sleep(300);
                        response = apiMethod(user, longPara1, longPara2, strPara3, strPara4, strPara5, longPara6, longPara7, strPara8);
                    }
                }
                else
                {
                    return response;
                }
            }

            return response;
        }
    }
}