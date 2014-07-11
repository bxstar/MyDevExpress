using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using log4net;
using System.Text.RegularExpressions;
using TaoBaoDataServer.WinClientData.Model;
using TaoBaoDataServer.WinClientData.BusinessLayer;
using WeifenLuo.WinFormsUI.Docking;

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmWord : DockContent
    {
        private static log4net.ILog logAX = LogManager.GetLogger("loggerAX");
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();

        Stopwatch watch = new Stopwatch();

        /// <summary>
        /// 用户登录信息
        /// </summary>
        TopSession session = new TopSession();

        /// <summary>
        /// 加盐，用于天猫分词
        /// </summary>
        string XXXX = "xxxx";

        /// <summary>
        /// 宝贝属性关键词
        /// </summary>
        List<string> lstAttr = new List<string>();

        /// <summary>
        /// 标题分词关键词
        /// </summary>
        List<string> lstTitleSplit = new List<string>();

        /// <summary>
        /// 创意分词关键词
        /// </summary>
        List<string> lstCreativeSplit = new List<string>();

        /// <summary>
        /// 标题，创意去重，类目匹配的关键词
        /// </summary>
        List<string> lstTitleCreative = new List<string>();

        /// <summary>
        /// 组词最终结果
        /// </summary>
        List<string> lstResult = new List<string>();


        public FrmWord()
        {
            InitializeComponent();
        }

        private void FrmWord_Load(object sender, EventArgs e)
        {
            //TraceListener输出方式设置，topsdk.log的生成奇怪
            System.Diagnostics.Debug.Listeners.Add(new MyTraceListenerWithColor(this.rtbResult));
            session.UserName = Config.UserName;
            session.TopSessions = Config.TopSessions;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            lstAttr.Clear();
            lstTitleSplit.Clear();
            lstCreativeSplit.Clear();
            lstTitleCreative.Clear();
            lstResult.Clear();
            rtbResult.Clear();
        }

        private void btnGetWord_Click(object sender, EventArgs e)
        {
            watch.Start();
            init();

            List<string> resultOne= GetWord(TechNet.GetItemId(txtNumID.Text));
            if (resultOne == null) return;
            if (resultOne.Count >= 200)
            {
                watch.Stop();
                OutPutMsg(string.Format("最终结果：总数{0}，耗时{1}", lstResult.Count.ToString(), watch.Elapsed.ToString()), lstResult);
            }
            else
            {
                OutPutMsg(string.Format("第一次结果：总数{0}，耗时{1}", lstResult.Count.ToString(), watch.Elapsed.ToString()), lstResult);
                OutPutMsg("根据自然排名销量第一的宝贝继续找词...", null);
                long numidTwo = GetItemIDByNatrueRankSellFirst(string.Join(" ", lstTitleCreative.ToArray()));
                lstAttr.Clear();
                lstTitleSplit.Clear();
                lstCreativeSplit.Clear();
                lstTitleCreative.Clear();
                GetWord(numidTwo);
                watch.Stop();
                OutPutMsg(string.Format("最终结果：总数{0}，耗时{1}", lstResult.Count.ToString(), watch.Elapsed.ToString()), lstResult);
            }
        }

        /// <summary>
        /// 根据宝贝ID取词
        /// </summary>
        private List<string> GetWord(long num_iid)
        {
            //宝贝
            Top.Api.Domain.Item itemObj = null;
            lstAttr = GetItemAttribute(num_iid, ref itemObj);
            if (itemObj == null)
                return null;
            //过滤属性
            lstAttr = FilterSpeicalAttr(lstAttr);


            OutPutMsg(string.Format("宝贝信息：{0}，{1}", itemObj.Title, itemObj.Cid), null);

            OutPutMsg("属性词", lstAttr);

            if (itemObj.Title.Length > 0)
            {
                string strTitleSplitWord = GetSpliterWordByTB(itemObj.Title);
                string[] arr = strTitleSplitWord.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                lstTitleSplit.AddRange(arr);
            }
            OutPutMsg("标题词", lstTitleSplit);

            if (txtCreative.Text.Length > 0)
            {
                string strCreativeSplitWord = GetSpliterWordByTB(txtCreative.Text);
                string[] arr = strCreativeSplitWord.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                lstCreativeSplit.AddRange(arr);
            }

            OutPutMsg("创意词", lstCreativeSplit);

            //合并标题词和创意词，去重复
            List<string> lstCatsForecast = lstTitleSplit.Union(lstCreativeSplit).ToList();
            var lstMatchCats = keywordHandler.ForecastCategory(session, lstCatsForecast, itemObj.Cid);
            lstTitleCreative = lstMatchCats.Where(o => o.CategoryLevel > 0).OrderBy(o => o.CategoryLevel).Select(o => o.Word).ToList();
            

            OutPutMsg("标题，创意去重，且类目匹配的关键词（核心词）", lstTitleCreative);

            // 循环组合标题创意词和属性词
            foreach (string t in lstTitleCreative)
            {
                foreach (string a in lstAttr.Where(o => o != t))
                {
                    lstResult.Add(t + " " + a);
                }
            }

            // 循环组合标题创意词自己
            foreach (string t1 in lstTitleCreative)
            {
                foreach (string t2 in lstTitleCreative.Where(o => o != t1))
                {
                    lstResult.Add(t1 + " " + t2);
                }
            }

            // 循环组合属性词自己，效果不好
            //foreach (string a1 in lstAttr)
            //{
            //    foreach (string a2 in lstAttr.Where(o => o != a1))
            //    {
            //        lstResult.Add(a1 + " " + a2);
            //    }
            //}

            lstResult.AddRange(lstTitleCreative);
            lstResult.AddRange(lstAttr);

            lstMatchCats = keywordHandler.ForecastCategory(session, lstResult, itemObj.Cid);
            lstResult = lstMatchCats.Where(o => o.CategoryLevel > 0).OrderBy(o => o.CategoryLevel).Select(o => o.Word + "," + o.CategoryLevel).ToList();

            return lstResult;
        }

        /// <summary>
        /// 从宝贝的属性获取关键词
        /// </summary>
        private List<string> GetItemAttribute(long itemId, ref Top.Api.Domain.Item itemObj)
        {
            var list = new List<string>();
            try
            {
                var response = TaobaoApiHandler.TaobaoItemGet(itemId);

                if (response != null && response.Item != null)
                {
                    //获取宝贝的相关信息
                    itemObj = response.Item;
                    //获取宝贝属性
                    string propsName = response.Item.PropsName;
                    if (!String.IsNullOrEmpty(propsName))
                    {
                        var strPropsNames = propsName.Split(';');
                        for (int i = 0; i < strPropsNames.Length; i++)
                        {
                            var attributes = strPropsNames[i].Split(':');
                            list.Add(attributes[attributes.Length - 1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logAX.Error("GetItemAttribute, 从宝贝的属性获取关键词：", ex);
            }
            return list;
        }

        /// <summary>
        /// 获取天猫分词结果
        /// </summary>
        private string GetSpliterWordByTB(string str)
        {
            try
            {
                string html = TechNet.DownLoadString("http://list.tmall.com/search_product.htm?q=" + TechNet.UrlEncode(str + XXXX, System.Text.Encoding.GetEncoding("gbk")) + "&user_action=initiative&at_topsearch=1&sort=st&type=p&cat=&style=", Encoding.GetEncoding("gbk"));
                Match match = Regex.Match(html, "没找到与(.*?)相关的商品");
                if (match.Success)
                {
                    string strTemp = match.Groups[1].Value;
                    str = Regex.Replace(strTemp, "<[^>]*>|“|”", "").ToLower().Replace(XXXX, "");
                }
                else
                {
                    logAX.Info("GetSpliterWordByTB，天猫分词失败，网页发生变化");    
                }
            }
            catch (Exception e1)
            {
                logAX.Error("GetSpliterWordByTB，获取天猫分词结果：", e1);
            }
            return str;
        }

        /// <summary>
        /// 获取自然搜索销量第一的宝贝ID
        /// </summary>
        private long GetItemIDByNatrueRankSellFirst(string word)
        {
            long numid = 1;
            string url = string.Format("http://s.taobao.com/search?q={0}&sort=sale-desc", word);
            string html = TechNet.DownLoadString(url, Encoding.GetEncoding("gbk"));
            Match match = Regex.Match(html, "nid=\"(\\d+)\"");
            if (match.Success)
            {
                string strTemp = match.Groups[1].Value;
                Int64.TryParse(strTemp, out numid);
            }
            else
            {
                logAX.Info("GetItemIDByNatrueRankSellFirst，获取自然搜索销量第一的宝贝ID失败，网页发生变化");
            }

            return numid;
        }

        /// <summary>
        /// 过滤特殊的属性，中文、英文括号外面的内容，如：迷你（最长边20cm）返回 迷你
        /// </summary>
        private List<string> FilterSpeicalAttr(List<string> lstAttr)
        {
            List<string> result = new List<string>();
            foreach (var item in lstAttr)
            {
                string str = Regex.Replace(item, @"\([^\)]*\)|\（[^\)]*\）|是|否|有|无|其他", "");
                if (str.Trim().Length != 0)
                    result.Add(str);
            }
            return result;
        }

        private void OutPutMsg(string strMsg, List<string> lstContent)
        {
            Debug.IndentLevel = 0;
            Debug.WriteLine(strMsg);
            if (lstContent == null || lstContent.Count == 0) return;
            Debug.IndentLevel = 1;
            foreach (var item in lstContent)
            {
                Debug.WriteLine(item);
            }
        }

    }


}
