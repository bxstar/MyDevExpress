using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaoBaoDataServer.WinClientData.Model;
using log4net;
using Top.Api.Domain;
using Top.Api.Response;
using NetServ.Net.Json;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using TaoBaoDataServer.WinClientData.BusinessLayer;
using iclickpro.AccessCommon;


namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// DevExpress控件绑定数据，暂未实现变动的数据颜色标识
    /// </summary>
    public partial class FrmAPITest : MyDockContent
    {
        private static log4net.ILog logger = LogManager.GetLogger("Logger");
        BusinessTaobaoApiHandler taobaoApiHandler = new BusinessTaobaoApiHandler();
        BusinessAdgroupHandler adgroupHandler = new BusinessAdgroupHandler();
        BusinessBatchHandler batchHandler = new BusinessBatchHandler();
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();
        BusinessUserHandler userHandler = new BusinessUserHandler();
        BusinessCampaignHandler campaignHandler = new BusinessCampaignHandler();
        BusinessReportHandler reportHandler = new BusinessReportHandler();

        IOutPut frmOutPut;
        MainForm frmMain;

        /// <summary>
        /// 选中的推广计划
        /// </summary>
        Campaign selectedCampaign = new Campaign();

        /// <summary>
        /// 选中的推广组
        /// </summary>
        ADGroup selectedAdGroup = null;

        /// <summary>
        /// 所有需要优化用户
        /// </summary>
        List<TopSession> lstUser = new List<TopSession>();

        /// <summary>
        /// 选中的用户
        /// </summary>
        TopSession selectedUser = null;

        public FrmAPITest()
        {
            InitializeComponent();
        }

        public FrmAPITest(IOutPut _frmOutPut, MainForm mainFrm)
        {
            frmOutPut = _frmOutPut;
            frmMain = mainFrm;
            InitializeComponent();
        }

        private void FrmAPITest_Load(object sender, EventArgs e)
        {
            chkKeywordRptRecentDays.Checked = true;
            chkAdgroupRptRecentDays.Checked = true;
            //txtNickName.Text = Config.UserName;
            //txtSession.Text = Config.TopSessions;
            //txtNickName.Text = "tp_世奇广告";
            //txtNickName.Text = "365style";
            //txtSession.Text = "61019215373bc02fd52c6184b07e1166003464abfeaf0f01077912345";
            txtNickName.Text = "dongyangerba";
            txtSession.Text = "6101213128cd7d38043e51b45cZZ98cceb63c259e80106c906597473";
            txtArticleCode.Text = Config.ArticleCode;
            //txtNickName.Text = "卓尚数码";
            //txtSession.Text = "61024253f01dda55629ab21465a819ebc12d96d3af049cd859748204";

            if (Config.AppKey == "21596194")
                cbxApp.SelectedIndex = 0;
            else if (Config.AppKey == "12341041")
                cbxApp.SelectedIndex = 1;
            else if (Config.AppKey == "12209394")
                cbxApp.SelectedIndex = 2;
                
            gridViewUser.IndicatorWidth = 50;
            gridViewAdgroup.IndicatorWidth = gridViewKeyword.IndicatorWidth = gridViewKeywordRpt.IndicatorWidth = gridViewCampaignRpt.IndicatorWidth = gridViewAdgroupRpt.IndicatorWidth = gridViewCreativeRpt.IndicatorWidth = 30;

            //显示行号
            gridViewUser.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewAdgroup.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewKeyword.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewKeywordRpt.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewCampaignRpt.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewAdgroupRpt.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewCreativeRpt.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);

            //排序完成后显示第一行
            gridViewUser.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewAdgroup.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewKeyword.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewKeywordRpt.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewCampaignRpt.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewAdgroupRpt.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewCreativeRpt.EndSorting += new EventHandler(gridViewEndSorting);
        }

        private TopSession GetSession()
        {
            TopSession session = new TopSession();
            session.UserName = txtNickName.Text;
            session.ProxyUserName = txtNickName.Text;
            session.TopSessions = txtSession.Text;
            return session;
        }

        private void btnGetToken_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();

            var authsignGetRes = taobaoApiHandler.TaobaoSimbaLoginAuthsignGet(session);
            if (authsignGetRes.IsError)
            {
                logger.Error("获取SubwayToken失败，" + authsignGetRes.Body);
                MessageBox.Show("获取失败：" + authsignGetRes.Body);
            }
            else
            {
                txtToken.Text = authsignGetRes.SubwayToken;
            }
        }

        private void btnGetCampaign_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            List<Campaign> lst = campaignHandler.GetCampaignOnline(session);
            dgvCampaign.DataSource = lst;
        }

        private void btnGetMajorCampaign_Click(object sender, EventArgs e)
        {
            TopSession session = userHandler.GetUserSession(txtNickName.Text.Trim());
            List<EntityCampaign> lst = campaignHandler.GetCampaign(session.UserID);
            dgvCampaign.DataSource = lst;
        }

        private void btnGetBudget_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            long result = campaignHandler.GetCampaignBudget(session, selectedCampaign.CampaignId);
            txtBudget.Text = result.ToString();
        }

        private void btnGetAdGroup_Click(object sender, EventArgs e)
        {
            gridControlAdgroup.DataSource = null;
            TopSession session = GetSession();
            List<ADGroup> result = adgroupHandler.GetAdgroupOnline(session, selectedCampaign.CampaignId);
            gridControlAdgroup.DataSource = result;
        }

        private void btnGetKeyword_Click(object sender, EventArgs e)
        {
            gridControlKeyword.DataSource = null;
            TopSession session = GetSession();
            List<Keyword> lstKeyword = new List<Keyword>();
            bool result = keywordHandler.GetKeywordOnline(session, selectedAdGroup.AdgroupId, ref lstKeyword);
            if (result)
            {
                var lstToDisplay = (from a in lstKeyword
                                    select new
                                    {
                                        a.Word,
                                        a.MaxPrice,
                                        a.Qscore,
                                        a.MatchScope,
                                        a.ModifiedTime,
                                        a.CreateTime,
                                        a.AuditStatus,
                                        a.AuditDesc,
                                        a.KeywordId,
                                        a.AdgroupId,
                                        a.CampaignId,
                                        a.IsDefaultPrice,
                                        a.IsGarbage,
                                        a.Nick
                                    }).ToList();
                gridControlKeyword.DataSource = lstToDisplay;
            }
        }

        private void btnGetQscore_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            if (chkGetCatQScore.Checked)
            {
                string strCatMatchQscore = string.Empty;
                List<KeywordQscore> result = GetQscore(session, selectedAdGroup.AdgroupId, ref strCatMatchQscore);
                txtCatmatchQscore.Text = strCatMatchQscore;
                dgvQscore.DataSource = new SortableBindingList<KeywordQscore>(result);
            }
            else
            {
                var response = taobaoApiHandler.TaobaoSimbaKeywordsQscoreGet(session, selectedAdGroup.AdgroupId);
                dgvQscore.DataSource = new SortableBindingList<KeywordQscore>(response.KeywordQscoreList);
            }
        }

        private void btnGetKeywordRpt_Click(object sender, EventArgs e)
        {//获取选中推广组的关键词报表
            gridControlKeywordRpt.DataSource = null;
            TopSession session = GetSession();
            int intReportDays = Convert.ToInt32(txtReportDays.Text.Trim());
            List<EntityKeywordRpt> lstBase = null; List<EntityKeywordRpt> lstEffect = null;
            if (chkKeywordRptRecentDays.Checked)
            {
                lstBase = keywordHandler.DownLoadKeywordBaseReport(session, selectedAdGroup.CampaignId, selectedAdGroup.AdgroupId, intReportDays);
                lstEffect = keywordHandler.DownLoadKeywordEffectReport(session, selectedAdGroup.CampaignId, selectedAdGroup.AdgroupId, intReportDays);
            }
            else if (chkKeywordRptDtp.Checked)
            {
                lstBase = keywordHandler.DownLoadKeywordBaseReport(session, selectedAdGroup.CampaignId, selectedAdGroup.AdgroupId, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"));
                lstEffect = keywordHandler.DownLoadKeywordEffectReport(session, selectedAdGroup.CampaignId, selectedAdGroup.AdgroupId, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"));
            }

            List<EntityKeywordRpt> lstAll = lstBase.Union(lstEffect).ToList();

            List<EntityKeywordRpt> data = (from a in lstAll
                                           group a by new { a.date, a.keywordstr } into b
                                           select new EntityKeywordRpt()
                                           {
                                               date = b.Key.date,
                                               campaignid = b.First().campaignid,
                                               adgroupid = b.First().adgroupid,
                                               keywordid = b.First().keywordid,
                                               keywordstr = b.Key.keywordstr,
                                               impressions = b.Sum(c => c.impressions),
                                               click = b.Sum(c => c.click),
                                               cost = b.Sum(c => c.cost),
                                               ctr = b.Sum(c => c.impressions) == 0 ? 0M : Math.Round(b.Sum(c => c.click) * 100M / b.Sum(c => c.impressions), 2),
                                               cpc = b.Sum(c => c.click) == 0 ? 0M : Math.Round(b.Sum(c => c.cost) / b.Sum(c => c.click), 2),
                                               avgpos = b.Sum(c => c.avgpos),
                                               directpay = b.Sum(c => c.directpay),
                                               indirectpay = b.Sum(c => c.indirectpay),
                                               roi = b.Sum(c => c.cost) == 0M ? 0M : Math.Round(b.Sum(c => c.directpay + c.indirectpay) / b.Sum(c => c.cost), 2),
                                               directpaycount = b.Sum(c => c.directpaycount),
                                               indirectpaycount = b.Sum(c => c.indirectpaycount),
                                               favitemcount = b.Sum(c => c.favitemcount),
                                               favshopcount = b.Sum(c => c.favshopcount)
                                           }).ToList();
            gridControlKeywordRpt.DataSource = data;
        }


        private void btnGetAllKeywordRpt_Click(object sender, EventArgs e)
        {//获取所有推广组的关键词报表
            gridControlKeywordRpt.DataSource = null;
            List<EntityKeywordRpt> lstAll = new List<EntityKeywordRpt>();
            TopSession session = GetSession();
            int intReportDays = Convert.ToInt32(txtReportDays.Text.Trim());

            string cacheKey = string.Empty;
            if (chkKeywordRptRecentDays.Checked)
            {
                cacheKey = string.Format("action:{0},username:{1},para:{2}", "keywordrpt", session.UserName, intReportDays);
            }
            else
            {
                cacheKey = string.Format("action:{0},username:{1},para:{2},{3}", "keywordrpt", session.UserName, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"));
            }

            object cacheValue = MyCache.GetLocalCache(cacheKey);
            if (cacheValue == null || !chkIsUseLocalCache.Checked)
            {
                for (int i = 0; i < gridViewAdgroup.DataRowCount; i++)
                {
                    ADGroup item = gridViewAdgroup.GetRow(gridViewAdgroup.GetRowHandle(i)) as ADGroup;
                    if (item != null)
                    {
                        List<EntityKeywordRpt> lstBase = null; List<EntityKeywordRpt> lstEffect = null;
                        if (chkKeywordRptRecentDays.Checked)
                        {
                            if (chkSoureDistinguish.Checked)
                            {//区分站内站外
                                MessageBox.Show("API不支持暂时无法实现");
                                //lstBase = keywordHandler.DownLoadKeywordBaseReport(session, item.CampaignId, item.AdgroupId, intReportDays, "1,2");
                                //lstEffect = keywordHandler.DownLoadKeywordEffectReport(session, item.CampaignId, item.AdgroupId, intReportDays, "1,2");
                            }
                            else
                            {//不区分站内站外
                                lstBase = keywordHandler.DownLoadKeywordBaseReport(session, item.CampaignId, item.AdgroupId, intReportDays);
                                lstEffect = keywordHandler.DownLoadKeywordEffectReport(session, item.CampaignId, item.AdgroupId, intReportDays);
                            }
                        }
                        else if (chkKeywordRptDtp.Checked)
                        {
                            if (chkSoureDistinguish.Checked)
                            {//区分站内站外
                                MessageBox.Show("API不支持暂时无法实现");
                                //lstBase = keywordHandler.DownLoadKeywordBaseReport(session, item.CampaignId, item.AdgroupId, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"), "1,2");
                                //lstEffect = keywordHandler.DownLoadKeywordEffectReport(session, item.CampaignId, item.AdgroupId, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"), "1,2");
                            }
                            else
                            {//不区分站内站外
                                lstBase = keywordHandler.DownLoadKeywordBaseReport(session, item.CampaignId, item.AdgroupId, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"));
                                lstEffect = keywordHandler.DownLoadKeywordEffectReport(session, item.CampaignId, item.AdgroupId, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"));
                            }
                        }
                        if (lstBase != null)
                            lstAll.AddRange(lstBase);
                        if (lstEffect != null)
                            lstAll.AddRange(lstEffect);
                    }
                }
                MyCache.SetLocalCache(cacheKey, lstAll);
            }
            else
            {
                lstAll = cacheValue as List<EntityKeywordRpt>;
                MessageBox.Show("本地缓存获取");
            }

            List<EntityKeywordRpt> data = new List<EntityKeywordRpt>();
            if (chkSoureDistinguish.Checked)
            {//区分站内站外来源
                data = (from a in lstAll
                        group a by new { a.date, a.keywordstr, a.adgroupid, a.source } into b
                        select new EntityKeywordRpt()
                        {
                            date = b.Key.date,
                            campaignid = b.First().campaignid,
                            adgroupid = b.Key.adgroupid,
                            keywordid = b.First().keywordid,
                            keywordstr = b.Key.keywordstr,
                            source = b.Key.source,
                            impressions = b.Sum(c => c.impressions),
                            click = b.Sum(c => c.click),
                            cost = b.Sum(c => c.cost),
                            ctr = b.Sum(c => c.impressions) == 0 ? 0M : Math.Round(b.Sum(c => c.click) * 100M / b.Sum(c => c.impressions), 2),
                            cpc = b.Sum(c => c.click) == 0 ? 0M : Math.Round(b.Sum(c => c.cost) / b.Sum(c => c.click), 2),
                            avgpos = b.Sum(c => c.avgpos),
                            directpay = b.Sum(c => c.directpay),
                            indirectpay = b.Sum(c => c.indirectpay),
                            roi = b.Sum(c => c.cost) == 0M ? 0M : Math.Round(b.Sum(c => c.directpay + c.indirectpay) / b.Sum(c => c.cost), 2),
                            directpaycount = b.Sum(c => c.directpaycount),
                            indirectpaycount = b.Sum(c => c.indirectpaycount),
                            favitemcount = b.Sum(c => c.favitemcount),
                            favshopcount = b.Sum(c => c.favshopcount)
                        }).ToList();
            }
            else
            {//不区分站内站外
                data = (from a in lstAll
                        group a by new { a.date, a.keywordstr, a.adgroupid } into b
                        select new EntityKeywordRpt()
                        {
                            date = b.Key.date,
                            campaignid = b.First().campaignid,
                            adgroupid = b.Key.adgroupid,
                            keywordid = b.First().keywordid,
                            keywordstr = b.Key.keywordstr,
                            impressions = b.Sum(c => c.impressions),
                            click = b.Sum(c => c.click),
                            cost = b.Sum(c => c.cost),
                            ctr = b.Sum(c => c.impressions) == 0 ? 0M : Math.Round(b.Sum(c => c.click) * 100M / b.Sum(c => c.impressions), 2),
                            cpc = b.Sum(c => c.click) == 0 ? 0M : Math.Round(b.Sum(c => c.cost) / b.Sum(c => c.click), 2),
                            directpay = b.Sum(c => c.directpay),
                            indirectpay = b.Sum(c => c.indirectpay),
                            roi = b.Sum(c => c.cost) == 0M ? 0M : Math.Round(b.Sum(c => c.directpay + c.indirectpay) / b.Sum(c => c.cost), 2),
                            directpaycount = b.Sum(c => c.directpaycount),
                            indirectpaycount = b.Sum(c => c.indirectpaycount),
                            favitemcount = b.Sum(c => c.favitemcount),
                            favshopcount = b.Sum(c => c.favshopcount)
                        }).ToList();
            }


            gridControlKeywordRpt.DataSource = data;

        }

        /// <summary>
        /// 线上，获取推广组下关键词的质量得分
        /// </summary>
        private List<KeywordQscore> GetQscore(TopSession session, long adgroupId, ref string strCatmatchQscore)
        {
            var keywordQscoreList = taobaoApiHandler.TaobaoSimbaKeywordscatQscoreGet(session, adgroupId);

            if (keywordQscoreList != null && keywordQscoreList.Qscore != null)
            {
                strCatmatchQscore = keywordQscoreList.Qscore.CatmatchQscore;
                return keywordQscoreList.Qscore.KeywordQscoreList;
            }

            return null;
        }

        private void dgvCampaign_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                Campaign cam = dgvCampaign.Rows[e.RowIndex].DataBoundItem as Campaign;
                if (cam != null)
                {
                    selectedCampaign = cam;
                    txtCampaignId.Text = selectedCampaign.CampaignId.ToString();
                }
                else
                {
                    EntityCampaign majorCam = dgvCampaign.Rows[e.RowIndex].DataBoundItem as EntityCampaign;
                    if (majorCam != null)
                    {
                        selectedCampaign = new Campaign() { CampaignId = majorCam.campaignid, Title = majorCam.title, OnlineStatus = majorCam.onlinestatus };
                        txtCampaignId.Text = selectedCampaign.CampaignId.ToString();
                    }
                }
            }
        }

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

        private void btnGetOneItem_Click(object sender, EventArgs e)
        {
            // 获取商品信息，不包含销量信息，已推广宝贝界面不需要显示，不保存销量信息，因为时刻在变
            ItemGetResponse response = taobaoApiHandler.TaobaoItemGetByNumIid(GetSession(), "pic_url,title", 17126603843);
        }

        private void btnGetBalance_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            var result = taobaoApiHandler.TaobaoSimbaAccountBalanceGet(session);
            txtBalance.Text = result.Balance;
        }

        private void btnSetBudget_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            double result = campaignHandler.SetCampaignBudget(session, selectedCampaign.CampaignId, Convert.ToInt64(txtBudget.Text));
            if (txtBudget.Text == result.ToString())
            {
                MessageBox.Show("设置日限额成功");
            }
        }

        private void btnGetCampaignBaseRpt_Click(object sender, EventArgs e)
        {
            gridControlCampaignRpt.DataSource = null;
            TopSession session = GetSession();
            long campaignId = Convert.ToInt64(txtCampaignId.Text);
            string strStartDay = DateTime.Now.AddDays(0 - 30).Date.ToString("yyyy-MM-dd");
            string strEndDay = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
            string json = campaignHandler.DownLoadCamapginBaseReport(session, campaignId, strStartDay, strEndDay);
            List<adrptcampaignbase> lstRpt = TechNet.JsonDeserialize<List<adrptcampaignbase>>(json);
            gridControlCampaignRpt.DataSource = lstRpt;
        }

        private void btnGetCampaignEffectRpt_Click(object sender, EventArgs e)
        {
            gridControlCampaignRpt.DataSource = null;
            TopSession session = GetSession();
            long campaignId = Convert.ToInt64(txtCampaignId.Text);
            string strStartDay = DateTime.Now.AddDays(0 - 30).Date.ToString("yyyy-MM-dd");
            string strEndDay = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
            string json = campaignHandler.DownLoadCampaignEffectReport(session, campaignId, strStartDay, strEndDay);
            List<adrptcampaigneffect> lstRpt = TechNet.JsonDeserialize<List<adrptcampaigneffect>>(json);
            gridControlCampaignRpt.DataSource = lstRpt;
        }

        private void btnGetCampaignRpt_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            long campaignId = Convert.ToInt64(txtCampaignId.Text);
            List<EntityCampaignReport> lstAll = new List<EntityCampaignReport>();
            lstAll = campaignHandler.DownLoadCampaignReport(session, campaignId, 30);

            gridControlCampaignRpt.DataSource = lstAll;
        }

        private void btnGetCategory_Click(object sender, EventArgs e)
        {
            treeListCats.DataSource = null;

            string strCatIds = txtCategoryIds.Text.Trim();
            List<InsightCategoryInfoDTO> lstCatInfo = null;
            List<InsightCategoryDataDTO> lstCatData = null;
            if (strCatIds.Length == 0)
            {
                lstCatInfo = CommonHandler.GetCatsFullInfo("0", null);
            }
            else
            {
                lstCatInfo = CommonHandler.GetCatsFullInfo("1", strCatIds);
            }

            lstCatData = CommonHandler.GetCatsData(strCatIds);

            //类目信息，大盘数据合并
            List<EntityCategoryEx> lstCatEx = new List<EntityCategoryEx>();
            foreach (var itemCatInfo in lstCatInfo)
            {
                EntityCategoryEx catEx = new EntityCategoryEx() { CatId = itemCatInfo.CatId, CatLevel = itemCatInfo.CatLevel, CatName = itemCatInfo.CatName, CatPathId = itemCatInfo.CatPathId, CatPathName = itemCatInfo.CatPathName, ParentCatId = itemCatInfo.ParentCatId };
                var itemCatData = lstCatData.Find(o => o.CatId == itemCatInfo.CatId);
                if (itemCatData != null)
                {
                    catEx.Impression = itemCatData.Impression;
                    catEx.Click = itemCatData.Click;
                    catEx.Cost = itemCatData.Cost;
                    catEx.Ctr = itemCatData.Ctr;
                    catEx.Cpc = itemCatData.Cpc;
                    catEx.Roi = itemCatData.Roi;
                    catEx.Competition = itemCatData.Competition;
                    catEx.Coverage = itemCatData.Coverage;
                    catEx.Directtransaction = itemCatData.Directtransaction;
                    catEx.Directtransactionshipping = itemCatData.Directtransactionshipping;
                    catEx.Indirecttransaction = itemCatData.Indirecttransaction;
                    catEx.Indirecttransactionshipping = itemCatData.Indirecttransactionshipping;
                    catEx.Favitemtotal = itemCatData.Favitemtotal;
                    catEx.Favshoptotal = itemCatData.Favshoptotal;
                    catEx.Favtotal = itemCatData.Favtotal;
                    catEx.Transactionshippingtotal = itemCatData.Transactionshippingtotal;
                    catEx.Transactiontotal = itemCatData.Transactiontotal;
                }
                lstCatEx.Add(catEx);
            }

            treeListCats.OptionsView.ShowSummaryFooter = true;


            treeListCats.DataSource = lstCatEx;
            treeListCats.Columns["Cpc"].SummaryFooterStrFormat = "平均值：{0:n0}";
            treeListCats.Columns["Cpc"].SummaryFooter = DevExpress.XtraTreeList.SummaryItemType.Average;
        }

        private void btnGetAllUser_Click(object sender, EventArgs e)
        {
            gridControlUser.DataSource = null;
            lstUser = userHandler.GetUserInfo(null).Where(o => o.UserName == o.ProxyUserName).ToList();
            gridControlUser.DataSource = lstUser;
        }

        private void btnInvertSelectionUser_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridViewUser.DataRowCount; i++)
            {
                TopSession user = gridViewUser.GetRow(gridViewUser.GetRowHandle(i)) as TopSession;
                user.IsEnableMajorization = !user.IsEnableMajorization;
            }

            gridControlUser.Refresh();
        }

        private void btnSelectionAllUser_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridViewUser.DataRowCount; i++)
            {
                TopSession user = gridViewUser.GetRow(gridViewUser.GetRowHandle(i)) as TopSession;
                user.IsEnableMajorization = true;
            }

            gridControlUser.Refresh();
        }

        private void btnSelectioned_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridViewUser.DataRowCount; i++)
            {
                TopSession user = gridViewUser.GetRow(gridViewUser.GetRowHandle(i)) as TopSession;
                user.IsEnableMajorization = false;
            }

            for (int i = 0; i < gridViewUser.SelectedRowsCount; i++)
            {
                var selectedRow = gridViewUser.GetRow(gridViewUser.GetRowHandle(i));
                TopSession user = selectedRow as TopSession;
                user.IsEnableMajorization = true;
            }
            gridControlUser.Refresh();

            MessageBox.Show("选中" + lstUser.Where(o => o.IsEnableMajorization == true).Count().ToString() + "条记录");
        }

        private void btnGetTopSession_Click(object sender, EventArgs e)
        {
            TopSession user = userHandler.GetUserSession(txtNickName.Text.Trim());
            if (user != null)
            {
                txtSession.Text = user.TopSessions;
            }
        }

        private void chkKeywordRptRecentDays_CheckedChanged(object sender, EventArgs e)
        {
            gbxRecentDays.Enabled = true;
            gbxDtp.Enabled = false;
            chkKeywordRptDtp.Checked = false;
        }

        private void chkKeywordRptDtp_CheckedChanged(object sender, EventArgs e)
        {
            gbxRecentDays.Enabled = false;
            gbxDtp.Enabled = true;
            chkKeywordRptRecentDays.Checked = false;
        }

        private void 查看推广组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EntityKeywordRpt rpt = gridViewKeywordRpt.GetFocusedRow() as EntityKeywordRpt;
            SelectedTab("获取推广组及宝贝");
            Boolean isFind = false;
            gridViewAdgroup.ClearSelection();
            for (int i = 0; i < gridViewAdgroup.DataRowCount; i++)
            {
                ADGroup adgroupItem = gridViewAdgroup.GetRow(gridViewAdgroup.GetRowHandle(i)) as ADGroup;
                if (adgroupItem.AdgroupId == rpt.adgroupid)
                {
                    isFind = true;
                    gridViewAdgroup.FocusedRowHandle = gridViewAdgroup.GetRowHandle(i);
                    gridViewAdgroup.SelectRow(gridViewAdgroup.FocusedRowHandle);
                    selectedAdGroup = adgroupItem;
                    txtAdGroupId.Text = adgroupItem.AdgroupId.ToString();
                    txtItemId.Text = adgroupItem.NumIid.ToString();
                }
            }
            if (!isFind)
            {
                MessageBox.Show("未找到推广组：" + rpt.adgroupid);
            }
        }

        /// <summary>
        /// 选中某一个Tabpage
        /// </summary>
        private void SelectedTab(string strName)
        {
            foreach (TabPage item in tabControl1.TabPages)
            {
                if (item.Text == strName)
                {
                    tabControl1.SelectedTab = item;
                    return;
                }
            }
        }

        private void btnAdgroupChange_Click(object sender, EventArgs e)
        {

            TopSession session = GetSession();
            var response = taobaoApiHandler.TaobaoSimbaAdgroupsChangedGet(session, new DateTime(2013, 10, 24, 8, 0, 0), 200, 1);
            List<ADGroup> lstAdgroupChange = response.Adgroups.AdgroupList;
            for (int i = 0; i < gridViewAdgroup.DataRowCount; i++)
            {
                //dgvAdgroup.Rows[i].DefaultCellStyle.BackColor = dgvAdgroup.Rows[i].DefaultCellStyle.
                ADGroup item = gridViewAdgroup.GetRow(gridViewAdgroup.GetRowHandle(i)) as ADGroup;
                if (item != null)
                {
                    if (lstAdgroupChange.Find(o => o.AdgroupId == item.AdgroupId) != null)
                    {
                        //gridViewAdgroup.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                        //gridControlAdgroup.
                    }
                }
            }


        }

        private void btnKeywordChange_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            var response = taobaoApiHandler.TaobaoSimbaKeywordsChangedGet(session, new DateTime(2013, 10, 24, 8, 0, 0), 200, 1);
            List<Keyword> lstChange = response.Keywords.KeywordList;
            for (int i = 0; i < gridViewKeyword.DataRowCount; i++)
            {
                //dgvAdgroup.Rows[i].DefaultCellStyle.BackColor = dgvAdgroup.Rows[i].DefaultCellStyle.
                Keyword item = gridViewKeyword.GetRow(gridViewKeyword.GetRowHandle(i)) as Keyword;
                if (item != null)
                {
                    if (lstChange.Find(o => o.KeywordId == item.KeywordId) != null)
                    {
                        //dgvKeyword.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    }
                }
            }
        }

        private void btnUpdateCampaign_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            long campaign = Convert.ToInt64(txtCampaignId.Text);
            var response = taobaoApiHandler.TaobaoSimbaCampaignUpdate(session, campaign, txtNewCampaignName.Text, "online");
            if (response.IsError)
            {
                MessageBox.Show(response.Body);
            }
        }

        private void 设置黑白名单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopSession session = userHandler.GetUserSession(txtNickName.Text.Trim());
            session.CampaignId = Convert.ToInt64(txtCampaignId.Text);
            long adgroupId = Convert.ToInt64(txtAdGroupId.Text);
            FrmSetKeywordCustom frm = new FrmSetKeywordCustom();
            frm.session = session;
            frm.adgroupId = adgroupId;
            frm.Show();
        }

        private void 同步关键词ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopSession session = userHandler.GetUserSession(txtNickName.Text.Trim());
            long adgroupId = Convert.ToInt64(txtAdGroupId.Text);

            List<Keyword> keywordList = new List<Keyword>();
            // 从线上获取一个推广组的关键词，可以优化（只需下面的质量得分，而后更新数据库中关键词的质量得分即可，不需要下载关键词）
            bool resultGetKeywordOnline = keywordHandler.GetKeywordOnline(session, adgroupId, ref keywordList);

            if (resultGetKeywordOnline)
            {
                frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},从线上下载推广组的关键词成功,关键词数{2}", session.UserName, adgroupId, keywordList.Count));
                // 删除数据库中的关键词
                keywordHandler.DeleteKeywordByAdgroupId(session.UserID, adgroupId);
                // 如果关键词存在的情况
                if (keywordList.Count > 0)
                {
                    // 将关键词存入本地数据库
                    keywordHandler.AddKeyword(session.UserID, keywordList);
                    frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载关键词数据入库完成", session.UserName, adgroupId));
                }
            }
            else
            {
                frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},从线上下载推广组的关键词失败", session.UserName, adgroupId));
            }
        }

        private void 同步推广组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedCampaign.CampaignId == 0)
            {
                MessageBox.Show("请先选中计划");
                return;
            }
            TopSession session = userHandler.GetUserSession(txtNickName.Text.Trim());
            if (campaignHandler.GetCampaign(session.UserID).Find(o => o.campaignid == selectedCampaign.CampaignId) == null)
            {
                MessageBox.Show("数据库中没有此计划，请重新选择需要同步推广组的计划");
                return;
            }

            List<ADGroup> lstAdgroup = adgroupHandler.GetAdgroupOnline(session, selectedCampaign.CampaignId);
            frmOutPut.OutPutMsg(string.Format("用户{0},计划:{1},从线上下载推广组成功，推广组数{2}，其中状态为推广中：{3}", session.UserID, selectedCampaign.CampaignId, lstAdgroup.Count, lstAdgroup.Where(o => o.OnlineStatus == "online").Count()));
            adgroupHandler.DeleteAdgroup(session.UserID);
            frmOutPut.OutPutMsg("本地删除推广组成功");
            foreach (var item in lstAdgroup)
            {
                adgroupHandler.AddAdgroup(session.UserID, item);
            }
            frmOutPut.OutPutMsg("同步成功");
        }

        private void 查询排名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dynamic k = gridViewKeyword.GetFocusedRow() as dynamic;
            if (k != null)
            {
                FrmSetKeywordRank frm = new FrmSetKeywordRank();
                frm.Current_Keyword = k.Word;
                frm.Current_KeywordId = k.KeywordId;
                frm.Current_NickName = txtNickName.Text;
                frm.Current_ItemId = Convert.ToInt64(txtItemId.Text);
                frm.Current_Price = k.MaxPrice;
                frm.Current_MatchScope = k.MatchScope;
                frm.Show();
            }
            else
                MessageBox.Show("请选中关键词");
        }

        /// <summary>
        /// 获取消费代码
        /// </summary>
        private void btnGetCode_Click(object sender, EventArgs e)
        {
            string articleCode = txtArticleCode.Text.Trim();

            EntityUserSubscribe result = userHandler.GetUserSubscribe(GetSession(), articleCode);
            txtItemCodes.Text = result.item_codes;
            txtDeadLine.Text = result.dead_line;
        }

        /// <summary>
        /// 读取托管状态
        /// </summary>
        private void btnCheckTuoguan_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridViewUser.DataRowCount; i++)
            {
                TopSession user = gridViewUser.GetRow(gridViewUser.GetRowHandle(i)) as TopSession;
                // 循环验证订购关系，不存在则设置为过期用户
                string itemCodes = userHandler.GetUserSubscribe(user, Config.ArticleCode).item_codes;
                frmOutPut.OutPutMsg(string.Format("用户{0},ID{1},获取的收费代码为{2}", user.ProxyUserName, user.UserID, itemCodes));
                //!string.IsNullOrEmpty(itemCodes) && 
                if (itemCodes != Config.ItemCode)
                {
                    //userHandler.UpdateUserState(user.UserID, 0);
                    user.Expire = true;
                    user.DeleteFlag = false;
                }
                else
                {
                    //userHandler.UpdateUserState(user.UserID, 1);
                    user.Expire = false;
                    user.DeleteFlag = true;
                }
            }
            gridControlUser.DataSource = lstUser;
        }

        private void btnDelKeyword_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gridViewKeyword.GetSelectedRows();
            if (selectedRows.Count() == 0) return;

            DialogResult dialogResult = MessageBox.Show("是否删除关键词？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.OK)
            {
                List<long> lstKeywordId = new List<long>();
                foreach (var rowIndex in selectedRows)
                {
                    dynamic k = gridViewKeyword.GetRow(rowIndex) as dynamic;
                    lstKeywordId.Add(k.KeywordId);
                }

                var response = keywordHandler.DeleteKeywordOnline(GetSession(), selectedCampaign.CampaignId, lstKeywordId);
                if (response.IsError)
                {
                    MessageBox.Show(response.ErrorMessage + "\r\n" + response.SubErrorMessage);
                }
                else
                {
                    MessageBox.Show("关键词删除成功");
                    gridViewKeyword.DeleteSelectedRows();
                }
            }

        }

        private void btnUpdateKeyword_Click(object sender, EventArgs e)
        {//选中词改价
            List<Keyword> lstKeywordNewPrice = new List<Keyword>();
            int[] selectedRows = gridViewKeyword.GetSelectedRows();
            if (selectedRows.Count() == 0) return;
            foreach (var rowIndex in selectedRows)
            {
                dynamic k = gridViewKeyword.GetRow(rowIndex) as dynamic;
                Keyword newKeywordPrice = new Keyword();
                newKeywordPrice.KeywordId = k.KeywordId;
                long newPrice = 0;
                if (txtNewPrice.Text.StartsWith("+") || txtNewPrice.Text.StartsWith("-"))
                {//调增或调减
                    if (txtNewPrice.Text.StartsWith("+"))
                    {//调增
                        if (txtNewPrice.Text.EndsWith("%"))
                        {
                            newPrice = k.MaxPrice + Convert.ToInt64(k.MaxPrice * Convert.ToDecimal(txtNewPrice.Text.TrimEnd('%')) / 100);
                        }
                        else
                        {
                            newPrice = k.MaxPrice + Convert.ToInt64(txtNewPrice.Text);
                        }
                    }
                    else
                    {//调减 
                        if (txtNewPrice.Text.EndsWith("%"))
                        {
                            newPrice = k.MaxPrice - Convert.ToInt64(k.MaxPrice * Convert.ToDecimal(txtNewPrice.Text.TrimEnd('%')) / 100);
                        }
                        else
                        {
                            newPrice = k.MaxPrice - Convert.ToInt64(txtNewPrice.Text);
                        }
                    }
                }
                else
                {//调成固定值
                    if (txtNewPrice.Text.EndsWith("%"))
                    {
                        newPrice = Convert.ToInt64(k.MaxPrice * Convert.ToDecimal(txtNewPrice.Text.TrimEnd('%')) / 100);
                    }
                    else
                    {
                        newPrice = Convert.ToInt64(txtNewPrice.Text);
                    }
                }
                newKeywordPrice.MaxPrice = newPrice;
                newKeywordPrice.MatchScope = k.MatchScope;
                newKeywordPrice.IsDefaultPrice = false;
                lstKeywordNewPrice.Add(newKeywordPrice);

                //string str = string.Format("{0},{1},{2},{3}", newKeywordPrice.KeywordId, newKeywordPrice.MaxPrice, newKeywordPrice.MatchScope, newKeywordPrice.IsDefaultPrice);
                //MessageBox.Show(str);
            }
            TopSession session = GetSession();
            var response = keywordHandler.ModifyKeywordOnline(session, lstKeywordNewPrice);
            if (response.IsError)
            {
                MessageBox.Show(response.ErrorMessage + "\r\n" + response.SubErrorMessage);
            }
            else
            {
                MessageBox.Show("关键词改价成功");
                btnGetKeyword_Click(null, null);
            }
        }

        private void btnAddKeyword_Click(object sender, EventArgs e)
        {
            FrmAddKeyword frm = new FrmAddKeyword();
            frm.CurrentSession = GetSession();
            frm.SetCurrentAdgrupId(selectedAdGroup.AdgroupId);
            frm.Show();
        }

        private void btnAdgroupsAddKeyword_Click(object sender, EventArgs e)
        {
            FrmAddKeyword frm = new FrmAddKeyword();
            frm.CurrentSession = GetSession();
            int[] selectedRows = gridViewAdgroup.GetSelectedRows();
            if (selectedRows.Count() == 0) return;

            List<long> lstAdgroupId = new List<long>();
            foreach (var rowIndex in selectedRows)
            {
                ADGroup adgroup = gridViewAdgroup.GetRow(rowIndex) as ADGroup;
                lstAdgroupId.Add(adgroup.AdgroupId);
            }

            frm.SetCurrentAdgrupIds(string.Join(",", lstAdgroupId));
            frm.Show();
        }

        private void gridViewUser_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.RowHandle != -1)
            {
                TopSession session = gridViewUser.GetRow(e.RowHandle) as TopSession;
                if (session != null)
                {
                    txtNickName.Text = session.ProxyUserName;
                    txtSession.Text = session.TopSessions;
                    selectedUser = session;
                }
            }
        }

        private void gridViewAdgroup_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.RowHandle != -1)
            {
                selectedAdGroup = gridViewAdgroup.GetRow(e.RowHandle) as ADGroup;
                txtAdGroupId.Text = selectedAdGroup.AdgroupId.ToString();
                txtItemId.Text = selectedAdGroup.NumIid.ToString();
            }
        }

        private void btnNewAdgrougForm_Click(object sender, EventArgs e)
        {
            object reportData = gridControlAdgroup.DataSource;
            FrmReport frm = new FrmReport(string.Format("计划：{0}，ID：{1}，推广组列表", selectedCampaign.Title, selectedCampaign.CampaignId));
            frm.ReportData = reportData;
            frm.Show();
        }

        private void btnNewKeywordForm_Click(object sender, EventArgs e)
        {
            object reportData = gridControlKeyword.DataSource;
            FrmReport frm = new FrmReport(string.Format("推广组ID：{0}，关键词列表", selectedAdGroup.AdgroupId));
            frm.ReportData = reportData;
            frm.Show();
        }

        private void btnNewKeywordRptForm_Click(object sender, EventArgs e)
        {
            string formTitle = string.Empty;
            List<EntityKeywordRpt> reportData = gridControlKeywordRpt.DataSource as List<EntityKeywordRpt>;
            if (reportData != null)
                formTitle = string.Format("计划：{0}，推广组ID：{1}，关键词报表", selectedCampaign.Title, string.Join(",", reportData.Select(o => o.adgroupid).Distinct()));
            else
                return;
            FrmReport frm = new FrmReport(formTitle);
            frm.ReportData = reportData;
            frm.Show();
        }

        private void btnNewCampaignRptForm_Click(object sender, EventArgs e)
        {
            object reportData = gridControlCampaignRpt.DataSource;
            FrmReport frm = new FrmReport(string.Format("计划：{0}，ID：{1}，计划报表", selectedCampaign.Title, selectedCampaign.CampaignId));
            frm.ReportData = reportData;
            frm.Show();
        }

        private void 网页打开宝贝ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string itemUrl = string.Format("http://item.taobao.com/item.htm?id={0}", txtItemId.Text);
            System.Diagnostics.Process.Start("iexplore.exe", itemUrl);
        }

        private void cbxApp_SelectedIndexChanged(object sender, EventArgs e)
        {//硬编码
            if (cbxApp.SelectedIndex == 0)
            {//安心代驾
                Config.AppKey = "21596194";
                Config.AppSecret = "5b6e8a9a5e1b600cc348445a24a7c846";
                Config.App_Title = "安心代驾数据分析";
                Config.ConnectionAP = "packet size=4096;user id=sa; PWD ='ShiQiTaoKuaiChe20120302'; data source=.;persist security info=False;initial catalog='AutomaticDrive_Mobile';Connect Timeout=30000;Asynchronous Processing=true;";
                taobaoApiHandler.SetTopClient(Config.C_Url, Config.AppKey, Config.AppSecret, "json");
                txtArticleCode.Text = Config.ArticleCode = "FW_GOODS-1897024";
                Config.ItemCode = "FW_GOODS-1897024-v2";
                Config.MajorizationPath = @"D:\自动开车\Majorization_Mobile\Majorization.exe";
            }
            else if (cbxApp.SelectedIndex == 1)
            {//淘快词托管
                Config.AppKey = "12341041";
                Config.AppSecret = "e54f4edc5ec2a485373fadac81e4cb5f";
                Config.App_Title = "淘快词托管数据分析";
                Config.ConnectionAP = "packet size=4096;user id=sa; PWD ='ShiQiTaoKuaiChe20120302'; data source=.;persist security info=False;initial catalog='AP';Connect Timeout=30000;Asynchronous Processing=true;";
                taobaoApiHandler.SetTopClient(Config.C_Url, Config.AppKey, Config.AppSecret, "json");
                txtArticleCode.Text = Config.ArticleCode = "ts-25420";
                Config.ItemCode = "ts-25420-v4";
                Config.MajorizationPath = @"D:\淘快词\AP\Majorization\Majorization.exe";
            }
            else if (cbxApp.SelectedIndex == 2)
            {//淘快车 
                Config.AppKey = "12209394";
                Config.AppSecret = "98fbb138ef7597283d798a42cb203103";
                Config.App_Title = "淘快车数据分析";
                taobaoApiHandler.SetTopClient(Config.C_Url, Config.AppKey, Config.AppSecret, "json");
                txtArticleCode.Text = Config.ArticleCode = "ts-21434";
                Config.ItemCode = string.Empty;
                Config.MajorizationPath = string.Empty;
            }
            frmMain.SetMainTitle(Config.App_Title);
        }

        private void btnNickAndSession_Click(object sender, EventArgs e)
        {
            object data = null;
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject.GetDataPresent(DataFormats.Text))
            {
                data = dataObject.GetData(DataFormats.Text);
            }
            if (data != null)
            {
                string str = data.ToString();
                string[] separatorHead = new string[] { "\r\n" };
                string[] separatorContent = new string[] { "\t" };
                string[] sourceHead = str.Split(separatorHead, StringSplitOptions.RemoveEmptyEntries);
                if (sourceHead.Length == 2)
                {//剪切板中有标题 
                    string[] source = sourceHead[1].Split(separatorContent, StringSplitOptions.RemoveEmptyEntries);
                    if (source.Length == 2)
                    {
                        this.txtNickName.Text = source[0];
                        this.txtSession.Text = source[1];
                    }
                }
                else
                {
                    string[] source = str.Split(separatorContent, StringSplitOptions.RemoveEmptyEntries);
                    if (source.Length == 2)
                    {
                        this.txtNickName.Text = source[0];
                        this.txtSession.Text = source[1];
                    }
                }
            }

        }

        private void btnCheckAddPrice_Click(object sender, EventArgs e)
        {
            dgvCampaign.DataSource = null;
            TopSession user = userHandler.GetUserSession(txtNickName.Text.Trim());
            EntityCampaign ec = campaignHandler.GetCampaign(user.UserID).Find(o => o.campaignid == selectedCampaign.CampaignId);
            List<EntityCampaign> lst = new List<EntityCampaign>();
            lst.Add(ec);
            dgvCampaign.DataSource = lst;
            txtCampaignId.Text = ec.campaignid.ToString();

            long budget = campaignHandler.GetCampaignBudget(user, ec.campaignid);
            txtBudget.Text = budget.ToString();
            string strMsg = string.Empty;
            Boolean result = batchHandler.IsNeedAddPrice(user, ec.campaignid, budget,ref strMsg);
            if (result)
            {
                MessageBox.Show(strMsg);
            }
            else
            {
                MessageBox.Show("不用加价");
            }
        }

        private void btnGetAdgroupRpt_Click(object sender, EventArgs e)
        {
            gridControlAdgroupRpt.DataSource = null;
            TopSession user = GetSession();
            int intReportDays = Convert.ToInt32(txtAdgroupReportDays.Text.Trim());
            List<EntityAdgroupReport> lstRpt = new List<EntityAdgroupReport>();
            if (chkAdgroupRptRecentDays.Checked)
            {
                lstRpt = adgroupHandler.DownLoadAdgroupReport(user, selectedCampaign.CampaignId, selectedAdGroup.AdgroupId, intReportDays);
            }
            else if (chkAdgroupRptDtp.Checked)
            {
                lstRpt = adgroupHandler.DownLoadAdgroupReport(user, selectedCampaign.CampaignId, selectedAdGroup.AdgroupId, dtpAdgroupRptStartDay.Value.ToString("yyyy-MM-dd"), dtpAdgroupRptEndDay.Value.ToString("yyyy-MM-dd"));
            }
            gridControlAdgroupRpt.DataSource = lstRpt;
        }

        private void btnGetAllAdgroupRpt_Click(object sender, EventArgs e)
        {
            gridControlAdgroupRpt.DataSource = null;
            TopSession user = GetSession();
            int intReportDays = Convert.ToInt32(txtAdgroupReportDays.Text.Trim());
            List<EntityAdgroupReport> lstRpt = new List<EntityAdgroupReport>();
            if (chkAdgroupRptRecentDays.Checked)
            {
                lstRpt = adgroupHandler.DownLoadAdgroupReportByCampaign(user, selectedCampaign.CampaignId, intReportDays);
            }
            else if (chkAdgroupRptDtp.Checked)
            {
                lstRpt = adgroupHandler.DownLoadAdgroupReportByCampaign(user, selectedCampaign.CampaignId, dtpAdgroupRptStartDay.Value.ToString("yyyy-MM-dd"), dtpAdgroupRptEndDay.Value.ToString("yyyy-MM-dd"));
            }
            gridControlAdgroupRpt.DataSource = lstRpt;
        }

        private void chkAdgroupRptRecentDays_CheckedChanged(object sender, EventArgs e)
        {
            gbxAdgroupRptRecentDays.Enabled = true;
            gbxAdgroupRptDtp.Enabled = false;
            chkAdgroupRptDtp.Checked = false;
        }

        private void chkAdgroupRptDtp_CheckedChanged(object sender, EventArgs e)
        {
            gbxAdgroupRptRecentDays.Enabled = false;
            gbxAdgroupRptDtp.Enabled = true;
            chkAdgroupRptRecentDays.Checked = false;
        }

        private void btnNewAdgroupRptForm_Click(object sender, EventArgs e)
        {
            object reportData = gridControlAdgroupRpt.DataSource;
            FrmReport frm = new FrmReport(string.Format("计划：{0}，ID：{1}，推广组报表", selectedCampaign.Title, selectedCampaign.CampaignId));
            frm.ReportData = reportData;
            frm.Show();
        }

        private void btnGetCreativeRpt_Click(object sender, EventArgs e)
        {
            gridControlCreativeRpt.DataSource = null;
            TopSession user = GetSession();
            int intReportDays = Convert.ToInt32(txtCreativeReportDays.Text.Trim());
            List<EntityCreativeReport> lstRpt = new List<EntityCreativeReport>();
            if (chkCreativeRptRecentDays.Checked)
            {
                lstRpt = adgroupHandler.DownLoadCreativeReport(user, selectedCampaign.CampaignId, selectedAdGroup.AdgroupId, intReportDays);
            }
            else if (chkCreativeRptDtp.Checked)
            {
                lstRpt = adgroupHandler.DownLoadCreativeReport(user, selectedCampaign.CampaignId, selectedAdGroup.AdgroupId, dtpCreativeRptStartDay.Value.ToString("yyyy-MM-dd"), dtpCreativeRptEndDay.Value.ToString("yyyy-MM-dd"));
            }
            gridControlCreativeRpt.DataSource = lstRpt;
        }

        private void btnGetAllCreativeRpt_Click(object sender, EventArgs e)
        {
            gridControlCreativeRpt.DataSource = null;
            TopSession user = GetSession();
            int intReportDays = Convert.ToInt32(txtCreativeReportDays.Text.Trim());
            List<EntityCreativeReport> lstRpt = new List<EntityCreativeReport>();
            for (int i = 0; i < gridViewAdgroup.DataRowCount; i++)
            {
                ADGroup item = gridViewAdgroup.GetRow(gridViewAdgroup.GetRowHandle(i)) as ADGroup;
                if (item != null)
                {
                    if (chkCreativeRptRecentDays.Checked)
                    {
                        var adgroupRpt = adgroupHandler.DownLoadCreativeReport(user, selectedCampaign.CampaignId, item.AdgroupId, intReportDays);
                        lstRpt.AddRange(adgroupRpt);
                    }
                    else if (chkCreativeRptDtp.Checked)
                    {
                        var adgroupRpt = adgroupHandler.DownLoadCreativeReport(user, selectedCampaign.CampaignId, item.AdgroupId, dtpCreativeRptStartDay.Value.ToString("yyyy-MM-dd"), dtpCreativeRptEndDay.Value.ToString("yyyy-MM-dd"));
                        lstRpt.AddRange(adgroupRpt);
                    }
                }
            }

            gridControlCreativeRpt.DataSource = lstRpt;
        }


        private void 网页打开宝贝ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EntityAdgroupReport rpt = null;
            rpt = gridViewAdgroupRpt.GetFocusedRow() as EntityAdgroupReport;
            if (rpt == null)
            {
                rpt = gridViewCreativeRpt.GetFocusedRow() as EntityAdgroupReport;
            }

            long itemId = 0;
            List<ADGroup> lstAdgroup = gridControlAdgroup.DataSource as List<ADGroup>;
            if (lstAdgroup != null)
            {
                ADGroup adgroup = lstAdgroup.Find(o => o.AdgroupId == rpt.adgroup_id);
                if (adgroup != null)
                {
                    itemId = adgroup.NumIid;
                }
            }
            if (itemId == 0)
            {
                TopSession session = GetSession();
                List<long> lstAdgroupId = new List<long>();
                lstAdgroupId.Add(rpt.adgroup_id);
                var response = taobaoApiHandler.TaobaoSimbaAdgroupsByAdgroupIds(session, lstAdgroupId);
                if (!response.IsError)
                {
                    ADGroup adgroup = response.Adgroups.AdgroupList.Find(o => o.AdgroupId == rpt.adgroup_id);
                    itemId = adgroup.NumIid;
                }
            }
            string itemUrl = string.Format("http://item.taobao.com/item.htm?id={0}", itemId);
            System.Diagnostics.Process.Start("iexplore.exe", itemUrl);
        }

        private void btnGetSchedule_Click(object sender, EventArgs e)
        {
            txtSchedule.Text = string.Empty;
            TopSession session = GetSession();
            long campaignId = selectedCampaign.CampaignId;
            var response = taobaoApiHandler.TaobaoSimbaCampaignScheduleGet(session, campaignId);
            txtSchedule.Text = response.CampaignSchedule.Schedule;
        }

        private void btnGetArea_Click(object sender, EventArgs e)
        {
            txtArea.Text = string.Empty;
            TopSession session = GetSession();
            long campaignId = selectedCampaign.CampaignId;
            var response = taobaoApiHandler.TaobaoSimbaCampaignAreaGet(session, campaignId);
            txtArea.Text = response.CampaignArea.Area;
        }

        private void btnGetPlatform_Click(object sender, EventArgs e)
        {
            txtPlatform.Text = string.Empty;
            TopSession session = GetSession();
            long campaignId = selectedCampaign.CampaignId;
            var response = taobaoApiHandler.TaobaoSimbaCampaignPlatformGet(session, campaignId);
            txtPlatform.Text = response.Body;
        }

        private void btnInvestmentChange_Click(object sender, EventArgs e)
        {
            TopSession session = userHandler.GetUserSession(txtNickName.Text);
            EntityCampaign myCampaign = null;
            if (selectedCampaign.CampaignId == 0)
            {
                MessageBox.Show("请选择推广计划后，再使用");
                return;
            }
            else
            {
                myCampaign = campaignHandler.GetCampaign(session.UserID).Find(o => o.campaignid == selectedCampaign.CampaignId);
            }

            string paramTpl = "userId={0} configId={1}";
            string param = string.Empty;

            if (txtInvestmentChange.Text.StartsWith("+"))
            {//执行增加投入
                decimal investChange = Convert.ToDecimal(txtInvestmentChange.Text.Substring(1));
                decimal newMaxCPC = myCampaign.clickcost * (1 + investChange / 100);
                campaignHandler.SetBudget(session.UserID, myCampaign.campaignid, campaignHandler.GetCampaignBudget(session, selectedCampaign.CampaignId), newMaxCPC);
                param = string.Format(paramTpl, session.UserID, CommonHandler.Const_MajorizationConfig浮动加价);
            }
            else if (txtInvestmentChange.Text.StartsWith("-"))
            {//执行减少投入 
                decimal investChange = Convert.ToDecimal(txtInvestmentChange.Text.Substring(1));
                decimal newMaxCPC = myCampaign.clickcost * (1 - investChange / 100);
                campaignHandler.SetBudget(session.UserID, myCampaign.campaignid, campaignHandler.GetCampaignBudget(session, selectedCampaign.CampaignId), newMaxCPC);
                param = string.Format(paramTpl, session.UserID, CommonHandler.Const_MajorizationConfig浮动降价);
            }
            else if (CommonFunction.IsInt(txtInvestmentChange.Text))
            {//找词优化，执行策略id 
                param = string.Format(paramTpl, session.UserID, Convert.ToInt32(txtInvestmentChange.Text));
            }
            else if (txtInvestmentChange.Text.Length == 0)
            {//默认策略优化 
                param = string.Format("userId={0}", session.UserID);
            }
            else
            {
                MessageBox.Show("参数输入错误");
                return;
            }

            DialogResult dialogResult = MessageBox.Show(string.Format("是否执行优化命令：{0} {1}？", Config.MajorizationPath, param), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.OK)
            {
                System.Diagnostics.Process.Start("cmd.exe", string.Format("/k {0} {1} /a", Config.MajorizationPath, param));
            }
        }
    }
}