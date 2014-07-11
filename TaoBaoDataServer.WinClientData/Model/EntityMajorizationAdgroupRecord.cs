using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    /// <summary>
    /// 推广组优化记录表
    /// </summary>
    public class EntityMajorizationAdgroupRecord
    {
        /// <summary>
        /// 本地ID，主键
        /// </summary>
        public int local_id { get; set; }

        /// <summary>
        /// 优化唯一标识码
        /// </summary>
        public string optimize_num { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int user_id { get; set; }

        /// <summary>
        /// 计划ID
        /// </summary>
        public long campaign_id { get; set; }

        /// <summary>
        /// 推广组ID
        /// </summary>
        public long adgroup_id { get; set; }

        /// <summary>
        /// 优化使用的配置，配置ID
        /// </summary>
        public int config_id { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime create_date { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime finish_date { get; set; }
    }
}
