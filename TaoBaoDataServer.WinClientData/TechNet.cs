using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using NetServ.Net.Json;

namespace TaoBaoDataServer.WinClientData
{
    public class TechNet
    {
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        //参数说明connectionDescription连接说明,reservedValue保留值 
        public static bool IsConnectedToInternet()
        {
            try
            {
                return TechNet.IsConnInternet("www.taobao.com");
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 判断是否能够连接网络
        /// </summary>
        public static bool IsConnInternet()
        {
            //有的时候windows系统问题，导致ping无法使用，可能出异常，直接返回false
            try
            {
                System.Net.NetworkInformation.Ping ping;
                System.Net.NetworkInformation.PingReply res;
                ping = new System.Net.NetworkInformation.Ping();
                res = ping.Send("www.baidu.com");
                if (res.Status != System.Net.NetworkInformation.IPStatus.Success)
                    return false;
                else
                    return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否能够连接网络,可以指定ping哪个
        /// </summary>
        public static bool IsConnInternet(string url)
        {
            url = url.ToLower().Replace("http://", "").Replace("https://", "").Trim();
            try
            {
                System.Net.NetworkInformation.Ping ping;
                System.Net.NetworkInformation.PingReply res;
                ping = new System.Net.NetworkInformation.Ping();
                res = ping.Send(url);
                if (res.Status != System.Net.NetworkInformation.IPStatus.Success)
                    return false;
                else
                    return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }

        /// <summary>
        /// 从当前的WebBrowser中取到cookie,根据传入的url和参数执行当前url
        /// </summary>
        /// <param name="url">要执行的url</param>
        /// <param name="cookie">当前使用的cookie</param>
        /// <param name="postData">需要传入的参数</param>
        /// <returns></returns>
        public static string UploadDataAsync(string url, string cookie, string postData)
        {
            try
            {
                string message = "";
                WebClient wb = new WebClient();
                wb.Encoding = Encoding.UTF8;
                wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                wb.Headers.Add("Cookie", cookie);
                byte[] postBy = Encoding.UTF8.GetBytes(postData);
                wb.Headers.Add("ContentLength", postBy.Length.ToString());
                wb.UploadDataAsync(new Uri(url), "POST", postBy);
                wb.Dispose();
                postData = "";
                cookie = "";
                url = "";
                return message;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取宝贝的id
        /// </summary>
        /// <param name="linkUrl">含有宝贝ID的网址</param>
        public static long GetItemId(string linkUrl)
        {
            string str = string.Empty;
            long itemID = 1;
            if (linkUrl.Contains("id="))
            {
                linkUrl = WebUtility.HtmlDecode(linkUrl);
                str = linkUrl.Replace("?id=", "ÿ").Replace("&id=", "ÿ").Split('ÿ')[1].Split('&')[0];//取？id之后第一个&之前的itemid,或者是&id后面的
            }
            else
            {
                str = linkUrl;
            }
            
            Int64.TryParse(str, out itemID);
            return itemID;
        }

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="str">要进行编码的字符串</param>
        /// <returns>编码后的字符串</returns>
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取数据，需要传递querystring过去,UploadData的方法 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string DownLoadString(string url, string cookie, string postData)
        {
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            //wb.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
            wb.Headers.Add("Accept-Encoding", "gzip, deflate");
            wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wb.Headers.Add("Cookie", cookie);
            byte[] postBy = Encoding.UTF8.GetBytes(postData);
            byte[] responseData = wb.UploadData(url, "POST", postBy);
            wb.Dispose();
            return GZipString(responseData, wb.Encoding, wb.ResponseHeaders["Content-Encoding"]);
        }

        /// <summary>
        /// 获取数据，需要传递querystring过去,采用改写的wenclient
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string UploadDataAsync(string url, string cookie, string postData, ref object currentUrl)
        {
            ClickProWebClient wb = new ClickProWebClient();
            wb.Encoding = Encoding.UTF8;
            wb.Headers.Add("Accept-Encoding", "gzip, deflate");
            wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wb.Headers.Add("Cookie", cookie);
            byte[] postBy = Encoding.UTF8.GetBytes(postData);
            byte[] responseData = wb.UploadData(url, "POST", postBy);
            currentUrl = wb.ResponseUri;
            wb.Dispose();
            return GZipString(responseData, wb.Encoding, wb.ResponseHeaders["Content-Encoding"]);
        }

        /// <summary>
        /// 获取数据，不需要传递条件参数
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string DownLoadString(string url, string cookie)
        {
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            wb.Headers.Add("Accept-Encoding", "gzip, deflate");
            wb.Headers.Add("Cookie", cookie);
            return GZipString(wb.DownloadData(url), wb.Encoding, wb.ResponseHeaders["Content-Encoding"]);
        }

        /// <summary>
        /// 获取数据，不需要传递条件参数
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string DownLoadStringWithOutGzip(string url, string cookie)
        {
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            wb.Headers.Add("Cookie", cookie);
            string result = wb.DownloadString(url);
            wb.Dispose();
            return result;
        }

        /// <summary>
        /// 获取数据，不需要传递条件参数
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string DownLoadString(string url)
        {
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            wb.Headers.Add("Accept-Encoding", "gzip, deflate");
            return GZipString(wb.DownloadData(url), wb.Encoding, wb.ResponseHeaders["Content-Encoding"]);
        }

        /// <summary>
        /// 获取数据，不需要传递条件参数
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string DownLoadString(string url, Encoding encoder)
        {
            WebClient wb = new WebClient();
            wb.Encoding = encoder;
            wb.Headers.Add("Accept-Encoding", "gzip, deflate");
            return GZipString(wb.DownloadData(url), wb.Encoding, wb.ResponseHeaders["Content-Encoding"]);
        }

        /// <summary>
        /// 获取数据，不需要传递条件参数
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string DownLoadString(string url, Encoding encoder, string cookie)
        {
            WebClient wb = new WebClient();
            wb.Encoding = encoder;
            wb.Headers.Add("Cookie", cookie);
            wb.Headers.Add("Accept-Encoding", "gzip, deflate");
            return GZipString(wb.DownloadData(url), wb.Encoding, wb.ResponseHeaders["Content-Encoding"]);
        }


        /// <summary>
        /// 获取数据，不需要传递条件参数
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cookie">cookie</param>
        /// <returns></returns>
        public static string DownLoadStringWithOutGzip(string url)
        {
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            string result = wb.DownloadString(url);
            wb.Dispose();
            return result;
        }

        /// <summary>
        /// 获取网页编码为GBK数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns></returns>
        public static string DownLoadGBKString(string url)
        {
            WebClient wb = new WebClient();
            wb.Headers.Add("Accept-Encoding", "gzip, deflate");
            wb.Encoding = Encoding.GetEncoding("GBK");
            return GZipString(wb.DownloadData(url), wb.Encoding, wb.ResponseHeaders["Content-Encoding"]);
        }

        /// <summary>
        /// 获取网页编码为GBK数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns></returns>
        public static string DownLoadGBKStringWithOutGzip(string url)
        {
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.GetEncoding("GBK");
            string result = wb.DownloadString(url);
            wb.Dispose();
            return result;
        }

        /// <summary>
        /// 使用指定的编码对象对 URL 字符串进行编码，模仿System.Web.HttpUtility的UrlEncode方法
        /// </summary>
        public static string UrlEncode(string str, Encoding e)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = e.GetBytes(str);
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 根据网址下载网页内容
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cookie">cookie</param>
        /// <returns>网页内容</returns>
        public static string DownGBKStringByUrl(string url, string cookie)
        {
            string html = "";
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.GetEncoding("GBK");
            wb.Headers.Add("Cookie", cookie);
            wb.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wb.Headers.Add("Accept-Encoding:gzip,deflate,sdch");
            byte[] responseData = wb.DownloadData(url);
            html = GzipString(responseData, Encoding.GetEncoding("GBK"));
            wb.Dispose();
            return html;
        }

        /// <summary>
        /// 解压gzip
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static byte[] HandleGzipMessage(byte[] data, int start)
        {
            MemoryStream memStream;
            int size = BitConverter.ToInt32(data, data.Length - 4);
            byte[] uncompressedData = new byte[size];
            memStream = new MemoryStream(data, start, (data.Length - start));
            memStream.Position = 0;
            GZipStream gzStream = new GZipStream(memStream, CompressionMode.Decompress);
            try
            {
                gzStream.Read(uncompressedData, 0, size);
            }
            catch (Exception gzError)
            {
                throw gzError;
            }
            gzStream.Close();
            return uncompressedData;
        }

        /// <summary>
        /// 把获取到的数据解析返回
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static string GzipString(byte[] data, Encoding encoder)
        {
            return encoder.GetString(HandleGzipMessage(data, 0));
        }

        public static string GZipString(byte[] byteArray, Encoding encoder, string sContentEncoding)
        {
            try
            {
                if (sContentEncoding == "gzip")
                {
                    // 处理　gzip string sContentEncoding = client.ResponseHeaders["Content-Encoding"]；if （sContentEncoding == "gzip"）
                    MemoryStream ms = new MemoryStream(byteArray);
                    MemoryStream msTemp = new MemoryStream();
                    int count = 0;
                    GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);
                    byte[] buf = new byte[1000];
                    while ((count = gzip.Read(buf, 0, buf.Length)) > 0)
                    { msTemp.Write(buf, 0, count); }
                    byteArray = msTemp.ToArray(); // end-gzip
                    return encoder.GetString(byteArray);
                }
                else
                {
                    return encoder.GetString(byteArray);
                }
            }
            catch
            {
                return encoder.GetString(byteArray);
            }
        }

        /// <summary>
        /// 用默认浏览器打开一个页面
        /// </summary>
        /// <param name="url"></param>
        public static void OpenUrlByDefaultBrowser(string url)
        {
            Process ps = new Process();
            try
            {
                string path = DefaultWebBrowserFilePath();
                if (!string.IsNullOrEmpty(path))
                {
                    ps.StartInfo.FileName = path;
                }
                else
                {
                    ps.StartInfo.FileName = "iexplore.exe";
                }
            }
            catch (Exception e1)
            {
                ps.StartInfo.FileName = "iexplore.exe";
            }
            ps.StartInfo.Arguments = WebUtility.HtmlDecode(url);
            ps.Start();
        }

        /// <summary>
        /// 用IE打开一个页面
        /// </summary>
        /// <param name="url"></param>
        public static void OpenUrlByIE(string url)
        {
            Process ps = new Process();
            try
            {
                ps.StartInfo.FileName = "iexplore.exe";
            }
            catch (Exception e1)
            {
            }
            ps.StartInfo.Arguments = WebUtility.HtmlDecode(url);
            ps.Start();
        }

        /// <summary>
        /// 获取默认浏览器的路径
        /// </summary>
        /// <returns></returns>
        static String DefaultWebBrowserFilePath()
        {
            string path = string.Empty;
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("http\\shell\\open\\command", false);
                path = key.GetValue("").ToString();
                if (path.Contains("\""))
                {
                    path = path.TrimStart('"');
                    path = path.Substring(0, path.IndexOf('"'));
                }
                key.Close();
            }
            catch (Exception e1)
            {

            }
            return path;
        }

        /// <summary>
        /// 捕获图片
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Image GetImgOnLine(string src)
        {
            ClickProWebClient wbc = new ClickProWebClient();
            Image img = null;
            byte[] content = wbc.DownloadData(src);
            MemoryStream stream = new MemoryStream(content);
            img = Image.FromStream(stream);
            wbc.Dispose();
            return img;
        }

        /// <summary>
        /// 空余时段占用内存的控制
        /// </summary>
        public static void SetWorkingSet(int maxWorkingSet)
        {
            try
            {//防止调用系统API出错
                IntPtr tmpSet = System.Diagnostics.Process.GetCurrentProcess().MinWorkingSet;
                if (maxWorkingSet < (int)tmpSet)
                {
                    maxWorkingSet = (int)tmpSet + 1000;
                }
                System.Diagnostics.Process.GetCurrentProcess().MaxWorkingSet = (IntPtr)maxWorkingSet;
            }
            catch { }
        }

        /// <summary>
        /// 把cookie清除全部赋空就可以
        /// </summary>
        public void SetEmptyCookie()
        {
            try
            {
                string UserProfile = Environment.GetEnvironmentVariable("USERPROFILE");
                string XPCookiesPath = @"\Cookies\";

                string VistaCookiesPath = @"\AppData\Roaming\Microsoft\Windows\Cookies\";
                int OsType = Environment.OSVersion.Version.Major;
                string path = string.Empty;
                if (OsType >= 6)
                {//Vista及以上版本 
                    path = UserProfile + VistaCookiesPath;
                }
                else
                {
                    path = UserProfile + XPCookiesPath;
                }
                string Dstr = "*.*";
                string ExceptStr = "index.dat";

                //删除cookie文件夹
                try
                {
                    foreach (string dFileName in Directory.GetFiles(path, Dstr))
                    {
                        if (dFileName == path + ExceptStr)
                            continue;
                        DeleteCookieFile(dFileName, "taobao.com");
                    }
                }
                catch
                {
                }

                //深层遍历（解决Vista Low权限问题）
                string[] LowPath = Directory.GetDirectories(path);
                foreach (string ThePath in LowPath)
                {
                    try
                    {
                        foreach (string dFileName in Directory.GetFiles(ThePath, Dstr))
                        {
                            if (dFileName == path + ExceptStr)
                                continue;
                            DeleteCookieFile(dFileName, "taobao.com");
                        }
                    }
                    catch
                    {
                    }
                }
                //调用系统API设置一个新的cookie目录
                UsingWithSystemAPI.InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0);
            }
            catch { }
        }

        void DeleteCookieFile(string path, string ide)
        {
            try
            {
                StreamReader reader = new StreamReader(path);
                string strcookie = reader.ReadToEnd();
                reader.Close();
                if (strcookie.Contains(ide) && strcookie.Contains("cookie2"))
                {
                    string[] value = strcookie.Split('*');
                    StringBuilder sb = new StringBuilder();
                    string tmp = string.Empty;
                    foreach (string s in value)
                    {
                        tmp = s.ToLower();
                        if (!tmp.Contains(ide) && s != "\n" && !tmp.Contains("cookie2"))
                        {
                            sb.Append(s + "*");
                        }
                    }
                    if (sb.ToString() != "")
                    {
                        Stream stream = new FileStream(path, FileMode.OpenOrCreate);
                        StreamWriter writer = new StreamWriter(stream);
                        writer.Write(sb.ToString());
                        writer.Close();
                        stream.Close();
                    }
                    else
                    {
                        File.Delete(path);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 构建一个webclient能够接受的cookie
        /// </summary>
        /// <param name="cookieIn"></param>
        /// <returns></returns>
        public static string BuildUserfulCookie(string cookieIn)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string[] strs = cookieIn.Split(';');
                WebClient wb = new WebClient();
                for (int i = 0; i < strs.Length; i++)
                {
                    try
                    {
                        wb.Headers.Add("Cookie", strs[i] + ";");
                        sb.Append(strs[i] + ";");
                    }
                    catch { }
                }
                wb.Dispose();
                return sb.ToString();
            }
            catch
            {
                return cookieIn;
            }
        }

        /// <summary>
        /// 打开带有cookie的页面
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        public static void OpenUrlWithMyCookie(string url, string cookie)
        {
            foreach (string c in cookie.ToString().Split(';'))
            {
                string name = string.Empty;
                string value = string.Empty;
                string[] item = c.Split('=');
                if (item.Length == 0)
                { }
                else if (item.Length == 1)
                {
                    name = item[0];
                }
                else
                {
                    name = item[0];
                    value = item[1] + ";expires=Sun,22-Feb-2099 00:00:00 GMT";
                }
                UsingWithSystemAPI.InternetSetCookie(url, name, value);
            }
            TechNet.OpenUrlByIE(url);
        }

        /// <summary>
        /// 解析淘宝的json字符串，返回可以解析的json
        /// </summary>
        public static string AnalysisTaoBaoJson(string json)
        {
            json = json.Substring(json.IndexOf('(') + 1);
            json = json.Substring(0, json.Length - 2);
            return json;
        }

        /// <summary>
        /// 转成整型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int JsonObjectToInt(IJsonType obj)
        {
            if (obj == null || obj is JsonNull || obj.ToString() == "") return 0;

            return Convert.ToInt32(obj.ToString().Split('.')[0]);
        }

        /// <summary>
        /// 转成Decimal
        /// </summary>
        public static Decimal JsonObjectToDecimal(IJsonType obj)
        {
            if (obj == null || obj is JsonNull || obj.ToString() == "") return 0;

            return Convert.ToDecimal(obj.ToString());
        }

        /// <summary>
        /// 转成字符串
        /// </summary>
        public static string JsonObjectToString(IJsonType obj)
        {
            if (obj == null || obj is JsonNull) return string.Empty;

            return obj.ToString();
        }

        /// <summary>
        /// 将对象列表分组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="lst">需要分组的对象列表</param>
        /// <param name="count">多少一组</param>
        /// <returns>分好组对象列表</returns>
        public static List<List<T>> SplitLst<T>(List<T> lst, int count)
        {
            List<List<T>> groupLstWord = new List<List<T>>();
            List<T> lstWord = new List<T>();
            for (int i = 0; i < lst.Count; i++)
            {
                if (i % count == 0)
                {
                    if (lstWord.Count != 0)
                    {
                        groupLstWord.Add(lstWord);
                    }
                    lstWord = new List<T>();
                    lstWord.Add(lst[i]);
                }
                else
                {
                    lstWord.Add(lst[i]);
                }
            }
            if (lst.Count != 0)
            {
                groupLstWord.Add(lstWord);
            }
            return groupLstWord;
        }

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        public static bool IsShuZhi(string source)
        {
            if (source != null)
            {
                return Regex.IsMatch(source, @"^[0-9.]+$");
            }
            return false;
        }

        /// <summary>
        /// MD5 加密算法
        /// </summary>
        public static string Md5Encrypt(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            UTF8Encoding en = new UTF8Encoding();
            byte[] md5Bt = en.GetBytes(s);
            byte[] cryBt = md5.ComputeHash(md5Bt);
            return BitConverter.ToString(cryBt).Replace("-", "");
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }


        /// <summary>
        /// 序列化对象为json
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">对象</param>
        /// <returns></returns>
        public string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }

    }

    public partial class ClickProWebClient : WebClient
    {

        Uri _responseUri;

        public Uri ResponseUri
        {
            get { return _responseUri; }
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            _responseUri = response.ResponseUri;
            return response;
        }

        /// <summary>
        /// 重写GetWebRequest,添加WebRequest对象超时时间
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {//设置超时15秒
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = 1000 * 15;
            request.ReadWriteTimeout = 1000 * 15;
            return request;
        }
    }
}
