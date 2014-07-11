using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    public class EntityAdgroup
    {
        /// <summary>
        /// 
        /// </summary>
        public int localadgroupid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long adgroupid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long campaignid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long numiid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string categoryids { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal defaultprice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string offlinetype { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string onlinestatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime createdate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime updatedate { get; set; }


        /// <summary>
        /// 推广组是否启用优化
        /// </summary>
        public Boolean IsEnableMajorization { get; set; }
    }
}
