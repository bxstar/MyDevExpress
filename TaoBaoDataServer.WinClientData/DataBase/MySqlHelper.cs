using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;

namespace TaoBaoDataServer.WinClientData.DataBase
{
    /// <summary>
    /// MySql操作封装
    /// </summary>
    public class MySqlHelper
    {
        public static string ConnString = Config.ConnectionAP;

        #region Execute and return DataSet or DataTable
        public static DataSet ExecuteDataSet(string SQL, string DBName)
        {
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(SQL, Config.GetCfgValue(DBName)))
            {
                DataSet ds = new DataSet();
                try
                {
                    adapter.SelectCommand.CommandTimeout = 600000;
                    adapter.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + " 数据库操作错误，请检测SQL语句：" + SQL);
                }
                return ds;
            }
        }

        public static DataSet ExecuteDataSet(string SQL)
        {
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(SQL, ConnString))
            {
                DataSet ds = new DataSet();
                try
                {
                    adapter.SelectCommand.CommandTimeout = 600000;
                    adapter.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + " 数据库操作错误，请检测SQL语句：" + SQL);
                }
                return ds;
            }
        }

        /// <summary>
        /// 通过sql语句返回导出CSV格式的string
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static string ExportCSV(string SQL)
        {
            StringBuilder sb = new StringBuilder();
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();

                MySqlDataReader dr = cmd.ExecuteReader();

                for (int count = 2; count < dr.FieldCount; count++)
                {
                    if (dr.GetName(count) != null)
                        sb.Append(dr.GetName(count));
                    if (count < dr.FieldCount - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("\r\n");
                while (dr.Read())
                {
                    for (int col = 2; col < dr.FieldCount; col++)
                    {
                        if (!dr.IsDBNull(col))
                        {
                            if (dr.GetValue(col).ToString().Contains(","))
                            {
                                sb.Append("\"" + dr.GetValue(col).ToString() + "\"");
                            }
                            else
                            {
                                sb.Append(dr.GetValue(col).ToString());
                            }
                        }
                        if (col != dr.FieldCount - 1)
                            sb.Append(",");

                    }
                    sb.Append("\r\n");
                    //if (!dr.IsDBNull(dr.FieldCount - 1))
                    //    sb.Append(dr.GetValue(
                    //    dr.FieldCount - 1).ToString().Replace(",", " "));
                }
                dr.Dispose();

            }
            return sb.ToString();
        }

        public static DataSet ExecuteDataSet(string spName, MySqlParameter[] sps)
        {
            using (MySqlDataAdapter adapter = new MySqlDataAdapter())
            {
                MySqlCommand cmd = new MySqlCommand(spName, new MySqlConnection(ConnString));
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (MySqlParameter p in sps)
                {
                    cmd.Parameters.Add(p);
                }
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adapter.Fill(ds);   //adapter.FillSchema(ds,SchemaType.Mapped);
                return ds;
            }
        }

        public static DataTable ExecuteTable(string SQL)
        {
            return ExecuteDataSet(SQL).Tables[0];
        }

        public static DataTable ExecuteTable(string SQL, string DBName)
        {
            return ExecuteDataSet(SQL, DBName).Tables[0];
        }

        public static DataTable ExecuteTable(string SQL, params object[] args)
        {
            return ExecuteTable(string.Format(SQL, args));
        }

        public static DataTable ExecuteTable(string SQL, string DBName, params object[] args)
        {
            return ExecuteTable(string.Format(SQL, args), DBName);
        }

        public static DataRow ExecuteRow(string SQL)
        {
            DataTable t = ExecuteDataSet(SQL).Tables[0];
            if (t.Rows.Count == 1)
                return t.Rows[0];
            return null;
        }

        public static DataRow ExecuteRow(string SQL, params object[] args)
        {
            return ExecuteRow(string.Format(SQL, args));
        }



        public static DataRow ExecuteRow(string spName, MySqlParameter[] sps)
        {
            DataTable t = ExecuteDataSet(spName, sps).Tables[0];
            if (t.Rows.Count == 1)
                return t.Rows[0];
            return null;
        }
        #endregion

        #region Execute None
        public static int ExecuteNone(string SQL)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();
                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + " 数据库操作错误，请检测SQL语句：" + SQL);
                }
            }
        }

        public static int ExecuteNone(string SQL, string type, params object[] args)
        {
            return ExecuteNone(string.Format(SQL, args), type);
        }


        public static int ExecuteSpNone(string spName, MySqlParameter[] ps)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                MySqlCommand cmd = new MySqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (MySqlParameter p in ps)
                {
                    cmd.Parameters.Add(p);
                }
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static int ExecuteTransSpNone(string spName, MySqlParameter[] ps)
        {
            int i = -1;
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                MySqlTransaction trans = null;

                conn.Open();
                trans = conn.BeginTransaction();
                MySqlCommand cmd = new MySqlCommand(spName, conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (MySqlParameter p in ps)
                {
                    cmd.Parameters.Add(p);
                }

                try
                {

                    i = cmd.ExecuteNonQuery();
                    trans.Commit();
                    return i;
                }
                catch (MySqlException se)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw se;
                }
                finally
                {
                    if (trans != null)
                        trans.Dispose();
                }
            }
        }
        #endregion

        #region Execute DataReader
        public static IDataReader ExecuteReader(string SQL)
        {
            MySqlConnection conn = new MySqlConnection(ConnString);
            MySqlCommand cmd = new MySqlCommand(SQL, conn);
            conn.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        public static IDataReader ExecuteReader(string SQL, string type, params object[] args)
        {
            return ExecuteReader(string.Format(SQL, args));
        }
        #endregion

        #region Execute object
        public static Object ExecuteObject(string SQL)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();
                try
                {
                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + " 数据库操作错误，请检测SQL语句：" + SQL);
                }
                return cmd.ExecuteScalar();
            }
        }

        public static object ExecuteObject(string SQL, string type, params object[] args)
        {
            return ExecuteObject(string.Format(SQL, args), type);
        }

        public static object ExecuteObject(string SQL, CommandType type2, MySqlParameter[] ps)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                MySqlCommand cmd = new MySqlCommand(SQL, conn);
                cmd.CommandType = type2;

                foreach (MySqlParameter p in ps)
                {
                    cmd.Parameters.Add(p);
                }
                conn.Open();
                cmd.Prepare();
                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj;
            }
        }
        #endregion

        #region Execute trans

        public static int ExecuteNone(string SQL, MySqlParameter[] ps)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                try
                {
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.Text;
                    //foreach (MySqlParameter p in ps)
                    for (int i = 0; i < ps.Length; i++)
                    {
                        if (ps[i] != null)
                        {
                            cmd.Parameters.Add(ps[i]);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            break;
                        }
                    }
                    trans.Commit();
                    return 1;
                }
                catch (MySqlException se)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw se;
                }
                finally
                {
                    if (trans != null)
                        trans.Dispose();
                }
            }
        }

        /// <summary>
        /// 适合于多个方法执行，保证数据完整性和一致性
        /// </summary>
        /// <param name="tran">事物</param>
        /// <param name="SQL">语句</param>
        /// <param name="ps">参数</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static int ExecuteNone(MySqlTransaction tran, string SQL, MySqlParameter[] ps)
        {
            if (tran == null)
            {
                using (MySqlConnection conn = new MySqlConnection(ConnString))
                {
                    conn.Open();
                    MySqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(SQL, conn);
                        cmd.CommandType = CommandType.Text;
                        for (int i = 0; i < ps.Length; i++)
                        {
                            if (ps[i] != null)
                            {
                                cmd.Parameters.Add(ps[i]);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                break;
                            }
                        }
                        trans.Commit();
                        return 1;
                    }
                    catch (MySqlException se)
                    {
                        if (trans != null)
                            trans.Rollback();
                        throw se;
                    }
                    finally
                    {
                        if (trans != null)
                            trans.Dispose();
                    }
                }
            }
            else
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(SQL, tran.Connection, tran);
                    cmd.CommandType = CommandType.Text;
                    for (int i = 0; i < ps.Length; i++)
                    {
                        if (ps[i] != null)
                        {
                            cmd.Parameters.Add(ps[i]);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            break;
                        }
                    }
                    return 1;
                }
                catch (MySqlException se)
                {
                    throw se;
                }
            }
        }

        public static int ExecuteNone2(string SQL, MySqlParameter[] ps, string DBName)
        {
            using (MySqlConnection conn = new MySqlConnection(Config.GetCfgValue(DBName)))
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.Text;
                    foreach (MySqlParameter p in ps)
                    {
                        cmd.Parameters.Add(p);
                    }
                    conn.Open();
                    cmd.Prepare();

                    cmd.ExecuteNonQuery();
                    return 1;
                }
                catch (MySqlException se)
                {
                    throw se;
                }
                finally
                {
                }
            }
        }

        /// <summary>
        /// 批量执行多条有一个参数的语句
        /// </summary>
        /// <param name="SQL">带参数的sql语句(只能有一个参数)</param>
        /// <param name="ps">参数(必须和sql语句一一对应)</param>
        /// <param name="type">1表示业务数据库 2表账户数据库</param>
        /// <returns></returns>
        public static int ExecuteNone2(string[] SQL, MySqlParameter[] ps)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;
                try
                {
                    for (int i = 0; i < SQL.Length; i++)
                    {
                        cmd.CommandText = SQL[i];
                        cmd.Parameters.Add(ps[i]);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return 1;
                }
                catch (MySqlException se)
                {
                    trans.Rollback();
                    throw se;
                }
                finally
                {
                }
            }
        }

        public static int ExecuteTransForOdbc(params string[] args)
        {
            MySqlConnection conn = new MySqlConnection(ConnString);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();
            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;

                for (int i = 0; i < args.Length; i++)
                {
                    cmd.CommandText = args[i].ToString();
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        /// <summary>
        /// 批量处理数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="args">语句</param>
        /// <returns></returns>
        public static int ExecuteTransForOdbc_Arrylist(ArrayList args)
        {
            MySqlConnection conn = new MySqlConnection(ConnString);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();
            string s = "";
            try
            {
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;

                for (int i = 0; i < args.Count; i++)
                {
                    s = args[i].ToString();
                    cmd.CommandText = args[i].ToString();
                    cmd.ExecuteNonQuery();
                }

                trans.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                WriteLog(s);
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="s"></param>
        public static void WriteLog(string s)
        {
            //Stream stream = new FileStream(@"C:\\数据库操作日志.txt", FileMode.OpenOrCreate);
            //StreamWriter writer = new StreamWriter(stream, Encoding.GetEncoding("gb2312"));
            //writer.WriteLine(s);
            //writer.Close();
            //stream.Close();
        }

        /// <summary>
        /// 批量处理数据(有事物)
        /// </summary>
        /// <param name="tran">事物</param>
        /// <param name="type">类型</param>
        /// <param name="args">语句</param>
        /// <returns></returns>
        public static int ExecuteTransForOdbc_Arrylist(MySqlTransaction tran, ArrayList args)
        {
            if (tran == null)
            {
                MySqlConnection conn = new MySqlConnection(ConnString);
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                try
                {
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.Connection = conn;

                    for (int i = 0; i < args.Count; i++)
                    {
                        cmd.CommandText = args[i].ToString();
                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            else
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(tran.Connection.ConnectionString);
                    for (int i = 0; i < args.Count; i++)
                    {
                        cmd.CommandText = args[i].ToString();
                        cmd.ExecuteNonQuery();
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //批量获取数据
        public static DataSet ExecuteDataSetForOdbc(string[] str)
        {
            using (MySqlDataAdapter adapter = new MySqlDataAdapter())
            {
                MySqlConnection conn = new MySqlConnection(ConnString);
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                for (int i = 0; i < str.Length; i++)
                {
                    cmd.CommandText = str[i];
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                }
                trans.Commit();
                return ds;
            }
        }

        public static DataSet ExecuteDataSetForOdbc(ArrayList list)
        {
            using (MySqlDataAdapter adapter = new MySqlDataAdapter())
            {
                MySqlConnection conn = new MySqlConnection(ConnString);
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                for (int i = 0; i < list.Count; i++)
                {
                    cmd.CommandText = list[i].ToString();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                }
                trans.Commit();
                return ds;
            }
        }

        public static DataSet ExecuteDataSetForOdbc(string SQL, MySqlParameter[] ps, string DBName)
        {
            using (MySqlDataAdapter adapter = new MySqlDataAdapter())
            {
                MySqlConnection conn = new MySqlConnection(Config.GetCfgValue(DBName));
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                try
                {
                    DataSet ds = new DataSet();
                    MySqlCommand cmd = new MySqlCommand(SQL, conn);
                    cmd.CommandType = CommandType.Text;
                    foreach (MySqlParameter p in ps)
                    {
                        if (p != null)
                        {
                            cmd.Parameters.Add(p);
                            cmd.Prepare();
                            adapter.SelectCommand = cmd;
                            adapter.Fill(ds);
                        }
                        else
                        {
                            break;
                        }
                    }
                    trans.Commit();
                    return ds;
                }
                catch (MySqlException se)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw se;
                }
                finally
                {
                    if (trans != null)
                        trans.Dispose();
                }
            }
        }

        public static int ExecuteTrans(string sql)
        {
            MySqlConnection conn = new MySqlConnection(ConnString);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn, trans);
                cmd.CommandType = CommandType.Text;
                int r = cmd.ExecuteNonQuery();
                trans.Commit();
                return r;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
        public static int ExecuteTrans(string SQL, params object[] args)
        {
            return ExecuteTrans(string.Format(SQL, args));
        }
        public static int ExecuteTrans(StringBuilder b)
        {
            return ExecuteTrans(b.ToString());
        }

        #region 参数化执行SQL

        /// <summary>
        /// 参数化执行SQL
        /// </summary>
        public static object ExecuteObjectByPara(string SQL, SqlPara[] paras)
        {
            return ExecuteObject(SQL, CommandType.Text, SqlParaToMySqlPara(paras));
        }
        /// <summary>
        /// 参数化执行SQL
        /// </summary>
        public static void ExecuteNoneByPara(string SQL, SqlPara[] paras, string DBName)
        {
            ExecuteNone2(SQL, SqlParaToMySqlPara(paras), DBName);
        }
        /// <summary>
        /// 参数化执行SQL
        /// </summary>
        public static DataSet ExecuteDataSetByPara(string SQL, SqlPara[] paras, string DBName)
        {
            return ExecuteDataSetForOdbc(SQL, SqlParaToMySqlPara(paras), DBName);
        }

        private static MySqlParameter[] SqlParaToMySqlPara(SqlPara[] paras)
        {
            if (paras == null) return null;
            List<MySqlParameter> lstPara = new List<MySqlParameter>(paras.Length);
            foreach (var item in paras)
            {
                MySqlParameter p = new MySqlParameter();
                p.ParameterName = item.ParaName;
                p.Size = item.ParaLength;
                p.Value = item.ParaValue;
                p.MySqlDbType = item.GetMySqlDbType();

                lstPara.Add(p);
            }
            return lstPara.ToArray();
        }
        #endregion

        #endregion

        #region select
        public static DataTable GetData(string tb, string qury)
        {
            return ExecuteTable(string.Format("select * from {0} {1}", tb, qury));
        }
        public static DataRow GetRowData(string tb, string qury)
        {
            return ExecuteRow(string.Format("select * from {0} {1}", tb, qury));
        }
        public static DataTable GetAllData(string tb)
        {
            return GetData(tb, "");
        }

        #endregion

    }

    /// <summary>
    /// 参数类
    /// </summary>
    public class SqlPara
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParaName { get; set; }
        /// <summary>
        /// 参数的数据类型
        /// </summary>
        public SqlParaType ParaType { get; set; }
        /// <summary>
        /// 参数值的数据长度
        /// </summary>
        public int ParaLength { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object ParaValue { get; set; }

        public SqlPara() { }

        public SqlPara(string pName, SqlParaType pType, int pLength, object pValue)
        {
            ParaName = pName;
            ParaType = pType;
            ParaLength = pLength;
            ParaValue = pValue;
        }

        public MySqlDbType GetMySqlDbType()
        {
            if (this.ParaType == SqlParaType.VarChar)
            {
                return MySqlDbType.VarChar;
            }
            else if (this.ParaType == SqlParaType.Int)
            {
                return MySqlDbType.Int32;
            }
            else if (this.ParaType == SqlParaType.LongInt)
            {
                return MySqlDbType.Int64;
            }
            else if (this.ParaType == SqlParaType.Bool)
            {
                return MySqlDbType.Bit;
            }
            else if (this.ParaType == SqlParaType.DateTime)
            {
                return MySqlDbType.DateTime;
            }
            else if (this.ParaType == SqlParaType.Text)
            {
                return MySqlDbType.Text;
            }
            else
            {
                return MySqlDbType.VarChar;
            }
        }
    }

    /// <summary>
    /// 参数的数据类型
    /// </summary>
    public enum SqlParaType
    {
        /// <summary>
        /// 字符串型
        /// </summary>
        VarChar,
        /// <summary>
        /// 长整型
        /// </summary>
        Int,
        /// <summary>
        /// 长整型
        /// </summary>
        LongInt,
        /// <summary>
        /// 布尔型
        /// </summary>
        Bool,
        /// <summary>
        /// 时间
        /// </summary>
        DateTime,
        /// <summary>
        /// 文本型
        /// </summary>
        Text
    }
}
