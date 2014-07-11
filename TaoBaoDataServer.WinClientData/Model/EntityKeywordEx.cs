using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    /// <summary>
    /// 关键词扩展类，基于Top.Api.Domain.Keyword扩展
    /// </summary>
    public class EntityKeywordEx
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int local_id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>		
        public int user_id { get; set; }
        /// <summary>
        /// 操作标识，有值说明针对于该关键词的操作已经成功提交至淘宝
        /// </summary>		
        public int optimize_num { get; set; }
        /// <summary>
        /// 0：无，1：加词，2：加价，3：降价，4：删词
        /// </summary>
        public TypeKeywordOpt modify_type { get; set; }
        /// <summary>
        /// 计划ID
        /// </summary>
        public long campaign_id { get; set; }
        /// <summary>
        /// 推广组ID
        /// </summary>
        public long adgroup_id { get; set; }
        /// <summary>
        /// 关键词ID
        /// </summary>		
        public long keyword_id { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>		
        public string keyword { get; set; }
        /// <summary>
        /// 质量得分
        /// </summary>
        public virtual int qscore { get; set; }
        /// <summary>
        /// 对于改价词，表示老的出价，新加的词该值为0，单位：分，暂时不存入数据库
        /// </summary>
        public virtual long old_price { get; set; }
        /// <summary>
        ///  出价（单位：分）
        /// </summary>
        public long max_price { get; set; }

        /// <summary>
        /// 词在类目下平均点击价格
        /// </summary>
        public long avgPrice { get; set; }
        /// <summary>
        /// 是否默认出价
        /// </summary>
        public Boolean is_default_price { get; set; }
        /// <summary>
        /// 匹配方式
        /// </summary>
        public string match_scope { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime create_time { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime modified_time { get; set; }

        /// <summary>
        /// 类目预测级别
        /// </summary>
        public virtual int categoryLevel { get; set; }

        /// <summary>
        /// 展现
        /// </summary>
        public virtual long pv { get; set; }

        /// <summary>
        /// 点击
        /// </summary>
        public virtual long click { get; set; }

        /// <summary>
        /// 点击率
        /// </summary>
        public virtual string ctr { get; set; }

        /// <summary>
        /// 花费
        /// </summary>
        public virtual decimal cost { get; set; }

        /// <summary>
        /// 成交额
        /// </summary>
        public virtual decimal directpay { get; set; }

        /// <summary>
        /// 成交数
        /// </summary>
        public virtual decimal directpayCount { get; set; }

        /// <summary>
        /// 宝贝收藏数
        /// </summary>
        public virtual decimal favitemcount { get; set; }

        /// <summary>
        /// 店铺收藏数
        /// </summary>
        public virtual decimal favshopcount { get; set; }
        /// <summary>
        /// 操作描述
        /// </summary>
        public string record { get; set; }
        /// <summary>
        /// 是否使用0未回滚（可以回滚），1已回滚 或 0撤销（可以撤销），1已撤销
        /// </summary>		
        public string used { get; set; }
        /// <summary>
        /// SEM用户ID
        /// </summary>
        public int sem_user_id { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime create_date { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime update_date { get; set; }
        /// <summary>
        ///  是否有效（1有效，0无效）
        /// </summary>
        public string delete_flag { get; set; }

        /// <summary>
        /// 命中条件
        /// </summary>
        public string match_condition { get; set; }

        /// <summary>
        /// 取词来源
        /// </summary>
        public string FindSource { get; set; }

        /// <summary>
        /// 转换成Top.Api.Domain.Keyword类型
        /// </summary>
        public Top.Api.Domain.Keyword ToKeyword(Boolean isDefaultPrice, string matchScope)
        {
            Top.Api.Domain.Keyword kw = new Top.Api.Domain.Keyword();
            kw.CampaignId = this.campaign_id;
            kw.AdgroupId = this.adgroup_id;
            kw.KeywordId = this.keyword_id;
            kw.Word = this.keyword;
            kw.MaxPrice = this.max_price;
            kw.IsDefaultPrice = false;
            kw.MatchScope = matchScope;

            return kw;
        }

        /// <summary>
        /// 转换成Top.Api.Domain.Keyword类型
        /// </summary>
        public Top.Api.Domain.Keyword ToKeyword(long maxPrice, Boolean isDefaultPrice, string matchScope)
        {
            Top.Api.Domain.Keyword kw = new Top.Api.Domain.Keyword();
            kw.CampaignId = this.campaign_id;
            kw.AdgroupId = this.adgroup_id;
            kw.KeywordId = this.keyword_id;
            kw.Word = this.keyword;
            kw.MaxPrice = maxPrice;
            kw.IsDefaultPrice = false;
            kw.MatchScope = matchScope;

            return kw;
        }
    }

    /// <summary>
    /// 关键词删词改价自定义流程
    /// </summary>
    public class KeywordProcess
    {
        /// <summary>
        /// 0：无，1：加词，2：加价，3：降价，4：删词
        /// </summary>
        public TypeKeywordOpt Opt { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 词属性，1,当天；2，最近3天；3,3天以前；4，所有
        /// </summary>
        public string WordTime { get; set; }
    }

    /// <summary>
    /// 关键词操作枚举
    /// </summary>
    public enum TypeKeywordOpt
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 加词
        /// </summary>
        AutoAddKeyword = 1,
        /// <summary>
        /// 加价
        /// </summary>
        AutoPriceIncrease = 2,
        /// <summary>
        /// 降价
        /// </summary>
        AutoPriceReduce = 3,
        /// <summary>
        /// 删词
        /// </summary>
        AutoDelKeyword = 4,

        /// <summary>
        /// 手动加词
        /// </summary>
        ManualAdd = 8,

        /// <summary>
        /// 手动修改
        /// </summary>
        ManualModify = 9,

        /// <summary>
        /// 手动删除
        /// </summary>
        ManualDel = 10
    }
}