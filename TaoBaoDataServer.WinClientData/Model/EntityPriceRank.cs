using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    /// <summary>
    /// 排名类
    /// </summary>
    public class EntityPriceRank
    {
        /// <summary>
        /// 出价
        /// </summary>
        public long max_price { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        public int rank { get; set; }
    }
}
