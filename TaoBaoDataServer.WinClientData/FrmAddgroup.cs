using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iclickpro.AccessCommon;
using TaoBaoDataServer.WinClientData.Model;

namespace TaoBaoDataServer.WinClientData
{
    public partial class FrmAddgroup : Form
    {
        IOutPut frmOutPut;
        TopSession user;
        BusinessLayer.BusinessTaobaoApiHandler bllTaoBao = new BusinessLayer.BusinessTaobaoApiHandler();

        public FrmAddgroup(IOutPut _frmOutPut,TopSession _user)
        {
            InitializeComponent();
            frmOutPut = _frmOutPut;
            user = _user;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            long itemID = Strings.GetItemId(txtItemID.Text);
            var response = bllTaoBao.TaobaoSimbaAdgroupAdd(user, user.CampaignId, itemID, Convert.ToInt64(txtDefaultPrice.Text), txtItemTitle.Text, txtImgUrl.Text);

            if (response != null && response.IsError)
            {
                frmOutPut.OutPutMsgFormat("添加推广组失败:{0}", response.Body);
            }

            
            if (response != null && !response.IsError)
            {
                MessageBox.Show("添加推广组成功");
            }
            
            
        }
    }
}
