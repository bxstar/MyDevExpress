using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    [Serializable]
    public class KeywordAvgData
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Word { get; set; }
        /// <summary>
        /// 词在类目下平均点击价格
        /// </summary>
        public long AvgPrice { get; set; }

        /// <summary>
        /// 词在类目下点击量
        /// </summary>
        public long Click { get; set; }

        /// <summary>
        /// 词在类目下竞争宝贝数(包括未展现客户)
        /// </summary>
        public long Competition { get; set; }

        /// <summary>
        /// 点击率
        /// </summary>
        public string Ctr { get; set; }

        /// <summary>
        /// 建议出价
        /// </summary>
        public double SuggestPrice { get; set; }

        /// <summary>
        ///  天数
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// 词在类目下展现量
        /// </summary>
        public long Pv { get; set; }

        /// <summary>
        /// 预测等级
        /// </summary>
        public int CategoryLevel { get; set; }

        /// <summary>
        ///  预测后的类目ID
        /// </summary>
        public long CategoryId { get; set; }

    }

    public class KeywordAvgDataComparer : IEqualityComparer<KeywordAvgData>
    {
        public bool Equals(KeywordAvgData p1, KeywordAvgData p2)
        {
            if (p1 == null)
                return p2 == null;
            return p1.Word == p2.Word;
        }

        public int GetHashCode(KeywordAvgData p)
        {
            if (p == null)
                return 0;
            return p.Word.GetHashCode();
        }
    }
}
