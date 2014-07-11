using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    public class EntityCampaign
    {
        /// <summary>
        /// 主键，推广计划标识
        /// </summary>
        public int localcampaignid { get; set; }

        /// <summary>
        /// 推广计划ID，来自淘宝
        /// </summary>
        public long campaignid { get; set; }

        /// <summary>
        /// 推广计划名称
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 用户设置的上下限状态；offline-下线；online-上线；
        /// </summary>
        public string onlinestatus { get; set; }

        /// <summary>
        /// 推广计划结算下线原因，1-余额不足；2-超过日限额，以分号分隔多个下线原因
        /// </summary>
        public string settlereason { get; set; }

        /// <summary>
        /// 推广计划结算状态，offline-下线；online-上线，
        /// </summary>
        public string settlestatus { get; set; }

        /// <summary>
        /// 日限额
        /// </summary>
        public decimal dailylimit { get; set; }

        /// <summary>
        /// 单次点击最高限价
        /// </summary>
        public decimal clickcost { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int userid { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string nick { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime createdate { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime updatedate { get; set; }
    }
}
