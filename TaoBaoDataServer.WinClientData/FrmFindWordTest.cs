using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using NetServ.Net.Json;
using TaoBaoDataServer.WinClientData.Model;

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmFindWordTest : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public FrmFindWordTest()
        {
            InitializeComponent();
        }

        private void FrmFindWord_Load(object sender, EventArgs e)
        {
            string strCut = string.Empty;
            string word = "欧莱雅 雪颜 美白 保湿 补 水 淡斑 护肤 套装 专柜正品 化妆品 豪华 礼盒装";

            IEnumerable<char> str = word.Reverse();             //后面的关键词更重要，所以从后往前截取

            int lastEmptyIndex = 0;                             //最后一次空格所在位置
            int charIndexWithoutEmpty = 0;                      //不包括空格的字符索引号
            int charIndex = 0;                                  //包括空格的字符索引号
            foreach (char c in str)
            {
                charIndex++;
                if (charIndexWithoutEmpty == 19)
                {//找到20个字符，不是空格
                    break;
                }
                if (c == ' ')
                {
                    lastEmptyIndex = charIndex;
                }
                else
                {
                    charIndexWithoutEmpty++;
                }
            }
            if (word[word.Length - charIndex] == ' ')
            {//为空，则刚好可以截取
                strCut = word.Substring(word.Length - charIndex);
            }
            else
            {//不为空，看后面有没有空格，没空格说明不能拆分，有空格所以刚好可以截取
                if (word[word.Length - charIndex - 1] == ' ')
                {
                    strCut = word.Substring(word.Length - charIndex);
                }
                else
                {
                    strCut = word.Substring(word.Length - lastEmptyIndex);
                }
            }

        }

        private void btnFindWord_Click(object sender, EventArgs e)
        {
            txtResult.Clear();
            List<string> lstFindedWord = GetTaobaoWantSearchList(txtWord.Text.Trim());
            string strResult = string.Join("\n", lstFindedWord.ToArray());
            txtResult.Text = strResult;
        }


        private void btnSearchList_Click(object sender, EventArgs e)
        {
            List<string> lstFindedWord = GetTaobaoSearchList(txtWord.Text.Trim());
            string strResult = string.Join("\n", lstFindedWord.ToArray());
            txtResult.Text = strResult;
        }

        /// <summary>
        /// 抓取淘宝下拉框的值
        /// </summary>
        /// <param name="keyword">种子关键词</param>
        /// <returns></returns>
        private List<string> GetTaobaoSearchList(string keyword)
        {
            string strJson = TechNet.DownLoadString("http://suggest.taobao.com/sug?code=utf-8&extras=1&callback=jsoncallback&q=" + keyword, Encoding.UTF8);
            List<string> listResult = TechNet.GetKeywordFromTaoBaoJson(strJson);
            return listResult;
        }

        /// <summary>
        /// 你是不是想找的词
        /// </summary>
        /// <param name="keyword">种子关键词</param>
        /// <returns></returns>
        private List<string> GetTaobaoWantSearchList(string keyword)
        {

            var listResult = new List<string>();

            string html = TechNet.DownLoadString("http://s.taobao.com/search?q=" + keyword, Encoding.GetEncoding("GBK"));

            MatchCollection match = Regex.Matches(html, @"<li><a.*trace=""relatedSearch"".*?>(.*?)<.*></li>", RegexOptions.IgnoreCase);
            foreach (Match m in match)
            {
                if (m.Success)
                {
                    listResult.Add(m.Groups[1].Value);
                }
            }

            return listResult;
        }

        /// <summary>
        /// 获取天猫分词结果，结果分两部分，一部分是分词，一部分是找到的结果词
        /// </summary>
        private string GetSpliterWordByTB(string str)
        {
            string splitWordHasEmpty = string.Empty;
            string html = TechNet.DownLoadString("http://list.tmall.com/search_product.htm?q=" + TechNet.UrlEncode(str + "xxxx", System.Text.Encoding.GetEncoding("gbk")) + "&user_action=initiative&at_topsearch=1&sort=st&type=p&cat=&style=", Encoding.GetEncoding("gbk"));
            Match match = Regex.Match(html, "没找到与(.*?)相关的商品");
            if (match.Success)
            {
                string strTemp = match.Groups[1].Value;
                splitWordHasEmpty = Regex.Replace(strTemp, "<[^>]*>|“|”", "").ToLower().Replace("xxxx", "");
            }

            return splitWordHasEmpty;
        }

        private void btnGetCreative_Click(object sender, EventArgs e)
        {
            string splitWord = string.Empty;        //分词20个不含空格
            string resultWord = string.Empty;       //找到的结果词
            string splitWordHasEmpty = string.Empty;

            GetCreativeByTB(txtWord.Text, ref splitWordHasEmpty, ref splitWord,ref resultWord);
            textBox1.Text = splitWordHasEmpty;
            txtCreativeOne.Text = splitWord;
            txtCreativeTwo.Text = resultWord;
        }

        /// <summary>
        /// 获取天猫分词结果用于创意，结果分两部分，一部分是分词，一部分是找到的结果词
        /// </summary>
        private void GetCreativeByTB(string str, ref string splitWordHasEmpty, ref string splitWord,ref string resultWord)
        {
            string html = TechNet.DownLoadString("http://list.tmall.com/search_product.htm?q=" + TechNet.UrlEncode(str + "xxxx", System.Text.Encoding.GetEncoding("gbk")) + "&user_action=initiative&at_topsearch=1&sort=st&type=p&cat=&style=", Encoding.GetEncoding("gbk"));
            Match matchOne = Regex.Match(html, "没找到与(.*?)相关的商品");
            if (matchOne.Success)
            {
                string strTemp = matchOne.Groups[1].Value;
                splitWordHasEmpty = Regex.Replace(strTemp, "<[^>]*>|“|”", "").ToLower().Replace("xxxx", "");
            }
            else
            {
                MessageBox.Show("天猫分词页面有变化");
            }
            if (splitWordHasEmpty.Replace(" ", "").Length <= 20)
            {//不超过20个字符直接返回
                splitWord = splitWordHasEmpty.Replace(" ", "");
            }
            else
            {//超过20个字符 
                string strCut = string.Empty;
                IEnumerable<char> charHasEmpty = splitWordHasEmpty.Reverse();             //后面的关键词更重要，所以从后往前截取

                int lastEmptyIndex = 0;                             //最后一次空格所在位置
                int charIndexWithoutEmpty = 0;                      //不包括空格的字符索引号
                int charIndex = 0;                                  //包括空格的字符索引号
                foreach (char c in charHasEmpty)
                {
                    charIndex++;
                    if (charIndexWithoutEmpty == 19)
                    {//找到20个字符，不是空格
                        break;
                    }
                    if (c == ' ')
                    {
                        lastEmptyIndex = charIndex;
                    }
                    else
                    {
                        charIndexWithoutEmpty++;
                    }
                }
                if (splitWordHasEmpty[splitWordHasEmpty.Length - charIndex] == ' ')
                {//为空，则刚好可以截取
                    strCut = splitWordHasEmpty.Substring(splitWordHasEmpty.Length - charIndex);
                }
                else
                {//不为空，看后面有没有空格，没空格说明不能拆分，有空格所以刚好可以截取
                    if (splitWordHasEmpty[splitWordHasEmpty.Length - charIndex - 1] == ' ')
                    {
                        strCut = splitWordHasEmpty.Substring(splitWordHasEmpty.Length - charIndex);
                    }
                    else
                    {
                        strCut = splitWordHasEmpty.Substring(splitWordHasEmpty.Length - lastEmptyIndex);
                    }
                }
                splitWord = strCut.Replace(" ", "");
            }

            Match matchTwo = Regex.Match(html, @"<p.*class=""nrt-guide"".*?>([\w\W]*?)</p>");
            if (matchTwo.Success)
            {
                string htmlTwo = matchTwo.Groups[1].Value;
                List<string> lstResultWord = new List<string>();
                MatchCollection matchResultWord = Regex.Matches(htmlTwo, "<a.*?>(.*)</a>");
                foreach (Match m in matchResultWord)
                {
                    string strMatchValue = m.Groups[1].Value;
                    string[] arrWord = strMatchValue.Split(' ');
                    foreach (string item in arrWord)
                    {
                        if (!lstResultWord.Contains(item))
                        {
                            lstResultWord.Add(item);
                        }
                    }
                }
                resultWord = string.Join(",", lstResultWord.ToArray());
            }
            else
            {
                MessageBox.Show("天猫分词页面有变化");
            }
        }
    }
}
