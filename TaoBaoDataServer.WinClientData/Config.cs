using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;

namespace TaoBaoDataServer.WinClientData
{
    public class Config
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public static string App_Title
        {
            get { return GetCfgValue("App_Title"); }
            set { SetCfgValue("App_Title", value); }
        }

        /// <summary>
        /// 淘快词智能版，业务帐户连接字符串
        /// </summary>
        public static string ConnectionZhiNeng
        {
            get { return GetCfgValue("ConnectionZhiNeng"); }
            set { SetCfgValue("ConnectionZhiNeng", value); }
        }

        /// <summary>
        /// 安心代驾，业务帐户连接字符串
        /// </summary>
        public static string ConnectionAP
        {
            get { return GetCfgValue("ConnectionAP"); }
            set { SetCfgValue("ConnectionAP", value); }
        }

        /// <summary>
        /// 订购代码
        /// </summary>
        public static string ArticleCode
        {
            get { return GetCfgValue("ArticleCode"); }
            set { SetCfgValue("ArticleCode", value); }
        }

        /// <summary>
        /// 托管版收费代码
        /// </summary>
        public static string ItemCode
        {
            get { return GetCfgValue("ItemCode"); }
            set { SetCfgValue("ItemCode", value); }
        }

        /// <summary>
        /// 优化程序全路径
        /// </summary>
        public static string MajorizationPath
        {
            get { return GetCfgValue("MajorizationPath"); }
            set { SetCfgValue("MajorizationPath", value); }
        }

        /// <summary>
        /// 词库表的表名
        /// </summary>
        public static string KeywordBankTableName
        {
            get { return GetCfgValue("KeywordBankTableName"); }
            set { SetCfgValue("KeywordBankTableName", value); }
        }

        /// <summary>
        /// 执行Sql超时时间
        /// </summary>
        public static string TimeOut
        {
            get { return GetCfgValue("TimeOut"); }
            set { SetCfgValue("TimeOut", value); }
        }

        public static string AppKey
        {
            get { return GetCfgValue("AppKey"); }
            set { SetCfgValue("AppKey", value); }
        }

        public static string AppSecret
        {
            get { return GetCfgValue("AppSecret"); }
            set { SetCfgValue("AppSecret", value); }
        }

        public static string C_Url
        {
            get { return GetCfgValue("C_Url"); }
            set { SetCfgValue("C_Url", value); }            
        }

        public static string UserName
        {
            get { return GetCfgValue("UserName"); }
            set { SetCfgValue("UserName", value); }          
        }

        public static string TopSessions
        {
            get { return GetCfgValue("TopSessions"); }
            set { SetCfgValue("TopSessions", value); }        
        }

        public static string GetCfgValue(string AppKey)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");

                XmlNode xNode;
                XmlElement xElemKey;
                //XmlElement xElemValue;

                xNode = xDoc.SelectSingleNode("//appSettings");

                xElemKey = (XmlElement)xNode.SelectSingleNode("//add[@key=\"" + AppKey + "\"]");
                if (xElemKey != null)
                    return xElemKey.GetAttribute("value");
                else
                    return string.Empty;
                //下面的语句读取的是ZjjcReport.vshost.exe.config中的配置值
                //它读的是程序运行开始的config后面的修改是不会体现在其中的，所以必须读取ZjjcReport.exe.config
                //return System.Configuration.ConfigurationSettings.AppSettings.Get(AppKey);
            }
            catch (XmlException err)
            {
                throw err;
            }
        }

        //写，引入 System.XML
        public static void SetCfgValue(string AppKey, string AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(System.Windows.Forms.Application.ExecutablePath + ".config");

            XmlNode xNode;
            XmlElement xElemKey;
            XmlElement xElemValue;

            xNode = xDoc.SelectSingleNode("//appSettings");

            xElemKey = (XmlElement)xNode.SelectSingleNode("//add[@key=\"" + AppKey + "\"]");
            if (xElemKey != null) xElemKey.SetAttribute("value", AppValue);
            else
            {
                xElemValue = xDoc.CreateElement("add");
                xElemValue.SetAttribute("key", AppKey);
                xElemValue.SetAttribute("value", AppValue);
                xNode.AppendChild(xElemValue);
            }
            xDoc.Save(System.Windows.Forms.Application.ExecutablePath + ".config");
        }
    }
}
