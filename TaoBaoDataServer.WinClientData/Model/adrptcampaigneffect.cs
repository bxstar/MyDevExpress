using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    public class adrptcampaigneffect
    {

        public virtual string date { get; set; }

        public virtual string nick { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string directpay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string indirectpay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string directpaycount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string indirectpaycount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string favitemcount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string favshopcount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string searchtype { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string source { get; set; }


    }
}
