using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// 日志输出接口
    /// </summary>
    public interface IOutPut
    {
        void OutPutMsg(string strMsg, List<string> lstContent);
        void OutPutMsg(string strMsg);

        /// <summary>
        /// 带参数输出
        /// </summary>
        /// <param name="strMsg">内容</param>
        /// <param name="args">参数</param>
        void OutPutMsgFormat(string strMsg, params object[] args);
    }

    /// <summary>
    /// 日志输出实现类
    /// </summary>
    public partial class FrmOutPut : WeifenLuo.WinFormsUI.Docking.DockContent,IOutPut
    {
        public FrmOutPut()
        {
            InitializeComponent();
        }

        private void FrmOutPut_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Listeners.Add(new MyTraceListener(this.rtxOutPut));
        }


        public void OutPutMsg(string strMsg)
        {
            Debug.WriteLine(strMsg);
        }

        public void OutPutMsgFormat(string strMsg, params object[] args)
        {
            Debug.WriteLine(string.Format(strMsg, args));
        }

        public void OutPutMsg(string strMsg, List<string> lstContent)
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

        private void 清除内容ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.rtxOutPut.Clear();
        }

    }
}
