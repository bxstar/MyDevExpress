/****************************** Module Header ******************************\
* Module Name:    DTOHelper.cs
* Project:        ServiceCommon
* Copyright (c) NCSoft Corporation.
*
* Provide a way to convert ADO.NET Data objcet to relevant Entity(Model) object(s)
* by fast reflection. 
* 
* Performance Test:
* ==============================================================
* 
* Get Model
* 
* by Common Way:            7600ms. 
* by DTOHelper Way:         7750ms.
* 
* ==============================================================
* 
* Get Model List
* 
* by Common Way:            6781.3879 ms. 
* by DTOHelper Way:         6815.3898 ms. 
* 
* ==============================================================
* 
* Get GetDictionary
* 
* by Common Way:            6789.3884 ms. 
* by DTOHelper Way:         7671.4387 ms. 
*
* ==============================================================
* 
* History:
* * 8/19/2009 13:53 PM Lance Zhang Created
* * 8/24/2009 21:29 PM Lance Zhang Add Dictionary<K, T> GetDictionary<K, T>
* * 9/09/2009 21:29 PM Lance Zhang Add T New<T>()
* * X/XX/2009 XX:XX PM XXXX        Reviewed
\***************************************************************************/


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace TaoBaoDataServer.WinClientData
{
    public static class DTOHelper
    {
        #region Private Members

        private static Dictionary<Type, FastCreateInstanceHandler> TypeCache = new Dictionary<Type, FastCreateInstanceHandler>();
        private static Dictionary<PropertyInfo, FastPropertySetHandler> PropCache = new Dictionary<PropertyInfo, FastPropertySetHandler>();
        private static readonly object syncRoot = new object();

        #endregion


        /// <summary>
        /// 获取基元类型(int,string,double, float,byte  and so on)数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> CreatePrimitiveTypeModelList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj;
            while (dr.Read())
            {
                obj = (T)dr[0];
                list.Add(obj);
            }

            return list;
        }

        ///// <summary>
        ///// Create entity(model) object list by IDataReader.
        ///// </summary>
        ///// <typeparam name="T">Type of Entity(Model)</typeparam>
        ///// <param name="dr">Related datereader</param>
        ///// <remarks>Please make sure the Fields Name of Entity(Model)
        ///// and input daterader are same.</remarks>
        ///// <returns>Object list of Entity(Model)</returns>
        //public static List<T> CreateModelList<T>(IDataReader dr)
        //{
        //    List<T> list = new List<T>();
        //    T obj;

        //    if (!TypeCache.Keys.Contains(typeof(T)))
        //    {
        //        lock (syncRoot)
        //        {
        //            if (!TypeCache.Keys.Contains(typeof(T)))
        //            {
        //                TypeCache.Add(typeof(T), EmitHelper.GetInstanceCreator(typeof(T)));
        //            }
        //        }
        //    }

        //    DataTable dcc = dr.GetSchemaTable();

        //    while (dr.Read())
        //    {
        //        obj = (T)TypeCache[typeof(T)].Invoke();

        //        foreach (PropertyInfo p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        //        {
        //            if (!HasColumn(dcc, p.Name))
        //                continue;

        //            if (!PropCache.Keys.Contains(p))
        //            {
        //                lock (syncRoot)
        //                {
        //                    if (!PropCache.Keys.Contains(p))
        //                    {
        //                        PropCache.Add(p, EmitHelper.GetPropertySetter(p));
        //                    }
        //                }
        //            }

        //            if (p.PropertyType.IsEnum)
        //            {
        //                PropCache[p](obj, ChangeType(dr[p.Name], typeof(int)));
        //            }
        //            else
        //                PropCache[p](obj, ChangeType(dr[p.Name], p.PropertyType));
        //        }

        //        list.Add(obj);

        //    }

        //    return list;
        //}
        /// <summary>
        /// Create entity(model) object list by IDataReader. 已优化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> CreateModelList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj;

            Type type = typeof(T);

            //if (!DTOTypeCache.ContainKeys(type))
            //{
            //    lock (syncRoot)
            //    {
            //        if (!DTOTypeCache.ContainKeys(type))
            //        {
            //            DTOTypeCache.Add(type, EmitHelper.GetInstanceCreator(type));
            //        }
            //    }
            //}
            if (!DTOTypeCache.ContainKeys(type))
            {
                lock (syncRoot)
                {
                    if (!DTOTypeCache.ContainKeys(type))
                    {
                        DTOTypeCache.Add(type, EmitHelper.GetInstanceCreator(type));
                    }
                }
            }
            //数据库得到的列名
            var columns = GetColumns(dr);
            var dtoType = DTOTypeCache.GetDTOType(type);
            bool loadProperty = false;
            bool loadFinished = false;
            IEnumerable<PropertyInfo> propertyInfos = null;
            while (dr.Read())
            {
                if (propertyInfos == null || loadProperty)
                {
                    loadProperty = false;
                    propertyInfos = dtoType.Properties.Count > 0 && dtoType.Properties.Count >= columns.Count ? dtoType.Properties.Select(o => o.Value.Info).ToArray() : type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    //  propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                }

                obj = (T)dtoType.Handler.Invoke();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (!loadFinished && !dtoType.Properties.ContainsKey(p.Name))
                    {
                        lock (syncRoot)
                        {
                            if (p.CanWrite && !dtoType.Properties.ContainsKey(p.Name))
                            {
                                dtoType.Properties.Add(p.Name, new DTOProperty(p, EmitHelper.GetPropertySetter(p)));
                                loadProperty = true;
                            }
                        }
                    }
                    if (!HasColumn(columns, p.Name) || !p.CanWrite)
                        continue; //属性不在返回的列中

                    var changedValue = ChangeType(dr[p.Name], p.PropertyType);
                    var py = dtoType.Properties[p.Name];
                    py.Handler(obj, changedValue);
                }

                list.Add(obj);
                loadFinished = true;
            }

            return list;
        }


        private static SortedList<string, string> GetColumns(IDataReader dr)
        {
            var columns = new SortedList<string, string>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                var name = dr.GetName(i);
                var key = name.ToUpper();

                if (columns.ContainsKey(key))
                    continue;

                columns.Add(key, name);
            }
            return columns;
        }

        private static bool HasColumn(SortedList<string, string> columns, string columnName)
        {
            return columns.ContainsKey(columnName.ToUpper());
        }
        //public static T CreateModel2<T>(IDataReader dr)
        //{

        //    T obj = default(T);

        //    if (!TypeCache.Keys.Contains(typeof(T)))
        //    {
        //        lock (syncRoot)
        //        {
        //            if (!TypeCache.Keys.Contains(typeof(T)))
        //            {
        //                TypeCache.Add(typeof(T), EmitHelper.GetInstanceCreator(typeof(T)));
        //            }
        //        }
        //    }

        //    DataTable dcc = dr.GetSchemaTable();

        //    if (dr.Read())
        //    {
        //        obj = (T)TypeCache[typeof(T)].Invoke();

        //        foreach (PropertyInfo p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        //        {
        //            if (!HasColumn(dcc, p.Name))
        //                continue;

        //            if (!PropCache.Keys.Contains(p))
        //            {
        //                lock (syncRoot)
        //                {
        //                    if (!PropCache.Keys.Contains(p))
        //                    {
        //                        PropCache.Add(p, EmitHelper.GetPropertySetter(p));
        //                    }
        //                }
        //            }

        //            if (p.PropertyType.IsEnum)
        //            {
        //                PropCache[p](obj, ChangeType(dr[p.Name], typeof(int)));
        //            }
        //            else
        //                PropCache[p](obj, ChangeType(dr[p.Name], p.PropertyType));
        //        }


        //    }

        //    return obj;
        //}

        public static T CreateModel2<T>(IDataReader dr)
        {
            return CreateModel<T>(dr);
        }

        public static T CreateModel<T>(IDataReader dr)
        {
            return CreateModel<T>(dr, false);
        }

        ///// <summary>
        ///// Create entity(model) object by IDataReader.
        ///// </summary>
        ///// <typeparam name="T">Type of Entity(Model)</typeparam>
        ///// <param name="dr">Related datereader</param>
        ///// <remarks>Please make sure the Fields Name of Entity(Model)
        ///// and input daterader are same.</remarks>
        ///// <returns>Object of Entity(Model)</returns>
        //public static T CreateModel<T>(IDataReader dr, bool checkColumn)
        //{
        //    if (dr.Read())
        //    {

        //        if (!TypeCache.Keys.Contains(typeof(T)))
        //        {
        //            lock (syncRoot)
        //            {
        //                if (!TypeCache.Keys.Contains(typeof(T)))
        //                {
        //                    TypeCache.Add(typeof(T), EmitHelper.GetInstanceCreator(typeof(T)));
        //                }
        //            }
        //        }

        //        T obj = (T)TypeCache[typeof(T)].Invoke();

        //        foreach (PropertyInfo p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        //        {
        //            if (!PropCache.Keys.Contains(p))
        //            {
        //                lock (syncRoot)
        //                {
        //                    if (!PropCache.Keys.Contains(p) && p.CanWrite)
        //                    {
        //                        PropCache.Add(p, EmitHelper.GetPropertySetter(p));
        //                    }
        //                }
        //            }
        //            if (readerExists(dr, p.Name))
        //            {
        //                PropCache[p](obj, ChangeType(dr[p.Name], p.PropertyType));
        //            }
        //        }

        //        return obj;

        //    }
        //    else
        //    {
        //        return default(T);
        //    }
        //}
        /// <summary>
        /// Create entity(model) object by IDataReader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="checkColumn"></param>
        /// <returns></returns>
        public static T CreateModel<T>(IDataReader dr, bool checkColumn)
        {
            Type type = typeof(T);

            if (!DTOTypeCache.ContainKeys(type))
            {
                lock (syncRoot)
                {
                    if (!DTOTypeCache.ContainKeys(type))
                    {
                        DTOTypeCache.Add(type, EmitHelper.GetInstanceCreator(type));
                    }
                }

            }

            var columns = GetColumns(dr);
            var dtoType = DTOTypeCache.GetDTOType(type);
            if (dr.Read())
            {
                var propertyInfos = dtoType.Properties.Count > 0 && dtoType.Properties.Count >= columns.Count ? dtoType.Properties.Select(o => o.Value.Info).ToArray() : type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                T obj = (T)dtoType.Handler.Invoke();

                foreach (PropertyInfo p in propertyInfos)
                {
                    if (!HasColumn(columns, p.Name))
                        continue;

                    if (p.CanWrite && !dtoType.Properties.ContainsKey(p.Name))
                    {
                        lock (syncRoot)
                        {
                            if (p.CanWrite && !dtoType.Properties.ContainsKey(p.Name))
                            {
                                dtoType.Properties.Add(p.Name, new DTOProperty(p, EmitHelper.GetPropertySetter(p)));
                            }
                        }
                    }
                    if (!p.CanWrite)
                        continue;

                    dtoType.Properties[p.Name].Handler(obj, ChangeType(dr[p.Name], p.PropertyType));
                }

                return obj;

            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Create entity(model) object Dictionary by IDataReader and given key name.
        /// </summary>
        /// <typeparam name="K">Type of Key field</typeparam>
        /// <typeparam name="T">Type of Entity(Model)</typeparam>
        /// <param name="keyName">name of key property</param>
        /// <param name="dr">Related datereader</param>
        /// <returns>Dictionary of models</returns>
        public static Dictionary<K, T> GetDictionary<K, T>(string keyName, IDataReader dr)
        {
            Dictionary<K, T> dic = new Dictionary<K, T>();

            T obj;

            if (!TypeCache.Keys.Contains(typeof(T)))
            {
                lock (syncRoot)
                {
                    if (!TypeCache.Keys.Contains(typeof(T)))
                    {
                        TypeCache.Add(typeof(T), EmitHelper.GetInstanceCreator(typeof(T)));
                    }
                }
            }

            object key = string.Empty;

            while (dr.Read())
            {
                obj = (T)TypeCache[typeof(T)].Invoke();

                foreach (PropertyInfo p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!PropCache.Keys.Contains(p))
                    {
                        lock (syncRoot)
                        {
                            if (!PropCache.Keys.Contains(p))
                            {
                                PropCache.Add(p, EmitHelper.GetPropertySetter(p));
                            }
                        }
                    }

                    PropCache[p](obj, ChangeType(dr[p.Name], p.PropertyType));

                    if (p.Name == keyName)
                    {
                        key = EmitHelper.GetPropertyGetter(p).Invoke(obj);
                    }
                }

                dic.Add((K)key, obj);

            }

            return dic;
        }

        /// <summary>
        /// Create an instance of Generic type.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <returns>an new instance</returns>
        public static T New<T>()
        {
            if (!TypeCache.Keys.Contains(typeof(T)))
            {
                lock (syncRoot)
                {
                    if (!TypeCache.Keys.Contains(typeof(T)))
                    {
                        TypeCache.Add(typeof(T), EmitHelper.GetInstanceCreator(typeof(T)));
                    }
                }
            }

            T obj = (T)TypeCache[typeof(T)].Invoke();
            return obj;
        }

        private static bool HasColumn(DataTable dt, string columnName)
        {
            dt.DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            bool has = dt.DefaultView.Count > 0;
            dt.DefaultView.RowFilter = string.Empty;
            return has;
        }

        /// <summary>
        /// Returns an Object with the specified Type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="value">An Object that implements the IConvertible interface.</param>
        /// <param name="conversionType">The Type to which value is to be converted.</param>
        /// <returns>An object whose Type is conversionType (or conversionType's underlying type if conversionType
        /// is Nullable&lt;&gt;) and whose value is equivalent to value. -or- a null reference, if value is a null
        /// reference and conversionType is not a value type.</returns>
        /// <remarks>
        /// This method exists as a workaround to System.Convert.ChangeType(Object, Type) which does not handle
        /// nullables as of version 2.0 (2.0.50727.42) of the .NET Framework. The idea is that this method will
        /// be deleted once Convert.ChangeType is updated in a future version of the .NET Framework to handle
        /// nullable types, so we want this to behave as closely to Convert.ChangeType as possible.
        /// This method was written by Peter Johnson at:
        /// http://aspalliance.com/author.aspx?uId=1026.
        /// </remarks>
        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }

            //if (conversionType.IsGenericType &&
            //  conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            //{
            //    if (value == null || value is DBNull)
            //    {
            //        return null;
            //    }
            //    conversionType.UnderlyingSystemType
            //    System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
            //    conversionType = nullableConverter.UnderlyingType;
            //}
            if (conversionType.IsGenericType)
            {
                var nullableRealType = Nullable.GetUnderlyingType(conversionType);
                if (nullableRealType != null)
                {
                    if (value == null || value is DBNull)
                        return null;

                    conversionType = nullableRealType;
                }
            }

            if (value == null || value is DBNull)
            {
                if (conversionType == typeof(bool))
                {
                    return false;
                }
                if (conversionType == typeof(DateTime))
                {
                    return DateTime.MinValue;
                }
                else if (conversionType == typeof(int) || conversionType.IsEnum || conversionType == typeof(Int64) || conversionType == typeof(byte))
                {
                    return 0;
                }
                else if (conversionType == typeof(Guid))
                {
                    return Guid.Empty;
                }
                else if (conversionType == typeof(double))
                {
                    return 0d;
                }
                else if (conversionType == typeof(decimal))
                {
                    return 0M;
                }
                else if (conversionType == typeof(float))
                {
                    return 0f;
                }
            }

            object o = null;
            try
            {
                if (conversionType.IsEnum)
                {
                    //  return Enum.ToObject(conversionType, (int)value);
                    return Convert.ChangeType(value, typeof(int));
                }

                o = Convert.ChangeType(value, conversionType);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return o;
        }

        /// <summary>
        /// Check if datareader has the given column.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static bool readerExists(IDataReader dr, string columnName)
        {
            dr.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            return (dr.GetSchemaTable().DefaultView.Count > 0);
        }
    }
}
