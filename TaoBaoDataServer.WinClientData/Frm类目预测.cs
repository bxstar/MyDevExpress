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
    public partial class Frm类目预测 : Form
    {
        BusinessTaobaoApiHandler TaobaoApiHandler = new BusinessTaobaoApiHandler();
        BusinessKeywordHandler keywordHandler = new BusinessKeywordHandler();

        /// <summary>
        /// 用户登录信息
        /// </summary>
        TopSession session = new TopSession();

        public Frm类目预测()
        {
            InitializeComponent();
        }

        private void Frm类目预测_Load(object sender, EventArgs e)
        {
            session.UserName = Config.UserName;
            session.TopSessions = Config.TopSessions;
        }

        private void btnBatchGet_Click(object sender, EventArgs e)
        {
            long categoryId = 50016819;
            List<string> lstKeyword = richTextBox1.Text.Split('\n').ToList();
            List<KeywordAndCategory> lstResult = keywordHandler.ForecastCategory(session, lstKeyword, categoryId);

        }

        private void btnOk_Click(object sender, EventArgs e)
        {


            
        }

    }
}
