using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using iclickpro.AccessCommon;
using WeifenLuo.WinFormsUI.Docking;

namespace TaoBaoDataServer.WinClientData
{
    /// <summary>
    /// 日志滚动输出，多线程实现，不用Application.DoEvents
    /// </summary>
    public partial class Form1 : DockContent
    {
        Thread Log;

        public Form1()
        {
            InitializeComponent();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            int pageIndex = 3;

            List<int> lstTest = new List<int>();
            for (int i = 0; i < 133; i++)
            {
                lstTest.Add(i);
            }
            var lst = lstTest.GetRange(0, 100);

            
            int pageSize = 50;
            int pageCount = 0;
            if (lstTest.Count >= ((pageIndex - 1) * pageSize + pageSize))
            {
                pageCount = pageSize;
            }
            else
            {
                pageCount = lstTest.Count - (pageIndex - 1) * pageSize;
            }

            lst = lstTest.GetRange((pageIndex - 1) * pageSize, pageCount);


            if (Log != null && Log.IsAlive)
            {
                Log.Abort();
                Log.Join();
                Log = null;
            }
            Log = new Thread(new ParameterizedThreadStart(OutPutLog.GetNumber));
            Log.Start(this);
            Log.IsBackground = true;

        }

        public void MsgAppend(string strMsg, Color c)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                int selectStart = richTextBox1.TextLength;
                richTextBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + strMsg + Environment.NewLine);
                richTextBox1.Select(selectStart, richTextBox1.TextLength - 1);
                richTextBox1.SelectionColor = c;
                richTextBox1.ScrollToCaret();
                
            }));
        }

        public class OutPutLog
        {
            /// <summary>
            /// 循环
            /// </summary>
            public static void GetNumber(object para)
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("运行了" + i + "次");
                    //消息写入richTextbox控件里
                    Form1 f = para as Form1;
                    f.MsgAppend(i.ToString(), Color.Red);
                    Thread.Sleep(200);
                    //Application.DoEvents();
                }
            }
        }

        private void btnMd5Encrypt_Click(object sender, EventArgs e)
        {
            txtMd5Encrypt.Text = CommonFunction.Md5Encrypt(txtPwd.Text.Trim());
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string strOrign = txtOrign.Text.Trim();
            txtEncrypt.Text = CryptHelper.Encrypt(strOrign);
            txtDecrypt.Clear();
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            txtDecrypt.Text = CryptHelper.Decrypt(txtEncrypt.Text);
        }
    }
}
