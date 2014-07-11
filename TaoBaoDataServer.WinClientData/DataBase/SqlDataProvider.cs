using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using log4net;
using System.Data;
using TaoBaoDataServer.WinClientData.Model;

namespace TaoBaoDataServer.WinClientData
{
	public class SqlDataProvider
	{
		private static log4net.ILog logAX = LogManager.GetLogger("loggerAX");

		#region Conn

		private static string connectionString;

        /// <summary>
        /// 淘快词智能版，数据库连接字符串
        /// </summary>
        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// 自动开车数据库
        /// </summary>
        public static SqlConnection GetAPSqlConnection()
        {
            return new SqlConnection(Config.ConnectionAP);
        }

		public SqlDataProvider()
		{
			try
			{
				connectionString = Config.ConnectionZhiNeng;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public SqlDataProvider(string con)
		{
			connectionString = con;
		}

		#endregion

        /// <summary>
        /// 从数据某张表的记录
        /// </summary>
        public static List<T> GetDataFromDB<T>(string tableName)
        {

            List<T> result = null;
            using (SqlConnection conn = GetAPSqlConnection())
            {
                string strSql = string.Format("SELECT * FROM {0}", tableName);
                using (SqlDataReader dr = SqlHelper.ExecuteReader(conn, strSql, CommandType.Text))
                {
                    if (dr.HasRows)
                    {
                        result = DTOHelper.CreateModelList<T>(dr);
                    }
                }
            }
            return result ?? new List<T>();

        }

        /// <summary>
        /// 将实体类转换为数据库参数
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns>数据库参数</returns>
        public static SqlParameter[] ConvertSqlParameter(IDictionary<string, object> paramers)
        {
            if (paramers != null)
            {
                var param = new SqlParameter[paramers.Count];
                int i = 0;
                foreach (var property in paramers)
                {
                    param[i] = new SqlParameter("@" + property.Key, property.Value);
                    i++;
                }
                return param;
            }
            else
            {
                return null;
            }
        }

        private Object DBNullParam(Object p)
        {
            if (p == null)
                return DBNull.Value;
            else
                return p;
        }
	}
}
