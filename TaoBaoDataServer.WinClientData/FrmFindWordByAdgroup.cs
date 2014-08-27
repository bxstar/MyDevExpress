using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaoBaoDataServer.WinClientData.Model;
using Top.Api.Domain;
using log4net;
using NetServ.Net.Json;
using System.Text.RegularExpressions;
using System.Threading;
using TaoBaoDataServer.WinClientData.BusinessLayer;

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmFindWordByAdgroup : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        IOutPut frmOutPut;
        private static log4net.ILog logger = LogManager.GetLogger("Logger");
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();
        BusinessAdgroupHandler adgroupHandler = new BusinessAdgroupHandler();
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();
        BusinessUserHandler userHandler = new BusinessUserHandler();

        public FrmFindWordByAdgroup()
        {
            InitializeComponent();
        }

        public FrmFindWordByAdgroup(IOutPut _frmOutPut)
        {
            frmOutPut = _frmOutPut;
            InitializeComponent();
        }

        private void FrmFindWordByAdgroup_Load(object sender, EventArgs e)
        {
            dgv.AutoGenerateColumns = false;
            dgv.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgv_RowPostPaint);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            TopSession session = userHandler.GetUserSession(txtNickName.Text);
            string adgroupId = txtAdgroupId.Text.Trim();

            double clickCost = 0;
            double.TryParse(txtMaxPrice.Text, out clickCost);
            if (clickCost == 0)
            {
                MessageBox.Show("请设置找词的单次点击限价");
                return;
            }
            ADGroup itemAdgroup = adgroupHandler.GetAdgroupByAdgroupId(Convert.ToInt64(adgroupId));
            if (itemAdgroup.AdgroupId == 0)
            {
                itemAdgroup = adgroupHandler.GetAdgroupOnlineByAdgroupId(session, Convert.ToInt64(adgroupId));
            }

            if (itemAdgroup.AdgroupId != 0)
            {
                frmOutPut.OutPutMsg("开始找词");
                List<EntityKeywordEx> lstKeyword = FindKeyword(session, itemAdgroup, 200, 1.8, new List<string>());
                string msg = string.Format("推广组{0}找词完成，找到{1}个词", itemAdgroup.AdgroupId, lstKeyword.Count);
                frmOutPut.OutPutMsg(msg);
                dgv.DataSource = new SortableBindingList<EntityKeywordEx>(lstKeyword);
            }
            else
            {
                frmOutPut.OutPutMsg("获取推广组信息失败");
            }
        }

        #region 取词模块

        #region 添加关键词

        /// <summary>
        /// 给用户找词
        /// </summary>
        private List<EntityKeywordEx> FindKeyword(TopSession user, ADGroup adgroup, int nCount, double clickCost, List<string> lstExistKeyword)
        {
            string parentCatId = string.Empty;
            // 获得最底级别的类目ID
            string catId = SplitCategoryId(adgroup.CategoryIds, ref parentCatId);
            List<EntityKeywordEx> keywordNeedAdd = new List<EntityKeywordEx>();
            string title = string.Empty;//宝贝标题
            try
            {
                List<EntityKeywordEx> responseWord = new List<EntityKeywordEx>();

                #region 类目分词

                var listCategoryNameWord = GetCategoryName(Convert.ToInt64(catId));
                if (listCategoryNameWord != null && listCategoryNameWord.Count > 0)
                {
                    foreach (string word in listCategoryNameWord)
                    {
                        if (responseWord.Where(o => o.keyword == word.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(word.ToLower().Trim()))
                        {//去重复
                            responseWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "类目分词" });
                        }
                    }
                }
                // 过滤特殊词
                responseWord = FilterSpecialWord(responseWord, title, parentCatId);
                #endregion

                #region 获取属性词

                var listAttribute = GetItemAttribute(adgroup.NumIid, ref title);
                if (listAttribute != null && listAttribute.Count > 0)
                {
                    foreach (string categoryName in listCategoryNameWord)
                    {// 循环组合类目词与属性词
                        foreach (string attribute in listAttribute)
                        {
                            string word = attribute.ToLower().Trim() + " " + categoryName.ToLower().Trim();
                            if (responseWord.Where(o => o.keyword == word.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(word.ToLower().Trim()))
                            {//去重复
                                responseWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "组合类目词与属性词" });
                            }
                        }
                    }
                    foreach (string attribute in listAttribute)
                    {
                        if (responseWord.Where(o => o.keyword == attribute.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(attribute.ToLower().Trim()))
                        {//去重复
                            responseWord.Add(new EntityKeywordEx() { keyword = attribute.ToLower().Trim(), FindSource = "属性词" });
                        }
                    }
                }

                responseWord = FilterSpecialWord(responseWord, title, parentCatId);
                #endregion

                #region  获取top100关键词

                var listTopWord = GetCatTop100Keyword(user, catId);
                if (listTopWord != null && listTopWord.Count > 0)
                {
                    foreach (string word in listTopWord)
                    {
                        if (responseWord.Where(o => o.keyword == word.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(word.ToLower().Trim()))
                        {//去重复
                            responseWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "top100关键词" });
                        }
                    }
                }

                responseWord = FilterSpecialWord(responseWord, title, parentCatId);
                #endregion

                #region 获取系统推荐词

                var listRecommendKeyword = GetRecommendKeyword(user, adgroup.AdgroupId);
                if (listRecommendKeyword != null && listRecommendKeyword.Count > 0)
                {
                    foreach (string word in listRecommendKeyword)
                    {
                        if (responseWord.Where(o => o.keyword == word.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(word.ToLower().Trim()))
                        {//去重复
                            responseWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "系统推荐词" });
                        }
                    }
                }

                responseWord = FilterSpecialWord(responseWord, title, parentCatId);
                #endregion

                #region 差分类目后拓展词

                var listCatExt = GetCategoryOtherWord(user, listCategoryNameWord);
                if (listCatExt != null && listCatExt.Count > 0)
                {
                    foreach (string word in listCatExt)
                    {
                        if (responseWord.Where(o => o.keyword == word.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(word.ToLower().Trim()))
                        {//去重复
                            responseWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "差分类目后拓展词" });
                        }
                    }
                }

                responseWord = FilterSpecialWord(responseWord, title, parentCatId);
                #endregion

                #region 从词库中取词，优先级最低
                if (responseWord.Count < (nCount))
                {//取到的词数量不够，再从爬网页的词库中获取
                    var listBank = GetKeywordByBank(catId);
                    if (listBank != null && listBank.Count > 0)
                    {
                        foreach (string word in listBank)
                        {
                            if (responseWord.Where(o => o.keyword == word.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(word.ToLower().Trim()))
                            {//去重复
                                responseWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "词库中取词" });
                            }
                        }
                    }
                    responseWord = FilterSpecialWord(responseWord, title, parentCatId);
                }

                #endregion

                // 获取关键词的指数以及最相关的类目信息
                List<KeywordAvgData> relatedWord = GetkeywordIndex(user, responseWord.Select(o => o.keyword).ToList(), catId);
                if (relatedWord != null && relatedWord.Count > 0)
                {
                    if (user.GetMajorConfig(adgroup.AdgroupId).IsMustLessClickCost)
                    {// 从找到的词中，获取指定数量的词，类目匹配，不取超过限价词，并按类目预测等级和点击率排序
                        var result = (from s in relatedWord where s.CategoryLevel > 0 && s.SuggestPrice <= clickCost orderby s.CategoryLevel ascending, s.Ctr descending select s).Take(nCount).ToList();
                        if ((result.Count * 1.0) / relatedWord.Count < 0.4 && (result.Count * 1.0 < nCount * 0.2))
                        {//经过类目匹配后的词太少，则从词库中找词
                            logger.InfoFormat("用户{0},推广组:{1},找到{2}个词,过滤后{3}个词,经过类目匹配后的词太少,继续从词库中取词", user.UserName, adgroup.AdgroupId, relatedWord.Count, result.Count);
                            List<EntityKeywordEx> responseBankWord = new List<EntityKeywordEx>();
                            var listBank = GetKeywordByBank(catId);
                            if (listBank != null && listBank.Count > 0)
                            {
                                foreach (string word in listBank)
                                {
                                    if (responseWord.Where(o => o.keyword == word.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(word.ToLower().Trim()))
                                    {//去重复
                                        responseBankWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "词库中取词" });
                                        responseWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "词库中取词" });
                                    }
                                }
                            }
                            responseBankWord = FilterSpecialWord(responseBankWord, title, parentCatId);
                            List<KeywordAvgData> relatedBankWord = GetkeywordIndex(user, responseBankWord.Select(o => o.keyword).ToList(), catId);
                            result = (from s in relatedWord.Union(relatedBankWord) where s.CategoryLevel > 0 orderby s.CategoryLevel ascending, s.Ctr descending select s).Take(nCount).ToList();
                        }
                        foreach (KeywordAvgData keywordinfo in result)
                        {
                            // 设置默认价格
                            EntityKeywordEx wordEx = responseWord.Find(o => o.keyword == keywordinfo.Word);
                            // 设置默认价格
                            long initialPrice = user.GetMajorConfig(adgroup.AdgroupId).MinPrice;
                            if (keywordinfo.SuggestPrice >= (initialPrice / 100.0))
                            {
                                initialPrice = Convert.ToInt64(keywordinfo.SuggestPrice * 100);
                            }
                            keywordNeedAdd.Add(new EntityKeywordEx()
                            {
                                campaign_id = adgroup.CampaignId,
                                adgroup_id = adgroup.AdgroupId,
                                keyword = keywordinfo.Word,
                                max_price = initialPrice,
                                is_default_price = false,
                                match_scope = user.GetMajorConfig(adgroup.AdgroupId).MatchScope,
                                FindSource = wordEx.FindSource,
                                pv = keywordinfo.Pv,
                                click = keywordinfo.Click,
                                ctr = keywordinfo.Ctr,
                                avgPrice = keywordinfo.AvgPrice,
                                categoryLevel = keywordinfo.CategoryLevel
                            });
                        }
                    }
                    else
                    {// 从找到的词中，获取指定数量的词，类目匹配，可以取超过限价词（使用单次点击限价来出价），并按类目预测等级和点击率排序
                        var result = (from s in relatedWord where s.CategoryLevel > 0 orderby s.CategoryLevel ascending, s.Ctr descending select s).Take(nCount).ToList();
                        if ((result.Count * 1.0) / relatedWord.Count < 0.4 && (result.Count * 1.0 < nCount * 0.2))
                        {//经过类目匹配的词太少，则再从词库取词
                            List<EntityKeywordEx> responseBankWord = new List<EntityKeywordEx>();
                            var listBank = GetKeywordByBank(catId);
                            if (listBank != null && listBank.Count > 0)
                            {
                                foreach (string word in listBank)
                                {
                                    if (responseBankWord.Where(o => o.keyword == word.ToLower().Trim()).Count() == 0 && !lstExistKeyword.Contains(word.ToLower().Trim()))
                                    {//去重复
                                        responseBankWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "词库中取词" });
                                        responseWord.Add(new EntityKeywordEx() { keyword = word.ToLower().Trim(), FindSource = "词库中取词" });
                                    }
                                }
                            }
                            responseBankWord = FilterSpecialWord(responseBankWord, title, parentCatId);
                            List<KeywordAvgData> relatedBankWord = GetkeywordIndex(user, responseBankWord.Select(o => o.keyword).ToList(), catId);
                            result = (from s in relatedWord.Union(relatedBankWord) where s.CategoryLevel > 0 orderby s.CategoryLevel ascending, s.Ctr descending select s).Take(nCount).ToList();
                        }
                        foreach (KeywordAvgData keywordinfo in result)
                        {
                            EntityKeywordEx wordEx = responseWord.Find(o => o.keyword == keywordinfo.Word);
                            // 初始价，默认为最低价
                            long initialPrice = user.GetMajorConfig(adgroup.AdgroupId).MinPrice;
                            if (keywordinfo.SuggestPrice >= (initialPrice / 100.0) && keywordinfo.SuggestPrice <= clickCost)
                            {//大于最低价小于最高价，则用建议出价
                                initialPrice = Convert.ToInt64(keywordinfo.SuggestPrice * 100);
                            }
                            else if (keywordinfo.SuggestPrice > clickCost)
                            {//大于最高价，直接用单次点击限价
                                initialPrice = Convert.ToInt64(clickCost * 100);
                            }
                            keywordNeedAdd.Add(new EntityKeywordEx()
                            {
                                campaign_id = adgroup.CampaignId,
                                adgroup_id = adgroup.AdgroupId,
                                keyword = keywordinfo.Word,
                                max_price = initialPrice,
                                is_default_price = false,
                                match_scope = user.GetMajorConfig(adgroup.AdgroupId).MatchScope,
                                FindSource = wordEx.FindSource,
                                pv = keywordinfo.Pv,
                                click = keywordinfo.Click,
                                ctr = keywordinfo.Ctr,
                                avgPrice = keywordinfo.AvgPrice,
                                categoryLevel = keywordinfo.CategoryLevel
                            });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/FindKeyword,给用户找词 失败：", ex);
            }

            //var duplicateWord = keywordNeedAdd.GroupBy(o => o.Word).Where(g => g.Count() > 1).Select(g => g.ElementAt(0));
            //foreach (var item in duplicateWord)
            //{
            //    System.Diagnostics.Debug.WriteLine(item.Word);
            //}

            return keywordNeedAdd;
        }
        #endregion

        #region 获取类目下top100
        /// <summary>
        /// 获取类目top100
        /// </summary>
        /// <param name="topCount">取词数</param>
        /// <param name="session">session</param>
        /// <param name="categoryId">类目id</param>
        /// <param name="listWords">存放找到的词的容器</param>
        /// <param name="hashWordsOnline">下载下来已经在推广的词</param>
        private List<string> GetCatTop100Keyword(TopSession session, string categoryId)
        {
            // 定义返回值
            List<string> result = new List<string>();
            try
            {
                // 从线上获取Top100的关键词
                var response = TaobaoApiHandler.TaobaoSimbaInsightCatstopwordGet(session, categoryId);
                // 获取Top100的关键词
                if (response != null && response.TopWords != null && response.TopWords.Count > 0)
                {
                    // 循环加入返回值
                    for (int i = 0; i < response.TopWords.Count; i++)
                    {
                        result.Add(response.TopWords[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetCatTop100Keyword,淘宝API获取类目下Top100的关键词 失败：", ex);
            }
            return result;
        }
        #endregion

        #region 获取推荐关键词
        /// <summary>
        /// 获取推荐关键词
        /// </summary>
        /// <param name="adGroupId">宝贝id</param>
        /// <param name="session"></param>
        /// <param name="listWords">存放找到的词的容器</param>
        /// <param name="hashWordsOnline">下载下来已经在推广的词</param>
        private List<string> GetRecommendKeyword(TopSession session, long adGroupId)
        {
            List<string> result = new List<string>();
            try
            {
                var response = TaobaoApiHandler.TaobaoSimbaKeywordsRecommendGet(session, adGroupId, "search_volume", 1, 200);

                if (response != null && response.RecommendWords != null && response.RecommendWords.RecommendWordList != null)
                {
                    foreach (RecommendWord word in response.RecommendWords.RecommendWordList)
                    {
                        result.Add(word.Word);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetRecommendKeyword,淘宝API获取推荐词 失败：", ex);
            }
            return result;
        }
        #endregion

        #region 获取类目名称
        /// <summary>
        /// 递归获取类目名称
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <param name="strCategoryName"></param>
        private List<string> GetCategoryName(long categoryIds)
        {
            List<string> list = new List<string>();
            try
            {
                // 定义参数
                var param = new Dictionary<string, object>();
                param.Add("category_id", categoryIds);
                // 定义Sql语句
                var sqlText = "SELECT category_name FROM ad_category where category_id = @category_id";
                // 数据库执行查询
                var dsCategory = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), sqlText, SqlDataProvider.ConvertSqlParameter(param));
                // 如果数据库存在的情况下
                if (dsCategory != null && dsCategory.Tables.Count > 0 && dsCategory.Tables[0].Rows.Count > 0)
                {
                    string catName = dsCategory.Tables[0].Rows[0][0].ToString();
                    // 如果类目存在
                    var strstmp = catName.Split('/');
                    // 循环加入返回值
                    for (int t = 0; t < strstmp.Length; t++)
                    {
                        list.Add(strstmp[t].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetCategoryName, 从数据库获取类目名称失败：", ex);
            }
            return list;
        }
        #endregion

        #region 获取宝贝属性
        /// <summary>
        /// 从宝贝的属性获取关键词
        /// </summary>
        /// <param name="itemId">宝贝的itemid</param>
        /// <param name="listNeedMatchCat">需要去匹配类目的词存放容器</param>
        /// <param name="hashWordsOnline">下载下来已经在推广的词</param>
        /// <returns></returns>
        private List<string> GetItemAttribute(long itemId, ref string title)
        {
            var list = new List<string>();
            try
            {
                var response = TaobaoApiHandler.TaobaoItemGet(itemId);

                if (response != null && response.Item != null)
                {
                    title = response.Item.Title;
                    // 获取宝贝属性
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
                logger.Error("BusinessBatchHandler/GetItemAttribute, 获取宝贝的属性：", ex);
            }
            return list;
        }
        #endregion

        #region 从词库中取词

        /// <summary>
        /// 从词库中取得关键词
        /// </summary>
        /// <returns></returns>
        private List<string> GetKeywordByBank(string categoryId)
        {
            var list = new List<string>();
            try
            {
                // 定义变量，存储参数
                var param = new Dictionary<string, object>();
                param.Add("fCategoryId", Convert.ToInt64(categoryId));
                // 定义Sql语句
                var sqlText = "SELECT fKeyword FROM ad_keyword_bank where fCategoryId=@fCategoryId";
                // 数据库执行查询
                var dsCategoryKeyword = SqlHelper.ExecuteDataSet(SqlDataProvider.GetAPSqlConnection(), sqlText, SqlDataProvider.ConvertSqlParameter(param));
                // 如果数据存在的情况下
                if (dsCategoryKeyword != null && dsCategoryKeyword.Tables.Count > 0 && dsCategoryKeyword.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCategoryKeyword.Tables[0].Rows.Count; i++)
                    {
                        list.Add(dsCategoryKeyword.Tables[0].Rows[i]["fKeyword"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetKeywordByBank, 从词库中获取关键词：", ex);
            }
            return list;
        }
        #endregion

        #region 拓展类目名称分词

        /// <summary>
        /// 拓展类目分词
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <returns></returns>
        private List<string> GetCategoryOtherWord(TopSession session, List<string> categoryNameList)
        {
            var result = new List<string>();
            try
            {
                if (categoryNameList != null && categoryNameList.Count > 0)
                {
                    foreach (string catName in categoryNameList)
                    {
                        // 从下拉框中获取
                        var taobaoSearchList = GetTaobaoSearchList(catName);
                        if (taobaoSearchList != null && taobaoSearchList.Count > 0)
                        {
                            foreach (string keyword in taobaoSearchList)
                            {
                                if (!result.Contains(keyword.Trim()))
                                {
                                    result.Add(keyword.Trim());
                                }
                            }
                        }
                        // 从你是不是想找中获取
                        var taobaoWantSearchList = GetTaobaoWantSearchList(catName);
                        if (taobaoWantSearchList != null && taobaoWantSearchList.Count > 0)
                        {
                            foreach (string keyword in taobaoWantSearchList)
                            {
                                if (!result.Contains(keyword.Trim()))
                                {
                                    result.Add(keyword.Trim());
                                }
                            }
                        }
                        // 从API中获取
                        var taobaoApiList = GetTaobaoApiList(session, catName);
                        if (taobaoApiList != null && taobaoApiList.Count > 0)
                        {
                            foreach (string keyword in taobaoApiList)
                            {
                                if (!result.Contains(keyword.Trim()))
                                {
                                    result.Add(keyword.Trim());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetCategoryOtherWord,拓展类目分词 失败：", ex);
            }
            return result;
        }
        #endregion

        #region 拓展属性词
        /// <summary>
        /// 拓展属性词
        /// </summary>
        private List<string> GetAttributeOtherWord(TopSession session, List<string> attributeList)
        {
            var result = new List<string>();
            try
            {
                if (attributeList != null && attributeList.Count > 0)
                {
                    foreach (string attribute in attributeList)
                    {
                        // 从下拉框中获取
                        var taobaoSearchList = GetTaobaoSearchList(attribute);
                        if (taobaoSearchList != null && taobaoSearchList.Count > 0)
                        {
                            foreach (string keyword in taobaoSearchList)
                            {
                                if (!result.Contains(keyword))
                                {
                                    result.Add(keyword);
                                }
                            }
                        }
                        // 从你是不是想找中获取
                        var taobaoWantSearchList = GetTaobaoWantSearchList(attribute);
                        if (taobaoWantSearchList != null && taobaoWantSearchList.Count > 0)
                        {
                            foreach (string keyword in taobaoWantSearchList)
                            {
                                if (!result.Contains(keyword))
                                {
                                    result.Add(keyword);
                                }
                            }
                        }
                        // 从API中获取
                        var taobaoApiList = GetTaobaoApiList(session, attribute);
                        if (taobaoApiList != null && taobaoApiList.Count > 0)
                        {
                            foreach (string keyword in taobaoApiList)
                            {
                                if (!result.Contains(keyword))
                                {
                                    result.Add(keyword);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/GetAttributeOtherWord,拓展类目分词 失败：", ex);
            }
            return result;
        }
        #endregion

        #region 抓取淘宝下拉框的值
        /// <summary>
        /// 抓取淘宝下拉框的值
        /// </summary>
        /// <param name="keyword">种子关键词</param>
        /// <returns></returns>
        private List<string> GetTaobaoSearchList(string keyword)
        {
            var listResult = new List<string>();
            string strJson = TechNet.DownLoadString("http://suggest.taobao.com/sug?code=utf-8&extras=1&callback=jsoncallback&q=" + keyword, Encoding.UTF8);
            strJson = TechNet.AnalysisTaoBaoJson(strJson);
            // 解析推广组json数据
            JsonObject data;
            // 解析推广组
            using (JsonParser parser = new JsonParser(new System.IO.StringReader(strJson), true))
            {
                data = parser.ParseObject();
            }
            JsonArray array = data["result"] as NetServ.Net.Json.JsonArray;
            foreach (JsonArray service in array)
            {
                listResult.Add(service[0].ToString());
            }
            return listResult;
        }

        /// <summary>
        /// 你是不是想找的词
        /// </summary>
        /// <param name="keyword">种子关键词</param>
        /// <returns></returns>
        private List<string> GetTaobaoWantSearchList(string keyword)
        {
            var lstResult = new List<string>();
            string html = TechNet.DownLoadString("http://s.taobao.com/search?q=" + keyword, Encoding.GetEncoding("GBK"));
            MatchCollection match = Regex.Matches(html, @"<li><a.*trace=""relatedSearch"".*?>(.*?)<.*></li>", RegexOptions.IgnoreCase);
            foreach (Match m in match)
            {
                if (m.Success)
                {
                    lstResult.Add(m.Groups[1].Value);
                }
                else
                {
                    logger.Info("你是不是想找的词正则匹配错误");
                }
            }
            return lstResult;
        }



        /// <summary>
        /// 根据核心词从淘宝API中拓展关键词
        /// </summary>
        private List<string> GetTaobaoApiList(TopSession session, string keyword)
        {
            var listResult = new List<string>();
            var listTaobao = TaobaoApiHandler.TaobaoSimbaInsightCatsrelatedwordGet(session, keyword);
            if (listTaobao != null && listTaobao.RelatedWords != null)
            {
                foreach (var strAntistop in listTaobao.RelatedWords)
                {
                    listResult.Add(strAntistop);
                }
            }
            return listResult;
        }

        #endregion

        #endregion


        #region 共通方法

        #region 要区分男女的类目ID

        #region 过滤特殊词
        /// <summary>
        /// 过滤特殊词
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private List<EntityKeywordEx> FilterSpecialWord(List<EntityKeywordEx> param, string title, string parentCatId)
        {
            var responseWord = param;
            #region 过滤男女
            string strGirl = "0";
            // 获取特殊类目，不分男女类别的类目
            var list = GetSpecialCategory();
            if (list.Contains(parentCatId))
            {
                if (title.Contains("男") && !title.Contains("女"))
                {
                    strGirl = "1";
                }
                if (title.Contains("女") && !title.Contains("男"))
                {
                    strGirl = "2";
                }
            }
            // 男装类目
            var listBoyCat = GetBoyCategory();
            if (listBoyCat.Contains(parentCatId))
            {
                strGirl = "1";
            }
            // 女装类目
            var listGirlCat = GetGirlCategory();
            if (listGirlCat.Contains(parentCatId))
            {
                strGirl = "2";
            }

            if (strGirl != "0")
            {
                // 存储过滤后的词
                List<EntityKeywordEx> listFilterWord = new List<EntityKeywordEx>();
                foreach (var strkeyword in responseWord)
                {
                    if (strGirl == "1" && !strkeyword.keyword.Contains("女"))
                    {
                        listFilterWord.Add(strkeyword);
                    }
                    if (strGirl == "2" && !strkeyword.keyword.Contains("男"))
                    {
                        listFilterWord.Add(strkeyword);
                    }
                }
                // 过滤后重新赋值给结果变量
                responseWord = listFilterWord;
            }
            //过滤纯数字
            responseWord = responseWord.Where(o => !TechNet.IsShuZhi(o.keyword)).ToList();

            #endregion
            return responseWord;
        }
        #endregion

        /// <summary>
        /// 特殊的接口，不分男女的类目
        /// </summary>
        /// <returns></returns>
        private List<string> GetSpecialCategory()
        {
            // 要区分男女的类目ID
            var list = new List<string>();
            // 运动服/运动包/颈环配件
            list.Add("50011699");
            // 运动鞋new
            list.Add("50012029");
            // 箱包皮具/热销女包/男包
            list.Add("50006842");
            // 女士内衣/男士内衣/家居服
            list.Add("1625");
            // 童装/童鞋/亲子装
            list.Add("50008165");
            // 品牌手表/流行手表
            list.Add("50005700");
            return list;
        }

        /// <summary>
        /// 特殊的类目接口，女的类目
        /// </summary>
        /// <returns></returns>
        private List<string> GetGirlCategory()
        {
            var list = new List<string>();
            // 女装/女士精品
            list.Add("16");
            // 女鞋
            list.Add("50006843");

            return list;
        }

        /// <summary>
        /// 特殊的类目接口，男的类目
        /// </summary>
        /// <returns></returns>
        private List<string> GetBoyCategory()
        {
            var list = new List<string>();
            // 男装
            list.Add("30");
            // 流行男鞋
            list.Add("50011740");
            return list;
        }
        #endregion

        #region 计算关键词的平均数据
        /// <summary>
        /// 取得一周内平均值
        /// </summary>
        /// <param name="param">一周关键词信息</param>
        /// <returns></returns>
        private KeywordAvgData GetKeywordAvgData(List<INRecordBase> param)
        {
            var avgData = new KeywordAvgData();
            decimal pv = 0;
            decimal cpc = 0;
            decimal click = 0;
            decimal competition = 0;
            // 求和
            foreach (var inRecordBase in param)
            {
                pv = pv + inRecordBase.Pv;
                cpc = cpc + inRecordBase.AvgPrice;
                click = click + inRecordBase.Click;
                competition = competition + inRecordBase.Competition;
            }
            // 天数
            avgData.Days = param.Count;

            var avgPv = Math.Round((pv / param.Count), 0);
            // 取平均值
            avgData.Pv = Convert.ToInt64(avgPv);
            // 算取平均价格
            decimal avgPrice = Math.Round(cpc / param.Count, 2);

            avgData.AvgPrice = Convert.ToInt64(avgPrice);
            // 计算点击量
            var avgClick = Math.Round((click / param.Count), 0);

            avgData.Click = Convert.ToInt64(avgClick);
            // 计算宝贝竞争数
            var avgCompetition = Math.Round((competition / param.Count), 0);

            avgData.Competition = Convert.ToInt64(avgCompetition);
            // 计算点击率
            if (avgData.Pv > 0)
            {
                var ctr = Convert.ToDecimal(avgData.Click) / Convert.ToDecimal(avgData.Pv);

                avgData.Ctr = Math.Round(ctr, 4).ToString();
            }
            else
            {
                avgData.Ctr = "0";
            }
            // 转化成Double类型的建议出价
            avgData.SuggestPrice = GetSuggestPrice(Convert.ToDouble(avgData.AvgPrice) / 100);

            return avgData;
        }

        /// <summary>
        /// 获取建议出价
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        private double GetSuggestPrice(double price)
        {

            double resultPrice = 0;

            if (price <= 0.6)
            {
                resultPrice = Convert.ToDouble((price * 0.9).ToString("0.00"));
            }
            else if (price > 0.6 && price < 1)
            {
                resultPrice = Convert.ToDouble((price * 0.8).ToString("0.00"));
            }
            else
            {
                resultPrice = Convert.ToDouble((price * 0.6).ToString("0.00"));
            }

            // 取得随机数
            Random random = new Random();
            var rndNum = random.Next(1, 10);

            var randomNum = Convert.ToDouble(rndNum) / 100;
            resultPrice = (1 + randomNum) * resultPrice;
            return Math.Round(resultPrice, 2);
        }
        #endregion

        #region 差分推广组返回的类目ID，并返回主类目ID
        /// <summary>
        /// 差分推广组返回的类目ID
        /// </summary>
        /// <param name="categoryId">类目ID</param>
        /// <param name="firstCatId">所属的主类目</param>
        /// <returns>返回类目ID</returns>
        private string SplitCategoryId(string categoryId, ref string firstCatId)
        {
            // 定义返回值
            string lastCatId = "";
            // 进行差分类目
            var catIds = categoryId.Trim().Replace(" ", ",").Split(',');
            if (catIds.Length > 0)
            {
                // 设置最低层级的类目
                lastCatId = catIds[catIds.Length - 1];
                // 设置主类目ID
                firstCatId = catIds[0];
            }
            // 结果返回
            return lastCatId;
        }
        #endregion

        #region 获取关键词的指数以及最匹配的类目信息

        /// <summary>
        /// 获取关键词的指数以及最相关的类目信息
        /// </summary>
        /// <param name="session">用户信息</param>
        /// <param name="lstword">需要预测的关键词列表</param>
        /// <param name="lstword">需要匹配的类目ID</param>
        public List<KeywordAvgData> GetkeywordIndex(TopSession session, List<string> lstword, string categoryId)
        {
            //存储结果集
            var resultLst = new List<KeywordAvgData>();
            //对关键词进行分组，每组最多170个
            List<List<string>> groupLstword = TechNet.SplitLst<string>(lstword, 170);

            // 循环取得每组关键词获得基础数据和类目
            foreach (List<string> group in groupLstword)
            {
                // 存储拼接后的关键词
                string strKeywords = string.Join(",", group.ToArray());
                // 词基础数据查询
                var response = TaobaoApiHandler.TaobaoSimbaInsightWordsbaseGetByWeek(session, strKeywords);
                // 判断取得的指数信息是否正确返回

                if (response != null && response.InWordBases != null && response.InWordBases.Count > 0)
                {
                    List<string> lstKeyword = new List<string>();
                    foreach (var wordBase in response.InWordBases)
                    {
                        lstKeyword.Add(wordBase.Word);
                        // 计算词的平均值
                        KeywordAvgData keywordAvgData = GetKeywordAvgData(wordBase.InRecordBaseList);
                        keywordAvgData.Word = wordBase.Word;
                        resultLst.Add(keywordAvgData);
                    }
                    // 预测关键词类目，将关键词按照长度排序，长度最小的可能拿不到类目，放在前面
                    List<KeywordAndCategory> lstCategory = keywordHandler.ForecastCategory(session, lstKeyword.OrderBy(o => o.Length).ToList(), Convert.ToInt64(categoryId));
                    foreach (var item in lstCategory)
                    {
                        KeywordAvgData k = resultLst.Find(o => o.Word == item.Word);
                        if (k != null)
                        {
                            k.CategoryLevel = item.CategoryLevel;
                        }
                    }
                }
            }
            // 去掉重复
            return resultLst.Distinct(new KeywordAvgDataComparer()).ToList();
        }
        #endregion



        #endregion

        private void dgv_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dgv.RowHeadersWidth - 4,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgv.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dgv.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void btnLocate_Click(object sender, EventArgs e)
        {
            Boolean isFind = false;
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                EntityKeywordEx keywordItem = dgv.Rows[i].DataBoundItem as EntityKeywordEx;
                if (keywordItem.keyword == txtKeyword.Text)
                {
                    isFind = true;
                    dgv.FirstDisplayedScrollingRowIndex = i;
                    dgv.CurrentCell = dgv.Rows[i].Cells[0];
                }
            }
            if (!isFind)
            {
                MessageBox.Show("未找到关键词：" + txtKeyword.Text);
            }
        }
    }
}
