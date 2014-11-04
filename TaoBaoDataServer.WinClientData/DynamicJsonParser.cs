using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Collections;
using System.Web.Script.Serialization;

namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// DynamicJson转换
    /// </summary>
    public class DynamicJsonParser
    {
        /// <summary>
        /// 从json字符串到对象。
        /// </summary>
        public dynamic FromJson(string jsonStr)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

            dynamic glossaryEntry = jss.Deserialize(jsonStr, typeof(object)) as dynamic;
            return glossaryEntry;
        }


        /// <summary>
        /// 从对象到json字符串
        /// </summary>
        public static string FromObject(object o)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 99999999;
            String json = serializer.Serialize(o);
            return json;
        }

        /// <summary>
        /// 从json字符串到对象
        /// </summary>
        public static T ToObject<T>(string str)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.MaxJsonLength = 99999999;
            return js.Deserialize<T>(str);
        }
    }

    public class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(object))
            {
                return new DynamicJsonObject(dictionary);
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(object) })); }
        }
    }

    public class DynamicJsonObject : DynamicObject
    {
        private IDictionary<string, object> Dictionary { get; set; }

        public DynamicJsonObject(IDictionary<string, object> dictionary)
        {
            this.Dictionary = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Dictionary[binder.Name];

            if (result is IDictionary<string, object>)
            {
                result = new DynamicJsonObject(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<DynamicJsonObject>((result as ArrayList).ToArray().Select(x => new DynamicJsonObject(x as IDictionary<string, object>)));
            }
            else if (result is ArrayList)
            {
                result = new List<object>((result as ArrayList).ToArray());
            }

            return this.Dictionary.ContainsKey(binder.Name);
        }

        public IDictionary<string, object> GetDictionary()
        {
            return this.Dictionary;
        }
    }
}
