using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    public class EntityKeywordRpt
    {
        public string date { get; set; }

        /// <summary>
        /// 用户名，暂时不显示
        /// </summary>
        //public string nick { get; set; }


        public long campaignid { get; set; }


        public long adgroupid { get; set; }


        public long keywordid { get; set; }

        public string keywordstr { get; set; }

        public int impressions { get; set; }


        public int click { get; set; }


        public decimal cost { get; set; }


        public decimal ctr { get; set; }


        public decimal cpc { get; set; }

        /// <summary>
        /// 平均排名
        /// </summary>
        public decimal avgpos { get; set; }

        /// <summary>
        /// 直接转化金额，单位（元）
        /// </summary>
        public decimal directpay { get; set; }

        /// <summary>
        /// 间接转化金额，单位（元）
        /// </summary>
        public decimal indirectpay { get; set; }

        /// <summary>
        /// 总转化金额，单位（元）
        /// </summary>
        public decimal pay
        {
            get { return directpay + indirectpay; }
        }

        public decimal roi { get; set; }

        /// <summary>
        /// 直接转化数
        /// </summary>
        public int directpaycount { get; set; }

        /// <summary>
        /// 间接转化数
        /// </summary>
        public int indirectpaycount { get; set; }

        /// <summary>
        /// 总转化数
        /// </summary>
        public int paycount
        {
            get { return directpaycount + indirectpaycount; }
        }

        /// <summary>
        /// 宝贝收藏数
        /// </summary>
        public int favitemcount { get; set; }

        /// <summary>
        /// 店铺收藏数
        /// </summary>
        public int favshopcount { get; set; }

        /// <summary>
        /// 总收藏数
        /// </summary>
        public int favcount
        {
            get { return favitemcount + favshopcount; }

        }

        public string searchtype { get; set; }

        /// <summary>
        /// 来源，1站内，2站外
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public int local_id { get; set; }
    }
}
