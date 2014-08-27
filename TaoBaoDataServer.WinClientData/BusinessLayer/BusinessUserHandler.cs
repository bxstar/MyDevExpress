using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TaoBaoDataServer.WinClientData.Model;
using System.Data;
using iclickpro.AccessCommon;
using Top.Api.Response;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class BusinessUserHandler
    {
        private static log4net.ILog logger = LogManager.GetLogger("Logger");
        BusinessTaobaoApiHandler taobaoApiHandler = new BusinessTaobaoApiHandler();

        /// <summary>
        /// 根据用户昵称获取用户session
        /// </summary>
        public TopSession GetUserSession(string nick)
        {
            TopSession session = new TopSession();
            var param = new Dictionary<string, object>();
            param.Add("nick", nick);
            DataSet ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), "SELECT * FROM ad_user WHERE proxy_user_name=[user_name] AND proxy_user_name=@nick", SqlNameAndParamer.ConvertSqlParameter(param));
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                session.UserID = Convert.ToInt32(ds.Tables[0].Rows[0]["local_user_id"]);
                session.ProxyUserName = ds.Tables[0].Rows[0]["proxy_user_name"].ToString();
                session.UserName = ds.Tables[0].Rows[0]["user_name"].ToString();
                session.TopSessions = ds.Tables[0].Rows[0]["user_session"].ToString();
                session.IsEnableMajorization = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsEnableMajorization"]);
                session.MajorConfigs = GetUserMajorConfigs(session.UserID);
                session.CreateDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["create_date"]);
            }
            return session;
        }

        /// <summary>
        /// 获取系统默认的和该用户的优化配置
        /// </summary>
        public List<EntityMajorConfig> GetUserMajorConfigs(int userID)
        {
            List<EntityMajorConfig> majorConfigs = new List<EntityMajorConfig>();
            var dsMajorConfig = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), string.Format("SELECT * FROM ad_majorization_config WHERE [user_id]={0} OR [user_id] IS NULL", userID));
            if (dsMajorConfig != null && dsMajorConfig.Tables.Count > 0 && dsMajorConfig.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsMajorConfig.Tables[0].Rows.Count; j++)
                {
                    System.Data.DataRow dr = dsMajorConfig.Tables[0].Rows[j];
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
                    config.TaskDelay = Convert.ToInt32(dr["task_delay"]);
                    config.IsEnableAddKeyword = Convert.ToBoolean(dr["is_enable_add_keyword"]);
                    config.IsMustLessClickCost = Convert.ToBoolean(dr["is_must_less_click_cost"]);
                    config.MinPrice = Convert.ToInt32(dr["min_price"]);
                    config.MaxWordCount = Convert.ToInt32(dr["max_word_count"]);
                    config.MatchScope = dr["match_scope"].ToString();
                    config.OptProcess = dr["opt_process"].ToString();
                    config.CreateDate = Convert.ToDateTime(dr["create_date"]);
                    majorConfigs.Add(config);
                }
            }
            return majorConfigs;
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        public bool UpdateUserState(int userId, int deleteFlag)
        {
            bool result = false;

            // 更新用户的状态
            var param = new Dictionary<string, object>();
            param.Add("local_user_id", userId);
            param.Add("delete_flag", deleteFlag);
            // 更新数据库影响的行数
            var nRowCount = SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), "UPDATE ad_user SET update_date=getdate(), delete_flag =@delete_flag WHERE local_user_id =@local_user_id", SqlNameAndParamer.ConvertSqlParameter(param));
            if (nRowCount > 0)
            {
                result = true;
            }

            return result;
        }


        /// <summary>
        /// 数据库，获取用户信息
        /// </summary>
        public List<TopSession> GetUserInfo(int? userId)
        {
            List<TopSession> lstUser = new List<TopSession>();
            try
            {
                // 读取数据库，从数据库中取得数据
                string strSql = string.Empty;
                if (userId == null)
                {
                    strSql = "SELECT a.* ,b.campaign_id,b.title campaign_name FROM ad_user a left join ad_campaign b on a.local_user_id=b.user_id where a.delete_flag = '1' and b.campaign_id is not null";
                }
                else
                {
                    strSql = string.Format("SELECT a.* ,b.campaign_id,b.title campaign_name FROM ad_user a left join ad_campaign b on a.local_user_id=b.user_id where local_user_id={0} ", userId);
                }
                DataSet ds = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), strSql);
                // 从数据库中读取用户信息
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        TopSession session = new TopSession();
                        DataRow drUser = ds.Tables[0].Rows[i];
                        SetModelByDataRow(drUser, session);
                        lstUser.Add(session);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessUserHandler/GetUserInfo,获取所有用户信息错误：", ex);
            }
            return lstUser;
        }

        /// <summary>
        /// 获取购买了托管版收费代码的客户
        /// </summary>
        public List<TopSession> GetCheckAfterUser(string ArticleCode)
        {
            // 从数据库中获取宝贝信息
            List<TopSession> userList = GetUserInfo(null);


            foreach (TopSession session in userList)
            {
                try
                {
                    //根据收费代码设置托管无效用户
                    EntityUserSubscribe userSubscribe = GetUserSubscribe(session, ArticleCode);
                    logger.InfoFormat("用户{0},ID{1},获取的收费代码为{2}", session.ProxyUserName, session.UserID, userSubscribe.item_codes);
                    if (userSubscribe.item_codes == null)
                    {//没有收费代码，token失效，则标识优化到期
                        UpdateUserState(session.UserID, 0);
                        session.Expire = true;
                    }
                    else
                    {
                        if (session.ItemCodes != userSubscribe.item_codes || session.DeadLine != userSubscribe.dead_line)
                        {//收费代码更新
                            session.ItemCodes = userSubscribe.item_codes;
                            session.DeadLine = userSubscribe.dead_line;
                            UpdateUserSubscribe(session);
                        }
                        session.Expire = false;
                    }
                }
                catch (Exception se)
                {
                    logger.Error("用户{0},ID{1},获取收费代码出错", se);
                }
            }

            // 返回未过期用户
            return userList.Where(o => !o.Expire).ToList();

        }

        /// <summary>
        /// 根据DataRow数据生成User对象
        /// </summary>
        private void SetModelByDataRow(DataRow drUser, TopSession session)
        {
            session.UserID = Convert.ToInt32(drUser["local_user_id"]);
            session.CampaignId = Convert.ToInt64(drUser["campaign_id"]);
            session.CampaignName = drUser["campaign_name"].ToString();
            session.ProxyUserName = drUser["proxy_user_name"].ToString();
            session.UserName = drUser["user_name"].ToString();
            session.TopSessions = drUser["user_session"].ToString();
            session.ItemCodes = drUser["item_codes"].ToString();
            session.DeadLine = drUser["deadline"].ToString();
            session.IsEnableMajorization = Convert.ToBoolean(drUser["IsEnableMajorization"]);
            session.MajorConfigs = GetUserMajorConfigs(session.UserID);
            session.CreateDate = Convert.ToDateTime(drUser["create_date"]);
            if (drUser["auth2_date"] != null && drUser["auth2_date"] != DBNull.Value)
            {
                session.IsAuth2 = true;
                session.Auth2Date = Convert.ToDateTime(drUser["auth2_date"]);
            }
            session.UserQQ = drUser["user_qq"].ToString();
            session.MainWangWang = drUser["main_wangwang"].ToString();
            session.ShopperWangWang = drUser["shopper_wangwang"].ToString();
            session.AvgProfitRate = drUser["avg_profit_rate"].ToString();
            session.UserEMail = drUser["user_email"].ToString();
        }

        /// <summary>
        /// 线上，返回用户的收费代码
        /// </summary>
        public EntityUserSubscribe GetUserSubscribe(TopSession user, string ArticleCode)
        {
            EntityUserSubscribe model = new EntityUserSubscribe();
            model.user_name = user.ProxyUserName;
            model.article_code = ArticleCode;
            var response = CommonHandler.DoTaoBaoApi<VasSubscribeGetResponse>(taobaoApiHandler.TaoBaoVasSubscribeGet, user, ArticleCode, 2);
            if (response != null && response.ArticleUserSubscribes != null && response.ArticleUserSubscribes.Count > 0)
            {
                model.item_codes = string.Join(",", response.ArticleUserSubscribes.Select(o => o.ItemCode).ToArray());
                model.dead_line = response.ArticleUserSubscribes.Select(o => o.Deadline).Max();
            }
            return model;
        }

        /// <summary>
        /// 更新用户的订购信息
        /// </summary>
        public bool UpdateUserSubscribe(TopSession user)
        {
            try
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                var param = new Dictionary<string, object>();
                param.Add("local_user_id", user.UserID);
                param.Add("item_codes", user.ItemCodes == null ? DBNull.Value : (object)user.ItemCodes);
                param.Add("deadline", user.DeadLine == null ? DBNull.Value : (object)user.DeadLine);
                SqlHelper.ExecuteNonQuery(SqlDataProvider.GetAPSqlConnection(), "UPDATE ad_user SET item_codes = @item_codes,deadline=@deadline,update_date=getdate() WHERE local_user_id = @local_user_id;", SqlNameAndParamer.ConvertSqlParameter(param));
            }
            catch (Exception se)
            {
                logger.Error("更新用户订购信息错误：", se);
                return false;
            }
            return true;
        }
    }
}