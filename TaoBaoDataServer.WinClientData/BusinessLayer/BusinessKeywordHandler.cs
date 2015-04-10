using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TaoBaoDataServer.WinClientData.Model;
using System.Data.SqlClient;
using System.Data;
using Top.Api.Domain;
using iclickpro.AccessCommon;
using Top.Api.Response;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class BusinessKeywordHandler
    {
        private static log4net.ILog logger = LogManager.GetLogger("Logger");
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();

        #region 添加关键词
        /// <summary>
        /// 线上，创建一批关键词 
        /// </summary>
        public ResponseKeyword AddKeywordOnline(TopSession session, long adgroupId, List<Keyword> lstKeyword)
        {
            // 定义返回值
            ResponseKeyword result = new ResponseKeyword();
            result.listResponseKeyword = new List<Keyword>();
            // 对关键词进行分组
            List<List<Keyword>> groupLstword = CommonHandler.SplitLst<Keyword>(lstKeyword, 100);
            // 分组执行添加关键词
            foreach (List<Keyword> group in groupLstword)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var keyword in group)
                {
                    string price = keyword.IsDefaultPrice ? "0" : keyword.MaxPrice.ToString();  //默认出价时price必须为零
                    string matchScope = string.IsNullOrEmpty(keyword.MatchScope) ? "4" : keyword.MatchScope;    //默认广泛匹配
                    sb.Append("{\"word\":\"" + keyword.Word + "\",\"maxPrice\":" + price + ",\"isDefaultPrice\":" + (keyword.IsDefaultPrice ? "1" : "0") + ",\"matchScope\":" + matchScope + "},");
                }
                string responseKeywordidPrices = "[" + sb.ToString().TrimEnd(',') + "]";
                DateTime dtStart = new DateTime();
                // 执行添加关键词操作
                var response = TaobaoApiHandler.TaobaoSimbaKeywordsvonAdd(session, adgroupId, responseKeywordidPrices);
                if (response.IsError)
                {
                    if (CommonHandler.IsBanMsg(response))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            System.Threading.Thread.Sleep(2000);
                            response = TaobaoApiHandler.TaobaoSimbaKeywordsvonAdd(session, adgroupId, responseKeywordidPrices);
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
                        if (result.listResponseKeyword.Count == 0)
                        {
                            logger.Error("线上创建关键词失败，" + response.Body);
                        }
                        else
                        {
                            logger.Error("线上创建关键词失败，部分关键词未能正确创建" + response.Body);
                        }
                        result.ErrorMessage = response.ErrMsg;
                        result.SubErrorMessage = response.SubErrMsg;
                        result.IsError = true;
                        return result;
                    }
                }
                if (response != null && response.Keywords != null && response.Keywords.Count > 0)
                {
                    result.listResponseKeyword.AddRange(response.Keywords);
                    result.IsError = false;
                }
            }
            logger.InfoFormat("线上需要创建{0}个关键词，实际创建{1}个关键词。", lstKeyword.Count, result.listResponseKeyword.Count);
            return result;
        }
        #endregion

        #region 给一批关键词改价
        /// <summary>
        /// 线上，给一批关键词改价
        /// </summary>
        public ResponseKeyword ModifyKeywordOnline(TopSession session, List<Keyword> lstKeyword)
        {
            // 定义返回值
            ResponseKeyword result = new ResponseKeyword();
            result.listResponseKeyword = new List<Keyword>();
            // 对关键词进行分组
            List<List<Keyword>> groupLstword = CommonHandler.SplitLst<Keyword>(lstKeyword, 100);
            // 分组循环执行改价操作
            foreach (List<Keyword> group in groupLstword)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var keyword in group)
                {
                    string price = keyword.IsDefaultPrice ? "0" : keyword.MaxPrice.ToString();  //默认出价时price必须为零
                    sb.Append("{\"keywordId\":\"" + keyword.KeywordId.ToString() + "\",\"maxPrice\":" + price + ",\"isDefaultPrice\":" + (keyword.IsDefaultPrice ? "1" : "0") + ",\"matchScope\":" + keyword.MatchScope + "},");
                }
                string responseKeywordidPrices = "[" + sb.ToString().TrimEnd(',') + "]";
                DateTime dtStart = DateTime.Now;
                var response = TaobaoApiHandler.TaobaoSimbaKeywordsPricevonSet(session, responseKeywordidPrices);
                if (response.IsError)
                {
                    if (CommonHandler.IsBanMsg(response))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            System.Threading.Thread.Sleep(2000);
                            response = TaobaoApiHandler.TaobaoSimbaKeywordsPricevonSet(session, responseKeywordidPrices);
                            if (response.IsError && CommonHandler.IsBanMsg(response) && dtStart.AddMinutes(5) > DateTime.Now)
                            {//超过5分钟放弃
                                isBanError = true;
                            }
                            else
                            {
                                if (dtStart.AddMinutes(5) <= DateTime.Now)
                                {
                                    logger.Error("线上给一批关键词改价出错，已重试5分钟" + response.Body);
                                }
                                isBanError = false;
                            }
                        }
                    }
                    else
                    {
                        if (result.listResponseKeyword.Count == 0)
                        {
                            logger.Error("线上给一批关键词改价失败，" + response.Body);
                        }
                        else
                        {
                            logger.Error("线上给一批关键词改价失败，部分关键词未能正确改价" + response.Body);
                        }
                        result.ErrorMessage = response.ErrMsg;
                        result.SubErrorMessage = response.SubErrMsg;
                        result.IsError = true;
                        return result;
                    }
                }
                if (response != null && response.Keywords != null && response.Keywords.Count > 0)
                {
                    result.listResponseKeyword.AddRange(response.Keywords);
                    result.IsError = false;
                }
            }
            logger.InfoFormat("线上需要修改{0}个关键词，实际修改{1}个关键词。", lstKeyword.Count, result.listResponseKeyword.Count);
            return result;
        }

        #endregion

        #region 删除一批关键词

        /// <summary>
        /// 线上，删除一批关键词
        /// </summary>
        public ResponseKeyword DeleteKeywordOnline(TopSession session, long campaignId, List<long> lstKeywordId)
        {
            List<Keyword> lstKeyword = new List<Keyword>();
            foreach (var item in lstKeywordId)
            {
                Keyword kw = new Keyword();
                kw.KeywordId = item;
                lstKeyword.Add(kw);
            }
            return DeleteKeywordOnline(session, campaignId, lstKeyword);
        }

        /// <summary>
        /// 线上，删除一批关键词
        /// </summary>
        public ResponseKeyword DeleteKeywordOnline(TopSession session, long campaignId, List<Keyword> lstKeyword)
        {
            // 定义返回值
            ResponseKeyword result = new ResponseKeyword();
            result.listResponseKeyword = new List<Keyword>();
            // 对关键词进行分组
            List<List<Keyword>> groupLstword = CommonHandler.SplitLst<Keyword>(lstKeyword, 100);
            // 分组循环执行删除操作
            foreach (List<Keyword> group in groupLstword)
            {
                DateTime dtStart = DateTime.Now;
                var response = TaobaoApiHandler.TaobaoSimbaKeywordsDeleteOne(session, campaignId, string.Join(",", group.Select(o => o.KeywordId.ToString()).ToArray()));
                if (response.IsError)
                {
                    if (CommonHandler.IsBanMsg(response))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            System.Threading.Thread.Sleep(2000);
                            response = TaobaoApiHandler.TaobaoSimbaKeywordsDeleteOne(session, campaignId, string.Join(",", group.Select(o => o.KeywordId.ToString()).ToArray()));
                            if (response.IsError && CommonHandler.IsBanMsg(response) && dtStart.AddMinutes(5) > DateTime.Now)
                            {//超过5分钟放弃
                                isBanError = true;
                            }
                            else
                            {
                                if (dtStart.AddMinutes(5) <= DateTime.Now)
                                {
                                    logger.Error("线上删除一批关键词出错，已重试5分钟" + response.Body);
                                }
                                isBanError = false;
                            }
                        }
                    }
                    else
                    {
                        if (result.listResponseKeyword.Count == 0)
                        {
                            logger.Error("线上删除关键词失败，" + response.Body);
                        }
                        else
                        {
                            logger.Error("线上删除关键词失败，部分关键词未能正确删除" + response.Body);
                        }
                        result.ErrorMessage = response.ErrMsg;
                        result.SubErrorMessage = response.SubErrMsg;
                        result.IsError = true;
                        return result;
                    }
                }
                if (response != null && response.Keywords != null && response.Keywords.Count > 0)
                {
                    result.listResponseKeyword.AddRange(response.Keywords);
                    result.IsError = false;
                }
            }
            logger.InfoFormat("线上需要删除{0}个关键词，实际删除{1}个关键词。", lstKeyword.Count, result.listResponseKeyword.Count);
            return result;
        }


        #endregion

        /// <summary>
        /// 线上，获取推广组下的关键词信息
        /// </summary>
        public Boolean GetKeywordOnline(TopSession session, long adgroupId, ref List<Keyword> listkeyword)
        {
            var lstKeywordReponse = CommonHandler.DoTaoBaoApi<SimbaKeywordsbyadgroupidGetResponse>(TaobaoApiHandler.TaoBaoSimbaKeywordsGet, session, adgroupId);

            if (lstKeywordReponse != null && lstKeywordReponse.Keywords != null)
            {
                listkeyword.AddRange(lstKeywordReponse.Keywords);
            }
            return true;
        }

        /// <summary>
        /// 数据库，添加关键词，不保存操作记录
        /// </summary>
        public void AddKeyword(int userID, List<Keyword> lstKeyword)
        {
            foreach (Keyword keyword in lstKeyword)
            {
                var param = new Dictionary<string, object>();
                param.Add("campaign_id", keyword.CampaignId);
                param.Add("adgroup_id", keyword.AdgroupId);
                param.Add("keyword_id", keyword.KeywordId);
                param.Add("keyword", keyword.Word);
                param.Add("is_default_price", keyword.IsDefaultPrice);
                param.Add("is_garbage", keyword.IsGarbage);
                param.Add("match_scope", keyword.MatchScope == null ? "4" : keyword.MatchScope);
                param.Add("max_price", keyword.MaxPrice);
                param.Add("qscore", keyword.Qscore == null ? "" : keyword.Qscore);
                param.Add("audit_status", keyword.AuditStatus);
                param.Add("audit_desc", keyword.AuditDesc == null ? "" : keyword.AuditDesc);
                param.Add("create_time", keyword.CreateTime);
                param.Add("modified_time", keyword.ModifiedTime);
                param.Add("user_id", userID);
                param.Add("user_type", 1);
                DateTime nowDate = DateTime.Now;
                param.Add("create_date", nowDate);
                param.Add("update_date", nowDate);
                SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), @"INSERT INTO ad_keyword (campaign_id, adgroup_id, keyword_id, keyword, is_default_price, 
					is_garbage, match_scope, max_price, qscore, audit_status, audit_desc, create_time, modified_time, user_id, user_type,
					create_date, update_date) VALUES (@campaign_id, @adgroup_id, @keyword_id, @keyword, 
					@is_default_price, @is_garbage, @match_scope, @max_price, @qscore, @audit_status, @audit_desc, @create_time,
					@modified_time, @user_id, @user_type, @create_date, @update_date);", SqlNameAndParamer.ConvertSqlParameter(param));

            }
        }

        /// <summary>
        /// 数据库，删除关键词，根据推广组编号
        /// </summary>
        public void DeleteKeywordByAdgroupId(int userId, long adgroupId)
        {
            string sqlText = @"DELETE FROM ad_keyword where [user_id] = @user_id and adgroup_id = @adgroup_id";
            var lstParameter = new List<SqlParameter>
									{
										new SqlParameter("@adgroup_id", adgroupId),
										new SqlParameter("@user_id", userId)
									};
            SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), sqlText, lstParameter.ToArray());
        }

        /// <summary>
        /// 数据库，从ad_keyword中找出关键词对象列表，支持模糊查询
        /// </summary>
        public List<Keyword> FindKeyword(int userId, long adgroupId, string keyword, Boolean isLike)
        {
            List<Keyword> lstKeyword = new List<Keyword>();
            var param = new Dictionary<string, object>();
            param.Add("user_id", userId);
            param.Add("adgroup_id", adgroupId);
            param.Add("keyword", keyword);

            string sql = string.Empty;
            if (isLike)
            {//模糊查询
                sql = "select * from ad_keyword where user_id=@user_id and adgroup_id=@adgroup_id and keyword like '%'+@keyword+'%'";
            }
            else
            {
                sql = "select * from ad_keyword where user_id=@user_id and adgroup_id=@adgroup_id and keyword=@keyword";
            }
            DataSet ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), sql, SqlNameAndParamer.ConvertSqlParameter(param));
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Keyword kw = new Keyword();
                    kw.KeywordId = Convert.ToInt64(ds.Tables[0].Rows[i]["keyword_id"]);
                    kw.AdgroupId = adgroupId;
                    kw.CampaignId = Convert.ToInt64(ds.Tables[0].Rows[i]["campaign_id"]);
                    kw.Word = ds.Tables[0].Rows[i]["keyword"].ToString();
                    kw.IsDefaultPrice = ds.Tables[0].Rows[i]["is_default_price"].ToString() == "1" ? true : false;
                    kw.MaxPrice = Int64.Parse(ds.Tables[0].Rows[i]["max_price"].ToString());
                    kw.MatchScope = ds.Tables[0].Rows[i]["match_scope"].ToString();
                    kw.Qscore = ds.Tables[0].Rows[i]["qscore"].ToString();
                    lstKeyword.Add(kw);
                }
            }
            return lstKeyword;
        }

        /// <summary>
        /// 下载关键词的基本报表数据，最近多少天
        /// </summary>
        public List<EntityKeywordRpt> DownLoadKeywordBaseReport(TopSession session, long campaignId, long adgroupId, int days)
        {
            string strStartDay = DateTime.Now.AddDays(0 - days).Date.ToString("yyyy-MM-dd");
            string strEndDay = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
            return DownLoadKeywordBaseReport(session, campaignId, adgroupId, strStartDay, strEndDay);
        }

        /// <summary>
        /// 下载关键词的基本报表数据，时间段
        /// </summary>
        public List<EntityKeywordRpt> DownLoadKeywordBaseReport(TopSession session, long campaignId, long adgroupId, string startDay, string endDay)
        {
            List<EntityKeywordRpt> lstAll = new List<EntityKeywordRpt>();
            //每页返回
            long pageSize = 500;
            long i = 0;
            while (true)
            {
                i = i + 1;
                // 下载推广组基础报表
                var response = CommonHandler.DoTaoBaoApi<SimbaRptAdgroupkeywordbaseGetResponse>(TaobaoApiHandler.TaobaoSimbaRptAdgroupkeywordbaseGet,
                    session, campaignId, adgroupId, startDay, endDay, "SUMMARY", pageSize, i, "SUMMARY");

                if (response == null || String.IsNullOrEmpty(response.RptAdgroupkeywordBaseList) || response.RptAdgroupkeywordBaseList == "[]" || response.RptAdgroupkeywordBaseList == "{}")
                {
                    break;
                }
                int returnPageSize = 0;
                string jsonBaseRpt = response.RptAdgroupkeywordBaseList.ToLower();
                if (!string.IsNullOrEmpty(jsonBaseRpt) && jsonBaseRpt.Length > 2)
                {
                    var data = new DynamicJsonParser().FromJson(jsonBaseRpt);
                    foreach (var item in data)
                    {
                        EntityKeywordRpt rpt = new EntityKeywordRpt();
                        rpt.date = Convert.ToString(item.date);
                        //rpt.nick = item.nick;                 //列太多暂时不显示
                        rpt.campaignid = Convert.ToInt64(item.campaignid);
                        rpt.adgroupid = Convert.ToInt64(item.adgroupid);
                        rpt.keywordid = Convert.ToInt64(item.keywordid);
                        rpt.keywordstr = Convert.ToString(item.keywordstr);
                        rpt.impressions = item.impressions == null ? 0 : Convert.ToInt32(item.impressions);
                        rpt.click = item.click == null ? 0 : Convert.ToInt32(item.click);
                        rpt.cost = item.cost == null ? 0M : Convert.ToDecimal(item.cost);
                        rpt.avgpos = item.avgpos == null ? 0M : Convert.ToDecimal(item.avgpos);
                        rpt.searchtype = Convert.ToString(item.searchtype);
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

            return lstAll;
        }

        /// <summary>
        /// 开始下载关键词的效果报表数据，最近多少天
        /// </summary>
        public List<EntityKeywordRpt> DownLoadKeywordEffectReport(TopSession session, long campaignId, long adgroupId, int days)
        {
            string strStartDay = DateTime.Now.AddDays(0 - days).Date.ToString("yyyy-MM-dd");
            string strEndDay = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
            return DownLoadKeywordEffectReport(session, campaignId, adgroupId, strStartDay, strEndDay);
        }

        /// <summary>
        /// 开始下载关键词的效果报表数据，时间段
        /// </summary>
        public List<EntityKeywordRpt> DownLoadKeywordEffectReport(TopSession session, long campaignId, long adgroupId, string startDay, string endDay)
        {
            List<EntityKeywordRpt> lstAll = new List<EntityKeywordRpt>();

            //每页返回
            long pageSize = 500;
            long pageIndex = 0;
            while (true)
            {
                pageIndex = pageIndex + 1;
                // 下载推广组效果报表
                var response = CommonHandler.DoTaoBaoApi<SimbaRptAdgroupkeywordeffectGetResponse>(TaobaoApiHandler.TaobaoSimbaRptAdgroupkeywordeffectGet, session, campaignId, adgroupId, startDay, endDay, "SUMMARY", pageSize, pageIndex, "SUMMARY");
                if (response == null || String.IsNullOrEmpty(response.RptAdgroupkeywordEffectList) || response.RptAdgroupkeywordEffectList == "[]" || response.RptAdgroupkeywordEffectList == "{}")
                {
                    break;
                }
                int returnPageSize = 0;
                string jsonEffectRpt = response.RptAdgroupkeywordEffectList.ToLower();
                if (!string.IsNullOrEmpty(jsonEffectRpt) && jsonEffectRpt.Length > 2)
                {
                    var data = new DynamicJsonParser().FromJson(jsonEffectRpt);
                    foreach (var item in data)
                    {
                        EntityKeywordRpt rpt = new EntityKeywordRpt();
                        rpt.date = Convert.ToString(item.date);
                        //rpt.nick = item.nick;              //列太多暂时不显示
                        rpt.campaignid = Convert.ToInt64(item.campaignid);
                        rpt.adgroupid = Convert.ToInt64(item.adgroupid);
                        rpt.keywordid = Convert.ToInt64(item.keywordid);
                        rpt.keywordstr = Convert.ToString(item.keywordstr);
                        rpt.directpay = item.directpay == null ? 0M : Convert.ToDecimal(item.directpay);
                        rpt.indirectpay = item.indirectpay == null ? 0M : Convert.ToDecimal(item.indirectpay);
                        rpt.directpaycount = item.directpaycount == null ? 0 : Convert.ToInt32(item.directpaycount);
                        rpt.indirectpaycount = item.indirectpaycount == null ? 0 : Convert.ToInt32(item.indirectpaycount);
                        rpt.favitemcount = item.favitemcount == null ? 0 : Convert.ToInt32(item.favitemcount);
                        rpt.favshopcount = item.favshopcount == null ? 0 : Convert.ToInt32(item.favshopcount);
                        rpt.searchtype = Convert.ToString(item.searchtype);
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
            return lstAll;
        }

        /// <summary>
        /// 获取关键词黑名单，白名单
        /// </summary>
        public List<EntityKeywordCustom> GetKeywordCustom(long adgroupId, Boolean isContainTaoBaoBlackList)
        {
            string strSql = string.Empty;
            if (isContainTaoBaoBlackList)
            {//包含淘宝认定的非法词
                strSql = string.Format("SELECT * FROM ad_keyword_custom akc WHERE akc.adgroup_id={0} OR (user_id IS NULL)", adgroupId);
            }
            else
            {
                strSql = string.Format("SELECT * FROM ad_keyword_custom akc WHERE akc.adgroup_id={0}", adgroupId);
            }
            var ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), strSql);
            List<EntityKeywordCustom> result = new List<EntityKeywordCustom>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    EntityKeywordCustom model = SetKeywordCustomFromDataRow(dr);
                    result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取用户和全局级别黑名单
        /// </summary>
        public List<EntityKeywordCustom> GetKeywordBlackList(int userId)
        {
            string strSql = string.Empty;
            //包含淘宝认定的非法词
            strSql = string.Format("SELECT * FROM ad_keyword_custom akc WHERE custom_type=0 and (akc.user_id={0} OR (user_id IS NULL))", userId);

            var ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), strSql);
            List<EntityKeywordCustom> result = new List<EntityKeywordCustom>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    EntityKeywordCustom model = SetKeywordCustomFromDataRow(dr);
                    result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// 保存关键词黑名单，白名单，并将其发布至线上
        /// </summary>
        public string SaveKeywordCustomLocalAndOnline(TopSession session, List<EntityKeywordCustom> lstNewKeywordCustom, long campaignId, long adgroupId, TypeKeywordCustomType customType)
        {
            //对于已经存在的，比较价格，匹配方式，如果不一样，修改
            //对于不存在的，添加
            //对于在旧关键词黑名单或白名单中没找到的，删除

            string errorMsg = string.Empty;
            List<EntityKeywordCustom> lstOldKeywordCustom = GetKeywordCustom(adgroupId, false);

            List<EntityKeywordCustom> lstNeedUpdate = new List<EntityKeywordCustom>();                  //需要修改的关键词黑名单或白名单
            List<EntityKeywordCustom> lstNeedUpdateOnline_AddLocal = new List<EntityKeywordCustom>();   //线上有关键词，本地没有的白名单，需要更新线上，并在本地加入白名单
            List<EntityKeywordCustom> lstNeedAdd = new List<EntityKeywordCustom>();                     //需要新增的关键词黑名单或白名单

            foreach (var newItem in lstNewKeywordCustom)
            {
                if (lstOldKeywordCustom.Where(o => o.keyword == newItem.keyword && o.custom_type != newItem.custom_type).Count() > 0)
                {//白名单的关键词不能设置到黑名单中去，黑名单的关键词也不能设置到白名单中去 
                    if (newItem.custom_type == TypeKeywordCustomType.WhiteList)
                    {
                        errorMsg = string.Format("关键词“{0}”已经在黑名单中存在，如果需要加入白名单，请先将其在黑名单中删除！", newItem.keyword);
                    }
                    else if (newItem.custom_type == TypeKeywordCustomType.BlackList)
                    {
                        errorMsg = string.Format("关键词“{0}”已经在白名单中存在，如果需要加入黑名单，请先将其在白名单中删除！", newItem.keyword);
                    }
                    return errorMsg;
                }

                EntityKeywordCustom oldItem = lstOldKeywordCustom.Find(o => o.keyword == newItem.keyword);
                if (oldItem == null)
                {
                    if (newItem.custom_type == TypeKeywordCustomType.WhiteList)
                    {//白名单有两种情况
                        var result = FindKeyword(session.UserID, adgroupId, newItem.keyword, false);
                        if (result.Count == 0)
                            lstNeedAdd.Add(newItem);                    //本地白名单和线上不存在该关键词，本地白名单和线上关键词都作为新增
                        else
                        {
                            newItem.keywordId = result[0].KeywordId;
                            newItem.is_succeed = true;                  //默认作为成功
                            lstNeedUpdateOnline_AddLocal.Add(newItem);  //本地白名单没有，线上存在该关键词，本地白名单作为新增，线上关键词作为修改
                        }

                    }
                    else
                    {//黑名单作为需要新增的 
                        lstNeedAdd.Add(newItem);
                    }
                }
                else
                {
                    if (newItem.custom_type == TypeKeywordCustomType.WhiteList)
                    {//白名单才需要修改的操作 
                        if (newItem.max_price != oldItem.max_price || newItem.match_scope != oldItem.match_scope)
                        {
                            lstNeedUpdate.Add(newItem);
                        }
                    }
                }
            }

            //需要删除的关键词黑名单或白名单
            List<EntityKeywordCustom> lstNeedDel = lstOldKeywordCustom.Where(x => x.custom_type == customType && !lstNewKeywordCustom.Select(y => y.keyword).Contains(x.keyword)).ToList();

            if (lstNeedAdd.Count > 0)
            {
                if (customType == TypeKeywordCustomType.WhiteList)
                {//新增白名单的关键词，添加至线上
                    var responseAddKeywordOnline = AddKeywordOnline(session, adgroupId, lstNeedAdd.Select(o => new Keyword() { Word = o.keyword, MaxPrice = o.max_price, MatchScope = o.match_scope }).ToList());
                    if (responseAddKeywordOnline.IsError)
                    {
                        errorMsg = responseAddKeywordOnline.SubErrorMessage;
                        logger.ErrorFormat("用户{0}保存关键词黑白名单出错：{1}", session.UserName, responseAddKeywordOnline.SubErrorMessage);
                        return errorMsg;
                    }
                    else
                    {
                        if (responseAddKeywordOnline.listResponseKeyword != null)
                        {
                            foreach (var item in lstNeedAdd)
                            {
                                if (responseAddKeywordOnline.listResponseKeyword.Select(o => o.Word).Contains(item.keyword))
                                    item.is_succeed = true;
                                else
                                    item.is_succeed = false;
                            }
                            AddKeywordCustom(lstNeedAdd);
                            AddKeyword(session.UserID, responseAddKeywordOnline.listResponseKeyword);
                        }
                    }
                }
                else if (customType == TypeKeywordCustomType.BlackList)
                {//新增黑名单的关键词，在线上删除
                    List<long> blackListKeywordId = new List<long>();
                    foreach (var item in lstNeedAdd)
                    {
                        List<Keyword> blackKeyword = FindKeyword(item.user_id, item.adgroup_id, item.keyword, item.match_scope != "1" ? true : false);
                        if (blackKeyword != null)
                        {//获取满足黑名单的关键词ID，如果关键词和数据库不是一致的，会导致黑名单的关键词无法删除，因为关键词一样但ID不一致，需要重新下载推广组的关键词再设置一次黑名单
                            blackListKeywordId.AddRange(blackKeyword.Select(o => o.KeywordId));
                        }
                    }
                    var responseDelKeywordOnline = DeleteKeywordOnline(session, campaignId, blackListKeywordId);
                    if (responseDelKeywordOnline.listResponseKeyword != null)
                    {
                        foreach (var item in lstNeedAdd)
                        {//黑名单的删除都返回成功
                            item.is_succeed = true;
                        }
                        AddKeywordCustom(lstNeedAdd);
                    }
                }
            }

            if (lstNeedUpdate.Count > 0 && customType == TypeKeywordCustomType.WhiteList)
            {//白名单才需要修改的操作 
                List<Keyword> lstKeywordNeedUpdate = new List<Keyword>();
                foreach (var item in lstNeedUpdate)
                {
                    Keyword k = new Keyword() { Word = item.keyword, MaxPrice = item.max_price, MatchScope = item.match_scope };
                    List<Keyword> lstFinded = FindKeyword(session.UserID, adgroupId, k.Word, false);
                    if (lstFinded.Count == 0)
                    {
                        return string.Format("关键词“{0}”在数据库中不存在，请先同步推广组下的关键词", item.keyword);
                    }
                    k.KeywordId = lstFinded[0].KeywordId;
                    lstKeywordNeedUpdate.Add(k);
                }

                var responseUpdateKeywordOnline = ModifyKeywordOnline(session, lstKeywordNeedUpdate);
                if (responseUpdateKeywordOnline.IsError)
                {
                    errorMsg = responseUpdateKeywordOnline.SubErrorMessage;
                    logger.ErrorFormat("用户{0}保存关键词黑白名单出错：{1}", session.UserName, responseUpdateKeywordOnline.SubErrorMessage);
                    return errorMsg;
                }
                else
                {
                    if (responseUpdateKeywordOnline.listResponseKeyword != null)
                    {
                        foreach (var item in lstNeedUpdate)
                        {
                            if (responseUpdateKeywordOnline.listResponseKeyword.Select(o => o.Word).Contains(item.keyword))
                                item.is_succeed = true;
                            else
                                item.is_succeed = false;
                        }
                        UpdateKeywordCustom(lstNeedUpdate);
                    }
                }

            }

            if (lstNeedUpdateOnline_AddLocal.Count > 0)
            {//本地加入白名单，线上更新关键词
                var responseUpdateKeywordOnline = ModifyKeywordOnline(session, lstNeedUpdateOnline_AddLocal.
                    Select(o => new Keyword() { KeywordId = o.keywordId, Word = o.keyword, MaxPrice = o.max_price, MatchScope = o.match_scope }).ToList());
                if (responseUpdateKeywordOnline.IsError)
                {
                    errorMsg = responseUpdateKeywordOnline.SubErrorMessage;
                    logger.ErrorFormat("用户{0}保存关键词黑白名单出错：{1}", session.UserName, responseUpdateKeywordOnline.SubErrorMessage);
                    return errorMsg;
                }
                else
                {
                    AddKeywordCustom(lstNeedUpdateOnline_AddLocal);
                }
            }

            if (lstNeedDel.Count > 0)
            {//本地删除黑白名单
                DeleteKeywordCustom(lstNeedDel);
            }

            return errorMsg;
        }

        /// <summary>
        /// 数据库，增加关键词白名单或黑名单
        /// </summary>
        public void AddKeywordCustom(List<EntityKeywordCustom> lst)
        {
            foreach (var model in lst)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into ad_keyword_custom(");
                strSql.Append("remark,find_source,create_date,update_date,user_id,adgroup_id,keyword,max_price,match_scope,custom_type,is_succeed,error_msg");
                strSql.Append(") values (");
                strSql.Append("@remark,@find_source,getdate(),getdate(),@user_id,@adgroup_id,@keyword,@max_price,@match_scope,@custom_type,@is_succeed,@error_msg");
                strSql.Append(") ");
                SqlParameter[] parameters = {
						new SqlParameter("@remark", SqlDbType.NVarChar,500) ,
						new SqlParameter("@find_source", SqlDbType.NVarChar,20) ,
						new SqlParameter("@user_id", SqlDbType.Int,4) ,
						new SqlParameter("@adgroup_id", SqlDbType.BigInt,8) ,
						new SqlParameter("@keyword", SqlDbType.NVarChar,200) ,
						new SqlParameter("@max_price", SqlDbType.BigInt,8) ,
						new SqlParameter("@match_scope", SqlDbType.NVarChar,5) ,
						new SqlParameter("@custom_type", SqlDbType.Int,4) ,
						new SqlParameter("@is_succeed", SqlDbType.Bit,1) ,
						new SqlParameter("@error_msg", SqlDbType.NVarChar,50)
						};

                parameters[0].Value = model.remark != null ? model.remark : (object)DBNull.Value;
                parameters[1].Value = model.find_source != null ? model.find_source : (object)DBNull.Value;
                parameters[2].Value = model.user_id == 0 ? DBNull.Value : (object)model.user_id;
                parameters[3].Value = model.adgroup_id == 0 ? DBNull.Value : (object)model.adgroup_id;
                parameters[4].Value = model.keyword;
                parameters[5].Value = model.max_price;
                parameters[6].Value = model.match_scope;
                parameters[7].Value = model.custom_type;
                parameters[8].Value = model.is_succeed;
                parameters[9].Value = model.error_msg != null ? model.error_msg : (object)DBNull.Value;

                SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), strSql.ToString(), parameters);

            }
        }

        /// <summary>
        /// 数据库，更新关键词白名单或黑名单
        /// </summary>
        public void UpdateKeywordCustom(List<EntityKeywordCustom> lst)
        {
            foreach (var model in lst)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update ad_keyword_custom set ");

                strSql.Append(" remark = @remark , ");
                strSql.Append(" find_source = @find_source , ");
                strSql.Append(" update_date = getdate() , ");
                strSql.Append(" max_price = @max_price , ");
                strSql.Append(" match_scope = @match_scope , ");
                strSql.Append(" is_succeed = @is_succeed , ");
                strSql.Append(" error_msg = @error_msg  ");
                strSql.Append(" where adgroup_id=@adgroup_id and keyword=@keyword ");

                SqlParameter[] parameters = {        
						new SqlParameter("@remark", SqlDbType.NVarChar,500) ,            
						new SqlParameter("@find_source", SqlDbType.NVarChar,20) ,                     
						new SqlParameter("@adgroup_id", SqlDbType.BigInt,8) ,            
						new SqlParameter("@keyword", SqlDbType.NVarChar,200) ,            
						new SqlParameter("@max_price", SqlDbType.BigInt,8) ,            
						new SqlParameter("@match_scope", SqlDbType.NVarChar,5) ,                
						new SqlParameter("@is_succeed", SqlDbType.Bit,1) ,            
						new SqlParameter("@error_msg", SqlDbType.NVarChar,50)             
						};

                parameters[0].Value = model.remark != null ? model.remark : (object)DBNull.Value;
                parameters[1].Value = model.find_source != null ? model.find_source : (object)DBNull.Value;
                parameters[2].Value = model.adgroup_id;
                parameters[3].Value = model.keyword;
                parameters[4].Value = model.max_price;
                parameters[5].Value = model.match_scope;
                parameters[6].Value = model.is_succeed;
                parameters[7].Value = model.error_msg != null ? model.error_msg : (object)DBNull.Value;

                SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), strSql.ToString(), parameters);
            }
        }

        /// <summary>
        /// 数据库，删除关键词白名单或黑名单
        /// </summary>
        public void DeleteKeywordCustom(List<EntityKeywordCustom> lst)
        {
            foreach (var model in lst)
            {
                string strSql = "delete from ad_keyword_custom where adgroup_id=@adgroup_id and keyword=@keyword";
                SqlParameter[] parameters = {
						new SqlParameter("@adgroup_id", SqlDbType.BigInt,8) ,            
						new SqlParameter("@keyword", SqlDbType.NVarChar,200)
											};

                parameters[0].Value = model.adgroup_id;
                parameters[1].Value = model.keyword;

                SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), strSql.ToString(), parameters);
            }
        }


        /// <summary>
        /// 设置KeywordCustom对象
        /// </summary>
        private EntityKeywordCustom SetKeywordCustomFromDataRow(DataRow dr)
        {
            EntityKeywordCustom model = new EntityKeywordCustom();
            model.local_id = int.Parse(dr["local_id"].ToString());
            if (dr["user_id"] != DBNull.Value)
            {
                model.user_id = int.Parse(dr["user_id"].ToString());
            }
            if (dr["adgroup_id"] != DBNull.Value)
            {
                model.adgroup_id = long.Parse(dr["adgroup_id"].ToString());
            }
            model.keyword = dr["keyword"].ToString();
            if (dr["max_price"] != DBNull.Value)
            {
                model.max_price = long.Parse(dr["max_price"].ToString());
            }
            model.match_scope = dr["match_scope"].ToString();
            model.custom_type = (TypeKeywordCustomType)int.Parse(dr["custom_type"].ToString());
            model.is_succeed = Convert.ToBoolean(dr["is_succeed"]);
            if (dr["remark"] != DBNull.Value)
            {
                model.remark = dr["remark"].ToString();
            }
            if (dr["find_source"] != DBNull.Value)
            {
                model.find_source = dr["find_source"].ToString();
            }
            if (dr["create_date"] != DBNull.Value)
            {
                model.create_date = Convert.ToDateTime(dr["create_date"]);
            }
            if (dr["update_date"] != DBNull.Value)
            {
                model.update_date = Convert.ToDateTime(dr["update_date"]);
            }
            return model;
        }
    }
}