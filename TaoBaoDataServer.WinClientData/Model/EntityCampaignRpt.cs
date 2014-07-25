using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    /// <summary>
    /// 计划报表
    /// </summary>
    public class EntityCampaignReport
    {
        public string date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int impressions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int click { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal cost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal ctr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal cpc { get; set; }

        /// <summary>
        /// 直接成交金额
        /// </summary>
        public decimal directpay { get; set; }

        /// <summary>
        /// 间接成交金额
        /// </summary>
        public decimal indirectpay { get; set; }

        /// <summary>
        /// 成交总额
        /// </summary>
        public decimal totalpay { get; set; }


        public decimal roi { get; set; }

        /// <summary>
        /// 总成交数=（直接成交+间接成交）
        /// </summary>
        public int totalpaycount { get; set; }

        /// <summary>
        /// 直接成交数
        /// </summary>
        public int directpaycount { get; set; }

        /// <summary>
        /// 间接成交数
        /// </summary>
        public int indirectpaycount { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int favitemcount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int favshopcount { get; set; }

        /// <summary>
        /// 推广计划编号
        /// </summary>
        public long campaign_id { get; set; }

        /// <summary>
        /// 点击成交转化率
        /// </summary>
        public decimal rate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal avgpos { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string nick { get; set; }
    }
}
