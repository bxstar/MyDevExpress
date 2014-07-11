﻿using System;
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
    public partial class FrmAPITest : DockContent
    {
        private static log4net.ILog logger = LogManager.GetLogger("loggerAX");
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
        /// 选中的推广计划ID
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
            //txtNickName.Text = Config.UserName;
            //txtSession.Text = Config.TopSessions;
            //txtNickName.Text = "tp_世奇广告";
            //txtNickName.Text = "365style";
            //txtSession.Text = "61019215373bc02fd52c6184b07e1166003464abfeaf0f01077912345";
            txtNickName.Text = "tp_世奇广告";
            txtSession.Text = "6102b26dab87ad6ec74c528f3b24efa26f113b6d024410a499456566";
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
            gridViewAdgroup.IndicatorWidth = gridViewKeyword.IndicatorWidth = gridViewKeywordRpt.IndicatorWidth = gridViewCampaignRpt.IndicatorWidth = 30;

            //显示行号
            gridViewUser.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewAdgroup.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewKeyword.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewKeywordRpt.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);
            gridViewCampaignRpt.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(gridViewCustomDrawRowIndicator);

            //排序完成后显示第一行
            gridViewUser.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewAdgroup.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewKeyword.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewKeywordRpt.EndSorting += new EventHandler(gridViewEndSorting);
            gridViewCampaignRpt.EndSorting += new EventHandler(gridViewEndSorting);
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
            EntityCampaign ec = campaignHandler.GetCampaign(session.UserID);
            List<EntityCampaign> lst = new List<EntityCampaign>();
            lst.Add(ec);
            dgvCampaign.DataSource = lst;
        }

        private void btnGetBudget_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            double result = GetCampaignBudget(session, selectedCampaign.CampaignId);
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
            string strCatMatchQscore = string.Empty;
            List<KeywordQscore> result = GetQscore(session, selectedAdGroup.AdgroupId, ref strCatMatchQscore);
            txtCatmatchQscore.Text = strCatMatchQscore;
            dgvQscore.DataSource = result;
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

            for (int i = 0; i < gridViewAdgroup.DataRowCount; i++)
            {
                ADGroup item = gridViewAdgroup.GetRow(gridViewAdgroup.GetRowHandle(i)) as ADGroup;
                if (item != null)
                {
                    List<EntityKeywordRpt> lstBase = null; List<EntityKeywordRpt> lstEffect = null;
                    if (chkKeywordRptRecentDays.Checked)
                    {
                        lstBase = keywordHandler.DownLoadKeywordBaseReport(session, item.CampaignId, item.AdgroupId, intReportDays);
                        lstEffect = keywordHandler.DownLoadKeywordEffectReport(session, item.CampaignId, item.AdgroupId, intReportDays);
                    }
                    else if (chkKeywordRptDtp.Checked)
                    {
                        lstBase = keywordHandler.DownLoadKeywordBaseReport(session, item.CampaignId, item.AdgroupId, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"));
                        lstEffect = keywordHandler.DownLoadKeywordEffectReport(session, item.CampaignId, item.AdgroupId, dtpKeywordRptStartDay.Value.ToString("yyyy-MM-dd"), dtpKeywordRptEndDay.Value.ToString("yyyy-MM-dd"));
                    }
                    if (lstBase != null)
                        lstAll.AddRange(lstBase);
                    if (lstEffect != null)
                        lstAll.AddRange(lstEffect);
                }
            }

            List<EntityKeywordRpt> data = (from a in lstAll
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

        /// <summary>
        /// 线上，获取一个计划的日限额
        /// </summary>
        private double GetCampaignBudget(TopSession session, long campaignId)
        {
            var response = taobaoApiHandler.TaobaoSimbaCampaignBudgetGet(session, campaignId);
            if (response != null && response.CampaignBudget != null)
            {
                return Convert.ToDouble(response.CampaignBudget.Budget);
            }
            else
            {
                // 默认设置为30元
                return 30;
            }
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

        /// <summary>
        /// 下载推广计划的基本报表，将淘宝的推广计划基础和效果数据下载或更新到数据库
        /// </summary>
        public string DownLoadCamapginBaseReport(TopSession session)
        {
            try
            {
                DateTime dtStart = DateTime.Now;
                // 下载推广计划的基本报表
                var report = taobaoApiHandler.TaobaoSimbaRptCampaignbaseGet(session, System.DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd"), System.DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd"));
                if (report.IsError)
                {
                    if (report.ErrMsg.Contains("ban") || report.SubErrMsg.Contains("ban"))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            System.Threading.Thread.Sleep(2000);
                            report = taobaoApiHandler.TaobaoSimbaRptCampaignbaseGet(session, System.DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd"), System.DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd"));
                            if (report.IsError && (report.ErrMsg.Contains("ban") || report.SubErrMsg.Contains("ban")) && dtStart.AddMinutes(5) > DateTime.Now)
                            {//超过5分钟放弃
                                isBanError = true;
                            }
                            else
                            {
                                if (dtStart.AddMinutes(5) <= DateTime.Now)
                                {
                                    logger.Error("线上下载推广计划的基本报表出错，已重试5分钟" + report.Body);
                                    return null;
                                }
                                isBanError = false;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return report.RptCampaignBaseList;
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/DownLoadCamapginBaseReport,淘宝API下载推广计划基础报表 失败：", ex);
                return null;
            }
        }

        /// <summary>
        /// 下载推广计划的效果报表，将淘宝的推广计划基础和效果数据下载或更新到数据库
        /// </summary>
        public string DownLoadCampaignEffectReport(TopSession session)
        {
            try
            {
                DateTime dtStart = DateTime.Now;
                // 下载推广组效果报表
                var report = taobaoApiHandler.TaobaoSimbaRptCampaigneffectGet(session, System.DateTime.Now.AddDays(0 - 30).Date.ToString("yyyy-MM-dd"), System.DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd"));
                if (report.IsError)
                {
                    if (report.ErrMsg.Contains("ban") || report.SubErrMsg.Contains("ban"))
                    {//遇到频繁访问的错误，需要多次访问
                        Boolean isBanError = true;
                        while (isBanError)
                        {
                            System.Threading.Thread.Sleep(2000);
                            report = taobaoApiHandler.TaobaoSimbaRptCampaigneffectGet(session, System.DateTime.Now.AddDays(0 - 30).Date.ToString("yyyy-MM-dd"), System.DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd"));
                            if (report.IsError && (report.ErrMsg.Contains("ban") || report.SubErrMsg.Contains("ban")) && dtStart.AddMinutes(5) > DateTime.Now)
                            {//超过5分钟放弃
                                isBanError = true;
                            }
                            else
                            {
                                if (dtStart.AddMinutes(5) <= DateTime.Now)
                                {
                                    logger.Error("线上下载推广计划的效果报表出错，已重试5分钟" + report.Body);
                                    return null;
                                }
                                isBanError = false;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return report.RptCampaignEffectList;
            }
            catch (Exception ex)
            {
                logger.Error("BusinessBatchHandler/DownLoadCampaignEffectReport,淘宝API下载推广组效果报表 失败：", ex);
                return null;
            }
        }

        public static int getPageCount(int totalNum, int pageSize)
        {
            // 每页显示的条数 = 0 的情况
            if (pageSize == 0)
            {
                return 0;
            }

            int nRemainder = totalNum % pageSize;
            if (nRemainder > 0)
            {
                return totalNum / pageSize + 1;
            }
            else
            {
                return totalNum / pageSize;
            }
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

        private void btnGetCampaignBaseRpt_Click(object sender, EventArgs e)
        {
            gridControlCampaignRpt.DataSource = null;
            TopSession session = GetSession();
            session.CampaignId = Convert.ToInt64(txtCampaignId.Text);
            string json = DownLoadCamapginBaseReport(session);
            List<adrptcampaignbase> lstRpt = TechNet.JsonDeserialize<List<adrptcampaignbase>>(json);
            gridControlCampaignRpt.DataSource = lstRpt;
        }

        private void btnGetCampaignEffectRpt_Click(object sender, EventArgs e)
        {
            gridControlCampaignRpt.DataSource = null;
            TopSession session = GetSession();
            session.CampaignId = Convert.ToInt64(txtCampaignId.Text);
            string json = DownLoadCampaignEffectReport(session);
            List<adrptcampaigneffect> lstRpt = TechNet.JsonDeserialize<List<adrptcampaigneffect>>(json);
            gridControlCampaignRpt.DataSource = lstRpt;
        }

        private void btnGetCampaignRpt_Click(object sender, EventArgs e)
        {
            gridControlCampaignRpt.DataSource = null;
            TopSession session = GetSession();
            session.CampaignId = Convert.ToInt64(txtCampaignId.Text);
            List<EntityCampaignReport> lstAll = new List<EntityCampaignReport>();
            string jsonBaseRpt = DownLoadCamapginBaseReport(session).ToLower();
            if (!string.IsNullOrEmpty(jsonBaseRpt) && jsonBaseRpt.Length > 2)
            {
                var arrBaseRpt = new DynamicJsonParser().FromJson(jsonBaseRpt);
                foreach (var item in arrBaseRpt)
                {
                    EntityCampaignReport rpt = new EntityCampaignReport();
                    rpt.date = item.date;
                    rpt.nick = item.nick;
                    rpt.impressions = item.impressions == null ? 0 : item.impressions;
                    rpt.click = item.click == null ? 0 : item.click;
                    rpt.cost = item.cost == null ? 0M : item.cost;
                    rpt.source = item.source;
                    rpt.avgpos = item.avgpos;
                    //if (rpt.source == 1)
                    //{
                    //    rpt.avgpos = item.avgpos == null ? 0 : item.avgpos;     //只保存站内的排名
                    //}
                    lstAll.Add(rpt);
                }
            }

            string jsonEffectRpt = DownLoadCampaignEffectReport(session).ToLower();
            if (!string.IsNullOrEmpty(jsonEffectRpt) && jsonEffectRpt.Length > 2)
            {
                var arrEffectRpt = new DynamicJsonParser().FromJson(jsonEffectRpt);
                foreach (var item in arrEffectRpt)
                {
                    EntityCampaignReport rpt = new EntityCampaignReport();
                    rpt.date = item.date;
                    rpt.nick = item.nick;
                    rpt.directpay = item.directpay == null ? 0M : item.directpay;
                    rpt.indirectpay = item.indirectpay == null ? 0M : item.indirectpay;
                    rpt.directpaycount = item.directpaycount == null ? 0 : item.directpaycount;
                    rpt.indirectpaycount = item.indirectpaycount == null ? 0 : item.indirectpaycount;
                    rpt.favitemcount = item.favitemcount == null ? 0 : item.favitemcount;
                    rpt.favshopcount = item.favshopcount == null ? 0 : item.favshopcount;
                    lstAll.Add(rpt);
                }
            }
            var lstCampaignRpt = (from a in lstAll
                                               group a by new { a.nick, a.date } into b
                                               select new 
                                               {
                                                   date = b.Key.date,
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
            gridControlCampaignRpt.DataSource = lstCampaignRpt;
            //gridControlCampaignRpt.DataSource = lstCampaignRpt.Select(o=>new {o.date,o.impressions,o.click,o.cost,o.ctr,o.cpc,o.avgpos,};
        }

        private void btnGetCategoryTop_Click(object sender, EventArgs e)
        {
            TopSession session = GetSession();
            string categoryIds = txtCategoryIds.Text;
            var result = taobaoApiHandler.TaobaoSimbaInsightCatstopwordGet(session, categoryIds);
            foreach (string word in result.TopWords)
            {
                richTextBox1.AppendText(word + "\n");
            }
        }

        public Boolean IsBanMsg(Top.Api.TopResponse response)
        {
            if (response.ErrMsg == null || response.SubErrMsg == null)
            {
                return false;
            }
            if (response.ErrMsg.Contains("Limited") || response.SubErrMsg.Contains("ban"))
            {
                return true;
            }
            else
            {
                return false;
            }
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
            lstUser = userHandler.GetUserInfo(null).Where(o => o.UserName == o.ProxyUserName).ToList();
            TopSession session = lstUser.Find(o => o.ProxyUserName == txtNickName.Text.Trim());
            if (session != null)
            {
                txtSession.Text = session.TopSessions;
            }
        }

        private void btnUpdateKeywordRpt_Click(object sender, EventArgs e)
        {//由于淘宝关键词报表的延迟性，需要经常手动更新转化数据
            int days = Convert.ToInt32(numKeywordRptDays.Value);
            Boolean isNeedUpdate = chkNeedUpdateKeywordRpt.Checked;

            int validUserCount = 0;
            foreach (var user in lstUser.Where(o => o.IsEnableMajorization))
            {
                EntityCampaign ec = campaignHandler.GetCampaign(user.UserID);

                if (ec.campaignid > 0 && ec.dailylimit != 0M && ec.clickcost != 0M)
                {
                    frmOutPut.OutPutMsg(string.Format("用户{0},开始更新关键词报表", user.UserName));
                }
                else
                {
                    if (ec.campaignid > 0)
                    {
                        frmOutPut.OutPutMsg(string.Format("用户{0},ID{1},未设置日限额或单次点击限价", user.UserName, user.UserID));
                    }
                    else
                    {
                        frmOutPut.OutPutMsg(string.Format("用户{0},ID{1},未选择计划", user.UserName, user.UserID));
                    }
                    continue;
                }

                // 线上，获取计划信息
                List<Campaign> onlineCampaign = campaignHandler.GetCampaignOnline(user);
                // 判断从线上获取的计划是否存在
                if (onlineCampaign == null || onlineCampaign.Count == 0)
                {
                    string strGetCampaignExMsg = "用户" + user.UserName + ",从淘宝API线上获取计划失败，没有得到返回的计划,可能是用户的Session过期...";
                    frmOutPut.OutPutMsg(strGetCampaignExMsg);
                    continue;
                }
                // 判断数据库中的计划在线上是否存在
                Campaign campaign = onlineCampaign.Find(o => o.CampaignId == ec.campaignid);
                if (campaign == null)
                {
                    string strOnlineCampaignFindExMsg = "用户" + user.UserName + ",从淘宝API线上获取到的计划与数据库中的计划ID没有相同的ID,请检查前端保存是否有错误...";
                    frmOutPut.OutPutMsg(strOnlineCampaignFindExMsg);
                    continue;
                }

                // 用户自己主动暂停计划
                if (campaign.OnlineStatus == "offline")
                {
                    string strOfflineExMsg = "用户" + user.UserName + ",暂停在该计划下的推广...";
                    frmOutPut.OutPutMsg(strOfflineExMsg);
                    continue;
                }
                else
                {
                    // 用户被迫下线
                    if (campaign.SettleStatus == "offline")
                    {
                        string message = "";
                        if (campaign.SettleReason == "1")
                        {// 余额不足下线
                            message = "用户" + user.UserName + ",余额不足,已经下线...";
                        }
                        else if (campaign.SettleReason == "2")
                        {// 超过日限额下线
                            message = "用户" + user.UserName + ",超过日限额,已经下线...";
                        }
                        else
                        {
                            message = "用户" + user.UserName + ",余额不足,超过日限额,已经下线...";
                        }
                        frmOutPut.OutPutMsg(message);
                        continue;
                    }
                }

                // 线上，获取该计划下的所有推广组
                List<ADGroup> lstAdgroupFromTaobao = adgroupHandler.GetAdgroupOnline(user, campaign.CampaignId);
                if (lstAdgroupFromTaobao.Count == 0)
                {
                    string strGetAdAdgroupExMsgTaobao = "用户" + user.UserName + ",直通车后台,该计划下没有找到推广组...";
                    frmOutPut.OutPutMsg(strGetAdAdgroupExMsgTaobao);
                    continue;
                }
                // 数据库，获取推广组信息
                List<EntityAdgroup> lstAdgroupFromDB;
                lstAdgroupFromDB = adgroupHandler.GetAdAdgroup(user.UserID, Convert.ToInt64(campaign.CampaignId)).Where(o => o.IsEnableMajorization).ToList();

                if (lstAdgroupFromDB.Count == 0)
                {
                    string strGetAdAdgroupExMsgDB = "用户" + user.UserName + ",没有在全自动推广计划下选择宝贝进行推广...";
                    frmOutPut.OutPutMsg(strGetAdAdgroupExMsgDB);
                    continue;
                }
                Boolean IsAdgroupExist = true;  //数据库中推广组是否在线上存在
                List<ADGroup> lstAdgroupNeedMajor = new List<ADGroup>();    //需要优化的推广组，这部分推广组在数据库中存在，且也在线上（线上有，而数据库中没有的推广组不进行优化）
                // 对比线上的推广组和服务器的推广组，不一致告警
                foreach (EntityAdgroup item in lstAdgroupFromDB)
                {
                    ADGroup adgroupExist = lstAdgroupFromTaobao.Find(o => o.AdgroupId == item.adgroupid && o.CampaignId == item.campaignid);
                    if (adgroupExist == null)
                    {
                        string strExMsg = "用户" + user.UserName + string.Format(",服务器上的推广组{0}在直通车后台不存在", item.adgroupid);
                        frmOutPut.OutPutMsg(strExMsg);
                        //删除服务器上的推广组，但最好告知用户不要在直通车后台删除推广组
                        //adgroupHandler.DeleteAdgroup(item.adgroupid);
                        IsAdgroupExist = false;
                    }
                    else
                    {
                        if (adgroupExist.OfflineType != item.offlinetype || adgroupExist.OnlineStatus != item.onlinestatus)
                        {//将线上的推广组状态同步到本地
                            adgroupHandler.UpdateAdgroupStatus(adgroupExist.AdgroupId, adgroupExist.OnlineStatus, adgroupExist.OfflineType);
                        }
                        //为了核对报表的一致性，线上暂停的推广组也下载更新其报告，但真实生产环境不这样做，真实生产环境如果用户将其设置推广中状态会继续下载其报告
                        lstAdgroupNeedMajor.Add(adgroupExist);
                    }
                }
                if (IsAdgroupExist)
                {
                    frmOutPut.OutPutMsg(string.Format("用户{0},ID{1},推广组数据验证成功,需要优化推广组数{2}", user.UserName, user.UserID, lstAdgroupNeedMajor.Count));
                }
                else
                {
                    frmOutPut.OutPutMsg(string.Format("用户{0},ID{1},推广组数据验证失败,已删除相应的推广组,以便继续优化,需要优化推广组数{2}", user.UserName, user.UserID, lstAdgroupNeedMajor.Count));
                }

                validUserCount++;       //有效用户数


                foreach (ADGroup adgroupItem in lstAdgroupNeedMajor)
                {
                    Boolean isDifferent = false;
                    // 下载推广组的基础报告
                    Boolean resultDownBaseRpt = batchHandler.DownLoadKeywordBaseReport(user, adgroupItem.CampaignId, adgroupItem.AdgroupId, days, isNeedUpdate, ref isDifferent);
                    int retryCount = 1;//重试次数，重试原因：这里不是防止ban，而是一些淘宝莫名其妙的异常比如Jayrock.Json.JsonException: The text '.' has the incorrect syntax for a number
                    while (!resultDownBaseRpt && retryCount < 4)
                    {
                        System.Threading.Thread.Sleep(2000);
                        frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载基础报告数据出错,重试{2}", user.UserName, adgroupItem.AdgroupId, retryCount++));
                        resultDownBaseRpt = batchHandler.DownLoadKeywordBaseReport(user, adgroupItem.CampaignId, adgroupItem.AdgroupId, days, isNeedUpdate, ref isDifferent);
                    }
                    if (resultDownBaseRpt)
                    {
                        if (!isDifferent)
                        {
                            frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载基础报告数据成功", user.UserName, adgroupItem.AdgroupId));
                        }
                        else
                        {
                            frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载基础报告数据数据发生变化，需要更新", user.UserName, adgroupItem.AdgroupId));
                        }
                    }
                    else
                        frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载基础报告数据失败", user.UserName, adgroupItem.AdgroupId));

                    isDifferent = false;
                    // 下载推广组的效果报告
                    Boolean resultDownEffectRpt = batchHandler.DownLoadKeywordEffectReport(user, adgroupItem.CampaignId, adgroupItem.AdgroupId, days, isNeedUpdate, ref isDifferent);
                    retryCount = 1;
                    while (!resultDownEffectRpt && retryCount < 4)
                    {
                        System.Threading.Thread.Sleep(2000);
                        frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载效果报告数据出错,重试{2}", user.UserName, adgroupItem.AdgroupId, retryCount++));
                        resultDownEffectRpt = batchHandler.DownLoadKeywordEffectReport(user, adgroupItem.CampaignId, adgroupItem.AdgroupId, days, isNeedUpdate, ref isDifferent);
                    }
                    if (resultDownEffectRpt)
                    {
                        if (!isDifferent)
                        {
                            frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载效果报告数据成功", user.UserName, adgroupItem.AdgroupId));
                        }
                        else
                        {
                            frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载效果报告数据数据发生变化，需要更新", user.UserName, adgroupItem.AdgroupId));
                        }
                    }
                    else
                        frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},下载效果报告数据失败", user.UserName, adgroupItem.AdgroupId));
                    // 插入下载状态，表示推广组已经成功下载完成
                    if (resultDownBaseRpt && resultDownEffectRpt)
                    {
                        frmOutPut.OutPutMsg(string.Format("用户{0},推广组:{1},成功更新关键词报表", user.UserName, adgroupItem.AdgroupId));
                    }
                }
            }

            frmOutPut.OutPutMsg(string.Format("有效用户数{0}", validUserCount));
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

            //int needUpdateCount = 0;
            ////批量修改计划，将淘快词名称去掉
            //var lst = lstUser.Where(o => o.CampaignName != null && o.CampaignName.StartsWith("淘快词"));
            //foreach (var session in lst)
            //{
            //    SimbaCampaignsGetResponse campaignsRes = TaobaoApiHandler.TaobaoSimbaCampaignsGet(session);
            //    if (campaignsRes.IsError)
            //    {
            //        frmOutPut.OutPutMsg(string.Format("用户{0}，ID{1} 获取计划发生错误，", session.ProxyUserName, session.UserID, campaignsRes.Body));
            //        continue;
            //    }
            //    Campaign campaignOnline = campaignsRes.Campaigns.Find(o => o.OnlineStatus == "online" && o.CampaignId == session.CampaignId && o.Title.StartsWith("淘快词"));
            //    if (campaignOnline != null)
            //    {
            //        needUpdateCount++;
            //    }
            //    //var response = TaobaoApiHandler.TaobaoSimbaCampaignUpdate(session, session.CampaignId, session.CampaignName.Replace("淘快词", ""), "online");
            //    //if (response.IsError)
            //    //{
            //    //    frmOutPut.OutPutMsg(string.Format("用户{0}，ID{1} 更改计划发生错误，", session.ProxyUserName, session.UserID, response.Body));
            //    //}
            //}

            //frmOutPut.OutPutMsg(needUpdateCount.ToString());
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            dgvCampaign.DataSource = null;
            TopSession session = GetSession();

            //string msg = string.Empty;
            //List<long> lstCampaignId = new List<long>();
            //lstCampaignId.Add(Convert.ToInt64(txtCampaignId.Text));
            //List<EntityCampaignReport> lstRpt =campaignHandler.GetCampaignRpt(session, lstCampaignId, 60, ref msg);

            //DateTime dtStart = new DateTime(2013, 10, 20);
            //DateTime dtEnd = new DateTime(2013, 10, 27);

            //var response = TaobaoApiHandler.TaobaoSimbaRptCustbaseGet(session, dtStart, dtEnd, 500, 1);
            //var response = TaobaoApiHandler.TaobaoSimbaRptCusteffectGet(session, dtStart, dtEnd, 500, 1);
            string str = string.Empty;
            List<EntityCampaignReport> lstReport = reportHandler.GetAllCampaignRpt(session, 30, ref str).OrderByDescending(o => o.date).ToList();
            dgvCampaign.DataSource = new SortableBindingList<EntityCampaignReport>(lstReport);

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
            TopSession session = userHandler.GetUserSession(txtNickName.Text.Trim());

            long campaign_id_online = Convert.ToInt64(txtCampaignId.Text);

            long campaign_id_local = campaignHandler.GetCampaign(session.UserID).campaignid;
            if (campaign_id_local != campaign_id_online)
            {
                MessageBox.Show("数据库中没有此计划，请重新选择需要同步推广组的计划");
                return;
            }

            List<ADGroup> lstAdgroup = adgroupHandler.GetAdgroupOnline(session, campaign_id_online);
            frmOutPut.OutPutMsg(string.Format("用户{0},计划:{1},从线上下载推广组成功，推广组数{2}，其中状态为推广中：{3}", session.UserID, campaign_id_online, lstAdgroup.Count, lstAdgroup.Where(o => o.OnlineStatus == "online").Count()));
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
            Keyword k = gridViewKeyword.GetFocusedRow() as Keyword;
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
        {
            if (cbxApp.SelectedIndex == 0)
            {//安心代驾
                Config.AppKey = "21596194";
                Config.AppSecret = "5b6e8a9a5e1b600cc348445a24a7c846";
                Config.App_Title = "安心代驾数据分析";
                Config.ConnectionAP = "packet size=4096;user id=sa; PWD ='ShiQiTaoKuaiChe20120302'; data source=.;persist security info=False;initial catalog='AutomaticDrive_Mobile';Connect Timeout=30000;Asynchronous Processing=true;";
                taobaoApiHandler.SetTopClient(Config.C_Url, Config.AppKey, Config.AppSecret, "json");
                txtArticleCode.Text = Config.ArticleCode = "FW_GOODS-1897024";
                Config.ItemCode = "FW_GOODS-1897024-v2";
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
            }
            else if (cbxApp.SelectedIndex == 2)
            {//淘快车 
                Config.AppKey = "12209394";
                Config.AppSecret = "98fbb138ef7597283d798a42cb203103";
                Config.App_Title = "淘快车数据分析";
                taobaoApiHandler.SetTopClient(Config.C_Url, Config.AppKey, Config.AppSecret, "json");
                txtArticleCode.Text = Config.ArticleCode = "ts-21434";
                Config.ItemCode = string.Empty;
            }
            frmMain.SetMainTitle(Config.App_Title);
        }

        private void gridViewEndSorting(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gv = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            gv.FocusedRowHandle = 0;
        }

        private void gridViewCustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator)
            {
                e.Info.DisplayText = Convert.ToString(e.RowHandle + 1);
            }
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
                string[] separator = new string[] { "\t" };
                string[] source = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (source.Count<string>() == 2)
                {
                    this.txtNickName.Text = source[0];
                    this.txtSession.Text = source[1];
                }
            }

        }

        private void btnCheckAddPrice_Click(object sender, EventArgs e)
        {
            dgvCampaign.DataSource = null;
            TopSession user = userHandler.GetUserSession(txtNickName.Text.Trim());
            EntityCampaign ec = campaignHandler.GetCampaign(user.UserID);
            List<EntityCampaign> lst = new List<EntityCampaign>();
            lst.Add(ec);
            dgvCampaign.DataSource = lst;
            txtCampaignId.Text = ec.campaignid.ToString();

            double budget = GetCampaignBudget(user, ec.campaignid);
            txtBudget.Text = budget.ToString();
            Boolean result = IsNeedAddPrice(user, ec.campaignid, budget);
            if (result)
            {
                
            }
            else
            {
                MessageBox.Show("不用加价");
            }
        }

        /// <summary>
        /// 根据最近3天的花费对比日限额，判断是否需要提价
        /// </summary>
        private Boolean IsNeedAddPrice(TopSession session, long campaignId, double budget)
        {
            decimal dailyLimit = budget > 3000 ? 30M : Convert.ToDecimal(budget);       //超过3000日限额的，包括未设置日限额的，统统算作3000
            Boolean resultAddPrice = false;
            List<EntityCampaignReport> lstReport = new List<EntityCampaignReport>();
            try
            {
                // 下载推广计划的基本报表
                var report = taobaoApiHandler.TaobaoSimbaRptCampaignbaseGet(session, campaignId, DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));

                if (report == null || String.IsNullOrEmpty(report.RptCampaignBaseList) || report.RptCampaignBaseList == "[]" || report.RptCampaignBaseList == "{}")
                {
                    return false;
                }
                // 解析推广组json数据
                NetServ.Net.Json.JsonArray data;
                // 解析推广组
                using (JsonParser parser = new JsonParser(new StringReader(report.RptCampaignBaseList), true))
                {
                    data = parser.ParseArray();
                }
                foreach (JsonObject service in data)
                {
                    EntityCampaignReport campaignReport = new EntityCampaignReport();
                    campaignReport.date = CommonFunction.JsonObjectToString(service["date"]);
                    campaignReport.impressions = (CommonFunction.JsonObjectToInt(service["impressions"])) + campaignReport.impressions;
                    campaignReport.cost = (CommonFunction.JsonObjectToDecimal(service["cost"]) / 100.0M) + campaignReport.cost;
                    lstReport.Add(campaignReport);
                }

                if (lstReport.Where(o => o.impressions > 0).Count() == 0)
                {//展现全为0的，数据有问题
                    return false;
                }
                Boolean isRecentlyCostLower = false;    //是否最近花费都小于日限额的70%
                if (lstReport.Max(o => o.cost) <= (dailyLimit * 0.7M))
                {//最近的花费都小于日限额的70%
                    isRecentlyCostLower = true;
                }
                else
                {//不满足直接返回
                    return false;
                }
                EntityMajorConfig addPriceConfig = CommonHandler.GetMajorConfig(CommonHandler.Const_MajorizationConfig浮动加价);
                Boolean isAddPriceLessFive = false;     //15天内是否加价策略连续执行少于5次
                List<EntityMajorizationAdgroupRecord> lstRecord = campaignHandler.GetMajorizationAdgroup(campaignId).Where
                    (o => o.create_date > DateTime.Now.AddDays(-15) && o.config_id == addPriceConfig.LocalID).ToList();
                if (lstRecord.GroupBy(o => o.create_date.ToString("yyyy-MM-dd")).Count() < 5)
                {
                    isAddPriceLessFive = true;
                }
                else
                {
                    isAddPriceLessFive = false;
                }

                if (isRecentlyCostLower && isAddPriceLessFive)
                {
                    resultAddPrice = true;
                    decimal recentlyAvgCost = lstReport.Where(o => o.impressions > 0).Average(o => o.cost);     //最近的平均花费
                    decimal addPriceRate = (dailyLimit - recentlyAvgCost) / dailyLimit;

                    string str = string.Format("日限额{0}，平均花费{1}，加价幅度{2}", dailyLimit, recentlyAvgCost, addPriceRate);
                    MessageBox.Show(str);
                }

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("用户{0},ID{1},判断是否需要提价失败：", session.ProxyUserName, session.UserID), ex);
                return false;
            }
            return resultAddPrice;
        }
    }
}