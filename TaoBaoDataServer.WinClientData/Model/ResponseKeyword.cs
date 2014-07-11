using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Top.Api.Domain;

namespace TaoBaoDataServer.WinClientData.Model
{
    [Serializable]
    public class ResponseKeyword
    {
        /// <summary>
        ///  提交返回成功的关键词
        /// </summary>
        public List<Keyword> listResponseKeyword { get; set; }

        /// <summary>
        /// 是否错误信息
        /// </summary>
        public Boolean IsError { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        ///  子错误信息
        /// </summary>
        public string SubErrorMessage { get; set; }
    }
}
