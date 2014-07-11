using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaoBaoDataServer.WinClientData.Model
{
    [Serializable]
    public class KeywordAndCategory
    {
        /// <summary>
        ///  关键词
        /// </summary>
        public virtual string Word { get; set; }

        /// <summary>
        ///  类目等级
        /// </summary>
        public virtual int CategoryLevel { get; set; }

        /// <summary>
        ///  类目Id
        /// </summary>
        public virtual string CategoryId { get; set; }
    }
}
