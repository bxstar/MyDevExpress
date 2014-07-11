using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaoBaoDataServer.WinClientData.Model;
using TaoBaoDataServer.WinClientData.BusinessLayer;

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmSetKeywordCustom : Form
    {
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();
        List<EntityKeywordCustom> lstUserBlackKeyword;
        List<EntityKeywordCustom> lstKeywordCustom;

        public long adgroupId;
        public TopSession session;


        public FrmSetKeywordCustom()
        {
            InitializeComponent();
        }

        private void FrmSetKeywordCustom_Load(object sender, EventArgs e)
        {
            cmbWhiteMatchScope.SelectedIndex = 0;
            cmbBlackMatchScope.SelectedIndex = 0;
            cmbBlackLevel.SelectedIndex = 2;
            lstKeywordCustom = keywordHandler.GetKeywordCustom(adgroupId, false);

            //获取用户和全局级别黑名单
            lstUserBlackKeyword = keywordHandler.GetKeywordBlackList(session.UserID);

            //白名单
            dgvWhite.AutoGenerateColumns = false;
            dgvWhite.DataSource = lstKeywordCustom.Where(o => o.custom_type == TypeKeywordCustomType.WhiteList).ToList();
            //黑名单
            dgvBlack.AutoGenerateColumns = false;

            dgvBlack.DataSource = GetUserBlackKeyword(lstKeywordCustom.Where(o => o.custom_type == TypeKeywordCustomType.BlackList).ToList(), lstUserBlackKeyword);
        }


        private void btnWhiteSet_Click(object sender, EventArgs e)
        {
            string result = keywordHandler.SaveKeywordCustomLocalAndOnline(session, lstKeywordCustom.Where(o => o.custom_type == TypeKeywordCustomType.WhiteList).ToList(), session.CampaignId, adgroupId, TypeKeywordCustomType.WhiteList);
            if (result.Length == 0)
            {
                MessageBox.Show("白名单保存成功！");
            }
            else
            {
                MessageBox.Show("白名单保存失败，原因：" + result);
            }
        }

        private void btnWhiteDel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvWhite.SelectedRows.Count; i++)
            {
                EntityKeywordCustom entity = dgvWhite.SelectedRows[i].DataBoundItem as EntityKeywordCustom;
                lstKeywordCustom.Remove(entity);
            }
            dgvWhite.DataSource = lstKeywordCustom.Where(o => o.custom_type == TypeKeywordCustomType.WhiteList).ToList();
        }

        private void btnWhiteAdd_Click(object sender, EventArgs e)
        {
            if (lstKeywordCustom.Where(o => o.keyword == txtWhiteKeyword.Text.Trim()).Count() > 0)
            {
                MessageBox.Show("白名单中不能添加重复的词");
            }

            EntityKeywordCustom entity = new EntityKeywordCustom();
            entity.keyword = txtWhiteKeyword.Text.Trim();
            entity.max_price = Convert.ToInt64(txtWhiteMaxPrice.Text.Trim());
            if (cmbWhiteMatchScope.Text == "广泛匹配")
            {
                entity.match_scope = "4";
            }
            else if (cmbWhiteMatchScope.Text == "中心词匹配")
            {
                entity.match_scope = "2";
            }
            else if (cmbWhiteMatchScope.Text == "精确匹配")
            {
                entity.match_scope = "1";
            }
            entity.find_source = txtWhiteFindSource.Text.Trim();
            entity.remark = txtWhiteRemark.Text.Trim();
            entity.custom_type = TypeKeywordCustomType.WhiteList;
            entity.user_id = session.UserID;
            entity.adgroup_id = adgroupId;

            lstKeywordCustom.Add(entity);

            dgvWhite.DataSource = null;
            dgvWhite.DataSource = lstKeywordCustom.Where(o => o.custom_type == TypeKeywordCustomType.WhiteList).ToList();

            SelectRowInDgv(dgvWhite, entity.keyword);
        }

        private void btnBlackSet_Click(object sender, EventArgs e)
        {
            string result = keywordHandler.SaveKeywordCustomLocalAndOnline(session, lstKeywordCustom.Where(o => o.custom_type == TypeKeywordCustomType.BlackList).ToList(), session.CampaignId, adgroupId, TypeKeywordCustomType.BlackList);
            if (result.Length == 0)
            {
                MessageBox.Show("黑名单保存成功！");
            }
            else
            {
                MessageBox.Show("黑名单保存失败，原因：" + result);
            }
        }

        private void btnBlackDel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvBlack.SelectedRows.Count; i++)
            {
                EntityKeywordCustom entity = dgvBlack.SelectedRows[i].DataBoundItem as EntityKeywordCustom;
                if (entity.user_id == 0)
                {
                    MessageBox.Show("全局黑名单不能删除");
                }
                else
                {
                    lstKeywordCustom.Remove(entity);
                }
            }
            dgvBlack.DataSource = GetUserBlackKeyword(lstKeywordCustom.Where(o => o.custom_type == TypeKeywordCustomType.BlackList).ToList(), lstUserBlackKeyword);
        }

        private void btnBlackAdd_Click(object sender, EventArgs e)
        {
            if (lstKeywordCustom.Where(o => o.keyword == txtBlackKeyword.Text.Trim()).Count() > 0)
            {
                MessageBox.Show("黑名单中不能添加重复的词");
            }

            EntityKeywordCustom entity = new EntityKeywordCustom();
            entity.keyword = txtBlackKeyword.Text.Trim();
            if (cmbBlackMatchScope.Text == "模糊匹配")
            {
                entity.match_scope = "4";
            }
            else if (cmbBlackMatchScope.Text == "精确匹配")
            {
                entity.match_scope = "1";
            }
            entity.find_source = txtBlackFindSource.Text.Trim();
            entity.remark = txtBlackRemark.Text.Trim();
            entity.custom_type = TypeKeywordCustomType.BlackList;
            if (cmbBlackLevel.Text == "全局黑名单")
            {
                entity.user_id = 0;
                entity.adgroup_id = 0;
            }
            else if (cmbBlackLevel.Text == "用户黑名单")
            {
                entity.user_id = session.UserID;
                entity.adgroup_id = 0;
            }
            else if (cmbBlackLevel.Text == "推广组黑名单")
            {
                entity.user_id = session.UserID;
                entity.adgroup_id = adgroupId;
            }

            lstKeywordCustom.Add(entity);

            dgvBlack.DataSource = null;
            dgvBlack.DataSource = GetUserBlackKeyword(lstKeywordCustom.Where(o => o.custom_type == TypeKeywordCustomType.BlackList).ToList(), lstUserBlackKeyword);

            SelectRowInDgv(dgvBlack, entity.keyword);
        }

        /// <summary>
        /// 根据关键词选中行
        /// </summary>
        private void SelectRowInDgv(DataGridView dgv, string keyword)
        {
            dgv.ClearSelection();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dynamic entity = dgv.Rows[i].DataBoundItem as dynamic;
                if (entity.keyword == keyword)
                {
                    dgv.Rows[i].Selected = true;
                }
            }
        }

        private List<EntityKeywordCustom> GetUserBlackKeyword(List<EntityKeywordCustom> l1, List<EntityKeywordCustom> l2)
        {
            List<EntityKeywordCustom> all = new List<EntityKeywordCustom>();
            all.AddRange(l1);
            all.AddRange(l2);
            return all;
        }
    }
}