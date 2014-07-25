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
        public int local_id { get; set; }

        /// <summary>
        /// 宝贝ID
        /// </summary>		
        public long item_id { get; set; }

        /// <summary>
        /// 宝贝标题
        /// </summary>
        public string item_title { get; set; }

        /// <summary>
        /// 宝贝类目，最底层的类目
        /// </summary>
        public long cid { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        public string categroy_name { get; set; }

        /// <summary>
        /// user_id
        /// </summary>		
        public long user_id { get; set; }
        /// <summary>
        /// campaign_id
        /// </summary>		
        public long campaign_id { get; set; }

        /// <summary>
        /// item_url
        /// </summary>		
        public string item_url { get; set; }

        public DateTime create_date { get; set; }

        public DateTime update_date { get; set; }

        #region 扩展属性
        /// <summary>
        /// 宝贝的属性列表
        /// </summary>
        public List<string> LstPropsName { get; set; }
        #endregion 
    }
}
