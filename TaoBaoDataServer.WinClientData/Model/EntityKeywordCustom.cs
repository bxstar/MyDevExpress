using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    /// <summary>
    /// 关键词人工设置类
    /// </summary>
    public class EntityKeywordCustom
    {
        /// <summary>
        /// 唯一标识id
        /// </summary>		
        public int local_id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>		
        public int user_id { get; set; }
        /// <summary>
        /// 推广组id
        /// </summary>		
        public long adgroup_id { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>		
        public string keyword { get; set; }
        /// <summary>
        /// 出价（单位：分）
        /// </summary>		
        public long max_price { get; set; }
        /// <summary>
        /// 匹配方式（1代表精确匹配，2代表子串匹配，4代表广泛匹配）
        /// </summary>
        public string match_scope { get; set; }
        /// <summary>
        /// 匹配方式，显示
        /// </summary>
        public string match_scope_display 
        {
            get 
            {
                if (custom_type == TypeKeywordCustomType.WhiteList)
                {
                    if (match_scope == "1")
                        return "精确匹配";
                    else if (match_scope == "2")
                        return "子串匹配";
                    else if (match_scope == "4")
                        return "广泛匹配";
                }
                else if (custom_type == TypeKeywordCustomType.BlackList)
                {
                    if (match_scope == "1")
                        return "精确匹配";
                    else
                        return "模糊匹配";
                }

                return ""; 
            }
        }
        /// <summary>
        /// 关键词人工设置类型，0黑名单，1白名单
        /// </summary>		
        public TypeKeywordCustomType custom_type { get; set; }
        /// <summary>
        /// 是否成功设置了黑名单或白名单，0失败，1成功
        /// </summary>		
        public bool is_succeed { get; set; }
        /// <summary>
        /// 操作失败的错误信息
        /// </summary>
        public string error_msg { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        public string remark { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string find_source { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime create_date { get; set; }
        /// <summary>
        /// 更改日期
        /// </summary>		
        public DateTime update_date { get; set; }

        #region 附加属性
        public long keywordId { get; set; }
        #endregion
    }

    /// <summary>
    /// 关键词自定义类型
    /// </summary>
    public enum TypeKeywordCustomType
    {
        /// <summary>
        /// 黑名单
        /// </summary>
        BlackList = 0,
        /// <summary>
        /// 白名单
        /// </summary>
        WhiteList = 1
    }
}
