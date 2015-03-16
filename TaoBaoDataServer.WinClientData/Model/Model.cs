using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    [Serializable]
    public class TopSession
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 推广计划ID
        /// </summary>
        public long CampaignId { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// sessionkey
        /// </summary>
        public string TopSessions { get; set; }

        /// <summary>
        /// 代理的子账户名
        /// </summary>
        public string ProxyUserName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 托管是否过期，true表示过期
        /// </summary>
        public bool Expire { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long OnlineUserID { get; set; }

        /// <summary>
        /// 登陆签名
        /// </summary>
        public string SubwayToken { get; set; }

        /// <summary>
        /// 优化配置参数
        /// </summary>
        public List<EntityMajorConfig> MajorConfigs { get; set; }

        /// <summary>
        /// 获取推广组的优化配置，如果该推广组没有配置则拿用户的优化配置，如果该用户没有配置则拿系统默认的优化配置
        /// </summary>
        /// <param name="AdgroupID">推广组ID，0表示获取用户或系统的默认值</param>
        /// <returns></returns>
        public EntityMajorConfig GetMajorConfig(long adgroupId)
        {
            EntityMajorConfig majorConfig = MajorConfigs.Find(o => o.AdgroupID == adgroupId && o.UserID != 0);
            if (majorConfig == null)
            {//推广组不存在，取用户默认配置
                majorConfig = MajorConfigs.Find(o => o.AdgroupID == 0 && o.UserID != 0);
                if (majorConfig == null)
                {//用户默认配置不存在，去系统默认配置
                    majorConfig = MajorConfigs.Find(o => o.UserID == 0);
                }
            }
            return majorConfig;
        }

        /// <summary>
        /// 代理用户
        /// </summary>
        public List<string> PoxyUserList { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 是否托管中
        /// </summary>
        public bool DeleteFlag { get; set; }

        /// <summary>
        /// 是否优化
        /// </summary>
        public bool IsEnableMajorization { get; set; }

        /// <summary>
        /// 收费代码
        /// </summary>
        public string ItemCodes { get; set; }

        /// <summary>
        /// 订购关系到期时间，如：2000-01-01 00:00:00
        /// </summary>
        public string DeadLine { get; set; }

        /// <summary>
        /// 是否二次授权
        /// </summary>
        public Boolean IsAuth2 { get; set; }

        /// <summary>
        /// 最近二次授权的时间
        /// </summary>
        public DateTime Auth2Date { get; set; }


        #region 扩展的用户信息
        /// <summary>
        /// QQ
        /// </summary>
        public string UserQQ { get; set; }

        /// <summary>
        /// 主旺旺
        /// </summary>
        public string MainWangWang { get; set; }

        /// <summary>
        /// 店长旺旺
        /// </summary>
        public string ShopperWangWang { get; set; }

        /// <summary>
        /// 店铺平均利润率
        /// </summary>
        public string AvgProfitRate { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string UserEMail { get; set; }

        #endregion
    }

    /// <summary>
    /// 优化配置参数类，映射ad_majorization_config
    /// </summary>
    public class EntityMajorConfig
    {
        public EntityMajorConfig() { }

        public EntityMajorConfig(int userID, long adgoupId, int taskDelay)
        {
            UserID = userID;
            AdgroupID = adgoupId;
            TaskDelay = taskDelay;
        }
        /// <summary>
        /// 主键，编号
        /// </summary>
        public int LocalID { get; set; }

        /// <summary>
        /// 配置名称，不允许重复，使用固定名称完成特殊功能
        /// </summary>
        public string ConfigTitle { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 推广组ID，为空则表示该配置作用于用户所有的推广组
        /// </summary>
        public long AdgroupID { get; set; }

        /// <summary>
        /// 优化代码，与其匹配的推广组才能执行该策略
        /// </summary>
        public string MajorCodes { get; set; }

        /// <summary>
        /// 任务延迟时间（单位：分钟），任务从生产到开始执行的延迟时间，该时间内可以停止或修改任务
        /// </summary>
        public int TaskDelay { get; set; }

        /// <summary>
        /// 是否启用加词，True：词不够最大词数时加词 False：不加词
        /// </summary>
        public Boolean IsEnableAddKeyword { get; set; }

        /// <summary>
        /// 加词的方式，用不同的来源等区分
        /// </summary>
        public string AddKeywordType { get; set; }

        /// <summary>
        /// 加词时，是否必须小于单次点击限价
        /// </summary>
        public Boolean IsMustLessClickCost { get; set; }

        /// <summary>
        /// 最低价格，默认加词不小于这个价格（单位：分）
        /// </summary>
        public int MinPrice { get; set; }

        /// <summary>
        /// 最大加词数量
        /// </summary>
        public int MaxWordCount { get; set; }

        /// <summary>
        /// 1代表精确匹配，2代表子串匹配，4代表广泛匹配
        /// </summary>
        public string MatchScope { get; set; }

        /*
        格式：“删词：(质量得分<5)并且(点击=0)并且(删词补充逻辑)；词属性：创建日期=2
                改价：质量得分>8 ；词属性：创建日期=2
                删词：展现=0并且(删词补充逻辑) ；词属性：创建日期>3
                删词：(展现>=500)并且(点击=0)并且(删词补充逻辑)；词属性：创建日期>3
                删词：展现<100并且点击=0并且(删词补充逻辑)；词属性：创建日期>3
                删词：点击率<0.1并且(删词补充逻辑)；词属性：创建日期>3
                改价：点击率>=0.1；词属性：创建日期>3'”
        */
        /// <summary>
        /// 删词改价处理流程，多条用换行分隔
        /// </summary>
        public string OptProcess { get; set; }

        /// <summary>
        /// 执行频率
        /// </summary>
        public int RunFrequency { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 备份时间
        /// </summary>
        public DateTime BackUpDate { get; set; }
    }

    /// <summary>
    /// 词库类
    /// </summary>
    public class KeywordBank
    {
        /// <summary>
        /// fID
        /// </summary>		
        public int fID { get; set; }
        /// <summary>
        /// fCategoryId
        /// </summary>		
        public long fCategoryId { get; set; }
        /// <summary>
        /// fCoreKeyword
        /// </summary>		
        public string fCoreKeyword { get; set; }
        /// <summary>
        /// fKeyword
        /// </summary>		
        public string fKeyword { get; set; }
        /// <summary>
        /// fpv
        /// </summary>		
        public long fpv { get; set; }
        /// <summary>
        /// fClick
        /// </summary>		
        public long fClick { get; set; }
        /// <summary>
        /// fCtr
        /// </summary>		
        public decimal fCtr { get; set; }
        /// <summary>
        /// fCpc
        /// </summary>		
        public decimal fCpc { get; set; }
        /// <summary>
        /// fConversionRate
        /// </summary>		
        public decimal fConversionRate { get; set; }
        /// <summary>
        /// fCategoryLevel
        /// </summary>		
        public int fCategoryLevel { get; set; }
        /// <summary>
        /// fCategoryFullId
        /// </summary>		
        public string fCategoryFullId { get; set; }
        /// <summary>
        /// fCategoryPath
        /// </summary>		
        public string fCategoryPath { get; set; }
        /// <summary>
        /// fCompetition
        /// </summary>		
        public long fCompetition { get; set; }
        /// <summary>
        /// fCreateDate
        /// </summary>		
        public DateTime fCreateDate { get; set; }
        /// <summary>
        /// fCreateUpdate
        /// </summary>		
        public DateTime fCreateUpdate { get; set; }
    }

    public enum ReturnStatus
    { 
        error,
        success,
        unmanaged
    }

    /// <summary>
    /// 用户订购信息，实体类
    /// </summary>
    public class EntityUserSubscribe
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// 订购代码
        /// </summary>
        public string article_code { get; set; }

        /// <summary>
        /// 收费代码
        /// </summary>
        public string item_codes { get; set; }

        /// <summary>
        /// 订购关系到期时间，如：2000-01-01 00:00:00
        /// </summary>
        public string dead_line { get; set; }
    }
}
