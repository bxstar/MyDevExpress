using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    /// <summary>
    /// 淘宝宝贝实体类
    /// </summary>
    public class EntityItem
    {
        /// <summary>
        /// 本地ID
        /// </summary>
        public int local_id { get; set; }

        /// <summary>
        /// 宝贝ID
        /// </summary>		
        public long item_id { get; set; }

        /// <summary>
        /// 宝贝的卖家昵称
        /// </summary>
        public string nick { get; set; }

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
        public string category_name { get; set; }

        /// <summary>
        /// 宝贝的主图地址
        /// </summary>
        public string pic_url { get; set; }
        #region 扩展属性
        /// <summary>
        /// 商品图片，多幅
        /// </summary>
        public List<string> LstItemImg { get; set; }

        /// <summary>
        /// 宝贝的属性列表
        /// </summary>
        public List<string> LstPropsName { get; set; }
        #endregion

    }
}
