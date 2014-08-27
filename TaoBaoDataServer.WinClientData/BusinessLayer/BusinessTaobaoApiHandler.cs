using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Top.Api;
using Top.Api.Response;
using Top.Api.Request;
using TaoBaoDataServer.WinClientData.Model;
using Top.Api.Domain;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    public class BusinessTaobaoApiHandler
    {
        /// <summary>
        /// TopAPI
        /// </summary>
        private static ITopClient _client = new DefaultTopClient(Config.C_Url, Config.AppKey, Config.AppSecret, "json");

        /// <summary>
        /// 根据参数设置top连接的客户端
        /// </summary>
        public void SetTopClient(string url, string appKey, string appSecret, string format)
        {
            _client = new DefaultTopClient(url, appKey, appSecret, format);
        }


        #region 店铺

        /// <summary>
        /// 获取卖家店铺的基本信息
        /// </summary>
        /// <param name="nick">卖家昵称</param>
        /// <param name="fields">需返回的字段列表。可选值：Shop 结构中的所有字段；多个字段之间用逗号(,)分隔</param>
        /// <returns></returns>
        public ShopGetResponse TaobaoShopGet(string nick, string fields)
        {
            var req = new ShopGetRequest
            {
                Nick = nick,
                Fields = fields
            };
            var response = _client.Execute(req);
            return response;
        }

        #endregion

        #region 商品

        /// <summary>
        /// 得到单个商品信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fields"></param>
        /// <param name="numIid"></param>
        /// <returns></returns>
        public ItemGetResponse TaobaoItemGetByNumIid(TopSession session, string fields, long numIid)
        {
            var req = new ItemGetRequest
            {
                Fields = fields,
                NumIid = numIid
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 批量获取商品信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fields"></param>
        /// <param name="numIid"></param>
        /// <returns></returns>
        public ItemsListGetResponse TaobaoItemListGet(TopSession session, string fields, string numIids)
        {
            var req = new ItemsListGetRequest
            {
                Fields = fields,
                NumIids = numIids
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 获取当前会话用户出售中的商品列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="fields">需返回的字段列表。可选值：Item商品结构体中的以下字段： approve_status,num_iid,title,nick,type,cid,pic_url,num,props,valid_thru,list_time,price,has_discount,has_invoice,has_warranty,has_showcase,modified,delist_time,postage_id,seller_cids,outer_id；字段之间用“,”分隔。 不支持其他字段，如果需要获取其他字段数据，调用taobao.item.get。</param>
        /// <param name="q">搜索字段。搜索商品的title。</param>
        /// <param name="orderBy">排序方式。格式为column:asc/desc ，column可选值:list_time(上架时间),delist_time(下架时间),num(商品数量)，modified(最近修改时间);默认上架时间降序(即最新上架排在前面)。如按照上架时间降序排序方式为list_time:desc</param>
        /// <param name="pageNo">页码。取值范围:大于零的整数。默认值为1,即默认返回第一页数据。用此接口获取数据时，当翻页获取的条数（page_no*page_size）超过10万,为了保护后台搜索引擎，接口将报错。所以请大家尽可能的细化自己的搜索条件，例如根据修改时间分段获取商品</param>
        /// <param name="pageSize">每页条数。取值范围:大于零的整数;最大值：200；默认值：40。用此接口获取数据时，当翻页获取的条数（page_no*page_size）超过10万,为了保护后台搜索引擎，接口将报错。所以请大家尽可能的细化自己的搜索条件，例如根据修改时间分段获取商品</param>
        /// <returns></returns>
        public ItemsOnsaleGetResponse TaobaoItemsOnsaleGet(TopSession session, string fields, string q, string orderBy, int pageNo, int pageSize)
        {
            var req = new ItemsOnsaleGetRequest { Fields = fields, Q = q, OrderBy = orderBy, PageNo = pageNo, PageSize = pageSize };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        #endregion

        #region 推广计划

        /// <summary>
        /// 取得一个客户的推广计划
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public SimbaCampaignsGetResponse TaobaoSimbaCampaignsGet(TopSession session)
        {
            var req = new SimbaCampaignsGetRequest
            {
                Nick = session.UserName
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.account.balance.get 获取用户余额
        /// </summary>
        public SimbaAccountBalanceGetResponse TaobaoSimbaAccountBalanceGet(TopSession session)
        {
            SimbaAccountBalanceGetRequest req = new SimbaAccountBalanceGetRequest();
            req.Nick = session.UserName;
            SimbaAccountBalanceGetResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 创建一个推广计划
        /// </summary>
        /// <param name="session"></param>
        /// <param name="title">推广计划名称，不能多余20个汉字，不能和客户其他推广计划同名。</param>
        /// <returns></returns>
        public SimbaCampaignAddResponse TaobaoSimbaCampaignAdd(TopSession session, string title)
        {
            var req = new SimbaCampaignAddRequest
            {
                Nick = session.UserName,
                Title = title
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        ///  更新一个推广计划
        /// </summary>
        /// <param name="session"></param>
        /// <param name="campaignId">推广计划Id</param>
        /// <param name="title">推广计划名称，不能多余20个汉字，不能和客户其他推广计划同名。</param>
        /// <param name="onlineStatus">用户设置的上下限状态；offline-下线；online-上线；</param>
        /// <returns></returns>
        public SimbaCampaignUpdateResponse TaobaoSimbaCampaignUpdate(TopSession session, long campaignId, string title, string onlineStatus)
        {
            var req = new SimbaCampaignUpdateRequest
            {
                Nick = session.UserName,
                CampaignId = campaignId,
                Title = title,
                OnlineStatus = onlineStatus
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得一个推广计划的日限额
        /// </summary>
        /// <param name="session"></param>
        /// <param name="campaignId">推广计划Id</param>
        /// <returns></returns>
        public SimbaCampaignBudgetGetResponse TaobaoSimbaCampaignBudgetGet(TopSession session, long campaignId)
        {
            var req = new SimbaCampaignBudgetGetRequest
            {
                Nick = session.UserName,
                CampaignId = campaignId
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 更新一个推广计划的日限额
        /// </summary>
        /// <param name="session"></param>
        /// <param name="campaignId">推广计划Id</param>
        /// <param name="budget">如果为空则取消限额；否则必须为整数，单位是元，不得小于30；</param>
        /// <returns></returns>
        public SimbaCampaignBudgetUpdateResponse TaobaoSimbaCampaignBudgetUpdate(TopSession session, long campaignId, long budget)
        {
            var req = new SimbaCampaignBudgetUpdateRequest
            {
                Nick = session.UserName,
                CampaignId = campaignId,
                Budget = budget,
                UseSmooth = true
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        #endregion

        #region 推广组

        /// <summary>
        /// 创建一个推广组
        /// </summary>
        /// <param name="session"></param>
        /// <param name="campaignId">推广计划Id</param>
        /// <param name="itemId">商品Id</param>
        /// <param name="defaultPrice">推广组默认出价，单位为分，不能小于5 不能大于日最高限额</param>
        /// <param name="title">创意标题，最多20个汉字</param>
        /// <param name="imgUrl">创意图片地址，必须是商品的图片之一</param>
        /// <returns></returns>
        public SimbaAdgroupAddResponse TaobaoSimbaAdgroupAdd(TopSession session, long campaignId, long itemId, long defaultPrice, string title, string imgUrl)
        {
            var req = new SimbaAdgroupAddRequest
            {

                Nick = session.UserName,
                CampaignId = campaignId,
                ItemId = itemId,
                DefaultPrice = defaultPrice,
                Title = title,
                ImgUrl = imgUrl
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 更新一个推广组的信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId">推广组Id</param>
        /// <param name="online_status">用户设置的上下线状态 offline-下线(暂停竞价)； online-上线；默认为online</param>
        /// <returns></returns>
        public SimbaAdgroupUpdateResponse TaobaoSimbaAdgroupUpdate(TopSession session, long adgroupId, string online_status)
        {
            var req = new SimbaAdgroupUpdateRequest
            {
                Nick = session.UserName,
                AdgroupId = adgroupId,
                OnlineStatus = online_status
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 删除一个推广组
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId">推广组Id</param>
        /// <returns></returns>
        public SimbaAdgroupDeleteResponse TaobaoSimbaAdgroupDelete(TopSession session, long adgroupId)
        {
            var req = new SimbaAdgroupDeleteRequest
            {
                Nick = session.UserName,
                AdgroupId = adgroupId,
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.adgroups.get 批量得到推广组 
        /// </summary>
        /// <param name="campaignId">推广计划编号</param>
        /// <param name="pageSize">页面行数</param>
        /// <param name="pageNo">当前页</param>
        /// <returns></returns>
        public SimbaAdgroupsbycampaignidGetResponse TaobaoSimbaAdgroupsGetByCampaignId(TopSession session, long campaignId, int pageSize, int pageNo)
        {
            var req = new SimbaAdgroupsbycampaignidGetRequest
            {

                Nick = session.UserName,
                CampaignId = campaignId,
                PageSize = pageSize,
                PageNo = pageNo
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.adgroupsbycampaignid.get 批量得到推广计划下的推广组
        /// </summary>
        /// <param name="campaignId">推广计划Id</param>
        /// <param name="pageSize">页面行数</param>
        /// <param name="pageNo">当前页</param>
        /// <returns></returns>
        public SimbaAdgroupsbycampaignidGetResponse TaobaoSimbaAdgroupsByCampaignId(TopSession session, long campaignId, int pageSize, int pageNo)
        {
            var req = new SimbaAdgroupsbycampaignidGetRequest
            {
                Nick = session.UserName,
                CampaignId = campaignId,
                PageSize = pageSize,
                PageNo = pageNo
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.adgroupsbyadgroupids.get 批量得到推广组
        /// </summary>
        /// <param name="adgroupIds">推广组ID列表</param>
        /// <param name="pageSize">页面行数</param>
        /// <param name="pageNo">当前页</param>
        /// <returns></returns>
        public SimbaAdgroupsbyadgroupidsGetResponse TaobaoSimbaAdgroupsByAdgroupIds(TopSession session, List<long> adgroupIds)
        {
            var req = new SimbaAdgroupsbyadgroupidsGetRequest
            {
                Nick = session.UserName,
                AdgroupIds = String.Join(",", adgroupIds.ConvertAll<string>(new Converter<long, string>(m => m.ToString())).ToArray()),
                PageNo = 1,
                PageSize = 200
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.adgroup.onlineitemsvon.get 获取用户上架在线销售的全部宝贝
        /// </summary>
        /// <param name="session"></param>
        /// <param name="orderField">排序字段，starts：按开始时间排序bidCount:按销量排序</param>
        /// <param name="orderBy">排序，true:降序， false:升序</param>
        /// <param name="pageNo">页尺寸，最大200</param>
        /// <param name="pageSize">页码，从1开始,最大50。最大只能获取1W个宝贝</param>
        /// <returns></returns>
        public SimbaAdgroupOnlineitemsvonGetResponse TaobaoSimbaAdgroupOnlineitemsvonGet(TopSession session, string orderField, bool orderBy, int pageNo, int pageSize)
        {
            var req = new SimbaAdgroupOnlineitemsvonGetRequest
            {
                Nick = session.UserName,
                OrderField = orderField,
                OrderBy = orderBy,
                PageNo = pageNo,
                PageSize = pageSize
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        #endregion

        #region 创意

        /// <summary>
        /// 根据推广组ID获得创意
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId">推广组Id</param>
        /// <returns></returns>
        public SimbaCreativesGetResponse TaobaoSimbaCreativesGetByAdgroupId(TopSession session, long adgroupId)
        {
            var req = new SimbaCreativesGetRequest
            {
                Nick = session.UserName,
                AdgroupId = adgroupId,
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 修改创意
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId">推广组Id</param>
        /// <param name="creativeId">创意Id</param>
        /// <param name="title">创意标题，最多20个汉字</param>
        /// <param name="imgUrl">创意图片地址，必须是推广组对应商品的图片之一</param>
        /// <returns></returns>
        public SimbaCreativeUpdateResponse TaobaoSimbaCreativeUpdate(TopSession session, long adgroupId, long creativeId, string title, string imgUrl)
        {
            var req = new SimbaCreativeUpdateRequest
            {
                Nick = session.UserName,
                AdgroupId = adgroupId,
                CreativeId = creativeId,
                Title = title,
                ImgUrl = imgUrl
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 增加创意
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId">推广组Id</param>
        /// <param name="title">创意标题，最多20个汉字</param>
        /// <param name="imgUrl">创意图片地址，必须是推广组对应商品的图片之一</param>
        /// <returns></returns>
        public SimbaCreativeAddResponse TaobaoSimbaCreativeAdd(TopSession session, long adgroupId, string title, string imgUrl)
        {
            var req = new SimbaCreativeAddRequest
            {
                Nick = session.UserName,
                AdgroupId = adgroupId,
                Title = title,
                ImgUrl = imgUrl
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        #endregion

        #region 关键词
        /// <summary>
        /// taobao.simba.keywordsvon.add 创建一批关键词
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId">推广组id</param>
        /// <param name="keywordPrices">关键词，出价和匹配方式json字符串，word:词，不能有一些特殊字符。maxPrice：价格，是整数，以“分”为单位，不能小于5，不能大于日限额,当使用默认出价时必须将这个值设置为0。; isDefaultPrice：是否使用默认出价，只能是0，1(0代表不使用，1代表使用)。matchscope只能是1,2,4（1代表精确匹配，2代表子串匹配，4代表广泛匹配）。</param>
        /// <returns></returns>
        public SimbaKeywordsvonAddResponse TaobaoSimbaKeywordsvonAdd(TopSession session, long adgroupId, string keywordPrices)
        {
            var req = new SimbaKeywordsvonAddRequest
            {
                Nick = session.UserName,
                AdgroupId = adgroupId,
                KeywordPrices = keywordPrices
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.keywords.pricevon.set 设置一批关键词的出价
        /// </summary>
        /// <param name="session"></param>
        /// <param name="keywordPrices">Json格式</param>
        /// <returns></returns>
        public SimbaKeywordsPricevonSetResponse TaobaoSimbaKeywordsPricevonSet(TopSession session, string keywordidPrices)
        {
            var req = new SimbaKeywordsPricevonSetRequest
            {
                Nick = session.UserName,
                KeywordidPrices = keywordidPrices
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.keywords.delete 删除一批关键词 
        /// </summary>
        /// <returns></returns>
        public SimbaKeywordsDeleteResponse TaobaoSimbaKeywordsDelete(TopSession session, long campaignId, List<long> keywordIds)
        {
            var req = new SimbaKeywordsDeleteRequest
            {
                Nick = session.UserName,
                CampaignId = campaignId,
                KeywordIds = String.Join(",", keywordIds.ConvertAll<string>(new Converter<long, string>(m => m.ToString())).ToArray())
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.keywords.delete 删除一个关键词 
        /// </summary>
        /// <returns></returns>
        public SimbaKeywordsDeleteResponse TaobaoSimbaKeywordsDeleteOne(TopSession session, long campaignId, string keywordIds)
        {
            var req = new SimbaKeywordsDeleteRequest
            {
                Nick = session.UserName,
                CampaignId = campaignId,
                KeywordIds = keywordIds
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        #endregion

        #region 直通车数据报表

        /// <summary>
        /// 获取登陆权限签名
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public SimbaLoginAuthsignGetResponse TaobaoSimbaLoginAuthsignGet(TopSession session)
        {
            var req = new SimbaLoginAuthsignGetRequest
            {
                Nick = session.UserName
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 推广计划报表基础数据对象
        /// </summary>
        /// <param name="session"></param>
        /// <param name="startTime">开始时间，格式yyyy-mm-dd</param>
        /// <param name="endTime">结束时间，格式yyyy-mm-dd</param>
        /// <returns></returns>
        public SimbaRptCampaignbaseGetResponse TaobaoSimbaRptCampaignbaseGet(TopSession session, string startTime, string endTime)
        {
            var req = new SimbaRptCampaignbaseGetRequest
            {
                Nick = session.UserName,
                SubwayToken = GetSubwayToken(session),
                CampaignId = session.CampaignId,
                SearchType = "SUMMARY",
                Source = "SUMMARY",
                StartTime = startTime,
                EndTime = endTime
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        public SimbaRptCampaignbaseGetResponse TaobaoSimbaRptCampaignbaseGet(TopSession session, long campaignId, string startTime, string endTime)
        {
            var req = new SimbaRptCampaignbaseGetRequest
            {
                Nick = session.UserName,
                SubwayToken = GetSubwayToken(session),
                CampaignId = campaignId,
                SearchType = "SUMMARY",
                Source = "SUMMARY",
                StartTime = startTime,
                EndTime = endTime
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 推广计划效果报表数据对象
        /// </summary>
        /// <param name="session"></param>
        /// <param name="startTime">开始时间，格式yyyy-mm-dd</param>
        /// <param name="endTime">结束时间，格式yyyy-mm-dd</param>
        /// <returns></returns>
        public SimbaRptCampaigneffectGetResponse TaobaoSimbaRptCampaigneffectGet(TopSession session, string startTime, string endTime)
        {
            var req = new SimbaRptCampaigneffectGetRequest
            {
                Nick = session.UserName,
                SubwayToken = GetSubwayToken(session),
                CampaignId = session.CampaignId,
                SearchType = "SUMMARY",
                Source = "SUMMARY",
                StartTime = startTime,
                EndTime = endTime
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        public SimbaRptCampaigneffectGetResponse TaobaoSimbaRptCampaigneffectGet(TopSession session, long campaignId, string startTime, string endTime)
        {
            var req = new SimbaRptCampaigneffectGetRequest
            {
                Nick = session.UserName,
                SubwayToken = GetSubwayToken(session),
                CampaignId = campaignId,
                SearchType = "SUMMARY",
                Source = "SUMMARY",
                StartTime = startTime,
                EndTime = endTime
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 推广计划下的推广组报表基础数据查询(只有汇总数据，无分类类型)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="startTime">开始时间，格式yyyy-mm-dd</param>
        /// <param name="endTime">结束时间，格式yyyy-mm-dd</param>
        /// <returns></returns>
        public SimbaRptCampadgroupbaseGetResponse TaobaoSimbaRptCampadgroupbaseGet(TopSession session, string startTime, string endTime)
        {
            var req = new SimbaRptCampadgroupbaseGetRequest
            {
                Nick = session.UserName,
                SubwayToken = session.SubwayToken,
                CampaignId = session.CampaignId,
                SearchType = "SUMMARY",
                Source = "SUMMARY",
                StartTime = startTime,
                EndTime = endTime
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        #endregion

        /// <summary>
        /// 取得类目属性预测
        /// </summary>
        /// <param name="strkeywords">关键词</param>
        /// <returns>取得类目</returns>
        public SimbaInsightCatsforecastGetResponse TaobaoSimbaInsightCatsforecastGet(TopSession session, string strkeywords)
        {
            var req = new SimbaInsightCatsforecastGetRequest { Nick = session.UserName, Words = strkeywords };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 词和类目查询
        /// </summary>
        /// <param name="strtypeids">用户选择的类别编号</param>
        /// <returns></returns>
        public SimbaInsightWordscatsGetResponse TaobaoSimbaInsightWordscatsGet(TopSession session, string strtypeids)
        {
            var req = new SimbaInsightWordscatsGetRequest { Nick = session.UserName, WordCategories = strtypeids, Filter = "PV,CLICK" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 类目相关词查询
        /// </summary>
        /// <param name="strkeywords">用户关键词</param>
        /// <returns></returns>
        public SimbaInsightCatsrelatedwordGetResponse TaobaoSimbaInsightCatsrelatedwordGet(TopSession session, string strkeywords)
        {
            var req = new SimbaInsightCatsrelatedwordGetRequest { Nick = session.UserName, Words = strkeywords, ResultNum = 10 };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 类目TOP词查询
        /// </summary>
        /// <returns></returns>
        public SimbaInsightCatstopwordGetResponse TaobaoSimbaInsightCatstopwordGet(TopSession session, string strTypeIds)
        {
            var req = new SimbaInsightCatstopwordGetRequest { Nick = session.UserName, CategoryIds = strTypeIds, ResultNum = 100 };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.insight.wordsanalysis.get 词分析数据查询 
        /// </summary>
        /// <returns></returns>
        public SimbaInsightWordsanalysisGetResponse TaobaoSimbaInsightWordsanalysisGet(TopSession session, string strWords, string stu)
        {
            var req = new SimbaInsightWordsanalysisGetRequest { Nick = session.UserName, Words = strWords, Stu = stu };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.tools.items.top.get 取得一个关键词的推广组排名列表 
        /// </summary>
        /// <param name="strWords">关键词</param>
        /// <param name="ip">IP</param>
        /// <returns></returns>
        public SimbaToolsItemsTopGetResponse TaobaoSimbaToolsItemsTopGet(TopSession session, string strWords, string ip)
        {
            var req = new SimbaToolsItemsTopGetRequest { Nick = session.UserName, Keyword = strWords, Ip = ip };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.insight.wordsbase.get 词基础数据查询 
        /// </summary>
        /// <param name="strWords">关键词</param>
        /// <returns></returns>
        public SimbaInsightWordsbaseGetResponse TaobaoSimbaInsightWordsbaseGetByDay(TopSession session, string strWords)
        {
            var req = new SimbaInsightWordsbaseGetRequest
            {
                Nick = session.UserName,
                Words = strWords,
                Time = "DAY",
                Filter = "PV,CLICK,AVGCPC,COMPETITION,CTR"
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.insight.wordsbase.get 词基础数据查询 
        /// </summary>
        /// <param name="strWords">关键词</param>
        /// <returns></returns>
        public SimbaInsightWordsbaseGetResponse TaobaoSimbaInsightWordsbaseGetByWeek(TopSession session, string strWords)
        {
            var req = new SimbaInsightWordsbaseGetRequest
            {
                Nick = session.UserName,
                Words = strWords,
                Time = "WEEK",
                Filter = "PV,CLICK,AVGCPC,COMPETITION,CTR"
            };
            var response = _client.Execute(req, session.TopSessions);
            if (response.InWordBases == null)
            {
                var reqDay = new SimbaInsightWordsbaseGetRequest
                {
                    Nick = session.UserName,
                    Words = strWords,
                    Time = "DAY",
                    Filter = "PV,CLICK,AVGCPC,COMPETITION,CTR"
                };
                response = _client.Execute(reqDay, session.TopSessions);
            }
            return response;
        }


        /// <summary>
        /// taobao.simba.insight.wordsbase.get 词基础数据查询 
        /// </summary>
        /// <param name="strWords">关键词</param>
        /// <returns></returns>
        public SimbaInsightWordsbaseGetResponse TaobaoSimbaInsightWordsbaseGetByMonth(TopSession session, string strWords)
        {
            var req = new SimbaInsightWordsbaseGetRequest
            {
                Nick = session.UserName,
                Words = strWords,
                Time = "MONTH",
                Filter = "PV,CLICK,AVGCPC,COMPETITION,CTR"
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.insight.catsbase.get 获取类目的基础数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="strCategoryIds"></param>
        /// <returns></returns>
        public SimbaInsightCatsbaseGetResponse TaobaoSimbaInsightCatsbaseGet(TopSession session, string strCategoryIds)
        {
            var req = new SimbaInsightCatsbaseGetRequest();
            req.Nick = session.UserName;
            req.CategoryIds = strCategoryIds;
            req.Time = "WEEK";
            req.Filter = "PV,CLICK,AVGCPC,COMPETITION,CTR";
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得产品信息
        /// </summary>
        /// <returns></returns>
        public ItemGetResponse TaobaoItemGet(long numIid)
        {
            var req = new ItemGetRequest { Fields = "num_iid,nick,title,price,cid,item_img,props_name,pic_url", NumIid = numIid };
            var response = _client.Execute(req, "");
            return response;
        }

        /// <summary>
        /// 类目的取得
        /// </summary>
        /// <param name="cid">属性编号</param>
        /// <returns>类目</returns>
        private ItemcatsGetResponse TaobaoItemcatsGet(long cid)
        {
            var req = new ItemcatsGetRequest { Fields = "cid,parent_cid,name,is_parent", Cids = cid.ToString() + "," };
            var response = _client.Execute(req);
            return response;
        }

        /// <summary>
        /// taobao.items.onsale.get 获取当前会话用户出售中的商品列表 
        /// </summary>
        /// <returns></returns>
        private ItemsOnsaleGetResponse TaobaoItemsOnsaleGet(TopSession session)
        {
            var req = new ItemsOnsaleGetRequest { Fields = "num_iid,title,cid" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.adgroups.item.exist 商品是否推广 
        /// </summary>
        /// <returns></returns>
        private SimbaAdgroupsItemExistResponse TaobaoSimbaAdgroupsItemExist(TopSession session, long campaignId, long itemId)
        {
            var req = new SimbaAdgroupsItemExistRequest
            {
                Nick = session.UserName,
                CampaignId = campaignId,
                ItemId = itemId
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 验证权限
        /// </summary>
        public SimbaCustomersAuthorizedGetResponse TaobaoSimbaCustomersAuthorizedGet(TopSession session)
        {
            var req = new SimbaCustomersAuthorizedGetRequest();
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.keywordsbyadgroupid.get 取得一个推广组的所有关键词
        /// </summary>
        public SimbaKeywordsbyadgroupidGetResponse TaoBaoSimbaKeywordsGet(TopSession session, long adGroupId)
        {
            var req = new SimbaKeywordsbyadgroupidGetRequest
            {
                Nick = session.UserName,
                AdgroupId = adGroupId
            };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得推广计划下，所有推广组报表的基础数据
        /// </summary>
        public SimbaRptCampadgroupbaseGetResponse TaobaoSimbaRptCampadgroupbaseGet(TopSession session, long campaignId, string starttime, string endtime)
        {
            string subtoken = GetSubwayToken(session);
            var req = new SimbaRptCampadgroupbaseGetRequest { SubwayToken = subtoken, Nick = session.UserName, CampaignId = campaignId, StartTime = starttime, EndTime = endtime, Source = "SUMMARY", PageNo = 1, PageSize = 500, SearchType = "SUMMARY" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得推广计划下，所有推广组报表的效果数据
        /// </summary>
        public SimbaRptCampadgroupeffectGetResponse TaobaoSimbaRptCampadgroupeffectGet(TopSession session, long campaignId, string starttime, string endtime)
        {
            string subtoken = GetSubwayToken(session);
            var req = new SimbaRptCampadgroupeffectGetRequest { SubwayToken = subtoken, Nick = session.UserName, CampaignId = campaignId, StartTime = starttime, EndTime = endtime, Source = "SUMMARY", PageNo = 1, PageSize = 500, SearchType = "SUMMARY" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得某个推广组报表的基础数据
        /// </summary>
        public SimbaRptAdgroupbaseGetResponse TaobaoSimbaRptAdgroupbaseGet(TopSession session, long campaignId, long adgroupId, string starttime, string endtime)
        {
            string subtoken = GetSubwayToken(session);
            var req = new SimbaRptAdgroupbaseGetRequest { SubwayToken = subtoken, Nick = session.UserName, CampaignId = campaignId, AdgroupId = adgroupId, StartTime = starttime, EndTime = endtime, Source = "SUMMARY", PageNo = 1, PageSize = 500, SearchType = "SUMMARY" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }


        /// <summary>
        /// 取得某个推广组报表的效果数据
        /// </summary>
        public SimbaRptAdgroupeffectGetResponse TaobaoSimbaRptAdgroupeffectGet(TopSession session, long campaignId, long adgroupId, string starttime, string endtime)
        {
            string subtoken = GetSubwayToken(session);
            var req = new SimbaRptAdgroupeffectGetRequest { SubwayToken = subtoken, Nick = session.UserName, CampaignId = campaignId, AdgroupId = adgroupId, StartTime = starttime, EndTime = endtime, Source = "SUMMARY", PageNo = 1, PageSize = 500, SearchType = "SUMMARY" };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 获取系统推荐词
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adGroupId">推广组id</param>
        /// <param name="orderBy">返回结果按照哪个排序，默认可以“search_volume”，详见API在线文档</param>
        /// <param name="pageNo">页号</param>
        /// <param name="pageSize">每次请求返回的个数</param>
        /// <returns></returns>
        public SimbaKeywordsRecommendGetResponse TaobaoSimbaKeywordsRecommendGet(TopSession session, long adGroupId, string orderBy, int pageNo, int pageSize)
        {
            var req = new SimbaKeywordsRecommendGetRequest { AdgroupId = adGroupId, Nick = session.UserName, OrderBy = orderBy, PageNo = pageNo, PageSize = pageSize };
            return _client.Execute(req, session.TopSessions);
        }

        /// <summary>
        /// 获取类目id对应的类目名称
        /// </summary>
        /// <param name="session"></param>
        /// <param name="categoryIds">完整的类目id，用,分隔比如 123,345,678</param>
        /// <returns></returns>
        public SimbaInsightCatsGetResponse TaobaoSimbaInsightCatsGet(TopSession session, string categoryIds)
        {
            var req = new SimbaInsightCatsGetRequest { Nick = session.UserName, CategoryIds = categoryIds };
            return _client.Execute(req, session.TopSessions);
        }

        /// <summary>
        /// 取得线上用户的信息
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="nick"></param>
        /// <returns></returns>
        public UserGetResponse GetUserResponse(string session, string nick)
        {
            var req = new UserGetRequest();
            req.Fields = "user_id,nick,seller_credit";
            req.Nick = nick;
            UserGetResponse response = _client.Execute(req, session);
            return response;
        }

        /// <summary>
        /// taobao.simba.rpt.adgroupkeywordbase.get 推广组下的词基础报表数据查询(明细数据不分类型查询)
        /// </summary>
        public SimbaRptAdgroupkeywordbaseGetResponse TaobaoSimbaRptAdgroupkeywordbaseGet(TopSession session,
                                                                             long campaignId,
                                                                             long adgroupId,
                                                                             string startTime,
                                                                             string endTime,
                                                                             string searchType,
                                                                             long pageSize,
                                                                             long pageNo,
                                                                             string source)
        {
            var req = new SimbaRptAdgroupkeywordbaseGetRequest { Nick = session.UserName, SubwayToken = GetSubwayToken(session), CampaignId = campaignId, AdgroupId = adgroupId, StartTime = startTime, EndTime = endTime, PageNo = pageNo, PageSize = pageSize, SearchType = searchType, Source = source };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }



        /// <summary>
        /// taobao.simba.rpt.adgroupkeywordeffect.get 推广组下的词效果报表数据查询(明细数据不分类型查询)
        /// </summary>
        public SimbaRptAdgroupkeywordeffectGetResponse TaobaoSimbaRptAdgroupkeywordeffectGet(TopSession session,
                                                                            long campaignId,
                                                                            long adgroupId,
                                                                            string startTime,
                                                                            string endTime,
                                                                            string searchType,
                                                                            long pageSize,
                                                                            long pageNo,
                                                                            string source)
        {
            var req = new SimbaRptAdgroupkeywordeffectGetRequest { Nick = session.UserName, SubwayToken = GetSubwayToken(session), CampaignId = campaignId, AdgroupId = adgroupId, StartTime = startTime, EndTime = endTime, PageNo = pageNo, PageSize = pageSize, SearchType = searchType, Source = source };
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得一个推广组的所有关键词的质量得分列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId"></param>
        /// <returns></returns>
        public SimbaKeywordsQscoreGetResponse TaobaoSimbaKeywordsQscoreGet(TopSession session, long adgroupId)
        {
            SimbaKeywordsQscoreGetRequest req = new SimbaKeywordsQscoreGetRequest();
            req.Nick = session.UserName;
            req.AdgroupId = adgroupId;
            SimbaKeywordsQscoreGetResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 取得一个推广组的所有关键词和类目出价的质量得分列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="adgroupId"></param>
        /// <returns></returns>
        public SimbaKeywordscatQscoreGetResponse TaobaoSimbaKeywordscatQscoreGet(TopSession session, long adgroupId)
        {
            SimbaKeywordscatQscoreGetRequest req = new SimbaKeywordscatQscoreGetRequest();
            req.Nick = session.UserName;
            req.AdgroupId = adgroupId;
            SimbaKeywordscatQscoreGetResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="session">用户信息</param>
        /// <returns></returns>
        private string GetSubwayToken(TopSession session)
        {
            SimbaLoginAuthsignGetRequest request = new SimbaLoginAuthsignGetRequest();
            request.Nick = session.UserName;
            SimbaLoginAuthsignGetResponse response = _client.Execute(request, session.TopSessions);
            string subwayToken = "";
            if (response != null)
            {
                subwayToken = response.SubwayToken;
            }
            return subwayToken;
        }

        /// <summary>
        /// taobao.vas.subscribe.get 获取用户订购的版本信息
        /// </summary>
        public VasSubscribeGetResponse TaoBaoVasSubscribeGet(TopSession session, string articleCode)
        {
            var req = new VasSubscribeGetRequest { Nick = session.ProxyUserName, ArticleCode = articleCode };
            VasSubscribeGetResponse response = _client.Execute(req);
            return response;
        }

        /// <summary>
        /// 分时折扣
        /// </summary>
        /// <param name="session">用户Session</param>
        /// <param name="campaignId">计划ID</param>
        /// <param name="schedule">分时折扣字符串：00:00-08:00:100,08:00-24:00:85;0;0;0;0;0;0</param>
        /// <returns></returns>
        public SimbaCampaignScheduleUpdateResponse TaobaoSimbaCampaignScheduleUpdate(TopSession session, long campaignId, string schedule)
        {

            SimbaCampaignScheduleUpdateRequest req = new SimbaCampaignScheduleUpdateRequest();
            req.Nick = session.UserName;
            req.CampaignId = campaignId;
            req.Schedule = schedule;
            SimbaCampaignScheduleUpdateResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }


        /// <summary>
        /// 预估排名信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="keywordId"></param>
        /// <param name="bidwordPrice"></param>
        /// <returns></returns>
        public SimbaKeywordKeywordforecastGetResponse TaobaoSimbaKeywordKeywordforecastGet(TopSession session, long keywordId, long bidwordPrice)
        {
            var req = new SimbaKeywordKeywordforecastGetRequest();
            req.Nick = session.UserName;
            req.KeywordId = keywordId;
            req.BidwordPrice = bidwordPrice;
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.adgroups.changed.get 分页获取修改的推广组ID和修改时间
        /// </summary>
        public SimbaAdgroupsChangedGetResponse TaobaoSimbaAdgroupsChangedGet(TopSession session, DateTime start_time, long page_size, long page_no)
        {
            SimbaAdgroupsChangedGetRequest req = new SimbaAdgroupsChangedGetRequest();
            req.Nick = session.UserName;
            req.StartTime = start_time;
            req.PageSize = page_size;
            req.PageNo = page_no;
            SimbaAdgroupsChangedGetResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.keywords.changed.get 分页获取修改过的关键词ID、宝贝id、修改时间
        /// </summary>
        public SimbaKeywordsChangedGetResponse TaobaoSimbaKeywordsChangedGet(TopSession session, DateTime start_time, long page_size, long page_no)
        {
            SimbaKeywordsChangedGetRequest req = new SimbaKeywordsChangedGetRequest();
            req.Nick = session.UserName;
            req.StartTime = start_time;
            req.PageSize = page_size;
            req.PageNo = page_no;
            SimbaKeywordsChangedGetResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.rpt.custbase.get 客户账户报表基础数据对象，所有计划数据
        /// </summary>
        public SimbaRptCustbaseGetResponse TaobaoSimbaRptCustbaseGet(TopSession session, DateTime start_time, DateTime end_time, long page_size, long page_no)
        {
            SimbaRptCustbaseGetRequest req = new SimbaRptCustbaseGetRequest();
            req.SubwayToken = GetSubwayToken(session);
            req.Nick = session.ProxyUserName;
            req.StartTime = start_time.ToString("yyyy-MM-dd");
            req.EndTime = end_time.ToString("yyyy-MM-dd");
            req.PageNo = page_no;
            req.PageSize = page_size;
            req.Source = "SUMMARY";
            SimbaRptCustbaseGetResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.rpt.custeffect.get 客户账户报表效果数据对象，所有计划数据
        /// </summary>
        public SimbaRptCusteffectGetResponse TaobaoSimbaRptCusteffectGet(TopSession session, DateTime start_time, DateTime end_time, long page_size, long page_no)
        {
            SimbaRptCusteffectGetRequest req = new SimbaRptCusteffectGetRequest();
            req.SubwayToken = GetSubwayToken(session);
            req.Nick = session.ProxyUserName;
            req.StartTime = start_time.ToString("yyyy-MM-dd");
            req.EndTime = end_time.ToString("yyyy-MM-dd");
            req.PageNo = page_no;
            req.PageSize = page_size;
            req.Source = "SUMMARY";
            SimbaRptCusteffectGetResponse response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.insight.toplevelcats.get 获取全部类目，一级类目
        /// </summary>
        public SimbaInsightToplevelcatsGetResponse TaobaoSimbaInsightToplevelcatsGet(TopSession session)
        {
            SimbaInsightToplevelcatsGetRequest req = new SimbaInsightToplevelcatsGetRequest();
            req.Nick = session.UserName;
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.campaign.schedule.get 获取分时折扣设置
        /// </summary>
        public SimbaCampaignScheduleGetResponse TaobaoSimbaCampaignScheduleGet(TopSession session, long campaignId)
        {
            SimbaCampaignScheduleGetRequest req = new SimbaCampaignScheduleGetRequest();
            req.Nick = session.UserName;
            req.CampaignId = campaignId;
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.campaign.area.get 获取计划的投放地域
        /// </summary>
        public SimbaCampaignAreaGetResponse TaobaoSimbaCampaignAreaGet(TopSession session, long campaignId)
        {
            SimbaCampaignAreaGetRequest req = new SimbaCampaignAreaGetRequest();
            req.Nick = session.UserName;
            req.CampaignId = campaignId;
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }

        /// <summary>
        /// taobao.simba.campaign.platform.get 取得一个推广计划的投放平台设置
        /// </summary>
        public SimbaCampaignPlatformGetResponse TaobaoSimbaCampaignPlatformGet(TopSession session, long campaignId)
        {
            SimbaCampaignPlatformGetRequest req = new SimbaCampaignPlatformGetRequest();
            req.Nick = session.UserName;
            req.CampaignId = campaignId;
            var response = _client.Execute(req, session.TopSessions);
            return response;
        }
    }
}