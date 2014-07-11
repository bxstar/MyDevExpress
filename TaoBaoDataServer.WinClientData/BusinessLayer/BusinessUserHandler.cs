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
        private static log4net.ILog logger = LogManager.GetLogger("loggerAX");
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
            List<TopSession> listSession = new List<TopSession>();
            // 读取数据库，从数据库中取得数据
            string strSql = string.Empty;
            if (userId == null)
            {
                strSql = "SELECT local_user_id,online_user_id ,campaign_id,title, user_name,proxy_user_name,user_session,a.create_date,delete_flag,IsEnableMajorization FROM ad_user a left join ad_campaign b on a.local_user_id=b.user_id ";
            }
            else
            {
                strSql = string.Format("SELECT local_user_id,online_user_id ,campaign_id,title, user_name,proxy_user_name,user_session,a.create_date,delete_flag,IsEnableMajorization FROM ad_user a left join ad_campaign b on a.local_user_id=b.user_id where local_user_id={0}", userId);
            }
            var dsUserInfo = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), strSql);
            // 从数据库中读取用户信息
            if (dsUserInfo != null && dsUserInfo.Tables.Count > 0 && dsUserInfo.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsUserInfo.Tables[0].Rows.Count; i++)
                {
                    TopSession session = new TopSession();
                    // 获取淘宝线上ID
                    session.OnlineUserID = Convert.ToInt64(dsUserInfo.Tables[0].Rows[i]["online_user_id"]);
                    // 获取淘宝用户名
                    session.UserName = dsUserInfo.Tables[0].Rows[i]["user_name"].ToString();
                    session.ProxyUserName = dsUserInfo.Tables[0].Rows[i]["proxy_user_name"].ToString();
                    // 获取本地ID
                    session.UserID = Convert.ToInt32(dsUserInfo.Tables[0].Rows[i]["local_user_id"]);
                    if (dsUserInfo.Tables[0].Rows[i]["campaign_id"] != DBNull.Value)
                        session.CampaignId = Convert.ToInt64(dsUserInfo.Tables[0].Rows[i]["campaign_id"]);
                    if (dsUserInfo.Tables[0].Rows[i]["title"] != DBNull.Value)
                        session.CampaignName = dsUserInfo.Tables[0].Rows[i]["title"].ToString();
                    // 获取用户的Session
                    session.TopSessions = dsUserInfo.Tables[0].Rows[i]["user_session"].ToString();
                    // 获取用户的优化配置
                    session.MajorConfigs = GetUserMajorConfigs(session.UserID);
                    session.CreateDate = Convert.ToDateTime(dsUserInfo.Tables[0].Rows[i]["create_date"]);
                    session.IsEnableMajorization = Convert.ToBoolean(dsUserInfo.Tables[0].Rows[i]["IsEnableMajorization"]);
                    if (dsUserInfo.Tables[0].Rows[i]["delete_flag"] != DBNull.Value)
                        session.DeleteFlag = dsUserInfo.Tables[0].Rows[i]["delete_flag"].ToString() == "1" ? true : false;
                    // 加入返回值
                    listSession.Add(session);
                }
            }
            return listSession;
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
    }
}
