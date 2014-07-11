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

        public decimal avgpos { get; set; }

        public decimal directpay { get; set; }


        public decimal indirectpay { get; set; }

        public decimal roi { get; set; }


        public int directpaycount { get; set; }


        public int indirectpaycount { get; set; }


        public int favitemcount { get; set; }


        public int favshopcount { get; set; }


        public string searchtype { get; set; }


        public string source { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public int local_id { get; set; }
    }
}
