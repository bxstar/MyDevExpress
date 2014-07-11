using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    /// <summary>
    /// 宝贝对象实体
    /// </summary>
    public class EntityItem
    {
        /// <summary>
        /// item_id
        /// </summary>		
        public long item_id { get; set; }
        /// <summary>
        /// user_id
        /// </summary>		
        public long user_id { get; set; }
        /// <summary>
        /// campaign_id
        /// </summary>		
        public long campaign_id { get; set; }
        /// <summary>
        /// item_title
        /// </summary>		
        public string item_title { get; set; }
        /// <summary>
        /// item_url
        /// </summary>		
        public string item_url { get; set; }
        /// <summary>
        /// is_success
        /// </summary>		
        public bool is_success { get; set; }     
    }
}
